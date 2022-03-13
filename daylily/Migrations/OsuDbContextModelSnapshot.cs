﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using daylily.Data;

#nullable disable

namespace daylily.Migrations
{
    [DbContext(typeof(OsuDbContext))]
    partial class OsuDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("daylily.Data.OsuToken", b =>
                {
                    b.Property<long>("QQ")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("qq");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<long>("ExpiresIn")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TokenType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("QQ");

                    b.ToTable("tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
