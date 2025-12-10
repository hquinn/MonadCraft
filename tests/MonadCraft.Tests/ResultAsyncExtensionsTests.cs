using Xunit;

namespace MonadCraft.Tests;

public class ResultAsyncExtensionsTests
{
    [Fact]
    public async Task MatchAsync_uses_success_function()
    {
        var value = await Task.FromResult(Result.Success<string, int>(2)).MatchAsync(v => v + 1, _ => -1);
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchAsync_uses_failure_function()
    {
        var value = await Task.FromResult(Result.Failure<string, int>("err")).MatchAsync(v => v + 1, _ => -1);
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task MatchAsync_async_uses_success_function()
    {
        var value = await Task.FromResult(Result.Success<string, int>(2))
            .MatchAsync(v => Task.FromResult(v + 1), _ => Task.FromResult(-1));
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchAsync_async_uses_failure_function()
    {
        var value = await Task.FromResult(Result.Failure<string, int>("err"))
            .MatchAsync(v => Task.FromResult(v + 1), _ => Task.FromResult(-1));
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task MapAsync_with_sync_mapper_on_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2)).MapAsync(v => v + 1);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task MapAsync_with_sync_mapper_on_failure()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err")).MapAsync(v => v + 1);
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task MapAsync_with_async_mapper_on_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2)).MapAsync(v => Task.FromResult(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task MapAsync_with_async_mapper_on_failure()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err")).MapAsync(v => Task.FromResult(v + 1));
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task EnsureAsync_predicate_true_keeps_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2)).EnsureAsync(v => v > 1, "bad");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task EnsureAsync_predicate_false_converts_to_failure()
    {
        var result = await Task.FromResult(Result.Success<string, int>(0)).EnsureAsync(v => v > 1, "bad");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task EnsureAsync_async_predicate_true_keeps_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2))
            .EnsureAsync(v => Task.FromResult(v > 1), "bad");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task EnsureAsync_async_predicate_false_converts_to_failure()
    {
        var result = await Task.FromResult(Result.Success<string, int>(0))
            .EnsureAsync(v => Task.FromResult(v > 1), "bad");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task EnsureAsync_leaves_failure_unchanged()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err")).EnsureAsync(v => v > 1, "bad");
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task MapErrorAsync_with_sync_mapper_on_failure()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err")).MapErrorAsync(e => e.Length);
        Assert.Equal(3, result.Error);
    }

    [Fact]
    public async Task MapErrorAsync_with_sync_mapper_on_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2)).MapErrorAsync(e => e.Length);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task MapErrorAsync_with_async_mapper_on_failure()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err"))
            .MapErrorAsync(e => Task.FromResult(e.Length));
        Assert.Equal(3, result.Error);
    }

    [Fact]
    public async Task MapErrorAsync_with_async_mapper_on_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2))
            .MapErrorAsync(e => Task.FromResult(e.Length));
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task BindAsync_with_sync_binder_on_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2))
            .BindAsync(v => Result.Success<string, int>(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task BindAsync_with_sync_binder_on_failure()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err"))
            .BindAsync(v => Result.Success<string, int>(v + 1));
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task BindAsync_with_async_binder_on_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2))
            .BindAsync(v => Task.FromResult(Result.Success<string, int>(v + 1)));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task BindAsync_with_async_binder_on_failure()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err"))
            .BindAsync(v => Task.FromResult(Result.Success<string, int>(v + 1)));
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task RecoverAsync_with_sync_recovery_on_failure()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err")).RecoverAsync(_ => 5);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task RecoverAsync_with_sync_recovery_on_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2)).RecoverAsync(_ => 5);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task RecoverAsync_with_async_recovery_on_failure()
    {
        var result = await Task.FromResult(Result.Failure<string, int>("err")).RecoverAsync(_ => Task.FromResult(5));
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task RecoverAsync_with_async_recovery_on_success()
    {
        var result = await Task.FromResult(Result.Success<string, int>(2)).RecoverAsync(_ => Task.FromResult(5));
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task OnSuccessAsync_action_runs_on_success()
    {
        var hit = false;
        await Task.FromResult(Result.Success<string, int>(2)).OnSuccessAsync(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSuccessAsync_action_skips_on_failure()
    {
        var hit = false;
        await Task.FromResult(Result.Failure<string, int>("err")).OnSuccessAsync(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnSuccessAsync_func_runs_on_success()
    {
        var hit = false;
        await Task.FromResult(Result.Success<string, int>(2)).OnSuccessAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSuccessAsync_func_skips_on_failure()
    {
        var hit = false;
        await Task.FromResult(Result.Failure<string, int>("err")).OnSuccessAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public async Task OnFailureAsync_action_runs_on_failure()
    {
        var hit = false;
        await Task.FromResult(Result.Failure<string, int>("err")).OnFailureAsync(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task OnFailureAsync_action_skips_on_success()
    {
        var hit = false;
        await Task.FromResult(Result.Success<string, int>(2)).OnFailureAsync(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnFailureAsync_func_runs_on_failure()
    {
        var hit = false;
        await Task.FromResult(Result.Failure<string, int>("err")).OnFailureAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnFailureAsync_func_skips_on_success()
    {
        var hit = false;
        await Task.FromResult(Result.Success<string, int>(2)).OnFailureAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public async Task SwitchAsync_actions_call_success_branch()
    {
        var hit = false;
        await Task.FromResult(Result.Success<string, int>(2)).SwitchAsync(_ => hit = true, _ => hit = false);
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_actions_call_failure_branch()
    {
        var hit = false;
        await Task.FromResult(Result.Failure<string, int>("err")).SwitchAsync(_ => hit = false, _ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_funcs_call_success_branch()
    {
        var hit = false;
        await Task.FromResult(Result.Success<string, int>(2))
            .SwitchAsync(_ => Task.Run(() => hit = true), _ => Task.Run(() => hit = false));
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_funcs_call_failure_branch()
    {
        var hit = false;
        await Task.FromResult(Result.Failure<string, int>("err"))
            .SwitchAsync(_ => Task.Run(() => hit = false), _ => Task.Run(() => hit = true));
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_action_then_func_calls_success_branch()
    {
        var hit = false;
        await Task.FromResult(Result.Success<string, int>(2))
            .SwitchAsync(_ => hit = true, _ => Task.Run(() => hit = false));
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_action_then_func_calls_failure_branch()
    {
        var hit = false;
        await Task.FromResult(Result.Failure<string, int>("err"))
            .SwitchAsync(_ => hit = false, _ => Task.Run(() => hit = true));
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_func_then_action_calls_success_branch()
    {
        var hit = false;
        await Task.FromResult(Result.Success<string, int>(2))
            .SwitchAsync(_ => Task.Run(() => hit = true), _ => hit = false);
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_func_then_action_calls_failure_branch()
    {
        var hit = false;
        await Task.FromResult(Result.Failure<string, int>("err"))
            .SwitchAsync(_ => Task.Run(() => hit = false), _ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task ToOptionalAsync_returns_some_for_success()
    {
        var option = await Task.FromResult(Result.Success<string, int>(2)).ToOptionalAsync();
        Assert.True(option.IsSome);
    }

    [Fact]
    public async Task ToOptionalAsync_returns_none_for_failure()
    {
        var option = await Task.FromResult(Result.Failure<string, int>("err")).ToOptionalAsync();
        Assert.True(option.IsNone);
    }
}