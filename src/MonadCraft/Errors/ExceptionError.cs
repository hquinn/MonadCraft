namespace MonadCraft.Errors;

/// <summary>
/// An implementation of <see cref="IError"/> that represents a caught System.Exception,
/// allowing traditional exceptions to be integrated into the monadic workflow.
/// </summary>
public readonly record struct ExceptionError(Exception Exception) : IError
{
    /// <inheritdoc />
    /// <remarks>
    /// The Code is derived from the simple name of the wrapped exception type (e.g., "IOException").
    /// </remarks>
    public string Code => Exception.GetType().Name;

    /// <inheritdoc />
    /// <remarks>
    /// The Message is the original message from the wrapped exception.
    /// </remarks>
    public string Message => Exception.Message;

    /// <inheritdoc />
    /// <remarks>
    /// An uncaught exception is treated as a critical failure.
    /// </remarks>
    public ErrorSeverity Severity => ErrorSeverity.Critical;

    /// <inheritdoc />
    /// <remarks>
    /// If the wrapped exception has an InnerException, it is recursively wrapped in a new ExceptionError.
    /// </remarks>
    public IError? Cause => Exception.InnerException is { } inner 
        ? new ExceptionError(inner) 
        : null;
}