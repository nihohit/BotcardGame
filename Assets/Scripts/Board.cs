using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class BoardContent {
  public BoardContent Copy() {
    var clone = MemberwiseClone() as BoardContent;
    if (clone is null) {
      throw new Exception("clone should not be null");
    }
    return clone;
  }
}

public class Enemy : BoardContent {
  public int Health;
}

public class Board {
  private BoardContent[][] content;

  public BoardContent ContentAt(Vector2Int position) {
    return content[position.x][position.y];
  }

  public Board NextBoard(ActionEffect effect) {
    var nextBoard = new Board();
    nextBoard.content = content
        .Select(subarray => subarray
            .Select(content => content is null ? null : content.Copy())
            .ToArray())
        .ToArray();
    return nextBoard;
  }
}