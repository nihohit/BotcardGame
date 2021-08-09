using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

class BoardTests {
  [Test]
  public void empty_board_creation() {
    var board = Board.CreateBoard(new Vector2Int(3, 3), Enumerable.Empty<Tuple<BoardContent, Vector2Int>>());

    Assert.That(board.getSize() == new Vector2Int(3, 3));
    for (int i = 0; i < 3; ++i) {
      for (int j = 0; j < 3; ++j) {
        Assert.That(board.ContentAt(new Vector2Int(i, j)) is null);
      }
    }
  }

  [Test]
  public void board_throws_when_checking_out_of_bounds() {
    var board = Board.CreateBoard(new Vector2Int(3, 3), Enumerable.Empty<Tuple<BoardContent, Vector2Int>>());

    Assert.Throws<IndexOutOfRangeException>(() => board.ContentAt(new Vector2Int(0, -1)));
    Assert.Throws<IndexOutOfRangeException>(() => board.ContentAt(new Vector2Int(3, 0)));
  }

  [Test]
  public void board_creation_with_content() {
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

  [Test]
  public void board_effect_move() {
    var content1 = new BoardContent() {
      Health = 1
    };
    var content2 = new BoardContent() {
      Health = 2
    };
    var size = new Vector2Int(4, 4);
    var board = Board.CreateBoard(size, new[] {
      Tuple.Create(content1, new Vector2Int(0, 1)),
      Tuple.Create(content2, new Vector2Int(2, 1)) }).NextBoard(new[] { new ActionEffect() {
        position = new Vector2Int(0, 1),
        move = new Vector2Int(0, 2)
      }});

    Assert.That(board.getSize() == size);
    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        if (i == 0 && j == 3) {
          Assert.That(board.ContentAt(new Vector2Int(i, j)).identifier == content1.identifier);
          Assert.That(board.ContentAt(new Vector2Int(i, j)).Health == content1.Health);
        } else if (i == 2 && j == 1) {
          Assert.That(board.ContentAt(new Vector2Int(i, j)).identifier == content2.identifier);
          Assert.That(board.ContentAt(new Vector2Int(i, j)).Health == content2.Health);
        } else {
          Assert.That(board.ContentAt(new Vector2Int(i, j)) is null);
        }
      }
    }
  }

  [Test]
  public void board_effect_damage() {
    var content1 = new BoardContent() {
      Health = 1
    };
    var content2 = new BoardContent() {
      Health = 2
    };
    var size = new Vector2Int(4, 4);
    var board = Board.CreateBoard(size, new[] {
      Tuple.Create(content1, new Vector2Int(0, 1)),
      Tuple.Create(content2, new Vector2Int(2, 1)) }).NextBoard(new[] { new ActionEffect() {
        position = new Vector2Int(2, 1),
        damage = 1
      }});

    Assert.That(board.getSize() == size);
    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        if (i == 0 && j == 1) {
          Assert.That(board.ContentAt(new Vector2Int(i, j)).identifier == content1.identifier);
          Assert.That(board.ContentAt(new Vector2Int(i, j)).Health == content1.Health);
        } else if (i == 2 && j == 1) {
          Assert.That(board.ContentAt(new Vector2Int(i, j)).identifier == content2.identifier);
          Assert.That(board.ContentAt(new Vector2Int(i, j)).Health == 1);
        } else {
          Assert.That(board.ContentAt(new Vector2Int(i, j)) is null);
        }
      }
    }
  }

  [Test]
  public void board_use_multiple_effects() {
    var content1 = new BoardContent() {
      Health = 3
    };
    var content2 = new BoardContent() {
      Health = 2
    };
    var size = new Vector2Int(4, 4);
    var board = Board.CreateBoard(size, new[] {
      Tuple.Create(content1, new Vector2Int(0, 1)),
      Tuple.Create(content2, new Vector2Int(2, 1)) }).NextBoard(new[] { new ActionEffect() {
        position = new Vector2Int(2, 1),
        damage = 1
      },
      new ActionEffect() {
        position = new Vector2Int(0, 1),
        damage = 1,
        move = new Vector2Int(0, 2)
      }});

    Assert.That(board.getSize() == size);
    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        if (i == 0 && j == 3) {
          Assert.That(board.ContentAt(new Vector2Int(i, j)).identifier == content1.identifier);
          Assert.That(board.ContentAt(new Vector2Int(i, j)).Health == 2);
        } else if (i == 2 && j == 1) {
          Assert.That(board.ContentAt(new Vector2Int(i, j)).identifier == content2.identifier);
          Assert.That(board.ContentAt(new Vector2Int(i, j)).Health == 1);
        } else {
          Assert.That(board.ContentAt(new Vector2Int(i, j)) is null);
        }
      }
    }
  }
}