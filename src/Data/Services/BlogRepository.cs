using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blogs.Core.Dto;
using Blogs.Core.Entities;
using Blogs.Core.Services.Interfaces;
using Blogs.Data.Contexts;

namespace Blogs.Data.Services;

public class BlogRepository(BlogsContext context) : IBlogRepository
{
    public async Task AddUserAsync(User user)
    {
        if (await context.Users.AnyAsync(x => x.Username == user.Username))
        {
            throw new ProblemDetailsException(new ValidationProblemDetails
            {
                Status = 422,
                Detail = "Cannot register user",
                Errors = { new KeyValuePair<string, string[]>("Username", new[] { "Username not available" }) }
            });
        }

        if (await context.Users.AnyAsync(x => x.Email == user.Email))
        {
            throw new ProblemDetailsException(new ValidationProblemDetails
            {
                Status = 422,
                Detail = "Cannot register user",
                Errors = { new KeyValuePair<string, string[]>("Email", new[] { "Email address already in use" }) }
            });
        }

        context.Users.Add(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public Task<User> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return context.Users.FirstAsync(x => x.Username == username, cancellationToken);
    }

    public async Task<UsersResponseDto> GetUsersAsync(UsersQuery usersQuery, CancellationToken cancellationToken)
    {
        var query = context.Users.Select(x => x);

        var total = await query.CountAsync(cancellationToken);
        var pageQuery = query
            .Skip(usersQuery.Offset).Take(usersQuery.Limit)
            .AsNoTracking();

        var users = await pageQuery.ToListAsync(cancellationToken);

        return new UsersResponseDto(users, total);
    }

    public void DeleteUser(User user)
    {
        context.Users.Remove(user);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ArticlesResponseDto> GetArticlesAsync(
        ArticlesQuery articlesQuery,
        string? username,
        bool isFeed,
        CancellationToken cancellationToken)
    {
        var query = context.Articles.Select(x => x);

        if (!string.IsNullOrWhiteSpace(articlesQuery.Author))
        {
            query = query.Where(x => x.Author.Username == articlesQuery.Author);
        }

        query = query.Include(x => x.Author);

        if (username is not null)
        {
            query = query.Include(x => x.Author)
                .ThenInclude(x => x.Followers.Where(fu => fu.FollowerUsername == username));
        }

        if (isFeed)
        {
            query = query.Where(x => x.Author.Followers.Any());
        }

        var total = await query.CountAsync(cancellationToken);
        var pageQuery = query
            .Skip(articlesQuery.Offset).Take(articlesQuery.Limit)
            .Include(x => x.Author)
            .AsNoTracking();

        var articles = await pageQuery.ToListAsync(cancellationToken);

        return new ArticlesResponseDto(articles, total);
    }

    public async Task<Article?> GetArticleBySlugAsync(string slug, bool asNoTracking,
        CancellationToken cancellationToken)
    {
        var query = context.Articles
            .Include(x => x.Author);

        if (asNoTracking)
        {
            query.AsNoTracking();
        }

        var article = await query
            .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

        if (article == null)
        {
            return article;
        }

        return article;
    }

    public void AddArticle(Article article)
    {
        context.Articles.Add(article);
    }

    public void DeleteArticle(Article article)
    {
        context.Articles.Remove(article);
    }

    public void AddArticleComment(Comment comment)
    {
        context.Comments.Add(comment);
    }

    public void RemoveArticleComment(Comment comment)
    {
        context.Comments.Remove(comment);
    }

    public async Task<List<Comment>> GetCommentsBySlugAsync(string slug, string? username,
        CancellationToken cancellationToken)
    {
        return await context.Comments.Where(x => x.Article.Slug == slug)
            .Include(x => x.Author)
            .ThenInclude(x => x.Followers.Where(fu => fu.FollowerUsername == username))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsFollowingAsync(string username, string followerUsername, CancellationToken cancellationToken)
    {
        return context.FollowedUsers.AnyAsync(
            x => x.Username == username && x.FollowerUsername == followerUsername,
            cancellationToken);
    }

    public void Follow(string username, string followerUsername)
    {
        context.FollowedUsers.Add(new UserLink(username, followerUsername));
    }

    public void UnFollow(string username, string followerUsername)
    {
        context.FollowedUsers.Remove(new UserLink(username, followerUsername));
    }
}
