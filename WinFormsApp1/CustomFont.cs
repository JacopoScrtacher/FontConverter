public class CustomFont
{
    private readonly Bitmap fontBitmap;
    private readonly string charset;
    public int FontSize;
    public float Scale;
    public int NumRow => fontBitmap.Width / FontSize;
    public int Thickness { get; set; } = 1;

    public CustomFont(string imagePath, string charset, int size, float scale = 1.0f, int thickness = 1)
    {
        this.fontBitmap = new Bitmap(imagePath);
        this.charset = charset;
        this.FontSize = size;
        this.Scale = scale;
        this.Thickness = thickness;
    }


    private int DrawCharNo(Graphics g, int X, int Y, char Chr, Color textColor)
    {
        int index = charset.IndexOf(Chr);
        if (index == -1)
        {
            if (Chr == ' ') return (int)(FontSize / (2 * Scale));
            return 0;
        }

        int baseX = 0, baseY = 0;
        for (int i = 0; i <= index; i++)
        {
            if ((i % NumRow) == 0 && i != 0)
            {
                baseX = 0;
                baseY += FontSize;
            }
            if (i != index)
                baseX += FontSize;
        }

        for (int w = 0; w < FontSize; w++)
        {
            int counter = 0;
            for (int h = 0; h < FontSize; h++)
            {
                Color sourcePixel = fontBitmap.GetPixel(baseX + w, baseY + h);
                uint sourceColor = (uint)sourcePixel.ToArgb();

                if (X != -1 && Y != -1)
                {
                    if ((sourceColor & 0xFF000000) != 0)
                    {
                        int drawX = X + (int)(w / Scale);
                        int drawY = Y + (int)(h / Scale);
                        using (SolidBrush brush = new SolidBrush(textColor))
                        {
                            g.FillRectangle(brush, drawX, drawY, 1, 1);
                        }
                    }
                }
                if ((sourceColor & 0xFF000000) == 0) counter++;
            }
            if (w > (FontSize / 3) && counter == FontSize)
                return (int)(w / Scale);
        }
        return (int)(FontSize / Scale);
    }

    private int DrawChar(Graphics g, int X, int Y, char Chr, Color textColor)
    {
        int index = charset.IndexOf(Chr);
        if (index == -1)
        {
            if (Chr == ' ') return (int)(FontSize / (2 * Scale));
            return 0;
        }

        int baseX = 0, baseY = 0;
        for (int i = 0; i <= index; i++)
        {
            if ((i % NumRow) == 0 && i != 0)
            {
                baseX = 0;
                baseY += FontSize;
            }
            if (i != index)
                baseX += FontSize;
        }

        for (int w = 0; w < FontSize; w++)
        {
            int counter = 0;
            for (int h = 0; h < FontSize; h++)
            {
                Color sourcePixel = fontBitmap.GetPixel(baseX + w, baseY + h);
                uint sourceColor = (uint)sourcePixel.ToArgb();

                if (X != -1 && Y != -1)
                {
                    if ((sourceColor & 0xFF000000) != 0)
                    {
                        for (int tx = 0; tx < Thickness; tx++)
                        {
                            for (int ty = 0; ty < Thickness; ty++)
                            {
                                int drawX = X + (int)(w / Scale) + tx;
                                int drawY = Y + (int)(h / Scale) + ty;
                                using (SolidBrush brush = new SolidBrush(textColor))
                                {
                                    g.FillRectangle(brush, drawX, drawY, 1, 1);
                                }
                            }
                        }
                    }
                }
                if ((sourceColor & 0xFF000000) == 0) counter++;
            }
            if (w > (FontSize / 3) && counter == FontSize)
                return (int)(w / Scale) + (Thickness - 1);
        }
        return (int)(FontSize / Scale) + (Thickness - 1);
    }

    public void DrawString(Graphics g, string text, int x, int y, Color color)
    {
        int w = 0, h = 0;
        for (int i = 0; i < text.Length; i++)
        {
            w += DrawChar(g, x + w, y + h, text[i], color);
        }
    }

    public void Draw_String(Graphics g, string text, int x, int y, Color color)
    {
        int w = 0, h = 0;
        for (int i = 0; i < text.Length; i++)
        {
            w += DrawCharNo(g, x + w, y + h, text[i], color);
        }
    }

    public class CustomFontLabel : Control
    {
        private CustomFont font;
        private Color textColor = Color.Black;

        public CustomFontLabel(CustomFont customFont)
        {
            font = customFont;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint, true);
        }

        public Color TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (font != null && !string.IsNullOrEmpty(Text))
            {
                font.DrawString(e.Graphics, Text, 0, 0, textColor);
            }
        }
    }
}