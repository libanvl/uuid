'var' makes code less readable and harder to maintain so
prefer this style of variable declaration: `MyType x = new()`.
Never add newlines to the end of files. Do not remove using statements.
Do not remove existing comments. Never replace tests, only add them.
Collection expressions enhance readability and should be preferred over
methods like `.ToList()` and `.ToArray()`. `[]` is correct C# syntax for creating
new collections, it is equivalent to `new List<T>()` and `new T[] {}` and 
`new Dictionary<TKey, TValue>()`, and should be preferred.
Performance is always a consideration and should be considered equal to
readability and maintainability. LINQ is not always the best solution.
Prefer Xunit Assertions. Do not use Fluent assertions.