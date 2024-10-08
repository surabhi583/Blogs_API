﻿using Comment = Blogs.Api.Models.Comment;

namespace Blogs.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ArticlesController(IArticlesHandler articlesHandler, ILogger<ArticlesController> _logger) : ControllerBase
{
    private string Username => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [HttpPost]
    [Authorize]
    public async Task<ArticleEnvelope<ArticleResponse>> CreateAsync(
        RequestEnvelope<ArticleEnvelope<NewArticleDto>> request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating Article...");
                 var article =
            await articlesHandler.CreateArticleAsync(request.Body.Article, Username, cancellationToken);
            var result = ArticlesMapper.MapFromArticleEntity(article);
            return new ArticleEnvelope<ArticleResponse>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex;
        }
    }

    [HttpGet]
    public async Task<ActionResult<ArticlesResponse>> GetAsync(
        [FromQuery] ArticlesQuery query, CancellationToken cancellationToken)
    {
        var response = await articlesHandler.GetArticlesAsync(query, Username, false, cancellationToken);
        var result = ArticlesMapper.MapFromArticles(response);
        return result;
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ArticleEnvelope<ArticleResponse>>> GetBySlugAsync(string slug,
        CancellationToken cancellationToken)
    {
        var article = await articlesHandler.GetArticleBySlugAsync(slug, Username, cancellationToken);
        var result = ArticlesMapper.MapFromArticleEntity(article);
        return new ArticleEnvelope<ArticleResponse>(result);
    }

    [Authorize]
    [HttpPut("{slug}")]
    public async Task<ActionResult<ArticleEnvelope<ArticleResponse>>> UpdateBySlugAsync(string slug,
        RequestEnvelope<ArticleEnvelope<ArticleUpdateDto>> request, CancellationToken cancellationToken)
    {
        var article = await articlesHandler.UpdateArticleAsync(request.Body.Article,
            slug,
            Username,
            cancellationToken);

        var result = ArticlesMapper.MapFromArticleEntity(article);
        return new ArticleEnvelope<ArticleResponse>(result);
    }

    [Authorize]
    [HttpDelete("{slug}")]
    public async Task<ActionResult<ArticleEnvelope<ArticleResponse>>> DeleteBySlugAsync(string slug,
        CancellationToken cancellationToken)
    {
        await articlesHandler.DeleteArticleAsync(slug, Username, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpGet("feed")]
    public async Task<ActionResult<ArticlesResponse>> GetFeedAsync([FromQuery] FeedQuery query,
        CancellationToken cancellationToken)
    {
        var articlesQuery = new ArticlesQuery(Username, query.Limit, query.Offset);
        var response = await articlesHandler.GetArticlesAsync(articlesQuery, Username, false, cancellationToken);
        var result = ArticlesMapper.MapFromArticles(response);
        return result;
    }

    [Authorize]
    [HttpPost("{slug}/comments")]
    public async Task<CommentEnvelope<Comment>> AddCommentAsync(string slug,
        RequestEnvelope<CommentEnvelope<CommentDto>> request, CancellationToken cancellationToken)
    {
        var result = await articlesHandler.AddCommentAsync(slug, Username, request.Body.Comment, cancellationToken);
        var comment = CommentMapper.MapFromCommentEntity(result);
        return new CommentEnvelope<Comment>(comment);
    }

    [HttpGet("{slug}/comments")]
    public async Task<ActionResult<CommentsEnvelope<List<Comment>>>> GetCommentAsync(string slug,
        CancellationToken cancellationToken)
    {
        var result = await articlesHandler.GetCommentsAsync(slug, Username, cancellationToken);
        var comments = result.Select(CommentMapper.MapFromCommentEntity);
        return new CommentsEnvelope<List<Comment>>(comments.ToList());
    }

    [Authorize]
    [HttpDelete("{slug}/comments/{commentId}")]
    public async Task<ActionResult<ArticleEnvelope<ArticleResponse>>> GetCommentAsync(string slug, int commentId,
        CancellationToken cancellationToken)
    {
        await articlesHandler.RemoveCommentAsync(slug, commentId, Username, cancellationToken);
        return Ok();
    }
}
