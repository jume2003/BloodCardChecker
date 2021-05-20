using OpenCvSharp;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BloodCardChecker
{
    /// <summary>
    /// BloodCardShow.xaml 的交互逻辑
    /// </summary>
    public partial class BloodCardShow : UserControl
    {
        public BloodCardShow()
        {
            InitializeComponent();
        }

        public void UpDataUI(BloodCardAnalysed booldcardinfo)
        {
            var img = booldcardinfo.img;
            if (img == null) return;
            Image[] tubes = { tube1, tube2, tube3, tube4, tube5, tube6, tube7, tube8 };
            Image[] words = { word1, word2, word3, word4, word5, word6, word7, word8 };
            Label[] tubeokngs = { tubeokng1, tubeokng2, tubeokng3, tubeokng4, tubeokng5, tubeokng6, tubeokng7, tubeokng8 };
            bool is_label_w_ok = booldcardinfo.space1 > booldcardinfo.bloodParameter.label_minw && booldcardinfo.space1 < booldcardinfo.bloodParameter.label_maxw && booldcardinfo.space2 > booldcardinfo.bloodParameter.label_minw && booldcardinfo.space2 < booldcardinfo.bloodParameter.label_maxw;
            labelonedcode.Content = booldcardinfo.one_d_code_data.code;

            numberocrimg.Source = Tools.OpenCvImgToImageSoure(booldcardinfo.orcnumber.number_img);
            labelspace.Content = booldcardinfo.orcnumber.numberstr;
            onedcodeimg.Source = Tools.OpenCvImgToImageSoure(booldcardinfo.one_d_code_data.code_img);
            booldcardinfo.img.Line(booldcardinfo.space3, booldcardinfo.bloodParameter.label_y, booldcardinfo.space4, booldcardinfo.bloodParameter.label_y, new Scalar(0, 0, 255), 2);
            booldcardinfo.img.Line(booldcardinfo.space3,booldcardinfo.bloodParameter.label_y+30, booldcardinfo.space3,booldcardinfo.bloodParameter.label_y- 30, new Scalar(0, 0, 255), 2);
            booldcardinfo.img.Line(booldcardinfo.space4,booldcardinfo.bloodParameter.label_y+ 30, booldcardinfo.space4,booldcardinfo.bloodParameter.label_y- 30, new Scalar(0, 0, 255), 2);
            Cv2.PutText(booldcardinfo.img, booldcardinfo.space1.ToString(), new OpenCvSharp.Point(booldcardinfo.space4- booldcardinfo.space1/2,booldcardinfo.bloodParameter.label_y-50), HersheyFonts.HersheySimplex, 2, new Scalar(255, 255, 255),3);


            booldcardinfo.img.Line(booldcardinfo.space5,booldcardinfo.bloodParameter.label_y, booldcardinfo.space6,booldcardinfo.bloodParameter.label_y, new Scalar(0, 0, 255), 2);
            booldcardinfo.img.Line(booldcardinfo.space5,booldcardinfo.bloodParameter.label_y + 30, booldcardinfo.space5,booldcardinfo.bloodParameter.label_y - 30, new Scalar(0, 0, 255), 2);
            booldcardinfo.img.Line(booldcardinfo.space6,booldcardinfo.bloodParameter.label_y + 30, booldcardinfo.space6,booldcardinfo.bloodParameter.label_y - 30, new Scalar(0, 0, 255), 2);
            Cv2.PutText(booldcardinfo.img, booldcardinfo.space2.ToString(), new OpenCvSharp.Point(booldcardinfo.space5- booldcardinfo.space2 / 2,booldcardinfo.bloodParameter.label_y-50), HersheyFonts.HersheySimplex,2, new Scalar(255, 255, 255),3);

            for (int i = 0; i < 8; i++)
            {
                tubes[i].Source = null;
                words[i].Source = null;
            }
            int tubecount = 0;
            int wordcout = 0;
            for (int i = 0; i < booldcardinfo.tobjs.Count; i++)
            {
                var tube = booldcardinfo.tobjs[i];
                int label = tube.label;
                var cutimg = tube.img;
                if (cutimg.Width != 0 && cutimg.Height != 0)
                {
                    if(i<8)
                    {
                        Cv2.CvtColor(cutimg, cutimg, ColorConversionCodes.GRAY2BGR);
                        cutimg.Line(cutimg.Width / 2, tube.miny, cutimg.Width / 2, tube.maxy, new Scalar(0, 0, 255), 1);
                        cutimg.Line(cutimg.Width / 2 - 10, tube.miny, cutimg.Width / 2 + 10, tube.miny, new Scalar(0, 255, 0), 1);
                        cutimg.Line(cutimg.Width / 2 - 10, tube.maxy, cutimg.Width / 2 + 10, tube.maxy, new Scalar(0, 255, 0), 1);
                        int pixh = tube.maxy - tube.miny;
                        Cv2.PutText(cutimg, tube.miny.ToString(), new OpenCvSharp.Point(cutimg.Width / 2 + 15, tube.miny + 5), HersheyFonts.HersheySimplex, 0.3, new Scalar(0, 0, 0));
                        Cv2.PutText(cutimg, pixh.ToString(), new OpenCvSharp.Point(cutimg.Width / 2 + 15, tube.miny + pixh / 2 + 5), HersheyFonts.HersheySimplex, 0.3, new Scalar(0, 0, 0));
                        Cv2.PutText(cutimg, tube.maxy.ToString(), new OpenCvSharp.Point(cutimg.Width / 2 + 15, tube.maxy + 5), HersheyFonts.HersheySimplex, 0.3, new Scalar(0, 0, 0));
                        for (int j = 0; j < tube.foreigs.Count; j++)
                        {
                            if (booldcardinfo.bloodParameter.is_show_space == false)
                            {
                                cutimg.Rectangle(tube.foreigs[j].rect, new Scalar(0, 0, 255), 1);
                                Cv2.PutText(cutimg, tube.foreigs[j].score.ToString(), new OpenCvSharp.Point(tube.foreigs[j].rect.X + 10, tube.foreigs[j].rect.Y + tube.foreigs[j].rect.Height), HersheyFonts.HersheySimplex, 0.3, new Scalar(255, 255, 255));

                            }
                        }

                        tubes[tubecount % 8].Source = Tools.OpenCvImgToImageSoure(cutimg);
                        tubecount++;
                    }
                    else
                    {
                        words[wordcout % 8].Source = Tools.OpenCvImgToImageSoure(cutimg);
                        wordcout++;
                    }
                }
            }
            for (int i = 0; i < booldcardinfo.is_tube_ok.Count;i++)
            {
                string []ok_str = { "Pass", "Fail" };
                int index = booldcardinfo.is_tube_ok[i] ? 0 : 1;
                var tube = booldcardinfo.tobjs[i];
                tubeokngs[i].Content = "结果:"+ok_str[index]+"\n"+
                    "上极:" + booldcardinfo.upliqmaxcv + "," + booldcardinfo.is_ulevermaxcv_ok + "\n" +
                    "下极:" + booldcardinfo.downliqmaxcv + "," + booldcardinfo.is_dlevermaxcv_ok+ "\n" +
                    "上异:" + booldcardinfo.upliqcv.ToString("f2") + "," + booldcardinfo.is_ulevercv_ok+ "\n" +
                    "下异:" + booldcardinfo.downliqcv.ToString("f2") + "," + booldcardinfo.is_dlevercv_ok+ "\n" +
                    "杂质:" + booldcardinfo.is_inspection_ok[i]+"\n"+
                    "上液:" + booldcardinfo.is_ulever_ok[i] + "\n" +
                    "下液:" + booldcardinfo.is_dlever_ok[i] + "\n" +
                    "颜色:" + booldcardinfo.is_color_ok[i] + "\n"+
                    "标签:" + booldcardinfo.is_labelfilp_ok + "\n" +
                    tube.infomation; 

            }
            string[] all_ok_str = { "O\nK", "N\nG" };
            labelisallok.Content = booldcardinfo.is_all_ok ? all_ok_str[0] : all_ok_str[1];


        }
    }
}
