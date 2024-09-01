using MasterDetailsAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MasterDetailsAuthentication.DAL
{
    public class MyDbContext:DbContext
    {
        public MyDbContext() : base("DbCon")
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderMaster> OrderMasters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(o => o.OrderMaster)
                .WithMany(o => o.OrderDetail)
                .HasForeignKey(o => o.OrderId);
        }
    }
}