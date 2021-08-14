using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {
  public Highlights highlight;
}

public class HighlightFactory : MonoBehaviour {
  private static readonly Dictionary<Highlights, string> _highlightToFilename = new Dictionary<Highlights, string> {
  { Highlights.Selection, "Selection" },
  { Highlights.Damage, "damage" },
  { Highlights.MoveOnceDown, "single_arrow_down" },
  { Highlights.MoveOnceLeft, "single_arrow_left" },
  { Highlights.MoveOnceRight, "single_arrow_right" },
  { Highlights.MoveOnceUp, "single_arrow_up" },
  { Highlights.MoveOnceDownRight, "single_arrow_down_right" },
  { Highlights.MoveOnceDownLeft, "single_arrow_down_left" },
  { Highlights.MoveOnceUpRight, "single_arrow_up_right" },
  { Highlights.MoveOnceUpLeft, "single_arrow_up_left" },
  { Highlights.MoveTwiceDownRight, "double_arrow_down_right" },
  { Highlights.MoveTwiceDownLeft, "double_arrow_down_left" },
  { Highlights.MoveTwiceUpRight, "double_arrow_up_right" },
  { Highlights.MoveTwiceUpLeft, "double_arrow_up_left" },
};
  private readonly Dictionary<Highlights, Queue<GameObject>> _highlights = new Dictionary<Highlights, Queue<GameObject>>();

  private void Start() {
    foreach (var value in (Highlights[])Enum.GetValues(typeof(Highlights))) {
      ReturnHighlight(GetHighlight(value));
    }
  }

  public GameObject GetHighlight(Highlights highlight) {
    if (!_highlights.TryGetValue(highlight, out Queue<GameObject> queue)) {
      queue = new Queue<GameObject>();
      _highlights[highlight] = queue;
    }

    if (queue.Count > 0) {
      var found = queue.Dequeue();
      found.SetActive(true);
      return found;
    }

    var obj = Instantiate(Resources.Load<GameObject>($"highlights/{_highlightToFilename[highlight]}"));
    var highlightComponent = obj.AddComponent<Highlight>();
    highlightComponent.highlight = highlight;
    return obj;
  }

  public void ReturnHighlight(GameObject obj) {
    _highlights[obj.GetComponent<Highlight>().highlight].Enqueue(obj);
    obj.SetActive(false);
  }
}
