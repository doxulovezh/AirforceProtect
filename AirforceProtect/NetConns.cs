using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace AirforceProtect
{

    [Serializable]
    public class NetConns
    {
         public BitmapImage  ico { set; get; }
        public int PID { set; get; }
        public string 应用程序名称 { set; get; }
        public string 网络连接状态 { set; get; }
        public string 协议 { set; get; }
        public double CPU使用率 { set; get; }
        public int 本地端口 { set; get; }
        public string 本地IP地址 { set; get; }
        public int 远程端口 { set; get; }
        public string 远程IP地址 { set; get; }
        public string 应用程序路径 { set; get; }

    }
}
