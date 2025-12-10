using Xunit;

namespace MonadCraft.Tests;

public class ResultTests
{
    [Fact]
    public void Success_sets_IsSuccess_true()
    {
        var result = Result.Success<string, int>(1);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Failure_sets_IsFailure_true()
    {
        var result = Result.Failure<string, int>("err");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Value_returns_for_success()
    {
        var value = Result.Success<string, int>(2).Value;
        Assert.Equal(2, value);
    }

    [Fact]
    public void Value_throws_for_failure()
    {
        var result = Result.Failure<string, int>("err");
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Error_returns_for_failure()
    {
        var error = Result.Failure<string, int>("err").Error;
        Assert.Equal("err", error);
    }

    [Fact]
    public void Error_throws_for_success()
    {
        var result = Result.Success<string, int>(1);
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void Match_returns_success_branch()
    {
        var value = Result.Success<string, int>(2).Match(v => v + 1, _ => -1);
        Assert.Equal(3, value);
    }

    [Fact]
    public void Match_returns_failure_branch()
    {
        var value = Result.Failure<string, int>("err").Match(v => v + 1, _ => -1);
        Assert.Equal(-1, value);
    }

    [Fact]
    public async Task MatchAsync_returns_success_branch()
    {
        var value = await Result.Success<string, int>(2)
            .MatchAsync(v => Task.FromResult(v + 1), _ => Task.FromResult(-1));
        Assert.Equal(3, value);
    }

    [Fact]
    public async Task MatchAsync_returns_failure_branch()
    {
        var value = await Result.Failure<string, int>("err")
            .MatchAsync(v => Task.FromResult(v + 1), _ => Task.FromResult(-1));
        Assert.Equal(-1, value);
    }

    [Fact]
    public void Map_runs_on_success()
    {
        var mapped = Result.Success<string, int>(2).Map(v => v + 1);
        Assert.Equal(3, mapped.Value);
    }

    [Fact]
    public void Map_skips_on_failure()
    {
        var mapped = Result.Failure<string, int>("err").Map(v => v + 1);
        Assert.Equal("err", mapped.Error);
    }

    [Fact]
    public async Task MapAsync_runs_on_success()
    {
        var mapped = await Result.Success<string, int>(2).MapAsync(v => Task.FromResult(v + 1));
        Assert.Equal(3, mapped.Value);
    }

    [Fact]
    public async Task MapAsync_skips_on_failure()
    {
        var mapped = await Result.Failure<string, int>("err").MapAsync(v => Task.FromResult(v + 1));
        Assert.Equal("err", mapped.Error);
    }

    [Fact]
    public void MapError_runs_on_failure()
    {
        var mapped = Result.Failure<string, int>("err").MapError(e => e.Length);
        Assert.Equal(3, mapped.Error);
    }

    [Fact]
    public void MapError_preserves_success()
    {
        var mapped = Result.Success<string, int>(2).MapError(e => e.Length);
        Assert.Equal(2, mapped.Value);
    }

    [Fact]
    public async Task MapErrorAsync_runs_on_failure()
    {
        var mapped = await Result.Failure<string, int>("err").MapErrorAsync(e => Task.FromResult(e.Length));
        Assert.Equal(3, mapped.Error);
    }

    [Fact]
    public async Task MapErrorAsync_preserves_success()
    {
        var mapped = await Result.Success<string, int>(2).MapErrorAsync(e => Task.FromResult(e.Length));
        Assert.Equal(2, mapped.Value);
    }

    [Fact]
    public void Bind_runs_on_success()
    {
        var bound = Result.Success<string, int>(2).Bind(v => Result.Success<string, int>(v + 1));
        Assert.Equal(3, bound.Value);
    }

    [Fact]
    public void Bind_skips_on_failure()
    {
        var bound = Result.Failure<string, int>("err").Bind(v => Result.Success<string, int>(v + 1));
        Assert.Equal("err", bound.Error);
    }

    [Fact]
    public async Task BindAsync_runs_on_success()
    {
        var bound = await Result.Success<string, int>(2)
            .BindAsync(v => Task.FromResult(Result.Success<string, int>(v + 1)));
        Assert.Equal(3, bound.Value);
    }

    [Fact]
    public async Task BindAsync_skips_on_failure()
    {
        var bound = await Result.Failure<string, int>("err")
            .BindAsync(v => Task.FromResult(Result.Success<string, int>(v + 1)));
        Assert.Equal("err", bound.Error);
    }

    [Fact]
    public void Ensure_keeps_success_when_valid()
    {
        var result = Result.Success<string, int>(2).Ensure(v => v > 1, "bad");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Ensure_converts_to_failure_when_invalid()
    {
        var result = Result.Success<string, int>(0).Ensure(v => v > 1, "bad");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Ensure_leaves_failure_unchanged()
    {
        var result = Result.Failure<string, int>("err").Ensure(v => v > 1, "bad");
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public async Task EnsureAsync_keeps_success_when_valid()
    {
        var result = await Result.Success<string, int>(2).EnsureAsync(v => Task.FromResult(v > 1), "bad");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task EnsureAsync_converts_to_failure_when_invalid()
    {
        var result = await Result.Success<string, int>(0).EnsureAsync(v => Task.FromResult(v > 1), "bad");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task EnsureAsync_leaves_failure_unchanged()
    {
        var result = await Result.Failure<string, int>("err").EnsureAsync(v => Task.FromResult(v > 1), "bad");
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public void Recover_converts_failure_to_success()
    {
        var result = Result.Failure<string, int>("err").Recover(_ => 5);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public void Recover_leaves_success_unchanged()
    {
        var result = Result.Success<string, int>(2).Recover(_ => 5);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task RecoverAsync_converts_failure_to_success()
    {
        var result = await Result.Failure<string, int>("err").RecoverAsync(_ => Task.FromResult(5));
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task RecoverAsync_leaves_success_unchanged()
    {
        var result = await Result.Success<string, int>(2).RecoverAsync(_ => Task.FromResult(5));
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public void OnSuccess_runs_for_success()
    {
        var hit = false;
        Result.Success<string, int>(2).OnSuccess(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public void OnSuccess_skips_for_failure()
    {
        var hit = false;
        Result.Failure<string, int>("err").OnSuccess(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnSuccessAsync_runs_for_success()
    {
        var hit = false;
        await Result.Success<string, int>(2).OnSuccessAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSuccessAsync_skips_for_failure()
    {
        var hit = false;
        await Result.Failure<string, int>("err").OnSuccessAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public void OnFailure_runs_for_failure()
    {
        var hit = false;
        Result.Failure<string, int>("err").OnFailure(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public void OnFailure_skips_for_success()
    {
        var hit = false;
        Result.Success<string, int>(2).OnFailure(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnFailureAsync_runs_for_failure()
    {
        var hit = false;
        await Result.Failure<string, int>("err").OnFailureAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnFailureAsync_skips_for_success()
    {
        var hit = false;
        await Result.Success<string, int>(2).OnFailureAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public void Switch_calls_success_action()
    {
        var hit = false;
        Result.Success<string, int>(2).Switch(_ => hit = true, _ => hit = false);
        Assert.True(hit);
    }

    [Fact]
    public void Switch_calls_failure_action()
    {
        var hit = false;
        Result.Failure<string, int>("err").Switch(_ => hit = false, _ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_funcs_call_success_action()
    {
        var hit = false;
        await Result.Success<string, int>(2)
            .SwitchAsync(_ => Task.Run(() => hit = true), _ => Task.Run(() => hit = false));
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_funcs_call_failure_action()
    {
        var hit = false;
        await Result.Failure<string, int>("err")
            .SwitchAsync(_ => Task.Run(() => hit = false), _ => Task.Run(() => hit = true));
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_action_then_func_calls_success_action()
    {
        var hit = false;
        await Result.Success<string, int>(2).SwitchAsync(_ => hit = true, _ => Task.Run(() => hit = false));
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_action_then_func_calls_failure_action()
    {
        var hit = false;
        await Result.Failure<string, int>("err").SwitchAsync(_ => hit = false, _ => Task.Run(() => hit = true));
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_func_then_action_calls_success_action()
    {
        var hit = false;
        await Result.Success<string, int>(2).SwitchAsync(_ => Task.Run(() => hit = true), _ => hit = false);
        Assert.True(hit);
    }

    [Fact]
    public async Task SwitchAsync_func_then_action_calls_failure_action()
    {
        var hit = false;
        await Result.Failure<string, int>("err").SwitchAsync(_ => Task.Run(() => hit = false), _ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public void GetValueOrDefault_returns_value_for_success()
    {
        var value = Result.Success<string, int>(2).GetValueOrDefault(9);
        Assert.Equal(2, value);
    }

    [Fact]
    public void GetValueOrDefault_returns_fallback_for_failure()
    {
        var value = Result.Failure<string, int>("err").GetValueOrDefault(9);
        Assert.Equal(9, value);
    }

    [Fact]
    public void GetValueOrElse_returns_value_for_success()
    {
        var value = Result.Success<string, int>(2).GetValueOrElse(_ => 9);
        Assert.Equal(2, value);
    }

    [Fact]
    public void GetValueOrElse_returns_fallback_for_failure()
    {
        var value = Result.Failure<string, int>("err").GetValueOrElse(err => err.Length);
        Assert.Equal(3, value);
    }

    [Fact]
    public void ToOptional_returns_some_for_success()
    {
        var option = Result.Success<string, int>(2).ToOptional();
        Assert.True(option.IsSome);
    }

    [Fact]
    public void ToOptional_returns_none_for_failure()
    {
        var option = Result.Failure<string, int>("err").ToOptional();
        Assert.True(option.IsNone);
    }

    [Fact]
    public void Select_projects_value()
    {
        var projected = Result.Success<string, int>(2).Select(v => v + 1);
        Assert.Equal(3, projected.Value);
    }

    [Fact]
    public void SelectMany_returns_projected_when_both_success()
    {
        var projected = Result.Success<string, int>(2)
            .SelectMany(v => Result.Success<string, int>(v + 1), (v, w) => v + w);
        Assert.Equal(5, projected.Value);
    }

    [Fact]
    public void SelectMany_returns_failure_when_binder_fails()
    {
        var projected = Result.Success<string, int>(2)
            .SelectMany(_ => Result.Failure<string, int>("err"), (v, w) => v + w);
        Assert.True(projected.IsFailure);
    }

    [Fact]
    public void SelectMany_returns_failure_when_initial_failure()
    {
        var projected = Result.Failure<string, int>("err")
            .SelectMany(v => Result.Success<string, int>(v + 1), (v, w) => v + w);
        Assert.True(projected.IsFailure);
    }

    [Fact]
    public void Deconstruct_outputs_value_and_error()
    {
        var result = Result.Success<string, int>(2);
        result.Deconstruct(out var value, out var error);
        Assert.Equal(2, value);
    }

    [Fact]
    public void Implicit_success_from_value()
    {
        Result<string, int> result = 3;
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Implicit_failure_from_error()
    {
        Result<string, int> result = "err";
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Try_returns_success_on_factory()
    {
        var result = Result.Try<string, int>(() => 4, _ => "err");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Try_returns_failure_on_exception()
    {
        var result = Result.Try<string, int>(() => throw new InvalidOperationException(), ex => ex.Message);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task TryAsync_returns_success_on_factory()
    {
        var result = await Result.TryAsync<string, int>(() => Task.FromResult(4), _ => "err");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TryAsync_returns_failure_on_exception()
    {
        var result =
            await Result.TryAsync<string, int>(() => Task.FromException<int>(new InvalidOperationException("boom")),
                ex => ex.Message);
        Assert.True(result.IsFailure);
    }
}