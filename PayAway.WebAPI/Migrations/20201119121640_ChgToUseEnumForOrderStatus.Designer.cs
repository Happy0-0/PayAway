﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PayAway.WebAPI.DataAccess;

namespace PayAway.WebAPI.Migrations
{
    [DbContext(typeof(SQLiteDBContext))]
    [Migration("20201119121640_ChgToUseEnumForOrderStatus")]
    partial class ChgToUseEnumForOrderStatus
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-rc.2.20475.6");

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.CatalogItemDBE", b =>
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

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.DemoCustomerDBE", b =>
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

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.MerchantDBE", b =>
                {
                    b.Property<int>("MerchantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSupportsTips")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MerchantGuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("MerchantName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("MerchantId");

                    b.HasIndex("MerchantGuid")
                        .IsUnique();

                    b.HasIndex("MerchantName")
                        .IsUnique();

                    b.ToTable("Merchants");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.OrderDBE", b =>
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

                    b.Property<int>("MerchantId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("OrderDateTimeUTC")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrderGuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("OrderId");

                    b.HasIndex("MerchantId");

                    b.HasIndex("OrderGuid")
                        .IsUnique();

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.OrderEventDBE", b =>
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

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.OrderLineItemDBE", b =>
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

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.DemoCustomerDBE", b =>
                {
                    b.HasOne("PayAway.WebAPI.Entities.v1.MerchantDBE", "Merchant")
                        .WithMany("DemoCustomers")
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Merchant");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.OrderDBE", b =>
                {
                    b.HasOne("PayAway.WebAPI.Entities.v1.MerchantDBE", "Merchant")
                        .WithMany("Orders")
                        .HasForeignKey("MerchantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Merchant");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.OrderEventDBE", b =>
                {
                    b.HasOne("PayAway.WebAPI.Entities.v1.OrderDBE", "Order")
                        .WithMany("OrderEvents")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.OrderLineItemDBE", b =>
                {
                    b.HasOne("PayAway.WebAPI.Entities.v1.OrderDBE", "Order")
                        .WithMany("OrderLineItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.MerchantDBE", b =>
                {
                    b.Navigation("DemoCustomers");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.OrderDBE", b =>
                {
                    b.Navigation("OrderEvents");

                    b.Navigation("OrderLineItems");
                });
#pragma warning restore 612, 618
        }
    }
}
