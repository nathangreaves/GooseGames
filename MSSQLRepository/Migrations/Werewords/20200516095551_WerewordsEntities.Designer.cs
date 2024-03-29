﻿// <auto-generated />
using System;
using MSSQLRepository.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MSSQLRepository.Migrations.Werewords
{
    [DbContext(typeof(WerewordsContext))]
    [Migration("20200516095551_WerewordsEntities")]
    partial class WerewordsEntities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Werewords")
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Entities.Werewords.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayerNumber")
                        .HasColumnType("int");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Status")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Entities.Werewords.PlayerRoundInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Correct")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Crosses")
                        .HasColumnType("int");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("QuestionMarks")
                        .HasColumnType("int");

                    b.Property<Guid>("RoundId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SecretRole")
                        .HasColumnType("int");

                    b.Property<int>("SoClose")
                        .HasColumnType("int");

                    b.Property<int>("Ticks")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("RoundId");

                    b.ToTable("PlayerRoundInformation");
                });

            modelBuilder.Entity("Entities.Werewords.PlayerVote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoundId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("VoteType")
                        .HasColumnType("int");

                    b.Property<Guid>("VotedPlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("RoundId");

                    b.HasIndex("VotedPlayerId");

                    b.ToTable("PlayerVotes");
                });

            modelBuilder.Entity("Entities.Werewords.Round", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("MayorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Outcome")
                        .HasColumnType("int");

                    b.Property<int>("RoundDurationMinutes")
                        .HasColumnType("int");

                    b.Property<DateTime>("RoundStartedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("SecretWord")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MayorId");

                    b.HasIndex("SessionId");

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("Entities.Werewords.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CurrentRoundId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SessionMasterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CurrentRoundId");

                    b.HasIndex("SessionMasterId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Entities.Werewords.Player", b =>
                {
                    b.HasOne("Entities.Werewords.Session", "Session")
                        .WithMany("Players")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entities.Werewords.PlayerRoundInformation", b =>
                {
                    b.HasOne("Entities.Werewords.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Werewords.Round", "Round")
                        .WithMany()
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entities.Werewords.PlayerVote", b =>
                {
                    b.HasOne("Entities.Werewords.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Entities.Werewords.Round", "Round")
                        .WithMany()
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Werewords.Player", "VotedPlayer")
                        .WithMany()
                        .HasForeignKey("VotedPlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entities.Werewords.Round", b =>
                {
                    b.HasOne("Entities.Werewords.Player", "Mayor")
                        .WithMany()
                        .HasForeignKey("MayorId");

                    b.HasOne("Entities.Werewords.Session", "Session")
                        .WithMany()
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Entities.Werewords.Session", b =>
                {
                    b.HasOne("Entities.Werewords.Round", "CurrentRound")
                        .WithMany()
                        .HasForeignKey("CurrentRoundId");

                    b.HasOne("Entities.Werewords.Player", "SessionMaster")
                        .WithMany()
                        .HasForeignKey("SessionMasterId");
                });
#pragma warning restore 612, 618
        }
    }
}
