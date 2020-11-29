using System;
using System.Collections.Generic;

namespace Network {

	public static class ThreadManager {
		private static readonly List<Action> _mainThreadBuffer = new List<Action>();
		private static readonly List<Action> _mainThreadActions = new List<Action>();

		private static bool _actionInBuffer = false;

		public static void Update() {
			if(_actionInBuffer){
				_mainThreadActions.Clear();
				lock(_mainThreadBuffer){
					_mainThreadActions.AddRange(_mainThreadBuffer);
					_mainThreadBuffer.Clear();
				}
				foreach(Action a in _mainThreadActions) a();
			}
		}

		public static void ExecuteOnMainThread(Action action){
			if(action != null){
				lock(_mainThreadBuffer){
					_mainThreadBuffer.Add(action);
					_actionInBuffer = true;
				}
			}
		}
	}
}
