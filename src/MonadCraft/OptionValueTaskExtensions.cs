namespace MonadCraft;

/// <summary>
///     Provides extension methods for working with <see cref="ValueTask{Optional}" /> objects.
///     These methods use the "ValueAsync" suffix to avoid overload resolution ambiguity with Task-based methods.
/// </summary>
public static class OptionValueTaskExtensions
{
    /// <summary>
    ///     Asynchronously transforms the Option by applying either the 'some' or 'none' function.
    /// </summary>
    public static async ValueTask<TOutput> MatchValueAsync<TValue, TOutput>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, TOutput> some,
        Func<TOutput> none)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.Match(some, none);
    }

    /// <summary>
    ///     Asynchronously transforms the Option by applying either the 'some' or 'none' function.
    /// </summary>
    public static async ValueTask<TOutput> MatchValueAsync<TValue, TOutput>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, ValueTask<TOutput>> some,
        Func<ValueTask<TOutput>> none)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.MatchValueAsync(some, none).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously transforms the contained value into a new type, returning a new Option.
    /// </summary>
    public static async ValueTask<Optional<TNewValue>> MapValueAsync<TValue, TNewValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, TNewValue> mapFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.Map(mapFunc);
    }

    /// <summary>
    ///     Asynchronously transforms the contained value into a new type, returning a new Option.
    /// </summary>
    public static async ValueTask<Optional<TNewValue>> MapValueAsync<TValue, TNewValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, ValueTask<TNewValue>> mapFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.MapValueAsync(mapFunc).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously chains an operation that itself returns an Option.
    /// </summary>
    public static async ValueTask<Optional<TNewValue>> BindValueAsync<TValue, TNewValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, Optional<TNewValue>> bindFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.Bind(bindFunc);
    }

    /// <summary>
    ///     Asynchronously chains an operation that itself returns a ValueTask of an Option.
    /// </summary>
    public static async ValueTask<Optional<TNewValue>> BindValueAsync<TValue, TNewValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, ValueTask<Optional<TNewValue>>> bindFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.BindValueAsync(bindFunc).ConfigureAwait(false);
    }

    /// <summary>
    ///     Returns the original Option if it yields Some; otherwise, returns the provided fallback Option.
    /// </summary>
    public static async ValueTask<Optional<TValue>> OrElseValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Optional<TValue> fallback)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.OrElse(fallback);
    }

    /// <summary>
    ///     Returns the original Option if it yields Some; otherwise, evaluates and returns the fallback Option.
    /// </summary>
    public static async ValueTask<Optional<TValue>> OrElseValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<Optional<TValue>> fallbackFactory)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.OrElse(fallbackFactory);
    }

    /// <summary>
    ///     Returns the original Option if it yields Some; otherwise, awaits and returns the fallback Option.
    /// </summary>
    public static async ValueTask<Optional<TValue>> OrElseValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<ValueTask<Optional<TValue>>> fallbackFactory)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        if (optional.IsSome) return optional;

        var fallback = await fallbackFactory().ConfigureAwait(false);
        return fallback;
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Option is Some.
    /// </summary>
    public static async ValueTask<Optional<TValue>> OnSomeValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Action<TValue> action)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.OnSome(action);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Option is Some.
    /// </summary>
    public static async ValueTask<Optional<TValue>> OnSomeValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, ValueTask> action)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.OnSomeValueAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Option is None.
    /// </summary>
    public static async ValueTask<Optional<TValue>> OnNoneValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Action action)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.OnNone(action);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Option is None.
    /// </summary>
    public static async ValueTask<Optional<TValue>> OnNoneValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<ValueTask> action)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.OnNoneValueAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously filters the Option based on a predicate.
    /// </summary>
    public static async ValueTask<Optional<TValue>> WhereValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, bool> predicate)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.Where(predicate);
    }

    /// <summary>
    ///     Asynchronously filters the Option based on an async predicate.
    /// </summary>
    public static async ValueTask<Optional<TValue>> WhereValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue, ValueTask<bool>> predicate)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.WhereValueAsync(predicate).ConfigureAwait(false);
    }

    /// <summary>
    ///     Gets the contained value or returns the provided fallback value if None.
    /// </summary>
    public static async ValueTask<TValue> GetValueOrDefaultValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        TValue fallback)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.GetValueOrDefault(fallback);
    }

    /// <summary>
    ///     Gets the contained value or computes a fallback value if None.
    /// </summary>
    public static async ValueTask<TValue> GetValueOrElseValueAsync<TValue>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TValue> fallbackFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.GetValueOrElse(fallbackFunc);
    }

    /// <summary>
    ///     Converts the Option to a Result. If Some, returns a Success. If None, returns a Failure.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> ToResultValueAsync<TValue, TError>(
        this ValueTask<Optional<TValue>> optionalTask,
        TError errorIfNone)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.ToResult(errorIfNone);
    }

    /// <summary>
    ///     Converts the Option to a Result. If Some, returns a Success. If None, returns a Failure.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> ToResultValueAsync<TValue, TError>(
        this ValueTask<Optional<TValue>> optionalTask,
        Func<TError> errorFactory)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.ToResult(errorFactory);
    }
}
