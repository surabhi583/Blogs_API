using Microsoft.EntityFrameworkCore;
using Blogs.Core.Entities;

namespace Blogs.Data.Contexts;

public class BlogsContext(DbContextOptions<BlogsContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Article> Articles { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<UserLink> FollowedUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(x => x.ArticleComments)
                .WithOne(x => x.Author);
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasOne(x => x.Article)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.ArticleId);
            entity.HasOne(x => x.Author)
                .WithMany(x => x.ArticleComments)
                .HasForeignKey(x => x.Username);
        });

        modelBuilder.Entity<UserLink>(entity =>
        {
            entity.HasKey(x => new { x.Username, x.FollowerUsername });
            entity.HasOne(x => x.User)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.Username);

            entity.HasOne(x => x.FollowerUser)
                .WithMany(x => x.FollowedUsers)
                .HasForeignKey(x => x.FollowerUsername);
        });
    }
}
