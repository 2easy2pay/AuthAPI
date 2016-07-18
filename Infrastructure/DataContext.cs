using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using WebAPI_FormsAuth.Entities;

namespace WebAPI_FormsAuth.Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext() : base("ShopEntities")
       {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public virtual System.Data.Entity.DbSet<Customer> Customers { get; set; }
        public virtual System.Data.Entity.DbSet<CustomerRole> CustomerRoles { get; set; }

    }
}