﻿// <auto-generated />
using System;
using Emanate.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Emanate.Web.Migrations
{
    [DbContext(typeof(AdminDbContext))]
    partial class AdminDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("Emanate.Web.Model.DisplayDevice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("ProfileId");

                    b.Property<Guid?>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.HasIndex("TypeId");

                    b.ToTable("DisplayDevices");
                });

            modelBuilder.Entity("Emanate.Web.Model.DisplayDeviceProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("DisplayDeviceProfiles");
                });

            modelBuilder.Entity("Emanate.Web.Model.DisplayDeviceType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Icon")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("DisplayDeviceType");
                });

            modelBuilder.Entity("Emanate.Web.Model.DisplayDevice", b =>
                {
                    b.HasOne("Emanate.Web.Model.DisplayDeviceProfile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId");

                    b.HasOne("Emanate.Web.Model.DisplayDeviceType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("Emanate.Web.Model.DisplayDeviceProfile", b =>
                {
                    b.HasOne("Emanate.Web.Model.DisplayDeviceType", "Type")
                        .WithMany("Profiles")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
