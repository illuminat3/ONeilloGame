using ONeilloApp.CustomObjects;
using System.Net;
using Newtonsoft.Json;
using System.Speech.Synthesis;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace ONeilloApp
{
    public partial class Form1 : Form
    {
        #region Variables

        bool isSpeakingEnabled; //Toggles Speech
        bool informationPanelEnabled; //Toggles Information Panel
        bool firstMove = true; //Used to hide textboxes and lock names
        string jsonFilePath = "Files/game_data.json"; //File Path to game_data.json
        CircleButton[,] buttons = new CircleButton[8, 8]; //Game Board for drawing
        CustomObjects.Rectangle informationPanel = new(); //Creates the information panel

        #endregion Variables


        #region Form Creation
        public Form1()
        {
            InitializeComponent(); //Initialise form elements
            SetupSettings(); //Get speech and information panel status from game_data.json
            WindowIcon(); //Gets the Icon file
            Board.InitialiseBoard(); //Create the board for calculatiosn
            PopulateGrid(); //Add all circle buttons to the visual board 
            this.Paint += new PaintEventHandler(DrawGrid); //Adds the grid
            informationPanel.Location = new Point(5, 730); //Places the information panel on the grid
            informationPanel.Size = new Size(685, 60); //Changes the size of the information panel
            this.Controls.Add(informationPanel); //Add the information panel to form
            this.ClientSize = new Size(695, 800); //Sets the window size
        }

        #endregion Form Creation


        #region Initialisation Functions

        /// <summary>
        /// Used to create a new game
        /// </summary>
        private void NewGame()
        {
            Board.InitialiseBoard(); //Create a starting position board for calculations
            UpdateGrid(); //Update the form to show the current position
            SetupSettings(); //Get the current status of speech and information panel from game_data

            //Setup the player names, allowing the user to change it and defaulting the names to "Player 1" and "Player 2"
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
            firstMove = true; //Used to toggle changing the player name
        }

        /// <summary>
        /// Gets and sets the values of isSpeakingEnabled and informationPanelEnabled from game_data.json
        /// </summary>
        private void SetupSettings()
        {
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(jsonFilePath));

            isSpeakingEnabled = data.Variables.Speech == 1;
            informationPanelEnabled = data.Variables.InformationPanel == 1;
        }

        /// <summary>
        /// It downloads the window icon from github and then applies it to the window
        /// </summary>
        private void WindowIcon()
        {
            this.Icon = new System.Drawing.Icon("Files/Icon.ico");
        }

        /// <summary>
        /// Adds all Buttons to Window and 2D Array
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
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

        /// <summary>
        /// Draws the grid on the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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



        /// <summary>
        /// Updates the window to match with the 2d Array of Buttons
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
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


        /// <summary>
        /// Used to show possible legal moves and to determine if a game is continueable
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the grid to show legal moves in grey
        /// </summary>
        private void DisplayLegalMoves()
        {
            foreach (CircleButton button in GetLegalMoves(Player.CurrentPlayer))
            {
                button.BackColor = Color.FromArgb(150, 150, 150);
            }
        }

        /// <summary>
        /// Gets the score
        /// </summary>
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

        /// <summary>
        /// Updates the json file to match with the current state
        /// </summary>
        private void UpdateGameData()
        {
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(jsonFilePath));

            data.Variables.Speech = isSpeakingEnabled ? 1 : 0;
            data.Variables.InformationPanel = informationPanelEnabled ? 1 : 0;

            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Opens the prompt that asks the user to save the game
        /// </summary>
        private void SaveGamePrompt()
        {
            if (!firstMove)
            {
                DialogResult dialogResult = MessageBox.Show("Would you like to save this game?", "Save Game", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    SaveGame();
                }
            }
        }

        /// <summary>
        /// Function to save the game to a json file
        /// </summary>
        private void SaveGame()
        {

            GameState gameState = new()
            {
                gameName = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
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

            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(jsonFilePath));
            data.Games.Add(JObject.FromObject(gameState));
            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(data, Formatting.Indented));
        }


        /// <summary>
        /// Function to load a game from a JSON file
        /// </summary>
        private void RestoreGame()
        {
            var gameNames = new List<string>();

            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(jsonFilePath));
            if (data?.Games != null)
            {
                foreach (var game in data.Games)
                {
                    gameNames.Add(game.gameName.ToString());
                }
            }

            if (gameNames.Count != 0) { 

            using (var form = new SelectGame(gameNames))
            {
                var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        int selectedGame = form.selectedGame;
                        GameState gameState = data.Games[selectedGame].ToObject<GameState>();


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
            }
        }

        /// <summary>
        /// Exits the game
        /// </summary>
        private void ExitGame()
        {
            Board.BlankBoard();
            UpdateGrid();
            SetupSettings();

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

        /// <summary>
        /// Allows the moves to be spoken audibly, it's asynchronous so it doesn't delay the next move
        /// </summary>
        /// <param name="move"></param>
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

        /// <summary>
        /// Handles Changing the current Turn count icon
        /// </summary>
        private void ToggleTurnCount()
        {
            if (informationPanelEnabled)
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
            else
            {
                player1TurnLabel.Visible = false;
                player2TurnLabel.Visible = false;
            }
        }



        #endregion Misc Functions


        #region Event Handlers

        /// <summary>
        /// Handles BoardTiles being clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Handles Creating a new game from the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGamePrompt();
            NewGame();
        }

        /// <summary>
        /// Handles Toggling the informationPanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void informationPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetupSettings();
            informationPanelEnabled = !informationPanelEnabled;
            informationPanel.Visible = informationPanelEnabled;
            informationPanelToolStripMenuItem.Checked = informationPanelEnabled;
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
            ToggleTurnCount();
            UpdateGameData();


        }

        /// <summary>
        /// Handles the about button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            string message = "Welcome to ONeillo\n\n" +
                     "This is my take on the classic game Othello\n" +
                     "This was developed by Matthew Rawson\n" +
                     "The GUI class was not used and this was built from the ground up";

            string title = "ONeillo";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles the save game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGame();
        }

        /// <summary>
        /// Handles the load game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestoreGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreGame();
        }

        /// <summary>
        /// Handles the speech button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetupSettings();
            isSpeakingEnabled = !isSpeakingEnabled;
            speakToolStripMenuItem.Checked = isSpeakingEnabled;
            UpdateGameData();
        }

        /// <summary>
        /// Handles the exit game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGamePrompt();
            ExitGame();
        }

        #endregion Event Handlers
    }
}