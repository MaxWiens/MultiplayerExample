using System;
using UnityEngine;
using System.Collections.Generic;
[Serializable]
public abstract class ItemSO : ScriptableObject {
  [SerializeField]
  private ItemTypeSet _itemTypes = null;
  public ISet<ItemTypeSO> ItemTypes => _itemTypes;
  public abstract System.Type HandlerType {get;}
  public abstract bool Effect(object o);
}