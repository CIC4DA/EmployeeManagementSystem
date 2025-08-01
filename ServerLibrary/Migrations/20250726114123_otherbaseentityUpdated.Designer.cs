﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServerLibrary.Data;

#nullable disable

namespace ServerLibrary.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250726114123_otherbaseentityUpdated")]
    partial class otherbaseentityUpdated
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BaseLibrary.Entities.ApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fullname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ApplicationUsers");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("BaseLibrary.Entities.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GeneralDepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GeneralDepartmentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("CivilId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Other")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Photo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TelephoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TownId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("TownId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("BaseLibrary.Entities.GeneralDepartment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("GeneralDepartments");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Doctor.Doctor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("EmployeeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicalDiagnosis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicalRecommendation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Overtime.Overtime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("EmployeeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("OvertimeTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("OvertimeTypeId");

                    b.ToTable("Overtimes");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Overtime.OvertimeType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OvertimeTypes");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Sanction.Sanction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("EmployeeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Punishment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PunishmentDate")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<int>("SanctionTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SanctionTypeId");

                    b.ToTable("Sanctions");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Sanction.SanctionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SanctionTypes");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Vacation.Vacation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("EmployeeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfDays")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("VacationTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VacationTypeId");

                    b.ToTable("Vacations");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Vacation.VacationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("VacationTypes");
                });

            modelBuilder.Entity("BaseLibrary.Entities.RefreshTokenInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RefreshTokenInfos");
                });

            modelBuilder.Entity("BaseLibrary.Entities.SystemRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SystemRoles");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Town", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Towns");
                });

            modelBuilder.Entity("BaseLibrary.Entities.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Branch", b =>
                {
                    b.HasOne("BaseLibrary.Entities.Department", "Department")
                        .WithMany("Branches")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("BaseLibrary.Entities.City", b =>
                {
                    b.HasOne("BaseLibrary.Entities.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Department", b =>
                {
                    b.HasOne("BaseLibrary.Entities.GeneralDepartment", "GeneralDepartment")
                        .WithMany("Departments")
                        .HasForeignKey("GeneralDepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GeneralDepartment");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Employee", b =>
                {
                    b.HasOne("BaseLibrary.Entities.Branch", "Branch")
                        .WithMany("Employees")
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaseLibrary.Entities.Town", "Town")
                        .WithMany("Employees")
                        .HasForeignKey("TownId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Town");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Overtime.Overtime", b =>
                {
                    b.HasOne("BaseLibrary.Entities.OtherEntities.Overtime.OvertimeType", "OvertimeType")
                        .WithMany("Overtimes")
                        .HasForeignKey("OvertimeTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OvertimeType");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Sanction.Sanction", b =>
                {
                    b.HasOne("BaseLibrary.Entities.OtherEntities.Sanction.SanctionType", "SanctionType")
                        .WithMany("Sanctions")
                        .HasForeignKey("SanctionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SanctionType");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Vacation.Vacation", b =>
                {
                    b.HasOne("BaseLibrary.Entities.OtherEntities.Vacation.VacationType", "VacationType")
                        .WithMany("Vacations")
                        .HasForeignKey("VacationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VacationType");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Town", b =>
                {
                    b.HasOne("BaseLibrary.Entities.City", "City")
                        .WithMany("Towns")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Branch", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("BaseLibrary.Entities.City", b =>
                {
                    b.Navigation("Towns");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Country", b =>
                {
                    b.Navigation("Cities");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Department", b =>
                {
                    b.Navigation("Branches");
                });

            modelBuilder.Entity("BaseLibrary.Entities.GeneralDepartment", b =>
                {
                    b.Navigation("Departments");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Overtime.OvertimeType", b =>
                {
                    b.Navigation("Overtimes");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Sanction.SanctionType", b =>
                {
                    b.Navigation("Sanctions");
                });

            modelBuilder.Entity("BaseLibrary.Entities.OtherEntities.Vacation.VacationType", b =>
                {
                    b.Navigation("Vacations");
                });

            modelBuilder.Entity("BaseLibrary.Entities.Town", b =>
                {
                    b.Navigation("Employees");
                });
#pragma warning restore 612, 618
        }
    }
}
