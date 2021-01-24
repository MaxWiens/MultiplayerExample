using UnityEngine;
using System;
public class FixedUpdater : MonoBehaviour {
	private Action _updateAction;
	public Action UpdateAction { set=> _updateAction = value; }
	protected virtual void OnEnable() {
		if(_updateAction == null){
			Debug.LogError("Update Action not set in updater!");
		}
	}
	private void FixedUpdate() => _updateAction();
}