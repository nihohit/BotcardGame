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
  public class Change {
    public Vector2Int startAt;
    public Vector2Int moveTo;

    public bool isDestroyed;

    public bool isDamaged;

    public bool moved() {
      return startAt != moveTo;
    }

    public bool somethingChanged() {
      return isDestroyed || isDamaged || moved();
    }
  }


  private Tilemap _tilemap;
  private HighlightFactory _highlightFactory;
  private Dictionary<UnitType, GameObject> _unitResources = new Dictionary<UnitType, GameObject>();
  private List<GameObject> _highlights = new List<GameObject>();
  private Dictionary<Vector2Int, GameObject> _unitLocations = new Dictionary<Vector2Int, GameObject>();

  private MainController _controller;

  // Start is called before the first frame update
  void Awake() {
    _highlightFactory = GetComponent<HighlightFactory>();
    _tilemap = GetComponent<Tilemap>();
    foreach (var value in (UnitType[])Enum.GetValues(typeof(UnitType))) {
      _unitResources[value] = Resources.Load<GameObject>(value.ToString());
    }
  }

  public void setController(MainController controller) {
    _controller = controller;
  }

  private Vector3 cellToWorld(Vector2Int tile) {
    return _tilemap.CellToWorld(adjustBoardCell(tile + new Vector2Int(1, 1)));
  }

  public void setBoard(Board board) {
    foreach (var contentIdentifier in board.GetAllContent()) {
      var location = board.positionOfContent(contentIdentifier);
      var content = board.getContent(contentIdentifier);
      var place = cellToWorld(location);
      var newUnit = Instantiate(_unitResources[content.visualInfo.type]);
      newUnit.transform.position = place;
      newUnit.transform.parent = this.transform;
      _unitLocations[location] = newUnit;
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

  public Vector2Int? MouseOnTile() {
    var tile = mouseOnTile();
    return _tilemap.HasTile(tile) ? new Vector2Int(tile.x, tile.y) : null as Nullable<Vector2Int>;
  }

  private Vector3Int adjustBoardCell(Vector2Int cell) {
    return new Vector3Int(cell.x, cell.y, 0);
  }

  private void setHighlight(HighlightInfo info) {
    var selection = _highlightFactory.GetHighlight(info.highlight);
    selection.transform.position = _tilemap.GetCellCenterWorld(adjustBoardCell(info.cell));
    _highlights.Add(selection);
  }

  public void FreeHightlights() {
    _highlights.ForEach(highlight => _highlightFactory.ReturnHighlight(highlight));
    _highlights.Clear();
  }

  public void OnMouseDown() {
    var cell = MouseOnTile();
    if (cell.HasValue) {
      _controller.TilePressed(cell.Value);
    }
  }

  public void SetHightlights(IEnumerable<HighlightInfo> highlights) {
    FreeHightlights();
    foreach (var highlight in highlights) { setHighlight(highlight); }
  }

  public void ShowChanges(IEnumerable<Change> changes) {
    var newPositions = new Dictionary<Vector2Int, GameObject>(_unitLocations);
    foreach (var change in changes) {
      var content = _unitLocations[change.startAt];
      var finalPosition = cellToWorld(change.moveTo);
      System.Action nonMoveAnimation = () => {
        if (change.isDamaged) {

        } else if (change.isDestroyed) {
          var explosion = _highlightFactory.GetHighlight(Highlights.Explosion);
          explosion.transform.position = finalPosition;
          explosion.transform.localScale = Vector3.zero;
          StartCoroutine(explosion.ScaleOverSeconds(new Vector3(0.2f, 0.2f, 0.2f), 0.7f, () => {
            _highlightFactory.ReturnHighlight(explosion);
            GameObject.Destroy(content);
          }));
        }
      };
      if (change.moved()) {
        StartCoroutine(content.MoveOverSpeed(finalPosition, 0.9f, nonMoveAnimation));
      } else {
        nonMoveAnimation();
      }
      if (!change.isDestroyed) {
        newPositions[change.moveTo] = content;
      }
    }
    _unitLocations = newPositions;
  }
}
