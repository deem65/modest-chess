# Known Bugs

Known issues in the current Modest Chess codebase.

This file represents the baseline before the core refactor. Do not remove an issue until it has been fixed and covered by a test where practical.

---

## Critical

### [ ] Turn order is not enforced

**Current behavior:** White or black can move regardless of whose turn it should be.

**Expected behavior:** White moves first, then turns alternate after every legal move.

**Area:** Move validation / game state

---

### [ ] The king can be captured

**Current behavior:** A move can capture the opposing king.

**Expected behavior:** Kings are never captured. A game ends when a player is checkmated.

**Area:** Move generation / move validation

---

### [ ] Custom board parsing can leave null squares

**Current behavior:** Numeric empty-square runs in the custom board parser can advance farther than the number of `Square` objects actually created, leaving null entries in the board.

**Expected behavior:** Every board position must always contain a valid square.

**Area:** Board parsing

---

### [ ] Pawn movement can access outside the board

**Current behavior:** A pawn reaching the final rank can cause move-generation logic to attempt to access positions outside the board.

**Expected behavior:** Pawn movement must remain in bounds and reaching the final rank must trigger promotion.

**Area:** Pawn move generation

---

## High

### [ ] Draw detection is incorrect

**Current behavior:** The current draw logic can produce incorrect results.

Known problems include:

- discarded `OrderBy()` results
- incorrect comparison logic
- incomplete insufficient-material detection

**Expected behavior:** Draw detection should correctly identify all supported draw conditions.

**Area:** Game rules

---

### [ ] Promotion is not implemented

**Expected behavior:** A pawn reaching the final rank must promote to one of:

- Queen
- Rook
- Bishop
- Knight

**Area:** Pawn rules / game state

---

### [ ] Castling is not implemented

**Expected behavior:** Support kingside and queenside castling while correctly enforcing:

- king has not moved
- relevant rook has not moved
- squares between king and rook are empty
- king is not currently in check
- king does not pass through check
- king does not end in check

**Area:** King rules / game state

---

### [ ] En passant is not implemented

**Expected behavior:** Track the previous pawn double-step and allow en passant only on the immediately following move when legal.

**Area:** Pawn rules / game state

---

### [ ] Threefold repetition is not implemented

**Expected behavior:** The game should recognize a draw when the same position occurs three times under the proper chess repetition rules.

**Area:** Game history / draw rules

---

### [ ] Fifty-move rule is not implemented

**Expected behavior:** Track the halfmove clock and allow a draw after fifty moves by each side without a pawn move or capture.

**Area:** Game state / draw rules

---

### [ ] The game does not reliably enter a terminal state

**Current behavior:** After checkmate or draw detection, the game does not have a proper persistent finished-game state.

**Expected behavior:** Once the game ends, further moves should be rejected until a new game begins.

**Area:** Game state / UI

---

## Medium

### [ ] Stalemate logic is incorrectly named / modeled

**Current behavior:** `IsInStalemate()` behaves more like a check for whether a player has any legal moves.

**Expected behavior:** Stalemate requires both:

1. the player is not in check
2. the player has no legal moves

**Area:** Game rules

---

### [ ] UI can display pseudo-legal moves

**Current behavior:** The UI may highlight moves that later fail because they leave the player's own king in check.

**Expected behavior:** Only fully legal moves should be shown to the player.

**Area:** UI / move generation

---

### [ ] Draw notification can appear more than once

**Current behavior:** Draw checking occurs while iterating over both players, allowing the same draw message to be displayed multiple times.

**Expected behavior:** A game result should be determined once and displayed once.

**Area:** UI / game result handling

---

### [ ] Empty custom-board input can crash

**Current behavior:** Submitting an empty board string can reach parsing logic that expects valid input.

**Expected behavior:** Invalid or empty input should be rejected cleanly.

**Area:** Custom board UI / parsing

---

### [ ] Custom-board input validation condition is incorrect

**Current behavior:** The current condition:

```csharp
boardInput != null || boardInput == string.Empty
```

does not correctly validate non-empty input.

**Expected behavior:** Only valid, non-empty input should be passed to the parser.

**Area:** Custom board UI

---

### [ ] Static board color state can leak between games

**Current behavior:** Board-related color state is stored statically and can therefore be shared unexpectedly across board instances.

**Expected behavior:** Each game should own its own independent state.

**Area:** Board model / architecture

---

### [ ] `pawnlessMoves` is static and does not represent the full fifty-move rule

**Current behavior:** The counter is shared globally and does not correctly implement the actual chess halfmove rule.

**Expected behavior:** The counter should belong to an individual game state and reset after either a pawn move or a capture.

**Area:** Game state / draw rules

---

## Low / Cleanup

### [ ] `DeepCopy()` creates unnecessary board state

**Current behavior:** A full default board is constructed and then replaced during copying.

**Expected behavior:** Copy only the required state without constructing data that is immediately discarded.

**Area:** Board model

---

### [ ] Position naming is confusing

**Current behavior:** Board coordinates use `X` and `Y` in a way that does not clearly map to chess concepts.

**Expected behavior:** Prefer chess-specific or clearly defined terminology such as `file` and `rank`.

**Area:** Core model

---

### [ ] Piece subclasses contain very little behavior

**Current behavior:** `King`, `Queen`, `Rook`, `Bishop`, `Knight`, and `Pawn` mostly exist to assign IDs/indexes while rule logic lives elsewhere.

**Expected behavior:** During the core refactor, decide whether inheritance is actually useful or whether a `PieceType` enum provides a cleaner model.

**Area:** Architecture

---

## Missing Rule Coverage

These are not necessarily bugs in existing code, but they must be completed before Modest Chess can be considered rules-complete.

- [ ] Promotion
- [ ] Castling
- [ ] En passant
- [ ] Threefold repetition
- [ ] Fifty-move rule
- [ ] Correct insufficient-material detection
- [ ] Reliable checkmate handling
- [ ] Reliable stalemate handling
- [ ] Complete game-over state

---

## Testing Baseline

As bugs are fixed, add regression tests where practical.

Priority test cases:

- [ ] White moves first
- [ ] Same player cannot move twice
- [ ] King cannot be captured
- [ ] King cannot move into check
- [ ] Pinned piece cannot expose its king
- [ ] Checkmate ends the game
- [ ] Stalemate produces a draw
- [ ] Promotion works for all four promotion pieces
- [ ] Kingside castling works
- [ ] Queenside castling works
- [ ] Castling through check is rejected
- [ ] En passant works only immediately after the pawn double-step
- [ ] Fifty-move counter resets after pawn moves
- [ ] Fifty-move counter resets after captures
- [ ] Repeated positions are tracked correctly
- [ ] Insufficient-material positions are detected correctly
- [ ] Pawn move generation never accesses outside the board

---

## Notes

The custom board-position format should likely be replaced with standard FEN instead of investing further work into fixing and extending the existing format.

The main architectural goal is to move match-level state into a dedicated `GameState` model before multiplayer is implemented.
