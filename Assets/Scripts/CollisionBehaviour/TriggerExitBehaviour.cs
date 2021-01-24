using UnityEngine;
using System;
public class TriggerExitBehaviour : MonoBehaviour {
	public event Action<Collider> Exited;
	private void OnTriggerExit(Collider other)
		=> Exited?.Invoke(other);
}
