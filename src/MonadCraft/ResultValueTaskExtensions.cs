namespace MonadCraft;

/// <summary>
///     Provides extension methods for working with <see cref="ValueTask{Result}" /> objects.
///     These methods use the "ValueAsync" suffix to avoid overload resolution ambiguity with Task-based methods.
/// </summary>
public static class ResultValueTaskExtensions
{
    /// <summary>
    ///     Asynchronously transforms the Result by applying either the 'success' or 'failure' function.
    /// </summary>
    public static async ValueTask<TOutput> MatchValueAsync<TError, TValue, TOutput>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, TOutput> success,
        Func<TError, TOutput> failure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Match(success, failure);
    }

    /// <summary>
    ///     Asynchronously transforms the Result by applying either the 'success' or 'failure' function.
    /// </summary>
    public static async ValueTask<TOutput> MatchValueAsync<TError, TValue, TOutput>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, ValueTask<TOutput>> success,
        Func<TError, ValueTask<TOutput>> failure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MatchValueAsync(success, failure).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously transforms the 'Success' value into a new type, keeping the 'Failure' error type the same.
    /// </summary>
    public static async ValueTask<Result<TError, TNewValue>> MapValueAsync<TError, TValue, TNewValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, TNewValue> mapFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Map(mapFunc);
    }

    /// <summary>
    ///     Asynchronously transforms the 'Success' value into a new type, keeping the 'Failure' error type the same.
    /// </summary>
    public static async ValueTask<Result<TError, TNewValue>> MapValueAsync<TError, TValue, TNewValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, ValueTask<TNewValue>> mapFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MapValueAsync(mapFunc).ConfigureAwait(false);
    }

    /// <summary>
    ///     Validates the success value and converts to Failure if the predicate returns false.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> EnsureValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, bool> predicate,
        TError error)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Ensure(predicate, error);
    }

    /// <summary>
    ///     Validates the success value asynchronously and converts to Failure if the predicate returns false.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> EnsureValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, ValueTask<bool>> predicate,
        TError error)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.EnsureValueAsync(predicate, error).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously transforms the 'Failure' error into a new type, keeping the 'Success' value type the same.
    /// </summary>
    public static async ValueTask<Result<TNewError, TValue>> MapErrorValueAsync<TError, TValue, TNewError>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TError, TNewError> mapFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.MapError(mapFunc);
    }

    /// <summary>
    ///     Asynchronously transforms the 'Failure' error into a new type, keeping the 'Success' value type the same.
    /// </summary>
    public static async ValueTask<Result<TNewError, TValue>> MapErrorValueAsync<TError, TValue, TNewError>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TError, ValueTask<TNewError>> mapFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MapErrorValueAsync(mapFunc).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously chains an operation that itself returns a Result.
    /// </summary>
    public static async ValueTask<Result<TError, TNewValue>> BindValueAsync<TError, TValue, TNewValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, Result<TError, TNewValue>> bindFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Bind(bindFunc);
    }

    /// <summary>
    ///     Asynchronously chains an operation that itself returns a Result.
    /// </summary>
    public static async ValueTask<Result<TError, TNewValue>> BindValueAsync<TError, TValue, TNewValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, ValueTask<Result<TError, TNewValue>>> bindFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindValueAsync(bindFunc).ConfigureAwait(false);
    }

    /// <summary>
    ///     Converts a Failure into a Success using the provided recovery function.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> RecoverValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TError, TValue> recovery)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Recover(recovery);
    }

    /// <summary>
    ///     Converts a Failure into a Success using an asynchronous recovery function.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> RecoverValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TError, ValueTask<TValue>> recovery)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.RecoverValueAsync(recovery).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Result is a 'Success'.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> OnSuccessValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Action<TValue> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.OnSuccess(action);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action if the Result is a 'Success'.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> OnSuccessValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, ValueTask> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.OnSuccessValueAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    ///     Performs a side-effect action if the Result is a 'Failure'.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> OnFailureValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Action<TError> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.OnFailure(action);
    }

    /// <summary>
    ///     Performs a side-effect action if the Result is a 'Failure'.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> OnFailureValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TError, ValueTask> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.OnFailureValueAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> SwitchValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Action<TValue> onSuccess,
        Action<TError> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Switch(onSuccess, onFailure);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> SwitchValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, ValueTask> onSuccess,
        Func<TError, ValueTask> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.SwitchValueAsync(onSuccess, onFailure).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> SwitchValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Action<TValue> onSuccess,
        Func<TError, ValueTask> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.SwitchValueAsync(onSuccess, onFailure).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    public static async ValueTask<Result<TError, TValue>> SwitchValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TValue, ValueTask> onSuccess,
        Action<TError> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.SwitchValueAsync(onSuccess, onFailure).ConfigureAwait(false);
    }

    /// <summary>
    ///     Converts the Result to an Option that is Some when Success and None when Failure.
    /// </summary>
    public static async ValueTask<Optional<TValue>> ToOptionalValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.ToOptional();
    }

    /// <summary>
    ///     Gets the 'Success' value or returns the provided fallback value if it's a 'Failure'.
    /// </summary>
    public static async ValueTask<TValue> GetValueOrDefaultValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        TValue fallback)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.GetValueOrDefault(fallback);
    }

    /// <summary>
    ///     Gets the 'Success' value or computes a fallback value from the error.
    /// </summary>
    public static async ValueTask<TValue> GetValueOrElseValueAsync<TError, TValue>(
        this ValueTask<Result<TError, TValue>> resultTask,
        Func<TError, TValue> fallbackFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.GetValueOrElse(fallbackFunc);
    }
}
