using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour {
  public int Health { get; set; }

  // Update is called once per frame
  void Update() {
    if (Health <= 0) {
      Destroy(gameObject);
    }
  }
}
