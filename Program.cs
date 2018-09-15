using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace PegJumper
{
    class Program
    {
        static int Width = 5;

        static void Main(string[] args)
        {
            var rand = new Random();

            var board = new PegBoard(Width, 1);
            var originalBoard = board.ToString();

            var winStepsBuilder = new StringBuilder();

            var winIdents = new List<string>();
            var winIdentBuilder = new StringBuilder();

            var highMoveCount = 0;
            var lowMoveCount = int.MaxValue;

            var actualMoves = new List<PegBoardMove>();

            for (int i = 1; winIdents.Count < 10; i++)
            {
                board = new PegBoard(Width, 1);

                while (true)
                {
                    var possibleMoves = board.GetAllPossibleMoves();

                    if (possibleMoves.Count == 0)
                    {
                        if (board.PegCount == 1)
                        {
                            var unique = true;

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

                    var choiceMove = possibleMoves[rand.Next(possibleMoves.Count - 1)];
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
