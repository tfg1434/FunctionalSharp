using static FPLibrary.F;

namespace FPLibrary.Wrappers.File; 

public readonly struct LiveRuntime : IHasFile<LiveRuntime> {
    private LiveRuntime(LiveRuntimeEnv env)
        => _env = env;

    public LiveRuntime() : this(new()) { }

    public LiveRuntimeEnv Env => _env ?? throw new InvalidOperationException("null, did you use the ctor?");

    public IO<LiveRuntime, IFileIO> FileIO 
        => IOSucc<LiveRuntime, IFileIO>(LiveFileIO.Default);
    
    private readonly LiveRuntimeEnv? _env;
}

public class LiveRuntimeEnv { }