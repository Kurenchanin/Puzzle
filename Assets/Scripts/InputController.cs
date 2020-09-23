using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
  [SerializeField]
  Camera camera;

  [SerializeField]
  float tapTolerance = .2f;

  float cameraZ;
  bool mousePressed;
  Vector3 previousMousePosition;
  float pressedTimestamp;
  bool holdFired;

  void Start()
  {
    cameraZ = camera.gameObject.transform.position.z;
  }

  void Update()
  {
    HandleMousePressed();
    HandleMouseReleased();
  }

  void FireClickEvent() 
  {    
    Debug.LogError("Click event");
  }

  void FireHoldEvent()
  {
    SelectTile(Input.mousePosition);
  }

  void FireMoveEvent()
  {
    var pos = GetWorldMousePos();    
    ProcessMove(pos);
    previousMousePosition = pos;
  }

  void FireReleaseEvent()
  {
    DeselectTile();
  }

  void HandleMousePressed()
  {
    if(!Input.GetMouseButton(0))
      return;

    if(!mousePressed)
    {
      if(tapTolerance > 0)
      {
        mousePressed = true;
        previousMousePosition = GetWorldMousePos();
        pressedTimestamp = Time.time;
        return;
      }

      if(!holdFired)
      {
        FireHoldEvent();
        holdFired = true;
      }
      
      mousePressed = true;
      previousMousePosition = GetWorldMousePos();
      return;
    }

    
    if(tapTolerance > 0)
    {
      var diff = Time.time - pressedTimestamp;
      if (diff > tapTolerance)
      {
        if(!holdFired) 
        {
          FireHoldEvent();
          holdFired = true;
        }
        else
          FireMoveEvent();
      }
      return;
    }
    
    if(holdFired) 
    {
      FireMoveEvent();
    }
  }

  Vector3 GetWorldMousePos()
  {
    var mousePos = Input.mousePosition;
    mousePos.z = cameraZ;
    return camera.ScreenToWorldPoint(mousePos);
  }

  void HandleMouseReleased()
  {
    if(Input.GetMouseButton(0) || !mousePressed)
      return;

    if(tapTolerance > 0)
    {
      var diff = Time.time - pressedTimestamp;
      if (diff <= tapTolerance)
      {
        FireClickEvent();
      }      
    }      

    if(holdFired) 
    {
      FireReleaseEvent();
      holdFired = false;
    }
    
    mousePressed = false;    
  }

  Tile selectedTile;
  Ray ray;
  RaycastHit[] results = new RaycastHit[3];
  void SelectTile(Vector3 position) 
  {    
    ray = camera.ScreenPointToRay(position);
    var count = Physics.RaycastNonAlloc(ray, results);

    if(count > 0) 
    {
      count = Math.Min(count, results.Length);
      for(var i = 0; i < count; i++) 
      {
        var hit = results[i];
        if(hit.collider == null)
          continue;

        var go = hit.collider.gameObject;
        var tile = go.GetComponent<Tile>();
        if (tile == null)
          continue;

        if(selectedTile != null)
        {
          selectedTile.Deselect();
        }

        selectedTile = tile;
        selectedTile.Select();
      }
    }
  }

  void ProcessMove(Vector3 mousePosition) 
  {
    if(selectedTile == null)
      return;

    var tilePosition = selectedTile.gameObject.transform.position;
    var a = previousMousePosition - tilePosition;
    var b = mousePosition - tilePosition;
    var angle = Vector3.SignedAngle(a, b, Vector3.up);

    selectedTile.ProcessMove(angle);
  }

  void DeselectTile() 
  {
    if(selectedTile == null)
      return;

    selectedTile.Deselect();
    selectedTile = null;
  }
}
