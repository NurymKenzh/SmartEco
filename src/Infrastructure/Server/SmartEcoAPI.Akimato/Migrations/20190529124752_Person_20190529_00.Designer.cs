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
    [Migration("20190529124752_Person_20190529_00")]
    partial class Person_20190529_00
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.DataProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("DataProvider");
                });

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

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.KATO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AreaType");

                    b.Property<string>("Code");

                    b.Property<int>("EgovId");

                    b.Property<int>("Level");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.Property<int?>("ParentEgovId");

                    b.HasKey("Id");

                    b.ToTable("KATO");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.KazHydrometAirPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInformation");

                    b.Property<decimal>("EastLongitude");

                    b.Property<string>("Name");

                    b.Property<decimal>("NorthLatitude");

                    b.Property<int>("Number");

                    b.HasKey("Id");

                    b.ToTable("KazHydrometAirPost");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.KazHydrometSoilPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInformation");

                    b.Property<decimal>("EastLongitude");

                    b.Property<string>("Name");

                    b.Property<decimal>("NorthLatitude");

                    b.Property<int>("Number");

                    b.HasKey("Id");

                    b.ToTable("KazHydrometSoilPost");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.Layer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GeoServerName");

                    b.Property<string>("GeoServerWorkspace");

                    b.Property<int?>("Hour");

                    b.Property<int?>("KATOId");

                    b.Property<int?>("MeasuredParameterId");

                    b.Property<string>("NameEN");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.Property<int?>("PollutionEnvironmentId");

                    b.Property<int?>("Season");

                    b.HasKey("Id");

                    b.HasIndex("KATOId");

                    b.HasIndex("MeasuredParameterId");

                    b.HasIndex("PollutionEnvironmentId");

                    b.ToTable("Layer");
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

                    b.Property<DateTime?>("DateTime");

                    b.Property<long?>("Ecomontimestamp_ms");

                    b.Property<int?>("MaxValueDay");

                    b.Property<int?>("MaxValueMonth");

                    b.Property<decimal?>("MaxValuePerMonth");

                    b.Property<decimal?>("MaxValuePerYear");

                    b.Property<int>("MeasuredParameterId");

                    b.Property<int?>("MonitoringPostId");

                    b.Property<int?>("Month");

                    b.Property<int?>("PollutionSourceId");

                    b.Property<decimal?>("Value");

                    b.Property<int?>("Year");

                    b.HasKey("Id");

                    b.HasIndex("MeasuredParameterId");

                    b.HasIndex("MonitoringPostId");

                    b.HasIndex("PollutionSourceId");

                    b.ToTable("MeasuredData");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.MeasuredParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("EcomonCode");

                    b.Property<decimal?>("MPC");

                    b.Property<string>("NameEN");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.HasKey("Id");

                    b.ToTable("MeasuredParameter");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.MonitoringPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInformation");

                    b.Property<int>("DataProviderId");

                    b.Property<decimal>("EastLongitude");

                    b.Property<string>("Name");

                    b.Property<decimal>("NorthLatitude");

                    b.Property<int>("Number");

                    b.Property<int>("PollutionEnvironmentId");

                    b.HasKey("Id");

                    b.HasIndex("DataProviderId");

                    b.HasIndex("PollutionEnvironmentId");

                    b.ToTable("MonitoringPost");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("Role");

                    b.HasKey("Id");

                    b.ToTable("Person");
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

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.PollutionEnvironment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NameEN");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.HasKey("Id");

                    b.ToTable("PollutionEnvironment");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.PollutionSource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("EastLongitude");

                    b.Property<string>("Name");

                    b.Property<decimal>("NorthLatitude");

                    b.HasKey("Id");

                    b.ToTable("PollutionSource");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.PollutionSourceData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("PollutantId");

                    b.Property<int>("PollutionSourceId");

                    b.Property<decimal>("Value");

                    b.HasKey("Id");

                    b.HasIndex("PollutantId");

                    b.HasIndex("PollutionSourceId");

                    b.ToTable("PollutionSourceData");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.Layer", b =>
                {
                    b.HasOne("SmartEcoAPI.Akimato.Models.KATO", "KATO")
                        .WithMany()
                        .HasForeignKey("KATOId");

                    b.HasOne("SmartEcoAPI.Akimato.Models.MeasuredParameter", "MeasuredParameter")
                        .WithMany()
                        .HasForeignKey("MeasuredParameterId");

                    b.HasOne("SmartEcoAPI.Akimato.Models.PollutionEnvironment", "PollutionEnvironment")
                        .WithMany()
                        .HasForeignKey("PollutionEnvironmentId");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.MeasuredData", b =>
                {
                    b.HasOne("SmartEcoAPI.Akimato.Models.MeasuredParameter", "MeasuredParameter")
                        .WithMany()
                        .HasForeignKey("MeasuredParameterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmartEcoAPI.Akimato.Models.MonitoringPost", "MonitoringPost")
                        .WithMany()
                        .HasForeignKey("MonitoringPostId");

                    b.HasOne("SmartEcoAPI.Akimato.Models.PollutionSource", "PollutionSource")
                        .WithMany()
                        .HasForeignKey("PollutionSourceId");
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.MonitoringPost", b =>
                {
                    b.HasOne("SmartEcoAPI.Akimato.Models.DataProvider", "DataProvider")
                        .WithMany()
                        .HasForeignKey("DataProviderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmartEcoAPI.Akimato.Models.PollutionEnvironment", "PollutionEnvironment")
                        .WithMany()
                        .HasForeignKey("PollutionEnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmartEcoAPI.Akimato.Models.PollutionSourceData", b =>
                {
                    b.HasOne("SmartEcoAPI.Akimato.Models.Pollutant", "Pollutant")
                        .WithMany()
                        .HasForeignKey("PollutantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmartEcoAPI.Akimato.Models.PollutionSource", "PollutionSource")
                        .WithMany()
                        .HasForeignKey("PollutionSourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
