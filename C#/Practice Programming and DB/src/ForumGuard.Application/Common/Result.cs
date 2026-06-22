namespace ForumGuard.Application.Common;

/// <summary>
/// Represents the outcome of an application-service operation that yields a value, carrying success or a typed failure.
/// <para>Used in place of exceptions for validation, not-found, authorization, and conflict control flow.</para>
/// </summary>
/// <typeparam name="TValue">The type of the value produced on success.</typeparam>
public sealed class Result<TValue>
{
    private Result(ResultStatus status, TValue? value, string? error)
    {
        Status = status;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Gets the classification of the outcome.
    /// </summary>
    public ResultStatus Status { get; }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess => Status == ResultStatus.Success;

    /// <summary>
    /// Gets the produced value when <see cref="IsSuccess"/> is <c>true</c>, otherwise <c>null</c>.
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// Gets the human-readable failure message when the operation failed, otherwise <c>null</c>.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Creates a successful result carrying the produced value.
    /// </summary>
    /// <param name="value">The value to convey.</param>
    /// <returns>A successful result.</returns>
    public static Result<TValue> Success(TValue value) =>
        new(ResultStatus.Success, value, null);

    /// <summary>
    /// Creates a validation-failure result.
    /// </summary>
    /// <param name="error">The validation message.</param>
    /// <returns>A validation-failure result.</returns>
    public static Result<TValue> Validation(string error) =>
        new(ResultStatus.Validation, default, error);

    /// <summary>
    /// Creates a forbidden-failure result.
    /// </summary>
    /// <param name="error">The authorization message.</param>
    /// <returns>A forbidden-failure result.</returns>
    public static Result<TValue> Forbidden(string error) =>
        new(ResultStatus.Forbidden, default, error);

    /// <summary>
    /// Creates a not-found-failure result.
    /// </summary>
    /// <param name="error">The not-found message.</param>
    /// <returns>A not-found-failure result.</returns>
    public static Result<TValue> NotFound(string error) =>
        new(ResultStatus.NotFound, default, error);

    /// <summary>
    /// Creates a conflict-failure result.
    /// </summary>
    /// <param name="error">The conflict message.</param>
    /// <returns>A conflict-failure result.</returns>
    public static Result<TValue> Conflict(string error) =>
        new(ResultStatus.Conflict, default, error);
}
