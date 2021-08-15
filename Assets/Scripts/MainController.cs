using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MainController : MonoBehaviour {
  private List<SystemPanelScript> _systemPanels;
  private Board _board;
  private GridScript _grid;

  // Start is called before the first frame update
  void Start() {
    _grid = FindObjectsOfType<GridScript>().First();
    _board = Board.CreateBoard(new Vector2Int(4, 4), new Tuple<BoardContent, Vector2Int>[] { });
    _systemPanels = FindObjectsOfType<SystemPanelScript>()
        .OrderBy((systemPanel) => systemPanel.gameObject.name)
        .ToList();
    var systems = new List<SystemState>{
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
            currentAction = null
        },
    };
    for (int i = 0; i < _systemPanels.Count; ++i) {
      var systemPanel = _systemPanels[i];
      if (i < systems.Count) {
        systemPanel.SetSystemState(systems[i]);
      } else {
        systemPanel.SetSystemState(null);
      }
    }
  }

  // Update is called once per frame
  void Update() {
    var currentTile = _grid.MouseOnTile();
    if (currentTile is null) {
      _grid.FreeHightlights();
    } else {
      var tile = currentTile.GetValueOrDefault();
      _grid.SetHightlights(new[]{new HighlightInfo() {
        cell = new Vector2Int(tile.x, tile.y),
        highlight = Highlights.Selection
      }
    });
    }
  }
}
