using Xunit;

namespace MonadCraft.Tests;

public class OptionAsyncExtensionsTests
{
    [Fact]
    public async Task MatchAsync_uses_some_function()
    {
        var value = await Task.FromResult(Optional.Some(2)).MatchAsync(v => v + 1, () => -1);
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchAsync_uses_none_function()
    {
        var value = await Task.FromResult(Optional.None<int>()).MatchAsync(v => v + 1, () => -1);
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task MatchAsync_async_some_function()
    {
        var value = await Task.FromResult(Optional.Some(2))
            .MatchAsync(v => Task.FromResult(v + 1), () => Task.FromResult(-1));
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchAsync_async_none_function()
    {
        var value = await Task.FromResult(Optional.None<int>())
            .MatchAsync(v => Task.FromResult(v + 1), () => Task.FromResult(-1));
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task MapAsync_with_sync_mapper_on_some()
    {
        var result = await Task.FromResult(Optional.Some(2)).MapAsync(v => v + 1);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task MapAsync_with_sync_mapper_on_none()
    {
        var result = await Task.FromResult(Optional.None<int>()).MapAsync(v => v + 1);
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task MapAsync_with_async_mapper_on_some()
    {
        var result = await Task.FromResult(Optional.Some(2)).MapAsync(v => Task.FromResult(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task MapAsync_with_async_mapper_on_none()
    {
        var result = await Task.FromResult(Optional.None<int>()).MapAsync(v => Task.FromResult(v + 1));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task BindAsync_with_sync_binder_on_some()
    {
        var result = await Task.FromResult(Optional.Some(2)).BindAsync(v => Optional.Some(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task BindAsync_with_sync_binder_on_none()
    {
        var result = await Task.FromResult(Optional.None<int>()).BindAsync(v => Optional.Some(v + 1));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task BindAsync_with_async_binder_on_some()
    {
        var result = await Task.FromResult(Optional.Some(2)).BindAsync(v => Task.FromResult(Optional.Some(v + 1)));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task BindAsync_with_async_binder_on_none()
    {
        var result = await Task.FromResult(Optional.None<int>()).BindAsync(v => Task.FromResult(Optional.Some(v + 1)));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task OrElseAsync_returns_self_when_some()
    {
        var result = await Task.FromResult(Optional.Some(1)).OrElseAsync(Optional.Some(2));
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async Task OrElseAsync_returns_fallback_when_none()
    {
        var result = await Task.FromResult(Optional.None<int>()).OrElseAsync(Optional.Some(2));
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task OrElseAsync_factory_runs_when_none()
    {
        var result = await Task.FromResult(Optional.None<int>()).OrElseAsync(() => Optional.Some(3));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task OrElseAsync_async_factory_runs_when_none()
    {
        var result = await Task.FromResult(Optional.None<int>()).OrElseAsync(() => Task.FromResult(Optional.Some(4)));
        Assert.Equal(4, result.Value);
    }

    [Fact]
    public async Task OrElseAsync_async_factory_skipped_when_some()
    {
        var result = await Task.FromResult(Optional.Some(1))
            .OrElseAsync((Func<Task<Optional<int>>>)(() => throw new InvalidOperationException("should not run")));
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async Task OnSomeAsync_action_runs_when_some()
    {
        var hit = false;
        await Task.FromResult(Optional.Some(1)).OnSomeAsync(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSomeAsync_action_skips_when_none()
    {
        var hit = false;
        await Task.FromResult(Optional.None<int>()).OnSomeAsync(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnSomeAsync_func_runs_when_some()
    {
        var hit = false;
        await Task.FromResult(Optional.Some(1)).OnSomeAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSomeAsync_func_skips_when_none()
    {
        var hit = false;
        await Task.FromResult(Optional.None<int>()).OnSomeAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public async Task OnNoneAsync_action_runs_when_none()
    {
        var hit = false;
        await Task.FromResult(Optional.None<int>()).OnNoneAsync(() => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task OnNoneAsync_action_skips_when_some()
    {
        var hit = false;
        await Task.FromResult(Optional.Some(1)).OnNoneAsync(() => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnNoneAsync_func_runs_when_none()
    {
        var hit = false;
        await Task.FromResult(Optional.None<int>()).OnNoneAsync(async () =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnNoneAsync_func_skips_when_some()
    {
        var hit = false;
        await Task.FromResult(Optional.Some(1)).OnNoneAsync(async () =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }
}