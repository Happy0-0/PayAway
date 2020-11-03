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
                .HasAnnotation("ProductVersion", "5.0.0-rc.2.20475.6");

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.CustomerDBE", b =>
                {
                    b.Property<Guid>("MerchantID")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CustomerID")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerPhoneNo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("MerchantID", "CustomerID");

                    b.HasIndex("MerchantID", "CustomerPhoneNo")
                        .IsUnique();

                    b.ToTable("Customers");

                    b.HasData(
                        new
                        {
                            MerchantID = new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"),
                            CustomerID = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"),
                            CustomerName = "Test Customer",
                            CustomerPhoneNo = "(513) 498-6016"
                        });
                });

            modelBuilder.Entity("PayAway.WebAPI.Entities.v1.MerchantDBE", b =>
                {
                    b.Property<Guid>("MerchantID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSupportsTips")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("MerchantName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("MerchantID");

                    b.ToTable("Merchants");

                    b.HasData(
                        new
                        {
                            MerchantID = new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"),
                            IsActive = true,
                            IsSupportsTips = true,
                            LogoUrl = "https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/4670e0dc-0335-4370-a3b1-24d9fa1dfdbf.png",
                            MerchantName = "Test Merchant #1"
                        },
                        new
                        {
                            MerchantID = new Guid("5d590431-95d2-4f8a-b2d9-6eb4d8cabc89"),
                            IsActive = false,
                            IsSupportsTips = true,
                            LogoUrl = "https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/062c5897-208a-486a-8c6a-76707b9c07eb.png",
                            MerchantName = "Test Merchant #2"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
