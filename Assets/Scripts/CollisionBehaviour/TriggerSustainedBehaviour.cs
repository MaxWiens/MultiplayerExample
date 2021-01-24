using UnityEngine;
using System;
public class TriggerSustainedBehaviour : MonoBehaviour {
	public event Action<Collider> Sustained;
	private void OnTriggerStay(Collider other)
		=> Sustained?.Invoke(other);
}