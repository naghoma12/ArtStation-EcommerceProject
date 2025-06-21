using ArtStation.Core.Services.Contract;
using ArtStation.Helper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ArtStation.Services
{
    public class SMSService : ISMSService
    {
        

        //var verification = VerificationResource.Create(
        //    to: "+201011037481",
        //    channel: "sms",
        //    pathServiceSid: "VAf00e0f9186f4034adc06ee6a827ff315"
        //);

        //    Console.WriteLine(verification.Sid);
         private readonly TwilioSettings _twilio;
        public SMSService(IOptions<TwilioSettings> twilio)
        {
            _twilio = twilio.Value;
        }

        public MessageResource SendVerificationCode(string mobileNumber)
        {
            TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);

            TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);

            var result = MessageResource.Create(
                    body: "sms from art station",
                    from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
                    to: mobileNumber
                );

            return result;
        }
    
    }
}
