using UnityEngine;
using UnityObject = UnityEngine.Object;
using System;
using System.Collections.Generic;

[Serializable]
public class ItemTypeSet : SerializableHashSet<ItemTypeSO>{}
[Serializable]
public class UnityObjectSet : SerializableHashSet<UnityObject>{}
[Serializable]
public class ItemTypeUnityObjectSetMap : SerializableDictionary<ItemTypeSO,UnityObjectSet>{}