using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SPColor = ScottPlot.Color;
using WinColor = System.Drawing.Color;
using ScottPlot.WinForms;

namespace YugiohShop
{
    public partial class FormDashboard : Form
    {

        private DateTime _fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private DateTime _toDate = DateTime.Today;
        private FormsPlot _formsPlot;

        public FormDashboard()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;          
            this.AutoScaleMode = AutoScaleMode.None;
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            SetupChart();
            LoadDashboard();
        }

        public void OnDateRangeChanged(string range)
        {
            switch (range)
            {
                case "Hôm nay":
                    _fromDate = DateTime.Today; _toDate = DateTime.Today; break;
                case "7 ngày":
                    _fromDate = DateTime.Today.AddDays(-6); _toDate = DateTime.Today; break;
                case "Tháng này":
                    _fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    _toDate = DateTime.Today; break;
                case "Tháng trước":
                    var f = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    _fromDate = f; _toDate = f.AddMonths(1).AddDays(-1); break;
            }
            LoadDashboard();
        }

        private (DateTime, DateTime) GetDateRange() => (_fromDate, _toDate);

        private void LoadDashboard()
        {
            var (from, to) = GetDateRange();
            try
            {
                LoadKpiCards(from, to);
                LoadTopProducts(from, to);
                LoadRevenueProfitStats(from, to);
                LoadRecentOrders(from, to);
                LoadLowStock();

                RenderChart(from, to);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dashboard: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadKpiCards(DateTime from, DateTime to)
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            string f = from.ToString("yyyy-MM-dd");
            string t = to.ToString("yyyy-MM-dd");
            string yy = yesterday.ToString("yyyy-MM-dd");

            decimal QueryDecimal(string sql) =>
                Convert.ToDecimal(DbHelper.Query(sql).Rows[0][0] ?? 0);
            int QueryInt(string sql) =>
                Convert.ToInt32(DbHelper.Query(sql).Rows[0][0] ?? 0);

            decimal revenue = QueryDecimal($"SELECT ISNULL(SUM(Total),0) FROM SalesInvoices WHERE CAST(SaleDate AS DATE) BETWEEN '{f}' AND '{t}'");
            decimal revYest = QueryDecimal($"SELECT ISNULL(SUM(Total),0) FROM SalesInvoices WHERE CAST(SaleDate AS DATE) = '{yy}'");

            int orders = QueryInt($"SELECT COUNT(*) FROM SalesInvoices WHERE CAST(SaleDate AS DATE) BETWEEN '{f}' AND '{t}'");
            int ordersYest = QueryInt($"SELECT COUNT(*) FROM SalesInvoices WHERE CAST(SaleDate AS DATE) = '{yy}'");

            int stock = QueryInt("SELECT ISNULL(SUM(Stock),0) FROM Products WHERE IsActive = 1");

            int newCust = QueryInt($"SELECT COUNT(*) FROM Customers WHERE CAST(CreatedAt AS DATE) BETWEEN '{f}' AND '{t}'");
            int newCustYest = QueryInt($"SELECT COUNT(*) FROM Customers WHERE CAST(CreatedAt AS DATE) = '{yy}'");

            lblRevenueTodayValue.Text = revenue.ToString("N0") + " ₫";
            lblTotalOrdersValue.Text = orders.ToString("N0");
            lblStockQuantityValue.Text = stock.ToString("N0");
            lblNewCustomersValue.Text = newCust.ToString("N0");

            SetTrendBadge(pnlRevenueTrend, lblRevenueTrend, revenue, revYest);
            SetTrendBadge(pnlOrdersTrend, lblOrdersTrend, orders, ordersYest);
            SetTrendBadge(pnlStockTrend, lblStockTrend, stock, stock);  
            SetTrendBadge(pnlCustomersTrend, lblCustomersTrend, newCust, newCustYest);
        }

        private void SetTrendBadge(Guna.UI2.WinForms.Guna2Panel pnlBadge, Control lblBadge,
                                            decimal today, decimal yesterday)
        {
            if (yesterday == 0 && today == 0)
            {
                pnlBadge.Visible = false;
                return;
            }

            pnlBadge.Visible = true;

            double pct = 0;
            if (yesterday == 0 && today > 0)
            {
                pct = 100; 
            }
            else
            {
                pct = (double)((today - yesterday) / yesterday * 100);
            }

            bool up = pct >= 0;

            pnlBadge.FillColor = up ? Color.FromArgb(220, 252, 231) : Color.FromArgb(254, 226, 226);
            pnlBadge.CustomBorderColor = Color.Transparent;
            pnlBadge.BorderRadius = 10;

            lblBadge.Parent = pnlBadge;      
            lblBadge.BringToFront();         
            lblBadge.BackColor = Color.Transparent;
            lblBadge.ForeColor = up ? Color.FromArgb(21, 128, 61) : Color.FromArgb(185, 28, 28);

            string arrow = up ? "▲" : "▼";
            lblBadge.Text = $"{arrow} {Math.Abs(pct):F1}% so với hôm qua";

            lblBadge.Dock = DockStyle.None;
            int x = 10;
            int y = (pnlBadge.Height - lblBadge.Height) / 2;
            lblBadge.Location = new Point(x, y + 1);
        }

        private void LoadTopProducts(DateTime from, DateTime to)
        {
            string f = from.ToString("yyyy-MM-dd");
            string t = to.ToString("yyyy-MM-dd");

            string sql = $@"
                        SELECT TOP 5
                            p.Name AS ProductName,
                            SUM(sd.Qty)                        AS TotalSold,
                            SUM(sd.Qty * sd.UnitSellPrice)     AS Revenue
                        FROM SalesDetails sd
                        JOIN Products p      ON sd.ProductId = p.ProductId
                        JOIN SalesInvoices s ON sd.SaleId    = s.SaleId
                        WHERE CAST(s.SaleDate AS DATE) BETWEEN '{f}' AND '{t}'
                        GROUP BY p.Name
                        ORDER BY TotalSold DESC";

            var dt = DbHelper.Query(sql);

            flpTopProducts.Controls.Clear();
            flpTopProducts.AutoScroll = true;

            flpTopProducts.WrapContents = false;
            flpTopProducts.FlowDirection = FlowDirection.TopDown;

            int rank = 1;
            foreach (DataRow row in dt.Rows)
            {
                var item = CreateTopProductItem(
                    rank,
                    row["ProductName"].ToString(),
                    Convert.ToInt32(row["TotalSold"]),
                    Convert.ToDecimal(row["Revenue"]));

                item.Width = Math.Max(200, flpTopProducts.ClientSize.Width - 20);
                flpTopProducts.Controls.Add(item);
                rank++;
            }
        }

        private Panel CreateTopProductItem(int rank, string name, int qty, decimal revenue)
        {
            var font = new Font("Be Vietnam Pro", 9f);

            var pnl = new Panel
            {
                Height = 48,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 4),
                Padding = new Padding(0),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            pnl.Width = Math.Max(200, flpTopProducts.ClientSize.Width - 20);

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(4, 0, 4, 0)
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32));  
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));   
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));  

            var lblRank = new Label
            {
                Text = rank.ToString(),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Be Vietnam Pro", 10f, FontStyle.Bold),
                ForeColor = rank == 1 ? Color.FromArgb(202, 138, 4)
                          : rank == 2 ? Color.FromArgb(148, 163, 184)
                          : rank == 3 ? Color.FromArgb(180, 83, 9)
                          : Color.FromArgb(148, 163, 184)
            };
            tbl.SetRowSpan(lblRank, 2);
            tbl.Controls.Add(lblRank, 0, 0);

            var lblName = new Label
            {
                Text = name,
                Dock = DockStyle.Fill,
                Font = new Font("Be Vietnam Pro", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(0)
            };
            tbl.Controls.Add(lblName, 1, 0);

            var lblQty = new Label
            {
                Text = $"Đã bán: {qty}",
                Dock = DockStyle.Fill,
                Font = new Font("Be Vietnam Pro", 8f),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.TopLeft
            };
            tbl.Controls.Add(lblQty, 1, 1);

            var lblRev = new Label
            {
                Text = FormatMoney(revenue),
                Dock = DockStyle.Fill,
                Font = new Font("Be Vietnam Pro", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                TextAlign = ContentAlignment.MiddleRight
            };
            tbl.SetRowSpan(lblRev, 2);
            tbl.Controls.Add(lblRev, 2, 0);

            pnl.Controls.Add(tbl);
            return pnl;
        }

        private void LoadRecentOrders(DateTime from, DateTime to)
        {
            string f = from.ToString("yyyy-MM-dd");
            string t = to.ToString("yyyy-MM-dd");

            string sql = @"
                        SELECT TOP 10
                            s.SaleId,
                            ISNULL(c.Name, N'Khách lẻ')   AS KhachHang,
                            s.SaleDate,
                            s.Total,
                            STRING_AGG(p.Name + ' x' + CAST(sd.Qty AS NVARCHAR(10)), ', ') AS SanPham
                        FROM SalesInvoices s
                        LEFT JOIN Customers c  ON s.CustomerId = c.CustomerId
                        JOIN SalesDetails sd   ON s.SaleId     = sd.SaleId
                        JOIN Products p        ON sd.ProductId = p.ProductId
                        GROUP BY s.SaleId, c.Name, s.SaleDate, s.Total
                        ORDER BY s.SaleDate DESC";

            var dt = DbHelper.Query(sql);

            dt.Columns.Add("STT", typeof(int));
            for (int i = 0; i < dt.Rows.Count; i++)
                dt.Rows[i]["STT"] = i + 1;

            dgvRecentOrders.DataSource = null;

            dgvRecentOrders.DataSource = dt;

            dgvRecentOrders.Columns["SaleId"].Visible = false;
            dgvRecentOrders.Columns["STT"].DisplayIndex = 0;
            dgvRecentOrders.Columns["KhachHang"].DisplayIndex = 1;
            dgvRecentOrders.Columns["SanPham"].DisplayIndex = 2;
            dgvRecentOrders.Columns["Total"].DisplayIndex = 3;
            dgvRecentOrders.Columns["SaleDate"].Visible = false;

            dgvRecentOrders.Columns["STT"].HeaderText = "#";
            dgvRecentOrders.Columns["STT"].Width = 36;
            dgvRecentOrders.Columns["KhachHang"].HeaderText = "Khách hàng";
            dgvRecentOrders.Columns["SanPham"].HeaderText = "Sản phẩm";
            dgvRecentOrders.Columns["Total"].HeaderText = "Tổng tiền";
            dgvRecentOrders.Columns["Total"].DefaultCellStyle.Format = "N0";

            dgvRecentOrders.ColumnHeadersHeight = 44;
            dgvRecentOrders.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            var headerStyle = dgvRecentOrders.ColumnHeadersDefaultCellStyle;
            headerStyle.BackColor = Color.FromArgb(239, 246, 255);   
            headerStyle.ForeColor = Color.FromArgb(100, 116, 139);   
            headerStyle.Font = new Font("Be Vietnam Pro", 9f, FontStyle.Bold);
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            headerStyle.Padding = new Padding(8, 0, 0, 0);
            dgvRecentOrders.EnableHeadersVisualStyles = false;

            dgvRecentOrders.Columns["STT"].HeaderText = "#";
            dgvRecentOrders.Columns["KhachHang"].HeaderText = "KHÁCH HÀNG";
            dgvRecentOrders.Columns["SanPham"].HeaderText = "SẢN PHẨM";
            dgvRecentOrders.Columns["Total"].HeaderText = "TỔNG TIỀN";

            dgvRecentOrders.RowTemplate.Height = 52;
            dgvRecentOrders.RowsDefaultCellStyle.Font = new Font("Be Vietnam Pro", 9.5f);
            dgvRecentOrders.RowsDefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30);
            dgvRecentOrders.RowsDefaultCellStyle.BackColor = Color.White;
            dgvRecentOrders.RowsDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            dgvRecentOrders.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgvRecentOrders.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRecentOrders.GridColor = Color.FromArgb(229, 231, 235);
            dgvRecentOrders.BackgroundColor = Color.White;
            dgvRecentOrders.BorderStyle = BorderStyle.None;
            dgvRecentOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecentOrders.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgvRecentOrders.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30);

            dgvRecentOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvRecentOrders.Columns["STT"].Width = 48;
            dgvRecentOrders.Columns["KhachHang"].Width = 160;
            dgvRecentOrders.Columns["SanPham"].FillWeight = 100;
            dgvRecentOrders.Columns["SanPham"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvRecentOrders.Columns["Total"].Width = 130;
            dgvRecentOrders.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRecentOrders.Columns["Total"].DefaultCellStyle.Font = new Font("Be Vietnam Pro", 9.5f, FontStyle.Bold);
            dgvRecentOrders.Columns["Total"].DefaultCellStyle.Format = "N0";

            dgvRecentOrders.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgvRecentOrders.Columns[e.ColumnIndex].Name == "STT" && e.Value != null)
                    e.Value = e.Value.ToString()!.PadLeft(3, '0');
            };

        }

        private void LoadLowStock()
        {
            const int thresholdSingleCard = 5;
            const int thresholdPack = 10; 
            const int thresholdFullBox = 5;

            string sql = $@"
                        SELECT ProductName, Stock, Category FROM (

                            -- SingleCard: từng variant riêng
                            SELECT
                                p.Name + ' [' + pv.Rarity + ']' AS ProductName,
                                pv.Stock AS Stock,
                                'SingleCard' AS Category
                            FROM ProductVariants pv
                            INNER JOIN Products p ON pv.ProductId = p.ProductId
                            WHERE p.IsActive = 1
                              AND p.Category = 'SingleCard'
                              AND pv.Stock <= {thresholdSingleCard}

                            UNION ALL

                            -- Pack                          ✅ THÊM MỚI
                            SELECT
                                p.Name AS ProductName,
                                SUM(pv.Stock) AS Stock,
                                'Pack' AS Category
                            FROM ProductVariants pv
                            INNER JOIN Products p ON pv.ProductId = p.ProductId
                            WHERE p.IsActive = 1
                              AND p.Category = 'Pack'
                            GROUP BY p.ProductId, p.Name
                            HAVING SUM(pv.Stock) <= {thresholdPack}

                            UNION ALL

                            -- FullBox
                            SELECT
                                p.Name AS ProductName,
                                SUM(pv.Stock) AS Stock,
                                'FullBox' AS Category
                            FROM ProductVariants pv
                            INNER JOIN Products p ON pv.ProductId = p.ProductId
                            WHERE p.IsActive = 1
                              AND p.Category = 'FullBox'
                            GROUP BY p.ProductId, p.Name
                            HAVING SUM(pv.Stock) <= {thresholdFullBox}

                        ) AS Combined
                        ORDER BY Stock ASC";

            var dt = DbHelper.Query(sql);

            PanelLowStock.Controls.Clear();
            PanelLowStock.AutoScroll = true;

            if (dt.Rows.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "✓ Không có sản phẩm nào sắp hết hàng",
                    ForeColor = Color.FromArgb(100, 180, 120),
                    Font = new Font("Be Vietnam Pro", 9f),
                    AutoSize = false,
                    Width = PanelLowStock.Width - 10,
                    Height = 40,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                PanelLowStock.Controls.Add(lblEmpty);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"PanelLowStock.Width = {PanelLowStock.Width}");
            System.Diagnostics.Debug.WriteLine($"Rows count = {dt.Rows.Count}");

            foreach (DataRow row in dt.Rows)
            {
                var item = new UCLowStockItem
                {
                    Dock = DockStyle.Top,   
                    Margin = new Padding(0, 0, 0, 4)
                };

                string cat = row["Category"].ToString();
                int threshold = cat == "SingleCard" ? thresholdSingleCard : thresholdFullBox;

                item.SetData(
                    row["ProductName"].ToString(),
                    Convert.ToInt32(row["Stock"]),
                    threshold);

                PanelLowStock.Controls.Add(item);
            }
        }

        private string FormatMoney(decimal amount)
        {
            return amount.ToString("N0") + " ₫";
        }

        private void dgvRecentOrders_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            if (dgvRecentOrders.Columns[e.ColumnIndex].Name != "Status") return;

            e.Paint(e.ClipBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

            string status = e.Value?.ToString() ?? "";
            bool isCancel = status.Contains("Hủ") || status.ToLower().Contains("cancel");

            Color bgColor = isCancel ? Color.FromArgb(254, 226, 226) : Color.FromArgb(220, 252, 231);
            Color textColor = isCancel ? Color.FromArgb(185, 28, 28) : Color.FromArgb(21, 128, 61);

            using var brush = new SolidBrush(bgColor);
            var rect = new Rectangle(e.CellBounds.X + 6, e.CellBounds.Y + 6,
                                      e.CellBounds.Width - 12, e.CellBounds.Height - 12);
            using var gp = new System.Drawing.Drawing2D.GraphicsPath();
            int r = 6;
            gp.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
            gp.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
            gp.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
            gp.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
            gp.CloseFigure();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillPath(brush, gp);

            using var txtBrush = new SolidBrush(textColor);
            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            e.Graphics.DrawString(status, new Font("Be Vietnam Pro", 8.5f, FontStyle.Bold),
                                  txtBrush, e.CellBounds, fmt);

            e.Handled = true;
        }

        private void LoadRevenueProfitStats(DateTime from, DateTime to)
        {
            string f = from.ToString("yyyy-MM-dd");
            string t = to.ToString("yyyy-MM-dd");

            string sql = $@"
                        SELECT TOP 5
                            p.Name AS ProductName,
                            SUM(sd.Qty) AS TotalSold,
                            SUM(sd.LineTotal) AS Revenue,
                            SUM(sd.LineTotal - (sd.Qty * sd.UnitCostPrice)) AS Profit
                        FROM SalesDetails sd
                        JOIN Products p ON sd.ProductId = p.ProductId
                        JOIN SalesInvoices s ON sd.SaleId = s.SaleId
                        WHERE CAST(s.SaleDate AS DATE) BETWEEN '{f}' AND '{t}'
                        GROUP BY p.Name
                        ORDER BY Revenue DESC";

            var dt = DbHelper.Query(sql);
            flpTopProducts.Controls.Clear();

            if (dt.Rows.Count == 0)
            {
                Label lblNoData = new Label { Text = "Không có dữ liệu bán hàng", Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, Height = 50, Font = new Font("Be Vietnam Pro", 9) };
                flpTopProducts.Controls.Add(lblNoData);
                return;
            }

            int rank = 1;
            foreach (DataRow row in dt.Rows)
            {
                var item = CreateRevenueItem(
                    rank,
                    row["ProductName"].ToString(),
                    Convert.ToDecimal(row["Revenue"]),
                    Convert.ToDecimal(row["Profit"]));

                item.Width = pnlTopProducts.Width - 30;
                flpTopProducts.Controls.Add(item);
                rank++;
            }
        }

        private Panel CreateRevenueItem(int rank, string name, decimal revenue, decimal profit)
        {
            Guna.UI2.WinForms.Guna2Panel pnl = new Guna.UI2.WinForms.Guna2Panel
            {
                Height = 85, 
                BackColor = Color.Transparent,
                FillColor = Color.White,
                BorderRadius = 10, 
                BorderColor = Color.FromArgb(220, 226, 235), 
                BorderThickness = 1, 
                Margin = new Padding(5, 5, 10, 10) 
            };

            Label lblName = new Label
            {
                Text = $"{rank}. {name}",
                Font = new Font("Be Vietnam Pro", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(15, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblRev = new Label
            {
                Text = $"Doanh thu: {revenue:N0} đ",
                Font = new Font("Be Vietnam Pro", 9.5F, FontStyle.Bold),
                ForeColor = Color.RoyalBlue,
                Location = new Point(30, 38),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblProfit = new Label
            {
                Text = $"Lợi nhuận: {profit:N0} đ",
                Font = new Font("Be Vietnam Pro", 9.5F, FontStyle.Italic),
                ForeColor = Color.SeaGreen,
                Location = new Point(30, 58),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            pnl.Controls.Add(lblName);
            pnl.Controls.Add(lblRev);
            pnl.Controls.Add(lblProfit);

            return pnl;
        }

        private void SetupChart()
        {
            _formsPlot = new FormsPlot();
            _formsPlot.Dock = DockStyle.Fill;
            pnlChart.Controls.Add(_formsPlot);
        }

        private void RenderChart(DateTime fromDate, DateTime toDate)
        {
            if (_formsPlot == null) return;

            _formsPlot.Plot.Clear();

            string f = _fromDate.ToString("yyyy-MM-dd");
            string t = _toDate.ToString("yyyy-MM-dd");

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

            var sRev = _formsPlot.Plot.Add.Scatter(xs, revenue);
            sRev.Color = new ScottPlot.Color(249, 115, 22);
            sRev.LegendText = "Doanh thu";

            var sProf = _formsPlot.Plot.Add.Scatter(xs, profit);
            sProf.Color = new ScottPlot.Color(16, 185, 129);
            sProf.LegendText = "Lợi nhuận";

            _formsPlot.Plot.Axes.DateTimeTicksBottom();
            _formsPlot.Plot.ShowLegend();
            _formsPlot.Refresh();
        }
    }
}