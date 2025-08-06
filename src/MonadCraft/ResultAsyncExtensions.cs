namespace MonadCraft;

/// <summary>
/// Provides a set of extension methods for working with <see cref="Task{Result}"/> objects.
/// </summary>
public static class ResultAsyncExtensions
{
    /// <summary>
    /// Asynchronously transforms the Result by applying either the 'success' or 'failure' function.
    /// </summary>
    public static async Task<TOutput> MatchAsync<TError, TValue, TOutput>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, TOutput> success,
        Func<TError, TOutput> failure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Match(success, failure);
    }
    
    /// <summary>
    /// Asynchronously transforms the Result by applying either the 'success' or 'failure' function.
    /// </summary>
    public static async Task<TOutput> MatchAsync<TError, TValue, TOutput>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, Task<TOutput>> success,
        Func<TError, Task<TOutput>> failure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MatchAsync(success, failure).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously transforms the 'Success' value into a new type, keeping the 'Failure' error type the same.
    /// If the Result is a 'Failure', it propagates the error.
    /// </summary>
    public static async Task<Result<TError, TNewValue>> MapAsync<TError, TValue, TNewValue>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, TNewValue> mapFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Map(mapFunc);
    }

    /// <summary>
    /// Asynchronously transforms the 'Success' value into a new type, keeping the 'Failure' error type the same.
    /// If the Result is a 'Failure', it propagates the error.
    /// </summary>
    public static async Task<Result<TError, TNewValue>> MapAsync<TError, TValue, TNewValue>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, Task<TNewValue>> mapFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MapAsync(mapFunc).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously transforms the 'Failure' error into a new type, keeping the 'Success' value type the same.
    /// If the Result is a 'Success', it propagates the value.
    /// </summary>
    public static async Task<Result<TNewError, TValue>> MapErrorAsync<TError, TValue, TNewError>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TError, TNewError> mapFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.MapError(mapFunc);
    }

    /// <summary>
    /// Asynchronously transforms the 'Failure' error into a new type, keeping the 'Success' value type the same.
    /// If the Result is a 'Success', it propagates the value.
    /// </summary>
    public static async Task<Result<TNewError, TValue>> MapErrorAsync<TError, TValue, TNewError>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TError, Task<TNewError>> mapFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MapErrorAsync(mapFunc).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously chains an operation that itself returns a Result.
    /// If the current Result is a Failure, it propagates the error.
    /// </summary>
    public static async Task<Result<TError, TNewValue>> BindAsync<TError, TValue, TNewValue>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, Result<TError, TNewValue>> bindFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Bind(bindFunc);
    }

    /// <summary>
    /// Asynchronously chains an operation that itself returns a Result.
    /// If the current Result is a Failure, it propagates the error.
    /// </summary>
    public static async Task<Result<TError, TNewValue>> BindAsync<TError, TValue, TNewValue>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, Task<Result<TError, TNewValue>>> bindFunc)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindAsync(bindFunc).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Asynchronously performs a side-effect action if the Result is a 'Success'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public static async Task<Result<TError, TValue>> OnSuccessAsync<TError, TValue>(
        this Task<Result<TError, TValue>> resultTask,
        Action<TValue> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.OnSuccess(action);
    }
    
    /// <summary>
    /// Asynchronously performs a side-effect action if the Result is a 'Success'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public static async Task<Result<TError, TValue>> OnSuccessAsync<TError, TValue>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.OnSuccessAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a side-effect action if the Result is a 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public static async Task<Result<TError, TValue>> OnFailureAsync<TError, TValue>(
        this Task<Result<TError, TValue>> resultTask,
        Action<TError> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.OnFailure(action);
    }

    /// <summary>
    /// Performs a side-effect action if the Result is a 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public static async Task<Result<TError, TValue>> OnFailureAsync<TError, TValue>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TError, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.OnFailureAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public static async Task<Result<TError, TValue>> SwitchAsync<TError, TValue>(
        this Task<Result<TError, TValue>> resultTask,
        Action<TValue> onSuccess,
        Action<TError> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Switch(onSuccess, onFailure);
    }

    /// <summary>
    /// Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public static async Task<Result<TError, TValue>> SwitchAsync<TError, TValue>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, Task> onSuccess,
        Func<TError, Task> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.SwitchAsync(onSuccess, onFailure).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public static async Task<Result<TError, TValue>> SwitchAsync<TError, TValue>(
        this Task<Result<TError, TValue>> resultTask,
        Action<TValue> onSuccess,
        Func<TError, Task> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.SwitchAsync(onSuccess, onFailure).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously performs a side-effect action based on if the Result is either a 'Success' or 'Failure'.
    /// </summary>
    /// <returns>The original, unchanged Result.</returns>
    public static async Task<Result<TError, TValue>> SwitchAsync<TError, TValue>(
        this Task<Result<TError, TValue>> resultTask,
        Func<TValue, Task> onSuccess,
        Action<TError> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.SwitchAsync(onSuccess, onFailure).ConfigureAwait(false);
    }
}