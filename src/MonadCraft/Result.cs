using System.Diagnostics.CodeAnalysis;

namespace MonadCraft;

/// <summary>
///     A static helper class for creating instances of <see cref="Result{TError, TValue}" />.
/// </summary>
public static class Result
{
    /// <summary>
    ///     Creates a Result in the 'Success' state.
    /// </summary>
    public static Result<TError, TValue> Success<TError, TValue>(TValue value)
    {
        return new Result<TError, TValue>(value);
    }

    /// <summary>
    ///     Creates a Result in the 'Failure' state.
    /// </summary>
    public static Result<TError, TValue> Failure<TError, TValue>(TError error)
    {
        return new Result<TError, TValue>(error);
    }

    /// <summary>
    ///     Executes a factory and wraps the result in a Success, or captures exceptions as Failure.
    /// </summary>
    public static Result<TError, TValue> Try<TError, TValue>(Func<TValue> factory, Func<Exception, TError> errorFactory)
    {
        try
        {
            return Success<TError, TValue>(factory());
        }
        catch (Exception ex)
        {
            return Failure<TError, TValue>(errorFactory(ex));
        }
    }

    /// <summary>
    ///     Executes an asynchronous factory and wraps the result in a Success, or captures exceptions as Failure.
    /// </summary>
    public static async Task<Result<TError, TValue>> TryAsync<TError, TValue>(
        Func<Task<TValue>> factory,
        Func<Exception, TError> errorFactory)
    {
        try
        {
            var value = await factory().ConfigureAwait(false);
            return Success<TError, TValue>(value);
        }
        catch (Exception ex)
        {
            return Failure<TError, TValue>(errorFactory(ex));
        }
    }
}

/// <summary>
///     Represents the result of an operation that can either succeed with a value or fail with an error.
///     This is a discriminated union of a <typeparamref name="TValue" /> (Success)
///     or a <typeparamref name="TError" /> (Failure).
/// </summary>
/// <typeparam name="TError">The type of the error value.</typeparam>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public readonly record struct Result<TError, TValue>
{
    private readonly TError? _error;
    private readonly TValue? _value;

    /// <summary>
    ///     Creates a new 'Success' Result.
    /// </summary>
    public Result(TValue value)
    {
        IsSuccess = true;
        _value = value;
        _error = default;
    }

    /// <summary>
    ///     Creates a new 'Failure' Result.
    /// </summary>
    public Result(TError error)
    {
        IsSuccess = false;
        _error = error;
        _value = default;
    }

    /// <summary>
    ///     Returns true if the Result is a 'Success'.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(false, nameof(_error))]
    public bool IsSuccess { get; }

    /// <summary>
    ///     Returns true if the Result is a 'Failure'.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(false, nameof(_value))]
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(true, nameof(_error))]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    ///     Gets the 'Success' value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the Result is in the 'Failure' state.</exception>
    public TValue Value
        => IsSuccess
            ? _value
            : throw new InvalidOperationException("Cannot access Value when in a Failure state.");

    /// <summary>
    ///     Gets the 'Failure' error.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the result is in a 'Success' state.</exception>
    public TError Error
        => IsFailure
            ? _error
            : throw new InvalidOperationException("Cannot access Error when in a Success state.");

    /// <summary>
    ///     Deconstructs the Result into its possible value and error.
    /// </summary>
    public void Deconstruct(out TValue? value, out TError? error)
    {
        value = _value;
        error = _error;
    }

    /// <summary>
    ///     Transforms the Result by applying either the 'success' or 'failure' function.
    /// </summary>
    public TOutput Match<TOutput>(Func<TValue, TOutput> success, Func<TError, TOutput> failure)
    {
        return IsSuccess
            ? success(Value)
            : failure(Error);
    }

    /// <summary>
    ///     LINQ-compatible projection.
    /// </summary>
    public Result<TError, TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector)
    {
        return Map(selector);
    }

    /// <summary>
    ///     LINQ-compatible bind with result selector.
    /// </summary>
    public Result<TError, TResult> SelectMany<TIntermediate, TResult>(
        Func<TValue, Result<TError, TIntermediate>> binder,
        Func<TValue, TIntermediate, TResult> projector)
    {
        if (IsFailure) return new Result<TError, TResult>(Error);

        var intermediate = binder(Value);
        return intermediate.IsSuccess
            ? new Result<TError, TResult>(projector(Value, intermediate.Value))
            : new Result<TError, TResult>(intermediate.Error);
    }

    /// <summary>
    ///     Asynchronously transforms the Result by applying either the 'success' or 'failure' function.
    /// </summary>
    public async Task<TOutput> MatchAsync<TOutput>(
        Func<TValue, Task<TOutput>> success,
        Func<TError, Task<TOutput>> failure)
    {
        return IsSuccess
            ? await success(Value).ConfigureAwait(false)
            : await failure(Error).ConfigureAwait(false);
    }

    /// <summary>
    ///     Transforms the 'Success' value into a new type, keeping the 'Failure' error type the same.
    ///     If the Result is a 'Failure', it propagates the error.
    /// </summary>
    public Result<TError, TNewValue> Map<TNewValue>(Func<TValue, TNewValue> mapFunc)
    {
        return IsSuccess
            ? new Result<TError, TNewValue>(mapFunc(Value))
            : new Result<TError, TNewValue>(Error);
    }

    /// <summary>
    ///     Asynchronously transforms the 'Success' value into a new type, keeping the 'Failure' error type the same.
    ///     If the Result is a 'Failure', it propagates the error.
    /// </summary>
    public async Task<Result<TError, TNewValue>> MapAsync<TNewValue>(Func<TValue, Task<TNewValue>> mapFunc)
    {
        return IsSuccess
            ? new Result<TError, TNewValue>(await mapFunc(Value).ConfigureAwait(false))
            : new Result<TError, TNewValue>(Error);
    }

    /// <summary>
    ///     Validates the success value and converts to Failure if the predicate returns false.
    /// </summary>
    public Result<TError, TValue> Ensure(Func<TValue, bool> predicate, TError error)
    {
        return IsSuccess && !predicate(Value)
            ? new Result<TError, TValue>(error)
            : this;
    }

    /// <summary>
    ///     Validates the success value asynchronously and converts to Failure if the predicate returns false.
    /// </summary>
    public async Task<Result<TError, TValue>> EnsureAsync(Func<TValue, Task<bool>> predicate, TError error)
    {
        if (IsFailure) return this;

        var isValid = await predicate(Value).ConfigureAwait(false);
        return isValid
            ? this
            : new Result<TError, TValue>(error);
    }

    /// <summary>
    ///     Transforms the 'Failure' error into a new type, keeping the 'Success' value type the same.
    ///     If the Result is a 'Success', it propagates the value.
    /// </summary>
    public Result<TNewError, TValue> MapError<TNewError>(Func<TError, TNewError> mapFunc)
    {
        return IsSuccess
            ? new Result<TNewError, TValue>(Value)
            : new Result<TNewError, TValue>(mapFunc(Error));
    }

    /// <summary>
    ///     Asynchronously transforms the 'Failure' error into a new type, keeping the 'Success' value type the same.
    ///     If the Result is a 'Success', it propagates the value.
    /// </summary>
    public async Task<Result<TNewError, TValue>> MapErrorAsync<TNewError>(Func<TError, Task<TNewError>> mapFunc)
    {
        return IsSuccess
            ? new Result<TNewError, TValue>(Value)
            : new Result<TNewError, TValue>(await mapFunc(Error).ConfigureAwait(false));
    }

    /// <summary>
    ///     Chains an operation that itself returns a Result.
    ///     If the current Result is a 'Failure', it propagates the error.
    /// </summary>
    public Result<TError, TNewValue> Bind<TNewValue>(Func<TValue, Result<TError, TNewValue>> bindFunc)
    {
        return IsSuccess
            ? bindFunc(Value)
            : new Result<TError, TNewValue>(Error);
    }

    /// <summary>
    ///     Asynchronously chains an operation that itself returns a Result.
    ///     If the current Result is a Failure, it propagates the error.
    /// </summary>
    public async Task<Result<TError, TNewValue>> BindAsync<TNewValue>(
        Func<TValue, Task<Result<TError, TNewValue>>> bindFunc)
    {
        return IsSuccess
            ? await bindFunc(Value).ConfigureAwait(false)
            : new Result<TError, TNewValue>(Error);
    }

    /// <summary>
    ///     Converts a Failure into a Success using the provided recovery function.
    /// </summary>
    public Result<TError, TValue> Recover(Func<TError, TValue> recovery)
    {
        return IsSuccess
            ? this
            : new Result<TError, TValue>(recovery(Error));
    }

    /// <summary>
    ///     Converts a Failure into a Success using an asynchronous recovery function.
    /// </summary>
    public async Task<Result<TError, TValue>> RecoverAsync(Func<TError, Task<TValue>> recovery)
    {
        return IsSuccess
            ? this
            : new Result<TError, TValue>(await recovery(Error).ConfigureAwait(false));
    }

    /// <summary>
    ///     Performs a side-effect action if the Result is a 'Success'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public Result<TError, TValue> OnSuccess(Action<TValue> action)
    {
        if (IsSuccess) action(Value);

        return this;
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Result is a 'Success'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public async Task<Result<TError, TValue>> OnSuccessAsync(Func<TValue, Task> action)
    {
        if (IsSuccess) await action(Value).ConfigureAwait(false);

        return this;
    }

    /// <summary>
    ///     Performs a side-effect action if the Result is a 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public Result<TError, TValue> OnFailure(Action<TError> action)
    {
        if (IsFailure) action(Error);

        return this;
    }

    /// <summary>
    ///     Performs a side-effect action if the Result is a 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public async Task<Result<TError, TValue>> OnFailureAsync(Func<TError, Task> action)
    {
        if (IsFailure) await action(Error).ConfigureAwait(false);

        return this;
    }

    /// <summary>
    ///     Performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public Result<TError, TValue> Switch(Action<TValue> onSuccess, Action<TError> onFailure)
    {
        if (IsSuccess)
            onSuccess(Value);
        else
            onFailure(Error);

        return this;
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public async Task<Result<TError, TValue>> SwitchAsync(Func<TValue, Task> onSuccess, Func<TError, Task> onFailure)
    {
        if (IsSuccess)
            await onSuccess(Value).ConfigureAwait(false);
        else
            await onFailure(Error).ConfigureAwait(false);

        return this;
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public async Task<Result<TError, TValue>> SwitchAsync(Action<TValue> onSuccess, Func<TError, Task> onFailure)
    {
        if (IsSuccess)
            onSuccess(Value);
        else
            await onFailure(Error).ConfigureAwait(false);

        return this;
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public async Task<Result<TError, TValue>> SwitchAsync(Func<TValue, Task> onSuccess, Action<TError> onFailure)
    {
        if (IsSuccess)
            await onSuccess(Value).ConfigureAwait(false);
        else
            onFailure(Error);

        return this;
    }

    /// <summary>
    ///     Gets the 'Success' value or returns the provided fallback value if it's a 'Failure'.
    /// </summary>
    public TValue GetValueOrDefault(TValue fallback)
    {
        return IsSuccess
            ? Value
            : fallback;
    }

    /// <summary>
    ///     Gets the 'Success' value or computes a fallback value from the error.
    /// </summary>
    public TValue GetValueOrElse(Func<TError, TValue> fallbackFunc)
    {
        return IsSuccess
            ? Value
            : fallbackFunc(Error);
    }

    /// <summary>
    ///     Converts the Result to an Option that is Some when Success and None when Failure.
    /// </summary>
    public Optional<TValue> ToOptional()
    {
        return IsSuccess
            ? new Optional<TValue>(Value)
            : Optional<TValue>.None;
    }

    /// <summary>
    ///     Creates a new 'Success' Result.
    /// </summary>
    public static Result<TError, TValue> Success(TValue value)
    {
        return new Result<TError, TValue>(value);
    }

    /// <summary>
    ///     Creates a new 'Failure' Result.
    /// </summary>
    public static Result<TError, TValue> Failure(TError error)
    {
        return new Result<TError, TValue>(error);
    }

    // Implicit operators for seamless creation
    public static implicit operator Result<TError, TValue>(TValue value)
    {
        return new Result<TError, TValue>(value);
    }

    public static implicit operator Result<TError, TValue>(TError error)
    {
        return new Result<TError, TValue>(error);
    }
}