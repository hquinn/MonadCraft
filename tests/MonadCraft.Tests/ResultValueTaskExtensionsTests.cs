using Xunit;

namespace MonadCraft.Tests;

public class ResultValueTaskExtensionsTests
{
    // Helper to create ValueTask<Result<TError, TValue>>
    private static ValueTask<Result<TError, TValue>> SuccessValueTask<TError, TValue>(TValue value)
        => ValueTask.FromResult(Result.Success<TError, TValue>(value));

    private static ValueTask<Result<TError, TValue>> FailureValueTask<TError, TValue>(TError error)
        => ValueTask.FromResult(Result.Failure<TError, TValue>(error));

    #region MatchValueAsync

    [Fact]
    public async Task MatchValueAsync_uses_success_function()
    {
        var value = await SuccessValueTask<string, int>(2).MatchValueAsync(v => v + 1, _ => -1);
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchValueAsync_uses_failure_function()
    {
        var value = await FailureValueTask<string, int>("err").MatchValueAsync(v => v + 1, _ => -1);
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task MatchValueAsync_async_uses_success_function()
    {
        var value = await SuccessValueTask<string, int>(2)
            .MatchValueAsync(v => ValueTask.FromResult(v + 1), _ => ValueTask.FromResult(-1));
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchValueAsync_async_uses_failure_function()
    {
        var value = await FailureValueTask<string, int>("err")
            .MatchValueAsync(v => ValueTask.FromResult(v + 1), _ => ValueTask.FromResult(-1));
        Assert.Equal(-1, value);
    }

    #endregion

    #region MapValueAsync

    [Fact]
    public async Task MapValueAsync_with_sync_mapper_on_success()
    {
        var result = await SuccessValueTask<string, int>(2).MapValueAsync(v => v + 1);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task MapValueAsync_with_sync_mapper_on_failure()
    {
        var result = await FailureValueTask<string, int>("err").MapValueAsync(v => v + 1);
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task MapValueAsync_with_async_mapper_on_success()
    {
        var result = await SuccessValueTask<string, int>(2).MapValueAsync(v => ValueTask.FromResult(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task MapValueAsync_with_async_mapper_on_failure()
    {
        var result = await FailureValueTask<string, int>("err").MapValueAsync(v => ValueTask.FromResult(v + 1));
        Assert.Equal("err", result.Error);
    }

    #endregion

    #region EnsureValueAsync

    [Fact]
    public async Task EnsureValueAsync_predicate_true_keeps_success()
    {
        var result = await SuccessValueTask<string, int>(2).EnsureValueAsync(v => v > 1, "bad");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task EnsureValueAsync_predicate_false_converts_to_failure()
    {
        var result = await SuccessValueTask<string, int>(0).EnsureValueAsync(v => v > 1, "bad");
        Assert.True(result.IsFailure);
        Assert.Equal("bad", result.Error);
    }

    [Fact]
    public async Task EnsureValueAsync_async_predicate_true_keeps_success()
    {
        var result = await SuccessValueTask<string, int>(2)
            .EnsureValueAsync(v => ValueTask.FromResult(v > 1), "bad");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task EnsureValueAsync_async_predicate_false_converts_to_failure()
    {
        var result = await SuccessValueTask<string, int>(0)
            .EnsureValueAsync(v => ValueTask.FromResult(v > 1), "bad");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task EnsureValueAsync_leaves_failure_unchanged()
    {
        var result = await FailureValueTask<string, int>("err").EnsureValueAsync(v => v > 1, "bad");
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task EnsureValueAsync_async_predicate_leaves_failure_unchanged()
    {
        var result = await FailureValueTask<string, int>("err")
            .EnsureValueAsync(v => ValueTask.FromResult(v > 1), "bad");
        Assert.Equal("err", result.Error);
    }

    #endregion

    #region MapErrorValueAsync

    [Fact]
    public async Task MapErrorValueAsync_with_sync_mapper_on_failure()
    {
        var result = await FailureValueTask<string, int>("err").MapErrorValueAsync(e => e.Length);
        Assert.Equal(3, result.Error);
    }

    [Fact]
    public async Task MapErrorValueAsync_with_sync_mapper_on_success()
    {
        var result = await SuccessValueTask<string, int>(5).MapErrorValueAsync(e => e.Length);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task MapErrorValueAsync_with_async_mapper_on_failure()
    {
        var result = await FailureValueTask<string, int>("err").MapErrorValueAsync(e => ValueTask.FromResult(e.Length));
        Assert.Equal(3, result.Error);
    }

    [Fact]
    public async Task MapErrorValueAsync_with_async_mapper_on_success()
    {
        var result = await SuccessValueTask<string, int>(5).MapErrorValueAsync(e => ValueTask.FromResult(e.Length));
        Assert.Equal(5, result.Value);
    }

    #endregion

    #region BindValueAsync

    [Fact]
    public async Task BindValueAsync_with_sync_binder_on_success()
    {
        var result = await SuccessValueTask<string, int>(2).BindValueAsync(v => Result.Success<string, int>(v + 1));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task BindValueAsync_with_sync_binder_on_failure()
    {
        var result = await FailureValueTask<string, int>("err").BindValueAsync(v => Result.Success<string, int>(v + 1));
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task BindValueAsync_with_async_binder_on_success()
    {
        var result = await SuccessValueTask<string, int>(2)
            .BindValueAsync(v => ValueTask.FromResult(Result.Success<string, int>(v + 1)));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public async Task BindValueAsync_with_async_binder_on_failure()
    {
        var result = await FailureValueTask<string, int>("err")
            .BindValueAsync(v => ValueTask.FromResult(Result.Success<string, int>(v + 1)));
        Assert.Equal("err", result.Error);
    }

    #endregion

    #region RecoverValueAsync

    [Fact]
    public async Task RecoverValueAsync_with_sync_recovery_on_failure()
    {
        var result = await FailureValueTask<string, int>("err").RecoverValueAsync(_ => 42);
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public async Task RecoverValueAsync_with_sync_recovery_on_success()
    {
        var result = await SuccessValueTask<string, int>(5).RecoverValueAsync(_ => 42);
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task RecoverValueAsync_with_async_recovery_on_failure()
    {
        var result = await FailureValueTask<string, int>("err").RecoverValueAsync(_ => ValueTask.FromResult(42));
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public async Task RecoverValueAsync_with_async_recovery_on_success()
    {
        var result = await SuccessValueTask<string, int>(5).RecoverValueAsync(_ => ValueTask.FromResult(42));
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
    }

    #endregion

    #region OnSuccessValueAsync

    [Fact]
    public async Task OnSuccessValueAsync_action_runs_when_success()
    {
        var hit = false;
        await SuccessValueTask<string, int>(1).OnSuccessValueAsync(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSuccessValueAsync_action_skips_when_failure()
    {
        var hit = false;
        await FailureValueTask<string, int>("err").OnSuccessValueAsync(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnSuccessValueAsync_func_runs_when_success()
    {
        var hit = false;
        await SuccessValueTask<string, int>(1).OnSuccessValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSuccessValueAsync_func_skips_when_failure()
    {
        var hit = false;
        await FailureValueTask<string, int>("err").OnSuccessValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    #endregion

    #region OnFailureValueAsync

    [Fact]
    public async Task OnFailureValueAsync_action_runs_when_failure()
    {
        var hit = false;
        await FailureValueTask<string, int>("err").OnFailureValueAsync(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task OnFailureValueAsync_action_skips_when_success()
    {
        var hit = false;
        await SuccessValueTask<string, int>(1).OnFailureValueAsync(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnFailureValueAsync_func_runs_when_failure()
    {
        var hit = false;
        await FailureValueTask<string, int>("err").OnFailureValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnFailureValueAsync_func_skips_when_success()
    {
        var hit = false;
        await SuccessValueTask<string, int>(1).OnFailureValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    #endregion

    #region SwitchValueAsync

    [Fact]
    public async Task SwitchValueAsync_sync_runs_success_action_when_success()
    {
        var successHit = false;
        var failureHit = false;
        await SuccessValueTask<string, int>(1).SwitchValueAsync(_ => successHit = true, _ => failureHit = true);
        Assert.True(successHit);
        Assert.False(failureHit);
    }

    [Fact]
    public async Task SwitchValueAsync_sync_runs_failure_action_when_failure()
    {
        var successHit = false;
        var failureHit = false;
        await FailureValueTask<string, int>("err").SwitchValueAsync(_ => successHit = true, _ => failureHit = true);
        Assert.False(successHit);
        Assert.True(failureHit);
    }

    [Fact]
    public async Task SwitchValueAsync_async_runs_success_action_when_success()
    {
        var successHit = false;
        var failureHit = false;
        await SuccessValueTask<string, int>(1).SwitchValueAsync(
            async _ =>
            {
                await Task.Delay(1);
                successHit = true;
            },
            async _ =>
            {
                await Task.Delay(1);
                failureHit = true;
            });
        Assert.True(successHit);
        Assert.False(failureHit);
    }

    [Fact]
    public async Task SwitchValueAsync_async_runs_failure_action_when_failure()
    {
        var successHit = false;
        var failureHit = false;
        await FailureValueTask<string, int>("err").SwitchValueAsync(
            async _ =>
            {
                await Task.Delay(1);
                successHit = true;
            },
            async _ =>
            {
                await Task.Delay(1);
                failureHit = true;
            });
        Assert.False(successHit);
        Assert.True(failureHit);
    }

    [Fact]
    public async Task SwitchValueAsync_sync_success_async_failure_runs_correct_action()
    {
        var successHit = false;
        var failureHit = false;
        await FailureValueTask<string, int>("err").SwitchValueAsync(
            _ => successHit = true,
            async _ =>
            {
                await Task.Delay(1);
                failureHit = true;
            });
        Assert.False(successHit);
        Assert.True(failureHit);
    }

    [Fact]
    public async Task SwitchValueAsync_sync_success_async_failure_runs_success_action_when_success()
    {
        var successHit = false;
        var failureHit = false;
        await SuccessValueTask<string, int>(1).SwitchValueAsync(
            _ => successHit = true,
            async _ =>
            {
                await Task.Delay(1);
                failureHit = true;
            });
        Assert.True(successHit);
        Assert.False(failureHit);
    }

    [Fact]
    public async Task SwitchValueAsync_async_success_sync_failure_runs_correct_action()
    {
        var successHit = false;
        var failureHit = false;
        await SuccessValueTask<string, int>(1).SwitchValueAsync(
            async _ =>
            {
                await Task.Delay(1);
                successHit = true;
            },
            _ => failureHit = true);
        Assert.True(successHit);
        Assert.False(failureHit);
    }

    [Fact]
    public async Task SwitchValueAsync_async_success_sync_failure_runs_failure_action_when_failure()
    {
        var successHit = false;
        var failureHit = false;
        await FailureValueTask<string, int>("err").SwitchValueAsync(
            async _ =>
            {
                await Task.Delay(1);
                successHit = true;
            },
            _ => failureHit = true);
        Assert.False(successHit);
        Assert.True(failureHit);
    }

    #endregion

    #region ToOptionalValueAsync

    [Fact]
    public async Task ToOptionalValueAsync_some_when_success()
    {
        var optional = await SuccessValueTask<string, int>(5).ToOptionalValueAsync();
        Assert.True(optional.IsSome);
        Assert.Equal(5, optional.Value);
    }

    [Fact]
    public async Task ToOptionalValueAsync_none_when_failure()
    {
        var optional = await FailureValueTask<string, int>("err").ToOptionalValueAsync();
        Assert.True(optional.IsNone);
    }

    #endregion

    #region GetValueOrDefaultValueAsync / GetValueOrElseValueAsync

    [Fact]
    public async Task GetValueOrDefaultValueAsync_returns_value_when_success()
    {
        var value = await SuccessValueTask<string, int>(5).GetValueOrDefaultValueAsync(10);
        Assert.Equal(5, value);
    }

    [Fact]
    public async Task GetValueOrDefaultValueAsync_returns_fallback_when_failure()
    {
        var value = await FailureValueTask<string, int>("err").GetValueOrDefaultValueAsync(10);
        Assert.Equal(10, value);
    }

    [Fact]
    public async Task GetValueOrElseValueAsync_returns_value_when_success()
    {
        var value = await SuccessValueTask<string, int>(5).GetValueOrElseValueAsync(_ => 10);
        Assert.Equal(5, value);
    }

    [Fact]
    public async Task GetValueOrElseValueAsync_returns_fallback_when_failure()
    {
        var value = await FailureValueTask<string, int>("err").GetValueOrElseValueAsync(_ => 10);
        Assert.Equal(10, value);
    }

    #endregion

    #region Instance Method Tests (on Result<TError, TValue>)

    [Fact]
    public async Task Instance_MatchValueAsync_uses_success_function()
    {
        var result = Result.Success<string, int>(2);
        var value = await result.MatchValueAsync(
            v => ValueTask.FromResult(v + 1),
            _ => ValueTask.FromResult(-1));
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task Instance_MatchValueAsync_uses_failure_function()
    {
        var result = Result.Failure<string, int>("err");
        var value = await result.MatchValueAsync(
            v => ValueTask.FromResult(v + 1),
            _ => ValueTask.FromResult(-1));
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task Instance_MapValueAsync_on_success()
    {
        var result = Result.Success<string, int>(2);
        var mapped = await result.MapValueAsync(v => ValueTask.FromResult(v + 1));
        Assert.Equal(3, mapped.Value);
    }

    [Fact]
    public async Task Instance_MapValueAsync_on_failure()
    {
        var result = Result.Failure<string, int>("err");
        var mapped = await result.MapValueAsync(v => ValueTask.FromResult(v + 1));
        Assert.Equal("err", mapped.Error);
    }

    [Fact]
    public async Task Instance_EnsureValueAsync_predicate_true_keeps_success()
    {
        var result = Result.Success<string, int>(2);
        var validated = await result.EnsureValueAsync(v => ValueTask.FromResult(v > 1), "bad");
        Assert.True(validated.IsSuccess);
    }

    [Fact]
    public async Task Instance_EnsureValueAsync_predicate_false_converts_to_failure()
    {
        var result = Result.Success<string, int>(0);
        var validated = await result.EnsureValueAsync(v => ValueTask.FromResult(v > 1), "bad");
        Assert.True(validated.IsFailure);
    }

    [Fact]
    public async Task Instance_MapErrorValueAsync_on_failure()
    {
        var result = Result.Failure<string, int>("err");
        var mapped = await result.MapErrorValueAsync(e => ValueTask.FromResult(e.Length));
        Assert.Equal(3, mapped.Error);
    }

    [Fact]
    public async Task Instance_BindValueAsync_on_success()
    {
        var result = Result.Success<string, int>(2);
        var bound = await result.BindValueAsync(v => ValueTask.FromResult(Result.Success<string, int>(v + 1)));
        Assert.Equal(3, bound.Value);
    }

    [Fact]
    public async Task Instance_BindValueAsync_on_failure()
    {
        var result = Result.Failure<string, int>("err");
        var bound = await result.BindValueAsync(v => ValueTask.FromResult(Result.Success<string, int>(v + 1)));
        Assert.Equal("err", bound.Error);
    }

    [Fact]
    public async Task Instance_RecoverValueAsync_on_failure()
    {
        var result = Result.Failure<string, int>("err");
        var recovered = await result.RecoverValueAsync(_ => ValueTask.FromResult(42));
        Assert.Equal(42, recovered.Value);
    }

    [Fact]
    public async Task Instance_RecoverValueAsync_on_success()
    {
        var result = Result.Success<string, int>(5);
        var recovered = await result.RecoverValueAsync(_ => ValueTask.FromResult(42));
        Assert.Equal(5, recovered.Value);
    }

    [Fact]
    public async Task Instance_OnSuccessValueAsync_runs_when_success()
    {
        var hit = false;
        var result = Result.Success<string, int>(1);
        await result.OnSuccessValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task Instance_OnSuccessValueAsync_skips_when_failure()
    {
        var hit = false;
        var result = Result.Failure<string, int>("err");
        await result.OnSuccessValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public async Task Instance_OnFailureValueAsync_runs_when_failure()
    {
        var hit = false;
        var result = Result.Failure<string, int>("err");
        await result.OnFailureValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task Instance_OnFailureValueAsync_skips_when_success()
    {
        var hit = false;
        var result = Result.Success<string, int>(1);
        await result.OnFailureValueAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public async Task Instance_SwitchValueAsync_runs_success_action_when_success()
    {
        var successHit = false;
        var failureHit = false;
        var result = Result.Success<string, int>(1);
        await result.SwitchValueAsync(
            async _ =>
            {
                await Task.Delay(1);
                successHit = true;
            },
            async _ =>
            {
                await Task.Delay(1);
                failureHit = true;
            });
        Assert.True(successHit);
        Assert.False(failureHit);
    }

    [Fact]
    public async Task Instance_SwitchValueAsync_runs_failure_action_when_failure()
    {
        var successHit = false;
        var failureHit = false;
        var result = Result.Failure<string, int>("err");
        await result.SwitchValueAsync(
            async _ =>
            {
                await Task.Delay(1);
                successHit = true;
            },
            async _ =>
            {
                await Task.Delay(1);
                failureHit = true;
            });
        Assert.False(successHit);
        Assert.True(failureHit);
    }

    #endregion
}
