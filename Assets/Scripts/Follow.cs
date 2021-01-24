using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {
	public bool SnapToTargetOnEnable = true;
	[Min(0)]
	public float SnappingDistance = 0.01f;
	[Min(0), Tooltip("Inverse of follow strength")]
	public float DampenTime = 0.01f;
	public Transform Target;

	private Vector3 _velocity = Vector3.zero;

	private void OnEnable() {
		if(SnapToTargetOnEnable){
			transform.position = Target.position;
		}
	}

	private void LateUpdate() {
		if(Vector3.Distance(transform.position, Target.position) <= SnappingDistance){
			transform.position = Target.position;
		}else{
			transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref _velocity, DampenTime);
		}
	}
}