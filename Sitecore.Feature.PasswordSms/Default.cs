namespace Sitecore.Feature.PasswordSms.sitecore.login
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Abstractions;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.DependencyInjection;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines;
    using Sitecore.Pipelines.GetSignInUrlInfo;
    using Sitecore.Pipelines.LoggedIn;
    using Sitecore.Pipelines.LoggingIn;
    using Sitecore.Pipelines.PasswordRecovery;
    using Sitecore.Security.Accounts;
    using Sitecore.SecurityModel.Cryptography;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.Authentication;
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [AllowDependencyInjection]
    public class Default : Sitecore.sitecore.login.Default
    {
        private string fullUserName = string.Empty;
        private bool PasswordRecoveryOnly
        {
            get
            {
                if (this.Request.QueryString["rc"] == "1")
                    return !Settings.Login.DisablePasswordRecovery;
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.sitecore.login.Default" /> class.
        /// </summary>
        public Default()
          : base()
        {
        }

        [Obsolete]
        public Default(BaseCorePipelineManager corePipelineManager)
          : base(corePipelineManager)
        {
        }

        public Default(
         BaseAuthenticationManager authenticationManager,
         BasePipelineFactory pipelineFactory,
         BaseTranslate translate,
         BaseLog log)
         : base(authenticationManager, pipelineFactory, translate, log, ServiceLocator.ServiceProvider.GetRequiredService<BaseCorePipelineManager>())
        {
        }
        public Default(
         BaseAuthenticationManager authenticationManager,
         BasePipelineFactory pipelineFactory,
         BaseTranslate translate,
         BaseLog log,
         BaseCorePipelineManager corePipelineManager)
           : base(authenticationManager, pipelineFactory, translate, log, corePipelineManager)
        {
        }
        protected void ForgotPasswordClicked(object sender, EventArgs e)
        {
            string text = this.UserNameForgot.Text;
            fullUserName = WebUtil.HandleFullUserName(text);
            if (Security.Accounts.User.Exists(fullUserName))
                Assert.ResultNotNull<Pipeline>(this.PipelineFactory.GetPipeline("passwordRecovery"), "passwordRecovery pipeline was not found").Start((PipelineArgs)new PasswordRecoveryArgs(this.Context)
                {
                    Username = text
                });
            if (this.PasswordRecoveryOnly)
            {
                this.login.Style["display"] = "block";
                this.passwordRecovery.Style["display"] = "none";
            }
            RenderSuccess("Your password has been sent to you. If you do not receive the sms with your password, please check that you've typed your user name correctly or contact your administrator.");
        }

        private void RenderSuccess(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            string s = this.Translate.Text(text);
            this.SuccessHolder.Visible = true;
            this.SuccessText.Text = HttpUtility.HtmlEncode(s);
        }
    }
}