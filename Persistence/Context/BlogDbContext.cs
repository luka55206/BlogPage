using BlogPage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogPage.Persistence.Context;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PostTags> PostTags { get; set; }
    

}