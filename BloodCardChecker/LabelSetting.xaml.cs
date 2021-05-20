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
using DaHenCamera;
using OpenCvSharp;
using System.Diagnostics;

namespace BloodCardChecker
{
    /// <summary>
    /// LabelSetting.xaml 的交互逻辑
    /// </summary>
    public partial class LabelSetting : System.Windows.Window
    {
        public DhCamera camera;
        public DispatcherTimer timer;
        public static BloodParameter bloodparameter;
        public static int x;
        public static int y;
        public static int w;
        public static int h;
        public static int s1;
        public static int s2;
        public double camera_ration_w = 1;
        public double camera_ration_h = 1;
        public LabelSetting()
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

            var img = camera.TakeImage(bloodparameter.onedcode_s3);
            camera1.Source = Tools.OpenCvImgToImageSoure(img);
            camera2.Source = Tools.OpenCvImgToImageSoure(img);

            UpDataParameter();
        }

        private void OnSelectCamera(object sender, SelectionChangedEventArgs e)
        {
            camera = Tools.camera[camera_sel.SelectedIndex];
            bloodparameter = Tools.booldcardinfo[camera_sel.SelectedIndex].bloodParameter;
            OnLoaded(sender, e);
            //事件响应
        }


        public void UpDataValue()
        {
            slider_x.Value = x;
            slider_y.Value = y;
            slider_w.Value = w;
            slider_h.Value = h;
            slider_s1.Value = s1;
            slider_s2.Value = s2;
        }

        public void UpDataParameter()
        {
            x =bloodparameter.label_x;
            y =bloodparameter.label_y;
            w =bloodparameter.label_w;
            h =bloodparameter.label_h;
            s1 =bloodparameter.label_s1;
            s2 =bloodparameter.label_s2;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            UpDataParameter();
            UpDataValue();
            UpDataUI(sender, e);
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

            textbox_x.Text = x.ToString();
            textbox_y.Text = y.ToString();
            textbox_w.Text = w.ToString();
            textbox_h.Text = h.ToString();
            textbox_s1.Text = s1.ToString();
            textbox_s2.Text = s2.ToString();

            checkbox_rect1.SetValue(Canvas.LeftProperty,x/ camera_ration_w);
            checkbox_rect1.SetValue(Canvas.TopProperty,y/ camera_ration_h);
            checkbox_rect1.SetValue(Canvas.WidthProperty,w/ camera_ration_w);
            checkbox_rect1.SetValue(Canvas.HeightProperty,h/ camera_ration_h);

            checkbox_rect2.SetValue(Canvas.LeftProperty, x / camera_ration_w);
            checkbox_rect2.SetValue(Canvas.TopProperty, y / camera_ration_h + camera1.Height);
            checkbox_rect2.SetValue(Canvas.WidthProperty, w / camera_ration_w);
            checkbox_rect2.SetValue(Canvas.HeightProperty, h / camera_ration_h);
        }

        private void TestLabelSpace(object sender, RoutedEventArgs e)
        {
            x = (int)slider_x.Value;
            y = (int)slider_y.Value;
            w = (int)slider_w.Value;
            h = (int)slider_h.Value;
            s1 = (int)slider_s1.Value;
            s2 = (int)slider_s2.Value;

            var img = camera.TakeImage(bloodparameter.onedcode_s3);
            var space = LabelSpace(img);
            labelspace.Content = "测试结果:" + space[0] + "," + space[1];
            img = img.CvtColor(ColorConversionCodes.BGR2GRAY);
            var img1 = img.Threshold(s1, 255, ThresholdTypes.Binary);
            var img2 = img.Threshold(s2, 255, ThresholdTypes.Binary);
            camera1.Source = Tools.OpenCvImgToImageSoure(img1);
            camera2.Source = Tools.OpenCvImgToImageSoure(img2);

           bloodparameter.label_x = x;
           bloodparameter.label_y = y;
           bloodparameter.label_w = w;
           bloodparameter.label_h = h;
           bloodparameter.label_s1 = s1;
           bloodparameter.label_s2 = s2;
           bloodparameter.Save();
        }

        public static List<int> LabelSpace(Mat img)
        {
            img = img.Clone();
            var space = Tools.CheckLabelSpace(img, 2, x, y, w, h, s1, s2);
            return space;
        }
    }
}
