namespace Sitecore.Feature.PasswordSms.Pipelines
{
    using Sitecore.Diagnostics;
    using Sitecore.Threading;
    using System.Net.Mail;
    using System.Threading;
    using Sitecore.Pipelines.PasswordRecovery;
    using System.Web;
    using System.Collections.Specialized;
    using System.Net;
    using System.IO;
    using Sitecore.Feature.PasswordSms.Model;
    using Newtonsoft.Json;

    public class SendPasswordRecoverySms : PasswordRecoveryProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Process(PasswordRecoveryArgs args)
        {
            Assert.IsNotNull((object)args, nameof(args));
            Assert.IsNotNull(args.CustomData, "args.CustomData");
            Assert.IsTrue(args.CustomData.TryGetValue(Constant.ApiKey, out object apiKey), "args.ApiKey");
            Assert.IsTrue(args.CustomData.TryGetValue(Constant.SmsUrl, out object smsUrl), "args.smsUrl");
            Assert.IsTrue(args.CustomData.TryGetValue(Constant.Message, out object message), "args.message");
            Assert.IsTrue(args.CustomData.TryGetValue(Constant.MobileNumber, out object mobileNumber), "args.mobileNumber");
            Assert.IsTrue(args.CustomData.TryGetValue(Constant.Sender, out object sender), "args.sender");

            var response = this.SendSms(smsUrl.ToString(), apiKey.ToString(), mobileNumber.ToString(), message.ToString(), sender.ToString());
            if (!string.IsNullOrEmpty(response?.ErrorContent))
                Log.Error(response.ErrorContent, this);
        }

        /// <summary>GenerateMailMessage</summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual ApiResponse SendSms(string apiUrl,string authKey, string mobileNumber,string message,string sender)
        {
            string urlEncodedMessage = HttpUtility.UrlEncode(message);
            byte[] response;
            using (var wb = new WebClient())
            {
                var url = new System.Uri(apiUrl);
                response = wb.UploadValues(url, new NameValueCollection()
                {
                {"apikey" , authKey},
                {"numbers" , mobileNumber},
                {"message" , urlEncodedMessage},
                {"sender" , sender}
                });
            }
            using (MemoryStream stream = new MemoryStream(response))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                   var op = streamReader.ReadToEnd();
                   return JsonConvert.DeserializeObject<ApiResponse>(op);
                }
            }
        }
    }
}
