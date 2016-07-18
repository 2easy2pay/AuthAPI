using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebAPI_FormsAuth.Entities;

namespace WebAPI_FormsAuth.Services
{
    public class LoginService
    {
        CustomerRegistrationService objCustomerRegistrationService = new CustomerRegistrationService();
        CustomerService objCustomerService = new CustomerService();
        FormsAuthenticationService objAuthService = new FormsAuthenticationService();
        public async Task<LoginResponse> LoginAsync(string userName, string password)
        {
            LoginResponse responseLoginAsync = new LoginResponse();
            var loginResult = objCustomerRegistrationService.ValidateCustomer(password, userName);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = objCustomerService.GetCustomerByMobileNo(userName);
                        //sign in new customer
                        objAuthService.SignIn(customer, true);
                        responseLoginAsync.Success = true;
                        responseLoginAsync.Token = userName;
                        break;
                    }
                case CustomerLoginResults.CustomerNotExist:
                    {
                        responseLoginAsync.Success = false;
                        responseLoginAsync.Token = string.Empty;
                        break;
                    }
                case CustomerLoginResults.Deleted:
                    {
                        responseLoginAsync.Success = false;
                        responseLoginAsync.Token = string.Empty;
                        break;
                    }
                case CustomerLoginResults.NotActive:
                    {
                        responseLoginAsync.Success = false;
                        responseLoginAsync.Token = string.Empty;
                        break;
                    }
                case CustomerLoginResults.NotRegistered:
                    {
                        responseLoginAsync.Success = false;
                        responseLoginAsync.Token = string.Empty;
                        break;
                    }
                case CustomerLoginResults.WrongPassword:
                    {
                        responseLoginAsync.Success = false;
                        responseLoginAsync.Token = string.Empty;
                        break;
                    }
                default:
                    {
                        responseLoginAsync.Success = false;
                        responseLoginAsync.Token = string.Empty;
                        break;
                    }
            }
            return responseLoginAsync;
        }
        
    }
}
