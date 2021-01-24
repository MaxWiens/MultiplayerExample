using UnityEngine;
using System;
[CreateAssetMenu(fileName = "NewScriptableObjectFactory", menuName = "MultiplayerExample/Factory/ScriptableObjectFactory", order = 0)]
public class ScriptableObjectFactory : FactorySO<ScriptableObject>{
	[SerializeField, NotNull]
	private ScriptableObject _object = null;

	public override ScriptableObject Create(){
		return CreateInstance(_object.GetType());
	}
}