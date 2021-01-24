using System;
using UnityEngine;
public class Server_ManagedPickup : Server_Pickup, IManaged {
	private Action<GameObject> _deactivator;

	public override ItemSO PickupItem(){
		ItemSO item = base.PickupItem();
		_deactivator?.Invoke(gameObject);
		return item;
	}

	public void SetCallbacks(Action<GameObject> deactivate) {
		_deactivator = deactivate;
	}

	private void OnDisable() {
		_deactivator?.Invoke(gameObject);
	}
}