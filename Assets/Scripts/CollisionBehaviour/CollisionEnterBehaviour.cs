using UnityEngine;
using System;
public class CollisionEnterBehaviour : MonoBehaviour {
	public event Action<Collision> Entered;
	private void OnCollisionEnter(Collision other)
		=> Entered?.Invoke(other);
}
