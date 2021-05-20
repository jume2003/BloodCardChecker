using OpenCvSharp;
using DaHenCamera;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BloodCardChecker
{
    /// <summary>
    /// OneDCodeSetting.xaml 的交互逻辑
    /// </summary>
    public partial class OneDCodeSetting : System.Windows.Window
    {
        public DhCamera camera;
        public DispatcherTimer timer;
        public static BloodParameter bloodparameter;
        public static int x;
        public static int y;
        public static int w;
        public static int h;
        public static int tubey;
        public static int s1;
        public static int s2;
        public static int s3;
        public double camera_ration_w = 1;
        public double camera_ration_h = 1;
        public OneDCodeSetting()
        {
            InitializeComponent();
            camera_sel.Items.Add("相机1");
            camera_sel.Items.Add("相机2");
            camera_sel.SelectedIndex = 0;
            camera = Tools.camera[0];
            bloodparameter = Tools.booldcardinfo[0].bloodParameter;
            camera1.Source = camera.GetImageSoure();
            camera_ration_w = camera.camera_img.Width / camera1.Width;
            camera_ration_h = camera.camera_img.Height / camera1.Height;
            //UI更新
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += UpDataUI;
            timer.Start();
            UpDataParameter();
        }
        private void OnSelectCamera(object sender, SelectionChangedEventArgs e)
        {
            camera = Tools.camera[camera_sel.SelectedIndex];
            bloodparameter = Tools.booldcardinfo[camera_sel.SelectedIndex].bloodParameter;
            UpDataParameter();
            OnLoaded(sender, e);
            //事件响应
        }
        public static void UpDataParameter()
        {
            x = bloodparameter.onedcode_x;
            y =bloodparameter.onedcode_y;
            w =bloodparameter.onedcode_w;
            h =bloodparameter.onedcode_h;
            s1 =bloodparameter.onedcode_s1;
            s2 =bloodparameter.onedcode_s2;
            s3 =bloodparameter.onedcode_s3;
            tubey =bloodparameter.tube_y;
        }

        public static void SelectCamera(int index)
        {
            bloodparameter = Tools.booldcardinfo[index].bloodParameter;
        }

        public void UpDataValue()
        {
           slider_x.Value = x;
           slider_y.Value = y;
           slider_w.Value = w;
           slider_h.Value = h;
           slider_s1.Value = s1;
           slider_s2.Value = s2;
           slider_s3.Value = s3;
           slider_tubey.Value = tubey;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            UpDataParameter();
            UpDataValue();
            UpDataUI(sender,e);
            TestOneDCode(sender, null);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            timer.Stop();
        }

        
        private void UpDataUI(object sender, EventArgs e)
        {
            x = (int)slider_x.Value;
            y = (int)slider_y.Value;
            w = (int)slider_w.Value;
            h = (int)slider_h.Value;
            s1 = (int)slider_s1.Value;
            s2 = (int)slider_s2.Value;
            s3 = (int)slider_s3.Value;
            tubey = (int)slider_tubey.Value;

            textbox_x.Text = x.ToString();
            textbox_y.Text = y.ToString();
            textbox_w.Text = w.ToString();
            textbox_h.Text = h.ToString();
            textbox_s1.Text = s1.ToString();
            textbox_s2.Text = s2.ToString();
            textbox_s3.Text = s3.ToString();
            textbox_tubey.Text = tubey.ToString();

            checkbox_rect1.SetValue(Canvas.LeftProperty, x / camera_ration_w);
            checkbox_rect1.SetValue(Canvas.TopProperty, y / camera_ration_h);
            checkbox_rect1.SetValue(Canvas.WidthProperty, w / camera_ration_w);
            checkbox_rect1.SetValue(Canvas.HeightProperty, h / camera_ration_h);
            checkbox_rect2.SetValue(Canvas.TopProperty, tubey / camera_ration_h+ camera1.Height);


        }

        private void TestOneDCode(object sender, RoutedEventArgs e)
        {
            var img = camera.TakeImage(s3);
            camera2.Source = Tools.OpenCvImgToImageSoure(img);
            camera3.Source = Tools.OpenCvImgToImageSoure(img);
            var code = Tools.GetOneDCode(img, x, y, w, h, s1,s2);
            camera1.Source = Tools.OpenCvImgToImageSoure(code.full_img);
            labelspace.Content = "测试结果:" + code.code;

          bloodparameter.onedcode_x = x;
          bloodparameter.onedcode_y = y;
          bloodparameter.onedcode_w = w;
          bloodparameter.onedcode_h = h;
          bloodparameter.onedcode_s1 = s1;
          bloodparameter.onedcode_s2 = s2;
          bloodparameter.onedcode_s3 = s3;
          bloodparameter.tube_y = tubey;
          bloodparameter.Save();
        }
    }
}
