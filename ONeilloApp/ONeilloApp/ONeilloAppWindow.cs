using ONeilloApp.CustomObjects;
using System.Net;
using Newtonsoft.Json;
using System.Speech.Synthesis;

namespace ONeilloApp
{
    public partial class Form1 : Form
    {
        #region Variables

        bool isSpeakingEnabled = false;
        bool firstMove = true;
        CircleButton[,] buttons = new CircleButton[8, 8];
        CustomObjects.Rectangle informationPanel = new();

        #endregion Variables


        #region Form Creation
        public Form1()
        {
            InitializeComponent();
            WindowIcon();
            Board.InitialiseBoard();
            PopulateGrid();
            this.Paint += new PaintEventHandler(DrawGrid);
            informationPanel.Location = new Point(10, 730);
            informationPanel.Size = new Size(670, 60);
            this.Controls.Add(informationPanel);


        }

        #endregion Form Creation


        #region Initialisation Functions

        //Creates the gameboard for a new game
        private void NewGame()
        {
            Board.InitialiseBoard();
            UpdateGrid();

            Player.player1Name = "Player 1";
            Player.player2Name = "Player 2";
            Player1TextBox.Text = Player.player1Name;
            Player2TextBox.Text = Player.player2Name;
            Player1TextBox.Visible = true;
            Player2TextBox.Visible = true;
            Player1TextBox.Enabled = true;
            Player2TextBox.Enabled = true;
            player1.Visible = false;
            player2.Visible = false;
            player1TurnLabel.Visible = false;
            player2TurnLabel.Visible = false;
            firstMove = true;
        }

        //It downloads the window icon from github and then applies it to the window
        private void WindowIcon()
        {
            this.Icon = new System.Drawing.Icon("Files/Icon.ico");
        }

        //Adds all Buttons to Window and 2D Array
        private void PopulateGrid()
        {


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    CircleButton button = new();
                    button.Size = new Size(80, 80);
                    button.Tag = new Point(i, j);
                    button.Location = new Point(i * (80 + 5) + 10, j * (80 + 5) + 40);
                    button.Click += Button_Click;
                    button.TabStop = false;
                    button.BackColor = Board.CurrentBoard[i, j].Value switch
                    {
                        PossibleValues.EMPTY => Color.FromArgb(0, 50, 50, 50),
                        PossibleValues.WHITE => Color.FromArgb(255, 255, 255),
                        PossibleValues.BLACK => Color.FromArgb(0, 0, 0),
                        _ => throw new NotImplementedException()
                    };
                    buttons[i, j] = button;
                    Controls.Add(button);

                }
            }
            DisplayLegalMoves();
        }

        //Draws the grid on the window
        private void DrawGrid(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Pen pen = new Pen(Color.Black, 1))
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        int x = (i * (80 + 5)) + 7;
                        int y = (j * (80 + 5)) + 35;
                        // Draw vertical line
                        g.DrawLine(pen, new Point(x, y + 2), new Point(x, y + 86));
                    }
                    //Draw horizontal line
                    g.DrawLine(pen, new Point(7, (i * (80 + 5) + 37)), new Point(686, (i * (80 + 5) + 37)));
                }
            }
        }



        //Updates the window to match with the 2d Array of Buttons
        private void UpdateGrid()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    switch (Board.CurrentBoard[i, j].Value)
                    {
                        case PossibleValues.EMPTY:
                            buttons[i, j].BackColor = Color.FromArgb(0, 50, 50, 50);
                            break;
                        case PossibleValues.WHITE:
                            buttons[i, j].BackColor = Color.FromArgb(255, 255, 255);
                            break;
                        case PossibleValues.BLACK:
                            buttons[i, j].BackColor = Color.FromArgb(0, 0, 0);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            DisplayLegalMoves();
        }


        //Used to show possible legal moves and to determine if a game is continueable
        private List<CircleButton> GetLegalMoves(PossibleValues Value)
        {
            List<CircleButton> LegalMoves = new();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board.IsLegalMove(i, j, Value))
                    {
                        LegalMoves.Add(buttons[i, j]);
                    }
                }
            }

            return LegalMoves;
        }

        //Updates the grid to show legal moves in grey
        private void DisplayLegalMoves()
        {
            foreach (CircleButton button in GetLegalMoves(Player.CurrentPlayer))
            {
                button.BackColor = Color.FromArgb(150, 150, 150);
            }
        }

        private void GetScore()
        {
            int blackScore = 0;
            int whiteScore = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (Board.CurrentBoard[i, j].Value)
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
            Player.player1Score = blackScore;
            Player.player2Score = whiteScore;

            player1.Text = $"{Player.player1Name}: {Player.player1Score}";
            player2.Text = $"{Player.player2Name}: {Player.player2Score}";
        }

        #endregion Initialisation Functions


        #region Misc Functions

        //Function to save the game to a json file
        private void SaveGame()
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            saveFileDialog.DefaultExt = "json";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                GameState gameState = new()
                {
                    Board = new PossibleValues[8, 8],
                    CurrentPlayer = Player.CurrentPlayer,
                    Player1Name = Player.player1Name,
                    Player2Name = Player.player2Name
                };

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        gameState.Board[i, j] = Board.CurrentBoard[i, j].Value;
                    }
                }

                string json = JsonConvert.SerializeObject(gameState);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        //Function to load a game from a JSON file
        private void LoadGame()
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                GameState gameState = JsonConvert.DeserializeObject<GameState>(json);

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Board.CurrentBoard[i, j].Value = gameState.Board[i, j];
                    }
                }

                Player.CurrentPlayer = gameState.CurrentPlayer;
                Player.player1Name = gameState.Player1Name;
                Player1TextBox.Text = gameState.Player1Name;
                Player2TextBox.Text = gameState.Player2Name;
                Player1TextBox.Enabled = false;
                Player2TextBox.Enabled = false;
                Player1TextBox.Visible = false;
                Player2TextBox.Visible = false;
                player1.Visible = true;
                player2.Visible = true;
                UpdateGrid();
            }
        }

        //Allows the moves to be spoken audibly, it's asynchronous so it doesn't delay the next move
        private void SpeakMove(string move)
        {
            if (isSpeakingEnabled)
            {
                SpeechSynthesizer synthesizer = new SpeechSynthesizer();

                synthesizer.SpeakCompleted += (sender, args) =>
                {
                    synthesizer.Dispose();
                };

                synthesizer.SpeakAsync(move);
            }
        }

        //Handles Changing the current Turn count icon
        private void ToggleTurnCount()
        {
            switch (Player.CurrentPlayer)
            {
                case PossibleValues.WHITE:
                    player1TurnLabel.Visible = false;
                    player2TurnLabel.Visible = true;
                    break;

                case PossibleValues.BLACK:
                    player1TurnLabel.Visible = true;
                    player2TurnLabel.Visible = false;
                    break;
            }
        }



        #endregion Misc Functions


        #region Event Handlers

        // Handles BoardTiles being clicked
        private void Button_Click(object? sender, EventArgs e)
        {

            Button? clickedButton = sender as Button;

            Point position = (Point)clickedButton.Tag;
            int x = position.X;
            int y = position.Y;

            if (Board.IsLegalMove(x, y, Player.CurrentPlayer))
            {
                if (firstMove)
                {
                    Player.player1Name = Player1TextBox.Text;
                    Player.player2Name = Player2TextBox.Text;
                    Player1TextBox.Visible = false;
                    Player2TextBox.Visible = false;
                    Player1TextBox.Enabled = false;
                    Player2TextBox.Enabled = false;
                    player1.Visible = informationPanel.Visible;
                    player2.Visible = informationPanel.Visible;
                    firstMove = false;
                }


                Board.MakeMove(x, y);

                string color = Player.CurrentPlayer == PossibleValues.BLACK ? "Black" : "White";
                char column = (char)('a' + x);
                int row = y + 1;
                SpeakMove($"{color} {column}{row}");

                Player.Toggle();
                UpdateGrid();
                ToggleTurnCount();
                GetScore();

                if (GetLegalMoves(Player.CurrentPlayer).Count == 0)
                {
                    if (GetLegalMoves(Player.Opposite()).Count == 0)
                    {
                        Player.GetWinner();
                        NewGame();
                    }
                    else
                    {
                        Player.Toggle();
                        UpdateGrid();
                        GetScore();
                    }


                }
            }
        }

        //Handles Creating a new game from the menu
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        //Handles Toggling the informationPanel
        private void informationPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            informationPanel.Visible = !informationPanel.Visible;
            informationPanelToolStripMenuItem.Checked = !informationPanel.Visible;
            switch (firstMove)
            {
                case true:
                    Player1TextBox.Visible = !Player1TextBox.Visible;
                    Player2TextBox.Visible = !Player2TextBox.Visible;
                    break;

                case false:
                    player1.Visible = !player1.Visible;
                    player2.Visible = !player2.Visible;
                    break;
            }


        }

        //Handles the about button
        private void AboutToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            string message = "Welcome to ONeillo\n\n" +
                     "This is my take on the classic game Othello\n" +
                     "This was developed by Matthew Rawson\n" +
                     "The GUI class was not used and this was built from the ground up";

            string title = "ONeillo";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Handles the save game button
        private void SaveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGame();
        }

        //Handles the load game button
        private void LoadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadGame();
        }

        //Handles the speech button
        private void SpeakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isSpeakingEnabled = !isSpeakingEnabled;
            speakToolStripMenuItem.Checked = isSpeakingEnabled;
        }

        #endregion Event Handlers
    }
}