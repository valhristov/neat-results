# Neat Results

Simple implementation of the Result "pattern" that forces you to handle failures.

Feel free to copy the classes into your application.

Bug reports are welcome. Suggestions will be considered, but my main incentive is to keep the code as simple as possible.

# How to use

## Create results

Create a successful result with a value:
```
return Result.Success(value);
```

Create failed result with error messages:
```
return Result.Failure(error);
```

## Transform results into other results

Transform results when chaining operations:
```
Result<int> GetInt() =>
	Result.Success(random.Next());

Result<string> Stringify(Result<int> result) =>
	result.Select(
		value => Result.Success(value.ToString()));
```

## Use result value ignoring the errors

When you are only interested in the result value:
```
var result = GetInt();
var i = result.ValueOrDefault(5); // will return 5 if result is failure
// or
var j = result.ValueOrThrow(); // will throw if result is failure
// or
var k = result.Value(
	value => value,
	errors => 
	{
		Logger.Log(errors); // log errors and return some other value
		return 5;
	});
```

## Sink result into a side effect action

Make side effects depending on the result outcome:

```
var result = GetInt();
result.Match(
	value => UseValue(value),
	errors => Logger.Log(errors)
);
```

