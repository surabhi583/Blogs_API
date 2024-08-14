namespace Blogs.Core.Services.Interfaces;

public interface IBlogRepository
{
    public Task AddUserAsync(User user);

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    public Task<User> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);

    public Task<UsersResponseDto> GetUsersAsync(UsersQuery usersQuery, CancellationToken cancellationToken);

    public void DeleteUser(User user);

    public Task SaveChangesAsync(CancellationToken cancellationToken);

    public Task<ArticlesResponseDto> GetArticlesAsync(ArticlesQuery articlesQuery, string? username, bool isFeed,
        CancellationToken cancellationToken);

    public Task<Article?> GetArticleBySlugAsync(string slug, bool asNoTracking, CancellationToken cancellationToken);

    public void AddArticle(Article article);

    public void DeleteArticle(Article article);
    public void AddArticleComment(Comment comment);
    public void RemoveArticleComment(Comment comment);

    public Task<List<Comment>>
        GetCommentsBySlugAsync(string slug, string? username, CancellationToken cancellationToken);


    public Task<bool> IsFollowingAsync(string username, string followerUsername, CancellationToken cancellationToken);

    public void Follow(string username, string followerUsername);

    public void UnFollow(string username, string followerUsername);
}
