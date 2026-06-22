namespace MeepleNight.Services.Dtos;

/// <summary>Result of a service command. Carries success, optional error code/message, and an id when relevant.</summary>
public sealed class Result
{
    public bool IsSuccess { get; private init; }
    public string? Error { get; private init; }
    public ResultErrorKind ErrorKind { get; private init; } = ResultErrorKind.None;

    public static Result Ok() => new() { IsSuccess = true };
    public static Result Fail(string error, ResultErrorKind kind = ResultErrorKind.Validation)
        => new() { IsSuccess = false, Error = error, ErrorKind = kind };

    public static Result NotFound(string message = "Not found") => Fail(message, ResultErrorKind.NotFound);
    public static Result Forbidden(string message = "Forbidden") => Fail(message, ResultErrorKind.Forbidden);
    public static Result Conflict(string message) => Fail(message, ResultErrorKind.Conflict);
}

public sealed class Result<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? Error { get; private init; }
    public ResultErrorKind ErrorKind { get; private init; } = ResultErrorKind.None;

    public static Result<T> Ok(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Fail(string error, ResultErrorKind kind = ResultErrorKind.Validation)
        => new() { IsSuccess = false, Error = error, ErrorKind = kind };

    public static Result<T> NotFound(string message = "Not found") => Fail(message, ResultErrorKind.NotFound);
    public static Result<T> Forbidden(string message = "Forbidden") => Fail(message, ResultErrorKind.Forbidden);
    public static Result<T> Conflict(string message) => Fail(message, ResultErrorKind.Conflict);
}

public enum ResultErrorKind
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Forbidden = 3,
    Conflict = 4
}
