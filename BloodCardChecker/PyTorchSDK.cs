using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenCvSharp;
using Spire.Barcode;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BloodCardChecker
{
    unsafe public partial class PyTorchSDK
    {
        [DllImport("PytorchSdk.dll", EntryPoint = "CreateModel")]
        public static extern IntPtr CreateModel(int w, int h);
        [DllImport("PytorchSdk.dll", EntryPoint = "LoadModel")]
        public static extern bool LoadModel(IntPtr model,string filename);
        [DllImport("PytorchSdk.dll", EntryPoint = "ImRead")]
        public static extern IntPtr ImRead(IntPtr model,string filename);
        [DllImport("PytorchSdk.dll", EntryPoint = "AddImage")]
        public static extern bool AddImage(IntPtr model,IntPtr data);
        [DllImport("PytorchSdk.dll", EntryPoint = "Test")]
        public static extern bool Test(IntPtr model,bool is_orc);
        [DllImport("PytorchSdk.dll", EntryPoint = "GetOutPut")]
        public static extern IntPtr GetOutPut(IntPtr model);
        [DllImport("PytorchSdk.dll", EntryPoint = "GetOutPutInt")]
        public static extern int GetOutPutInt(IntPtr model);


    }
}