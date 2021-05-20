using OpenCvSharp;
using DaHenCamera;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// OrcNumberSetting.xaml 的交互逻辑
    /// </summary>
    public partial class OrcNumberSetting : System.Windows.Window
    {
        public DhCamera camera;
        public DispatcherTimer timer;
        public static BloodParameter bloodparameter;
        public static int x;
        public static int y;
        public static int w;
        public static int h;
        public static int s1;
        public OrcNumberSetting()
        {
            InitializeComponent();
            camera_sel.Items.Add("相机1");
            camera_sel.Items.Add("相机2");
            camera_sel.SelectedIndex = 0;
            camera = Tools.camera[0];
            bloodparameter = Tools.booldcardinfo[0].bloodParameter;
            camera1.Source = camera.GetImageSoure();
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


        public void UpDataParameter()
        {

            x =bloodparameter.orcnumber_x;
            y =bloodparameter.orcnumber_y;
            w =bloodparameter.orcnumber_w;
            h =bloodparameter.orcnumber_h;
            s1 =bloodparameter.orcnumber_s1;
        }

        public void UpDataValue()
        {
            slider_x.Value = x;
            slider_y.Value = y;
            slider_w.Value = w;
            slider_h.Value = h;
            slider_s1.Value = s1;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            UpDataParameter();
            UpDataValue();
            UpDataUI(sender, e);
            //TestOneDCode(sender, null);
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

            textbox_x.Text = x.ToString();
            textbox_y.Text = y.ToString();
            textbox_w.Text = w.ToString();
            textbox_h.Text = h.ToString();
            textbox_s1.Text = s1.ToString();

            checkbox_rect1.SetValue(Canvas.LeftProperty, x / CameraSetting.camera_ration);
            checkbox_rect1.SetValue(Canvas.TopProperty, y / CameraSetting.camera_ration);
            checkbox_rect1.SetValue(Canvas.WidthProperty, w / CameraSetting.camera_ration);
            checkbox_rect1.SetValue(Canvas.HeightProperty, h / CameraSetting.camera_ration);
        }



        private void TestOrcNumber(object sender, RoutedEventArgs e)
        {
            var img = camera.TakeImage(bloodparameter.onedcode_s3);
            if (img == null) return;
            var orcnumber = Tools.GetOrcNumber(img, x, y, w, h, s1, bloodparameter.onedcode_s2, bloodparameter.ID);
            camera1.Source = Tools.OpenCvImgToImageSoure(img);
            camera2.Source = Tools.OpenCvImgToImageSoure(orcnumber.full_img);
            camera3.Source = Tools.OpenCvImgToImageSoure(orcnumber.number_img);
            labelretnumbers.Content = orcnumber.numberstr;


           bloodparameter.orcnumber_x = x;
           bloodparameter.orcnumber_y = y;
           bloodparameter.orcnumber_w = w;
           bloodparameter.orcnumber_h = h;
           bloodparameter.orcnumber_s1 = s1;
           bloodparameter.Save();
        }
    }
}
