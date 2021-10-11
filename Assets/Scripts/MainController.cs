using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MainController : MonoBehaviour {
  private List<SystemPanelScript> _systemPanels;
  private Board _board;
  private GridScript _grid;
  private Action _selectedAction;
  private List<SystemState> _systems;

  // Start is called before the first frame update
  void Start() {
    _grid = FindObjectsOfType<GridScript>().First();
    _board = Board.CreateBoard(new Vector2Int(4, 4), new Tuple<BoardContent, Vector2Int>[] {
      new Tuple<BoardContent, Vector2Int>(new BoardContent(){Health = 2, visualInfo = new VisualInfo() { type = UnitType.Cannon}}, new Vector2Int(1,0)),
      new Tuple<BoardContent, Vector2Int>(new BoardContent(){Health = 3, visualInfo = new VisualInfo() { type = UnitType.HoverTank}}, new Vector2Int(2,1)),
      new Tuple<BoardContent, Vector2Int>(new BoardContent(){Health = 4, visualInfo = new VisualInfo() { type = UnitType.HeavyTank}}, new Vector2Int(1,3)),
      new Tuple<BoardContent, Vector2Int>(new BoardContent(){Health = 1, visualInfo = new VisualInfo() { type = UnitType.FastRocket}}, new Vector2Int(3,0)),
    });
    _grid.setBoard(_board);
    _grid.setController(this);
    _systemPanels = FindObjectsOfType<SystemPanelScript>()
        .OrderBy((systemPanel) => systemPanel.gameObject.name)
        .ToList();
    _systems = new List<SystemState>{
        new SystemState(){
            pilot = null,
            system = new MechSystem() {
                systemImageFilename = "2",
            },
            currentAction = new PushMissileAction()
        },
        new SystemState(){
            pilot = null,
            system = new MechSystem() {
                systemImageFilename = "1",
            },
            currentAction = new ShootLaserAction()
        },
    };
    for (int i = 0; i < _systemPanels.Count; ++i) {
      var systemPanel = _systemPanels[i];
      systemPanel.Index = i;
      if (i < _systems.Count) {
        systemPanel.SetSystemState(_systems[i]);
      } else {
        systemPanel.SetSystemState(null);
      }
    }
  }

  private void handleInput() {
    if (Input.GetMouseButtonDown(1)) {
      _selectedAction = null;
    }
  }

  public void TilePressed(Vector2Int tile) {
    if (_selectedAction == null) {
      return;
    }
    var effects = _selectedAction.actionEffects(_board, tile).ToList();
    var oldBoard = _board;
    var (board, changes) = _board.NextBoard(effects);
    _board = board;
    _grid.ShowChanges(oldBoard.GetAllContent().Select(identifier => {
      var change = new GridScript.Change();
      change.startAt = oldBoard.positionOfContent(identifier);
      change.isDestroyed = _board.getContent(identifier) == null;
      if (!change.isDestroyed) {
        change.moveTo = _board.positionOfContent(identifier);
      }
      return change;
    }));
  }

  private void handleHighlights() {
    var currentTile = _grid.MouseOnTile();
    if (currentTile is null) {
      _grid.FreeHightlights();
      return;
    }

    var tile = currentTile.GetValueOrDefault();
    var highlights = _selectedAction is null ? new[]{new HighlightInfo() {
        cell = new Vector2Int(tile.x, tile.y),
        highlight = Highlights.Selection
      }
    } : _selectedAction.actionEffects(_board, tile).Select(effect => new HighlightInfo() {
      cell = effect.position,
      highlight = effect.highlight
    });
    _grid.SetHightlights(highlights);
  }

  // Update is called once per frame
  void Update() {
    handleInput();
    handleHighlights();
  }

  public void ActionSelected(SystemPanelScript panel) {
    _selectedAction = _systems[panel.Index].currentAction;
  }
}
