using BlogPage.Domain.Entities;
using BlogPage.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BlogPage.Application.Posts;

public class PostService
{
    private readonly BlogDbContext _db;

    public PostService(BlogDbContext db)
    {
        _db = db;
    }
    
    public  async Task<PostDto> CreatePostAsync(
        CreatePostCommand postCommand)
    {
        var cleanTags = postCommand.Tags
            .Select( t => t.Trim().ToLower() )
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();

        var existingTags = await _db.Tags
            .Where(t => cleanTags.Contains(t.Name))
            .ToListAsync();
        
        var newTags = cleanTags
            .Where(t => existingTags.All(et => et.Name != t))
            .Select(t => new Tag { Name = t })
            .ToList();

        _db.Tags.AddRange(newTags);
        //await _db.SaveChangesAsync();
        
        var allTags = existingTags.Concat(newTags).ToList();

        var post = new Post
        {
            AuthorId = postCommand.AuthorId,
            Title = postCommand.Title,
            Content = postCommand.Content,
            PublishDate = DateTime.UtcNow

        };
        
        
        _db.Posts.Add(post);
        //await _db.SaveChangesAsync();

        var postTags = allTags.Select(tag => new PostTags
        {
            Post = post,
            Tag = tag
        });
        
        _db.PostTags.AddRange(postTags);
        await _db.SaveChangesAsync();
        
             
             
        await _db.Entry(post)
            .Reference(p => p.Author)
            .LoadAsync();

        return PostMapper.toDto(post);
    }
}