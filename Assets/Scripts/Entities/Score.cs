using UnityEngine;
using System;
public class Score : MonoBehaviour {
	[SerializeField]
	private int _value;
	public int Value {get=>_value;
		set{
			_value=value;
			ScoreChanged?.Invoke(this);
		}
	}
	public event Action<Score> ScoreChanged;
}