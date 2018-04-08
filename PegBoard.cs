using System;
using System.Collections.Generic;
using System.Text;

namespace PegJumper
{
    // Temporary data class to reduce use of excess looping.
    public class PegHole
    {
        public PegHole(int number, bool hasPeg, int row, int column)
        {
            Number = number;
            HasPeg = hasPeg;
            Row = row;
            Column = column;
        }

        public int Number;
        public bool HasPeg;
        public int Row;
        public int Column;
    }

    public struct PegBoardMove
    {
        public int Start;
        public int Popped;
        public int End;
    }

    public sealed class PegBoard
    {
        private const int MinimumRowCount = 5;
        private const int DefaultEmptyHole = 1;
        private const int MoveDistance = 2;

        private PegHole[][] holes;

        public PegBoard(int rows = MinimumRowCount, int emptyHole = DefaultEmptyHole)
        {
            //if (rows < MinimumRowCount)
            //        holes = new PegHole[MinimumRowCount][];
            //    else
                    holes = new PegHole[rows][];

            int holeIndex = 1;
            int holesPerRow = 1;
            for (int r = 0; r < rows; r++)
            {
                holes[r] = new PegHole[holesPerRow];

                for (int c = 0; c < holesPerRow; c++)
                {
                    holes[r][c] = new PegHole(holeIndex, holeIndex != emptyHole, r, c);
                    holeIndex++;
                }

                holesPerRow++;
            }

            HoleCount = holeIndex - 1;
        }

        public int HoleCount { get; }

        public int PegCount
        {
            get
            {
                int pegs = 0;

                for (int r = 0; r < holes.Length; r++)
                {
                    for (int c = 0; c < holes[r].Length; c++)
                    {
                        if (holes[r][c].HasPeg)
                            pegs++;
                    }
                }

                return pegs;
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
        public PegHole TryMovePeg(int target, int destination, bool check = false)
        {
            PegHole targetHole, middleHole, destinationHole;

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

            middleHole = null;
            
            if (destinationHole.Row == targetHole.Row)
            {
                // The destination hole is on the same row as the target hole.
                int distance = destinationHole.Column - targetHole.Column;

                if (Math.Abs(distance) == MoveDistance)
                {
                    int middleOffset;

                    if (distance > 0)
                        middleOffset = -1;
                    else
                        middleOffset = 1;

                    middleHole = GetPegHole(destination + middleOffset);
                }
            }
            else
            {
                // The destination hole is on a row above or below the target hole.
                int distance = destinationHole.Row - targetHole.Row;

                if (Math.Abs(distance) == MoveDistance)
                {
                    int middleOffset;

                    if (distance > 0)
                        middleOffset = 1;
                    else
                        middleOffset = -1;
                    
                    if (destinationHole.Column == targetHole.Column)
                        middleHole = holes[targetHole.Row + middleOffset][targetHole.Column];
                    else if (destinationHole.Column == targetHole.Column + distance)
                        middleHole = holes[targetHole.Row + middleOffset][targetHole.Column + middleOffset];
                }
            }

            if (!check && middleHole != null)
            {
                targetHole.HasPeg = false;
                middleHole.HasPeg = false;
                destinationHole.HasPeg = true;
            }

            return middleHole;
        }

        public List<PegBoardMove> GetAllPossibleMoves()
        {
            var moves = new List<PegBoardMove>();

            for (int r = 0; r < holes.Length; r++)
            {
                for (int c = 0; c < holes[r].Length; c++)
                {
                    var move = new PegBoardMove();
                    move.Start = holes[r][c].Number;
                    move.Popped = -1;

                    // left
                    if (c - 2 >= 0)
                    {
                        move.End = holes[r][c - 2].Number;
                        var possibility = TryMovePeg(move.Start, move.End, true);

                        if (possibility != null)
                        {
                            if (possibility.HasPeg)
                            {
                                move.Popped = possibility.Number;
                                moves.Add(move);
                            }

                        }
                    }

                    // right
                    if (c + 2 < holes[r].Length)
                    {
                        move.End = holes[r][c + 2].Number;
                        var possibility = TryMovePeg(move.Start, move.End, true);

                        if (possibility != null)
                        {
                            if (possibility.HasPeg)
                            {
                                move.Popped = possibility.Number;
                                moves.Add(move);
                            }

                        }
                    }

                    // above
                    if (r + 2 < holes.Length)
                    for (int nc = 0; nc < holes[r + 2].Length; nc++)
                    {
                        move.End = holes[r + 2][nc].Number;
                        var possibility = TryMovePeg(move.Start, move.End, true);

                        if (possibility != null)
                        {
                            if (possibility.HasPeg)
                            {
                                move.Popped = possibility.Number;
                                moves.Add(move);
                            }
                        }
                    }

                    // below
                    if (r - 2 >= 0)
                    for (int nc = 0; nc < holes[r - 2].Length; nc++)
                    {
                        move.End = holes[r - 2][nc].Number;
                        var possibility = TryMovePeg(move.Start, move.End, true);

                        if (possibility != null)
                        {
                            if (possibility.HasPeg)
                            {
                                move.Popped = possibility.Number;
                                moves.Add(move);
                            }
                        }
                    }
                }
            }

            return moves;
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

            return null;
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
    }
}
