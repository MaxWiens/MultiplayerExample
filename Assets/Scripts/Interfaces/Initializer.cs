using UnityEngine;
/// <summary>
/// Initializable MonoBehaviour
/// </summary>
public abstract class Initializer : MonoBehaviour, IInitializable {
	public abstract void Init(params object[] args);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}
public abstract class Initializer<T1> : MonoBehaviour, IInitializable<T1> {
	public abstract void Init(T1 arg1);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}
public abstract class Initializer<T1,T2> : MonoBehaviour, IInitializable<T1,T2> {
	public abstract void Init(T1 arg1, T2 arg2);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}
public abstract class Initializer<T1,T2,T3> : MonoBehaviour, IInitializable<T1,T2,T3> {
	public abstract void Init(T1 arg1, T2 arg2, T3 arg3);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}
public abstract class Initializer<T1,T2,T3,T4> : MonoBehaviour, IInitializable<T1,T2,T3,T4> {
	public abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}
public abstract class Initializer<T1,T2,T3,T4,T5> : MonoBehaviour, IInitializable<T1,T2,T3,T4,T5> {
	public abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}
public abstract class Initializer<T1,T2,T3,T4,T5,T6> : MonoBehaviour, IInitializable<T1,T2,T3,T4,T5,T6> {
	public abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}
public abstract class Initializer<T1,T2,T3,T4,T5,T6,T7> : MonoBehaviour, IInitializable<T1,T2,T3,T4,T5,T6,T7> {
	public abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}
public abstract class Initializer<T1,T2,T3,T4,T5,T6,T7,T8> : MonoBehaviour, IInitializable<T1,T2,T3,T4,T5,T6,T7,T8> {
	public abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
	public abstract bool IsInitialized {get;}
	private void Start() {
		if(!IsInitialized) throw new NotInitializedException();
	}
}