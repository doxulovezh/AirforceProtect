using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AirforceProtect
{



    /// <summary>
    /// 通过API获取进程图标
    /// </summary>
    public class ProcessAPI
    {
        [DllImport("Shell32.dll")]
        private static extern int SHGetFileInfo
        (
        string pszPath,
        uint dwFileAttributes,
        out SHFILEINFO psfi,
        uint cbfileInfo,
        SHGFI uFlags
        );

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero; iIcon = 0; dwAttributes = 0; szDisplayName = ""; szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            public string szTypeName;
        };

        private enum SHGFI
        {
            SmallIcon = 0x00000001,
            LargeIcon = 0x00000000,
            Icon = 0x00000100,
            DisplayName = 0x00000200,
            Typename = 0x00000400,
            SysIconIndex = 0x00004000,
            UseFileAttributes = 0x00000010
        }
        //获取进程图标
        public static Icon GetIcon(string strPath, bool bSmall)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;
            if (bSmall)
                flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
            else
                flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;

            SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);
            return Icon.FromHandle(info.hIcon);
        }


        //获取进程图标
        public static Icon GetIcon(int pid, bool bSmall)
        {

            try
            {
                var p = System.Diagnostics.Process.GetProcessById(pid);

                return GetIcon(p.MainModule.FileName, bSmall);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //获取进程名称
        public static string GetProcessNameByPID(int processID)
        {
            //could be an error here if the process die before we can get his name
            try
            {
                Process p = Process.GetProcessById((int)processID);
                return p.ProcessName;
            }
            catch (Exception ex)
            {
                return "Unknown";
            }
        }
        //获取进程路径
        public static string GetProcessPathByPID(int processID)
        {
            //could be an error here if the process die before we can get his name
            try
            {
                Process p = Process.GetProcessById((int)processID);
                return p.MainModule.FileName;
            }
            catch (Exception ex)
            {
                return "Unknown";
            }
        }
        public static string KillProcessByPID(int processID)
        {
            //could be an error here if the process die before we can get his name
            try
            {
                Process p = Process.GetProcessById((int)processID);
                p.Kill();
                return "Success";
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public static double UsingProcess(int PID)

        {
            try
            {
                double value = 0;
                using (var pro = Process.GetProcessById(PID))

                {

                    //间隔时间（毫秒）

                    int interval = 20000;

                    //上次记录的CPU时间

                    var prevCpuTime = TimeSpan.Zero;

                    //while (false)

                    //{

                    //当前时间

                    var curTime = pro.TotalProcessorTime;

                    //间隔时间内的CPU运行时间除以逻辑CPU数量

                    value = Math.Round((curTime - prevCpuTime).TotalMilliseconds / interval / Environment.ProcessorCount, 5);

                    prevCpuTime = curTime;

                    //输出


                    //Thread.Sleep(interval);

                    //}
                }
                return value;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }


        //public static double GetCPU_WD()
        //{
        //    PerformanceCounter[] counters = new PerformanceCounter[System.Environment.ProcessorCount];
        //    for (int i = 0; i < counters.Length; i++)
        //    {
        //        counters[i] = new PerformanceCounter("Processor", "% Processor Time", i.ToString());
        //    }
        //    List<float> wd = new List<float>();
        //    for (int i = 0; i < counters.Length; i++)
        //    {
        //        float f = counters[i].NextValue();
        //        Console.WriteLine("CPU-{0}: {1:f}%", i, f);
        //        wd.Add(f);
        //    }
        //    float dd = 0;
        //    for(int i = 0; i < wd.Count; i++)
        //    {
        //        if (wd[i] > dd)
        //        {
        //            dd = wd[i];
        //        }
        //    }
        //    return dd;
        //}

        public static string GetCPU_WD()
        {
            Double CPUtprt = 0;

            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(@"root\WMI", "Select * From MSAcpi_ThermalZoneTemperature");

            foreach (System.Management.ManagementObject mo in mos.Get())

            {

                CPUtprt = Convert.ToDouble(Convert.ToDouble(mo.GetPropertyValue("CurrentTemperature").ToString()) - 2732) / 10;

                return CPUtprt.ToString();

            }
            return "";
        }
    }

}
