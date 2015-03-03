using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullDataWorker
{
    public class AppStatusModel
    {
        public string appName { get; set; }
        public string appId   { get; set; }
        public string appUrl  { get; set; }
        public string status  { get; set; }
        public int    reviews { get; set; }
    }
}
