namespace FPLibrary.Tests; 

public class StatefulTests {
    private abstract record Tree<T>;

    private record Leaf<T>(T Value) : Tree<T>;
    
    private record Branch<T>(Tree<T> Left, Tree<T> Right) : Tree<T>;
    
    
}