public partial class ExampleForm : Form
{
    private CustomFont customFont;
    private TextBox textBox1;
    private Button convertButton;
    private Button exportButton;
    private Button clipboardButton;
    private Panel panel1;
    private CheckBox showMessageBox;
    private NumericUpDown thicknessNumeric;
    public double Thickness { get; set; } = 1.0;
    public ExampleForm()
    {
        InitializeComponent();
        this.BackColor = SystemColors.Control;
        InitializeControls();
        customFont = new CustomFont("NewFont.png", "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~", 18)
        {
            Thickness = 1
        };
    }

    private void InitializeControls()
    {
        // TextBox for input
        textBox1 = new TextBox
        {
            Location = new Point(10, 10),
            Size = new Size(300, 25),
            Font = new Font("Arial", 12)
        };
        Controls.Add(textBox1);

        // Convert Button
        convertButton = new Button
        {
            Location = new Point(320, 10),
            Size = new Size(80, 25),
            Text = "Convert"
        };
        convertButton.Click += ConvertButton_Click;
        Controls.Add(convertButton);

        // Export Button
        exportButton = new Button
        {
            Location = new Point(410, 10),
            Size = new Size(80, 25),
            Text = "Export",
            Enabled = false
        };
        exportButton.Click += ExportButton_Click;
        Controls.Add(exportButton);

        // Thickness Controls
           var thicknessLabel = new Label
        {
            Location = new Point(10, 45),
            Size = new Size(65, 20),
            Text = "Thickness:"
        };
        Controls.Add(thicknessLabel);

        thicknessNumeric = new NumericUpDown
        {
            Location = new Point(75, 45),
            Size = new Size(60, 20),
            Minimum = 0.1M,
            Maximum = 5M,
            Value = 1M,
            DecimalPlaces = 1,
            Increment = 0.1M
        };
        thicknessNumeric.ValueChanged += ThicknessNumeric_ValueChanged;
        Controls.Add(thicknessNumeric);

 
  var scaleLabel = new Label
    {
        Location = new Point(150, 45),
        Size = new Size(45, 20),
        Text = "Scale:"
    };
    Controls.Add(scaleLabel);

    var scaleNumeric = new NumericUpDown
    {
        Location = new Point(195, 45),
        Size = new Size(60, 20),
        Minimum = 0.1M,
        Maximum = 10M,
        Value = 1M,
        DecimalPlaces = 1,
        Increment = 0.1M
    };
    scaleNumeric.ValueChanged += (s, e) =>
    {
        if (customFont != null)
        {
            customFont.Scale = (float)scaleNumeric.Value;
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                ConvertButton_Click(null, EventArgs.Empty);
            }
        }
    };
    Controls.Add(scaleNumeric);

    // Move checkbox to the right of scale control
    showMessageBox = new CheckBox
    {
        Location = new Point(270, 45),
        Size = new Size(120, 20),
        Text = "Show Success",
        Checked = true
    };
    Controls.Add(showMessageBox);

        // Clipboard Button
        clipboardButton = new Button
        {
            Location = new Point(410, 45),
            Size = new Size(80, 25),
            Text = "Copy",
            Enabled = false
        };
        clipboardButton.Click += ClipboardButton_Click;
        Controls.Add(clipboardButton);

        // Preview Panel
        panel1 = new Panel
        {
            Location = new Point(10, 80),
            Size = new Size(480, 200),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.Transparent
        };
        Controls.Add(panel1);

        this.ClientSize = new Size(500, 290);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
    }

  private void ThicknessNumeric_ValueChanged(object sender, EventArgs e)
    {
        if (customFont != null)
        {
            customFont.Thickness = (int)thicknessNumeric.Value;
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                ConvertButton_Click(null, EventArgs.Empty);
            }
        }
    }
    private void ConvertButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(textBox1.Text)) return;

            using (Bitmap bmp = new Bitmap(panel1.Width, panel1.Height))
            {
                bmp.MakeTransparent();
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    customFont.DrawString(g, textBox1.Text, 0, 0, Color.Black);
                }
                
                panel1.BackgroundImage?.Dispose();
                panel1.BackgroundImage = new Bitmap(bmp);
            }
            
            exportButton.Enabled = true;
            clipboardButton.Enabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private Bitmap GetCroppedImage()
    {
        if (panel1.BackgroundImage == null) return null;

        using (Bitmap sourceBmp = (Bitmap)panel1.BackgroundImage)
        {
            Rectangle bounds = GetImageBounds(sourceBmp);
            if (bounds.IsEmpty) return null;

            Bitmap croppedBmp = new Bitmap(bounds.Width, bounds.Height);
            croppedBmp.MakeTransparent();
            
            using (Graphics g = Graphics.FromImage(croppedBmp))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(sourceBmp, 
                    new Rectangle(0, 0, bounds.Width, bounds.Height),
                    bounds,
                    GraphicsUnit.Pixel);
            }
            
            return croppedBmp;
        }
    }

   private void ClipboardButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(textBox1.Text)) return;

            using (Bitmap bmp = new Bitmap(panel1.Width, panel1.Height))
            {
                bmp.MakeTransparent();
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    customFont.DrawString(g, textBox1.Text, 0, 0, Color.Black);
                }

                Rectangle bounds = GetImageBounds(bmp);
                if (!bounds.IsEmpty)
                {
                    using (Bitmap croppedBmp = new Bitmap(bounds.Width, bounds.Height))
                    {
                        croppedBmp.MakeTransparent();
                        using (Graphics g = Graphics.FromImage(croppedBmp))
                        {
                            g.Clear(Color.Transparent);
                            g.DrawImage(bmp, 
                                new Rectangle(0, 0, bounds.Width, bounds.Height),
                                bounds,
                                GraphicsUnit.Pixel);
                        }
                        Clipboard.SetImage(croppedBmp);
                        if (showMessageBox.Checked)
                        {
                            MessageBox.Show("Copied to clipboard!");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void ExportButton_Click(object sender, EventArgs e)
    {
        try
        {
            using (Bitmap croppedBmp = GetCroppedImage())
            {
                if (croppedBmp != null)
                {
                    using (SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "PNG Image|*.png",
                        Title = "Save Image"
                    })
                    {
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            croppedBmp.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            if (showMessageBox.Checked)
                            {
                                MessageBox.Show("Saved successfully!");
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private Rectangle GetImageBounds(Bitmap bmp)
    {
        int minX = bmp.Width, minY = bmp.Height, maxX = 0, maxY = 0;

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixel = bmp.GetPixel(x, y);
                if (pixel.A > 0)
                {
                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }
        }

        return minX > maxX ? Rectangle.Empty 
            : new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        panel1.BackgroundImage?.Dispose();
        
    }
}