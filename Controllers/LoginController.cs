using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using WebAPI_FormsAuth.Services;

namespace WebAPI_FormsAuth.Controllers
{
    [RoutePrefix("api/loginctrl")]
    public class LoginController : ApiController
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        #endregion

        #region Ctor
        public LoginController()
        {

        }
        public LoginController(IAuthenticationService authenticationService,
            ICustomerService customerService, ICustomerRegistrationService customerRegisterService)
        {
            this._authenticationService = authenticationService;
            this._customerService = customerService;
            this._customerRegistrationService = customerRegisterService;
        }

        #endregion
        [HttpPost, AllowAnonymous, Route("login")]
        public async Task<HttpResponseMessage> Login([FromBody]LoginRequest request)
        {
            var loginService = new LoginService();
            LoginResponse response = await loginService.LoginAsync(request.username, request.password);
            if (response.Success)
            {
                FormsAuthentication.SetAuthCookie(response.Token, false);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost, AllowAnonymous, Route("logout")]
        public void Signout()
        {
            FormsAuthentication.SignOut();

            if (HttpContext.Current.Session != null)
                HttpContext.Current.Session.Abandon();
        }

        [HttpGet, Authorize(Roles = "admin"), Route("name")]
        public string GetMyName()
        {
            return "Test";
        }

    }
}