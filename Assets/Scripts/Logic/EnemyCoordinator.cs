
public enum EnemySystemTrajectory { Direct, Ballistic, Piercing }

public class EnemyWeaponSystem {
  public EnemySystemTrajectory trajectory;
  public int damage;
  public int minRange;
  public int maxRange;
}

public class Enemy : BoardContent {
  public int speed;
  public List<EnemyWeaponSystem> systems;
}

public class EnemyPlanningResults {
  public Dictionary<Guid, List<Vector2Int>> movements;
  public Dictionary<Guid, EnemyWeaponSystem> intentions;

  public Board newBoard;
}

public class EnemyCoordinator {
  public EnemyPlanningResults planActions(Board board) {
    return new EnemyPlanningResults {
      movements = new Dictionary<Guid, List<Vector2Int>>(),
      intentions = new Dictionary<Guid, EnemyWeaponSystem>(),
      newBoard = board
    };
  }
}