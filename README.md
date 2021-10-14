# FunctionalSharp: A lightweight functional programming library

![example workflow](https://github.com/torontofangirl/FunctionalSharp/actions/workflows/tests.yml/badge.svg)

### WIP C# functional programming library
This library brings common functional programming patterns and types to C#.

### Why don't you just use a functional programming language?
I choose to use C# over an FP language for the .NET ecosystem
and large userbase. Furthermore, all paradigms have their merits and C#, being
multi-paradigm, is great for this purpose.

## Features
#### Maybe&lt;T&gt;
`Maybe<T>` is a discriminated union type with two possible states: `Just<T>`, and `Nothing`

This eliminates `NullReferenceException`s entirely, since you are forced to handle both states
by the compiler (unlike `null`). Furthermore, your type signature is much more robust, and your
method becomes honest.

#### Either&lt;L, R&gt;
`Either<L, R>` is a discriminated union type with two possible states: `Left<L>`, and `Right<R>`

Either is the functional way of handling exceptions. This is advantageous since function signatures
are more robust. Also, methods become honest and pure, and you can do away with exception handling.

#### Map&lt;K, V&gt;
`Map<K, V>` is a WIP implementation of an Immutable dictionary. It uses an AVL tree as the under
the hood to provide O(N) in worst-case scenarios.

#### Lst&lt;T&gt;
`Lst<T>` is a WIP immutable singly linked list, commonly used in functional programming.

#### Unit
`Unit` is common in functional programming languages -- it allows you to represent the absence of 
data. This is advantageous over `void` since it's not a real type. A common use is bridging the
gap between `Action` (delegate with void return type) and `Func` (delegate with a return type).

So, you only have write one overload for `Func`, and then call `.ToFunc()` to convert the 
`Action` to a `Unit`-returning `Func`.

