﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB;

#nullable disable

namespace WWC._240711.ASPNETCore.Extensions.Migrations
{
    [DbContext(typeof(ConfigDbContext))]
    [Migration("20240723131335_Init2")]
    partial class Init2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB.Entity.ConfigurationInfo", b =>
                {
                    b.Property<Guid>("ConfigKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar");

                    b.Property<string>("ParentID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(3000)
                        .HasColumnType("varchar");

                    b.HasKey("ConfigKey");

                    b.ToTable("ConfigurationInfos");
                });
#pragma warning restore 612, 618
        }
    }
}