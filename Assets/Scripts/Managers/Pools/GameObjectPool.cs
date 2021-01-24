using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameObjectPool", menuName = "MultiplayerExample/Pool/GameObjectPool", order = 0)]
public class GameObjectPoolSO : PoolSO<GameObject> {
	[SerializeField, NotNull]
	private GameObjectFactory _factory = null;
	protected override IFactory<GameObject> factory => _factory;
	/// <summary>
	/// Gets an enabled GameObject
	/// </summary>
	/// <returns>Gameobject from pool, null if full</returns>
	public override GameObject Get(){
		GameObject o = base.Get();
		o?.SetActive(true);
		return o;
	}

	/// <summary>
	/// Return gameobject to pool and disables it.
	/// /// </summary>
	/// <param name="pooledObject">Pooled object to return</param>
	/// <returns>true if object returned successfully false if object doesn't belong in pool</returns>
	public override bool PutBack(GameObject pooledObject) {
		if(base.PutBack(pooledObject)){
			pooledObject.SetActive(false);
			return true;
		}
		return false;
	}
}