using System;

namespace FPLibrary;

public record class CancelledError() : Error("Cancelled", null);

public record class IOError(string Message, Exception? Ex) : Error(Message, Ex);


