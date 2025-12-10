# MonadCraft

Lightweight `Optional` and `Result` types with async helpers, LINQ support, and single-assert test coverage.

## Installation

```bash
dotnet add package MonadCraft
```

Targets: `net10.0` (and compatible lower TFMs via NuGet dependency resolution).

## Quickstart

### Optional

```csharp
var maybeName = Optional.Some("Ada");

var greeting = maybeName
	.Select(name => name.ToUpperInvariant())
	.Where(name => name.StartsWith('A'))
	.OrElse(Optional.None<string>())
	.GetValueOrDefault("Guest");

var safeNumber = Optional.Try(() => int.Parse("12"));
```

### Result

```csharp
var parsed = Result.Try<string, int>(
	() => int.Parse("42"),
	ex => ex.Message);

var validated = await parsed
	.EnsureAsync(value => Task.FromResult(value > 0), "must be positive")
	.RecoverAsync(error => Task.FromResult(-1));

var projection =
	from a in Result.Success<string, int>(2)
	from b in Result.Success<string, int>(3)
	select a + b; // 5
```

## Key APIs

- `Optional<T>`: `Some`, `None`, `Try`, `Match/MatchAsync`, `Map/MapAsync`, `Bind/BindAsync`, `OnSome/OnNone` (+ async), `Where`, `OrElse`, `GetValueOrDefault`, `GetValueOrElse`, `ToEnumerable`, `ToResult`.
- `Result<TError, TValue>`: `Success`, `Failure`, `Try/TryAsync`, `Match/MatchAsync`, `Map/MapAsync`, `MapError/MapErrorAsync`, `Bind/BindAsync`, `Ensure/EnsureAsync`, `Recover/RecoverAsync`, `OnSuccess/OnFailure` (+ async), `Switch/SwitchAsync` overloads, `GetValueOrDefault`, `GetValueOrElse`, `ToOptional`.
- Async extensions: `OptionAsyncExtensions` and `ResultAsyncExtensions` mirror the sync APIs for `Task<Optional<T>>` and `Task<Result<TError,TValue>>` (including `OrElseAsync`, `EnsureAsync`, `RecoverAsync`, `ToOptionalAsync`).

## LINQ support

Both `Optional<T>` and `Result<TError, TValue>` implement `Select`/`SelectMany` so you can compose with query syntax.

```csharp
var total =
	from a in Optional.Some(1)
	from b in Optional.Some(2)
	select a + b; // Some(3)

var combined =
	from a in Result.Success<string, int>(1)
	from b in Result.Success<string, int>(2)
	select a + b; // Success(3)
```
