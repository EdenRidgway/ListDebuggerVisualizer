using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ListDebuggerVisualizer;

namespace DataHelper.DataGridViewSummary
{

    public class SummaryControlContainer : Control
    {

        #region Declare variables

        private Hashtable sumBoxHash;
        private DataGridViewSummary dgv;
        private Label sumRowHeaderLabel;
        private bool lastVisibleState;

        public int InitialHeight { get; set; }
        public bool LastVisibleState { get; set; }
        public Color SummaryRowBackColor { get; set; }

        /// <summary>
        /// Event is raised when visibility changes and the
        /// lastVisibleState is not the new visible state
        /// </summary>
        public event EventHandler VisibilityChanged;

        #endregion

        #region Constructors

        public SummaryControlContainer(DataGridViewSummary dgv)
        {
            if (dgv == null)
                throw new Exception("DataGridView is null!");

            this.dgv = dgv;

            sumBoxHash = new Hashtable();
            sumRowHeaderLabel = new Label();


            this.dgv.CreateSummary += new EventHandler(dgv_CreateSummary);
            this.dgv.RowsAdded += new DataGridViewRowsAddedEventHandler(dgv_RowsAdded);
            this.dgv.RowsRemoved += new DataGridViewRowsRemovedEventHandler(dgv_RowsRemoved);
            this.dgv.CellValueChanged += new DataGridViewCellEventHandler(dgv_CellValueChanged);

            this.dgv.Scroll += new ScrollEventHandler(dgv_Scroll);
            this.dgv.ColumnWidthChanged += new DataGridViewColumnEventHandler(dgv_ColumnWidthChanged);
            this.dgv.RowHeadersWidthChanged += new EventHandler(dgv_RowHeadersWidthChanged);
            this.VisibleChanged += new EventHandler(SummaryControlContainer_VisibleChanged);

            this.dgv.ColumnAdded += new DataGridViewColumnEventHandler(dgv_ColumnAdded);
            this.dgv.ColumnRemoved += new DataGridViewColumnEventHandler(dgv_ColumnRemoved);
            this.dgv.ColumnStateChanged += new DataGridViewColumnStateChangedEventHandler(dgv_ColumnStateChanged);
            this.dgv.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler(dgv_ColumnDisplayIndexChanged);

        }

        private void dgv_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            //resizeSumBoxes();
            RecreateSumBoxes();
        }

        private void dgv_ColumnStateChanged(object sender, DataGridViewColumnStateChangedEventArgs e)
        {
            ResizeSumBoxes();
        }

        private void dgv_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            RecreateSumBoxes();
        }

        private void dgv_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            RecreateSumBoxes();
        }

        private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            ReadOnlyTextBox roTextBox = (ReadOnlyTextBox)sumBoxHash[dgv.Columns[e.ColumnIndex]];
            if (roTextBox != null)
            {
                if (roTextBox.IsSummary)
                    CalculateSummaries();
            }
        }

        private void dgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CalculateSummaries();
        }

        private void dgv_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateSummaries();
        }

        private void SummaryControlContainer_VisibleChanged(object sender, EventArgs e)
        {
            if (lastVisibleState != this.Visible)
            {
                OnVisiblityChanged(sender, e);
            }
        }

        protected void OnVisiblityChanged(object sender, EventArgs e)
        {
            if (VisibilityChanged != null)
                VisibilityChanged(sender, e);

            lastVisibleState = this.Visible;
        }

        #endregion

        #region Events and delegates

        private void dgv_CreateSummary(object sender, EventArgs e)
        {
            RecreateSumBoxes();
            CalculateSummaries();
        }

        private void dgv_Scroll(object sender, ScrollEventArgs e)
        {
            ResizeSumBoxes();
        }

        private void dgv_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ResizeSumBoxes();
        }

        private void dgv_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            ResizeSumBoxes();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResizeSumBoxes();
        }

        private void dgv_Resize(object sender, EventArgs e)
        {
            ResizeSumBoxes();
        }

        #endregion

        #region Functions

        /// <summary>
        /// Checks if passed object is of type of integer
        /// </summary>
        /// <param name="o">object</param>
        /// <returns>true/ false</returns>
        protected bool IsInteger(object o)
        {
            if (o is Int64)
                return true;
            if (o is Int32)
                return true;
            if (o is Int16)
                return true;
            return false;
        }

        /// <summary>
        /// Checks if passed object is of type of decimal/ double
        /// </summary>
        /// <param name="o">object</param>
        /// <returns>true/ false</returns>
        protected bool IsDecimal(object o)
        {
            if (o is Decimal)
                return true;
            if (o is Single)
                return true;
            if (o is Double)
                return true;
            return false;
        }

        /// <summary>
        /// Enable manual refresh of the SummaryDataGridView
        /// </summary>
        internal void RefreshSummary()
        {
            CalculateSummaries();
        }

        /// <summary>
        /// Calculate the Sums of the summary columns
        /// </summary>
        private void CalculateSummaries()
        {
            foreach (ReadOnlyTextBox roTextBox in sumBoxHash.Values)
            {
                if (roTextBox.IsSummary)
                {
                    roTextBox.Tag = 0;
                    roTextBox.Text = "0";
                    roTextBox.Invalidate();
                }
            }

            if (dgv.SummaryColumns == null || dgv.SummaryColumns.Length == 0 || sumBoxHash.Count == 0)
            {
                return;
            }

            foreach (DataGridViewRow dgvRow in dgv.Rows)
            {
                foreach (DataGridViewCell dgvCell in dgvRow.Cells)
                {
                    foreach (DataGridViewColumn dgvColumn in sumBoxHash.Keys)
                    {
                        if (dgvCell.OwningColumn.Equals(dgvColumn))
                        {
                            ReadOnlyTextBox sumBox = (ReadOnlyTextBox)sumBoxHash[dgvColumn];

                            if (sumBox != null && sumBox.IsSummary)
                            {
                                object value = dgvCell.Value;

                                if (value != null && !(value is DBNull))
                                {
                                    JToken jToken = value as JToken;

                                    if (jToken != null)
                                    {
                                        value = jToken.GetValue();
                                    }

                                    if (IsInteger(value))
                                    {
                                        sumBox.Tag = Convert.ToInt64(sumBox.Tag) + Convert.ToInt64(value);
                                    }
                                    else if (IsDecimal(value))
                                    {
                                        sumBox.Tag = Convert.ToDecimal(sumBox.Tag) + Convert.ToDecimal(value);
                                    }

                                    sumBox.Text = string.Format("{0}", sumBox.Tag);
                                    sumBox.Invalidate();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create summary boxes for each Column of the DataGridView        
        /// </summary>
        private void RecreateSumBoxes()
        {
            ReadOnlyTextBox sumBox;

            foreach (Control control in sumBoxHash.Values)
            {
                this.Controls.Remove(control);
            }
            sumBoxHash.Clear();

            int iCnt = 0;

            List<DataGridViewColumn> sortedColumns = SortedColumns;
            foreach (DataGridViewColumn dgvColumn in sortedColumns)
            {
                sumBox = new ReadOnlyTextBox();
                sumBoxHash.Add(dgvColumn, sumBox);

                sumBox.Top = 0;
                sumBox.Height = dgv.RowTemplate.Height;
                sumBox.BorderColor = dgv.GridColor;

                if (SummaryRowBackColor == null)
                    sumBox.BackColor = dgv.DefaultCellStyle.BackColor;
                else
                    sumBox.BackColor = SummaryRowBackColor;

                sumBox.BringToFront();

                if (dgv.ColumnCount - iCnt == 1)
                    sumBox.IsLastColumn = true;

                if (dgv.SummaryColumns != null && dgv.SummaryColumns.Length > 0)
                {
                    for (int iCntX = 0; iCntX < dgv.SummaryColumns.Length; iCntX++)
                    {
                        if (dgv.SummaryColumns[iCntX] == dgvColumn.DataPropertyName || dgv.SummaryColumns[iCntX] == dgvColumn.Name)
                        {
                            dgvColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                            dgvColumn.CellTemplate.Style.Format = dgv.FormatString;
                            sumBox.TextAlign = TextHelper.TranslateGridColumnAligment(dgvColumn.DefaultCellStyle.Alignment);
                            sumBox.IsSummary = true;

                            sumBox.FormatString = dgvColumn.CellTemplate.Style.Format;

                            if (dgvColumn.ValueType == typeof(Int32) || dgvColumn.ValueType == typeof(Int16) ||
                                dgvColumn.ValueType == typeof(Int64) || dgvColumn.ValueType == typeof(Single) ||
                                dgvColumn.ValueType == typeof(Double) || dgvColumn.ValueType == typeof(Single) ||
                                dgvColumn.ValueType == typeof(Decimal))
                            {
                                sumBox.Tag = Activator.CreateInstance(dgvColumn.ValueType);
                            }
                        }
                    }
                }

                try
                {
                    sumBox.BringToFront();
                    this.Controls.Add(sumBox);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error adding summary box: " + ex.Message);
                }

                iCnt++;
            }

            if (dgv.DisplaySumRowHeader)
            {
                sumRowHeaderLabel.Font = new Font(dgv.DefaultCellStyle.Font, dgv.SumRowHeaderTextBold ? FontStyle.Bold : FontStyle.Regular);
                sumRowHeaderLabel.Anchor = AnchorStyles.Left;
                sumRowHeaderLabel.TextAlign = ContentAlignment.MiddleLeft;
                sumRowHeaderLabel.Height = sumRowHeaderLabel.Font.Height;
                sumRowHeaderLabel.Top = Convert.ToInt32(Convert.ToDouble(this.InitialHeight - sumRowHeaderLabel.Height) / 2F);
                sumRowHeaderLabel.Text = dgv.SumRowHeaderText;

                sumRowHeaderLabel.ForeColor = dgv.DefaultCellStyle.ForeColor;
                sumRowHeaderLabel.Margin = new Padding(0);
                sumRowHeaderLabel.Padding = new Padding(0);

                this.Controls.Add(sumRowHeaderLabel);
            }

            CalculateSummaries();
            ResizeSumBoxes();
        }

        /// <summary>
        /// Order the columns in the way they are displayed
        /// </summary>
        private List<DataGridViewColumn> SortedColumns
        {
            get
            {
                List<DataGridViewColumn> result = new List<DataGridViewColumn>();
                DataGridViewColumn column = dgv.Columns.GetFirstColumn(DataGridViewElementStates.None);
                if (column == null)
                    return result;

                result.Add(column);

                while ((column = dgv.Columns.GetNextColumn(column, DataGridViewElementStates.None, DataGridViewElementStates.None)) != null)
                    result.Add(column);

                return result;
            }
        }

        /// <summary>
        /// Resize the summary Boxes depending on the 
        /// width of the Columns of the DataGridView
        /// </summary>
        private void ResizeSumBoxes()
        {
            this.SuspendLayout();

            if (sumBoxHash.Count == 0) return;

            try
            {
                int rowHeaderWidth = dgv.RowHeadersVisible ? dgv.RowHeadersWidth - 1 : 0;
                int sumLabelWidth = dgv.RowHeadersVisible ? dgv.RowHeadersWidth - 1 : 0;
                int curPos = rowHeaderWidth;

                if (dgv.DisplaySumRowHeader && sumLabelWidth > 0)
                {
                    if (dgv.RightToLeft == RightToLeft.Yes)
                    {
                        if (sumRowHeaderLabel.Dock != DockStyle.Right)
                            sumRowHeaderLabel.Dock = DockStyle.Right;
                    }
                    else
                    {
                        if (sumRowHeaderLabel.Dock != DockStyle.Left)
                            sumRowHeaderLabel.Dock = DockStyle.Left;

                    }
                }
                else
                {
                    if (sumRowHeaderLabel.Visible)
                        sumRowHeaderLabel.Visible = false;
                }

                int iCnt = 0;
                Rectangle oldBounds;

                foreach (DataGridViewColumn dgvColumn in SortedColumns) //dgv.Columns)
                {
                    ReadOnlyTextBox sumBox = (ReadOnlyTextBox)sumBoxHash[dgvColumn];

                    try
                    {
                        if (sumBox == null) continue;

                        oldBounds = sumBox.Bounds;
                        if (!dgvColumn.Visible)
                        {
                            sumBox.Visible = false;
                            continue;
                        }

                        int from = curPos - dgv.HorizontalScrollingOffset;

                        int width = dgvColumn.Width + (iCnt == 0 ? 0 : 0);

                        if (from < rowHeaderWidth)
                        {
                            width -= rowHeaderWidth - from;
                            from = rowHeaderWidth;
                        }

                        if (from + width > this.Width)
                            width = this.Width - from;

                        if (width < 4)
                        {
                            if (sumBox.Visible)
                                sumBox.Visible = false;
                        }
                        else
                        {
                            if (this.RightToLeft == RightToLeft.Yes)
                                from = this.Width - from - width;

                            if (sumBox.Left != from || sumBox.Width != width)
                                sumBox.SetBounds(from, 0, width, 0, BoundsSpecified.X | BoundsSpecified.Width);

                            if (!sumBox.Visible)
                                sumBox.Visible = true;
                        }

                        curPos += dgvColumn.Width + (iCnt == 0 ? 0 : 0);

                        if (oldBounds != sumBox.Bounds)
                            sumBox.Invalidate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to make summary row visible");
                    }

                    iCnt++;
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        #endregion
    }
}
