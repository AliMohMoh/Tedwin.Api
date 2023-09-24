using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Tedwin.Api.Model;

namespace Tedwin.Api.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BlogPost>()
             .HasOne(bp => bp.User)
             .WithMany()
             .HasForeignKey(bp => bp.UserId)
             .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BlogPostTags>()
            .HasKey(bpt => new { bpt.BlogPostId, bpt.TagId });

        modelBuilder.Entity<BlogPostTags>()
            .HasOne(bpt => bpt.BlogPost)
            .WithMany(bp => bp.BlogPostTags)
            .HasForeignKey(bpt => bpt.BlogPostId);

        modelBuilder.Entity<BlogPostTags>()
            .HasOne(bpt => bpt.Tag)
            .WithMany()
            .HasForeignKey(bpt => bpt.TagId);
    }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<BlogPostTags> BlogPostTags { get; set; }
}
