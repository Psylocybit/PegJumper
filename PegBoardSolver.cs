using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PegJumper
{
    public class PegBoardSolver
    {
        public const int DefaultWinCap = 10;

        private Random random;

        private PegBoard pegBoard;

        private List<string> wins;
        private int maxWins;

        private StringBuilder stepsBuilder, winBuilder;

        public PegBoardSolver() : this(new PegBoard())
        { }

        public PegBoardSolver(PegBoard board, int winCap = DefaultWinCap)
        {
            this.random = new Random();
            this.pegBoard = board;
            this.wins = new List<string>();
            this.maxWins = winCap;
            this.stepsBuilder = new StringBuilder();
            this.winBuilder = new StringBuilder();
            this.IsRunning = false;
        }

        public bool IsRunning { get; private set; }

        public int WinCountCap
        {
            get => this.maxWins;
            set
            {
                if (IsRunning)
                {
                    // No
                }
                else
                    this.maxWins = value;
            }
        }

        public void Process()
        {
            this.IsRunning = true;

            int highMoveCount = 0;
            int lowMoveCount = int.MaxValue;

            var actualMoves = new List<PegBoardMove>();

            while (wins.Count < this.maxWins)
            {
                var workingBoard = this.pegBoard.Clone() as PegBoard;

                while (true)
                {
                    var possibleMoves = workingBoard.GetAllPossibleMoves();
                    var movesList = new List<PegBoardMove>(possibleMoves);

                    if (movesList.Count == 0)
                    {
                        if (workingBoard.PegCount == 1)
                        {
                            bool unique = true;

                            foreach (var win in this.wins)
                            {
                                if (win == winBuilder.ToString())
                                {
                                    unique = false;
                                    stepsBuilder.Clear();
                                    break;
                                }
                            }

                            if (unique)
                            {
                                this.wins.Add(winBuilder.ToString());

                                Console.Clear();
                                Console.WriteLine(workingBoard);
                                Console.WriteLine(stepsBuilder);
                                stepsBuilder.Clear();
                                Thread.Sleep(2000);
                            }
                        }
                        break;
                    }

                    var choiceMove = movesList[this.random.Next(movesList.Count - 1)];
                    workingBoard.TryMovePeg(choiceMove.Start, choiceMove.End);
                    actualMoves.Add(choiceMove);

                    this.stepsBuilder.AppendLine(string.Format("{0} -> {1}; pop {2}",
                        choiceMove.Start,
                        choiceMove.End,
                        choiceMove.Popped));

                    this.winBuilder.Append(choiceMove.Popped);
                }

                winBuilder.Clear();

                if (actualMoves.Count < lowMoveCount)
                    lowMoveCount = actualMoves.Count;

                if (actualMoves.Count > highMoveCount)
                    highMoveCount = actualMoves.Count;

                actualMoves.Clear();
                stepsBuilder.Clear();
            }

            this.IsRunning = false;
        }
    }
}
