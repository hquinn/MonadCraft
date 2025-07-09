namespace MonadCraft.Errors;

public enum ErrorSeverity
{
    /// <summary>
    /// An informational message that does not indicate a problem.
    /// </summary>
    Info,
    /// <summary>
    /// Indicates a potential issue that does not prevent the operation from continuing.
    /// </summary>
    Warning,
    /// <summary
    /// >A recoverable error that prevents the current operation from succeeding.
    /// </summary>
    Error,
    /// <summary>
    /// A critical, likely unrecoverable error.
    /// </summary>
    Critical
}

/// <summary>
/// Represents a generic error with a code and a message.
/// </summary>
public interface IError
{
    /// <summary>
    /// A unique string code for programmatic error handling.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// The human-readable message describing the error.
    /// </summary>
    string Message { get; }
    
    /// <summary>
    /// The underlying error that caused this error, if any.
    /// </summary>
    IError? Cause { get; }
    
    /// <summary>
    /// The severity level of the error.
    /// </summary>
    ErrorSeverity Severity { get; }
}