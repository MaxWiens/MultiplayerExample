using UnityEngine;

[CreateAssetMenu(fileName = "PointCube", menuName = "MultiplayerExample/PointCube", order = 0)]
public class PointCubeSO : ItemSO {
  [SerializeField]
  private int _value = 1;
  public int Value => _value;

  public override System.Type HandlerType => typeof(IScorable);
	public override bool Effect(object o) {
		if(o is IScorable s){
      s.Value += _value;
      return true;
    }
    return false;
	}
}