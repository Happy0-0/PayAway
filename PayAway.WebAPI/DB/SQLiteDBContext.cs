using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;

using PayAway.WebAPI.Entities.Database;

namespace PayAway.WebAPI.DB
{
    /// <summary>
    /// Class SQLiteDBContext./
    /// Implements the <see cref="Microsoft.EntityFrameworkCore.DbContext" />
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    /// <remarks>
    /// MBE entities should NOT leak down into this class
    /// </remarks>
    public class SQLiteDBContext : DbContext
    {

        public DbSet<MerchantDBE> Merchants { get; set; }

        public DbSet<DemoCustomerDBE> DemoCustomers { get; set; }

        public DbSet<CatalogItemDBE> CatalogItems { get; set; }

        public DbSet<OrderDBE> Orders { get; set; }

        public DbSet<OrderEventDBE> OrderEvents { get; set; }

        public DbSet<OrderLineItemDBE> OrderLineItems { get; set; }


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
            // By convention, non-composite primary keys of type short, int, long, or Guid are set up to have values generated for inserted entities,
            // if a value isn't provided by the application. 

            #region === Merchants =========================================================
            modelBuilder.Entity<MerchantDBE>().ToTable("Merchants");
            modelBuilder.Entity<MerchantDBE>()
                .HasKey(m => new { m.MerchantId });         // <== auto generated value in DB
            modelBuilder.Entity<MerchantDBE>()
                .HasIndex(m => new { m.MerchantGuid })
                .IsUnique();
            modelBuilder.Entity<MerchantDBE>()
                .HasIndex(m => new { m.MerchantName })      // you can have dup merchant names
                .IsUnique();
            modelBuilder.Entity<MerchantDBE>()
                .Property(m => m.MerchantGuid)
                .HasValueGenerator<GuidValueGenerator>();   // <== auto generated value by EF before calling db
            #endregion

            #region === DemoCustomers =========================================================
            modelBuilder.Entity<DemoCustomerDBE>().ToTable("DemoCustomers");
            modelBuilder.Entity<DemoCustomerDBE>()
                .HasKey(dc => new { dc.DemoCustomerId });     // <== auto generated value in DB
            modelBuilder.Entity<DemoCustomerDBE>()
                .HasIndex(dc => new { dc.DemoCustomerGuid })
                .IsUnique();
            modelBuilder.Entity<DemoCustomerDBE>()  
                .HasIndex(dc => new { dc.MerchantId, dc.CustomerPhoneNo })  // cannot have dup customer phone nos
                .IsUnique();
            modelBuilder.Entity<DemoCustomerDBE>()
                .Property(dc => dc.DemoCustomerGuid)
                .HasValueGenerator<GuidValueGenerator>();   // <== auto generated value by EF before calling db
            #endregion

            #region === CatalogItems =========================================================
            modelBuilder.Entity<CatalogItemDBE>().ToTable("CatalogItems");
            modelBuilder.Entity<CatalogItemDBE>()
                .HasKey(ci => new { ci.CatalogItemId });      // <== auto generated value in DB
            modelBuilder.Entity<CatalogItemDBE>()
                .HasIndex(ci => new { ci.MerchantId, ci.ItemName })    // cannot have dump Item names on the same merchant
                .IsUnique();
            modelBuilder.Entity<CatalogItemDBE>()
                .HasIndex(ci => new { ci.CatalogItemGuid })
                .IsUnique();
            modelBuilder.Entity<CatalogItemDBE>()
                .Property(ci => ci.CatalogItemGuid)
                .HasValueGenerator<GuidValueGenerator>();   // <== auto generated value by EF before calling db
            #endregion

            #region === Orders =========================================================
            modelBuilder.Entity<OrderDBE>().ToTable("Orders");
            modelBuilder.Entity<OrderDBE>()
                .HasKey(o => new { o.OrderId });            // <== auto generated value in DB
            modelBuilder.Entity<OrderDBE>()
                .HasIndex(o => new { o.OrderGuid})
                .IsUnique();
            modelBuilder.Entity<OrderDBE>()
                .Property(o => o.OrderGuid)
                .HasValueGenerator<GuidValueGenerator>();   // <== auto generated value by EF before calling db
            modelBuilder.Entity<OrderDBE>()
                .Property(c => c.Status)
                .HasConversion<int>();
            #endregion

            #region === OrderLineItems =========================================================
            modelBuilder.Entity<OrderLineItemDBE>().ToTable("OrderLineItems");
            modelBuilder.Entity<OrderLineItemDBE>()
                .HasKey(oli => new { oli.OrderLineItemId});  // <== auto generated value in DB
            modelBuilder.Entity<OrderLineItemDBE>()
                .HasIndex(oli => new { oli.OrderId, oli.ItemName })  // cannot have dump Item names on the same order
                .IsUnique();
            #endregion

            #region === OrderEvents =========================================================
            modelBuilder.Entity<OrderEventDBE>().ToTable("OrderEvents");
            modelBuilder.Entity<OrderEventDBE>()
                .HasKey(oe => new { oe.OrderEventId});       // <== auto generated value
            modelBuilder.Entity<OrderEventDBE>()
                .HasIndex(oe => new { oe.OrderId });        // <= not unqiue key, used for faster retrieval
            modelBuilder.Entity<OrderEventDBE>()
                .Property(c => c.OrderStatus)
                .HasConversion<int>();
            #endregion
        }

        #endregion

        #region ==== Reset DB =============

        /// <summary>
        /// Resets the database.
        /// </summary>
        /// <param name="isPreloadEnabled">The is preload enabled.</param>
        public static void ResetDB(bool isPreloadEnabled)
        {
            // Step 1: Purge the exisiting Merchants all dependant items
            //         1.1     DemoCustomers
            //         1.2     OrderEvents
            //         1.3     OrderItems
            //         1.4     Orders
            //         1.5     CatalogItems
            //         1.6     Merchant
            var existingMerchants = SQLiteDBContext.GetAllMerchants();
            
            foreach (var existingMerchant in existingMerchants)
            {
                SQLiteDBContext.DeleteMerchant(existingMerchant.MerchantId);
                // this deletes via cascading delete
                //  === Step 1.1: Demo Customers ======================================
                //  === Step 1.2: OrderEvents ======================================
                //  === Step 1.3: OrderItems ======================================
                //  === Step 1.4: CatalogItems ======================================
            }

            // delete "default" catalog items under MerchantId 0
            var existingCatalogItems = SQLiteDBContext.GetCatalogItems(0);

            foreach (var existingCatalogItem in existingCatalogItems)
            {
                SQLiteDBContext.DeleteCatalogItem(existingCatalogItem.CatalogItemId);
            }

            // Step 2:Optionally reload the seed data
            //         2.1     Merchant
            //         2.1     DemoCustomers
            //         2.3     CatalogItems
            //         2.4     Orders
            //         2.5     OrderEvents
            //         2.6     OrderItems

            #region === Reload the Catalog Items ===========================
            var seedCatalogItems = SeedData.GetSeedCatalogueItems();
            foreach (var seedCatalogItem in seedCatalogItems)
            {
                SQLiteDBContext.InsertCatalogItem(seedCatalogItem);
            }
            #endregion

            if (isPreloadEnabled)
            {
                #region === Step 2.1: Reload the Merchants ===============================
                var seedMerchants = SeedData.GetSeedMerchants();
                foreach (var seedMerchant in seedMerchants)
                {
                    SQLiteDBContext.InsertMerchant(seedMerchant);
                }
                #endregion

                #region === Step 2.2: Reload the DemoCustomers ===========================
                var seedDemoCustomers = SeedData.GetSeedDemoCustomers();
                foreach(var seedDemoCustomer in seedDemoCustomers)
                {
                    SQLiteDBContext.InsertDemoCustomer(seedDemoCustomer);
                }
                #endregion                

                #region === Step 2.4: Reload the Orders ===========================
                var seedOrders = SeedData.GetSeedOrders();
                foreach(var seedOrder in seedOrders)
                {
                    SQLiteDBContext.InsertOrder(seedOrder);
                }
                #endregion

                #region === Step 2.5: Reload the Order Events ===========================
                var seedOrderEvents = SeedData.GetOrderEvents();
                foreach(var seedOrderEvent in seedOrderEvents)
                {
                    SQLiteDBContext.InsertOrderEvent(seedOrderEvent);
                }
                #endregion

                #region === Step 2.6: Reload the Order Line Items ===========================
                var seedOrderLineItems = SeedData.GetOrderLineItems();
                foreach(var seedOrderLineItem in seedOrderLineItems)
                {
                    SQLiteDBContext.InsertOrderLineItem(seedOrderLineItem);
                }
                #endregion
            }
        }
                
        #endregion

        #region ==== Merchant Methods =====
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
        /// Gets a specific merchant using the public guid.
        /// </summary>
        /// <param name="merchantGuid">The merchant unique identifier.</param>
        /// <returns>MerchantDBE.</returns>
        internal static MerchantDBE GetMerchant(Guid merchantGuid)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbMerchant = context.Merchants.FirstOrDefault(m => m.MerchantGuid == merchantGuid);

                return dbMerchant;
            }
        }

        /// <summary>
        /// Gets the merchant and any related demo customers.
        /// </summary>
        /// <param name="merchantGuid">The merchant unique identifier.</param>
        /// <returns>MerchantDBE.</returns>
        /// <exception cref="ApplicationException">Merchant: [{merchantGuid}] is not valid</exception>
        internal static MerchantDBE GetMerchantAndDemoCustomers(Guid merchantGuid)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbMerchantAndDemoCustomers = context.Merchants
                                                        .Include(m => m.DemoCustomers)
                                                        .FirstOrDefault(m => m.MerchantGuid == merchantGuid);

                return dbMerchantAndDemoCustomers;
            }
        }

        /// <summary>
        /// Gets a specific merchant using the internal id.
        /// </summary>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <returns>MerchantDBE.</returns>
        internal static MerchantDBE GetMerchant(int merchantId)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbMerchant = context.Merchants.FirstOrDefault(m => m.MerchantId == merchantId);

                return dbMerchant;
            }
        }

        /// <summary>
        /// Inserts the merchant 
        /// </summary>
        /// <param name="newMerchant">The new merchant.</param>
        /// <returns>MerchantDBE.</returns>
        /// <remarks>
        /// only used by the ResetDB method so we can keep the same guids across reloads.
        /// </remarks>
        internal static MerchantDBE InsertMerchant(MerchantDBE newMerchant)
        {
            using (var context = new SQLiteDBContext())
            {
                context.Merchants.Add(newMerchant);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // exception was raised by the db (ex: UK violation)
                    var sqlException = ex.InnerException;

                    // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                    throw new ApplicationException(sqlException.Message);
                }
                catch (Exception)
                {
                    // rethrow exception
                    throw;
                }

                return newMerchant;
            }
        }

        /// <summary>
        /// Deletes the merchant.
        /// </summary>
        /// <param name="merchantID">The merchant unique identifier.</param>
        internal static bool DeleteMerchant(int merchantID)
        {
            using (var context = new SQLiteDBContext())
            {
                var currentMerchant = context.Merchants.FirstOrDefault(m => m.MerchantId == merchantID);

                if (currentMerchant != null)
                {
                    context.Merchants.Remove(currentMerchant);
                    context.SaveChanges();

                    return true;
                }
                else
                {
                    // returns false if the merchant did not exist
                    return false;
                }
            }
        }

        /// <summary>
        /// Updates the merchant.
        /// </summary>
        /// <param name="merchant">The merchant.</param>
        /// <exception cref="ApplicationException">Merchant: [{merchant.MerchantID}] is not valid</exception>
        internal static void UpdateMerchant(MerchantDBE merchant)
        {
            using (var context = new SQLiteDBContext())
            {
                // turn off change tracking since we are going to overwite the entity
                // Note: I would not do this if there was a db assigned unique id for the record
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                // try and find the existing merchant
                var currentMerchant = context.Merchants.FirstOrDefault(m => m.MerchantGuid == merchant.MerchantGuid);

                if (currentMerchant == null)
                {
                    throw new ApplicationException($"Merchant: [{merchant.MerchantGuid}] is not valid");
                }

                context.Update(merchant);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // exception was raised by the db (ex: UK violation)
                    var sqlException = ex.InnerException;

                    // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                    throw new ApplicationException(sqlException.Message);
                }
                catch (Exception)
                {
                    // rethrow exception
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the active merchant.
        /// </summary>
        /// <param name="merchantId">The merchant identifier.</param>
        internal static void SetActiveMerchantForDemo(int merchantId)
        {
            //gets all merchants who are already active (logically should only be 0 or 1)
            var merchantsToChange = SQLiteDBContext.GetAllMerchants().Where(am => am.IsActive).ToList();

            //set other active merchant to inactive
            foreach (var merchant in merchantsToChange)
            {
                merchant.IsActive = false;
                SQLiteDBContext.UpdateMerchant(merchant);
            }

            // query the DB
            var activeMerchant = SQLiteDBContext.GetMerchant(merchantId);

            //set merchant to active
            activeMerchant.IsActive = true;

            //update merchant in the db
            SQLiteDBContext.UpdateMerchant(activeMerchant);
        }

        /// <summary>
        /// Gets active merchant
        /// </summary>
        /// <returns>active merchant</returns>
        internal static MerchantDBE GetActiveMerchant()
        {
            using (var context = new SQLiteDBContext())
            {
                //query the db to get active merchant
                var activeMerchant = context.Merchants.Where(m => m.IsActive).FirstOrDefault();

                return activeMerchant;
            }
        }


        #endregion

        #region ==== DemoCustomer Methods =======

        /// <summary>
        /// Gets the demo customers.
        /// </summary>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <returns>List&lt;CustomerDBE&gt;.</returns>
        internal static List<DemoCustomerDBE> GetDemoCustomers(int merchantId)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbDemoCustomers = context.DemoCustomers.Where(m => m.MerchantId == merchantId).ToList();

                return dbDemoCustomers;
            }
        }

        /// <summary>
        /// Inserts new customer into DB (only used by the ResetDB method so we can keep the same guids across reloads).
        /// </summary>
        /// <param name="newDemoCustomer">object containing information for new customer</param>
        /// <returns></returns>
        /// <remarks>
        /// only used by the ResetDB method so we can keep the same guids across reloads.
        /// </remarks>
        internal static DemoCustomerDBE InsertDemoCustomer( DemoCustomerDBE newDemoCustomer)
        {
            using (var context = new SQLiteDBContext())
            {
                context.DemoCustomers.Add(newDemoCustomer);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // exception was raised by the db (ex: UK violation)
                    var sqlException = ex.InnerException;

                    // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                    throw new ApplicationException(sqlException.Message);
                }
                catch (Exception)
                {
                    // rethrow exception
                    throw;
                }

                return newDemoCustomer;
            }
        }

        /// <summary>
        /// Deletes the customer.
        /// </summary>
        /// <param name="demoCustomerId">The customer identifier.</param>
        /// <exception cref="ApplicationException">Customer: [{customerID}] on Merchant: [{merchantID}] is not valid</exception>
        internal static bool DeleteDemoCustomer(int demoCustomerId)
        {
            using (var context = new SQLiteDBContext())
            {
                var currentDemoCustomer = context.DemoCustomers.FirstOrDefault(c => c.DemoCustomerId == demoCustomerId);

                if (currentDemoCustomer != null)
                {
                    context.DemoCustomers.Remove(currentDemoCustomer);
                    context.SaveChanges();

                    return true;
                }
                else
                {
                    // returns false if the demoCustomer did not exist
                    return false;
                }
            }
        }

        /// <summary>
        /// Updates a customer
        /// </summary>
        /// <param name="demoCustomer">object containing information about customers</param>
        /// <exception cref="ApplicationException">Customer: [{customer.CustomerID}] is not valid</exception>
        public static void UpdateDemoCustomer(DemoCustomerDBE demoCustomer)
        {
            using (var context = new SQLiteDBContext())
            {
                // turn off change tracking since we are going to overwite the entity
                // Note: I would not do this if there was a db assigned unique id for the record
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                var currentCustomer = context.DemoCustomers.FirstOrDefault(dc => dc.DemoCustomerId == demoCustomer.DemoCustomerId);

                if (currentCustomer != null)
                {
                    context.Update(demoCustomer);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        // exception was raised by the db (ex: UK violation)
                        var sqlException = ex.InnerException;

                        // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                        throw new ApplicationException(sqlException.Message);
                    }
                    catch (Exception)
                    {
                        // rethrow exception
                        throw;
                    }
                }
                else
                {
                    throw new ApplicationException($"Customer: [{demoCustomer.DemoCustomerId}] is not valid on MerchantID: [{demoCustomer.MerchantId}]");
                }
            }
        }


        #endregion

        #region ==== CatalogItem Methods =======

        /// <summary>
        /// Gets the catalog items.
        /// </summary>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <returns>System.Collections.Generic.List&lt;PayAway.WebAPI.Entities.v1.CatalogItemDBE&gt;.</returns>
        internal static List<CatalogItemDBE> GetCatalogItems(int merchantId)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbCatalogItems = context.CatalogItems
                                                .Where(ci => ci.MerchantId == merchantId)
                                                .OrderBy(ci => ci.CatalogItemId)
                                                .ToList();

                return dbCatalogItems;
            }
        }

        /// <summary>
        /// Gets a catalog item.
        /// </summary>
        /// <param name="catalogItemGuid">The unique catalog item identifier.</param>
        /// <returns>System.Collections.Generic.List&lt;PayAway.WebAPI.Entities.v1.CatalogItemDBE&gt;.</returns>
        internal static CatalogItemDBE GetCatalogItem(Guid catalogItemGuid)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbCatalogItem = context.CatalogItems.FirstOrDefault(ci => ci.CatalogItemGuid == catalogItemGuid);

                return dbCatalogItem;
            }
        }

        /// <summary>
        /// Inserts the catalog item.
        /// </summary>
        /// <param name="newCatalogItem">The new catalog item.</param>
        /// <returns>CatalogItemDBE.</returns>
        /// <remarks>
        /// only used by the ResetDB method so we can keep the same guids across reloads.
        /// </remarks>
        private static CatalogItemDBE InsertCatalogItem(CatalogItemDBE newCatalogItem)
        {
            using (var context = new SQLiteDBContext())
            {
                context.CatalogItems.Add(newCatalogItem);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // exception was raised by the db (ex: UK violation)
                    var sqlException = ex.InnerException;

                    // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                    throw new ApplicationException(sqlException.Message);
                }
                catch (Exception)
                {
                    // rethrow exception
                    throw;
                }

                return newCatalogItem;
            }
        }


        /// <summary>
        /// Deletes the catalog item.
        /// </summary>
        /// <param name="catalogItemId">The catalog item identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool DeleteCatalogItem(int catalogItemId)
        {
            using (var context = new SQLiteDBContext())
            {
                var currentCatalogItem = context.CatalogItems.FirstOrDefault(ci => ci.CatalogItemId == catalogItemId);

                if (currentCatalogItem != null)
                {
                    context.CatalogItems.Remove(currentCatalogItem);
                    context.SaveChanges();

                    return true;
                }
                else
                {
                    // returns false if the CatalogItem did not exist
                    return false;
                }
            }
        }
        #endregion

        #region ==== Order Methods =======

        /// <summary>
        /// Gets a list of Orders
        /// </summary>
        /// <param name="merchantId">Merchant Unique Indentifier.</param>
        /// <returns>List of orders</returns>
        internal static List<OrderDBE> GetOrders(int merchantId)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbOrders = context.Orders
                                        .Where(o => o.MerchantId == merchantId)
                                        .OrderByDescending(o => o.OrderId)          // delegate sorting to DB
                                        .ToList();

                return dbOrders;
            }

        }

        /// <summary>
        /// Get a specific order by orderGuid
        /// </summary>
        /// <param name="orderGuid">order Unique Indentifier.</param>
        /// <returns>a specific order</returns>
        internal static OrderDBE GetOrder(Guid orderGuid)
        {
            using (var context = new SQLiteDBContext())
            {
                var dbOrder = context.Orders.FirstOrDefault(o => o.OrderGuid == orderGuid);

                return dbOrder;
            }

        }

        /// <summary>
        /// Get a specific order by orderGuid
        /// </summary>
        /// <param name="orderGuid">order Unique Indentifier.</param>
        /// <returns>a specific order</returns>
        internal static OrderDBE GetOrderExploded(Guid orderGuid)
        {
            using (var context = new SQLiteDBContext())
            {
                
                var dbOrder = context.Orders
                                    .Include(o => o.Merchant)
                                    .Include(o => o.OrderEvents)
                                    .Include(o => o.OrderLineItems)
                                    .FirstOrDefault(o => o.OrderGuid == orderGuid);
                if (dbOrder == null)
                {
                    return null;
                }
                // sort the order events descending by d/t in memory
                dbOrder.OrderEvents = dbOrder.OrderEvents.OrderByDescending(oe => oe.EventDateTimeUTC).ToList();

                return dbOrder;
                                
            }

        }

        /// <summary>
        /// Inserts new order
        /// </summary>
        /// <param name="newOrder">The new order</param>
        /// <returns>A new order.</returns>
        /// <remarks>
        /// only used by the ResetDB method so we can keep the same guids across reloads.
        /// </remarks>
        internal static OrderDBE InsertOrder(OrderDBE newOrder)
        {
            using (var context = new SQLiteDBContext())
            {
                context.Orders.Add(newOrder);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // exception was raised by the db (ex: UK violation)
                    var sqlException = ex.InnerException;

                    // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                    throw new ApplicationException(sqlException.Message);
                }
                catch (Exception)
                {
                    // rethrow exception
                    throw;
                }

                return newOrder;
            }
        }

        /// <summary>
        /// Deletes order
        /// </summary>
        /// <param name="orderID">Identifier for order</param>
        /// <returns></returns>
        internal static bool DeleteOrder(int orderID)
        {
            using (var context = new SQLiteDBContext())
            {
                var currentOrder = context.Orders.FirstOrDefault(o => o.OrderId == orderID);

                if (currentOrder != null)
                {
                    context.Orders.Remove(currentOrder);
                    context.SaveChanges();

                    return true;
                }
                else
                {
                    // returns false if the order did not exist
                    return false;
                }
            }
        }

        /// <summary>
        /// Updates an order
        /// </summary>
        /// <param name="order">The order</param>
        /// <exception cref="ApplicationException">Order: [{order.OrderGuid}] is not valid</exception>
        internal static void UpdateOrder(OrderDBE order)
        {
            using (var context = new SQLiteDBContext())
            {
                // turn off change tracking since we are going to overwite the entity
                // Note: I would not do this if there was a db assigned unique id for the record
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                var currentOrder = context.Orders.FirstOrDefault(o => o.OrderGuid == order.OrderGuid);

                if (currentOrder == null)
                {
                    throw new ApplicationException($"Order: [{order.OrderGuid}] is not valid");
                }

                context.Update(order);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // exception was raised by the db (ex: UK violation)
                    var sqlException = ex.InnerException;

                    // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                    throw new ApplicationException(sqlException.Message);
                }
                catch (Exception)
                {
                    // rethrow exception
                    throw;
                }
            }
        }

        #endregion

        #region ==== OrderEvent Methods =======

        /// <summary>
        /// Inserts new order event
        /// </summary>
        /// <param name="newOrderEvent">The new order event.</param>
        /// <returns>OrderEventDBE</returns>
        /// <remarks>
        /// only used by the ResetDB method so we can keep the same guids across reloads.
        /// </remarks>
        internal static OrderEventDBE InsertOrderEvent(OrderEventDBE newOrderEvent)
        {
            using (var context = new SQLiteDBContext())
            {
                context.OrderEvents.Add(newOrderEvent);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // exception was raised by the db (ex: UK violation)
                    var sqlException = ex.InnerException;

                    // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                    throw new ApplicationException(sqlException.Message);
                }
                catch (Exception)
                {
                    // rethrow exception
                    throw;
                }

                return newOrderEvent;
            }
        }

        #endregion

        #region ==== OrderLineItem Methods =======

        /// <summary>
        /// Inserts new line item
        /// </summary>
        /// <param name="newLineItem"></param>
        /// <returns></returns>
        /// <remarks>
        /// only used by the ResetDB method so we can keep the same guids across reloads.
        /// </remarks>
        internal static OrderLineItemDBE InsertOrderLineItem(OrderLineItemDBE newLineItem)
        {
            using (var context = new SQLiteDBContext())
            {
                context.OrderLineItems.Add(newLineItem);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // exception was raised by the db (ex: UK violation)
                    var sqlException = ex.InnerException;

                    // we do this to disconnect the exception that bubbles up from the dbcontext which will be disposed when it leaves this method
                    throw new ApplicationException(sqlException.Message);
                }
                catch (Exception)
                {
                    // rethrow exception
                    throw;
                }

                return newLineItem;
            }
        }

        /// <summary>
        /// Delete Order line items
        /// </summary>
        /// <param name="orderId"></param>
        internal static void DeleteOrderLineItems(int orderId)
        {
            using (var context = new SQLiteDBContext())
            {
                var orderLineItems = context.OrderLineItems.Where(a => a.OrderId == orderId);
                foreach(var orderLineItem in orderLineItems)
                {
                    context.Remove(orderLineItem);
                    context.SaveChanges();
                }
                
            }
        }
        #endregion

    }
}
