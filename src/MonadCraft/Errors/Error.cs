namespace MonadCraft.Errors;

/// <summary>
/// A generic, strongly typed implementation of <see cref="IError"/>.
/// </summary>
/// <typeparam name="TContext">The type of the contextual data for the error.</typeparam>
public readonly record struct Error<TContext> : IError
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public IError? Cause { get; init; }
    public required ErrorSeverity Severity { get; init; }
    
    /// <summary>
    /// The strongly typed, contextual information about the error.
    /// </summary>
    public required TContext Context { get; init; }
}