namespace ONeilloApp
{
    public partial class SelectGame : Form
    {
        private ListBox listBox;
        private Button loadButton;
        public int selectedGame { get; private set; }

        public SelectGame(List<string> games)
        {
            listBox = new ListBox { Dock = DockStyle.Fill, DataSource = games };
            loadButton = new Button { Text = "Load Game", Dock = DockStyle.Bottom };
            loadButton.Click += LoadButton_Click;

            Controls.Add(listBox);
            Controls.Add(loadButton);
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            selectedGame = listBox.SelectedIndex;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
