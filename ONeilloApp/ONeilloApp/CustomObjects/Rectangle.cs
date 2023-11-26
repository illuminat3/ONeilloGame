namespace ONeilloApp.CustomObjects
{
    internal class Rectangle : Panel
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            Pen pen = new Pen(Color.Black, 2); // Black pen with width 2
            System.Drawing.Rectangle rect = new(0, 0, Width, Height);

            graphics.DrawRectangle(pen, rect);
        }
    }
}
