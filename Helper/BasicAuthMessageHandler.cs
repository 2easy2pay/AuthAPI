using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebAPI_FormsAuth.Helper
{
    public class BasicAuthMessageHandler : DelegatingHandler
    {
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";

        public adminPrincipalProvider PrincipalProvider = new adminPrincipalProvider();

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            AuthenticationHeaderValue authValue = request.Headers.Authorization;
            if (authValue != null && authValue.Parameter != "undefined" && !String.IsNullOrWhiteSpace(authValue.Parameter))
            {
                string email = authValue.Parameter;
                if (HttpContext.Current.Session == null || HttpContext.Current.Session["userToken"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["userToken"].ToString()))
                {
                    HttpContext.Current.Session["userToken"] = email;
                }
                else
                {
                    email = HttpContext.Current.Session["userToken"].ToString();
                }

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(email))
                {
                    IPrincipal principalObj = PrincipalProvider.createPrincipal(email, "Admin");
                    Thread.CurrentPrincipal = principalObj;
                    HttpContext.Current.User = principalObj;
                }
            }
            return base.SendAsync(request, cancellationToken)
               .ContinueWith(task =>
               {
                   var response = task.Result;
                   if (response.StatusCode == HttpStatusCode.Unauthorized
                       && !response.Headers.Contains(BasicAuthResponseHeader))
                   {
                       response.Headers.Add(BasicAuthResponseHeader
                           , BasicAuthResponseHeaderValue);
                   }
                   return response;
               });
        }

    }
}
