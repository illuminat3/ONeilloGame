namespace ONeilloApp.CustomObjects
{
    internal class CircleButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.Clear(Parent.BackColor);
            using (var brush = new SolidBrush(BackColor))
            {
                g.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            }

            if (Focused)
            {
                using var pen = new Pen(Color.Black);
                g.DrawEllipse(pen, 0, 0, Width - 1, Height - 1);
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
