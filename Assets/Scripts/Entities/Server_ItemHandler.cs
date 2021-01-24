using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;

public class Server_ItemHandler : MonoBehaviour {
	[SerializeField]
	private ItemTypeUnityObjectSetMap _itemHandlerMap = null;

	public bool Register(ItemTypeSO itemType, UnityObject handler){
		if(_itemHandlerMap.TryGetValue(itemType, out UnityObjectSet set)){
			return set.Add(handler);
		}else{
			set = new UnityObjectSet();
			set.Add(handler);
			_itemHandlerMap.Add(itemType, set);
			return true;
		}
	}

	public bool Unregister(ItemTypeSO itemType, UnityObject handler){
		if(_itemHandlerMap.TryGetValue(itemType, out UnityObjectSet set)){
			return set.Remove(handler);
		}
		return false;
	}

	public bool HandleItem(ItemSO item) {
		bool handled = false;
		foreach(var itemType in item.ItemTypes){
			if(_itemHandlerMap.TryGetValue(itemType, out UnityObjectSet set)){
				foreach (UnityObject o in set)
					item.Effect(o);
				handled = true;
			}
		}
		return handled;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Server_ItemHandler))]
public class Server_ItemHandler_Editor : Editor {
	private ItemSO _itemToGive = null;
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if(Application.IsPlaying(target)) {
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Give Item", GUILayout.Height(20f)) && _itemToGive != null)
				(target as Server_ItemHandler).HandleItem(_itemToGive as ItemSO);
			_itemToGive = EditorGUILayout.ObjectField(_itemToGive, typeof(ItemSO), false, GUILayout.Height(20f)) as ItemSO;
			GUILayout.EndHorizontal();
		}
	}
}
#endif