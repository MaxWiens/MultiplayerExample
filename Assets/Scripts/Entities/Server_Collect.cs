using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class Server_Collect : MonoBehaviour {
	[SerializeField, NotNull]
	private TriggerEnterBehaviour _triggerEnterBehaviour = null;
	[SerializeField, NotNull]
	private Server_ItemHandler _itemHandler = null;
	[SerializeField]
	private ItemTypeSet _pickupsToCollect = null;

	private void OnEnable() {
		_triggerEnterBehaviour.Entered += CheckCollision;
	}

	private void OnDisable() {
		_triggerEnterBehaviour.Entered -= CheckCollision;
	}

	private void CheckCollision(Collider other) {
		if(other.gameObject.TryGetComponent<IPickupable>(out IPickupable p) &&_pickupsToCollect.Overlaps(p.Item.ItemTypes)){
			if(_itemHandler.HandleItem(p.Item)){
				p.PickupItem();
			}
		}
	}
}
