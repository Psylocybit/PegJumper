using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace PegJumper
{
    public sealed class Program
    {
        private const int Width = 5;

        public static void Main(string[] args)
        {
            var rand = new Random();

            // Need a PegBoardSolver class with solve options
            // var solver = new PegBoardSolver(...);
            var board = new PegBoard(Width, 1);
            var originalBoard = board.ToString();

            var winStepsBuilder = new StringBuilder();

            var winIdents = new List<string>();
            var winIdentBuilder = new StringBuilder();

            int highMoveCount = 0;
            int lowMoveCount = int.MaxValue;

            var actualMoves = new List<PegBoardMove>();

            for (int i = 1; winIdents.Count < 10; i++)
            {
                board = new PegBoard(Width, 1);

                // Hmmm...
                while (true)
                {
                    var possibleMoves = board.GetAllPossibleMoves();
                    var movesList = new List<PegBoardMove>(possibleMoves);

                    if (movesList.Count == 0)
                    {
                        if (board.PegCount == 1)
                        {
                            bool unique = true;

                            foreach (var ident in winIdents)
                            {
                                if (ident == winIdentBuilder.ToString())
                                {
                                    unique = false;
                                    winStepsBuilder.Clear();
                                    break;
                                }
                            }

                            if (unique)
                            {
                                winIdents.Add(winIdentBuilder.ToString());
                                Console.Clear();
                                Console.WriteLine(board);
                                Console.WriteLine(winStepsBuilder);
                                winStepsBuilder.Clear();

                                Thread.Sleep(2000);
                            }
                        }
                        break;
                    }

                    var choiceMove = movesList[rand.Next(movesList.Count - 1)];
                    board.TryMovePeg(choiceMove.Start, choiceMove.End);
                    actualMoves.Add(choiceMove);

                    winStepsBuilder.AppendLine(string.Format("{0} -> {1}; pop {2}",
                                                             choiceMove.Start,
                                                             choiceMove.End,
                                                             choiceMove.Popped));

                    winIdentBuilder.Append(choiceMove.Popped);
                }

                winIdentBuilder.Clear();

                if (actualMoves.Count < lowMoveCount)
                    lowMoveCount = actualMoves.Count;

                if (actualMoves.Count > highMoveCount)
                    highMoveCount = actualMoves.Count;

                actualMoves.Clear();
                winStepsBuilder.Clear();
            }

            winIdents.Clear();
        }
    }
}
