using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainController : MonoBehaviour {
  private List<SystemPanelScript> _systemPanels;

  // Start is called before the first frame update
  void Start() {
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

  }
}
