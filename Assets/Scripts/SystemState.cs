public class MechSystem {
  public string systemImageFilename;
}

public class Pilot {
  public string pilotImageFilename;
}

public class Action {
  public string actionImageFilename;
  public bool wasUsed;
}

public class SystemState {
  public Pilot pilot;
  public MechSystem system;
  public Action currentAction;
}
