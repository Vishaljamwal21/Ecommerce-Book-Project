using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Project_Ecomm_1.Utility
{
    public class TwillioService
    {
        public class TwilioService
        {
            private readonly string _accountSid;
            private readonly string _authToken;
            public string AccountSid { get; set; }
            public string AuthToken { get; set; }

            public TwilioService(IConfiguration configuration)
            {
                _accountSid = configuration["Twilio:AccountSid"];
                _authToken = configuration["Twilio:AuthToken"];

                TwilioClient.Init(_accountSid, _authToken);
            }

            public void SendMessage(string to, string from, string body)
            {
                MessageResource.Create(
                    to: new Twilio.Types.PhoneNumber(to),
                    from: new Twilio.Types.PhoneNumber(from),
                    body: body
                );
            }
        }
    }
}
