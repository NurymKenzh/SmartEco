﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SmartEcoAPI.Akimato.Data;

namespace SmartEcoAPI.Akimato.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190129163239_PollutionSource_20190129_00")]
    partial class PollutionSource_20190129_00
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.EcomonMonitoringPoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("EastLongitude");

                    b.Property<decimal>("NorthLatitude");

                    b.Property<int>("Number");

                    b.HasKey("Id");

                    b.ToTable("EcomonMonitoringPoint");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTimeEnd");

                    b.Property<DateTime>("DateTimeStart");

                    b.Property<decimal>("EastLongitude");

                    b.Property<decimal>("NorthLatitude");

                    b.Property<int>("Number");

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.MeasuredData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime");

                    b.Property<int?>("EcomonMonitoringPointId");

                    b.Property<long?>("Ecomontimestamp_ms");

                    b.Property<int>("MeasuredParameterId");

                    b.Property<decimal>("Value");

                    b.HasKey("Id");

                    b.HasIndex("EcomonMonitoringPointId");

                    b.HasIndex("MeasuredParameterId");

                    b.ToTable("MeasuredData");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.MeasuredParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("EcomonCode");

                    b.Property<string>("NameEN");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.HasKey("Id");

                    b.ToTable("MeasuredParameter");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.Pollutant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NameEN");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.HasKey("Id");

                    b.ToTable("Pollutant");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.PollutionSource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("PollutionSource");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.MeasuredData", b =>
                {
                    b.HasOne("SmartEcoAPI.Akimato.Models.EcomonMonitoringPoint", "EcomonMonitoringPoint")
                        .WithMany()
                        .HasForeignKey("EcomonMonitoringPointId");

                    b.HasOne("SmartEcoAPI.Akimato.Models.MeasuredParameter", "MeasuredParameter")
                        .WithMany()
                        .HasForeignKey("MeasuredParameterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
