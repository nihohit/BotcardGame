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
  MoveTwiceDown,
  MoveTwiceLeft,
  MoveTwiceRight,
  MoveTwiceUp,
  Explosion
}

public class ActionEffect {
  public Vector2Int position;
  public int damage;
  public Vector2Int move;

  public Highlights highlight;

  public Vector2Int NextPosition() {
    return position + move;
  }
}

public abstract class Action {
  public bool wasUsed;

  public abstract string GetActionImageFilename();

  public abstract bool canAct(Board board, Vector2Int position);

  public abstract IEnumerable<ActionEffect> actionEffects(Board board, Vector2Int position);
}

public class PushMissileAction : Action {
  public override bool canAct(Board board, Vector2Int position) {
    return true;
  }

  public override IEnumerable<ActionEffect> actionEffects(Board board, Vector2Int position) {
    List<ActionEffect> effects = new List<ActionEffect>();

    var boardSize = board.getSize();

    effects.Add(new ActionEffect() {
      damage = 1,
      position = position,
      highlight = Highlights.Damage
    });
    if (position.y > 0) {
      effects.Add(new ActionEffect() {
        position = position + new Vector2Int(0, -1),
        move = new Vector2Int(0, -2),
        highlight = Highlights.MoveTwiceRight
      });
    }
    if (position.x > 0) {
      effects.Add(new ActionEffect() {
        position = position + new Vector2Int(-1, 0),
        move = new Vector2Int(-2, 0),
        highlight = Highlights.MoveTwiceDown
      });
    }
    if (position.x < boardSize.x - 1) {
      effects.Add(new ActionEffect() {
        position = position + new Vector2Int(1, 0),
        move = new Vector2Int(2, 0),
        highlight = Highlights.MoveTwiceUp
      });
    }
    if (position.y < boardSize.y - 1) {
      effects.Add(new ActionEffect() {
        position = position + new Vector2Int(0, 1),
        move = new Vector2Int(0, 2),
        highlight = Highlights.MoveTwiceLeft
      });
    }

    return effects;
  }

  public override string GetActionImageFilename() {
    return "3";
  }
}

public class ShootLaserAction : Action {
  public override bool canAct(Board board, Vector2Int position) {
    return board.ContentAt(position) != null;
  }

  public override IEnumerable<ActionEffect> actionEffects(Board board, Vector2Int position) {
    return new[]{new ActionEffect() {
        damage = 4,
        position = position,
        highlight = Highlights.Damage
      }};
  }

  public override string GetActionImageFilename() {
    return "2";
  }
}

public class SystemState {
  public Pilot pilot;
  public MechSystem system;
  public Action currentAction;
}
