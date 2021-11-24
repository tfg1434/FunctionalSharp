<p align="center">
  
  <img src="FunctionalSharp.png" style="height: 250px;">
  
</p>

<h1 align="center">FunctionalSharp</h1>
<p align="center">Functional types and patterns for C#</p>

<p align="center">
  
  <img src="https://github.com/torontofangirl/FunctionalSharp/actions/workflows/tests.yml/badge.svg">
  
</p>
  
| FunctionalSharp | Vanilla |
| :-------------: | :-----: |
| <img src="https://cdn.discordapp.com/attachments/818274903769481237/911704205226557440/Untitled_2.jpeg" alt="functional pic"> | <img src="https://cdn.discordapp.com/attachments/818274903769481237/911704205700521984/Untitled_3.jpeg" alt="low level pic"> |
| <ul><li>Flat, monadic composition</li><li>No boilerplate (try/catch, TryParseXXX, ...)</li></ul> | <ul><li>try/catch boilerplate</li><li>Clunky TryParseXXX pattern</li></ul> |
| <img src="https://cdn.discordapp.com/attachments/818274903769481237/911704204819722240/Untitled_1.jpeg" alt="functional pic"> | <img src="https://cdn.discordapp.com/attachments/818274903769481237/911704205494997042/Untitled.jpeg" alt="low level pic"> |
| <ul><li>One line</li><li>Infinite ranges</li></ul> | <ul><li>Difficult to read</li></ul> |
| <img src="https://cdn.discordapp.com/attachments/818274903769481237/911704205926998076/Untitled_4.jpeg" alt="functional pic"> | <img src="https://cdn.discordapp.com/attachments/818274903769481237/911704206208028672/Untitled_5.jpeg" alt="low level pic"> |
| <ul><li>List pattern matching</li><li>Functional style singly linked list</li></ul> | <ul><li>Mutates original collection</li></ul> |

## Setup
Install the package from nuget or add a reference to the dll in your project.

Then, all you need to do is add 
```cs
global using FunctionalSharp;
global using static FunctionalSharp.F;
global using Unit = System.ValueTuple;
```
into a file for each **project** you want to use this lib in.

## Documentation
XML docs are provided for most functions. 
If these are insufficient, you can always look at the source code; I've tried my best to make the code as pretty as 
possible.

Usage examples and information about key types are provided in [Types](#types)

## Types
## Unit
`Unit` is defined as `using Unit = System.ValueTuple;`. To return `Unit` from a method, simply use `return Unit()`.

`Unit` is advantageous over `void` because `void` cannot be used like a real type. Using `Unit` in place of `void` lets 
you do things like interop between `Func` and `Action`, or make Linq queries easier. You can easily convert an `Action`
to a `Unit`-returning `Func` by using `.ToFunc()`.

In fact, there have even been discussions about adding [`Unit`-like functionality to C#](https://github.com/dotnet/csharplang/blob/2802e29f4c539faa058855f54b5653daa9c087b2/meetings/2021/LDM-2021-10-25.md#delegate-type-argument-improvements).

