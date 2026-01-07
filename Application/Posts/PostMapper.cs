using BlogPage.Domain.Entities;

namespace BlogPage.Application.Posts;

public class PostMapper
{
    public static PostDto toDto(Post post) =>
        new PostDto(
            post.Id,
            post.Title, 
            post.Content, 
            post.PublishDate,
            post.Author.Username,
            post.PostTags
                .Select(pt => pt.Tag.Name)
                .ToList()
        );
}