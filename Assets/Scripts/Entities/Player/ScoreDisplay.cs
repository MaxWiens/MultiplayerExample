using UnityEngine;
using TMPro;
using System;

public class ScoreDisplay : MonoBehaviour {
	[SerializeField, NotNull]
	private Score _score = null;
	[SerializeField, NotNull]
	private TextMeshPro _text = null;
	private void OnEnable() {
		_score.ScoreChanged += OnScoreChanged;
	}
	private void OnDisable() {
		_score.ScoreChanged -= OnScoreChanged;
	}
	private void Start() {
		OnScoreChanged(_score);
	}
	private void OnScoreChanged(Score s){
		_text.text = s.Value.ToString();
	}
}