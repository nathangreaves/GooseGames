﻿// <auto-generated />
using System;
using MSSQLRepository.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MSSQLRepository.Migrations.Global
{
    [DbContext(typeof(GlobalContext))]
    [Migration("20201106234457_NullableGame")]
    partial class NullableGame
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Global")
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Entities.Global.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayerNumber")
                        .HasColumnType("int");

                    b.Property<Guid?>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Entities.Global.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Game")
                        .HasColumnType("int");

                    b.Property<Guid?>("GameSessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("SessionMasterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Password")
                        .IsUnique()
                        .HasFilter("[Password] IS NOT NULL");

                    b.HasIndex("SessionMasterId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Entities.Global.Player", b =>
                {
                    b.HasOne("Entities.Global.Session", "Session")
                        .WithMany("Players")
                        .HasForeignKey("SessionId");
                });

            modelBuilder.Entity("Entities.Global.Session", b =>
                {
                    b.HasOne("Entities.Global.Player", "SessionMaster")
                        .WithMany()
                        .HasForeignKey("SessionMasterId");
                });
#pragma warning restore 612, 618
        }
    }
}
