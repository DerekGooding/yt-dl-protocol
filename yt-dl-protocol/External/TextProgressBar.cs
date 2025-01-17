using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ProgressBarSample;

public enum ProgressBarDisplayMode
{
    NoText,
    Percentage,
    CurrProgress,
    CustomText,
    TextAndPercentage,
    TextAndCurrProgress
}

public class TextProgressBar : ProgressBar
{
    [Description("Font of the text on ProgressBar"), Category("Additional Options")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Font TextFont { get; set; } = new Font(FontFamily.GenericSerif, 11, FontStyle.Bold|FontStyle.Italic);
    
    private SolidBrush _textColourBrush = (SolidBrush) Brushes.Black;
    [Category("Additional Options")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color TextColor {
        get {
            return _textColourBrush.Color;
        }
        set
        {
            _textColourBrush.Dispose();
            _textColourBrush = new SolidBrush(value);
        }
    }

    private SolidBrush _progressColourBrush = (SolidBrush) Brushes.LightGreen;
    [Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color ProgressColor
    {
        get
        {
            return _progressColourBrush.Color;
        }
        set
        {
            _progressColourBrush.Dispose();
            _progressColourBrush = new SolidBrush(value);
        }
    }

    private ProgressBarDisplayMode _visualMode = ProgressBarDisplayMode.CurrProgress;
    [Category("Additional Options"), Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ProgressBarDisplayMode VisualMode {
        get {
            return _visualMode;
        }
        set
        {
            _visualMode = value;
            Invalidate();//redraw component after change value from VS Properties section
        }
    }

    private string _text = string.Empty;

    [Description("If it's empty, % will be shown"), Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string CustomText
    {
        get => _text;
        set
        {
            _text = value;
            Invalidate();//redraw component after change value from VS Properties section
        }
    }

    private string TextToDraw
    {
        get
        {
            string text = CustomText;

            switch (VisualMode)
            {
                case (ProgressBarDisplayMode.Percentage):
                    text = PercentageStr;
                    break;
                case (ProgressBarDisplayMode.CurrProgress):
                    text = CurrProgressStr;
                    break;
                case (ProgressBarDisplayMode.TextAndCurrProgress):
                    text = $"{CustomText}: {CurrProgressStr}";
                    break;
                case (ProgressBarDisplayMode.TextAndPercentage):
                    text = $"{CustomText}: {PercentageStr}";
                    break;
                case ProgressBarDisplayMode.NoText:
                    break;
                case ProgressBarDisplayMode.CustomText:
                    break;
            }

            return text;
        }
        set { }
    }

    private string PercentageStr => $"{(int)((float)Value - Minimum) / ((float)Maximum - Minimum) * 100} %";

    private string CurrProgressStr => $"{Value}/{Maximum}";

    public TextProgressBar()
    {
        Value = Minimum;
        FixComponentBlinking();
    }

    private void FixComponentBlinking() => SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        
        DrawProgressBar(g);

        DrawStringIfNeeded(g);
    }

    private void DrawProgressBar(Graphics g)
    {
        Rectangle rect = ClientRectangle;

        ProgressBarRenderer.DrawHorizontalBar(g, rect);

        rect.Inflate(-3, -3);

        if (Value > 0)
        {
            Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);

            g.FillRectangle(_progressColourBrush, clip);
        }
    }

    private void DrawStringIfNeeded(Graphics g)
    {
        if (VisualMode != ProgressBarDisplayMode.NoText)
        {
            
            string text = TextToDraw;

            SizeF len = g.MeasureString(text, TextFont);

            Point location = new Point(((Width / 2) - (int)len.Width / 2), ((Height / 2) - (int)len.Height / 2));
            
            g.DrawString(text, TextFont, (Brush)_textColourBrush, location);
        }
    }
    
    public new void Dispose()
    {
        _textColourBrush.Dispose();
        _progressColourBrush.Dispose();
        base.Dispose();
    }
}

