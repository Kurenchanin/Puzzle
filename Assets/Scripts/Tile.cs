using UnityEngine;
using System.Collections.Generic;

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

