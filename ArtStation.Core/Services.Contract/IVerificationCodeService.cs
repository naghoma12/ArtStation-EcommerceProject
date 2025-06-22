using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Services.Contract
{
    public  interface IVerificationCodeService
    {
        public string GenerateCode(string phone);
        public bool ValidateCode(string phone, string inputCode);
    }
}
