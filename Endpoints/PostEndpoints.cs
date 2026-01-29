using System.Security.Claims;
using BlogPage.Application.Posts;
using BlogPage.Domain.Entities;
using BlogPage.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using BlogPage.Application.Common;

namespace BlogPage.Endpoints;

public static class PostEndpoints
{
    public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
     //Post create post
     app.MapPost("/posts", async (CreatePostRequest request, ClaimsPrincipal userPrincipal, BlogDbContext db, PostService postService)
             =>
         {
             var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             if (userIdClaim is null)
                 return Results.Unauthorized();
             
             int authorId = int.Parse(userIdClaim);

             var postCommand = new CreatePostCommand(
                 authorId,
                 request.Title,
                 request.Content,
                 request.Tags);
             
             var postDto = await postService.CreatePostAsync(postCommand);
             
             return Results.Created($"/posts/{postDto.Id}", postDto); //{post.Id}
         }
     ).RequireAuthorization();

    //get all posts
     app.MapGet("/posts", async ([AsParameters] PostFilterParams filters,
             BlogDbContext db) =>
         {
           

             var query = db.Posts
                 .Include(p => p.Author)
                 .Include(p => p.PostTags)
                 .ThenInclude(pt => pt.Tag)
                 .AsQueryable();

             // Search filter
             if (!string.IsNullOrEmpty(filters.Search))
             {
                 var searchLower = filters.Search.ToLower();
                 query = query.Where(p =>
                     p.Title.ToLower().Contains(searchLower) ||
                     p.Content.ToLower().Contains(searchLower));
             }

             // Tag filter
             if (!string.IsNullOrEmpty(filters.Tag))
             {
                 var tagLower = filters.Tag.ToLower();
                 query = query.Where(p =>
                     p.PostTags.Any(pt => pt.Tag.Name == tagLower));
             }

             // Author filter
             if (filters.AuthorId.HasValue)
             {
                 query = query.Where(p => p.AuthorId == filters.AuthorId.Value);
             }

             // Sorting
             query = filters.SortBy?.ToLower() switch
             {
                 "title" => filters.SortOrder?.ToLower() == "asc"
                     ? query.OrderBy(p => p.Title)
                     : query.OrderByDescending(p => p.Title),
                 "author" => filters.SortOrder?.ToLower() == "asc"
                     ? query.OrderBy(p => p.Author.Username)
                     : query.OrderByDescending(p => p.Author.Username),
                 _ => filters.SortOrder?.ToLower() == "asc"
                     ? query.OrderBy(p => p.PublishDate)
                     : query.OrderByDescending(p => p.PublishDate)
             };

             var paginatedPosts = await query.ToPaginatedListAsync(
                 filters.Page,
                 filters.PageSize);

             return Results.Ok(new PaginatedResult<PostDto>
             {
                 Data = paginatedPosts.Data.Select(PostMapper.toDto).ToList(),
                 Page = paginatedPosts.Page,
                 PageSize = paginatedPosts.PageSize,
                 TotalCount = paginatedPosts.TotalCount,
                 TotalPages = paginatedPosts.TotalPages
             });
             
             

         }
     );
     
     
     app.MapGet("posts/{id}", async (int id, BlogDbContext db) =>
     {
         
         
         var post = await db.Posts
             .Include(p => p.Author)
             .Where(post => post.Id == id)
             .FirstOrDefaultAsync();
         
         
         if(post is null)
             return Results.NotFound();
         
         return Results.Ok(PostMapper.toDto(post));

     }
     );

     app.MapPut("/posts/{id}", async (int id, UpdatePostRequest request, ClaimsPrincipal userPrincipal, BlogDbContext db) =>
         {
            
             
             var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             
             if(userIdClaim is null)
                 return Results.Unauthorized();
             
             var postAuthorId = int.Parse(userIdClaim);
             
             var post = await db.Posts
                              .Include(p => p.Author)
                              .Include(p => p.PostTags)
                              .FirstOrDefaultAsync(p => p.Id == id);

             if (post is null)
             {
                 return Results.NotFound();
             }
             
             if(postAuthorId != post.AuthorId)
                 return Results.Forbid();
             
             
             post.Title =  request.title; 
             post.Content = request.content;

             await db.SaveChangesAsync();
             
             return Results.Ok(PostMapper.toDto(post));

         }
         
     );


     app.MapDelete("/posts/{id}", async (int id, ClaimsPrincipal userPrincipal, BlogDbContext db) => 
         {
             var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             
             if (userIdClaim is null)
                 return Results.Unauthorized();
             
             var authorId = int.Parse(userIdClaim);

             var post = await db.Posts
                 .Include(p => p.Author)
                 .FirstOrDefaultAsync(p => p.Id == id);
             
             if (post is null)
                 return Results.NotFound();
             
             
             if(authorId != post.AuthorId)
                 return Results.Forbid();
             
             db.Posts.Remove(post);
             await db.SaveChangesAsync();
             
             return Results.NoContent();
             
         }
     );



     return app;
    }
}