using UnityEngine;
using System;
public class TriggerEnterBehaviour : MonoBehaviour {
	public event Action<Collider> Entered;
	private void OnTriggerEnter(Collider other)
		=> Entered?.Invoke(other);
}
