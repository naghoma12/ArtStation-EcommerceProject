using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace ArtStation.Core.Services.Contract
{
    public interface ISMSService
    {
        MessageResource SendVerificationCode(string toPhoneNumber, string code);
    }
}
