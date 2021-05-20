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
    /// TestSetting.xaml 的交互逻辑
    /// </summary>
    public partial class TestSetting : Window
    {
        public DispatcherTimer timer;
        public static BloodParameter bloodparameter;

        public static int lever_step;
        public static int lever_umax;
        public static int lever_umin;
        public static double lever_ucv;
        public static double lever_maxucv;
        public static int lever_dmax;
        public static int lever_dmin;
        public static double lever_dcv;
        public static double lever_maxdcv;

        public static int tube_tw;
        public static int tube_ew;
        public static int tube_bh;

        public static int inspection_step;
        public static int inspection_maxarea;
        public static int inspection_minarea;
        public static double inspection_raio;
        public static int inspection_count;
        public static double inspection_pixscore;

        public static double yhmin;
        public static double yhmax;
        public static double ysmin;
        public static double ysmax;
        public static double yvmin;
        public static double yvmax;

        public static double bhmin;
        public static double bhmax;
        public static double bsmin;
        public static double bsmax;
        public static double bvmin;
        public static double bvmax;

        public static double nhmin;
        public static double nhmax;
        public static double nsmin;
        public static double nsmax;
        public static double nvmin;
        public static double nvmax;

        public static double lnhmin;
        public static double lnhmax;
        public static double lnsmin;
        public static double lnsmax;
        public static double lnvmin;
        public static double lnvmax;

        public static int label_minw;
        public static int label_maxw;

        public static bool is_space;//标签距离
        public static bool is_labelfilp;//标签反转

        public static bool is_uinspection;//上液面
        public static bool is_dinspection;//下液面
        public static bool is_onedcode;//一维码
        public static bool is_color;//颜色
        public static bool is_inspection;//杂质
        public static bool is_savedata;//是否保存数据

        public static bool is_show_space;//标签距离
        public static bool is_show_uinspection;//上液面
        public static bool is_show_dinspection;//下液面

        public static int tbc1;//颜色
        public static int tbc2;//
        public static int tbc3;//
        public static int tbc4;//
        public static int tbc5;//
        public static int tbc6;//
        public static int tbc7;//
        public static int tbc8;//


        public TestSetting()
        {
            InitializeComponent();
            camera_sel.Items.Add("相机1");
            camera_sel.Items.Add("相机2");
            camera_sel.SelectedIndex = 0;
            bloodparameter = Tools.booldcardinfo[0].bloodParameter;
            ComboBox[] cobobox_tc = { combobox_tc1, combobox_tc2, combobox_tc3, combobox_tc4, combobox_tc5, combobox_tc6, combobox_tc7, combobox_tc8 };
            for(int i=0;i< cobobox_tc.Count();i++)
            {
                string[] colorstr = { "错色", "黄色", "蓝色", "无色","任何" };
                for(int j=0;j<colorstr.Count();j++)
                {
                    cobobox_tc[i].Items.Add(colorstr[j]);
                }
                cobobox_tc[i].SelectedIndex = 0;
            }
            UpDataParameter();
        }

        private void OnSelectCamera(object sender, SelectionChangedEventArgs e)
        {
            bloodparameter = Tools.booldcardinfo[camera_sel.SelectedIndex].bloodParameter;
            UpDataParameter();
            OnLoaded(sender,e);
        }


        public void UpDataParameter()
        {
            lever_step = bloodparameter.lever_step;
            lever_umax = bloodparameter.lever_umax;
            lever_umin = bloodparameter.lever_umin;
            lever_ucv = bloodparameter.lever_ucv;
            lever_maxucv = bloodparameter.lever_maxucv;
            lever_dmax = bloodparameter.lever_dmax;
            lever_dmin = bloodparameter.lever_dmin;
            lever_dcv = bloodparameter.lever_dcv;
            lever_maxdcv = bloodparameter.lever_maxdcv;

            tube_tw = bloodparameter.tube_tw;
            tube_ew = bloodparameter.tube_ew;
            tube_bh = bloodparameter.tube_bh;

            inspection_step = bloodparameter.inspection_step;
            inspection_maxarea = bloodparameter.inspection_maxarea;
            inspection_minarea = bloodparameter.inspection_minarea;
            inspection_raio = bloodparameter.inspection_raio;
            inspection_count = bloodparameter.inspection_count;
            inspection_pixscore = bloodparameter.inspection_pixscore;

            yhmin = bloodparameter.yhmin;
            yhmax = bloodparameter.yhmax;
            ysmin = bloodparameter.ysmin;
            ysmax = bloodparameter.ysmax;
            yvmin = bloodparameter.yvmin;
            yvmax = bloodparameter.yvmax;

            bhmin = bloodparameter.bhmin;
            bhmax = bloodparameter.bhmax;
            bsmin = bloodparameter.bsmin;
            bsmax = bloodparameter.bsmax;
            bvmin = bloodparameter.bvmin;
            bvmax = bloodparameter.bvmax;

            nhmin = bloodparameter.nhmin;
            nhmax = bloodparameter.nhmax;
            nsmin = bloodparameter.nsmin;
            nsmax = bloodparameter.nsmax;
            nvmin = bloodparameter.nvmin;
            nvmax = bloodparameter.nvmax;

            lnhmin = bloodparameter.lnhmin;
            lnhmax = bloodparameter.lnhmax;
            lnsmin = bloodparameter.lnsmin;
            lnsmax = bloodparameter.lnsmax;
            lnvmin = bloodparameter.lnvmin;
            lnvmax = bloodparameter.lnvmax;

            label_minw = bloodparameter.label_minw;
            label_maxw = bloodparameter.label_maxw;

            is_space = bloodparameter.is_space;
            is_labelfilp = bloodparameter.is_labelfilp;
            is_uinspection = bloodparameter.is_uinspection;
            is_dinspection = bloodparameter.is_dinspection;
            is_onedcode = bloodparameter.is_onedcode;
            is_color = bloodparameter.is_color;
            is_inspection = bloodparameter.is_inspection;
            is_savedata = bloodparameter.is_savedata;

            is_show_space = bloodparameter.is_show_space;
            is_show_uinspection = bloodparameter.is_show_uinspection;
            is_show_dinspection = bloodparameter.is_show_dinspection;

            tbc1 = bloodparameter.tbc1;
            tbc2 = bloodparameter.tbc2;
            tbc3 = bloodparameter.tbc3;
            tbc4 = bloodparameter.tbc4;
            tbc5 = bloodparameter.tbc5;
            tbc6 = bloodparameter.tbc6;
            tbc7 = bloodparameter.tbc7;
            tbc8 = bloodparameter.tbc8;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            UpDataParameter();

            textbox_lever_step.Text = lever_step.ToString();
            textbox_lever_umax.Text = lever_umax.ToString();
            textbox_lever_umin.Text = lever_umin.ToString();
            textbox_lever_ucv.Text = lever_ucv.ToString();
            textbox_lever_maxucv.Text = lever_maxucv.ToString();
            textbox_lever_dmax.Text = lever_dmax.ToString();
            textbox_lever_dmin.Text = lever_dmin.ToString();
            textbox_lever_dcv.Text = lever_dcv.ToString();
            textbox_lever_maxdcv.Text = lever_maxdcv.ToString();

            textbox_tubetw.Text = tube_tw.ToString();
            textbox_tubeew.Text = tube_ew.ToString();
            textbox_tubebh.Text = tube_bh.ToString();

            textbox_inspection_step.Text = inspection_step.ToString();
            textbox_inspection_maxarea.Text = inspection_maxarea.ToString();
            textbox_inspection_minarea.Text = inspection_minarea.ToString();
            textbox_inspection_raio.Text = inspection_raio.ToString();
            textbox_inspection_count.Text = inspection_count.ToString();
            textbox_inspection_pixscore.Text = inspection_pixscore.ToString();

            textbox_color_yminh.Text = yhmin.ToString();
            textbox_color_ymaxh.Text = yhmax.ToString();

            textbox_color_ymins.Text = ysmin.ToString();
            textbox_color_ymaxs.Text = ysmax.ToString();

            textbox_color_yminv.Text = yvmin.ToString();
            textbox_color_ymaxv.Text = yvmax.ToString();

            textbox_color_bminh.Text = bhmin.ToString();
            textbox_color_bmaxh.Text = bhmax.ToString();
        
            textbox_color_bmins.Text = bsmin.ToString();
            textbox_color_bmaxs.Text = bsmax.ToString();

            textbox_color_bminv.Text = bvmin.ToString();
            textbox_color_bmaxv.Text = bvmax.ToString();

            textbox_color_nminh.Text = nhmin.ToString();
            textbox_color_nmaxh.Text = nhmax.ToString();

            textbox_color_nmins.Text = nsmin.ToString();
            textbox_color_nmaxs.Text = nsmax.ToString();

            textbox_color_nminv.Text = nvmin.ToString();
            textbox_color_nmaxv.Text = nvmax.ToString();

            textbox_color_lnminh.Text = lnhmin.ToString();
            textbox_color_lnmaxh.Text = lnhmax.ToString();

            textbox_color_lnmins.Text = lnsmin.ToString();
            textbox_color_lnmaxs.Text = lnsmax.ToString();

            textbox_color_lnminv.Text = lnvmin.ToString();
            textbox_color_lnmaxv.Text = lnvmax.ToString();

            textbox_label_minw.Text = label_minw.ToString();
            textbox_label_maxw.Text = label_maxw.ToString();


            checkbox_is_space.IsChecked =  is_space;
            checkbox_is_labelfilp.IsChecked = is_labelfilp;
            checkbox_is_uinspection.IsChecked = is_uinspection;
            checkbox_is_dinspection.IsChecked = is_dinspection;
            checkbox_is_onedcode.IsChecked = is_onedcode;
            checkbox_is_color.IsChecked = is_color;
            checkbox_is_inspection.IsChecked = is_inspection;
            checkbox_is_savedata.IsChecked = is_savedata;

            checkbox_is_show_space.IsChecked = is_show_space;
            checkbox_is_show_uinspection.IsChecked = is_show_uinspection;
            checkbox_is_show_dinspection.IsChecked = is_show_dinspection;

            combobox_tc1.SelectedIndex = tbc1;
            combobox_tc2.SelectedIndex = tbc2;
            combobox_tc3.SelectedIndex = tbc3;
            combobox_tc4.SelectedIndex = tbc4;
            combobox_tc5.SelectedIndex = tbc5;
            combobox_tc6.SelectedIndex = tbc6;
            combobox_tc7.SelectedIndex = tbc7;
            combobox_tc8.SelectedIndex = tbc8;
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {

            lever_step = int.Parse(textbox_lever_step.Text);
            lever_umax = int.Parse(textbox_lever_umax.Text);
            lever_umin = int.Parse(textbox_lever_umin.Text);
            lever_ucv = double.Parse(textbox_lever_ucv.Text);
            lever_maxucv = double.Parse(textbox_lever_maxucv.Text);

            lever_dmax = int.Parse(textbox_lever_dmax.Text);
            lever_dmin = int.Parse(textbox_lever_dmin.Text);
            lever_dcv = double.Parse(textbox_lever_dcv.Text);
            lever_maxdcv = double.Parse(textbox_lever_maxdcv.Text);

            tube_tw = int.Parse(textbox_tubetw.Text);
            tube_ew = int.Parse(textbox_tubeew.Text);
            tube_bh = int.Parse(textbox_tubebh.Text);

            inspection_step = int.Parse(textbox_inspection_step.Text);
            inspection_maxarea = int.Parse(textbox_inspection_maxarea.Text);
            inspection_minarea = int.Parse(textbox_inspection_minarea.Text);
            inspection_raio = double.Parse(textbox_inspection_raio.Text);
            inspection_count = int.Parse(textbox_inspection_count.Text);
            inspection_pixscore = double.Parse(textbox_inspection_pixscore.Text);

            yhmin = double.Parse(textbox_color_yminh.Text);
            yhmax = double.Parse(textbox_color_ymaxh.Text);
            ysmin = double.Parse(textbox_color_ymins.Text);
            ysmax = double.Parse(textbox_color_ymaxs.Text);
            yvmin = double.Parse(textbox_color_yminv.Text);
            yvmax = double.Parse(textbox_color_ymaxv.Text);

            bhmin = double.Parse(textbox_color_bminh.Text);
            bhmax = double.Parse(textbox_color_bmaxh.Text);
            bsmin = double.Parse(textbox_color_bmins.Text);
            bsmax = double.Parse(textbox_color_bmaxs.Text);
            bvmin = double.Parse(textbox_color_bminv.Text);
            bvmax = double.Parse(textbox_color_bmaxv.Text);

            nhmin = double.Parse(textbox_color_nminh.Text);
            nhmax = double.Parse(textbox_color_nmaxh.Text);
            nsmin = double.Parse(textbox_color_nmins.Text);
            nsmax = double.Parse(textbox_color_nmaxs.Text);
            nvmin = double.Parse(textbox_color_nminv.Text);
            nvmax = double.Parse(textbox_color_nmaxv.Text);

            lnhmin = double.Parse(textbox_color_lnminh.Text);
            lnhmax = double.Parse(textbox_color_lnmaxh.Text);
            lnsmin = double.Parse(textbox_color_lnmins.Text);
            lnsmax = double.Parse(textbox_color_lnmaxs.Text);
            lnvmin = double.Parse(textbox_color_lnminv.Text);
            lnvmax = double.Parse(textbox_color_lnmaxv.Text);

            label_minw = int.Parse(textbox_label_minw.Text);
            label_maxw = int.Parse(textbox_label_maxw.Text);

            is_space = bool.Parse(checkbox_is_space.IsChecked.ToString());
            is_labelfilp = bool.Parse(checkbox_is_labelfilp.IsChecked.ToString());
            is_uinspection = bool.Parse(checkbox_is_uinspection.IsChecked.ToString());
            is_dinspection = bool.Parse(checkbox_is_dinspection.IsChecked.ToString());
            is_onedcode = bool.Parse(checkbox_is_onedcode.IsChecked.ToString());
            is_color = bool.Parse(checkbox_is_color.IsChecked.ToString());
            is_inspection = bool.Parse(checkbox_is_inspection.IsChecked.ToString());
            is_savedata = bool.Parse(checkbox_is_savedata.IsChecked.ToString());

            is_show_space = bool.Parse(checkbox_is_show_space.IsChecked.ToString());
            is_show_uinspection = bool.Parse(checkbox_is_show_uinspection.IsChecked.ToString());
            is_show_dinspection = bool.Parse(checkbox_is_show_dinspection.IsChecked.ToString());

            tbc1 = combobox_tc1.SelectedIndex;
            tbc2 = combobox_tc2.SelectedIndex;
            tbc3 = combobox_tc3.SelectedIndex;
            tbc4 = combobox_tc4.SelectedIndex;
            tbc5 = combobox_tc5.SelectedIndex;
            tbc6 = combobox_tc6.SelectedIndex;
            tbc7 = combobox_tc7.SelectedIndex;
            tbc8 = combobox_tc8.SelectedIndex;


            bloodparameter.lever_step = lever_step;
            bloodparameter.lever_umax = lever_umax;
            bloodparameter.lever_umin = lever_umin;
            bloodparameter.lever_ucv = lever_ucv;
            bloodparameter.lever_maxucv = lever_maxucv;

            bloodparameter.lever_dmax = lever_dmax;
            bloodparameter.lever_dmin = lever_dmin;
            bloodparameter.lever_dcv = lever_dcv;
            bloodparameter.lever_maxdcv = lever_maxdcv;

            bloodparameter.tube_tw = tube_tw;
            bloodparameter.tube_ew = tube_ew;
            bloodparameter.tube_bh = tube_bh;

            bloodparameter.inspection_step = inspection_step;
            bloodparameter.inspection_maxarea = inspection_maxarea;
            bloodparameter.inspection_minarea = inspection_minarea;
            bloodparameter.inspection_raio = inspection_raio;
            bloodparameter.inspection_count = inspection_count;
            bloodparameter.inspection_pixscore = inspection_pixscore;

            bloodparameter.yhmin = yhmin;
            bloodparameter.yhmax = yhmax;
            bloodparameter.ysmin = ysmin;
            bloodparameter.ysmax = ysmax;
            bloodparameter.yvmin = yvmin;
            bloodparameter.yvmax = yvmax;

            bloodparameter.bhmin = bhmin;
            bloodparameter.bhmax = bhmax;
            bloodparameter.bsmin = bsmin;
            bloodparameter.bsmax = bsmax;
            bloodparameter.bvmin = bvmin;
            bloodparameter.bvmax = bvmax;

            bloodparameter.nhmin = nhmin;
            bloodparameter.nhmax = nhmax;
            bloodparameter.nsmin = nsmin;
            bloodparameter.nsmax = nsmax;
            bloodparameter.nvmin = nvmin;
            bloodparameter.nvmax = nvmax;

            bloodparameter.lnhmin = lnhmin;
            bloodparameter.lnhmax = lnhmax;
            bloodparameter.lnsmin = lnsmin;
            bloodparameter.lnsmax = lnsmax;
            bloodparameter.lnvmin = lnvmin;
            bloodparameter.lnvmax = lnvmax;

            bloodparameter.label_minw = label_minw;
            bloodparameter.label_maxw = label_maxw;

            bloodparameter.is_space = is_space;
            bloodparameter.is_labelfilp = is_labelfilp;
            bloodparameter.is_uinspection = is_uinspection;
            bloodparameter.is_dinspection = is_dinspection;
            bloodparameter.is_onedcode = is_onedcode;
            bloodparameter.is_color = is_color;
            bloodparameter.is_inspection = is_inspection;
            bloodparameter.is_savedata = is_savedata;

            bloodparameter.is_show_space = is_show_space;
            bloodparameter.is_show_uinspection = is_show_uinspection;
            bloodparameter.is_show_dinspection = is_show_dinspection;

            bloodparameter.tbc1 = tbc1;
            bloodparameter.tbc2 = tbc2;
            bloodparameter.tbc3 = tbc3;
            bloodparameter.tbc4 = tbc4;
            bloodparameter.tbc5 = tbc5;
            bloodparameter.tbc6 = tbc6;
            bloodparameter.tbc7 = tbc7;
            bloodparameter.tbc8 = tbc8;

            bloodparameter.Save();
        }

    }
}
