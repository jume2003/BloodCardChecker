using Newtonsoft.Json;
using OpenCvSharp;
using DaHenCamera;
using Spire.Barcode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using DaHenCameraEnum;

namespace BloodCardChecker
{
    public class OneDCodeData
    {
        public string code="";
        public Mat code_img = new Mat();
        public Mat full_img = new Mat();
    }
    public class InspectionRect
    {
        public OpenCvSharp.Rect rect;
        public int score;
        public double pixscore;
        public InspectionRect(OpenCvSharp.Rect recttem,int scoretem, double pixscoretem)
        {
            rect = recttem;
            score = scoretem;
            pixscore = pixscoretem;
        }
        public bool IsClose(InspectionRect recttem)
        {
            int centerleft = Math.Abs((recttem.rect.Left + recttem.rect.Width / 2) - (rect.Left + rect.Width / 2));
            int centertop = Math.Abs((recttem.rect.Top + recttem.rect.Height / 2) - (rect.Top + rect.Height / 2));
            int temrectarea = recttem.rect.Width * recttem.rect.Height;
            int rectarea = rect.Width * rect.Height;
            bool iscenter = (centerleft<5&& centertop<5&& rectarea> temrectarea);
            bool isclose = Math.Abs(recttem.rect.Left - rect.Left) < 5 && Math.Abs(recttem.rect.Top - rect.Top) < 5 && Math.Abs(recttem.rect.Right - rect.Right) < 5 && Math.Abs(recttem.rect.Bottom - rect.Bottom) < 5;
            return isclose|| iscenter;
        }
    }
    public class Tools
    {
        public static Dictionary<string, Mat> template_list = new Dictionary<string, Mat>();
        public static DhCamera[] camera = new DhCamera[2];
        public bool is_init = false;
        public static BloodCardAnalysed[] booldcardinfo = {new BloodCardAnalysed(0), new BloodCardAnalysed(1)};
        public static IntPtr []Unet = new IntPtr[2];
        public static IntPtr []Ocrnet = new IntPtr[2];
        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        ~Tools()
        {
            DhCamera.UninitDhSdk();
        }
        public void SetTimplateImage(string filename)
        {
            if (template_list.ContainsKey(filename) == false)
            {
                var img = Cv2.ImRead(filename, ImreadModes.Grayscale);
                template_list.Add(filename, img);
            }
        }

        public static Mat GetTimplateImage(string filename)
        {
            if(template_list.ContainsKey(filename)==false)
            {
                var img = Cv2.ImRead(filename, ImreadModes.Grayscale);
                template_list.Add(filename, img);
            }
            return template_list[filename];
        }
        public static BitmapSource OpenCvImgToImageSoure(Mat img)
        {
            if (img == null|| img.Width==0|| img.Height==0) return null;
            var pic = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
            IntPtr ip = pic.GetHbitmap();
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);
            return bitmapSource;
        }

        public static Mat BitmapToMat(Bitmap srcbit)
        {
            int iwidth = srcbit.Width;
            int iheight = srcbit.Height;
            int iByte = iwidth * iheight * 3;
            byte[] result = new byte[iByte];
            int step;
            Rectangle rect = new Rectangle(0, 0, iwidth, iheight);
            BitmapData bmpData = srcbit.LockBits(rect, ImageLockMode.ReadWrite, srcbit.PixelFormat);
            IntPtr iPtr = bmpData.Scan0;
            Marshal.Copy(iPtr, result, 0, iByte);
            step = bmpData.Stride;
            srcbit.UnlockBits(bmpData);
            return new Mat(srcbit.Height, srcbit.Width, new MatType(MatType.CV_8UC3), result, step);
        }

        public static Mat MergeImage(Mat img1, Mat img2)
        {
            if (img1.Width == 0 && img1.Width == 0) return img2;
            Mat des = new Mat();
            des.Create(img1.Height, img1.Width + img2.Width, img1.Type());
            Mat r1 = des[new OpenCvSharp.Rect(0, 0, img1.Width, img1.Height)];
            img1.CopyTo(r1);
            Mat r2 = des[new OpenCvSharp.Rect(img1.Width, 0, img2.Width, img1.Height)];
            img2.CopyTo(r2);
            return des;
        }

        public static Mat ReSizeImage(Mat img)
        {
            Mat dstimg = new Mat(new OpenCvSharp.Size(img.Width + 50, img.Height + 50), MatType.CV_8UC1, new Scalar(255, 0, 0));
            Mat roiimg = dstimg[new OpenCvSharp.Rect(25, 25, img.Width, img.Height)];
            img.CopyTo(roiimg);
            return dstimg;
        }

        public bool Init()
        {
            DhCamera.InitDhSdk();
            for (int i = 0; i < camera.Count(); i++)
            {
                camera[i] = new DhCamera(i);
                if (camera[i].Open() == false) return false;
                camera[i].SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_RED, booldcardinfo[i].bloodParameter.camre_rb);
                camera[i].SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_BLUE, booldcardinfo[i].bloodParameter.camre_gb);
                camera[i].SetBalanceRatio(BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_GREEN, booldcardinfo[i].bloodParameter.camre_bb);
                camera[i].SetGain(booldcardinfo[i].bloodParameter.camre_gain);
                camera[i].SetExposureTime(booldcardinfo[i].bloodParameter.onedcode_s3);
                Unet[i] = PyTorchSDK.CreateModel(64, 128);
                PyTorchSDK.LoadModel(Unet[i], "model/unet_gpu.pt");
                Ocrnet[i] = PyTorchSDK.CreateModel(28, 28);
                PyTorchSDK.LoadModel(Ocrnet[i], "model/orc_gpu.pt");
            }
            SetTimplateImage("template.jpg");
            SetTimplateImage("icon.jpg");
            SetTimplateImage("a.jpg");
            SetTimplateImage("mask.jpg");
            return true;
        }

        public static string CheckOrc(List<Mat> imgs,int index)
        {
            string orc_str = "";
            if(imgs.Count!=0 )
            {
                foreach (var img in imgs)
                {
                    PyTorchSDK.AddImage(Ocrnet[index], img.Data);
                }
                PyTorchSDK.Test(Ocrnet[index],true);
                var data = PyTorchSDK.GetOutPutInt(Ocrnet[index]);
                while (data != -1)
                {
                    orc_str += data;
                    data = PyTorchSDK.GetOutPutInt(Ocrnet[index]);
                }
            }
            return orc_str;
        }

        public static OneDCodeData GetOneDCode(Mat img,int x,int y,int w,int h,int s1, int s2)
        {
            img = img.Clone() * (s2 / 1000);
            img = img.CvtColor(ColorConversionCodes.BGR2GRAY);
            img = img.Threshold(s1, 255, ThresholdTypes.Binary);

            OneDCodeData codedata = new OneDCodeData();
            var code = img.SubMat(y, y + h, x,x+w);
            code = code.Resize(new OpenCvSharp.Size(code.Width * 0.4, code.Height * 0.4));

            string[] data = BarcodeScanner.Scan(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(code), BarCodeType.Code128);
            if(data.Length!=0)
            codedata.code =  data[0];
            codedata.code = codedata.code.Replace("-", "");
            codedata.code_img = code;
            codedata.full_img = img;
            return codedata;
        }

        public static BloodCardOrcNumber GetOrcNumber(Mat img, int x, int y, int w, int h, int s1, int s2,int index)
        {
            List<OpenCvSharp.Rect> rects_list = new List<OpenCvSharp.Rect>();
            BloodCardOrcNumber bloodcardorc = new BloodCardOrcNumber();
            img = img.Clone() * (s2 / 1000);
            img = img.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat img_gary = null;
            for (int k=0;k<5;k++)
            {
                rects_list.Clear();
                img_gary = img.Threshold(s1 + (k-2) * 3, 255, ThresholdTypes.Binary);
                img_gary = img_gary.SubMat(y, y + h, x, x + w);
                bloodcardorc.full_img = img_gary.Clone();
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchyIndexes;
                img_gary.FindContours(out contours, out hierarchyIndexes, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
                for (int i = 0; i < contours.Length; i++)
                {
                    var rect = Cv2.BoundingRect(contours[i]);
                    if (rect.Width > 10 && rect.Width < 50 && rect.Height > 10 && rect.Height < 50)
                    {
                        var numberblock = img_gary.SubMat(rect.Top, rect.Bottom, rect.Left, rect.Right);
                        numberblock = numberblock.Resize(new OpenCvSharp.Size(28, 28));
                        var pixmean = numberblock.Mean();
                        if (pixmean.Val0 < 180)
                        rects_list.Add(rect);
                    }
                }
                if (rects_list.Count() == 20) break;
            }


            ////排序
            List<Mat> number_list = new List<Mat>();
            Mat mergeimg = new Mat();
            rects_list.Sort((a, b) => a.Left.CompareTo(b.Left));
            for (int i = 0; i < rects_list.Count; i++)
            {
                var rect = rects_list[i];
                var numberblock = img_gary.SubMat(rect.Top, rect.Bottom, rect.Left, rect.Right);
                numberblock = numberblock.Resize(new OpenCvSharp.Size(28, 28));
                var pixmean = numberblock.Mean();
                if(pixmean.Val0<180)
                {
                    number_list.Add(numberblock);
                    mergeimg = Tools.MergeImage(mergeimg, numberblock);
                }
            }
            if (mergeimg.Width != 0 && mergeimg.Height != 0)
                bloodcardorc.number_img = mergeimg.Clone();
            else
                bloodcardorc.number_img = null;
            bloodcardorc.numberstr = Tools.CheckOrc(number_list, index);
            return bloodcardorc;
        }
        //水平
        public static List<double> HistogramScoresH(Mat img, int step)
        {
            int w = img.Width;
            int h = img.Height;
            int steprow = w / step;
            List<double> scores = new List<double>();
            for (int i = 0; i < steprow; i++)
            {
                var imgtem = img.SubMat(0, h, i * step, (i + 1) * step);
                var score = imgtem.Mean();
                scores.Add(score.Val0);
            }
            return scores;
        }
        //垂直
        public static List<double> HistogramScoresV(Mat img, int step)
        {
            int w = img.Width;
            int h = img.Height;
            int steproh = h / step;
            List<double> scores = new List<double>();
            for (int i = 0; i < steproh; i++)
            {
                var imgtem = img.SubMat(i * step, (i + 1) * step,0,w);
                var score = imgtem.Mean();
                scores.Add(score.Val0);
            }
            return scores;
        }

        public static int UpPoint(List<double> scores,int index, int size)
        {
            double maxx = 0;
            int maxi = 0;
            double maxy = 0;
            for (int i = index; i < scores.Count - size; i++)
            {
                for (int j = 0; j < size - 1; j++)
                {
                    if (scores[i + j] - scores[i] > maxx)
                    {
                        maxx = scores[i + j] - scores[i];
                        maxy = scores[i];
                        maxi = i;
                    }
                }
            }
            return maxi;
        }

        public static int DownPoint(List<double> scores,int index,int size)
        {
            double maxx = 0;
            int maxi = 0;
            double maxy = 0;
            for(int i= index; i<scores.Count-size;i++)
            {
                for (int j = 0; j < size - 1;j++)
                {
                    if(scores[i + j] - scores[i] < maxx)
                    {
                        maxx = scores[i + j] - scores[i]; 
                        maxy = scores[i + j];
                        maxi = i + j;
                    }
                }
            }
            return maxi;
        }

        public static Mat MakeMask(Mat img, int x, int y, int w, int h, int s1)
        {
            img = img.Clone();
            img.FloodFill(new OpenCvSharp.Point(x, y), new Scalar(0));
            img = img.CvtColor(ColorConversionCodes.BGR2GRAY).Threshold(s1, 255, ThresholdTypes.Binary);
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(w, h));
            img = img.MorphologyEx(MorphTypes.Open, kernel);
            img = img.MorphologyEx(MorphTypes.Close, kernel);
            img = img.CvtColor(ColorConversionCodes.GRAY2BGR);
            return img;
        }
        //变异系数
        public static double BianYiXiShu(List<List<int>> values_tem,int index)
        {
            List<int> tubes_c = new List<int>();
            foreach (var item in values_tem)
            {
                tubes_c.Add(item[index]);
            }
            double count = tubes_c.Count();
            double mean = tubes_c.Sum() / count;
            double sd = 0;
            foreach(var item in tubes_c)
            {
                sd+=Math.Pow(item - mean, 2);
            }
            sd = Math.Sqrt(sd / (count-1));
            double cv = sd / mean;
            return cv;
        }
        //极值
        public static double Extremal(List<List<int>> values_tem, int index)
        {
            List<int> tubes_c = new List<int>();
            foreach (var item in values_tem)
            {
                tubes_c.Add(item[index]);
            }
            double maxcv = tubes_c.Max() - tubes_c.Min();
            return maxcv;
        }
        //融合图片
        public static Mat MixImg(Mat imgl, Mat imgh)
        {
            Mat msak1 = new Mat();
            Mat mask = MakeMask(imgh, 685, 685,70,70,10);
            Cv2.BitwiseNot(mask, msak1);
            Mat img = (imgh & mask);
            Mat img1 = (imgl & msak1);
            img = img | img1;
            return img;
        }
        //标签检测
        public static List<int> CheckLabelSpace(Mat img,int step,int x,int y,int w,int h,int s1,int s2)
        {
            img = img.CvtColor(ColorConversionCodes.BGR2GRAY);
            var cutimg = img.SubMat(y, y + h, x, x + w);
            var img1 = cutimg.Threshold(s1, 255, ThresholdTypes.Binary);
            var img2 = cutimg.Threshold(s2, 255, ThresholdTypes.Binary);
            var scores1 = HistogramScoresH(img1, step);
            var scores2 = HistogramScoresH(img2, step);
            int down1 = DownPoint(scores1,0, 5);
            int up1 = UpPoint(scores1,0, 5);
            int down2 = DownPoint(scores2,0, 5);
            int up2 = UpPoint(scores2,0, 5);
            int space1 = (down1 - down2) * step;
            int space2 = (up2 - up1) * step;
            List<int> ret = new List<int>();
            ret.Add(space1);
            ret.Add(space2);
            ret.Add(x+down1*step);
            ret.Add(x+down2*step);
            ret.Add(x + up1 * step);
            ret.Add(x + up2 * step);
            return ret;
        }
        //右左空间
        public static List<int> TubeArea(Mat img,int tubew,int tubeew,int step)
        {
            List<int> ret = new List<int>();
            img = img.SubMat(img.Height / 3, img.Height / 2, 0, img.Width);
            var hvsimg = img.CvtColor(ColorConversionCodes.BGR2GRAY);

            List<double> scores = HistogramScoresH(hvsimg, step);
            //scores = scores.Take(scores.Count/2).ToList();
            int maxil = UpPoint(scores,0, 5);
            int maxir = 0;
            if (maxil > scores.Count / 2) 
            {
                maxir = maxil;
                maxil = maxil- tubew/ step;
            }
            else
            {
                maxir = maxil+ tubew / step;
            }
            maxil = maxil * step + tubeew;
            maxir = maxir * step- tubeew;
            if (maxil < 0) maxil = 0;
            if (maxir < 0) maxir = 0;
            if (maxil > img.Width) maxil = img.Width;
            if (maxir > img.Width) maxir = img.Width;
            ret.Add(maxil);
            ret.Add(maxir);
            return ret;
        }
        //颜色检测
        public static List<List<double>> ColorSort(List<Mat> imgs, int x, int y, int w, int h,
            double yhmin, double yhmax, double ysmin, double ysmax, double yvmin, double yvmax,
            double bhmin, double bhmax, double bsmin, double bsmax, double bvmin, double bvmax,
            double nhmin, double nhmax, double nsmin, double nsmax, double nvmin, double nvmax)
        {
            List<List<double>> ret = new List<List<double>>();
            foreach (var img in imgs)
            {
                ret.Add(new List<double>());
                if (x < 0) x = 0;
                if (y < 0) y = 0;
                int x1 = x + w;
                int y1 = y + h;
                if (x1 >= img.Width) x1 = img.Width - 1;
                if (y1 >= img.Height) x1 = img.Height - 1;
                var cutimg = img.SubMat(y, y1, x, x1);
                cutimg = cutimg.CvtColor(ColorConversionCodes.BGR2HSV);
                var hsvcount = cutimg.Mean();
                double color_h = hsvcount[0] * 2.0;
                double color_s = hsvcount[1] / 255 * 100.0;
                double color_v = hsvcount[2] / 255 * 100.0;

                ret[ret.Count-1].Add(0);
                ret[ret.Count - 1].Add(color_h);
                ret[ret.Count - 1].Add(color_s);
                ret[ret.Count - 1].Add(color_v);

                if (color_h > yhmin && color_h < yhmax &&
                    color_s > ysmin && color_s < ysmax &&
                    color_v > yvmin && color_v < yvmax)
                {
                    ret[ret.Count - 1][0] = 1;
                }
                else if (color_h > bhmin && color_h < bhmax &&
                         color_s > bsmin && color_s < bsmax &&
                         color_v > bvmin && color_v < bvmax)
                {
                    ret[ret.Count - 1][0] = 2;
                }
                else if (color_h > nhmin && color_h < nhmax &&
                         color_s > nsmin && color_s < nsmax &&
                         color_v > nvmin && color_v < nvmax)
                {
                    ret[ret.Count - 1][0] = 3;
                }
            }
            return ret;
        }
        //上下液面检测
        public static List<List<int>> LiquidLevel(List<Mat> imgs, int step,int x,int y,int w,int h)
        {
            List<List<int>> ret = new List<List<int>>();
            foreach (var item in imgs)
            {
                var img = item.SubMat(y, y + h, x, x + w);
                List<double> scores = HistogramScoresV(img, step);
                int maxi = UpPoint(scores, 0, 5);
                int maxi1 = DownPoint(scores, maxi, 5);
                ret.Add(new List<int>());
                ret[ret.Count-1].Add(maxi * step);
                ret[ret.Count - 1].Add(maxi1 * step);
            }
            return ret;
        }
        //模板匹配
        public static List<Mat> MyMatchTemplate(Mat img,OpenCvSharp.Rect img_roi, string templatename,double threshold,int tubey,List<OpenCvSharp.Rect> rect_list=null)
        {
            List<Mat> imgs = new List<Mat>();
            Stopwatch sw = new Stopwatch();
            Mat img_gray = new Mat();
            img = img.Clone();
            img = img.SubMat(img_roi);
            Cv2.CvtColor(img, img_gray, ColorConversionCodes.BGR2GRAY);
            var template = GetTimplateImage(templatename);
            Mat result = new Mat();
            double minVal, maxVal;
            OpenCvSharp.Point minLoc, maxLoc, maxFloc;
            Cv2.MatchTemplate(img_gray, template, result, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);
            maxFloc = maxLoc;
            int count = 0;
            OpenCvSharp.Rect last_rect = new OpenCvSharp.Rect();
            int ww = img.Width / template.Width - 1;
            for (int i = 0; i < ww; i++)
            {
                int px = i * template.Width;
                int py = maxFloc.Y;
                OpenCvSharp.Rect roi = new OpenCvSharp.Rect(px, py, template.Width, template.Height);
                if (roi.Y + roi.Height > result.Height) roi.Height = result.Height - roi.Y;
                Mat RoiResult = new Mat(result, roi);
                Cv2.MinMaxLoc(RoiResult, out minVal, out maxVal, out minLoc, out maxLoc);//查找极值
                var rect = new OpenCvSharp.Rect(px + maxLoc.X, py + maxLoc.Y, template.Width, template.Height);
                if (maxVal > threshold && Math.Abs(last_rect.X - rect.X) > 50)
                {
                    count++;
                    if(tubey!=0)
                    rect.Y = tubey- img_roi.Y;
                    Mat retimg = new Mat(img, rect);
                    if(rect_list!=null)
                    rect_list.Add(rect);
                    last_rect = rect;
                    imgs.Add(retimg);
                    if (count >= 8) break;
                }
            }
            return imgs;
        }
        //杂质检测
        public static List<List<InspectionRect>> ForeignInspection(List<Mat> imgs,List<List<int>> liquilevel,ref List<Mat> retimgs,int inspection_count,int inspection_minarea,int netindex)
        {
            List<List<InspectionRect>> rectlist = new List<List<InspectionRect>>();
            Mat mask = GetTimplateImage("mask.jpg");
            Cv2.Resize(mask, mask, new OpenCvSharp.Size(64, 128));
            foreach (var item in imgs)
            {
                PyTorchSDK.AddImage(Unet[netindex],item.Data);
            }
            PyTorchSDK.Test(Unet[netindex],false);
            var data = PyTorchSDK.GetOutPut(Unet[netindex]);
            int index = 0;
          
            while (data != IntPtr.Zero)
            {
                Mat img_tem = new Mat(128, 64, MatType.CV_8U, data);
                //要为奇数//膨胀
                //Mat se = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(1, 2), new OpenCvSharp.Point(0, 0));
                //Cv2.Dilate(img_tem, img_tem, se, new OpenCvSharp.Point(-1, -1), 1);
                img_tem = img_tem.Threshold(inspection_count, 255, ThresholdTypes.Binary);
                retimgs.Add(img_tem.Clone());
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchyIndexes;

                Mat img_masked = new Mat();
                img_tem.CopyTo(img_masked, mask);

                img_masked.FindContours(out contours, out hierarchyIndexes, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
                rectlist.Add(new List<InspectionRect>());
                for (int i = 0; i < contours.Length; i++)
                {
                    var rect = Cv2.BoundingRect(contours[i]);
                    if(rect.Width* rect.Height> inspection_minarea&&(rect.Y>liquilevel[index][0]|| rect.Y+rect.Height > liquilevel[index][0]))
                    rectlist[rectlist.Count - 1].Add(new InspectionRect(rect, rect.Width * rect.Height, rect.Width * rect.Height));
                }
                data = PyTorchSDK.GetOutPut(Unet[netindex]);
                index++;
            }
            return rectlist;
        }

        public static List<int> LabelSpace(Mat img, int label_x, int label_y, int label_w, int label_h, int label_s1, int label_s2)
        {
            img = img.Clone();
            var space = Tools.CheckLabelSpace(img, 2,label_x, label_y, label_w,label_h, label_s1,label_s2);
            return space;
        }

        //字母颜色
        public static List<Mat> WordImages(Mat img,int x, int y,int space,int w,int h)
        {
            List<Mat> imgs = new List<Mat>();
            for(int i=0;i<8;i++)
            {
                imgs.Add(img.SubMat(y, y + h, x+i* space, x + w+ i * space));
            }
            return imgs;
        }

        public BloodCardAnalysed Explain(int index)
        {
            var img = camera[index].TakeImage();
            booldcardinfo[index].img = img;
            booldcardinfo[index].Explain(img);
            return booldcardinfo[index];
        }

        public BloodCardAnalysed Explain(int index,Mat img)
        {
            booldcardinfo[index].img = img;
            booldcardinfo[index].Explain(img);
            return booldcardinfo[index];
        }

    }
}
