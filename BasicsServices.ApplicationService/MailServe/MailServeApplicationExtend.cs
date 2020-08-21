using BasicsServices.IApplicationService.ApplicationCommon;
using BasicsServices.IApplicationService.MailServe;
using BasicsServices.IDomainService.MailServe;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.ApplicationService.MailServe
{
    public partial class MailServeApplication : IMailServeApplication
    {
        private readonly IMailServeDomain _mailServeDomain;
        private readonly IEnableLoginApplication _enableLoginApplication;
        public MailServeApplication(IMailServeDomain mailServeDomain
            , IEnableLoginApplication enableLoginApplication
            )
        {
            _mailServeDomain = mailServeDomain;
            _enableLoginApplication = enableLoginApplication;
        }
    }
}
