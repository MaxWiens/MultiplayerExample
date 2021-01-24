using UnityEngine;
using System;
[CreateAssetMenu(fileName = "NewGameObjectFactory", menuName = "MultiplayerExample/Factory/GameObjectFactory", order = 0)]
public class GameObjectFactory : FactorySO<GameObject> {

	[SerializeField, NotNull]
	private GameObject _prefab = null;

	public override GameObject Create(){
		return Instantiate(_prefab);
	}
}