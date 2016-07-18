using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI_FormsAuth.Entities;

namespace WebAPI_FormsAuth.Services
{
    /// <summary>
  /// Customer registration service
  /// </summary>
    public  class CustomerRegistrationService 
    {

        EncryptionService objEncryptionService = new EncryptionService();
        CustomerService objCustomerService = new CustomerService();
        #region Methods

        /// <summary>
        /// Validate customer
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        public virtual CustomerLoginResults ValidateCustomer(string password, string MobileNo = "")
        {
            CustomerService objCustomerService = new CustomerService();
            EncryptionService objEncryptionService = new EncryptionService();
            var customer =
                objCustomerService.GetCustomerByMobileNo(MobileNo);
            if (customer == null)
                return CustomerLoginResults.CustomerNotExist;
            if (customer.Deleted)
                return CustomerLoginResults.Deleted;
            if (!customer.Active)
                return CustomerLoginResults.NotActive;
            //only registered can login
            //if (!customer.IsRegistered())
            //    return CustomerLoginResults.NotRegistered;

            string pwd;
            pwd = objEncryptionService.CreatePasswordHash(password, customer.PasswordSalt, "SHA1");
            bool isValid = pwd == customer.Password;
            if (!isValid)
                return CustomerLoginResults.WrongPassword;

            //save last login date
            customer.LastLoginDateUtc = DateTime.UtcNow;
            objCustomerService.UpdateCustomer(customer);
            return CustomerLoginResults.Successful;
        }

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
         
            if (request.Customer == null)
                throw new ArgumentException("Can't load current customer");

            var result = new CustomerRegistrationResult();

            if (request.Customer.IsRegistered())
            {
                result.AddError("Current customer is already registered");
                return result;
            }
           
            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError("PasswordIsNotProvided");
                return result;
            }
            
            //at this point request is valid
            request.Customer.Username = request.Username;
            request.Customer.Email = request.Email;
            request.Customer.PasswordFormatId = (int) PasswordFormat.Hashed;

            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    {
                        request.Customer.Password = request.Password;
                    }
                    break;
                case PasswordFormat.Encrypted:
                    {
                       
                    }
                    break;
                case PasswordFormat.Hashed:
                    {
                        string saltKey = objEncryptionService.CreateSaltKey(5);
                        request.Customer.PasswordSalt = saltKey;
                        request.Customer.Password = objEncryptionService.CreatePasswordHash(request.Password, saltKey, "SHA1");
                    }
                    break;
                default:
                    break;
            }

            request.Customer.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = objCustomerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredRole == null)
                throw new ArgumentNullException("'Registered' role could not be loaded");
            request.Customer.CustomerRoles.Add(registeredRole);
            //remove from 'Guests' role
            var guestRole = request.Customer.CustomerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests);
            if (guestRole != null)
                request.Customer.CustomerRoles.Remove(guestRole);
            objCustomerService.UpdateCustomer(request.Customer);
            return result;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual ChangePasswordResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ChangePasswordResult();
            //if (String.IsNullOrWhiteSpace(request.Email))
            //{
            //    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
            //    return result;
            //}
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError("PasswordIsNotProvided");
                return result;
            }

            var customer = objCustomerService.GetCustomerByMobileNo(request.MobileNo);
            if (customer == null)
            {
                result.AddError("MobileNoNotFound");
                return result;
            }


            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                string oldPwd;
                oldPwd = objEncryptionService.CreatePasswordHash(request.OldPassword, customer.PasswordSalt, "SHA1");

                bool oldPasswordIsValid = oldPwd == customer.Password;
                if (!oldPasswordIsValid)
                    result.AddError("OldPasswordDoesntMatch");

                if (oldPasswordIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                string saltKey = objEncryptionService.CreateSaltKey(5);
                customer.PasswordSalt = saltKey;
                customer.Password = objEncryptionService.CreatePasswordHash(request.NewPassword, saltKey, "SHA1");
                customer.PasswordFormatId = (int)request.NewPasswordFormat;

                //PasswordFormat = request.NewPasswordFormat;
                objCustomerService.UpdateCustomer(customer);
            }

            return result;
        }


        public virtual ChangePasswordResult ResetPassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ChangePasswordResult();
            //if (String.IsNullOrWhiteSpace(request.Email))
            //{
            //    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
            //    return result;
            //}
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError("PasswordIsNotProvided");
                return result;
            }

            var customer = objCustomerService.GetCustomerByMobileNo(request.MobileNo);
            if (customer == null)
            {
                result.AddError("MobileNoNotFound");
                return result;
            }

            string saltKey = objEncryptionService.CreateSaltKey(5);
            customer.PasswordSalt = saltKey;
            customer.Password = objEncryptionService.CreatePasswordHash(request.NewPassword, saltKey, "SHA1");
            customer.PasswordFormatId = (int)request.NewPasswordFormat;
            objCustomerService.UpdateCustomer(customer);

            return result;
        }


        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newUsername">New Username</param>
        public virtual void SetUsername(Customer customer, string newUsername)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            newUsername = newUsername.Trim();

            if (newUsername.Length > 100)
                throw new ArgumentNullException("UsernameTooLong");

            var user2 = objCustomerService.GetCustomerByMobileNo(newUsername);
            if (user2 != null && customer.Id != user2.Id)
                throw new ArgumentNullException("UsernameAlreadyExists");

            customer.Username = newUsername;
            objCustomerService.UpdateCustomer(customer);
        }


        /// <summary>
        /// Sets a customer fullname
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="fullName">New fullname</param>
        public virtual void SetFullName(Customer customer, string fullName)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            customer.FullName = fullName;
            objCustomerService.UpdateCustomer(customer);
        }

        #endregion
    }
}