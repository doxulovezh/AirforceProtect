using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirforceProtect
{

    [Serializable]
    public class FileMD5
    {
        public string type { set; get; }
        public string FileName { set; get; }
        public string MD5 { set; get; }
        public string Path { set; get; }
    }
}
