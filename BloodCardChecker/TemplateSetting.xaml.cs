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
    /// TemplateSetting.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateSetting : System.Windows.Window
    {
        public DhCamera camera;
        public DispatcherTimer timer;
        public static BloodParameter bloodparameter;
        public static double camera_ration_w = 1.0f;
        public static double camera_ration_h = 1.0f;

        public static int tut_x;
        public static int tut_y;
        public static int tut_w;
        public static int tut_h;

        public static int icon_x;
        public static int icon_y;
        public static int icon_w;
        public static int icon_h;

        public static int at_x;
        public static int at_y;
        public static int at_w;
        public static int at_h;
        public TemplateSetting()
        {
            InitializeComponent();
            camera_sel.Items.Add("相机1");
            camera_sel.Items.Add("相机2");
            camera_sel.SelectedIndex = 0;
            camera = Tools.camera[0];
            bloodparameter = Tools.booldcardinfo[0].bloodParameter;
            camera1.Source = camera.GetImageSoure();
            camera_ration_w = camera.camera_img.Width / camera1.Width;
            camera_ration_h = camera.camera_img.Height/ camera1.Height;
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
            tut_x = bloodparameter.tut_x;
            tut_y = bloodparameter.tut_y;
            tut_w = bloodparameter.tut_w;
            tut_h = bloodparameter.tut_h;

            icon_x = bloodparameter.icon_x;
            icon_y = bloodparameter.icon_y;
            icon_w = bloodparameter.icon_w;
            icon_h = bloodparameter.icon_h;

            at_x = bloodparameter.at_x;
            at_y = bloodparameter.at_y;
            at_w = bloodparameter.at_w;
            at_h = bloodparameter.at_h;
        }

        public static void SelectCamera(int index)
        {
            bloodparameter = Tools.booldcardinfo[index].bloodParameter;
        }

        public void UpDataValue()
        {
            tutsli_x.Value = tut_x;
            tutsli_y.Value = tut_y;
            tutsli_w.Value = tut_w;
            tutsli_h.Value = tut_h;

            iconsli_x.Value = icon_x;
            iconsli_y.Value = icon_y;
            iconsli_w.Value = icon_w;
            iconsli_h.Value = icon_h;

            atsli_x.Value = at_x;
            atsli_y.Value = at_y;
            atsli_w.Value = at_w;
            atsli_h.Value = at_h;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            UpDataParameter();
            UpDataValue();
            UpDataUI(sender, e);
            SaveTemplate(sender, null);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            timer.Stop();
        }


        private void UpDataUI(object sender, EventArgs e)
        {
            tut_x = (int)tutsli_x.Value;
            tut_y = (int)tutsli_y.Value;
            tut_w = (int)tutsli_w.Value;
            tut_h = (int)tutsli_h.Value;

            icon_x = (int)iconsli_x.Value;
            icon_y = (int)iconsli_y.Value;
            icon_w = (int)iconsli_w.Value;
            icon_h = (int)iconsli_h.Value;

            at_x = (int)atsli_x.Value;
            at_y = (int)atsli_y.Value;
            at_w = (int)atsli_w.Value;
            at_h = (int)atsli_h.Value;

            tuttext_x.Text = tut_x.ToString();
            tuttext_y.Text = tut_y.ToString();
            tuttext_w.Text = tut_w.ToString();
            tuttext_h.Text = tut_h.ToString();

            icontext_x.Text = icon_x.ToString();
            icontext_y.Text = icon_y.ToString();
            icontext_w.Text = icon_w.ToString();
            icontext_h.Text = icon_h.ToString();

            attext_x.Text = at_x.ToString();
            attext_y.Text = at_y.ToString();
            attext_w.Text = at_w.ToString();
            attext_h.Text = at_h.ToString();

            checkbox_rect1.SetValue(Canvas.LeftProperty, tut_x / camera_ration_w);
            checkbox_rect1.SetValue(Canvas.TopProperty, tut_y / camera_ration_h);
            checkbox_rect1.SetValue(Canvas.WidthProperty, tut_w / camera_ration_w);
            checkbox_rect1.SetValue(Canvas.HeightProperty, tut_h / camera_ration_h);


            checkbox_rect2.SetValue(Canvas.LeftProperty, at_x / camera_ration_w);
            checkbox_rect2.SetValue(Canvas.TopProperty, at_y / camera_ration_h);
            checkbox_rect2.SetValue(Canvas.WidthProperty, at_w / camera_ration_w);
            checkbox_rect2.SetValue(Canvas.HeightProperty, at_h / camera_ration_h);

            checkbox_rect3.SetValue(Canvas.LeftProperty, icon_x / camera_ration_w);
            checkbox_rect3.SetValue(Canvas.TopProperty, icon_y / camera_ration_h);
            checkbox_rect3.SetValue(Canvas.WidthProperty, icon_w / camera_ration_w);
            checkbox_rect3.SetValue(Canvas.HeightProperty, icon_h / camera_ration_h);


        }

        private void SaveTemplate(object sender, RoutedEventArgs e)
        {
            bloodparameter.tut_x = tut_x;
            bloodparameter.tut_y = tut_y;
            bloodparameter.tut_w = tut_w;
            bloodparameter.tut_h = tut_h;

            bloodparameter.icon_x = icon_x;
            bloodparameter.icon_y = icon_y;
            bloodparameter.icon_w = icon_w;
            bloodparameter.icon_h = icon_h;

            bloodparameter.at_x = at_x;
            bloodparameter.at_y = at_y;
            bloodparameter.at_w = at_w;
            bloodparameter.at_h = at_h;

            bloodparameter.Save();
        }
    }
}
