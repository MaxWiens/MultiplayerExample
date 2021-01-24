using UnityEngine;
using System;
public class CollisionExitBehaviour : MonoBehaviour {
	public event Action<Collision> Exited;
	private void OnCollisionExit(Collision other)
		=> Exited?.Invoke(other);
}
