using ForumGuard.Application.Abstractions;
using ForumGuard.Application.Common;
using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Web.Services.Interfaces;

namespace ForumGuard.Web.Services;

/// <summary>
/// Implements the create-thread use case (SDD-FORUM-001 context).
/// <para>Validates the title, constructs a <see cref="ForumThread"/>, and commits it through the
/// repository and <see cref="IUnitOfWork"/> boundary.</para>
/// </summary>
public sealed class ThreadCreationService : IThreadCreationService
{
    private readonly IForumThreadRepository _threadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes the service with its persistence and clock dependencies.
    /// </summary>
    /// <param name="threadRepository">The repository that tracks the new thread.</param>
    /// <param name="unitOfWork">The commit boundary that persists the thread.</param>
    /// <param name="clock">The UTC time source for the creation timestamp.</param>
    public ThreadCreationService(IForumThreadRepository threadRepository, IUnitOfWork unitOfWork, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(threadRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(clock);
        _threadRepository = threadRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> CreateThreadAsync(string? title, Guid createdById, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string trimmedTitle = title?.Trim() ?? string.Empty;
        if (trimmedTitle.Length == 0)
        {
            return Result<Guid>.Validation("A thread title is required.");
        }

        if (trimmedTitle.Length > ForumThread.MaxTitleLength)
        {
            return Result<Guid>.Validation($"The title must not exceed {ForumThread.MaxTitleLength} characters.");
        }

        ForumThread thread = new(trimmedTitle, createdById, _clock.UtcNow);
        _threadRepository.Add(thread);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result<Guid>.Success(thread.Id);
    }
}
