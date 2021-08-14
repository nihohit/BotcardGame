using UnityEngine;
using System.Collections.Generic;

public class MechSystem {
  public string systemImageFilename;
}

public class Pilot {
  public string pilotImageFilename;
}

public enum Highlights {
  Selection,
  Damage,
  MoveOnceDown,
  MoveOnceLeft,
  MoveOnceRight,
  MoveOnceUp,
  MoveOnceDownRight,
  MoveOnceDownLeft,
  MoveOnceUpRight,
  MoveOnceUpLeft,
  MoveTwiceDownRight,
  MoveTwiceDownLeft,
  MoveTwiceUpRight,
  MoveTwiceUpLeft
}

public class ActionEffect {
  public Vector2Int position;
  public int damage;
  public Vector2Int move;

  public Highlights highlight;
}

public abstract class Action {
  public bool wasUsed;

  public abstract string GetActionImageFilename();

  public abstract bool canAct(Board board, Vector2Int position);

  public abstract List<ActionEffect> actionEffects(Board board, Vector2Int position);
}

public class PushMissileAction : Action {
  public override bool canAct(Board board, Vector2Int position) {
    return true;
  }

  public override List<ActionEffect> actionEffects(Board board, Vector2Int position) {
    List<ActionEffect> effects = new List<ActionEffect>();

    var boardSize = board.getSize();

    effects.Add(new ActionEffect() {
      damage = 1,
      position = position
    });
    if (position.x >= 0 && position.y >= 0) {
      effects.Add(new ActionEffect() {
        position = position + new Vector2Int(-1, -1),
        move = new Vector2Int(-2, -2)
      });
    }
    if (position.x >= 0 && position.y < boardSize.y) {
      effects.Add(new ActionEffect() {
        position = position + new Vector2Int(-1, 1),
        move = new Vector2Int(-2, 2)
      });
    }
    if (position.x < boardSize.x && position.y >= 0) {
      effects.Add(new ActionEffect() {
        position = position + new Vector2Int(1, -1),
        move = new Vector2Int(2, -2)
      });
    }
    if (position.x < boardSize.x && position.y < boardSize.y) {
      effects.Add(new ActionEffect() {
        position = position + new Vector2Int(1, 1),
        move = new Vector2Int(2, 2)
      });
    }

    return effects;
  }

  public override string GetActionImageFilename() {
    return "3";
  }
}

public class SystemState {
  public Pilot pilot;
  public MechSystem system;
  public Action currentAction;
}
