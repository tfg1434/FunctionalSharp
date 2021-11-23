using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

/// <summary>
/// Error is a record that may contain an inner Exception
/// </summary>
/// <param name="Message">Message for this error</param>
public record class Error(string Message) {
    /// <summary>
    /// Ctor
    /// </summary>
    public Error() : this("") { }
    
    //public Error(string message)... (primary constructor)

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="ex">Inner exception</param>
    public Error(Exception ex) : this(ex.Message) 
        => Ex = Just(ex);
    
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="ex">Inner exception</param>
    public Error(string message, Exception ex) : this(message) 
        => Ex = Just(ex);
    
    /// <summary>
    /// Inner exception for this <see cref="Error"/>
    /// </summary>
    [Pure]
    public Maybe<Exception> Ex { get; init; }
    
    /// <summary>
    /// Whether this <see cref="Error"/> contains an inner exception
    /// </summary>
    [Pure]
    public bool HasEx => Ex.IsJust;
    
    /// <summary>
    /// Unsafely get the inner exception
    /// </summary>
    /// <returns>Exception if <see cref="Ex"/> is in Just state</returns>
    /// <exception cref="InvalidOperationException"><see cref="Ex"/> is in Nothing state</exception>
    [Pure]
    public Exception ToEx()
        => Ex.Match(() => throw new InvalidOperationException("Error does not contain an exception"),
            ex => ex);
    
    /// <summary>
    /// Pattern-match type of inner exception
    /// </summary>
    /// <typeparam name="E">Type to match</typeparam>
    /// <returns>Whether inner exception is of type <typeparamref name="E"/></returns>
    [Pure]
    public bool Is<E>() where E : Exception
        => Ex.Map(ex => ex is E).GetOr(false);
    
    [Pure]
    public override string ToString() 
        => $"{Message} {Ex}";
}


