using System;

namespace PegJumper
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var board = new PegBoard();
            var solver = new PegBoardSolver(board);
            solver.Process();
        }
    }
}
