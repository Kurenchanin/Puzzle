using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelController : MonoBehaviour
{
  [SerializeField]
  List<Tile> tiles;

  [SerializeField]
  Camera camera;

  void Start()
  {

  }


  
  bool mousePressed;
  Vector3 previousMousePosition;
  void Update()
  { 
    if(Input.GetMouseButton(0)) 
    {
      if(mousePressed)
      {
         var mousePos = Input.mousePosition;
         mousePos.z = 35;
        var pos = camera.ScreenToWorldPoint(mousePos);

        //it was pressed on previous frame
        ProcessMove(pos);
        previousMousePosition = pos;
      } 
      else
      {
        SelectTile(Input.mousePosition);
        mousePressed = true;
        
        var mousePos = Input.mousePosition;
        mousePos.z = 35;
        previousMousePosition = camera.ScreenToWorldPoint(mousePos);
      }
    }
    else 
    {
      if(mousePressed) 
      {
        DeselectTile();
        mousePressed = false;
      }
      else
      {
        //do nothing
      }
    }
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
    //Debug.LogError($"Angle = {angle}");

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
