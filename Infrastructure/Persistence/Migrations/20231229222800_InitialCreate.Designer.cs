﻿// <auto-generated />
using System;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231229222800_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "english_level", new[] { "pre_intermediate", "intermediate", "upper_intermediate", "advanced" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "payment_status", new[] { "pending", "to_be_approved", "paid_by_card", "to_be_paid_by_cash", "cancelled" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "role", new[] { "member", "admin" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OrdinalNumber")
                        .HasColumnType("integer");

                    b.Property<Guid>("SpeakingId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SpeakingId");

                    b.ToTable("Photo");
                });

            modelBuilder.Entity("Domain.Entities.Registration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<PaymentStatus>("PaymentStatus")
                        .HasColumnType("payment_status");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("SpeakingId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SpeakingId");

                    b.HasIndex("UserId");

                    b.ToTable("Registrations");
                });

            modelBuilder.Entity("Domain.Entities.Source", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsCustom")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Sources");

                    b.HasData(
                        new
                        {
                            Id = new Guid("4ee0db47-324f-499c-a4d4-584b5b065f7c"),
                            IsCustom = false,
                            Title = "Рекомендація від друзів"
                        },
                        new
                        {
                            Id = new Guid("94a4d26e-7684-4cd9-9bf6-63e250ac3713"),
                            IsCustom = false,
                            Title = "Реклама в інстаграмі"
                        },
                        new
                        {
                            Id = new Guid("e66d6631-33d6-4d74-9ba0-e6a87658e377"),
                            IsCustom = false,
                            Title = "Кафе \"Buono\""
                        },
                        new
                        {
                            Id = new Guid("e31133c4-578f-4abc-a678-cfcb258354a3"),
                            IsCustom = false,
                            Title = "Кафе \"Nest City Cafe\""
                        },
                        new
                        {
                            Id = new Guid("51786e0f-ebf3-46ba-9e40-cab616a32588"),
                            IsCustom = false,
                            Title = "News Brovary"
                        },
                        new
                        {
                            Id = new Guid("6c60c909-c857-4006-834b-f0a55ee0c936"),
                            IsCustom = false,
                            Title = "Кафе \"І не тільки\""
                        },
                        new
                        {
                            Id = new Guid("963e4265-8d78-4107-ae64-e352aad047b6"),
                            IsCustom = false,
                            Title = "Книжковий клуб \"Культурні\""
                        },
                        new
                        {
                            Id = new Guid("c8825caf-2504-4af3-9596-4a8392a03fc9"),
                            IsCustom = false,
                            Title = "Телеграм канал SpeakUp"
                        },
                        new
                        {
                            Id = new Guid("5f854f90-b8be-44b9-8a7a-7704f489fef6"),
                            IsCustom = false,
                            Title = "Інстраграм сторінка SpeakUp"
                        });
                });

            modelBuilder.Entity("Domain.Entities.Speaking", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DurationMinutes")
                        .HasColumnType("integer");

                    b.Property<string>("Intro")
                        .HasColumnType("text");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.Property<int>("Seats")
                        .HasColumnType("integer");

                    b.Property<DateTime>("TimeOfEvent")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("VenueId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("VenueId");

                    b.ToTable("Speakings");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<EnglishLevel>("EnglishLevel")
                        .HasColumnType("english_level");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Role>("Role")
                        .HasColumnType("role");

                    b.Property<Guid>("SourceId")
                        .HasColumnType("uuid");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<bool>("TransferTicket")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("SourceId");

                    b.HasIndex("TelegramId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entities.Venue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("InstagramUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LocationUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("Domain.Entities.Photo", b =>
                {
                    b.HasOne("Domain.Entities.Speaking", "Speaking")
                        .WithMany("Photos")
                        .HasForeignKey("SpeakingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Speaking");
                });

            modelBuilder.Entity("Domain.Entities.Registration", b =>
                {
                    b.HasOne("Domain.Entities.Speaking", "Speaking")
                        .WithMany("Registrations")
                        .HasForeignKey("SpeakingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Registrations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Speaking");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.Speaking", b =>
                {
                    b.HasOne("Domain.Entities.Venue", "Venue")
                        .WithMany("Speakings")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.HasOne("Domain.Entities.Source", "Source")
                        .WithMany("Users")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Source");
                });

            modelBuilder.Entity("Domain.Entities.Source", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Domain.Entities.Speaking", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("Registrations");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Navigation("Registrations");
                });

            modelBuilder.Entity("Domain.Entities.Venue", b =>
                {
                    b.Navigation("Speakings");
                });
#pragma warning restore 612, 618
        }
    }
}
