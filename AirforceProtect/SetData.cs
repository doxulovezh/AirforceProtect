using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirforceProtect
{

    [Serializable]
    public class SetData
    {
        public bool protectexe { set; get; }
        public bool protectdll { set; get; }
        public bool protectxml { set; get; }
        public bool protectconfig { set; get; }
        public bool protectOnlyRead { set; get; }
        public bool protectDriver { set; get; }
      
    }
}
