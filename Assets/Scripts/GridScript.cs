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
    _unitResources[UnitType.Cannon] = Resources.Load<GameObject>("Cannon");
    _unitResources[UnitType.FastRocket] = Resources.Load<GameObject>("Fast Rocket");
    _unitResources[UnitType.FastLargeTank] = Resources.Load<GameObject>("FastLargeTank");
    _unitResources[UnitType.HeavyHoverTank] = Resources.Load<GameObject>("Heavy Hover Tank");
    _unitResources[UnitType.HeavyTank] = Resources.Load<GameObject>("Heavy Tank");
    _unitResources[UnitType.HoverTank] = Resources.Load<GameObject>("Hover Tank");
    _unitResources[UnitType.LargeTank] = Resources.Load<GameObject>("Large Tank");
    _unitResources[UnitType.Plane] = Resources.Load<GameObject>("Plane");
    _unitResources[UnitType.Rocket] = Resources.Load<GameObject>("Rocket");
    _unitResources[UnitType.SmallTank] = Resources.Load<GameObject>("Small Tank");
    _unitResources[UnitType.Tank] = Resources.Load<GameObject>("Tank");
    _unitResources[UnitType.WalkingCannon] = Resources.Load<GameObject>("Walking Cannon");

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
            Debug.Log($"{count} {foo}");
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
