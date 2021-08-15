using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class HighlightInfo {
  public Highlights highlight;
  public Vector2Int cell;
}

public class GridScript : MonoBehaviour {
  enum UnitType { Cannon, FastRocket, FastLargeTank, HeavyHoverTank, HeavyTank, HoverTank, LargeTank, Plane, Rocket, SmallTank, Tank, WalkingCannon };

  private Tilemap _tilemap;
  private HighlightFactory _highlightFactory;
  private Dictionary<UnitType, GameObject> _unitResources = new Dictionary<UnitType, GameObject>();
  private List<GameObject> _highlights = new List<GameObject>();

  // Start is called before the first frame update
  void Start() {
    _highlightFactory = GetComponent<HighlightFactory>();
    _tilemap = GetComponent<Tilemap>();
    foreach (var value in (UnitType[])Enum.GetValues(typeof(UnitType))) {
      _unitResources[value] = Resources.Load<GameObject>(value.ToString());
    }
    var count = 0;
    for (int z = _tilemap.cellBounds.zMin; z < _tilemap.cellBounds.zMax; ++z) {
      for (int x = _tilemap.cellBounds.xMin; x < _tilemap.cellBounds.xMax; ++x) {
        for (int y = _tilemap.cellBounds.yMin; y < _tilemap.cellBounds.yMax; ++y) {
          if (count >= _unitResources.Count) {
            break;
          }
          if (!_tilemap.HasTile(new Vector3Int(x, y, z))) {
            continue;
          }
          // TODO: why is the +1 necesary?
          var place = _tilemap.CellToWorld(new Vector3Int(x + 1, y + 1, z));
          UnitType foo = (UnitType)count;
          var newUnit = Instantiate(_unitResources[foo]);
          newUnit.transform.position = place;
          newUnit.transform.parent = this.transform;
          newUnit.GetComponent<UnitScript>().Health = 1;
          ++count;
        }
      }
    }
  }

  static readonly Vector3 adjustment = new Vector3(0, -0.15f, 0);
  private Vector3 adjustedWorldCoordinates(Vector3 coords) {
    return coords + adjustment;
  }

  private Vector3Int mouseOnTile() {
    var worldCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    var result = _tilemap.WorldToCell(adjustedWorldCoordinates(worldCoords));
    result.z = _tilemap.cellBounds.zMin;
    return result;
  }

  public Vector3Int? MouseOnTile() {
    var tile = mouseOnTile();
    return _tilemap.HasTile(tile) ? tile : null as Nullable<Vector3Int>;
  }

  private void setHighlight(HighlightInfo info) {
    var selection = _highlightFactory.GetHighlight(info.highlight);
    selection.transform.position = _tilemap.GetCellCenterWorld(new Vector3Int(info.cell.x, info.cell.y, 0));
    _highlights.Add(selection);
  }

  public void FreeHightlights() {
    _highlights.ForEach(highlight => _highlightFactory.ReturnHighlight(highlight));
    _highlights.Clear();
  }

  public void OnMouseDown() {
    Vector3Int cell = mouseOnTile();
    Debug.Log(cell);
  }

  public void SetHightlights(IEnumerable<HighlightInfo> highlights) {
    FreeHightlights();
    foreach (var highlight in highlights) { setHighlight(highlight); }
  }
}
