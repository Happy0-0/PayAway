﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PayAway.WebAPI.DB;

namespace PayAway.WebAPI.Migrations
{
    [DbContext(typeof(SQLiteDBContext))]
    partial class SQLiteDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.CatalogItemDBE", b =>
                {
                    b.Property<int>("CatalogItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("CatalogItemGuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ItemUnitPrice")
                        .HasColumnType("TEXT");

                    b.Property<int>("MerchantId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CatalogItemId");

                    b.HasIndex("CatalogItemGuid")
                        .IsUnique();

                    b.HasIndex("MerchantId", "ItemName")
                        .IsUnique();

                    b.ToTable("CatalogItems");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.DemoCustomerDBE", b =>
                {
                    b.Property<int>("DemoCustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerPhoneNo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DemoCustomerGuid")
                        .HasColumnType("TEXT");

                    b.Property<int>("MerchantId")
                        .HasColumnType("INTEGER");

                    b.HasKey("DemoCustomerId");

                    b.HasIndex("DemoCustomerGuid")
                        .IsUnique();

                    b.HasIndex("MerchantId", "CustomerPhoneNo")
                        .IsUnique();

                    b.ToTable("DemoCustomers");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.MerchantDBE", b =>
                {
                    b.Property<int>("MerchantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSupportsTips")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LogoFileName")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MerchantGuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("MerchantName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MerchantUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("MerchantId");

                    b.HasIndex("MerchantGuid")
                        .IsUnique();

                    b.HasIndex("MerchantName")
                        .IsUnique();

                    b.ToTable("Merchants");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.OrderDBE", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AuthCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreditCardNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ExpMonth")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExpYear")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MerchantId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("OrderDateTimeUTC")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrderGuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("RefOrderId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("TipAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("OrderId");

                    b.HasIndex("MerchantId");

                    b.HasIndex("OrderGuid")
                        .IsUnique();

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.OrderEventDBE", b =>
                {
                    b.Property<int>("OrderEventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EventDateTimeUTC")
                        .HasColumnType("TEXT");

                    b.Property<string>("EventDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("INTEGER");

                    b.HasKey("OrderEventId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderEvents");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.OrderLineItemDBE", b =>
                {
                    b.Property<int>("OrderLineItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("CatalogItemGuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ItemUnitPrice")
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderId")
                        .HasColumnType("INTEGER");

                    b.HasKey("OrderLineItemId");

                    b.HasIndex("OrderId", "ItemName")
                        .IsUnique();

                    b.ToTable("OrderLineItems");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.DemoCustomerDBE", b =>
                {
                    b.HasOne("PayAway.WebAPI.Entities.Database.MerchantDBE", "Merchant")
                        .WithMany("DemoCustomers")
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Merchant");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.OrderDBE", b =>
                {
                    b.HasOne("PayAway.WebAPI.Entities.Database.MerchantDBE", "Merchant")
                        .WithMany("Orders")
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Merchant");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.OrderEventDBE", b =>
                {
                    b.HasOne("PayAway.WebAPI.Entities.Database.OrderDBE", "Order")
                        .WithMany("OrderEvents")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.OrderLineItemDBE", b =>
                {
                    b.HasOne("PayAway.WebAPI.Entities.Database.OrderDBE", "Order")
                        .WithMany("OrderLineItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.MerchantDBE", b =>
                {
                    b.Navigation("DemoCustomers");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.Database.OrderDBE", b =>
                {
                    b.Navigation("OrderEvents");

                    b.Navigation("OrderLineItems");
                });
#pragma warning restore 612, 618
        }
    }
}
