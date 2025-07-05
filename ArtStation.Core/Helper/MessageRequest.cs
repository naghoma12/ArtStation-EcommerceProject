using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class MessageRequest
    {
        public string TitleAR { get; set; }
        public string TitleEN { get; set; }
        public string BodyAR { get; set; }
        public string BodyEN { get; set; }
        public string DeviceToken { get; set; }
    }
}
