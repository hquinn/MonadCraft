using Xunit;

namespace MonadCraft.Tests;

public class OptionalTests
{
    [Fact]
    public void Some_sets_IsSome_true()
    {
        var option = Optional.Some(1);
        Assert.True(option.IsSome);
    }

    [Fact]
    public void None_sets_IsNone_true()
    {
        var option = Optional.None<int>();
        Assert.True(option.IsNone);
    }

    [Fact]
    public void Value_returns_inner_for_some()
    {
        var option = Optional.Some("hi");
        Assert.Equal("hi", option.Value);
    }

    [Fact]
    public void Value_throws_for_none()
    {
        var option = Optional.None<string>();
        Assert.Throws<InvalidOperationException>(() => option.Value);
    }

    [Fact]
    public void Error_constructor_guard_null()
    {
        Assert.Throws<ArgumentNullException>(() => new Optional<string>(null!));
    }

    [Fact]
    public void Implicit_non_null_creates_some()
    {
        Optional<int> option = 2;
        Assert.True(option.IsSome);
    }

    [Fact]
    public void Implicit_null_creates_none()
    {
        string? value = null;
        Optional<string> option = value;
        Assert.True(option.IsNone);
    }

    [Fact]
    public void Map_runs_on_some()
    {
        var mapped = Optional.Some(2).Map(v => v + 1);
        Assert.Equal(3, mapped.Value);
    }

    [Fact]
    public void Map_skips_on_none()
    {
        var mapped = Optional.None<int>().Map(v => v + 1);
        Assert.True(mapped.IsNone);
    }

    [Fact]
    public async Task MapAsync_runs_on_some()
    {
        var mapped = await Optional.Some(2).MapAsync(v => Task.FromResult(v + 1));
        Assert.Equal(3, mapped.Value);
    }

    [Fact]
    public async Task MapAsync_skips_on_none()
    {
        var mapped = await Optional.None<int>().MapAsync(v => Task.FromResult(v + 1));
        Assert.True(mapped.IsNone);
    }

    [Fact]
    public void Bind_runs_on_some()
    {
        var bound = Optional.Some(2).Bind(v => Optional.Some(v + 1));
        Assert.Equal(3, bound.Value);
    }

    [Fact]
    public void Bind_skips_on_none()
    {
        var bound = Optional.None<int>().Bind(v => Optional.Some(v + 1));
        Assert.True(bound.IsNone);
    }

    [Fact]
    public async Task BindAsync_runs_on_some()
    {
        var bound = await Optional.Some(2).BindAsync(v => Task.FromResult(Optional.Some(v + 1)));
        Assert.Equal(3, bound.Value);
    }

    [Fact]
    public async Task BindAsync_skips_on_none()
    {
        var bound = await Optional.None<int>().BindAsync(v => Task.FromResult(Optional.Some(v + 1)));
        Assert.True(bound.IsNone);
    }

    [Fact]
    public void Match_returns_some_branch()
    {
        var result = Optional.Some(2).Match(v => v + 1, () => -1);
        Assert.Equal(3, result);
    }

    [Fact]
    public void Match_returns_none_branch()
    {
        var result = Optional.None<int>().Match(v => v + 1, () => -1);
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task MatchAsync_returns_some_branch()
    {
        var result = await Optional.Some(2).MatchAsync(v => Task.FromResult(v + 1), () => Task.FromResult(-1));
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task MatchAsync_returns_none_branch()
    {
        var result = await Optional.None<int>().MatchAsync(v => Task.FromResult(v + 1), () => Task.FromResult(-1));
        Assert.Equal(-1, result);
    }

    [Fact]
    public void OnSome_runs_for_some()
    {
        var hit = false;
        Optional.Some(1).OnSome(_ => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public void OnSome_skips_for_none()
    {
        var hit = false;
        Optional.None<int>().OnSome(_ => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnSomeAsync_runs_for_some()
    {
        var hit = false;
        await Optional.Some(1).OnSomeAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnSomeAsync_skips_for_none()
    {
        var hit = false;
        await Optional.None<int>().OnSomeAsync(async _ =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public void OnNone_runs_for_none()
    {
        var hit = false;
        Optional.None<int>().OnNone(() => hit = true);
        Assert.True(hit);
    }

    [Fact]
    public void OnNone_skips_for_some()
    {
        var hit = false;
        Optional.Some(1).OnNone(() => hit = true);
        Assert.False(hit);
    }

    [Fact]
    public async Task OnNoneAsync_runs_for_none()
    {
        var hit = false;
        await Optional.None<int>().OnNoneAsync(async () =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.True(hit);
    }

    [Fact]
    public async Task OnNoneAsync_skips_for_some()
    {
        var hit = false;
        await Optional.Some(1).OnNoneAsync(async () =>
        {
            await Task.Delay(1);
            hit = true;
        });
        Assert.False(hit);
    }

    [Fact]
    public void Where_keeps_when_predicate_true()
    {
        var filtered = Optional.Some(2).Where(v => v > 1);
        Assert.True(filtered.IsSome);
    }

    [Fact]
    public void Where_discards_when_predicate_false()
    {
        var filtered = Optional.Some(1).Where(v => v > 1);
        Assert.True(filtered.IsNone);
    }

    [Fact]
    public void OrElse_returns_self_when_some()
    {
        var result = Optional.Some(1).OrElse(Optional.Some(2));
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void OrElse_returns_fallback_when_none()
    {
        var result = Optional.None<int>().OrElse(Optional.Some(2));
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public void OrElse_factory_runs_when_none()
    {
        var result = Optional.None<int>().OrElse(() => Optional.Some(3));
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void GetValueOrDefault_returns_value_when_some()
    {
        var value = Optional.Some(1).GetValueOrDefault(9);
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValueOrDefault_returns_fallback_when_none()
    {
        var value = Optional.None<int>().GetValueOrDefault(9);
        Assert.Equal(9, value);
    }

    [Fact]
    public void GetValueOrElse_returns_value_when_some()
    {
        var value = Optional.Some(1).GetValueOrElse(() => 9);
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValueOrElse_returns_factory_when_none()
    {
        var value = Optional.None<int>().GetValueOrElse(() => 9);
        Assert.Equal(9, value);
    }

    [Fact]
    public void ToEnumerable_returns_item_when_some()
    {
        var items = Optional.Some(5).ToEnumerable();
        Assert.Contains(5, items);
    }

    [Fact]
    public void ToEnumerable_empty_when_none()
    {
        var items = Optional.None<int>().ToEnumerable();
        Assert.Empty(items);
    }

    [Fact]
    public void ToResult_success_when_some()
    {
        var result = Optional.Some("ok").ToResult("err");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ToResult_failure_when_none()
    {
        var result = Optional.None<string>().ToResult("err");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void ToResult_factory_runs_when_none()
    {
        var result = Optional.None<int>().ToResult(() => "err");
        Assert.Equal("err", result.Error);
    }

    [Fact]
    public void Try_returns_some_on_success()
    {
        var option = Optional.Try(() => 4);
        Assert.True(option.IsSome);
    }

    [Fact]
    public void Try_returns_none_on_exception()
    {
        var option = Optional.Try<int>(() => throw new InvalidOperationException());
        Assert.True(option.IsNone);
    }

    [Fact]
    public void Select_projects_value()
    {
        var projected = Optional.Some(2).Select(v => v + 1);
        Assert.Equal(3, projected.Value);
    }

    [Fact]
    public void SelectMany_returns_projected_when_all_some()
    {
        var projected = Optional.Some(2).SelectMany(v => Optional.Some(v + 1), (v, w) => v + w);
        Assert.Equal(5, projected.Value);
    }

    [Fact]
    public void SelectMany_returns_none_when_binder_none()
    {
        var projected = Optional.Some(2).SelectMany(_ => Optional<int>.None, (v, w) => v + w);
        Assert.True(projected.IsNone);
    }

    [Fact]
    public void SelectMany_returns_none_when_initial_none()
    {
        var projected = Optional<int>.None.SelectMany(v => Optional.Some(v + 1), (v, w) => v + w);
        Assert.True(projected.IsNone);
    }
}