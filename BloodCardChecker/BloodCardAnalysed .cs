using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodCardChecker
{
    public class BloodParameter
    {
        public BloodParameter(int id)
        {
             ID = id;
        }
        public int ID { get; set; }
        public int label_x { get; set; }
        public int label_y { get; set; }
        public int label_w { get; set; }
        public int label_h { get; set; }
        public int label_s1 { get; set; }
        public int label_s2 { get; set; }

        public int onedcode_x { get; set; }
        public int onedcode_y { get; set; }
        public int onedcode_w { get; set; }
        public int onedcode_h { get; set; }
        public int onedcode_s1 { get; set; }
        public int onedcode_s2 { get; set; }
        public int onedcode_s3 { get; set; }

        public int tut_x { get; set; }
        public int tut_y { get; set; }
        public int tut_w { get; set; }
        public int tut_h { get; set; }

        public int icon_x { get; set; }
        public int icon_y { get; set; }
        public int icon_w { get; set; }
        public int icon_h { get; set; }

        public int at_x { get; set; }
        public int at_y { get; set; }
        public int at_w { get; set; }
        public int at_h { get; set; }

        public int orcnumber_x { get; set; }
        public int orcnumber_y { get; set; }
        public int orcnumber_w { get; set; }
        public int orcnumber_h { get; set; }
        public int orcnumber_s1 { get; set; }

        public double camre_exptime { get; set; }
        public double camre_gain { get; set; }
        public double camre_rb { get; set; }
        public double camre_gb { get; set; }
        public double camre_bb { get; set; }


        public int lever_step { get; set; }
        public int lever_umax { get; set; }
        public int lever_umin { get; set; }
        public double lever_ucv { get; set; }
        public double lever_maxucv { get; set; }


        public int lever_dmax { get; set; }
        public int lever_dmin { get; set; }
        public double lever_dcv { get; set; }
        public double lever_maxdcv { get; set; }

        public int tube_tw { get; set; }
        public int tube_ew { get; set; }
        public int tube_bh { get; set; }
        public int tube_y { get; set; }

        public int inspection_step { get; set; }
        public int inspection_maxarea { get; set; }
        public int inspection_minarea { get; set; }
        public double inspection_raio { get; set; }
        public int inspection_count { get; set; }
        public double inspection_pixscore { get; set; }

        public double yhmin { get; set; }
        public double yhmax { get; set; }
        public double ysmin { get; set; }
        public double ysmax { get; set; }
        public double yvmin { get; set; }
        public double yvmax { get; set; }

        public double bhmin { get; set; }
        public double bhmax { get; set; }
        public double bsmin { get; set; }
        public double bsmax { get; set; }
        public double bvmin { get; set; }
        public double bvmax { get; set; }

        public double nhmin { get; set; }
        public double nhmax { get; set; }
        public double nsmin { get; set; }
        public double nsmax { get; set; }
        public double nvmin { get; set; }
        public double nvmax { get; set; }

        public double lnhmin { get; set; }
        public double lnhmax { get; set; }
        public double lnsmin { get; set; }
        public double lnsmax { get; set; }
        public double lnvmin { get; set; }
        public double lnvmax { get; set; }

        public int label_minw { get; set; }
        public int label_maxw { get; set; }


        public bool is_space { get; set; }
        public bool is_labelfilp{ get; set; }
        public bool is_uinspection { get; set; }
        public bool is_dinspection { get; set; }
        public bool is_onedcode { get; set; }
        public bool is_color { get; set; }
        public bool is_inspection { get; set; }
        public bool is_savedata { get; set; }

        public bool is_show_space { get; set; }
        public bool is_show_uinspection { get; set; }
        public bool is_show_dinspection { get; set; }

        public int tbc1 { get; set; }
        public int tbc2 { get; set; }
        public int tbc3 { get; set; }
        public int tbc4 { get; set; }
        public int tbc5 { get; set; }
        public int tbc6 { get; set; }
        public int tbc7 { get; set; }
        public int tbc8 { get; set; }

        public int word_x { get; set; }
        public int word_y { get; set; }
        public int word_space { get; set; }
        public int word_w { get; set; }
        public int word_h { get; set; }

        public void Save()
        {
            string jsonData = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText("parameter"+ID+".txt", jsonData, Encoding.UTF8);
        }
        public BloodParameter Read()
        {
            string jsonData = System.IO.File.ReadAllText("parameter" + ID + ".txt");
            BloodParameter descJson = JsonConvert.DeserializeObject<BloodParameter>(jsonData);//反序列化
            return descJson;
        }
    }

    public class BloodObjs
    {
        public int miny { get; set; }
        public int maxy { get; set; }
        public int minx { get; set; }
        public int maxx { get; set; }
        public int leverdh { get; set; }//下液体厚度
        public double color_h { get; set; }
        public double color_s { get; set; }
        public double color_v { get; set; }
        public int color { get; set; }
        public double[] bbox { get; set; } = new double[4];
        public int label { get; set; }
        public double score { get; set; }
        public bool is_inspection_ok { get; set; }//杂质
        public bool is_ulever_ok { get; set; }//上清液
        public bool is_dlever_ok { get; set; }//下清液
        public List<InspectionRect> foreigs { get; set; } = new List<InspectionRect>();
        [Newtonsoft.Json.JsonIgnore()]
        public Mat img = new Mat();
        public OpenCvSharp.Rect colorarea { get; set; } = new OpenCvSharp.Rect();
        public string infomation { get; set; } = "";
        public bool isok { get; set; }
    }
    public class BloodCardOrcNumber
    {
        public List<int> numbers = new List<int>();
        public string numberstr;
        [Newtonsoft.Json.JsonIgnore()]
        public Mat full_img = new Mat();
        public Mat number_img = new Mat();
        public void Updata()
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numberstr = numberstr + numbers[i];
            }
        }
    }
    public class BloodCardAnalysed
    {
        public BloodCardAnalysed(int id)
        {
            ID = id;
            bloodParameter = new BloodParameter(ID);
            bloodParameter = bloodParameter.Read();
        }
        public int ID = 0;
        public string filename { get; set; } = "";
        public bool isfinish { get; set; } = true;
        public bool isshow { get; set; } = true;
        public string code { get; set; }
        public int[] coderect { get; set; } = new int[4];
        public int space1 { get; set; }
        public int space2 { get; set; }
        public int space3 { get; set; }
        public int space4 { get; set; }
        public int space5 { get; set; }
        public int space6 { get; set; }
        public bool is_iocn  { get; set; } = false;
        public double upliqcv { get; set; } = 0;
        public double downliqcv { get; set; } = 0;
        public double upliqmaxcv { get; set; } = 0;
        public double downliqmaxcv { get; set; } = 0;
        public bool is_space_ok { get; set; }//标签距离
        public bool is_labelfilp_ok { get; set; } = false;//是否标反转
        public bool is_onedcode_ok { get; set; }//一维码
        public bool is_all_ok { get; set; }
        public BloodParameter bloodParameter;
        public List<bool> is_inspection_ok { get; set; } = new List<bool>();//杂质
        public List<bool> is_ulever_ok { get; set; } = new List<bool>();//上清液
        public List<bool> is_dlever_ok { get; set; } = new List<bool>();//下清液

        public bool is_ulevercv_ok { get; set; } = false;//上清液
        public bool is_dlevercv_ok { get; set; } = false;//下清液
        public bool is_ulevermaxcv_ok { get; set; } = false;//上清液
        public bool is_dlevermaxcv_ok { get; set; } = false;//下清液
      

        public List<bool> is_color_ok { get; set; } = new List<bool>();//颜色
        public List<bool> is_tube_ok { get; set; } = new List<bool>();//每个管的综合判断
        public OneDCodeData one_d_code_data = new OneDCodeData();
        [Newtonsoft.Json.JsonIgnore()]
        public Mat img;
        public BloodCardOrcNumber orcnumber { get; set; } = new BloodCardOrcNumber();
        public List<BloodObjs> tobjs { get; set; } = new List<BloodObjs>();
        public bool isok { get; set; }
        public void Save(string filenametem)
        {
            string jsonData = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(filenametem, jsonData, Encoding.UTF8);
        }
        public BloodCardAnalysed Read(string filenametem)
        {
            string jsonData = System.IO.File.ReadAllText(filenametem);
            BloodCardAnalysed descJson = JsonConvert.DeserializeObject<BloodCardAnalysed>(jsonData);//反序列化
            return descJson;
        }
        public void Explain(Mat img)
        {
            if (img == null) return;
            tobjs.Clear();
            is_inspection_ok.Clear();
            is_ulever_ok.Clear();
            is_dlever_ok.Clear();
            is_tube_ok.Clear();
            is_color_ok.Clear();
           
            string[] colorstr = { "错色", "黄色", "蓝色", "无色" };
            List<Mat> retimgs = new List<Mat>();
            if (bloodParameter.is_onedcode)
                orcnumber = Tools.GetOrcNumber(img, bloodParameter.orcnumber_x, bloodParameter.orcnumber_y, bloodParameter.orcnumber_w, bloodParameter.orcnumber_h, bloodParameter.orcnumber_s1, bloodParameter.onedcode_s2,ID);
            var imgs = Tools.MyMatchTemplate(img,new Rect(bloodParameter.tut_x, bloodParameter.tut_y, bloodParameter.tut_w, bloodParameter.tut_h), "template.jpg", 0.4f, bloodParameter.tube_y);
            List<OpenCvSharp.Rect> arect_list = new List<Rect>();
            is_iocn = false;
            var aimgs = Tools.MyMatchTemplate(img,new Rect(bloodParameter.at_x, bloodParameter.at_y, bloodParameter.at_w, bloodParameter.at_h), "a.jpg", 0.5f, 0, arect_list);
            if (aimgs.Count == 0)
            {
                aimgs = Tools.MyMatchTemplate(img ,new Rect(bloodParameter.icon_x, bloodParameter.icon_y, bloodParameter.icon_w, bloodParameter.icon_h), "icon.jpg", 0.5f, 0);
                is_iocn = true;
            }

            if (bloodParameter.is_onedcode)
                one_d_code_data = Tools.GetOneDCode(img, bloodParameter.onedcode_x, bloodParameter.onedcode_y, bloodParameter.onedcode_w, bloodParameter.onedcode_h, bloodParameter.onedcode_s1, bloodParameter.onedcode_s2);
            if (imgs.Count < 8)
            {
                Console.WriteLine("模板未检测:"+ imgs.Count);
                return;
            }
            var color = Tools.ColorSort(imgs, imgs[0].Width / 2 - 25, imgs[0].Height / 2 - 25, 50, 50,
            bloodParameter.yhmin, bloodParameter.yhmax, bloodParameter.ysmin, bloodParameter.ysmax, bloodParameter.yvmin, bloodParameter.yvmax,
            bloodParameter.bhmin, bloodParameter.bhmax, bloodParameter.bsmin, bloodParameter.bsmax, bloodParameter.bvmin, bloodParameter.bvmax,
            bloodParameter.lnhmin, bloodParameter.lnhmax, bloodParameter.lnsmin, bloodParameter.lnsmax, bloodParameter.lnvmin, bloodParameter.lnvmax);
            foreach (var item in imgs)
            {
                Cv2.Resize(item, item, new OpenCvSharp.Size(64, 128));
                Cv2.CvtColor(item, item, ColorConversionCodes.BGR2GRAY);
            }
            foreach (var item in aimgs)
            {
                Cv2.CvtColor(item, item, ColorConversionCodes.BGR2GRAY);
            }

            var liquilevel = new List<List<int>>();
            var foregrects = new List<List<InspectionRect>>();
            upliqcv = 0;
            downliqcv = 0;
            upliqmaxcv = 0;
            downliqmaxcv = 0;
            is_ulevercv_ok = true;
            is_dlevercv_ok = true;
            is_ulevermaxcv_ok = true;
            is_dlevermaxcv_ok = true;
            if (bloodParameter.is_uinspection|| bloodParameter.is_dinspection)
            {
                liquilevel = Tools.LiquidLevel(imgs, bloodParameter.lever_step, 0, 0, imgs[0].Width, (int)(imgs[0].Height * 0.7f));
                upliqcv = Tools.BianYiXiShu(liquilevel,0);
                upliqmaxcv = Tools.Extremal(liquilevel, 0);
                downliqcv = Tools.BianYiXiShu(liquilevel, 1);
                downliqmaxcv = Tools.Extremal(liquilevel, 1);

                is_ulevercv_ok = upliqcv <= bloodParameter.lever_ucv;
                is_dlevercv_ok = downliqcv <= bloodParameter.lever_dcv;
                is_ulevermaxcv_ok = upliqmaxcv <= bloodParameter.lever_maxucv;
                is_dlevermaxcv_ok = downliqmaxcv <= bloodParameter.lever_maxdcv;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    liquilevel.Add(new List<int>());
                    liquilevel[i].Add(0);
                    liquilevel[i].Add(0);
                }
            }
            if (bloodParameter.is_inspection)
            {
                foregrects = Tools.ForeignInspection(imgs, liquilevel, ref retimgs, bloodParameter.inspection_count, bloodParameter.inspection_minarea,ID);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    foregrects.Add(new List<InspectionRect>());
                }
            }

            List<int> space = new List<int>();
            if (bloodParameter.is_space)
            space = Tools.LabelSpace(img, bloodParameter.label_x, bloodParameter.label_y, bloodParameter.label_w, bloodParameter.label_h, bloodParameter.label_s1, bloodParameter.label_s2);
            if(space.Count!=0)
            {
                space1 = space[0];
                space2 = space[1];
                space3 = space[2];
                space4 = space[3];
                space5 = space[4];
                space6 = space[5];
            }
            for (int i = 0; i < imgs.Count; i++)
            {
                tobjs.Add(new BloodObjs());
                var tube = tobjs[tobjs.Count - 1];
                if (bloodParameter.is_show_space)
                {
                    Mat notimgs = new Mat();
                    Mat nimg = new Mat();
                    Cv2.BitwiseNot(retimgs[i], notimgs);
                    Cv2.BitwiseNot(imgs[i], nimg, notimgs);
                    tube.img = nimg;
                }
                else
                    tube.img = imgs[i];
                tube.miny = liquilevel[i][0];
                tube.maxy = liquilevel[i][1];
                tube.foreigs = foregrects[i];
                tube.color = (int)color[i][0];
                tube.color_h = color[i][1];
                tube.color_s = color[i][2];
                tube.color_v = color[i][3];
                tube.infomation = colorstr[tube.color] + "\nh:" + String.Format("{0:F}", tube.color_h) + "\ns:" + String.Format("{0:F}", tube.color_s) + "\nv:" + String.Format("{0:F}", tube.color_v);
            }

            for (int i = 0; i < aimgs.Count; i++)
            {
                tobjs.Add(new BloodObjs());
                var tube = tobjs[tobjs.Count - 1];
                tube.img = aimgs[i];
            }
            //标签
            is_space_ok = bloodParameter.is_space==false|| aimgs.Count != 0&&(space1 >= bloodParameter.label_minw && space1 <= bloodParameter.label_maxw && space2 >= bloodParameter.label_minw && space2 <= bloodParameter.label_maxw);
            //标签是否反转
            is_labelfilp_ok = bloodParameter.is_labelfilp == false || is_iocn ? tobjs[0].color != 2 : tobjs[0].color == 2;
            //一维码判断
            is_onedcode_ok = bloodParameter.is_onedcode==false|| is_iocn || orcnumber.numberstr == one_d_code_data.code;
            //杂质判断
            for (int i = 0; i < foregrects.Count; i++)
            {
                is_inspection_ok.Add(foregrects[i].Count == 0);
            }
            //上清液
            for (int i = 0; i < liquilevel.Count; i++)
            {
                is_ulever_ok.Add(liquilevel[i][0] >= bloodParameter.lever_umin && liquilevel[i][0] <= bloodParameter.lever_umax);
            }
            //下清液
            for (int i = 0; i < liquilevel.Count; i++)
            {
                is_dlever_ok.Add(liquilevel[i][1] >= bloodParameter.lever_dmin && liquilevel[i][1] <= bloodParameter.lever_dmax);
            }
            //颜色
            int[] tube_colors = {
                bloodParameter.tbc1, bloodParameter.tbc2, bloodParameter.tbc3, bloodParameter.tbc4,
                bloodParameter.tbc5, bloodParameter.tbc6, bloodParameter.tbc7, bloodParameter.tbc8 };
            for (int i = 0; i < tube_colors.Count(); i++)
            {
                is_color_ok.Add(bloodParameter.is_color==false||tobjs[i].color == tube_colors[i]);
            }
            //综合判断
            for (int i = 0; i < imgs.Count; i++)
            {
                is_tube_ok.Add((bloodParameter.is_inspection == false || is_inspection_ok[i])&&
                    (bloodParameter.is_uinspection == false || (is_ulever_ok[i]&&is_ulevercv_ok&&is_ulevermaxcv_ok)) &&
                    (bloodParameter.is_dinspection == false || (is_dlever_ok[i]&&is_dlevercv_ok&&is_dlevermaxcv_ok))&&
                    (bloodParameter.is_color == false || is_color_ok[i]));
            }
            //综合判断
            is_all_ok = is_space_ok && is_labelfilp_ok && is_onedcode_ok;
            for (int i = 0; i < is_tube_ok.Count; i++)
            {
                is_all_ok = is_all_ok && is_tube_ok[i];
            }
        }
    }
}