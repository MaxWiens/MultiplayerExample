using System;
using UnityEngine;
public interface IManaged {
	void SetCallbacks(Action<GameObject> deactivate);
}