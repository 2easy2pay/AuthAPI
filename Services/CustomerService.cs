using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI_FormsAuth.Entities;
using WebAPI_FormsAuth.Infrastructure;

namespace WebAPI_FormsAuth.Services
{
    /// <summary>
    /// Customer service
    /// </summary>
    public  class CustomerService 
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        private const string CUSTOMERROLES_BY_SYSTEMNAME_KEY = "Nop.customerrole.systemname-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CUSTOMERROLES_PATTERN_KEY = "Nop.customerrole.";

        #endregion


        DataContext context = new DataContext();
        #region Methods

        #region Customers
        public  Customer GetCustomerByMobileNo(string mobileno)
        {
            var customer = new Customer();
            if (string.IsNullOrWhiteSpace(mobileno))
                return null;
           
                var query = from c in context.Customers
                            orderby c.Id
                            where c.MobileNo == "+965" + mobileno
                            select c;
                customer = query.FirstOrDefault();
                //customer = context.Customers.Where(x => x.MobileNo.Equals("+965" + mobileno, StringComparison.CurrentCultureIgnoreCase)).OrderBy(x => x.Id).FirstOrDefault();
           
            
            return customer;
        }

        /// <summary>
        /// Get customer by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Customer</returns>
        public  Customer GetCustomerBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;
            var customer = new Customer();
            
                var query = from c in context.Customers
                            orderby c.Id
                            where c.SystemName == systemName
                            select c;
                customer = query.FirstOrDefault();
            return customer;
        }

        
        /// <summary>
        /// Insert a guest customer
        /// </summary>
        /// <returns>Customer</returns>
        public  Customer InsertGuestCustomer()
        {
            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            //add to 'Guests' role
            var guestRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (guestRole == null)
                throw new ArgumentNullException("'Guests' role could not be loaded");
            customer.CustomerRoles.Add(guestRole);
          
                context.Customers.Add(customer);
                context.SaveChanges();
            return customer;
        }

        /// <summary>
        /// Insert a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public  void InsertCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
                context.Customers.Add(customer);
                context.SaveChanges();
            ////event notification
            //_eventPublisher.EntityInserted(customer);
        }

        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public  void UpdateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
                context.SaveChanges();

            ////event notification
            //_eventPublisher.EntityUpdated(customer);
        }


        #endregion

        #region Customer roles



        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer role</returns>
        public  CustomerRole GetCustomerRoleById(int customerRoleId)
        {
            if (customerRoleId == 0)
                return null;
            var customerRole = new CustomerRole();
                var query = from c in context.CustomerRoles
                            orderby c.Id
                            where c.Id == customerRoleId
                            select c;
                customerRole = query.FirstOrDefault();
            return customerRole;
        }

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="systemName">Customer role system name</param>
        /// <returns>Customer role</returns>
        public  CustomerRole GetCustomerRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;
            var customerRole = new CustomerRole();
                var query = from cr in context.CustomerRoles
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                customerRole = query.FirstOrDefault();
            return customerRole;
        }

        /// <summary>
        /// Gets all customer roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Customer roles</returns>
        public  IList<CustomerRole> GetAllCustomerRoles(bool showHidden = false)
        {
            IList<CustomerRole> customerRoles = new List<CustomerRole>();
                var query = from cr in context.CustomerRoles
                            orderby cr.Name
                            where (showHidden || cr.Active)
                            select cr;
                customerRoles = query.ToList();
            return customerRoles;
        }

        /// <summary>
        /// Inserts a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public  void InsertCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");
                context.CustomerRoles.Add(customerRole);
                context.SaveChanges();

            //_cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);

            ////event notification
            //_eventPublisher.EntityInserted(customerRole);
        }

        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        public  void UpdateCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");
                context.SaveChanges();

            //_cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);

            ////event notification
            //_eventPublisher.EntityUpdated(customerRole);
        }

        #endregion

        #endregion
    }
}