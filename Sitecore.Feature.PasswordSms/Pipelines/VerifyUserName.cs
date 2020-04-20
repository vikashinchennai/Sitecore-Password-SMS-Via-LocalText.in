namespace Sitecore.Feature.PasswordSms.Pipelines
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.SecurityModel;
    using System;
    using System.Text.RegularExpressions;
    using Sitecore.Pipelines.PasswordRecovery;
    using Sitecore.Security.Accounts;
    using Sitecore.Web;

    public class VerifyUserName : PasswordRecoveryProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Process(PasswordRecoveryArgs args)
        {
            Assert.IsNotNull((object)args, nameof(args));
            Assert.IsNotNullOrEmpty(args.Username, "args.Username");
            args.Username = WebUtil.HandleFullUserName(args.Username);
            string mobileNumber = this.GetMobileNumberFromUsername(args.Username);
            if (string.IsNullOrEmpty(mobileNumber))
            {
                Log.Error("The mobile of the specified user wasn't found.", (object)this);
                args.AbortPipeline();
            }
            args.CustomData.Add(Constant.MobileNumber, mobileNumber);
        }

        /// <summary>Returns email address from username</summary>
        /// <param name="username"></param>
        /// <returns></returns>
        protected virtual string GetMobileNumberFromUsername(string username)
        {
            if (!User.Exists(username))
                return string.Empty;
            return User.FromName(username, false).Profile.GetCustomProperty("Mobile Number");
        }
    }
}
