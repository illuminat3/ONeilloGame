namespace ONeilloApp
{
    internal class Player
    {
        public static PossibleValues CurrentPlayer = PossibleValues.BLACK;
        public static string player1Name = "player1";
        public static string player2Name = "player2";
        public static int player1Score = 2;
        public static int player2Score = 2;


        public static void Toggle()
        {
            switch (CurrentPlayer)
            {
                case PossibleValues.BLACK:
                    CurrentPlayer = PossibleValues.WHITE;
                    break;

                default:
                    CurrentPlayer = PossibleValues.BLACK;
                    break;
            }
        }

        public static PossibleValues Opposite()
        {
            switch (CurrentPlayer)
            {
                case PossibleValues.BLACK:
                    return PossibleValues.WHITE;


                default:
                    return PossibleValues.BLACK;

            }
        }

        public static void GetWinner()
        {
            string Winner = player1Score > player2Score ? "Black" :
                       player2Score > player1Score ? "White" : "Drawn Game";

            string resultMessage = $"Game Over!\n\n{(Winner == "Drawn Game" ? "Drawn Game" : $"{Winner} wins")}\n" +
                                $"{player1Name}: {player1Score}\n" +
                               $"{player2Name}: {player2Score}";

            MessageBox.Show(resultMessage, "Game Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
