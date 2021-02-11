using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DataHelper.DataGridViewSummary
{
    public partial class ReadOnlyTextBox : Control
    {
        private StringFormat format;
        public bool IsSummary { get; set; }
        public bool IsLastColumn { get; set; }
        public string FormatString { get; set; }
        public Color BorderColor { get; set; } = Color.Black;

        public ReadOnlyTextBox()
        {
            InitializeComponent();

            format = new StringFormat( StringFormatFlags.NoWrap  | StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces);
            format.LineAlignment = StringAlignment.Center;

            this.Height = 10;
            this.Width = 10;

            this.Padding = new Padding(2);
        }

        public ReadOnlyTextBox(IContainer container)
        {
            container.Add(this);
            InitializeComponent();

            this.TextChanged += new EventHandler(ReadOnlyTextBox_TextChanged);
        }

        private void ReadOnlyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FormatString) && !string.IsNullOrEmpty(Text))
            {
                Text = string.Format(FormatString, Text);
            }
        }


        private HorizontalAlignment textAlign = HorizontalAlignment.Left;
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TextAlign
        {
            get { return textAlign; }
            set 
            {
                textAlign = value;
                setFormatFlags();
            }
        }

        private StringTrimming trimming = StringTrimming.None;
        [DefaultValue(StringTrimming.None)]
        public StringTrimming Trimming
        {
            get { return trimming; }
            set
            {
                trimming = value;
                setFormatFlags();
            }
        }

        private void setFormatFlags()
        {
            format.Alignment = TextHelper.TranslateAligment(TextAlign);
            format.Trimming = trimming;                
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int subWidth = 0;
            Rectangle textBounds;

            if (!string.IsNullOrEmpty(FormatString) && !string.IsNullOrEmpty(Text))
            {
                Text = String.Format("{0:" + FormatString + "}", Convert.ToDecimal(Text));
            }

            textBounds = new Rectangle(this.ClientRectangle.X + 2, this.ClientRectangle.Y + 2, this.ClientRectangle.Width - 2 , this.ClientRectangle.Height - 2 );
            using (Pen pen = new Pen(BorderColor))
            {
                if (IsLastColumn)
                    subWidth = 1;

                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
                e.Graphics.DrawRectangle(pen, this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - subWidth , this.ClientRectangle.Height - 1);             
                e.Graphics.DrawString(Text, Font, Brushes.Black, textBounds , format );
            }
        }
    }
}


