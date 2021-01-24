public interface IPool<T> {
	void Fill(int amount);
	T Get();
	bool PutBack(T pooledObject);
}