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
  private BoardContent[][] content;

  public BoardContent ContentAt(Vector2Int position) {
    return content[position.x][position.y];
  }

  private void putContentAt(Vector2Int position, BoardContent newContent) {
    content[position.x][position.y] = newContent;
  }


  public Board NextBoard(IEnumerable<ActionEffect> effects) {
    var nextBoard = new Board();
    nextBoard.content = content
        .Select(subarray => subarray
            .Select(content => content is null ? null : content.Copy())
            .ToArray())
        .ToArray();

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
        if (ContentAt(position) is null) {
          finalLocation = position;
        }
      }
      nextBoard.putContentAt(effect.position, null);
      nextBoard.putContentAt(finalLocation, contentInTile);

    }
    return nextBoard;
  }
}