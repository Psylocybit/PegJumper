using System;

namespace PegJumper
{
    public class PegHole
    {
        public int Row, Column;
        public int Number;
        public bool HasPeg;

        public PegHole(int row, int column, int number, bool hasPeg)
        {
            this.Row = row;
            this.Column = column;
            this.Number = number;
            this.HasPeg = hasPeg;
        }
    }
}
