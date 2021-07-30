using UnityEngine;
using System.Collections.Generic;

public class MechSystem {
  public string systemImageFilename;
}

public class Pilot {
  public string pilotImageFilename;
}

public class ActionEffect {
  public Vector2Int position;
  public int damage;
  public Vector2Int newPosition;
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
