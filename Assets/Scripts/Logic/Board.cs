using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class BoardContent {
  public int Health;

  public Guid identifier = Guid.NewGuid();

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

  public Vector2Int getSize() {
    return new Vector2Int(xSize, ySize);
  }

  public BoardContent ContentAt(Vector2Int position) {
    return content[position.x, position.y]?.Copy();
  }

  private void putContentAt(Vector2Int position, BoardContent newContent) {
    content[position.x, position.y] = newContent?.Copy();
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

    foreach (var effect in effects) {
      var contentInTile = ContentAt(effect.position);
      if (contentInTile is null) {
        continue;
      }
      contentInTile.Health -= effect.damage;

      var maxValue = Math.Max(effect.move.x, effect.move.y);
      var finalLocation = effect.position;
        var adjustedMove = effect.move * i / maxValue;
        var position = effect.position + adjustedMove;
        if (withinBounds(position) && ContentAt(position) is null) {
          finalLocation = position;
        }
      }
      nextBoard.putContentAt(effect.position, null);
      nextBoard.putContentAt(finalLocation, contentInTile);
    }
    return nextBoard;
  }
}