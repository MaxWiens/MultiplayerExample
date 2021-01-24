using UnityEngine;

public class Score : MonoBehaviour, IScorable {
	[SerializeField]
	private int _value;
	public int Value {get=>_value; set=>_value=value;}
}