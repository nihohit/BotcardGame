using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum UnitType { Cannon, FastRocket, FastLargeTank, HeavyHoverTank, HeavyTank, HoverTank, LargeTank, Plane, Rocket, SmallTank, Tank, WalkingCannon };

public class VisualInfo {
  public UnitType type;
}

public class BoardContent {
  public int Health = 1;

  public Guid identifier = Guid.NewGuid();

  public VisualInfo visualInfo;

  public BoardContent Copy() {
    var clone = MemberwiseClone() as BoardContent;
    if (clone is null) {
      throw new Exception("clone should not be null");
    }
    return clone;
  }
}

public class Board {
  private BoardContent[,] content;
  private Dictionary<Guid, Vector2Int> positions = new Dictionary<Guid, Vector2Int>();

  private int xSize {
    get {
      return content.GetLength(0);
    }
  }

  private int ySize {
    get {
      return content.GetLength(1);
    }
  }

  public Vector2Int positionOfContent(Guid identifier) {
    return positions[identifier];
  }

  public BoardContent getContent(Guid identifier) {
    if (!positions.TryGetValue(identifier, out Vector2Int position)) {
      return null;
    }
    return ContentAt(position);
  }

  public Vector2Int getSize() {
    return new Vector2Int(xSize, ySize);
  }

  public BoardContent ContentAt(Vector2Int position) {
    return content[position.x, position.y]?.Copy();
  }

  public IEnumerable<Guid> GetAllContent() {
    return positions.Keys;
  }

  private void putContentAt(Vector2Int position, BoardContent newContent) {
    if (newContent is null) {
      content[position.x, position.y] = null;
      return;
    }
    if (newContent.Health <= 0) {
      content[position.x, position.y] = null;
      positions.Remove(newContent.identifier);
    } else {
      positions[newContent.identifier] = position;
      content[position.x, position.y] = newContent?.Copy();
    }
  }

  private BoardContent[,] copyBoard() {
    var newContent = new BoardContent[xSize, ySize];
    for (int i = 0; i < xSize; ++i) {
      for (int j = 0; j < ySize; ++j) {
        newContent[i, j] = content[i, j]?.Copy();
      }
    }
    return newContent;
  }

  public static Board CreateBoard(Vector2Int size, IEnumerable<Tuple<BoardContent, Vector2Int>> insertedContent) {
    var nextBoard = new Board();
    nextBoard.content = new BoardContent[size.x, size.y];

    foreach (var pair in insertedContent) {
      var position = pair.Item2;
      var content = pair.Item1;
      Debug.Assert(nextBoard.withinBounds(position));
      Debug.Assert(nextBoard.ContentAt(position) is null);
      nextBoard.putContentAt(position, content);
    }

    return nextBoard;
  }

  private bool withinBounds(Vector2Int position) {
    return position.x >= 0 &&
          position.y >= 0 &&
          position.x < xSize &&
          position.y < ySize;
  }

  public Board NextBoard(IEnumerable<ActionEffect> effects) {
    var nextBoard = new Board();
    nextBoard.content = copyBoard();
    nextBoard.positions = positions.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);

    foreach (var effect in effects) {
      var contentInTile = ContentAt(effect.position);
      if (contentInTile is null) {
        continue;
      }
      contentInTile.Health -= effect.damage;

      var maxValue = Math.Max(Math.Abs(effect.move.x), Math.Abs(effect.move.y));
      var finalLocation = effect.position;
      for (int i = 1; i <= maxValue; ++i) {
        var adjustedMove = effect.move * i / maxValue;
        var position = effect.position + adjustedMove;
        if (!withinBounds(position)) {
          break;
        } else if (ContentAt(position) is null) {
          finalLocation = position;
        } else {
          var collided = nextBoard.ContentAt(position);
          collided.Health -= 1;
          nextBoard.putContentAt(position, collided);
          contentInTile.Health -= 1;
          break;
        }
      }
      nextBoard.putContentAt(effect.position, null);
      nextBoard.putContentAt(finalLocation, contentInTile);
    }
    return nextBoard;
  }
}