using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using OpenCvSharp;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using GxIAPINET;
using Spire.Barcode;
using System.Drawing;
using ZLG;

namespace BloodCardChecker
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

   
    public partial class MainWindow : System.Windows.Window
    {
        public static ZLGCanComm zlg_can = new ZLGCanComm();
        public static Tools tools = new Tools();
        public static FM316 codesancer = new FM316();
        public static bool isrealtime = false;
        public static bool checkonece = false;
        public static bool iscollect = false;
        public Stopwatch timeused = new Stopwatch();
        public Mat []imgls = new Mat[2];
        ~MainWindow()
        {
            codesancer.Close();
        }
        public MainWindow()
        {
            InitializeComponent();
            zlg_can.Connect();
            if (tools.Init() == false) return;
            Tools.camera[0].TakeImage();
            Tools.camera[1].TakeImage();
            camera1.Source = Tools.camera[0].GetImageSoure();
            camera2.Source = Tools.camera[1].GetImageSoure();
            imgls[0] = new Mat();
            imgls[1] = new Mat();
            //实时
            DispatcherTimer timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(500);
            timer1.Tick += UpData_RealTime;
            timer1.Start();
            //实时
            DispatcherTimer timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(100);
            timer2.Tick += Time_Collect;
            timer2.Start();
        }
        public bool last_collect_go = false;
        private void Time_Collect(object sender, EventArgs e)
        {
            if(iscollect)
            {
                var img = Tools.camera[0].TakeImage(Tools.booldcardinfo[0].bloodParameter.onedcode_s3);
                var imgs = Tools.MyMatchTemplate(img,new OpenCvSharp.Rect(0, 0, img.Width, img.Height), "template.jpg", 0.5f, Tools.booldcardinfo[0].bloodParameter.tube_y);
                bool is_collect_go = imgs.Count != 0;
                if (last_collect_go != is_collect_go && is_collect_go)
                {
                    //GoTest();
                    Thread.Sleep(1000);
                    var uiinfo = tools.Explain(0);
                    ok_count = 4;
                    for (int i = 0; i < imgs.Count; i++)
                    {
                        var tube = Tools.booldcardinfo[0].tobjs[i];
                        string filename = "datas/good/" + GetSysTimeStr() + ".jpg";
                        if (tube.foreigs.Count != 0) filename = "datas/ng/" + GetSysTimeStr() + ".jpg";
                        imgs[i].ImWrite(filename);
                    }
                }
                last_collect_go = is_collect_go;

            }
        }
        private void UpData_RealTime(object sender, EventArgs e)
        {
            if (isrealtime)
            {
                if (zlg_can.IsTakeImg() && ok_count == 0)
                {
                    zlg_can.SetRegister("03-ff01", 1, 0b111);
                    GoTest();
                }
            }
            if (ok_count == 4)
            {
                System.Windows.Controls.Image[] camers = { camera1, camera2 };
                BloodCardShow[] bloodshow = { bloodshow1, bloodshow2 };
                for (int i = 0; i < 2; i++)
                {
                    var uiinfo = Tools.booldcardinfo[i];
                    bloodshow[i].UpDataUI(uiinfo);
                    camers[i].Source = Tools.OpenCvImgToImageSoure(uiinfo.img);
                }
                ok_count = 0;
            }
        }

        public string SaveJpg(BitmapSource bitmap,string filename)
        {
            filename = "check/"+ filename + ".jpg";
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)bitmap));
            using (FileStream stream = new FileStream(filename, FileMode.Create))
                encoder.Save(stream);
            return filename;
        }

        public string GetSysTimeStr()
        {
            System.DateTime datetime = System.DateTime.Now;
            string timestr = string.Format("{0:0000}-{1:00}-{2:00}-{3:00}-{4:00}-{5:00}-{6}", datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Ticks);
            return timestr;
        }

        private void Button_CatchImage(object sender, RoutedEventArgs e)
        {
            camera1.Source = Tools.camera[0].GetImageSoure();
            camera2.Source = Tools.camera[1].GetImageSoure();
        }

        public void GetRects(Mat temp, Mat wafer)
        {
            //读取图片
            Mat result = new Mat(); //匹配结果
            //模板匹配
            Cv2.MatchTemplate(wafer, temp, result, TemplateMatchModes.CCoeffNormed);//最好匹配为1,值越小匹配越差
            Double minVul;
            Double maxVul;
            OpenCvSharp.Point minLoc = new OpenCvSharp.Point(0, 0);
            OpenCvSharp.Point maxLoc = new OpenCvSharp.Point(0, 0);
            OpenCvSharp.Point matchLoc = new OpenCvSharp.Point(0, 0);
            Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax, -1);//归一化
            Cv2.MinMaxLoc(result, out minVul, out maxVul, out minLoc, out maxLoc);//查找极值
            matchLoc = maxLoc;//最大值坐标
            //result.Set(matchLoc.Y, matchLoc.X, 0);//改变最大值为最小值  
            Mat mask = wafer.Clone();//复制整个矩阵
            //画框显示
            Cv2.Rectangle(mask, matchLoc, new OpenCvSharp.Point(matchLoc.X + temp.Cols, matchLoc.Y + temp.Rows), Scalar.Green, 2);

            Console.WriteLine("最大值：{0}，X:{1}，Y:{2}", maxVul, matchLoc.Y, matchLoc.X);
            Console.WriteLine("At获取最大值(Y,X)：{0}", result.At<float>(matchLoc.Y, matchLoc.X));
            Console.WriteLine("result的类型：{0}", result.GetType());

            //循环查找画框显示
            Double threshold = 0.8;
            Mat maskMulti = wafer.Clone();//复制整个矩阵

            for (int i = 1; i < result.Rows - temp.Rows; i += temp.Rows)//行遍历
            {

                for (int j = 1; j < result.Cols - temp.Cols; j += temp.Cols)//列遍历
                {
                    OpenCvSharp.Rect roi = new OpenCvSharp.Rect(j, i, temp.Cols, temp.Rows);        //建立感兴趣
                    Mat RoiResult = new Mat(result, roi);
                    Cv2.MinMaxLoc(RoiResult, out minVul, out maxVul, out minLoc, out maxLoc);//查找极值
                    matchLoc = maxLoc;//最大值坐标
                    if (maxVul > threshold)
                    {

                        //画框显示
                        Cv2.Rectangle(maskMulti, new OpenCvSharp.Point(j + maxLoc.X, i + maxLoc.Y), new OpenCvSharp.Point(j + maxLoc.X + temp.Cols, i + maxLoc.Y + temp.Rows), Scalar.Green, 2);
                        string axis = '(' + Convert.ToString(i + maxLoc.Y) + ',' + Convert.ToString(j + maxLoc.X) + ')';
                        Cv2.PutText(maskMulti, axis, new OpenCvSharp.Point(j + maxLoc.X, i + maxLoc.Y), HersheyFonts.HersheyPlain, 1, Scalar.Red, 1, LineTypes.Link4);

                    }

                }
            }


            //新建窗体显示图片
            Cv2.Resize(maskMulti, maskMulti,new OpenCvSharp.Size(512, 384));
            using (new OpenCvSharp.Window("maskMulti image", maskMulti))
            {
                Cv2.WaitKey();
            }
         
        }

        private void Button_Test(object sender, RoutedEventArgs e)
        {
        }

        private void Button_CatchAndSaveImage(object sender, RoutedEventArgs e)
        {
            Button_CatchImage(sender, e);
            Button_SaveImage(sender, e);
        }

        private void Button_SaveImage(object sender, RoutedEventArgs e)
        {
            try
            {
                System.DateTime datetime = System.DateTime.Now;
                string timestr = string.Format("{0:0000}-{1:00}-{2:00}-{3:00}-{4:00}-{5:00}", datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second);
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.Filter = "Image Files (*.jpg)|*.jpg | All Files | *.*";
                sfd.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
                sfd.FileName = timestr;
                if (sfd.ShowDialog() == true)
                {
                    var encoder = new JpegBitmapEncoder();

                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Tools.camera[0].GetImageSoure()));
                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                        encoder.Save(stream);
                }
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message);
            }
        }

        private void Button_CameraSetting(object sender, RoutedEventArgs e)
        {
            CameraSetting cameraset = new CameraSetting();
            cameraset.Show();
        }

        private void Button_LabelSetting(object sender, RoutedEventArgs e)
        {
            LabelSetting labelset = new LabelSetting();
            labelset.Show();
        }

        private void Button_OneDCodeSetting(object sender, RoutedEventArgs e)
        {
            OneDCodeSetting onedcodeset = new OneDCodeSetting();
            onedcodeset.Show();
        }

        private void Button_TemplateSetting(object sender, RoutedEventArgs e)
        {
            TemplateSetting templatesetting = new TemplateSetting();
            templatesetting.Show();
        }

        private void Button_TestSetting(object sender, RoutedEventArgs e)
        {
            TestSetting testsetting = new TestSetting();
            testsetting.Show();
        }

        private void Button_OrcNumberSetting(object sender, RoutedEventArgs e)
        {
            OrcNumberSetting orcnumbersetting = new OrcNumberSetting();
            orcnumbersetting.Show();
        }

        private void Button_CheckImageByTimer(object sender, RoutedEventArgs e)
        {
            isrealtime = !isrealtime;
        }
        public void FinishTest()
        {
            if(Tools.booldcardinfo[0].is_all_ok&& Tools.booldcardinfo[1].is_all_ok)
                zlg_can.SendOK();
            else
                zlg_can.SendNG();
        }
        public int ok_count = 0;
        public void GoTest()
        {
            if (ok_count == 0)
            {
                ok_count++;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Thread thread = new Thread(() =>
                {
                    zlg_can.SendLight1();
                    var uiinfo = tools.Explain(0);
                    ok_count++;
                    if (ok_count == 3)
                    {
                        sw.Stop();
                        Console.WriteLine("总运行时间：" + sw.ElapsedMilliseconds + " Pass:" + (Tools.booldcardinfo[0].is_all_ok && Tools.booldcardinfo[1].is_all_ok));
                        ok_count++;
                        FinishTest();
                    }

                });
                thread.Start();

                Thread thread1 = new Thread(() =>
                {
                    zlg_can.SendLight2();
                    var uiinfo = tools.Explain(1);
                    ok_count++;
                    if (ok_count == 3)
                    {
                        sw.Stop();
                        Console.WriteLine("总运行时间：" + sw.ElapsedMilliseconds + " Pass:"+ (Tools.booldcardinfo[0].is_all_ok&& Tools.booldcardinfo[1].is_all_ok));
                        ok_count++;
                        FinishTest();
                    }
                });
                thread1.Start();
            }

        }

        public void GoTest(Mat imgl, Mat imgh,int index)
        {
            
        }

        private void Button_CheckImageByPic(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog opndlg = new System.Windows.Forms.OpenFileDialog();
            opndlg.Filter = "所有图像文件|*.bmp;*.pcx;*.png;*.jpg;*.gif";
            opndlg.Title = "打开图像文件";
            if (opndlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var img = Cv2.ImRead(opndlg.FileName, 0);
                tools.Explain(0, img);
            }
        }
        private void Button_Collect(object sender, RoutedEventArgs e)
        {
            iscollect = !iscollect;
        }
       
        private void Button_CheckImageByHand(object sender, RoutedEventArgs e)
        {
            GoTest();
        }

        private void Button_CheckResult(object sender, RoutedEventArgs e)
        {
            codesancer.Open("COM3");
            codesancer.Start(true);
            codesancer.DataReceived += OndeCodeScaner;
        }

        public void OndeCodeScaner(AbstractScaner scaner, string code)
        {

        }

    }
}
