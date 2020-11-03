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
                });
#pragma warning restore 612, 618
        }
    }
}
