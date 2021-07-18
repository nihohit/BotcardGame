using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridScript : MonoBehaviour {
  enum UnitType { Cannon, FastRocket, FastLargeTank, HeavyHoverTank, HeavyTank, HoverTank, LargeTank, Plane, Rocket, SmallTank, Tank, WalkingCannon };

  private Tilemap _tilemap;
  Dictionary<UnitType, GameObject> _unitResources = new Dictionary<UnitType, GameObject>();

  // Start is called before the first frame update
  void Start() {
    _tilemap = GetComponent<Tilemap>();
    foreach (var value in (UnitType[])Enum.GetValues(typeof(UnitType))) {
      _unitResources[value] = Resources.Load<GameObject>(value.ToString());
    }

    Debug.Log(_tilemap.cellBounds);
    var count = 0;
    for (int z = _tilemap.cellBounds.zMin; z < _tilemap.cellBounds.zMax; ++z) {
      for (int x = _tilemap.cellBounds.xMin; x < _tilemap.cellBounds.xMax; ++x) {
        for (int y = _tilemap.cellBounds.yMin; y < _tilemap.cellBounds.yMax; ++y) {
          if (!_tilemap.HasTile(new Vector3Int(x, y, z))) {
            continue;
          }
          // TODO: why is the +1 necesary?
          var place = _tilemap.CellToWorld(new Vector3Int(x + 1, y + 1, z));
          if (count < _unitResources.Count) {
            UnitType foo = (UnitType)count;
            var newUnit = Instantiate(_unitResources[foo]);
            newUnit.transform.position = place;
            ++count;
          }
        }
      }
    }
  }

  // Update is called once per frame
  void Update() {

  }

  public void OnMouseDown() {
    var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3Int cell = _tilemap.WorldToCell(position);
    Debug.Log(cell);
  }
}
