using Guna.UI2.WinForms;
using ScottPlot.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using ClosedXML.Excel;

namespace YugiohShop
{
    public partial class FormStatistics : Form
    {

        private static readonly Color CLR_ACTIVE_BG = Color.FromArgb(65, 105, 225);
        private static readonly Color CLR_ACTIVE_FORE = Color.White;
        private static readonly Color CLR_ACTIVE_BORDER = Color.FromArgb(65, 105, 225);
        private static readonly Color CLR_INACTIVE_BG = Color.FromArgb(245, 247, 250);
        private static readonly Color CLR_INACTIVE_FORE = Color.FromArgb(80, 96, 120);
        private static readonly Color CLR_INACTIVE_BORDER = Color.FromArgb(203, 213, 225);

        private static readonly ScottPlot.Color SP_REVENUE = new ScottPlot.Color(249, 115, 22);
        private static readonly ScottPlot.Color SP_PROFIT = new ScottPlot.Color(16, 185, 129);

        private Guna2Button _activeQuickRange;
        private Guna2Button _activeChartType;
        private Guna2Button _activeSeriesMode;

        private string _chartType = "line";
        private string _seriesMode = "both";
        private FormsPlot _formsPlot;
        private bool _isUpdatingDate = false; 

        public FormStatistics()
        {
            InitializeComponent();
            InitFormsPlot();
            AlignActionButtons();

            SetupTopProductsGrid();

            InitButtonStyles();

            SetQuickRange(btnThangNay);
            SetChartType(btnLine);
            SetSeriesMode(btnBoth);
            UpdateLastUpdated();
            LoadChartData();
        }

        private void AlignActionButtons()
        {
            btnRefreshStatistics.Location = new Point(156, 517);
            btnRefreshStatistics.Size = new Size(125, 36);

            btnExportExcel.Location = new Point(19, 565);
            btnExportExcel.Size = new Size(262, 36);
            btnExportExcel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SetupTopProductsGrid()
        {
            dgvTopProducts.BackgroundColor = Color.White;
            dgvTopProducts.BorderStyle = BorderStyle.None;
            dgvTopProducts.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTopProducts.EnableHeadersVisualStyles = false;
            dgvTopProducts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvTopProducts.RowHeadersVisible = false;
            dgvTopProducts.AllowUserToAddRows = false;
            dgvTopProducts.ReadOnly = true;
            dgvTopProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvTopProducts.ColumnHeadersHeight = 45;
            dgvTopProducts.RowTemplate.Height = 40;

            dgvTopProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(65, 105, 225); // RoyalBlue
            dgvTopProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTopProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Be Vietnam Pro", 10F, FontStyle.Bold);
            dgvTopProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvTopProducts.DefaultCellStyle.Font = new Font("Be Vietnam Pro", 10F, FontStyle.Regular);
            dgvTopProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 242, 255); // Xanh nhạt khi chọn
            dgvTopProducts.DefaultCellStyle.SelectionForeColor = Color.RoyalBlue;
            dgvTopProducts.DefaultCellStyle.BackColor = Color.White;
            dgvTopProducts.DefaultCellStyle.ForeColor = Color.Black;

            dgvTopProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void InitFormsPlot()
        {
            _formsPlot = new FormsPlot { Dock = DockStyle.Fill };
            PanelChart.Controls.Add(_formsPlot);
        }

        private void StyleActive(Guna2Button btn)
        {
            btn.FillColor = Color.FromArgb(65, 105, 225);  // RoyalBlue
            btn.ForeColor = Color.White;
            btn.BorderColor = Color.FromArgb(65, 105, 225);
            btn.BorderThickness = 1;
        }

        private void StyleInactive(Guna2Button btn)
        {
            btn.FillColor = Color.FromArgb(245, 247, 250);
            btn.ForeColor = Color.FromArgb(80, 96, 120);
            btn.BorderColor = Color.FromArgb(203, 213, 225);
            btn.BorderThickness = 1;
        }

        private void StyleChartTypeActive(Guna2Button btn)
        {
            btn.FillColor = Color.RoyalBlue;
            btn.ForeColor = Color.White;
            btn.BorderColor = Color.RoyalBlue;
            btn.BorderThickness = 0;
            btn.Font = new Font(btn.Font, FontStyle.Bold);
        }

        private void StyleChartTypeInactive(Guna2Button btn)
        {
            btn.FillColor = Color.WhiteSmoke;
            btn.ForeColor = Color.Black;
            btn.BorderColor = Color.RoyalBlue;
            btn.BorderThickness = 2;
            btn.Font = new Font(btn.Font, FontStyle.Regular);
        }

        private void InitButtonStyles()
        {
            foreach (var b in new[] { btnHomNay, btnBayNgay, btnThangNay, btnThangTruoc })
                StyleInactive(b);
            foreach (var b in new[] { btnLine, btnBar })
                StyleChartTypeInactive(b);
            foreach (var b in new[] { btnBoth, btnRevenue, btnProfit })
                StyleInactive(b);

            btnRefreshStatistics.FillColor = Color.White;
            btnRefreshStatistics.ForeColor = Color.FromArgb(80, 96, 120);
            btnRefreshStatistics.BorderColor = Color.FromArgb(203, 213, 225);
            btnRefreshStatistics.BorderThickness = 1;
        }

        private void SetQuickRange(Guna2Button active)
        {
            foreach (var b in new[] { btnHomNay, btnBayNgay, btnThangNay, btnThangTruoc })
                StyleInactive(b);
            StyleActive(active);
        }

        private void SetDateRange(DateTime fromDate, DateTime toDate)
        {
            _isUpdatingDate = true; 

            if (fromDate > dtpToDate.Value)
            {
                dtpToDate.Value = toDate;
                dtpFromDate.Value = fromDate;
            }
            else
            {
                dtpFromDate.Value = fromDate;
                dtpToDate.Value = toDate;
            }

            _isUpdatingDate = false; 
            LoadChartData(); 
        }

        private void btnHomNay_Click(object sender, EventArgs e)
        {
            SetDateRange(DateTime.Today, DateTime.Today);
            SetQuickRange(btnHomNay);
        }

        private void btnBayNgay_Click(object sender, EventArgs e)
        {
            SetDateRange(DateTime.Today.AddDays(-6), DateTime.Today);
            SetQuickRange(btnBayNgay);
        }

        private void btnThangNay_Click(object sender, EventArgs e)
        {
            SetDateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
            SetQuickRange(btnThangNay);
        }

        private void btnThangTruoc_Click(object sender, EventArgs e)
        {
            var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            SetDateRange(first, first.AddMonths(1).AddDays(-1));
            SetQuickRange(btnThangTruoc);
        }

        private void SetChartType(Guna2Button active)
        {
            foreach (var b in new[] { btnLine, btnBar })
                StyleChartTypeInactive(b);
            StyleChartTypeActive(active);
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            _chartType = "line";
            SetChartType(btnLine);
            RenderChart();
        }

        private void btnBar_Click(object sender, EventArgs e)
        {
            _chartType = "bar";
            SetChartType(btnBar);
            RenderChart();
        }

        private void SetSeriesMode(Guna2Button active)
        {
            foreach (var b in new[] { btnBoth, btnRevenue, btnProfit })
                StyleInactive(b);
            StyleActive(active);
        }

        private void btnBoth_Click(object sender, EventArgs e)
        {
            _seriesMode = "both";
            SetSeriesMode(btnBoth);
            RenderChart();
        }

        private void btnRevenue_Click(object sender, EventArgs e)
        {
            _seriesMode = "revenue";
            SetSeriesMode(btnRevenue);
            RenderChart();
        }

        private void btnProfit_Click(object sender, EventArgs e)
        {
            _seriesMode = "profit";
            SetSeriesMode(btnProfit);
            RenderChart();
        }

        private void btnRefreshStatistics_Click(object sender, EventArgs e)
        {
            SetDateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
            SetQuickRange(btnThangNay);

            _chartType = "line";
            SetChartType(btnLine);
            _seriesMode = "both";
            SetSeriesMode(btnBoth);

            RenderChart();
        }

        private void UpdateLastUpdated()
        {
            lblLastUpdated.Text = $"Cập nhật: {DateTime.Now:HH:mm dd/MM/yyyy}";
        }

        private void LoadTopProducts()
        {
            try
            {
                using var conn = new Microsoft.Data.SqlClient.SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string sql = @"
                    SELECT TOP 10
                        p.Name AS ProductName,
                        SUM(sd.Qty) AS TotalSold,
                        SUM(sd.LineTotal) AS TotalRevenue
                    FROM SalesInvoices si
                    INNER JOIN SalesDetails sd ON si.SaleId = sd.SaleId
                    INNER JOIN Products p ON sd.ProductId = p.ProductId
                    WHERE CAST(si.SaleDate AS DATE) >= @FromDate
                      AND CAST(si.SaleDate AS DATE) <= @ToDate
                    GROUP BY p.ProductId, p.Name
                    ORDER BY TotalSold DESC";

                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FromDate", dtpFromDate.Value.Date);
                cmd.Parameters.AddWithValue("@ToDate", dtpToDate.Value.Date);

                var dt = new System.Data.DataTable();
                new Microsoft.Data.SqlClient.SqlDataAdapter(cmd).Fill(dt);

                dgvTopProducts.DataSource = dt;

                if (dgvTopProducts.Columns.Contains("ProductName"))
                {
                    dgvTopProducts.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                    dgvTopProducts.Columns["ProductName"].FillWeight = 50;
                }
                if (dgvTopProducts.Columns.Contains("TotalSold"))
                {
                    dgvTopProducts.Columns["TotalSold"].HeaderText = "SL bán";
                    dgvTopProducts.Columns["TotalSold"].FillWeight = 20;
                    dgvTopProducts.Columns["TotalSold"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (dgvTopProducts.Columns.Contains("TotalRevenue"))
                {
                    dgvTopProducts.Columns["TotalRevenue"].HeaderText = "Doanh thu mang lại";
                    dgvTopProducts.Columns["TotalRevenue"].FillWeight = 30;
                    dgvTopProducts.Columns["TotalRevenue"].DefaultCellStyle.Format = "N0";
                    dgvTopProducts.Columns["TotalRevenue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải Top Sản phẩm: " + ex.Message);
            }
        }

        private void LoadChartData()
        {
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.",
                    "Lỗi khoảng thời gian", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RenderChart();
            LoadTopProducts();
            UpdateLastUpdated();
        }

        private void RenderChart()
        {
            _formsPlot.Plot.Clear();

            string f = dtpFromDate.Value.ToString("yyyy-MM-dd");
            string t = dtpToDate.Value.ToString("yyyy-MM-dd");

            // Câu lệnh SQL "thần thánh" lấy doanh thu/lợi nhuận theo từng ngày
            string sql = $@"
                SELECT 
                    CAST(s.SaleDate AS DATE) AS Ngay,
                    SUM(sd.LineTotal) AS DoanhThu,
                    SUM(sd.LineTotal - (sd.Qty * sd.UnitCostPrice)) AS LoiNhuan
                FROM SalesInvoices s
                JOIN SalesDetails sd ON s.SaleId = sd.SaleId
                WHERE CAST(s.SaleDate AS DATE) BETWEEN '{f}' AND '{t}'
                GROUP BY CAST(s.SaleDate AS DATE)
                ORDER BY Ngay";

            var dt = DbHelper.Query(sql);

            if (dt.Rows.Count == 0)
            {
                _formsPlot.Refresh();
                return;
            }

            double[] xs = new double[dt.Rows.Count];
            double[] revenue = new double[dt.Rows.Count];
            double[] profit = new double[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                xs[i] = Convert.ToDateTime(dt.Rows[i]["Ngay"]).ToOADate();
                revenue[i] = Convert.ToDouble(dt.Rows[i]["DoanhThu"]);
                profit[i] = Convert.ToDouble(dt.Rows[i]["LoiNhuan"]);
            }

            bool showRevenue = _seriesMode == "both" || _seriesMode == "revenue";
            bool showProfit = _seriesMode == "both" || _seriesMode == "profit";

            if (_chartType == "line")
            {
                if (showRevenue)
                {
                    var s = _formsPlot.Plot.Add.Scatter(xs, revenue);
                    s.Color = SP_REVENUE; s.LegendText = "Doanh thu";
                }
                if (showProfit)
                {
                    var s = _formsPlot.Plot.Add.Scatter(xs, profit);
                    s.Color = SP_PROFIT; s.LegendText = "Lợi nhuận";
                }
            }
            else 
            {
                double bw = 0.35;
                int count = dt.Rows.Count;

                if (showRevenue && showProfit)
                {
                    double[] xR = new double[count], xP = new double[count];
                    for (int i = 0; i < count; i++) { xR[i] = xs[i] - bw / 2; xP[i] = xs[i] + bw / 2; }

                    var bR = _formsPlot.Plot.Add.Bars(xR, revenue);
                    bR.Color = SP_REVENUE; bR.LegendText = "Doanh thu";
                    foreach (var b in bR.Bars) b.Size = bw;

                    var bP = _formsPlot.Plot.Add.Bars(xP, profit);
                    bP.Color = SP_PROFIT; bP.LegendText = "Lợi nhuận";
                    foreach (var b in bP.Bars) b.Size = bw;
                }
                else if (showRevenue)
                {
                    var bR = _formsPlot.Plot.Add.Bars(xs, revenue);
                    bR.Color = SP_REVENUE; bR.LegendText = "Doanh thu";
                }
                else if (showProfit)
                {
                    var bP = _formsPlot.Plot.Add.Bars(xs, profit);
                    bP.Color = SP_PROFIT; bP.LegendText = "Lợi nhuận";
                }
            }

            _formsPlot.Plot.Axes.DateTimeTicksBottom();
            _formsPlot.Plot.ShowLegend();
            _formsPlot.Refresh();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingDate) return;

            foreach (var b in new[] { btnHomNay, btnBayNgay, btnThangNay, btnThangTruoc }) StyleInactive(b);
            LoadChartData();
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingDate) return;
            foreach (var b in new[] { btnHomNay, btnBayNgay, btnThangNay, btnThangTruoc }) StyleInactive(b);
            LoadChartData();
        }

        private void FormStatistics_Load(object sender, EventArgs e) { }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            DateTime fromDate = dtpFromDate.Value;
            DateTime toDate = dtpToDate.Value;

            string fileName = $"BaoCaoDoanhThu_{fromDate:ddMMyyyy}_{toDate:ddMMyyyy}.xlsx";

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Workbook|*.xlsx";
            sfd.FileName = fileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        var ws1 = workbook.Worksheets.Add("Doanh Thu và Lợi Nhuận");
                        ws1.Cell("A1").Value = "BÁO CÁO DOANH THU VÀ LỢI NHUẬN";
                        ws1.Range("A1:C1").Merge().Style.Font.SetBold().Font.FontSize = 16;
                        ws1.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws1.Cell("A2").Value = $"Từ ngày: {fromDate:dd/MM/yyyy} - Đến ngày: {toDate:dd/MM/yyyy}";
                        ws1.Range("A2:C2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws1.Range("A2:C2").Style.Font.SetItalic();

                        ws1.Cell("A4").Value = "Ngày";
                        ws1.Cell("B4").Value = "Doanh Thu (VNĐ)";
                        ws1.Cell("C4").Value = "Lợi Nhuận (VNĐ)";
                        ws1.Range("A4:C4").Style.Font.Bold = true;
                        ws1.Range("A4:C4").Style.Fill.BackgroundColor = XLColor.RoyalBlue;
                        ws1.Range("A4:C4").Style.Font.FontColor = XLColor.White;

                        string sql = $@"
                            SELECT 
                                CAST(s.SaleDate AS DATE) AS Ngay,
                                SUM(sd.LineTotal) AS DoanhThu,
                                SUM(sd.LineTotal - (sd.Qty * sd.UnitCostPrice)) AS LoiNhuan
                            FROM SalesInvoices s
                            JOIN SalesDetails sd ON s.SaleId = sd.SaleId
                            WHERE CAST(s.SaleDate AS DATE) BETWEEN '{fromDate:yyyy-MM-dd}' AND '{toDate:yyyy-MM-dd}'
                            GROUP BY CAST(s.SaleDate AS DATE)
                            ORDER BY Ngay";

                        var dtData = DbHelper.Query(sql);

                        int rowIdx = 5;
                        foreach (System.Data.DataRow row in dtData.Rows)
                        {
                            ws1.Cell(rowIdx, 1).Value = Convert.ToDateTime(row["Ngay"]).ToString("dd/MM/yyyy");
                            ws1.Cell(rowIdx, 2).Value = Convert.ToDecimal(row["DoanhThu"]);
                            ws1.Cell(rowIdx, 3).Value = Convert.ToDecimal(row["LoiNhuan"]);
                            ws1.Cell(rowIdx, 2).Style.NumberFormat.Format = "#,##0";
                            ws1.Cell(rowIdx, 3).Style.NumberFormat.Format = "#,##0";
                            rowIdx++;
                        }
                        ws1.Columns().AdjustToContents();

                        if (dgvTopProducts.Rows.Count > 0)
                        {
                            var ws2 = workbook.Worksheets.Add("Top Sản Phẩm");
                            ws2.Cell("A1").Value = "TOP SẢN PHẨM BÁN CHẠY";
                            ws2.Range("A1:C1").Merge().Style.Font.SetBold().Font.FontSize = 16;
                            ws2.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            ws2.Cell("A2").Value = $"Từ ngày: {fromDate:dd/MM/yyyy} - Đến ngày: {toDate:dd/MM/yyyy}";
                            ws2.Range("A2:C2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws2.Range("A2:C2").Style.Font.SetItalic();

                            for (int i = 0; i < dgvTopProducts.Columns.Count; i++)
                            {
                                ws2.Cell(4, i + 1).Value = dgvTopProducts.Columns[i].HeaderText;
                                ws2.Cell(4, i + 1).Style.Font.Bold = true;
                                ws2.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.ForestGreen;
                                ws2.Cell(4, i + 1).Style.Font.FontColor = XLColor.White;
                            }

                            for (int i = 0; i < dgvTopProducts.Rows.Count; i++)
                            {
                                for (int j = 0; j < dgvTopProducts.Columns.Count; j++)
                                {
                                    ws2.Cell(i + 5, j + 1).Value = dgvTopProducts.Rows[i].Cells[j].Value?.ToString() ?? "";
                                }
                            }
                            ws2.Columns().AdjustToContents();
                        }

                        workbook.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi");
                }
            }
        }


    }
}