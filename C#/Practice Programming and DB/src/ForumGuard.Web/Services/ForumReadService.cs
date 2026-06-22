using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Infrastructure.Identity;
using ForumGuard.Infrastructure.Persistence;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumGuard.Web.Services;

/// <summary>
/// Builds read-only forum projections over the EF Core context (SDD-FORUM-001, SDD-FORUM-010).
/// <para>Joins forum entities with <see cref="ApplicationUser"/> for display names and applies the
/// public-visibility invariant; never mutates state and never exposes ORM entities to pages.</para>
/// </summary>
public sealed class ForumReadService : IForumReadService
{
    private readonly ForumGuardDbContext _context;

    /// <summary>
    /// Initializes the read service with the EF Core context.
    /// </summary>
    /// <param name="context">The database context used for read-only projections.</param>
    public ForumReadService(ForumGuardDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ThreadListItem>> ListThreadsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ForumThreads
            .AsNoTracking()
            .OrderByDescending(thread => thread.CreatedAtUtc)
            .Select(thread => new ThreadListItem(
                thread.Id,
                thread.Title,
                _context.Users
                    .Where(user => user.Id == thread.CreatedById)
                    .Select(user => user.DisplayName)
                    .FirstOrDefault() ?? "Unknown",
                thread.CreatedAtUtc,
                _context.Comments.Count(comment => comment.ThreadId == thread.Id
                    && (comment.Status == CommentStatus.Published
                        || comment.Status == CommentStatus.ApprovedByModerator))))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ThreadDetailView?> GetThreadDetailAsync(Guid threadId, CancellationToken cancellationToken = default)
    {
        ForumThread? thread = await _context.ForumThreads
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == threadId, cancellationToken)
            .ConfigureAwait(false);

        if (thread is null)
        {
            return null;
        }

        string creatorName = await GetDisplayNameAsync(thread.CreatedById, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<ThreadCommentView> comments = await LoadVisibleCommentsAsync(threadId, cancellationToken).ConfigureAwait(false);

        return new ThreadDetailView(thread.Id, thread.Title, creatorName, thread.CreatedAtUtc, comments);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<QueueRowView>> GetQueueRowsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .AsNoTracking()
            .Where(comment => comment.Status == CommentStatus.FlaggedForReview)
            .OrderBy(comment => comment.CreatedAtUtc)
            .ThenBy(comment => comment.Id)
            .Select(comment => new QueueRowView(
                comment.Id,
                _context.Users
                    .Where(user => user.Id == comment.AuthorId)
                    .Select(user => user.DisplayName)
                    .FirstOrDefault() ?? "Unknown",
                comment.Body,
                comment.CreatedAtUtc,
                comment.AnalysisLabel,
                comment.AnalysisScore))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<ThreadCommentView>> LoadVisibleCommentsAsync(Guid threadId, CancellationToken cancellationToken)
    {
        return await _context.Comments
            .AsNoTracking()
            .Where(comment => comment.ThreadId == threadId
                && (comment.Status == CommentStatus.Published
                    || comment.Status == CommentStatus.ApprovedByModerator))
            .OrderBy(comment => comment.PublishedAtUtc)
            .ThenBy(comment => comment.CreatedAtUtc)
            .ThenBy(comment => comment.Id)
            .Select(comment => new ThreadCommentView(
                comment.Id,
                _context.Users
                    .Where(user => user.Id == comment.AuthorId)
                    .Select(user => user.DisplayName)
                    .FirstOrDefault() ?? "Unknown",
                comment.Body,
                comment.PublishedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<string> GetDisplayNameAsync(Guid userId, CancellationToken cancellationToken)
    {
        string? displayName = await _context.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return displayName ?? "Unknown";
    }
}
