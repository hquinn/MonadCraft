namespace MonadCraft;

/// <summary>
/// Provides extension methods for working with <see cref="Task{Optional}"/> objects.
/// </summary>
public static class OptionAsyncExtensions
{
    /// <summary>
    /// Asynchronously transforms the Option by applying either the 'some' or 'none' function.
    /// </summary>
    public static async Task<TOutput> MatchAsync<TValue, TOutput>(
        this Task<Optional<TValue>> optionalTask,
        Func<TValue, TOutput> some,
        Func<TOutput> none)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.Match(some, none);
    }
    
    /// <summary>
    /// Asynchronously transforms the Option by applying either the 'some' or 'none' function.
    /// </summary>
    public static async Task<TOutput> MatchAsync<TValue, TOutput>(
        this Task<Optional<TValue>> optionalTask,
        Func<TValue, Task<TOutput>> some,
        Func<Task<TOutput>> none)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.MatchAsync(some, none);
    }

    /// <summary>
    /// Asynchronously transforms the contained value into a new type, returning a new Option.
    /// </summary>
    public static async Task<Optional<TNewValue>> MapAsync<TValue, TNewValue>(
        this Task<Optional<TValue>> optionalTask,
        Func<TValue, TNewValue> mapFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.Map(mapFunc);
    }

    /// <summary>
    /// Asynchronously transforms the contained value into a new type, returning a new Option.
    /// </summary>
    public static async Task<Optional<TNewValue>> MapAsync<TValue, TNewValue>(
        this Task<Optional<TValue>> optionalTask,
        Func<TValue, Task<TNewValue>> mapFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.MapAsync(mapFunc);
    }

    /// <summary>
    /// Asynchronously chains an operation that itself returns a Task of an Option.
    /// </summary>
    public static async Task<Optional<TNewValue>> BindAsync<TValue, TNewValue>(
        this Task<Optional<TValue>> optionalTask,
        Func<TValue, Optional<TNewValue>> bindFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.Bind(bindFunc);
    }

    /// <summary>
    /// Asynchronously chains an operation that itself returns a Task of an Option.
    /// </summary>
    public static async Task<Optional<TNewValue>> BindAsync<TValue, TNewValue>(
        this Task<Optional<TValue>> optionalTask,
        Func<TValue, Task<Optional<TNewValue>>> bindFunc)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.BindAsync(bindFunc);
    }

    /// <summary>
    /// Asynchronously performs a side-effect action if the Option is Some.
    /// </summary>
    public static async Task<Optional<TValue>> OnSomeAsync<TValue>(
        this Task<Optional<TValue>> optionalTask,
        Action<TValue> action)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.OnSome(action);
    }

    /// <summary>
    /// Asynchronously performs a side-effect action if the Option is Some.
    /// </summary>
    public static async Task<Optional<TValue>> OnSomeAsync<TValue>(
        this Task<Optional<TValue>> optionalTask,
        Func<TValue, Task> action)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.OnSomeAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously performs a side-effect action if the Option is None.
    /// </summary>
    public static async Task<Optional<TValue>> OnNoneAsync<TValue>(
        this Task<Optional<TValue>> optionalTask,
        Action action)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return optional.OnNone(action);
    }

    /// <summary>
    /// Asynchronously performs a side-effect action if the Option is None.
    /// </summary>
    public static async Task<Optional<TValue>> OnNoneAsync<TValue>(
        this Task<Optional<TValue>> optionalTask,
        Func<Task> action)
    {
        var optional = await optionalTask.ConfigureAwait(false);
        return await optional.OnNoneAsync(action).ConfigureAwait(false);
    }
}