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
    CollectionAssert.AreEqual(new[] { content1.identifier, content2.identifier }, board.GetAllContent());
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
    CollectionAssert.AreEqual(new[] { content1.identifier, content2.identifier }, board.GetAllContent());
    Assert.That(board.positionOfContent(content1.identifier) == new Vector2Int(0, 3));
    Assert.That(board.positionOfContent(content2.identifier) == new Vector2Int(2, 1));

    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        var position = new Vector2Int(i, j);
        var content = board.ContentAt(position);
        if (position == board.positionOfContent(content1.identifier)) {
          Assert.That(content.identifier == content1.identifier);
          Assert.That(content.Health == content1.Health);
        } else if (position == board.positionOfContent(content2.identifier)) {
          Assert.That(content.identifier == content2.identifier);
          Assert.That(content.Health == content2.Health);
        } else {
          Assert.That(content is null);
        }
      }
    }
  }

  [Test]
  public void board_effects_dont_move_off_grid() {
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
        move = new Vector2Int(-1, 0)
      }});

    Assert.That(board.getSize() == size);
    CollectionAssert.AreEqual(new[] { content1.identifier, content2.identifier }, board.GetAllContent());
    Assert.That(board.positionOfContent(content1.identifier) == new Vector2Int(0, 1));
    Assert.That(board.positionOfContent(content2.identifier) == new Vector2Int(2, 1));

    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        var position = new Vector2Int(i, j);
        var content = board.ContentAt(position);
        if (position == board.positionOfContent(content1.identifier)) {
          Assert.That(content.identifier == content1.identifier);
          Assert.That(content.Health == content1.Health);
        } else if (position == board.positionOfContent(content2.identifier)) {
          Assert.That(content.identifier == content2.identifier);
          Assert.That(content.Health == content2.Health);
        } else {
          Assert.That(content is null);
        }
      }
    }
  }

  [Test]
  public void board_effects_move_damge_and_stop_on_collision() {
    var content1 = new BoardContent() {
      Health = 2
    };
    var content2 = new BoardContent() {
      Health = 2
    };
    var size = new Vector2Int(4, 4);
    var board = Board.CreateBoard(size, new[] {
      Tuple.Create(content1, new Vector2Int(0, 1)),
      Tuple.Create(content2, new Vector2Int(2, 1)) }).NextBoard(new[] { new ActionEffect() {
        position = new Vector2Int(0, 1),
        move = new Vector2Int(2, 0)
      }});

    Assert.That(board.getSize() == size);
    CollectionAssert.AreEqual(new[] { content1.identifier, content2.identifier }, board.GetAllContent());
    Assert.That(board.positionOfContent(content1.identifier) == new Vector2Int(1, 1));
    Assert.That(board.positionOfContent(content2.identifier) == new Vector2Int(2, 1));

    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        var position = new Vector2Int(i, j);
        var content = board.ContentAt(position);
        if (position == board.positionOfContent(content1.identifier)) {
          Assert.That(content.identifier == content1.identifier);
          Assert.That(content.Health == content1.Health - 1);
        } else if (position == board.positionOfContent(content2.identifier)) {
          Assert.That(content.identifier == content2.identifier);
          Assert.That(content.Health == content2.Health - 1);
        } else {
          Assert.That(content is null);
        }
      }
    }
  }

  [Test]
  public void board_effects_destroy_units_with_no_health() {
    var content1 = new BoardContent() {
      Health = 2
    };
    var content2 = new BoardContent() {
      Health = 1
    };
    var size = new Vector2Int(4, 4);
    var board = Board.CreateBoard(size, new[] {
      Tuple.Create(content1, new Vector2Int(0, 1)),
      Tuple.Create(content2, new Vector2Int(2, 1)) }).NextBoard(new[] { new ActionEffect() {
        position = new Vector2Int(0, 1),
        move = new Vector2Int(2, 0)
      }});

    Assert.That(board.getSize() == size);
    CollectionAssert.AreEqual(new[] { content1.identifier }, board.GetAllContent());
    Assert.That(board.positionOfContent(content1.identifier) == new Vector2Int(1, 1));

    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        var position = new Vector2Int(i, j);
        var content = board.ContentAt(position);
        if (position == board.positionOfContent(content1.identifier)) {
          Assert.That(content.identifier == content1.identifier);
          Assert.That(content.Health == content1.Health - 1);
        } else {
          Assert.That(content is null);
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
    CollectionAssert.AreEqual(new[] { content1.identifier, content2.identifier }, board.GetAllContent());
    Assert.That(board.positionOfContent(content1.identifier) == new Vector2Int(0, 1));
    Assert.That(board.positionOfContent(content2.identifier) == new Vector2Int(2, 1));

    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        var position = new Vector2Int(i, j);
        var content = board.ContentAt(position);
        if (position == board.positionOfContent(content1.identifier)) {
          Assert.That(content.identifier == content1.identifier);
          Assert.That(content.Health == content1.Health);
        } else if (position == board.positionOfContent(content2.identifier)) {
          Assert.That(content.identifier == content2.identifier);
          Assert.That(content.Health == content2.Health - 1);
        } else {
          Assert.That(content is null);
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
    CollectionAssert.AreEqual(new[] { content1.identifier, content2.identifier }, board.GetAllContent());
    Assert.That(board.positionOfContent(content1.identifier) == new Vector2Int(0, 3));
    Assert.That(board.positionOfContent(content2.identifier) == new Vector2Int(2, 1));

    for (int i = 0; i < size.x; ++i) {
      for (int j = 0; j < size.y; ++j) {
        var position = new Vector2Int(i, j);
        var content = board.ContentAt(position);
        if (position == board.positionOfContent(content1.identifier)) {
          Assert.That(content.identifier == content1.identifier);
          Assert.That(content.Health == content1.Health - 1);
        } else if (position == board.positionOfContent(content2.identifier)) {
          Assert.That(content.identifier == content2.identifier);
          Assert.That(content.Health == content2.Health - 1);
        } else {
          Assert.That(content is null);
        }
      }
    }
  }
}
