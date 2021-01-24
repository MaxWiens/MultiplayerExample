using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
public abstract class PoolSO<T> : ScriptableObject, IPool<T> where T : class {
	[SerializeField]
	private int _maxPoolSize = 20;
	protected readonly Stack<T> availableObjects = new Stack<T>();
	protected readonly HashSet<T> activeObjects = new HashSet<T>();

	public int MaxPoolSize => _maxPoolSize < 0 ? int.MaxValue : _maxPoolSize;
	public int Count => availableObjects.Count + activeObjects.Count;

	protected abstract IFactory<T> factory {get;}

	public bool HasObjectAvailable => availableObjects.Count > 0;

	public bool IsFull => activeObjects.Count >= MaxPoolSize && availableObjects.Count == 0;

	public virtual T Get() {
		T t;
		if(availableObjects.Count > 0)
			t = availableObjects.Pop();
		else if(activeObjects.Count < MaxPoolSize)
			t = factory.Create();
		else
			return null;
		activeObjects.Add(t);
		return t;
	}

	public virtual bool PutBack(T pooledObject) {
		if(activeObjects.Remove(pooledObject)){
			availableObjects.Push(pooledObject);
			return true;
		}
		return false;
	}

	public void Fill(int amount){
		int c = Count;
		if(amount + c > MaxPoolSize){
			amount = c - amount;
		}
		for (int i = 0; i < amount; i++) {
			availableObjects.Push(factory.Create());
		}
	}

	public void FillTo(int amount){
		if(amount > MaxPoolSize){
			amount = MaxPoolSize;
		}
		for (int i = Count; i < amount; i++) {
			availableObjects.Push(factory.Create());
		}
	}

	public IEnumerable<T> ActiveObjects => activeObjects;
	public IEnumerable<T> InactiveObjects => availableObjects;
}