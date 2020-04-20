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
    public class PopulateSms: PasswordRecoveryProcessor
    {
        public override void Process(PasswordRecoveryArgs args)
        {
            Assert.IsNotNull((object)args, nameof(args));
            this.LoadSmsFields(args);
        }

        protected virtual void LoadSmsFields(PasswordRecoveryArgs args)
        {
            using (new SecurityDisabler())
            {
                Item obj = Client.CoreDatabase.GetItem("{32CAA3DB-0395-4B8C-A924-D2103BD259B9}");
                if (!this.IsExist(obj))
                    obj = Client.CoreDatabase.GetItem(new ID("{32CAA3DB-0395-4B8C-A924-D2103BD259B9}"), Language.Parse(Settings.Login.PasswordRecoveryDefaultLanguage));
                Assert.IsNotNull((object)obj, "recoverySmsItem");

                args.CustomData.Add(Constant.SmsUrl, obj[new ID("{30A47B60-A6D8-46B7-9F0C-3D0BB6B25FA1}")]);
                args.CustomData.Add(Constant.ApiKey, obj[new ID("{208A31DF-213E-433B-A81A-753A14FC22C9}")]);
                args.CustomData.Add(Constant.Sender, obj[new ID("{1491C164-A206-498E-844E-0A06C50A5966}")]);
                args.CustomData.Add(Constant.Message, this.GetEmailContent(args, obj[new ID("{0D693995-0FA2-4131-AEEF-94EA8AE6E1A8}")]));
            }
        }

       protected virtual bool IsExist(Item item)
        {
            if (item != null)
                return item.Versions.Count > 0;
            return false;
        }

       protected virtual string GetEmailContent(PasswordRecoveryArgs args, string emailContentTemplate)
        {
            if (emailContentTemplate.IndexOf("%Password%", StringComparison.InvariantCultureIgnoreCase) < 0)
                throw new ArgumentException("Missing password token in password recovery Sms");
            //if (emailContentTemplate.IndexOf("%UserName%", StringComparison.InvariantCultureIgnoreCase) < 0)
            //    throw new ArgumentException("Missing user name token in password recovery Sms");
            return Regex.Replace(Regex.Replace(emailContentTemplate, "%Password%", args.Password, RegexOptions.IgnoreCase), "%UserName%", args.Username, RegexOptions.IgnoreCase);
        }
    }
}
