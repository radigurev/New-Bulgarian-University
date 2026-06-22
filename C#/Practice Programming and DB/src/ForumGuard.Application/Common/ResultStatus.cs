namespace ForumGuard.Application.Common;

/// <summary>
/// Classifies the outcome of an application-service operation for caller mapping (e.g. HTTP status).
/// <para>See <see cref="Result{T}"/>.</para>
/// </summary>
public enum ResultStatus
{
    /// <summary>
    /// The operation succeeded.
    /// </summary>
    Success,

    /// <summary>
    /// The input failed validation.
    /// </summary>
    Validation,

    /// <summary>
    /// The caller is not permitted to perform the operation.
    /// </summary>
    Forbidden,

    /// <summary>
    /// A referenced resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// The operation conflicts with the current state of the resource.
    /// </summary>
    Conflict
}
