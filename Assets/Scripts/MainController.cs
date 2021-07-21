using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainController : MonoBehaviour {
  private List<SystemPanelScript> _systemPanels;

  // Start is called before the first frame update
  void Start() {
    _systemPanels = FindObjectsOfType<SystemPanelScript>().OrderBy((systemPanel) => systemPanel.gameObject.name).ToList();
  }

  // Update is called once per frame
  void Update() {

  }
}
