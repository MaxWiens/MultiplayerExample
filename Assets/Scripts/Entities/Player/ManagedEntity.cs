using System;
using UnityEngine;

public class ManagedEntity : MonoBehaviour, IEntity, IManaged {
	public EntityData Data {get; set;}
	private Action<GameObject> _deactivator;

	public void SetCallbacks(Action<GameObject> deactivate){
		_deactivator = deactivate;
	}

	private void OnDisable() {
		_deactivator?.Invoke(this.gameObject);
	}
}