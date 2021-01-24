public interface IInitiable{ bool IsInitialized {get;} }
public interface IInitializable : IInitiable { void Init(params object[] args); }
public interface IInitializable<T1> : IInitiable { void Init(T1 arg1); }
public interface IInitializable<T1,T2> : IInitiable { void Init(T1 arg1, T2 arg2); }
public interface IInitializable<T1,T2,T3> : IInitiable { void Init(T1 arg1, T2 arg2, T3 arg3); }
public interface IInitializable<T1,T2,T3,T4> : IInitiable { void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4); }
public interface IInitializable<T1,T2,T3,T4,T5> : IInitiable { void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5); }
public interface IInitializable<T1,T2,T3,T4,T5,T6> : IInitiable { void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6); }
public interface IInitializable<T1,T2,T3,T4,T5,T6,T7> : IInitiable { void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7); }
public interface IInitializable<T1,T2,T3,T4,T5,T6,T7,T8> : IInitiable { void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8); }

[System.Serializable]
public class InitalizationException : System.Exception
{
	public InitalizationException() { }
	public InitalizationException(string message) : base(message) { }
	public InitalizationException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class NotInitializedException : InitalizationException
{
	public NotInitializedException() { }
	public NotInitializedException(string message) : base(message) { }
	public NotInitializedException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class FailedToInitalizeException : InitalizationException
{
	public FailedToInitalizeException() { }
	public FailedToInitalizeException(string message) : base(message) { }
	public FailedToInitalizeException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class AlreadyInitalizedException : InitalizationException
{
	public AlreadyInitalizedException() { }
	public AlreadyInitalizedException(string message) : base(message) { }
	public AlreadyInitalizedException(string message, System.Exception inner) : base(message, inner) { }
}
