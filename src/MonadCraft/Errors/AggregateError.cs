namespace MonadCraft.Errors;

/// <summary>
/// An implementation of <see cref="IError"/> that contains a collection of other errors.
/// </summary>
public readonly record struct AggregateError : IError
{
    /// <summary>
    /// The collection of errors that have been aggregated.
    /// </summary>
    public required IReadOnlyList<IError> InnerErrors { get; init; }

    /// <inheritdoc />
    public string Code => "AggregateError";

    /// <inheritdoc />
    public string Message => $"Multiple errors occurred ({InnerErrors.Count}). See InnerErrors for details.";

    /// <inheritdoc />
    /// <remarks>
    /// The severity of an AggregateError is always the highest severity of any of its inner errors.
    /// </remarks>
    public ErrorSeverity Severity => InnerErrors.Any() 
        ? InnerErrors.Max(e => e.Severity) 
        : ErrorSeverity.Error;

    /// <inheritdoc />
    /// <remarks>
    /// An AggregateError represents a collection of parallel failures, not a single causal chain.
    /// Therefore, its direct Cause is always null. The cause of individual inner errors can be inspected.
    /// </remarks>
    public IError? Cause => null;
}