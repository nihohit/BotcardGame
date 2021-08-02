using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

class BoardTests {
  [Test]
  public void TestEmptyBoardCreation() {
    var board = Board.CreateBoard(new Vector2Int(3, 3), Enumerable.Empty<Tuple<BoardContent, Vector2Int>>());

    Assert.That(board.getSize() == new Vector2Int(3, 3));
    for (int i = 0; i < 3; ++i) {
      for (int j = 0; j < 3; ++j) {
        Assert.That(board.ContentAt(new Vector2Int(i, j)) is null);
      }
    }
  }
}
