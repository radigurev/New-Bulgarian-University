namespace MeepleNight.Domain.Interfaces;

/// <summary>Reads identity of the user that owns the current request scope.</summary>
public interface ICurrentUserAccessor
{
    Guid? UserId { get; }
    string? DisplayName { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
}
