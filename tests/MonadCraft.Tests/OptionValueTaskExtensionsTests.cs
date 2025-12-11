using Xunit;

namespace MonadCraft.Tests;

public class OptionValueTaskExtensionsTests
{
    // Helper to create ValueTask<Optional<T>>
    private static ValueTask<Optional<T>> SomeValueTask<T>(T value) => ValueTask.FromResult(Optional.Some(value));
    private static ValueTask<Optional<T>> NoneValueTask<T>() => ValueTask.FromResult(Optional.None<T>());

    #region MatchValueAsync

    [Fact]
    public async Task MatchValueAsync_uses_some_function()
    {
        var value = await SomeValueTask(2).MatchValueAsync(v => v + 1, () => -1);
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchValueAsync_uses_none_function()
    {
        var value = await NoneValueTask<int>().MatchValueAsync(v => v + 1, () => -1);
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task MatchValueAsync_async_some_function()
    {
        var value = await SomeValueTask(2)
            .MatchValueAsync(v => ValueTask.FromResult(v + 1), () => ValueTask.FromResult(-1));
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchValueAsync_async_none_function()
    {
        var value = await NoneValueTask<int>()
            .MatchValueAsync(v => ValueTask.FromResult(v + 1), () => ValueTask.FromResult(-1));
        Assert.Equal(-1, value);
    }

    #endregion

    #region MapValueAsync

    [Fact]
    public async Task MapValueAsync_with_sync_mapper_on_some()
    {
        var result = await SomeValueTask(2).MapValueAsync(v => v + 1);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task MapValueAsync_with_sync_mapper_on_none()
    {
        var result = await NoneValueTask<int>().MapValueAsync(v => v + 1);
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task MapValueAsync_with_async_mapper_on_some()
    {
        var result = await SomeValueTask(2).MapValueAsync(v => ValueTask.FromResult(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task MapValueAsync_with_async_mapper_on_none()
    {
        var result = await NoneValueTask<int>().MapValueAsync(v => ValueTask.FromResult(v + 1));
        Assert.True(result.IsNone);
    }

    #endregion

    #region BindValueAsync

    [Fact]
    public async Task BindValueAsync_with_sync_binder_on_some()
    {
        var result = await SomeValueTask(2).BindValueAsync(v => Optional.Some(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task BindValueAsync_with_sync_binder_on_none()
    {
        var result = await NoneValueTask<int>().BindValueAsync(v => Optional.Some(v + 1));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task BindValueAsync_with_async_binder_on_some()
    {
        var result = await SomeValueTask(2).BindValueAsync(v => ValueTask.FromResult(Optional.Some(v + 1)));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task BindValueAsync_with_async_binder_on_none()
    {
        var result = await NoneValueTask<int>().BindValueAsync(v => ValueTask.FromResult(Optional.Some(v + 1)));
        Assert.True(result.IsNone);
    }

    #endregion

    #region OrElseValueAsync

    [Fact]
    public async Task OrElseValueAsync_returns_self_when_some()
    {
        var result = await SomeValueTask(1).OrElseValueAsync(Optional.Some(2));
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async Task OrElseValueAsync_returns_fallback_when_none()
    {
        var result = await NoneValueTask<int>().OrElseValueAsync(Optional.Some(2));
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task OrElseValueAsync_factory_runs_when_none()
    {
        var result = await NoneValueTask<int>().OrElseValueAsync(() => Optional.Some(3));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task OrElseValueAsync_async_factory_runs_when_none()
    {
        var result = await NoneValueTask<int>().OrElseValueAsync(() => ValueTask.FromResult(Optional.Some(4)));
        Assert.Equal(4, result.Value);
    }

    [Fact]
    public async Task OrElseValueAsync_async_factory_skipped_when_some()
    {
        var result = await SomeValueTask(1)
            .OrElseValueAsync((Func<ValueTask<Optional<int>>>)(() => throw new InvalidOperationException("should not run")));
        Assert.Equal(1, result.Value);
    }

    #endregion

    #region OnSomeValueAsync

    [Fact]
    public async Task OnSomeValueAsync_action_runs_when_some()
    {
        var hit = false;
        await SomeValueTask(1).OnSomeValueAsync(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSomeValueAsync_action_skips_when_none()
    {
        var hit = false;
        await NoneValueTask<int>().OnSomeValueAsync(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnSomeValueAsync_func_runs_when_some()
    {
        var hit = false;
        await SomeValueTask(1).OnSomeValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSomeValueAsync_func_skips_when_none()
    {
        var hit = false;
        await NoneValueTask<int>().OnSomeValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    #endregion

    #region OnNoneValueAsync

    [Fact]
    public async Task OnNoneValueAsync_action_runs_when_none()
    {
        var hit = false;
        await NoneValueTask<int>().OnNoneValueAsync(() => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task OnNoneValueAsync_action_skips_when_some()
    {
        var hit = false;
        await SomeValueTask(1).OnNoneValueAsync(() => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnNoneValueAsync_func_runs_when_none()
    {
        var hit = false;
        await NoneValueTask<int>().OnNoneValueAsync(async () =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnNoneValueAsync_func_skips_when_some()
    {
        var hit = false;
        await SomeValueTask(1).OnNoneValueAsync(async () =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    #endregion

    #region WhereValueAsync

    [Fact]
    public async Task WhereValueAsync_keeps_matching_value()
    {
        var result = await SomeValueTask(5).WhereValueAsync(v => v > 3);
        Assert.True(result.IsSome);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task WhereValueAsync_filters_non_matching_value()
    {
        var result = await SomeValueTask(2).WhereValueAsync(v => v > 3);
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task WhereValueAsync_async_predicate_keeps_matching_value()
    {
        var result = await SomeValueTask(5).WhereValueAsync(v => ValueTask.FromResult(v > 3));
        Assert.True(result.IsSome);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task WhereValueAsync_async_predicate_filters_non_matching_value()
    {
        var result = await SomeValueTask(2).WhereValueAsync(v => ValueTask.FromResult(v > 3));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task WhereValueAsync_none_stays_none()
    {
        var result = await NoneValueTask<int>().WhereValueAsync(v => v > 3);
        Assert.True(result.IsNone);
    }

    #endregion

    #region GetValueOrDefaultValueAsync / GetValueOrElseValueAsync

    [Fact]
    public async Task GetValueOrDefaultValueAsync_returns_value_when_some()
    {
        var value = await SomeValueTask(5).GetValueOrDefaultValueAsync(10);
        Assert.Equal(5, value);
    }

    [Fact]
    public async Task GetValueOrDefaultValueAsync_returns_fallback_when_none()
    {
        var value = await NoneValueTask<int>().GetValueOrDefaultValueAsync(10);
        Assert.Equal(10, value);
    }

    [Fact]
    public async Task GetValueOrElseValueAsync_returns_value_when_some()
    {
        var value = await SomeValueTask(5).GetValueOrElseValueAsync(() => 10);
        Assert.Equal(5, value);
    }

    [Fact]
    public async Task GetValueOrElseValueAsync_returns_fallback_when_none()
    {
        var value = await NoneValueTask<int>().GetValueOrElseValueAsync(() => 10);
        Assert.Equal(10, value);
    }

    #endregion

    #region ToResultValueAsync

    [Fact]
    public async Task ToResultValueAsync_success_when_some()
    {
        var result = await SomeValueTask(5).ToResultValueAsync("error");
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task ToResultValueAsync_failure_when_none()
    {
        var result = await NoneValueTask<int>().ToResultValueAsync("error");
        Assert.True(result.IsFailure);
        Assert.Equal("error", result.Error);
    }

    [Fact]
    public async Task ToResultValueAsync_factory_success_when_some()
    {
        var result = await SomeValueTask(5).ToResultValueAsync(() => "error");
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task ToResultValueAsync_factory_failure_when_none()
    {
        var result = await NoneValueTask<int>().ToResultValueAsync(() => "computed error");
        Assert.True(result.IsFailure);
        Assert.Equal("computed error", result.Error);
    }

    #endregion

    #region Instance Method Tests (on Optional<T>)

    [Fact]
    public async Task Instance_MatchValueAsync_uses_some_function()
    {
        var optional = Optional.Some(2);
        var value = await optional.MatchValueAsync(
            v => ValueTask.FromResult(v + 1),
            () => ValueTask.FromResult(-1));
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task Instance_MatchValueAsync_uses_none_function()
    {
        var optional = Optional.None<int>();
        var value = await optional.MatchValueAsync(
            v => ValueTask.FromResult(v + 1),
            () => ValueTask.FromResult(-1));
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task Instance_MapValueAsync_on_some()
    {
        var optional = Optional.Some(2);
        var result = await optional.MapValueAsync(v => ValueTask.FromResult(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task Instance_MapValueAsync_on_none()
    {
        var optional = Optional.None<int>();
        var result = await optional.MapValueAsync(v => ValueTask.FromResult(v + 1));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task Instance_BindValueAsync_on_some()
    {
        var optional = Optional.Some(2);
        var result = await optional.BindValueAsync(v => ValueTask.FromResult(Optional.Some(v + 1)));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task Instance_BindValueAsync_on_none()
    {
        var optional = Optional.None<int>();
        var result = await optional.BindValueAsync(v => ValueTask.FromResult(Optional.Some(v + 1)));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task Instance_OnSomeValueAsync_runs_when_some()
    {
        var hit = false;
        var optional = Optional.Some(1);
        await optional.OnSomeValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task Instance_OnSomeValueAsync_skips_when_none()
    {
        var hit = false;
        var optional = Optional.None<int>();
        await optional.OnSomeValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public async Task Instance_OnNoneValueAsync_runs_when_none()
    {
        var hit = false;
        var optional = Optional.None<int>();
        await optional.OnNoneValueAsync(async () =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task Instance_OnNoneValueAsync_skips_when_some()
    {
        var hit = false;
        var optional = Optional.Some(1);
        await optional.OnNoneValueAsync(async () =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public async Task Instance_WhereValueAsync_keeps_matching_value()
    {
        var optional = Optional.Some(5);
        var result = await optional.WhereValueAsync(v => ValueTask.FromResult(v > 3));
        Assert.True(result.IsSome);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task Instance_WhereValueAsync_filters_non_matching_value()
    {
        var optional = Optional.Some(2);
        var result = await optional.WhereValueAsync(v => ValueTask.FromResult(v > 3));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task Instance_WhereValueAsync_none_stays_none()
    {
        var optional = Optional.None<int>();
        var result = await optional.WhereValueAsync(v => ValueTask.FromResult(v > 3));
        Assert.True(result.IsNone);
    }

    #endregion
}
