using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
  [SerializeField]
  Camera camera;

  [SerializeField]
  float zPos = 35; //TODO: get from camera?
  
  bool mousePressed;
  Vector3 previousMousePosition;

  float pressedTimestamp;

  [SerializeField]
  float tapTolerance = .5f;

  void Update()
  {
    HandleMousePressed();
    HandleMouseReleased();
  }

  void HandleMousePressed()
  {
    if(!Input.GetMouseButton(0))
      return;

    if(!mousePressed)
    {
      SelectTile(Input.mousePosition);
      mousePressed = true;

      previousMousePosition = GetWorldMousePos();
      return;
    }
    
    var pos = GetWorldMousePos();
    //it was pressed on previous frame
    ProcessMove(pos);
    previousMousePosition = pos;
  }

  Vector3 GetWorldMousePos()
  {
    var mousePos = Input.mousePosition;
    mousePos.z = zPos;
    return camera.ScreenToWorldPoint(mousePos);
  }

  void HandleMouseReleased()
  {
    if(Input.GetMouseButton(0) || !mousePressed)
      return;

    DeselectTile();
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
