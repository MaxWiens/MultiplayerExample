using UnityEngine;

public class Billboard : MonoBehaviour {
	public virtual void LateUpdate(){
		Vector3 v = Camera.main.transform.forward;
		if(v.y < -0.2f) v.y = -0.2f;
		transform.forward = v;
	}
}
