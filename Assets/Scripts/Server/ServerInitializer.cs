using UnityEngine;
public class ServerInitializer : Initializer<ScriptableObject> {
  [SerializeField]
  private ScriptableObject _server;
  private bool _isInitialized = false;

  public override bool IsInitialized {
    get {
      if(_isInitialized) return true;
      if(_server!=null) return (_isInitialized = true);
      return false;
    }
  }
  public override void Init(ScriptableObject server) { 
    if(IsInitialized) throw new AlreadyInitalizedException();
    _server = server;
  }
}