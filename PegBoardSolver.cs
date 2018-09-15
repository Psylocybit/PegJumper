using System;

namespace PegJumper
{
    public class PegBoardSolver
    {
        private PegBoard pegBoard;

        public PegBoardSolver() : this(new PegBoard())
        { }

        public PegBoardSolver(PegBoard board)
        {
            this.pegBoard = board;
        }
    }
}
