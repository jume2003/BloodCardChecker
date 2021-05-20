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
using System.Threading;
using DaHenCameraEnum;

namespace BloodCardChecker
{
    /// <summary>
    /// CameraSetting.xaml 的交互逻辑
    /// </summary>
    public partial class CameraSetting : Window
    {
        public DhCamera camera;
        public DispatcherTimer timer;
        public static BloodParameter bloodparameter;
        public static double camera_ration = 4.0f;
        public static double selector_red = 0;
        public static double selector_blue = 0;
        public static double selector_green = 0;
        public static double selector_gain = 0;

        public CameraSetting()
        {
            InitializeComponent();
            camera_sel.Items.Add("相机1");
            camera_sel.Items.Add("相机2");
            camera_sel.SelectedIndex = 0;
            camera = Tools.camera[0];
            bloodparameter = Tools.booldcardinfo[0].bloodParameter;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
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


        private void OnClosed(object sender, EventArgs e)
        {
            timer.Stop();
        }

        public void UpDataParameter()
        {
            selector_gain =bloodparameter.camre_gain;
            selector_red =bloodparameter.camre_rb;
            selector_green =bloodparameter.camre_gb;
            selector_blue =bloodparameter.camre_bb;

            camera.SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_RED, selector_red);
            camera.SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_BLUE, selector_green);
            camera.SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_GREEN, selector_blue);
            camera.SetGain(selector_gain);
            camera.SetExposureTime(bloodparameter.onedcode_s3);
            //camera.Play();
        }

        public void UpDataValue()
        {
            setvaluered.Value = selector_red;
            setvalueblue.Value = selector_blue;
            setvaluegreen.Value = selector_green;
            setvaluegain.Value = selector_gain;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            UpDataParameter();
            UpDataValue();
            UpDataUI(sender, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
           bloodparameter.camre_gain = selector_gain;
           bloodparameter.camre_rb = selector_red;
           bloodparameter.camre_gb = selector_green;
           bloodparameter.camre_bb = selector_blue;
           bloodparameter.Save();
        }

        private void UpDataUI(object sender, EventArgs e)
        {
            selector_red = setvaluered.Value;
            selector_blue = setvalueblue.Value;
            selector_green = setvaluegreen.Value;
            selector_gain = setvaluegain.Value;

            label_setvaluegain.Content = selector_gain;
            label_setvaluered.Content = selector_red;
            label_setvalueblue.Content = selector_blue;
            label_setvaluegreen.Content = selector_green;


            camera.SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_RED, selector_red);
            camera.SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_BLUE, selector_green);
            camera.SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_GREEN, selector_blue);
            camera.SetGain(selector_gain);
            camera.TakeImage(bloodparameter.onedcode_s3);
            camera1.Source = camera.GetImageSoure();
        }
        
    }
}
