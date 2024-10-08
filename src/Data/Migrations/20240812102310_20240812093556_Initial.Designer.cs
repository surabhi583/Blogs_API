﻿// <auto-generated />
using System;
using Blogs.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Blogs.Data.Migrations
{
    [DbContext(typeof(BlogsContext))]
    [Migration("20240812102310_20240812093556_Initial")]
    partial class _20240812093556_Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Blogs.Core.Entities.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuthorUsername")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("AuthorUsername");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Blogs.Core.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("ArticleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("Username");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Blogs.Core.Entities.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Username");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Blogs.Core.Entities.UserLink", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FollowerUsername")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Username", "FollowerUsername");

                    b.HasIndex("FollowerUsername");

                    b.ToTable("FollowedUsers");
                });

            modelBuilder.Entity("Blogs.Core.Entities.Article", b =>
                {
                    b.HasOne("Blogs.Core.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Blogs.Core.Entities.Comment", b =>
                {
                    b.HasOne("Blogs.Core.Entities.Article", "Article")
                        .WithMany("Comments")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Blogs.Core.Entities.User", "Author")
                        .WithMany("ArticleComments")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Blogs.Core.Entities.UserLink", b =>
                {
                    b.HasOne("Blogs.Core.Entities.User", "FollowerUser")
                        .WithMany("FollowedUsers")
                        .HasForeignKey("FollowerUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Blogs.Core.Entities.User", "User")
                        .WithMany("Followers")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FollowerUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Blogs.Core.Entities.Article", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Blogs.Core.Entities.User", b =>
                {
                    b.Navigation("ArticleComments");

                    b.Navigation("FollowedUsers");

                    b.Navigation("Followers");
                });
#pragma warning restore 612, 618
        }
    }
}
