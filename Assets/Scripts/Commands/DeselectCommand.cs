using UnityEngine;

public class DeselectCommand : ICommand
{
  const int targetY = 0;
  const int speed = -10;

  GameObject go;

  public DeselectCommand(GameObject go)
  {
    this.go = go;
  }

  public void Execute()
  {
    var pos = go.transform.localPosition;
    pos.y += speed * Time.deltaTime;
    if(pos.y < targetY)
      pos.y = targetY;
    go.transform.localPosition = pos;
  }

  public bool IsExecuted()
  {
    var localPos = go.transform.localPosition;
    return localPos.y == targetY;
  }
}