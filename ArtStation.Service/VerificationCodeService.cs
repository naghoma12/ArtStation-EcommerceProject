using ArtStation.Core.Services.Contract;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Services
{
    public class VerificationCodeService:IVerificationCodeService
    {
        private readonly IMemoryCache _cache;

        public VerificationCodeService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateCode(string phone)
        {
            var code = new Random().Next(100000, 999999).ToString();
            _cache.Set(phone, code, TimeSpan.FromMinutes(5));
            return code;
        }

        public bool ValidateCode(string phone, string inputCode)
        {
            return _cache.TryGetValue(phone, out string correctCode) && correctCode == inputCode;
        }
    
}
}
