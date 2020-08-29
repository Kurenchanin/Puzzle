using UnityEngine;
using System.Collections.Generic;
using System;

public interface ICommand
{
  void Execute();   

  bool IsExecuted();
}

public class SelectCommand : ICommand
{
  const int targetY = 2;
  const int speed = 10;

  GameObject go;

  public SelectCommand(GameObject go) 
  {
    this.go = go;
  }

  public void Execute() 
  {
    var pos = go.transform.localPosition;
    pos.y += speed * Time.deltaTime;
    if(pos.y > targetY)
      pos.y = targetY;
    go.transform.localPosition = pos;
  }

  public bool IsExecuted()
  {
    var localPos = go.transform.localPosition;
    return localPos.y == targetY;
  }
}

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

public class MoveCommand : ICommand
{
  GameObject go;
  float angle;
  
  bool finish;
  bool rotationDone;

  float rotationSpeed = 30;
  public MoveCommand(GameObject go)
  {
    this.go = go;
  }

  public void Execute()
  {    
    if (!finish) 
    { //следим за пальцем пользователя
      go.transform.Rotate(0, angle * 4, 0); //move to constants
    }
    else 
    {
       go.transform.localRotation = Quaternion.Lerp(go.transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);

      if(Quaternion.Angle(go.transform.localRotation, targetRotation) < 0.005f) {
        go.transform.localRotation = targetRotation;
        rotationDone = true;
      }
    }

    //Debug.LogError($"name = {go.name}, finish = {finish}, rotationDone = {rotationDone}, lr = {go.transform.localRotation}, tr = {targetRotation}");
  }

  public bool IsExecuted()
  {
    if (!finish)
      return false;
  
    return rotationDone;
  }

  public void SetAngle(float angle)
  {
    this.angle = angle;
  }

  Quaternion targetRotation;
  public void Finish()
  {
    finish = true;

    var eulers = go.transform.eulerAngles;
    var yAngle = eulers.y;

    var targetAngle = (float)Math.Round(yAngle / 90f) * 90f;

    targetRotation = Quaternion.Euler(0, targetAngle, 0);
  }
}

public class Tile : MonoBehaviour 
{
  LinkedList<ICommand> commands = new LinkedList<ICommand>();

  public void Select() 
  {    
    if(commands.Count > 0 && commands.Last.Value is DeselectCommand)
    {
      commands.RemoveLast();
    }
    commands.AddLast(new SelectCommand(gameObject));
  }

  public void Deselect() 
  {
    if(commands.Count > 0 && commands.Last.Value is SelectCommand) 
    {
      commands.RemoveLast();
    }

    if(commands.Count > 0)
    {
      var moveCommand = (MoveCommand)FindMoveCommand();
      if(moveCommand != null)
        moveCommand.Finish();
    }
    commands.AddLast(new DeselectCommand(gameObject));
  }

  public void ProcessMove(float angle)
  {
    var moveCommand = (MoveCommand)FindMoveCommand();
    if(moveCommand == null) 
    {
      moveCommand = new MoveCommand(gameObject);
      commands.AddLast(moveCommand);
    }

    moveCommand.SetAngle(angle);
  }

  public ICommand FindMoveCommand() 
  {
    if(commands.Count == 0)
      return null;

    var iterator = commands.GetEnumerator();
    while(iterator.MoveNext())
    {
      var current = iterator.Current;
      if (current is MoveCommand)
        return current;
    }

    return null;
  }

  void Update() 
  {
    if(commands.Count == 0)
      return;

    var command = commands.First.Value;
    if(command.IsExecuted()) 
    {
      commands.RemoveFirst();
      return;
    }

    command.Execute();
    if(command.IsExecuted()) 
    {
      commands.RemoveFirst();
      return;
    }
  }
}

