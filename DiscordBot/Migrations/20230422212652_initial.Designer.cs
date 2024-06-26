﻿// <auto-generated />
using System;
using DiscordBot.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DiscordBot.Migrations
{
    [DbContext(typeof(BotContext))]
    [Migration("20230422212652_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DiscordBot.DataAccess.Models.Permission", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Command")
                        .HasColumnType("int");

                    b.Property<decimal>("PermissionRequired")
                        .HasColumnType("decimal(20,0)");

                    b.Property<Guid>("ServerConfigId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ServerConfigId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("PermissionId");

                    b.ToTable("AllowedRoles");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Models.SelfRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<Guid>("ServerConfigId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ServerConfigId");

                    b.ToTable("SelfRoles");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Models.ServerConfig", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("AutoRoleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal?>("ConfirmRoleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("MusicBotVoteTreshold")
                        .HasColumnType("int");

                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.ToTable("ServerConfigs");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Models.Permission", b =>
                {
                    b.HasOne("DiscordBot.DataAccess.Models.ServerConfig", "ServerConfig")
                        .WithMany("Permissions")
                        .HasForeignKey("ServerConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServerConfig");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Models.Role", b =>
                {
                    b.HasOne("DiscordBot.DataAccess.Models.Permission", "Permission")
                        .WithMany("AllowedRoles")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Models.SelfRole", b =>
                {
                    b.HasOne("DiscordBot.DataAccess.Models.ServerConfig", "ServerConfig")
                        .WithMany("SelfRoles")
                        .HasForeignKey("ServerConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServerConfig");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Models.Permission", b =>
                {
                    b.Navigation("AllowedRoles");
                });

            modelBuilder.Entity("DiscordBot.DataAccess.Models.ServerConfig", b =>
                {
                    b.Navigation("Permissions");

                    b.Navigation("SelfRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
