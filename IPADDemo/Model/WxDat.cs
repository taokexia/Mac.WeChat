using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPADDemo.Model
{
    public class WxDat
    {
        public string data { get; set; }
        public string message { get; set; }
        public int status { get; set; }
    }

    public class WxToken {
        public string token { get; set; }
        public string message { get; set; }
        public int status { get; set; }
        public long uin { get; set; }
    }
}
