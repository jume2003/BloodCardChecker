using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ZLG
{
    public class RecSafeData
    {
        public static readonly object Lock = new object();
        public string data;
        public RecSafeData(string datatem)
        {
            data = datatem;
        }
    }

    public class SendSafeData
    {
        public VCI_CAN_OBJ sendobj;
        public string key_str;
        public bool is_wait;
        public int wait_time = 0;
        public SendSafeData(VCI_CAN_OBJ sendobj_tem,bool is_wait_tem, string key_str_tem, int wait_time_tem)
        {
            sendobj = sendobj_tem;
            key_str = key_str_tem;
            is_wait = is_wait_tem;
            wait_time = wait_time_tem;
        }
        public SendSafeData(int wait_time_tem)
        {
            wait_time = wait_time_tem;
        }
    }

    public class ZLGCanComm
    {
        private object IntlLock = new object();
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        //[DllImport("controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);
        public IDictionary<String, int> RegisterMap = new ConcurrentDictionary<String, int>();
        const UInt32 DevType = 3;//VCI_USBCAN1
        const UInt32 DevIndex = 0;
        const UInt32 CanIndex = 0;
        const byte ID = 0x01;
        public bool IsOpen = false;
        public Thread rec_work_thread = null;
        public Thread send_work_thread = null;
        public bool Connect()
        {
            if (!IsOpen)
            {
                IsOpen = VCI_OpenDevice(DevType, DevIndex, CanIndex) != 0;
                VCI_ERR_INFO pErrInfo = new VCI_ERR_INFO();
                VCI_ReadErrInfo(DevType, DevIndex, CanIndex, ref pErrInfo);
                if (IsOpen)
                {
                    VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                    config.AccCode = 0x00;
                    config.AccMask = System.Convert.ToUInt32("0xFFFFFFFF" , 16);
                    config.Timing0 = 0x00;
                    config.Timing1 = 0x1c;
                    config.Filter = 0x01;
                    config.Mode = 0x00;
                    VCI_InitCAN(DevType, DevIndex, CanIndex, ref config);
                    VCI_StartCAN(DevType, DevIndex, CanIndex);
                    VCI_BOARD_INFO br = new VCI_BOARD_INFO();
                    VCI_ReadBoardInfo(DevType, DevIndex, ref br);
                    if (rec_work_thread == null)
                    {
                        rec_work_thread = new Thread(RecWorkThread);
                        rec_work_thread.Start();
                    }
                }
                else
                {
                    Console.WriteLine("Can卡初始化失败!");
                }
            }
            return IsOpen;
        }

        ~ZLGCanComm()
        {
            if (rec_work_thread != null)
            {
                rec_work_thread.Join();
                rec_work_thread.Abort();
            }
            if (send_work_thread != null)
            {
                send_work_thread.Join();
                send_work_thread.Abort();
            }
        }

        public void RecWorkThread()
        {
            while(true)
            {
                try
                {
                    Can_Rec_Tick();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public bool Close()
        {
            uint res = 0;
            if (this.IsOpen)
            {
                res = VCI_CloseDevice(DevType, 0);
                this.IsOpen = res != 0;
            }
            return res == 0;
        }
        public Boolean ClearBuffer()
        {
            if (IsOpen)
            {
                return VCI_ClearBuffer(DevType, DevIndex, CanIndex) != 0;
            }
            else
            {
                return false;
            }
        }
        public unsafe bool Send(byte targetId, byte[] data, byte mask)
        {
            string sendmsg = "";
            bool is_send_ok = false;
            Thread.Sleep(2);
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            string adressstr = string.Format("{0:D2}-{1:x}{2:x}", targetId, data[2], data[3]);

            sendobj.SendType = 0x00;//02 自发自收 //0x00;//正常发送
            sendobj.RemoteFlag = 0x00;//数据帧
            sendobj.ExternFlag = 0x00;//标准帧
            targetId = (byte)((0xFF << 3 | mask) & targetId);
            var canid = targetId << 3 | (mask & 0b111);
            sendobj.ID = System.Convert.ToUInt32(canid);
            int len = data.Length;
            sendobj.DataLen = System.Convert.ToByte(len);

            sendmsg = string.Format("send:0x{0:x4} ", canid);
            for (var j = 0; j < Math.Min(len, 8); j++)
            {
                sendobj.Data[j] = data[j];
                sendmsg += string.Format("0x{0:x2} ", data[j]);
            }
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            sendmsg += "TimeStamp:" + Convert.ToInt64(ts.TotalSeconds).ToString();
            is_send_ok = VCI_Transmit(DevType, DevIndex, CanIndex, ref sendobj, 1) != 0;
            return is_send_ok;
        }

        unsafe public void Can_Rec_Tick()
        {
            UInt32 res = new UInt32();
            res = VCI_GetReceiveNum(DevType, DevIndex, CanIndex);
            if (res == 0) return;
            /////////////////////////////////////
            UInt32 con_maxlen = 100;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);
            res = VCI_Receive(DevType, DevIndex, CanIndex, pt, con_maxlen, 5);
            ////////////////////////////////////////////////////////
            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ? reviceObj = null;
                try
                {
                    reviceObj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)(pt.ToInt64() + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (!reviceObj.HasValue)
                {
                    break;
                }
                var obj = reviceObj.Value;
                var targetID = (byte)(obj.ID >> 3);
                var mask = (byte)(obj.ID & 0b111);
                Console.Write("rec: 0x{0:x4} ", obj.ID);
                for (var j = 0; j < obj.DataLen; j++)
                {
                    Console.Write("0x{0:x2} ", obj.Data[j]);
                }
                Console.WriteLine("");
                if (targetID != (byte)(ID & (0xFF << 3 | mask)))
                {
                    Console.WriteLine("ID不符，数据丢弃");
                    continue;
                }
                if (obj.RemoteFlag == 0)
                {
                    byte len = (byte)(Math.Min(obj.DataLen % 50, 8));
                    byte[] data = new byte[len];
                    for (byte j = 0; j < len; j++)
                    {
                        data[j] = obj.Data[j];
                    }
                    string CanAddress = "";
                    int FunCode = 0;
                    ParseCanData(ref CanAddress, ref FunCode, data);
                    switch (FunCode)
                    {
                        case 0x03:
                            {
                                var val = ByteUtil.BytesToInt(data[4], data[5], data[6], data[7]);
                                SetInt(CanAddress, val);
                                break;
                            }
                    }
                }
                Marshal.FreeHGlobal(pt);
            }
        }

        public void SetInt(String CanAddress, int value)
        {
            String key = CanAddress.ToUpper();
            lock (IntlLock)
            {
                if (this.RegisterMap.ContainsKey(key))
                {
                    RegisterMap[key] = value;
                }
                else
                {
                    RegisterMap.Add(key, value);
                }
            }
        }

        public int GetInt(String CanAddress, int defalutVal)
        {
            if (CanAddress == null) return defalutVal;
            String key = CanAddress.ToUpper();
            lock (IntlLock)
            {
                if (RegisterMap.ContainsKey(key))
                {
                    return RegisterMap[key];
                }
                else
                {
                    return defalutVal;
                }
            }
        }

        private Byte[] GenerateSendData(byte[] addr, int FunCode, params byte[] datas)
        {
            Byte[] result = new Byte[8];
            result[0] = ID;
            result[1] = (Byte)FunCode;
            result[2] = addr[0];
            result[3] = addr[1];
            var len = Math.Min(datas.Length, 4);
            for (byte i = 0; i < len; i++)
            {
                result[i + 4] = datas[i];
            }
            return result;
        }

        public ValueTuple<Byte, Byte[]>? ParseCanAddress(String CanAddress)
        {
            if (String.IsNullOrEmpty(CanAddress)) return null;
            Regex reg = new Regex(@"^[\dA-Fa-f]{2}\-[\dA-Fa-f]{4}$");
            if (!reg.IsMatch(CanAddress)) return null;
            String[] strs = CanAddress.Split('-');
            Byte targetId = Convert.ToByte(strs[0], 16);
            Byte[] bs = new byte[] { Convert.ToByte(strs[1].Substring(0, 2), 16), Convert.ToByte(strs[1].Substring(2, 2), 16) };
            return ValueTuple.Create(targetId, bs);
        }

        public void ParseCanData(ref string CanAddress, ref int FunCode,byte[] recData)
        {
            byte sourceID = recData[0];
            CanAddress = $"{ByteUtil.ToHex(recData[0])}-{ByteUtil.ToHex(recData[2], recData[3])}";
            FunCode = (int)recData[1];
        }

        public Boolean SetRegister(string CanAddress, int value, byte mask)
        {
            var addrs = ParseCanAddress(CanAddress);
            if (addrs == null) return false;
            (Byte TargetId, Byte[] addr) = addrs.Value;
            var sendData = GenerateSendData(addr, 0x06, ByteUtil.Int4ToBytes(value));
            return Send(TargetId, sendData, mask);
        }

        public Boolean ReadRegister(string CanAddress, byte mask)
        {
            var addrs = ParseCanAddress(CanAddress);
            if (addrs == null) return false;
            (Byte TargetId, Byte[] addr) = addrs.Value;
            var sendData = GenerateSendData(addr,0x03);
            return Send(TargetId, sendData, mask);
        }

        public bool IsTakeImg()
        {
            bool ret = ReadRegister("03-ff01", 0b111);
            Thread.Sleep(10);
            int reti = GetInt("03-ff01", -1);
            return ret && reti == 0;
        }

        public void SendNG()
        {
            SetRegister("03-ff02", 1, 0b111);
            Thread.Sleep(100);
            SetRegister("03-ff02", 0, 0b111);
        }

        public void SendOK()
        {
            SetRegister("03-ff03", 1, 0b111);
            Thread.Sleep(100);
            SetRegister("03-ff03", 0, 0b111);
        }

        public void SendLight1()
        {
            SetRegister("03-ff04", 1, 0b111);
            Thread.Sleep(100);
            SetRegister("03-ff04", 0, 0b111);
        }

        public void SendLight2()
        {
            SetRegister("03-ff05", 1, 0b111);
            Thread.Sleep(100);
            SetRegister("03-ff05", 0, 0b111);
        }
    }
}
