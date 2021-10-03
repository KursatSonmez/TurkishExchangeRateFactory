﻿// <auto-generated />
using System;
using ExchangeRateFactory.Demo.Customize.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ExchangeRateFactory.Demo.Migrations
{
    [DbContext(typeof(ExchangeRateFactoryDbContext))]
    [Migration("20210929131226_AddExchangeRates")]
    partial class AddExchangeRates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ExchangeRateFactory.Data.Entities.ExchangeRate<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("BanknoteBuying")
                        .HasPrecision(18, 5)
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BanknoteSelling")
                        .HasPrecision(18, 5)
                        .HasColumnType("decimal(18,5)");

                    b.Property<string>("BulletinNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<decimal?>("CrossRateOther")
                        .HasPrecision(18, 5)
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal?>("CrossRateUSD")
                        .HasPrecision(18, 5)
                        .HasColumnType("decimal(18,5)");

                    b.Property<string>("CurrencyCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CurrencyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExchangeRateDate")
                        .HasColumnType("Date");

                    b.Property<decimal>("ForexBuying")
                        .HasPrecision(18, 5)
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("ForexSelling")
                        .HasPrecision(18, 5)
                        .HasColumnType("decimal(18,5)");

                    b.Property<string>("Isim")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("Date");

                    b.Property<int>("Unit")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("ExchangeRateDate", "CurrencyCode")
                        .IsUnique();

                    b.ToTable("ExchangeRates");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ExchangeRate<Guid>");
                });

            modelBuilder.Entity("ExchangeRateFactory.Demo.Data.Entities.ExchangeRate", b =>
                {
                    b.HasBaseType("ExchangeRateFactory.Data.Entities.ExchangeRate<System.Guid>");

                    b.HasDiscriminator().HasValue("ExchangeRate");
                });
#pragma warning restore 612, 618
        }
    }
}
