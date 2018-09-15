using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PegJumper
{
    // Not used at the moment
    internal enum PegBoardRelation
    {
        None,
        All,
        Left,
        Right,
        Above,
        Below
    }

    internal struct PegBoardMove
    {
        public int Start;
        public int End;
        public int Popped;

        public PegBoardMove(int start, int end = -1, int popped = -1)
        {
            this.Start = start;
            this.End = end;
            this.Popped = popped;
        }
    }

    /*
     * Example peg board (5):
     * 
     *          o   <--- peg 1 (index 0)
     *         x x
     *        x x x
     *       x x x x
     *      x x x x x
     * 
     * Example peg move options:
     * 
     *          o   o      <--- Row above (x2)
     *           - -       <--- Row above (x1)
     *        o - x - o    <--- Same row
     *           - -       <--- Row below (x1)
     *          o   o      <--- Row below (x2)
     * 
     * If trying to move peg, p, to hole, d, then:
     *      The row of d must be the same as row of p or p +- 2 where p +- 2 >= 0 and <= peg row count - 2
     */
    public class PegBoard : ICloneable
    {
        internal const int MinimumRowCount = 5;
        internal const int DefaultEmptyHole = 1;
        internal const int MoveDistance = 2;

        private PegHole[][] holes;

        public PegBoard(int rows = MinimumRowCount, int emptyHole = DefaultEmptyHole)
        {
            this.holes = new PegHole[rows][];
            this.PegCount = 0;

            int holeIndex = 1;
            int holesPerRow = 1;

            for (int r = 0; r < rows; r++)
            {
                holes[r] = new PegHole[holesPerRow];

                for (int c = 0; c < holesPerRow; c++)
                {
                    var hasPeg = holeIndex != emptyHole;

                    if (hasPeg)
                        this.PegCount++;

                    this.holes[r][c] = new PegHole(r, c, holeIndex, hasPeg);
                    holeIndex++;
                }

                holesPerRow++;
            }

            this.HoleCount = holeIndex - 1;

            this.StartingHole = emptyHole;
        }

        private PegBoard(PegHole[][] pegHoles)
        {
            this.holes = pegHoles.Select(s => s.ToArray()).ToArray();
            //source.Select(s => s.ToArray()).ToArray();

            // Calculate other variables. For now this is just for ICloneable.
        }

        public int StartingHole { get; }

        public int RowCount => this.holes.Length;

        public int HoleCount { get; private set; }

        public int PegCount { get; private set; }

        public PegHole? TryMovePeg(int target, int destination, bool check = false)
        {
            PegHole targetHole, destinationHole;
            PegHole? middleHole = null;

            // Make sure target & destination peg holes are within bounds.
            if (target <= 0 || destination <= 0 || target == destination)
                return null;

            targetHole = GetPegHole(target);

            // Make sure the target peg hole is valid and not empty.
            if (!targetHole.HasPeg)
                return null;
            
            destinationHole = GetPegHole(destination);

            // Make sure the destination peg hole is valid and empty.
            if (destinationHole.HasPeg)
                return null;

            if (destinationHole.Row == targetHole.Row)
            {
                // The destination hole is on the same row as the target hole.
                int distance = destinationHole.Column - targetHole.Column;

                if (Math.Abs(distance) == MoveDistance)
                {
                    int middleOffset = distance > 0 ? -1 : 1;
                    middleHole = GetPegHole(destination + middleOffset);
                }
            }
            else
            {
                // The destination hole is on a row above or below the target hole.
                int distance = destinationHole.Row - targetHole.Row;

                if (Math.Abs(distance) == MoveDistance)
                {
                    int middleOffset = distance > 0 ? 1 : -1;

                    if (destinationHole.Column == targetHole.Column)
                        middleHole = holes[targetHole.Row + middleOffset][targetHole.Column];
                    else if (destinationHole.Column == targetHole.Column + distance)
                        middleHole = holes[targetHole.Row + middleOffset][targetHole.Column + middleOffset];
                }
            }

            if (!check && (middleHole?.HasPeg ?? false))
            {
                this.holes[targetHole.Row][targetHole.Column].HasPeg = false;
                this.holes[middleHole.Value.Row][middleHole.Value.Column].HasPeg = false;
                this.holes[destinationHole.Row][destinationHole.Column].HasPeg = true;
                this.PegCount--;
            }

            return middleHole;
        }

        internal IEnumerable<PegBoardMove> GetAllPossibleMoves()
        {
            for (int r = 0; r < holes.Length; r++)
            {
                for (int c = 0; c < holes[r].Length; c++)
                {
                    var possibleMoves = GetPossibleMoves(r, c);

                    foreach (var move in possibleMoves)
                    {
                        yield return move;
                    }
                }
            }
        }

        internal IEnumerable<PegBoardMove> GetPossibleMoves(int row, int col)
        {
            PegBoardMove move;

            var r = row;
            var c = col;

            // left
            if (c - 2 >= 0)
            {
                move = new PegBoardMove(holes[r][c].Number, holes[r][c - 2].Number);

                var possibility = TryMovePeg(move.Start, move.End, check: true);

                if (possibility?.HasPeg ?? false)
                {
                    move.Popped = possibility.Value.Number;
                    yield return move;
                }
            }

            // right
            if (c + 2 < holes[r].Length)
            {
                move = new PegBoardMove(holes[r][c].Number, holes[r][c + 2].Number);

                var possibility = TryMovePeg(move.Start, move.End, check: true);

                if (possibility?.HasPeg ?? false)
                {
                    move.Popped = possibility.Value.Number;
                    yield return move;
                }
            }

            // above
            if (r + 2 < holes.Length)
            {
                move = new PegBoardMove(holes[r][c].Number);

                for (int nc = 0; nc < holes[r + 2].Length; nc++)
                {
                    move.End = holes[r + 2][nc].Number;

                    var possibility = TryMovePeg(move.Start, move.End, check: true);

                    if (possibility?.HasPeg ?? false)
                    {
                        move.Popped = possibility.Value.Number;
                        yield return move;
                    }
                }
            }

            // below
            if (r - 2 >= 0)
            {
                move = new PegBoardMove(holes[r][c].Number);

                for (int nc = 0; nc < holes[r - 2].Length; nc++)
                {
                    move.End = holes[r - 2][nc].Number;

                    var possibility = TryMovePeg(move.Start, move.End, check: true);

                    if (possibility?.HasPeg ?? false)
                    {
                        move.Popped = possibility.Value.Number;
                        yield return move;
                    }
                }
            }
        }

        private PegHole GetPegHole(int targetHole)
        {
            if (targetHole > HoleCount || targetHole < 1)
                throw new IndexOutOfRangeException("Target hole number is not a valid hole.");

            for (int r = 0; r < holes.Length; r++)
            {
                for (int c = 0; c < holes[r].Length; c++)
                {
                    if (holes[r][c].Number == targetHole)
                    {
                        return holes[r][c];
                    }
                }
            }

            throw new Exception("Invalid target hole.");
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            int space;
            for (int r = 0; r < holes.Length; r++)
            {
                for (space = 1; space <= (holes.Length - (r + 1)); space++)
                    stringBuilder.Append(' ');

                for (int c = 0; c < holes[r].Length; c++)
                {
                    stringBuilder.Append(holes[r][c].HasPeg ? 'x' : 'o');

                    if (r > 0 && c < holes[r].Length)
                        stringBuilder.Append(' ');
                }

                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }

        public object Clone()
        {
            return new PegBoard(this.holes)
            {
                PegCount = this.PegCount,
                HoleCount = this.HoleCount
            };
        }
    }
}