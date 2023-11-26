namespace ONeilloApp
{
    public enum PossibleValues
    {
        EMPTY,
        WHITE,
        BLACK
    }

    internal class BoardTile
    {
        public PossibleValues Value;

        public BoardTile()
        {
            this.Value = PossibleValues.EMPTY;
        }
    }
}
