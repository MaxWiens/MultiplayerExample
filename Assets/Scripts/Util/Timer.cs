using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ITimer {
	bool IsLooping {get;}
	float EndTime {get;}
	float CurrentTime {get;}
	bool IsFinished {get;}
	bool IsActive {get;}
	void SetActive(bool isActive, bool reset);
	void ResetTime();
}

[Serializable]
public class Timer {
	/// <summary>Invoked when the timer has reached it's end and when a loop is complete.</summary>
	public event Action Finished;
	/// <summary>Invoked whenever the timer becomes inactive from finishing or being paused.</summary>
	public event Action BecameInactive;
	/// <summary>Invoked whenever the timer becomes active after from being inactive.</summary>
	public event Action BecameActive;

	[SerializeField, Min(0f)]
	private float _endTime = 1f;
	[SerializeField]
	private bool _isLooping = false;
	[SerializeField]
	private bool _isActive = true;

	public bool IsLooping => _isLooping;
	public float EndTime => _endTime;
	public float CurrentTime {get; private set;} = 0f;
	public float PercentComplete => CurrentTime/EndTime;
	public bool IsActive => _isActive;
	public bool HasFinished {get; private set;} = false;
	public bool IsFinished => CurrentTime >= EndTime;

	private Func<bool> _stopPredicate = null;
	private bool _resetAfterStopPredicate = false;
	private float _stopTimer = 0f;

	public void ResetTimer() => CurrentTime = 0;

	public void ResetContinue(){
		CurrentTime = 0;
		_isActive = true;
	}

	public void Continue(){
		_isActive = true;
		_stopPredicate = null;
	}

	public void Stop(){
		_isActive = false;
	}

	public void StopUntil(float timeElapsed, bool restartAfter = false) {
		_isActive = false;
		_stopTimer = 0f;
		_stopPredicate = ()=>_stopTimer > timeElapsed ? true : false;
		_resetAfterStopPredicate = restartAfter;
	}

	public void StopUntil(Func<bool> predicate, bool restartAfter = false) {
		_isActive = false;
		_stopPredicate = predicate;
		_resetAfterStopPredicate = restartAfter;
	}

	//public void WaitUntil
	public Timer() { }
	public Timer(float endTime){
		if(endTime <= 0)
			throw new System.ArgumentException("End time of a time must be greater than zero");
		_endTime = endTime;
	}

	public Timer(float endTime, bool isLooping) : this(endTime) {
		_isLooping = isLooping;
	}

	public void Tick(float dt) {
		if(_stopPredicate != null && _stopPredicate()){
			_isActive = true;
			_stopPredicate = null;
			if(_resetAfterStopPredicate) ResetTimer();
		}

		if(IsActive){
			CurrentTime += dt;
			if(CurrentTime >= EndTime){
				Finished?.Invoke();
				if(IsLooping)
					CurrentTime -= EndTime;
				else{
					SetActive(false);
					CurrentTime = EndTime;
				}
			}
		}
	}

	public void SetActive(bool isActive, bool reset = false){
		if(reset) ResetTimer();
		if(IsActive != isActive && (IsLooping || !HasFinished)){
			_isActive = isActive;
			if(IsActive)
				BecameActive?.Invoke();
			else
				BecameInactive?.Invoke();
		}
	}
}