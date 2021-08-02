using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class BoardContent {
  public int Health;
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

  public Vector2Int getSize() {
    return new Vector2Int(content.GetLength(0), content.GetLength(1));
  }

  public BoardContent ContentAt(Vector2Int position) {
    return content[position.x, position.y];
  }

  private void putContentAt(Vector2Int position, BoardContent newContent) {
    content[position.x, position.y] = newContent;
  }

  private BoardContent[,] copyBoard() {
    var size = getSize();
    var newContent = new BoardContent[size.x, size.y];
    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        newContent[i, j] = content[i, j].Copy();
      }
    }
    return newContent;
  }

  public Board NextBoard(IEnumerable<ActionEffect> effects) {
    var nextBoard = new Board();
    nextBoard.content = copyBoard();
    var size = getSize();

    foreach (var effect in effects) {
      var contentInTile = ContentAt(effect.position);
      if (contentInTile is null) {
        continue;
      }
      contentInTile.Health -= effect.damage;

      var maxValue = Math.Max(effect.move.x, effect.move.y);
      var finalLocation = effect.position;
      for (int i = 1; i < maxValue; ++i) {
        var adjustedMove = effect.move * i / maxValue;
        var position = effect.position + adjustedMove;
        if (position.x >= 0 &&
            position.y >= 0 &&
            position.x < size.x &&
            position.y < size.y &&
            ContentAt(position) is null) {
          finalLocation = position;
        }
      }
      nextBoard.putContentAt(effect.position, null);
      nextBoard.putContentAt(finalLocation, contentInTile);
    }
    return nextBoard;
  }
}