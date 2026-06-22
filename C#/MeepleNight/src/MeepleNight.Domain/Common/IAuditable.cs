namespace MeepleNight.Domain.Common;

/// <summary>Marker for entities that carry audit and soft-delete columns.</summary>
public interface IAuditable
{
    DateTime CreatedAtUtc { get; set; }
    Guid? CreatedByUserId { get; set; }
    DateTime? UpdatedAtUtc { get; set; }
    Guid? UpdatedByUserId { get; set; }
}

/// <summary>Marker for entities supporting soft delete via an IsActive flag.</summary>
public interface ISoftDeletable
{
    bool IsActive { get; set; }
}
