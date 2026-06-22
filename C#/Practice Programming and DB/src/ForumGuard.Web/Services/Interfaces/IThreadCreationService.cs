using ForumGuard.Application.Common;

namespace ForumGuard.Web.Services.Interfaces;

/// <summary>
/// Defines the create-thread use case for authenticated active users (SDD-FORUM-001 context).
/// <para>Persists a new <c>ForumThread</c> through the repository and Unit of Work boundary.</para>
/// </summary>
public interface IThreadCreationService
{
    /// <summary>
    /// Creates a forum thread authored by the supplied active user.
    /// </summary>
    /// <param name="title">The thread title; required, at most 200 characters.</param>
    /// <param name="createdById">The identifier of the authoring active user.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A success result with the new thread id, or a typed validation failure.</returns>
    Task<Result<Guid>> CreateThreadAsync(string? title, Guid createdById, CancellationToken cancellationToken = default);
}
