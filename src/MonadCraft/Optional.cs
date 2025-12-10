using System.Diagnostics.CodeAnalysis;

namespace MonadCraft;

/// <summary>
///     A static helper class for creating instances of <see cref="Optional{TValue}" />.
/// </summary>
public static class Optional
{
    /// <summary>
    ///     Creates an Option that holds a value (the 'Some' state).
    /// </summary>
    public static Optional<TValue> Some<TValue>(TValue value)
    {
        return new Optional<TValue>(value);
    }

    /// <summary>
    ///     Creates an Option that holds no value (the 'None' state).
    /// </summary>
    public static Optional<TValue> None<TValue>()
    {
        return Optional<TValue>.None;
    }

    /// <summary>
    ///     Executes a factory and wraps the result in an Option, returning None if the factory throws.
    /// </summary>
    public static Optional<TValue> Try<TValue>(Func<TValue> factory)
    {
        try
        {
            return Some(factory());
        }
        catch
        {
            return None<TValue>();
        }
    }
}

/// <summary>
///     Represents a value that may or may not exist.
///     This type is used to explicitly handle the absence of a value, avoiding null reference exceptions.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public readonly record struct Optional<TValue>
{
    /// <summary>
    ///     Represents the 'None' state, containing no value.
    /// </summary>
    public static readonly Optional<TValue> None = new();

    private readonly TValue? _value;

    /// <summary>
    ///     Creates a new Option in the 'Some' state, containing the provided value.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the provided value is null.</exception>
    public Optional(TValue value)
    {
        _value = value ??
                 throw new ArgumentNullException(nameof(value), "Cannot create a Some option with a null value.");
        IsSome = true;
    }

    /// <summary>
    ///     Creates a new Option in the 'None' state, representing the absence of a value.
    /// </summary>
    public Optional()
    {
        IsSome = false;
        _value = default;
    }

    /// <summary>
    ///     Returns true if the Option contains a value ('Some').
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(true, nameof(_value))]
    public bool IsSome { get; }

    /// <summary>
    ///     Returns true if the Option does not contain a value ('None').
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(false, nameof(_value))]
    public bool IsNone => !IsSome;

    /// <summary>
    ///     Gets the contained value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the Option is in a 'None' state.</exception>
    public TValue Value
        => IsSome
            ? _value
            : throw new InvalidOperationException("Cannot access Value when Option is None.");

    /// <summary>
    ///     LINQ-compatible projection.
    /// </summary>
    public Optional<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector)
    {
        return Map(selector);
    }

    /// <summary>
    ///     LINQ-compatible bind with result selector.
    /// </summary>
    public Optional<TResult> SelectMany<TIntermediate, TResult>(
        Func<TValue, Optional<TIntermediate>> binder,
        Func<TValue, TIntermediate, TResult> projector)
    {
        if (!IsSome) return Optional<TResult>.None;

        var intermediate = binder(Value);
        return intermediate.IsSome
            ? new Optional<TResult>(projector(Value, intermediate.Value))
            : Optional<TResult>.None;
    }

    /// <summary>
    ///     Transforms the Option by applying either the 'some' or 'none' function.
    /// </summary>
    public TOutput Match<TOutput>(Func<TValue, TOutput> some, Func<TOutput> none)
    {
        return IsSome
            ? some(Value)
            : none();
    }

    /// <summary>
    ///     Asynchronously transforms the Option by applying either the 'some' or 'none' function.
    /// </summary>
    public async Task<TOutput> MatchAsync<TOutput>(Func<TValue, Task<TOutput>> some, Func<Task<TOutput>> none)
    {
        return IsSome
            ? await some(Value).ConfigureAwait(false)
            : await none().ConfigureAwait(false);
    }

    /// <summary>
    ///     Transforms the contained value into a new type, returning a new Option.
    ///     If the Option is None, it propagates the None state.
    /// </summary>
    public Optional<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> mapFunc)
    {
        return IsSome
            ? new Optional<TNewValue>(mapFunc(Value))
            : Optional<TNewValue>.None;
    }

    /// <summary>
    ///     Asynchronously transforms the contained value into a new type, returning a new Option.
    /// </summary>
    public async Task<Optional<TNewValue>> MapAsync<TNewValue>(Func<TValue, Task<TNewValue>> mapFunc)
    {
        return IsSome
            ? new Optional<TNewValue>(await mapFunc(Value).ConfigureAwait(false))
            : Optional<TNewValue>.None;
    }

    /// <summary>
    ///     Chains an operation that itself returns an Option. Also known as 'Bind' or 'FlatMap'.
    /// </summary>
    public Optional<TNewValue> Bind<TNewValue>(Func<TValue, Optional<TNewValue>> bindFunc)
    {
        return IsSome
            ? bindFunc(Value)
            : Optional<TNewValue>.None;
    }

    /// <summary>
    ///     Asynchronously chains an operation that itself returns a Task of an Option.
    /// </summary>
    public async Task<Optional<TNewValue>> BindAsync<TNewValue>(Func<TValue, Task<Optional<TNewValue>>> bindFunc)
    {
        return IsSome
            ? await bindFunc(Value).ConfigureAwait(false)
            : Optional<TNewValue>.None;
    }

    /// <summary>
    ///     Performs a side-effect action if the Option is Some.
    /// </summary>
    /// <returns>The original, unchanged Option.</returns>
    public Optional<TValue> OnSome(Action<TValue> action)
    {
        if (IsSome) action(Value);

        return this;
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Option is Some.
    /// </summary>
    public async Task<Optional<TValue>> OnSomeAsync(Func<TValue, Task> action)
    {
        if (IsSome) await action(Value).ConfigureAwait(false);

        return this;
    }

    /// <summary>
    ///     Performs a side-effect action if the Option is None.
    /// </summary>
    /// <returns>The original, unchanged Option.</returns>
    public Optional<TValue> OnNone(Action action)
    {
        if (IsNone) action();

        return this;
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Option is None.
    /// </summary>
    public async Task<Optional<TValue>> OnNoneAsync(Func<Task> action)
    {
        if (IsNone) await action().ConfigureAwait(false);

        return this;
    }

    /// <summary>
    ///     Returns this Option if the predicate is true for the contained value; otherwise, returns None.
    /// </summary>
    public Optional<TValue> Where(Func<TValue, bool> predicate)
    {
        return IsSome && predicate(Value)
            ? this
            : None;
    }

    /// <summary>
    ///     Returns this Option if it contains a value; otherwise returns the provided fallback Option.
    /// </summary>
    public Optional<TValue> OrElse(Optional<TValue> fallback)
    {
        return IsSome
            ? this
            : fallback;
    }

    /// <summary>
    ///     Returns this Option if it contains a value; otherwise evaluates and returns the fallback Option.
    /// </summary>
    public Optional<TValue> OrElse(Func<Optional<TValue>> fallbackFactory)
    {
        return IsSome
            ? this
            : fallbackFactory();
    }

    /// <summary>
    ///     Gets the contained value or returns the provided fallback value if None.
    /// </summary>
    public TValue GetValueOrDefault(TValue fallback)
    {
        return IsSome
            ? Value
            : fallback;
    }

    /// <summary>
    ///     Gets the contained value or computes a fallback value if None.
    /// </summary>
    public TValue GetValueOrElse(Func<TValue> fallbackFunc)
    {
        return IsSome
            ? Value
            : fallbackFunc();
    }

    /// <summary>
    ///     Exposes the contained value as a single-item sequence or an empty sequence when None.
    /// </summary>
    public IEnumerable<TValue> ToEnumerable()
    {
        if (IsSome) yield return Value;
    }

    /// <summary>
    ///     Converts the Option to a Result. If Some, returns a Success. If None, returns a Failure with the provided error.
    /// </summary>
    public Result<TError, TValue> ToResult<TError>(TError errorIfNone)
    {
        return IsSome
            ? Result<TError, TValue>.Success(Value)
            : Result<TError, TValue>.Failure(errorIfNone);
    }

    /// <summary>
    ///     Converts the Option to a Result. If Some, returns a Success.
    ///     If None, returns a Failure with an error produced by the factory function.
    /// </summary>
    public Result<TError, TValue> ToResult<TError>(Func<TError> errorFactory)
    {
        return IsSome
            ? Result<TError, TValue>.Success(Value)
            : Result<TError, TValue>.Failure(errorFactory());
    }

    // Implicit operators for convenience
    public static implicit operator Optional<TValue>(TValue? value)
    {
        return value is not null
            ? new Optional<TValue>(value)
            : None;
    }
}