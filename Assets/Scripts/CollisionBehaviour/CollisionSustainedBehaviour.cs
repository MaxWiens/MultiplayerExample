using UnityEngine;
using System;
public class CollisionSustainedBehaviour : MonoBehaviour {
	public event Action<Collision> Sustained;
	private void OnCollisionStay(Collision other)
		=> Sustained?.Invoke(other);
}