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

  [Test]
  public void TestThrowsWhenCheckingOutOfBoard() {
    var board = Board.CreateBoard(new Vector2Int(3, 3), Enumerable.Empty<Tuple<BoardContent, Vector2Int>>());

    Assert.Throws<IndexOutOfRangeException>(() => board.ContentAt(new Vector2Int(0, -1)));
    Assert.Throws<IndexOutOfRangeException>(() => board.ContentAt(new Vector2Int(3, 0)));
  }

  [Test]
  public void TestBoardCreationWithContent() {
    var content1 = new BoardContent();
    var content2 = new BoardContent();
    var board = Board.CreateBoard(new Vector2Int(3, 3), new[] {
      Tuple.Create(content1, new Vector2Int(0, 1)),
      Tuple.Create(content2, new Vector2Int(2, 1)) });

    Assert.That(board.getSize() == new Vector2Int(3, 3));
    for (int i = 0; i < 3; ++i) {
      for (int j = 0; j < 3; ++j) {
        if (i == 0 && j == 1) {
          Assert.That(board.ContentAt(new Vector2Int(i, j)).identifier == content1.identifier);
        } else if (i == 2 && j == 1) {
          Assert.That(board.ContentAt(new Vector2Int(i, j)).identifier == content2.identifier);
        } else {
          Assert.That(board.ContentAt(new Vector2Int(i, j)) is null);
        }
      }
    }
  }
}
