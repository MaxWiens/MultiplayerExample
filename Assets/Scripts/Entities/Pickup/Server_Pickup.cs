using System;
using UnityEngine;
public class Server_Pickup : MonoBehaviour, IPickupable {
  [SerializeField, NotNull]
  private ItemSO _item = null;
  public ItemSO Item => _item;

  public virtual ItemSO PickupItem() {
    ItemSO p = _item;
    return p;
  }
}