using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using GxIAPINET;
using OpenCvSharp;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using DaHenCameraEnum;

namespace DaHenCamera
{
    public class DhCamera
    {
        public int camera_id = 0;
        public string camera_sn = "";
        public IGXDevice objDevice = null;
        public IGXStream objIGXStream = null;
        public IGXFeatureControl objIGXFeatureControl = null;
        public Mat camera_img = null;
        public int last_exposuretime = 0;
        public static List<IGXDeviceInfo> listGXDeviceInfo = new List<IGXDeviceInfo>();
        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        ~DhCamera()
        {
            Close();
            InitDhSdk();
        }
        public DhCamera(int index)
        {
            camera_id = index;
        }
        public static void InitDhSdk()
        {
            IGXFactory.GetInstance().Init();
            //枚举超时时间假设为200ms
            listGXDeviceInfo.Clear();
            IGXFactory.GetInstance().UpdateAllDeviceList(200, listGXDeviceInfo);
            foreach (IGXDeviceInfo objDeviceInfo in listGXDeviceInfo)
            {
                Console.WriteLine(objDeviceInfo.GetModelName());
                Console.WriteLine(objDeviceInfo.GetVendorName());
            }
        }

        public static void UninitDhSdk()
        {
            IGXFactory.GetInstance().Uninit();
        }

        public static List<IGXDeviceInfo> GetCameraList()
        {
            return listGXDeviceInfo;
        }

        public void SetExposureTime(int ExposureTime)
        {
            if (last_exposuretime != ExposureTime)
            {
                objIGXFeatureControl.GetFloatFeature("ExposureTime").SetValue(ExposureTime);
                last_exposuretime = ExposureTime;
            }
        }

        public bool Open()
        {
            if (listGXDeviceInfo.Count > 0 && objDevice == null)
            {
                if(camera_id< listGXDeviceInfo.Count)
                {
                    camera_sn = listGXDeviceInfo[camera_id].GetSN();
                    objDevice = IGXFactory.GetInstance().OpenDeviceBySN(camera_sn, GxIAPINET.GX_ACCESS_MODE.GX_ACCESS_EXCLUSIVE);
                    objIGXStream = objDevice.OpenStream(0);
                    objIGXFeatureControl = objDevice.GetRemoteFeatureControl();
                    objIGXStream.RegisterCaptureCallback(objDevice, OnFrameCallbackFun);
                    objIGXStream.StartGrab();
                }
                else
                {
                    Console.WriteLine("相机初始化失败!");
                    return false;
                }
                return true;
            }
            return false;
        }

        public void Close()
        {
            if (objDevice != null)
            {
                objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();
                objIGXStream.StopGrab();
                objIGXStream.UnregisterCaptureCallback();
                objIGXStream.Close();
                objDevice.Close();
            }
        }

        public static BitmapSource OpenCvImgToImageSoure(Mat img)
        {
            if (img == null || img.Width == 0 || img.Height == 0) return null;
            var pic = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
            IntPtr ip = pic.GetHbitmap();
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);
            return bitmapSource;
        }

        public Mat TakeImage()
        {
            int fail_count = 0;
            camera_img = null;
            objIGXFeatureControl.GetCommandFeature("AcquisitionStart").Execute();
            while (camera_img == null)
            {
                fail_count++;
                Thread.Sleep(10);
                if(fail_count>100)
                {
                    objIGXFeatureControl.GetCommandFeature("AcquisitionStart").Execute();
                    fail_count = 0;
                }
            }
            return camera_img;
        }

        public Mat TakeImage(double time)
        {
            SetExposureTime(time);
            return TakeImage();
        }

        public BitmapSource GetImageSoure()
        {
            return OpenCvImgToImageSoure(camera_img);
        }

        public void OnFrameCallbackFun(object obj, IFrameData objIFrameData)
        {
            if (objIFrameData.GetStatus() == GxIAPINET.GX_FRAME_STATUS_LIST.GX_FRAME_STATUS_SUCCESS)
            {
                Mat img = null;
                IntPtr pRGB24Buffer = IntPtr.Zero;
                int size = (int)objIFrameData.GetHeight() * (int)objIFrameData.GetWidth() * 3;
                byte[] imgdata = new byte[size];
                pRGB24Buffer = objIFrameData.ConvertToRGB24(GxIAPINET.GX_VALID_BIT_LIST.GX_BIT_0_7, GxIAPINET.GX_BAYER_CONVERT_TYPE_LIST.GX_RAW2RGB_NEIGHBOUR, true);
                Marshal.Copy(pRGB24Buffer, imgdata, 0, imgdata.Length);
                img = new Mat((int)objIFrameData.GetHeight(), (int)objIFrameData.GetWidth(), MatType.CV_8UC3, imgdata);
                Cv2.Flip(img, img, 0);
                camera_img = img;
                objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();
                objIGXStream.FlushQueue();
            }
        }


        public void SetExposureTime(double time)
        {
            objIGXFeatureControl.GetFloatFeature("ExposureTime").SetValue(time);
        }

        public double GetMinExposureTime()
        {
            if (null != objIGXFeatureControl)
            {
                return objIGXFeatureControl.GetFloatFeature("ExposureTime").GetMin();
            }
            return 10;
        }

        public double GetMaxExposureTime()
        {
            if (null != objIGXFeatureControl)
            {
                return objIGXFeatureControl.GetFloatFeature("ExposureTime").GetMax();
            }
            return 5000;
        }
        public void SetGain(double gain)
        {
            objIGXFeatureControl.GetFloatFeature("Gain").SetValue(gain);
        }

        public double GetMinGain()
        {
            return objIGXFeatureControl.GetFloatFeature("Gain").GetMin();
        }

        public double GetMaxGain()
        {
            return objIGXFeatureControl.GetFloatFeature("Gain").GetMax();
        }

        public double GetExposureTime()
        {
            return objIGXFeatureControl.GetFloatFeature("ExposureTime").GetValue();
        }

        public double GetGain()
        {
            return objIGXFeatureControl.GetFloatFeature("Gain").GetValue();
        }

        private void SetBalanceWhiteChanel(BalanceWhiteChanelEnum balanceWhiteChanelEnum)
        {
            String val = "";
            if (balanceWhiteChanelEnum == BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_BLUE)
            {
                val = "Blue";
            }
            else if (balanceWhiteChanelEnum == BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_GREEN)
            {
                val = "Green";
            }
            else if (balanceWhiteChanelEnum == BalanceWhiteChanelEnum.BALANCE_RATIO_SELECTOR_RED)
            {
                val = "Red";
            }
            objIGXFeatureControl.GetEnumFeature("BalanceRatioSelector").SetValue(val);
        }

        public void SetBalanceWhiteAuto(BalanceWhiteAutoStatusEnum balanceWhiteAutoStatusEnum)
        {
            String val = "Off";
            if (balanceWhiteAutoStatusEnum == BalanceWhiteAutoStatusEnum.BALANCE_WHITE_AUTO_OFF)
            {
                val = "Off";
            }
            else if (balanceWhiteAutoStatusEnum == BalanceWhiteAutoStatusEnum.BALANCE_WHITE_AUTO_CONTINUOUS)
            {
                val = "Continuous";
            }
            else if (balanceWhiteAutoStatusEnum == BalanceWhiteAutoStatusEnum.BALANCE_WHITE_AUTO_ONCE)
            {
                val = "Once";
            }
            objIGXFeatureControl.GetEnumFeature("BalanceWhiteAuto").SetValue(val);
        }

        public void SetBalanceRatio(BalanceWhiteChanelEnum balanceWhiteChanelEnum, double val)
        {
            SetBalanceWhiteChanel(balanceWhiteChanelEnum);
            objIGXFeatureControl.GetFloatFeature("BalanceRatio").SetValue(val);
        }

        public double GetBalanceRatio(BalanceWhiteChanelEnum balanceWhiteChanelEnum)
        {
            SetBalanceWhiteChanel(balanceWhiteChanelEnum);
            return objIGXFeatureControl.GetFloatFeature("BalanceRatio").GetValue();
        }

        public double GetMinBalanceRatio(BalanceWhiteChanelEnum balanceWhiteChanelEnum)
        {
            SetBalanceWhiteChanel(balanceWhiteChanelEnum);
            return objIGXFeatureControl.GetFloatFeature("BalanceRatio").GetMin();
        }

        public double GetMaxBalanceRatio(BalanceWhiteChanelEnum balanceWhiteChanelEnum)
        {
            SetBalanceWhiteChanel(balanceWhiteChanelEnum);
            return objIGXFeatureControl.GetFloatFeature("BalanceRatio").GetMax();
        }

        public string[] SupportResolution()
        {
            return new String[] { "2048(H) x 1536(V)" };
        }


    }
}
