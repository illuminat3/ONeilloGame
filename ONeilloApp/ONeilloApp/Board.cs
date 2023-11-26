namespace ONeilloApp
{
    internal class Board
    {
        public static BoardTile[,] CurrentBoard;

        public static void InitialiseBoard()
        {
            CurrentBoard = new BoardTile[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    CurrentBoard[i, j] = new BoardTile();
                }
            }

            // Set the initial four discs
            CurrentBoard[3, 3].Value = PossibleValues.WHITE;
            CurrentBoard[3, 4].Value = PossibleValues.BLACK;
            CurrentBoard[4, 3].Value = PossibleValues.BLACK;
            CurrentBoard[4, 4].Value = PossibleValues.WHITE;
        }

        


        public static bool IsLegalMove(int x, int y, PossibleValues playerColor)
        {
            PossibleValues opponentColor = playerColor == PossibleValues.BLACK ? PossibleValues .WHITE : PossibleValues.BLACK;

            if (CurrentBoard[x, y].Value != PossibleValues.EMPTY) return false;

            int[] directionsX = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] directionsY = { -1, 0, 1, 1, 1, 0, -1, -1 };

            for (int i = 0; i < 8; i++)
            {
                int dx = directionsX[i];
                int dy = directionsY[i];
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newX < 8 && newY >= 0 && newY < 8 && CurrentBoard[newX, newY].Value == opponentColor)
                {
                    int step = 2;
                    while (true)
                    {
                        newX = x + dx * step;
                        newY = y + dy * step;

                        if (newX < 0 || newX >= 8 || newY < 0 || newY >= 8 || CurrentBoard[newX, newY].Value == PossibleValues.EMPTY)
                        {
                            break;
                        }

                        if (CurrentBoard[newX, newY].Value == playerColor)
                        {
                            return true;
                        }

                        step++;
                    }
                }
            }

            return false;
        }


        public static void MakeMove(int x, int y)
        {
            if (CurrentBoard[x, y].Value != PossibleValues.EMPTY)
            {
                throw new InvalidOperationException("The selected square is not empty.");
            }

            PossibleValues OppositeColor = Player.Opposite();
            CurrentBoard[x, y].Value = Player.CurrentPlayer;

            int[] directionsX = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] directionsY = { -1, 0, 1, 1, 1, 0, -1, -1 };

            for (int direction = 0; direction < 8; direction++)
            {
                int dx = directionsX[direction];
                int dy = directionsY[direction];

                int currentX = x + dx;
                int currentY = y + dy;

                bool foundOpposite = false;

                // Move in the direction and check if there is a line of opposite color
                while (IsWithinBoard(currentX, currentY) && CurrentBoard[currentX, currentY].Value == OppositeColor)
                {
                    foundOpposite = true;
                    currentX += dx;
                    currentY += dy;
                }

                // Check if the line ends with the current player's color
                if (foundOpposite && IsWithinBoard(currentX, currentY) && CurrentBoard[currentX, currentY].Value == Player.CurrentPlayer)
                {
                    // Flip the pieces
                    while (currentX != x || currentY != y)
                    {
                        currentX -= dx;
                        currentY -= dy;
                        CurrentBoard[currentX, currentY].Value = Player.CurrentPlayer;
                    }
                }
            }

        }

        //Checks to make sure a given coordinate is within the board (stops infinite searches)
        private static bool IsWithinBoard(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }


        private static void CountPoints()
        {
            int blackScore = 0;
            int whiteScore = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (Board.CurrentBoard[i,j].Value)
                    {
                        case PossibleValues.BLACK:
                            blackScore++;
                            break;

                        case PossibleValues.WHITE:
                            whiteScore++; 
                            break;

                        default:
                            break;

                    }
                }
            }
        }


    }
}
