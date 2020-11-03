using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Entities.v1;

namespace PayAway.WebAPI.DB
{
    public class SQLiteDBContext : DbContext
    {

        public DbSet<MerchantDBE> Merchants { get; set; }

        public DbSet<CustomerDBE> Customers { get; set; }

        #region ==== Configure DB ================

        /// <summary>
        /// <para>
        /// Override this method to configure the database (and other options) to be used for this context.
        /// This method is called for each instance of the context that is created.
        /// The base implementation does nothing.
        /// </para>
        /// <para>
        /// In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have been passed
        /// to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
        /// the options have already been set, and skip some or all of the logic in
        /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
        /// </para>
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context. Databases (and other extensions)
        /// typically define extension methods on this object that allow you to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=PrestoPayv2.db;");
        }

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.</remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchantDBE>().ToTable("Merchants");
            modelBuilder.Entity<MerchantDBE>()
                .HasKey(m => new { m.MerchantID });
            modelBuilder.Entity<MerchantDBE>()
                .HasIndex(m => new { m.MerchantName })
                .IsUnique();

            modelBuilder.Entity<CustomerDBE>().ToTable("Customers");
            modelBuilder.Entity<CustomerDBE>()
                .HasKey(c => new { c.MerchantID, c.CustomerID });
            modelBuilder.Entity<CustomerDBE>()
                .HasIndex(c => new { c.MerchantID, c.CustomerPhoneNo })
                .IsUnique();

            modelBuilder.Seed();
        }

        #endregion

        /// <summary>
        /// Gets the merchants.
        /// </summary>
        /// <returns>List&lt;MerchantDBE&gt;.</returns>
        internal static List<MerchantDBE> GetAllMerchants()
        {
            using (var context = new SQLiteDBContext())
            {
                return context.Merchants.ToList();
            }
        }

        /// <summary>
        /// Gets the merchant.
        /// </summary>
        /// <param name="merchantID">The merchant unique identifier.</param>
        /// <returns>MerchantDBE.</returns>
        /// <exception cref="ApplicationException">Merchant: [{merchantGuid}] is not valid</exception>
        internal static MerchantDBE GetMerchant(Guid merchantID)
        {
            using (var context = new SQLiteDBContext())
            { 
                var dbMerchant = context.Merchants.FirstOrDefault(m => m.MerchantID == merchantID);

                return dbMerchant;
            }
        }

        /// <summary>
        /// Gets the customers.
        /// </summary>
        /// <param name="merchantID">The merchant identifier.</param>
        /// <returns>List&lt;CustomerDBE&gt;.</returns>
        internal static List<CustomerDBE> GetCustomers(Guid merchantID)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbCustomers = context.Customers.Where(m => m.MerchantID == merchantID).ToList();

                return dbCustomers;
            }
        }

        /// <summary>
        /// Inserts the merchant.
        /// </summary>
        /// <param name="newMerchant">The merchant.</param>
        /// <returns>MerchantDBE.</returns>
        internal static MerchantDBE InsertMerchant(NewMerchantMBE newMerchant)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbMerchant = new MerchantDBE
                {
                    MerchantID = Guid.NewGuid(),
                    MerchantName = newMerchant.MerchantName,
                    IsSupportsTips = newMerchant.IsSupportsTips
                };

                context.Merchants.Add(dbMerchant);
                context.SaveChanges();

                return dbMerchant;
            }
        }
    }
}
