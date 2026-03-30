using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using WinColor = System.Drawing.Color;
using SPColor = ScottPlot.Color;

namespace YugiohShop
{
    public partial class FormDashboard : Form
    {
        private DateTime _fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private DateTime _toDate = DateTime.Today;

        public FormDashboard()
        {
            InitializeComponent();
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            //SetupTimePeriod();
            SetupRecentOrdersGrid();
            LoadDashboard();
        }

        // ════════════════════════════════════
        // SETUP
        // ════════════════════════════════════

        //private void SetupTimePeriod()
        //{
        //    cboDateRange.Items.Clear();
        //    cboDateRange.Items.Add("Hôm nay");
        //    cboDateRange.Items.Add("7 ngày qua");
        //    cboDateRange.Items.Add("30 ngày qua");
        //    cboDateRange.Items.Add("Tháng này");
        //    cboDateRange.SelectedIndex = 0;
        //}

        private (DateTime from, DateTime to) GetDateRange()
        {
            return (_fromDate, _toDate);
        }

        private void SetupRecentOrdersGrid()
        {
            dgvRecentOrders.Columns.Clear();
            dgvRecentOrders.AutoGenerateColumns = false;
            dgvRecentOrders.AllowUserToAddRows = false;
            dgvRecentOrders.ReadOnly = true;
            dgvRecentOrders.RowHeadersVisible = false;
            dgvRecentOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecentOrders.BackgroundColor = WinColor.White;
            dgvRecentOrders.BorderStyle = BorderStyle.None;
            dgvRecentOrders.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRecentOrders.EnableHeadersVisualStyles = false;
            dgvRecentOrders.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvRecentOrders.ColumnHeadersHeight = 36;
            dgvRecentOrders.RowTemplate.Height = 32;
            dgvRecentOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvRecentOrders.ColumnHeadersDefaultCellStyle.BackColor = WinColor.FromArgb(47, 128, 237);
            dgvRecentOrders.ColumnHeadersDefaultCellStyle.ForeColor = WinColor.White;
            dgvRecentOrders.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            dgvRecentOrders.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvRecentOrders.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9);
            dgvRecentOrders.DefaultCellStyle.SelectionBackColor = WinColor.FromArgb(220, 235, 255);
            dgvRecentOrders.DefaultCellStyle.SelectionForeColor = WinColor.Black;
            dgvRecentOrders.AlternatingRowsDefaultCellStyle.BackColor = WinColor.FromArgb(248, 250, 252);

            dgvRecentOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Thời gian",
                DataPropertyName = "SaleDate",
                FillWeight = 22
            });
            dgvRecentOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Khách hàng",
                DataPropertyName = "CustomerName",
                FillWeight = 32
            });
            dgvRecentOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tổng tiền",
                DataPropertyName = "Total",
                FillWeight = 26,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvRecentOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nhân viên",
                DataPropertyName = "UserName",
                FillWeight = 20
            });
        }

        // ════════════════════════════════════
        // LOAD DASHBOARD
        // ════════════════════════════════════

        private void LoadDashboard()
        {
            var (from, to) = GetDateRange();
            //lblWelcome.Text = $"Xin chào, {CurrentUser.FullName} 👋   •   {cbTimePeriod.Text}";
            LoadKpiCards(from, to);
            LoadChart(from, to);
            LoadRecentOrders();
            LoadTopProducts(from, to);
        }

        // ════════════════════════════════════
        // KPI CARDS
        // ════════════════════════════════════

        private void LoadKpiCards(DateTime from, DateTime to)
        {
            flpCards.Controls.Clear();

            try
            {
                using var conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string dateFilter = $"CAST(SaleDate AS DATE) BETWEEN '{from:yyyy-MM-dd}' AND '{to:yyyy-MM-dd}'";

                decimal revenue = Convert.ToDecimal(new SqlCommand(
                    $"SELECT ISNULL(SUM(Total), 0) FROM SalesInvoices WHERE {dateFilter}",
                    conn).ExecuteScalar());

                int totalOrders = Convert.ToInt32(new SqlCommand(
                    $"SELECT COUNT(*) FROM SalesInvoices WHERE {dateFilter}",
                    conn).ExecuteScalar());

                int newCustomers = Convert.ToInt32(new SqlCommand(
                    $"SELECT COUNT(*) FROM Customers WHERE CAST(CreatedAt AS DATE) BETWEEN '{from:yyyy-MM-dd}' AND '{to:yyyy-MM-dd}'",
                    conn).ExecuteScalar());

                int ordersWithCustomer = Convert.ToInt32(new SqlCommand(
                    $"SELECT COUNT(*) FROM SalesInvoices WHERE CustomerId IS NOT NULL AND {dateFilter}",
                    conn).ExecuteScalar());

                double convRate = totalOrders > 0
                    ? Math.Round((double)ordersWithCustomer / totalOrders * 100, 1)
                    : 0;

                flpCards.Controls.Add(CreateKpiCard(
                    "Doanh thu", revenue.ToString("N0") + " đ", "💰",
                    WinColor.FromArgb(47, 128, 237),
                    WinColor.FromArgb(25, 47, 128, 237)));

                flpCards.Controls.Add(CreateKpiCard(
                    "Đơn hàng", totalOrders.ToString(), "🛒",
                    WinColor.FromArgb(39, 174, 96),
                    WinColor.FromArgb(25, 39, 174, 96)));

                flpCards.Controls.Add(CreateKpiCard(
                    "Khách mới", newCustomers.ToString(), "👤",
                    WinColor.FromArgb(156, 39, 176),
                    WinColor.FromArgb(25, 156, 39, 176)));

                flpCards.Controls.Add(CreateKpiCard(
                    "Conversion Rate", convRate + "%", "📈",
                    WinColor.FromArgb(230, 126, 34),
                    WinColor.FromArgb(25, 230, 126, 34)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load KPI: " + ex.Message);
            }
        }

        private Guna.UI2.WinForms.Guna2Panel CreateKpiCard(
            string title, string value, string icon,
            WinColor accent, WinColor iconBg)
        {
            var card = new Guna.UI2.WinForms.Guna2Panel();
            card.Size = new System.Drawing.Size(210, 100);
            card.Margin = new System.Windows.Forms.Padding(0, 0, 14, 0);
            card.FillColor = WinColor.White;
            card.BorderRadius = 12;
            card.ShadowDecoration.Enabled = true;
            card.ShadowDecoration.Color = WinColor.FromArgb(15, 0, 0, 0);
            card.ShadowDecoration.Depth = 8;
            card.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 3, 0, 0);

            var iconPanel = new Guna.UI2.WinForms.Guna2Panel();
            iconPanel.Size = new System.Drawing.Size(48, 48);
            iconPanel.Location = new System.Drawing.Point(14, 26);
            iconPanel.FillColor = iconBg;
            iconPanel.BorderRadius = 24;

            var lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new System.Drawing.Font("Segoe UI Emoji", 16);
            lblIcon.Size = new System.Drawing.Size(48, 48);
            lblIcon.Location = new System.Drawing.Point(0, 0);
            lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            iconPanel.Controls.Add(lblIcon);

            var lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            lblTitle.ForeColor = WinColor.FromArgb(140, 140, 140);
            lblTitle.Location = new System.Drawing.Point(72, 24);
            lblTitle.Size = new System.Drawing.Size(130, 18);

            var lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold);
            lblValue.ForeColor = WinColor.FromArgb(25, 25, 25);
            lblValue.Location = new System.Drawing.Point(70, 46);
            lblValue.Size = new System.Drawing.Size(134, 30);
            lblValue.AutoEllipsis = true;

            card.Controls.Add(iconPanel);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);

            return card;
        }

        // ════════════════════════════════════
        // CHART
        // ════════════════════════════════════

        private void LoadChart(DateTime from, DateTime to)
        {
            try
            {
                using var conn = new SqlConnection(DbConfig.ConnectionString);
                string sql = $@"
                    SELECT CAST(SaleDate AS DATE) AS Day,
                           ISNULL(SUM(Total), 0)  AS Revenue
                    FROM SalesInvoices
                    WHERE CAST(SaleDate AS DATE) BETWEEN '{from:yyyy-MM-dd}' AND '{to:yyyy-MM-dd}'
                    GROUP BY CAST(SaleDate AS DATE)
                    ORDER BY Day";

                var dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);

                var allDays = Enumerable.Range(0, (to - from).Days + 1)
                    .Select(i => from.AddDays(i)).ToList();

                double[] xs = allDays.Select(d => d.ToOADate()).ToArray();
                double[] ys = allDays.Select(d =>
                {
                    var row = dt.AsEnumerable()
                        .FirstOrDefault(r => Convert.ToDateTime(r["Day"]).Date == d.Date);
                    return row != null ? Convert.ToDouble(row["Revenue"]) : 0;
                }).ToArray();

                formsPlot1.Plot.Clear();

                var scatter = formsPlot1.Plot.Add.Scatter(xs, ys);
                scatter.Color = SPColor.FromARGB(0xFF2F80ED);
                scatter.LineWidth = 2.5f;
                scatter.MarkerSize = 6;

                formsPlot1.Plot.Axes.DateTimeTicksBottom();
                formsPlot1.Plot.Axes.Left.Label.Text = "Doanh thu (đ)";
                formsPlot1.Plot.FigureBackground.Color = SPColor.FromARGB(0xFFFFFFFF);
                formsPlot1.Plot.DataBackground.Color = SPColor.FromARGB(0xFFF8F9FA);

                formsPlot1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load chart: " + ex.Message);
            }
        }

        // ════════════════════════════════════
        // ĐƠN GẦN ĐÂY
        // ════════════════════════════════════

        private void LoadRecentOrders()
        {
            try
            {
                using var conn = new SqlConnection(DbConfig.ConnectionString);
                string sql = @"
                    SELECT TOP 8
                        FORMAT(si.SaleDate, 'HH:mm dd/MM') AS SaleDate,
                        ISNULL(si.CustomerName, 'Khách lẻ') AS CustomerName,
                        si.Total,
                        ISNULL(u.FullName, '') AS UserName
                    FROM SalesInvoices si
                    LEFT JOIN Users u ON u.UserId = si.UserId
                    ORDER BY si.SaleDate DESC";

                var dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);
                dgvRecentOrders.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load đơn gần đây: " + ex.Message);
            }
        }

        // ════════════════════════════════════
        // TOP SẢN PHẨM
        // ════════════════════════════════════

        private void LoadTopProducts(DateTime from, DateTime to)
        {
            flpTopProducts.Controls.Clear();

            try
            {
                using var conn = new SqlConnection(DbConfig.ConnectionString);
                string sql = $@"
                    SELECT TOP 5
                        p.Name,
                        SUM(sd.Qty)       AS TotalQty,
                        SUM(sd.LineTotal) AS TotalRevenue
                    FROM SalesDetails sd
                    INNER JOIN Products p ON p.ProductId = sd.ProductId
                    INNER JOIN SalesInvoices si ON si.SaleId = sd.SaleId
                    WHERE CAST(si.SaleDate AS DATE) BETWEEN '{from:yyyy-MM-dd}' AND '{to:yyyy-MM-dd}'
                    GROUP BY p.Name
                    ORDER BY TotalQty DESC";

                var dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);

                string[] medals = { "🥇", "🥈", "🥉", "4️⃣", "5️⃣" };
                int rank = 0;

                foreach (DataRow row in dt.Rows)
                {
                    var item = new Panel();
                    item.Size = new System.Drawing.Size(flpTopProducts.Width - 20, 46);
                    item.BackColor = WinColor.White;
                    item.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);

                    var lblRank = new Label();
                    lblRank.Text = medals[rank];
                    lblRank.Font = new System.Drawing.Font("Segoe UI Emoji", 14);
                    lblRank.Location = new System.Drawing.Point(6, 9);
                    lblRank.Size = new System.Drawing.Size(32, 28);

                    var lblName = new Label();
                    lblName.Text = row["Name"].ToString();
                    lblName.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
                    lblName.ForeColor = WinColor.FromArgb(30, 30, 30);
                    lblName.Location = new System.Drawing.Point(44, 4);
                    lblName.Size = new System.Drawing.Size(flpTopProducts.Width - 70, 18);
                    lblName.AutoEllipsis = true;

                    var lblDetail = new Label();
                    lblDetail.Text = $"Đã bán: {row["TotalQty"]}  •  {Convert.ToDecimal(row["TotalRevenue"]):N0} đ";
                    lblDetail.Font = new System.Drawing.Font("Segoe UI", 8);
                    lblDetail.ForeColor = WinColor.Gray;
                    lblDetail.Location = new System.Drawing.Point(44, 24);
                    lblDetail.Size = new System.Drawing.Size(flpTopProducts.Width - 70, 16);

                    item.Controls.Add(lblRank);
                    item.Controls.Add(lblName);
                    item.Controls.Add(lblDetail);
                    flpTopProducts.Controls.Add(item);
                    rank++;
                }

                if (dt.Rows.Count == 0)
                {
                    var lbl = new Label();
                    lbl.Text = "Chưa có dữ liệu trong kỳ này";
                    lbl.ForeColor = WinColor.Gray;
                    lbl.Font = new System.Drawing.Font("Segoe UI", 9);
                    lbl.AutoSize = true;
                    lbl.Margin = new System.Windows.Forms.Padding(8);
                    flpTopProducts.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi top sản phẩm: " + ex.Message);
            }
        }

        // ════════════════════════════════════
        // EVENT
        // ════════════════════════════════════

        //private void cbTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    LoadDashboard();
        //}

        // FormDashboard.cs — thêm hàm public này

        public void RefreshByRange(string range)
        {
            switch (range)
            {
                case "Hôm nay":
                    _fromDate = DateTime.Today;
                    _toDate = DateTime.Today;
                    break;
                case "7 ngày":
                    _fromDate = DateTime.Today.AddDays(-6);
                    _toDate = DateTime.Today;
                    break;
                case "Tháng này":
                    _fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    _toDate = DateTime.Today;
                    break;
                case "Tháng trước":
                    var f = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    _fromDate = f;
                    _toDate = f.AddMonths(1).AddDays(-1);
                    break;
            }
            LoadDashboard(); // ← hàm có sẵn của bạn
        }

        public void OnDateRangeChanged(string range)
        {
            switch (range)
            {
                case "Hôm nay":
                    _fromDate = DateTime.Today;
                    _toDate = DateTime.Today;
                    break;
                case "7 ngày":
                    _fromDate = DateTime.Today.AddDays(-6);
                    _toDate = DateTime.Today;
                    break;
                case "Tháng này":
                    _fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    _toDate = DateTime.Today;
                    break;
                case "Tháng trước":
                    var f = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    _fromDate = f;
                    _toDate = f.AddMonths(1).AddDays(-1);
                    break;
            }
            LoadDashboard();  // ← gọi lại hàm có sẵn, dùng _fromDate/_toDate mới
        }
    }
}