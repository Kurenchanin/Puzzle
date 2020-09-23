using System;
using UnityEngine;

public class MoveCommand : ICommand
{
  GameObject go;
  float angle;
  
  bool finish;
  bool rotationDone;

  float rotationSpeed = 10;
  public MoveCommand(GameObject go)
  {
    this.go = go;
  }

  public void Execute()
  {    
    if (!finish) 
    { //следим за пальцем пользователя
      go.transform.Rotate(0, angle * 8, 0); //move to constants
    }
    else 
    {
      go.transform.localRotation = Quaternion.Lerp(go.transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
      var angle = Quaternion.Angle(go.transform.localRotation, targetRotation);
      if(angle < 0.5f) {
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