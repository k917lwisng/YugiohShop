using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScottPlot;
using ScottPlot.TickGenerators;

namespace YugiohShop
{
    public partial class FormStatistics : Form
    {
        private ScottPlot.WinForms.FormsPlot formsPlotStatistics = null!;
        private string currentMainMode = "TimeSeries"; 
        private string currentChartMode = "Revenue";   
        private string currentDgvMode = "DailySum";    
        private ToolTip chartToolTip = new ToolTip();

        private double[] currentPositions = Array.Empty<double>();
        private double[] currentValues = Array.Empty<double>();
        private string[] currentLabels = Array.Empty<string>();

        public FormStatistics()
        {
            InitializeComponent();
        }

        private void FormStatistics_Load(object sender, EventArgs e)
        {
            CreateScottPlotInLeftPanel();
            SetupDgv();

            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            // Setup combobox chế độ biểu đồ
            cbChartMode.Items.Clear();
            cbChartMode.Items.Add("Doanh thu theo ngày");
            cbChartMode.Items.Add("Lợi nhuận theo ngày");
            cbChartMode.Items.Add("Top sản phẩm bán chạy");
            cbChartMode.Items.Add("Top sản phẩm lợi nhuận cao");
            cbChartMode.SelectedIndex = 0;

            // Setup combobox chế độ bảng
            cbDgvMode.Items.Clear();
            cbDgvMode.Items.Add("Tổng hợp theo ngày");
            cbDgvMode.Items.Add("Danh sách hóa đơn");
            cbDgvMode.Items.Add("Top bán chạy");
            cbDgvMode.Items.Add("Top lợi nhuận cao");
            cbDgvMode.SelectedIndex = 0;

            LoadStatistics();
            RefreshAll();
            UpdateLastUpdated();
        }

        private void CreateScottPlotInLeftPanel()
        {
            leftMiddlePanel.Controls.Clear();

            formsPlotStatistics = new ScottPlot.WinForms.FormsPlot();
            formsPlotStatistics.Name = "formsPlotStatistics";
            formsPlotStatistics.Dock = DockStyle.Fill;
            formsPlotStatistics.Margin = new Padding(5);

            leftMiddlePanel.Controls.Add(formsPlotStatistics);

            formsPlotStatistics.MouseMove -= formsPlotStatistics_MouseMove;
            formsPlotStatistics.MouseMove += formsPlotStatistics_MouseMove;
        }

        private void LoadChartData()
        {
            try
            {
                string fromDate = dtpFromDate.Value.ToString("yyyy-MM-dd");
                string toDate = dtpToDate.Value.ToString("yyyy-MM-dd");

                var plt = formsPlotStatistics.Plot;
                plt.Clear();

                string selectedChart = cbChartMode.SelectedItem?.ToString() ?? "";

                if (selectedChart == "Doanh thu theo ngày" || selectedChart == "Lợi nhuận theo ngày")
                {
                    LoadTimeSeriesChart(fromDate, toDate, selectedChart == "Doanh thu theo ngày");
                }
                else if (selectedChart == "Top sản phẩm bán chạy")
                {
                    LoadTopProductsBarChart(fromDate, toDate, byQty: true);
                }
                else
                {
                    LoadTopProductsBarChart(fromDate, toDate, byQty: false);
                }

                formsPlotStatistics.Refresh();
                UpdateLastUpdated();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load biểu đồ: " + ex.Message);
            }
        }

        private void LoadTimeSeriesChart(string fromDate, string toDate, bool isRevenue)
        {
            string sql;
            if (isRevenue)
            {
                sql = $@"
                SELECT CAST(SaleDate AS DATE) AS ReportDate,
                       ISNULL(SUM(Total), 0) AS Amount
                FROM SalesInvoices
                WHERE CAST(SaleDate AS DATE) >= '{fromDate}'
                  AND CAST(SaleDate AS DATE) <= '{toDate}'
                GROUP BY CAST(SaleDate AS DATE)
                ORDER BY CAST(SaleDate AS DATE)";
                            }
                            else
                            {
                                sql = $@"
                SELECT CAST(si.SaleDate AS DATE) AS ReportDate,
                       ISNULL(SUM(sd.LineTotal - (sd.UnitCostPrice * sd.Qty)), 0) AS Amount
                FROM SalesDetails sd
                INNER JOIN SalesInvoices si ON sd.SaleId = si.SaleId
                WHERE CAST(si.SaleDate AS DATE) >= '{fromDate}'
                  AND CAST(si.SaleDate AS DATE) <= '{toDate}'
                GROUP BY CAST(si.SaleDate AS DATE)
                ORDER BY CAST(si.SaleDate AS DATE)";
            }

            DataTable dt = DbHelper.Query(sql);
            var plt = formsPlotStatistics.Plot;

            currentPositions = new double[dt.Rows.Count];
            currentValues = new double[dt.Rows.Count];
            currentLabels = new string[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                currentPositions[i] = i;
                currentValues[i] = Convert.ToDouble(dt.Rows[i]["Amount"]);
                currentLabels[i] = Convert.ToDateTime(dt.Rows[i]["ReportDate"]).ToString("dd/MM");
            }

            if (currentValues.Length > 0)
            {
                var scatter = plt.Add.Scatter(currentPositions, currentValues);
                scatter.LineWidth = 2;
                scatter.MarkerSize = 7;
                scatter.Color = isRevenue
                    ? ScottPlot.Color.FromHex("#2F80ED")
                    : ScottPlot.Color.FromHex("#27AE60");
            }

            plt.Axes.Bottom.TickGenerator = new NumericManual(currentPositions, currentLabels);
            plt.Title(isRevenue ? "Biểu đồ doanh thu theo ngày" : "Biểu đồ lợi nhuận theo ngày");
            plt.XLabel("Ngày");
            plt.YLabel(isRevenue ? "Doanh thu (đ)" : "Lợi nhuận (đ)");
        }

        private void LoadTopProductsBarChart(string fromDate, string toDate, bool byQty)
        {
            string orderBy = byQty ? "SUM(sd.Qty) DESC" : "SUM(sd.LineTotal - sd.UnitCostPrice * sd.Qty) DESC";
            string valueCol = byQty
                ? "SUM(sd.Qty)"
                : "ISNULL(SUM(sd.LineTotal - (sd.UnitCostPrice * sd.Qty)), 0)";

            string sql = $@"
                        SELECT TOP 10
                            p.Name,
                            {valueCol} AS Value
                        FROM SalesDetails sd
                        INNER JOIN Products p ON sd.ProductId = p.ProductId
                        INNER JOIN SalesInvoices si ON sd.SaleId = si.SaleId
                        WHERE CAST(si.SaleDate AS DATE) >= '{fromDate}'
                          AND CAST(si.SaleDate AS DATE) <= '{toDate}'
                        GROUP BY p.ProductId, p.Name
                        ORDER BY {orderBy}";

            DataTable dt = DbHelper.Query(sql);
            var plt = formsPlotStatistics.Plot;

            currentPositions = new double[dt.Rows.Count];
            currentValues = new double[dt.Rows.Count];
            currentLabels = new string[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                currentPositions[i] = i;
                currentValues[i] = Convert.ToDouble(dt.Rows[i]["Value"]);
                currentLabels[i] = dt.Rows[i]["Name"].ToString() ?? "";
            }

            if (currentValues.Length > 0)
            {
                var bar = plt.Add.Bars(currentValues);
                bar.Color = byQty
                    ? ScottPlot.Color.FromHex("#F39C12")
                    : ScottPlot.Color.FromHex("#8E44AD");
            }

            plt.Axes.Bottom.TickGenerator = new NumericManual(currentPositions, currentLabels);
            plt.Title(byQty ? "Top 10 sản phẩm bán chạy" : "Top 10 sản phẩm lợi nhuận cao");
            plt.XLabel("Sản phẩm");
            plt.YLabel(byQty ? "Số lượng bán" : "Lợi nhuận (đ)");
        }

        private void formsPlotStatistics_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentValues.Length == 0) return;

            var mouse = formsPlotStatistics.Plot.GetCoordinates(e.X, e.Y);
            double minDistance = double.MaxValue;
            int nearestIndex = -1;

            for (int i = 0; i < currentPositions.Length; i++)
            {
                double dx = currentPositions[i] - mouse.X;
                double dy = currentValues[i] - mouse.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = i;
                }
            }

            if (nearestIndex >= 0)
            {
                string label = currentLabels[nearestIndex];
                double value = currentValues[nearestIndex];
                string selectedChart = cbChartMode.SelectedItem?.ToString() ?? "";

                string valueStr = selectedChart == "Top sản phẩm bán chạy"
                    ? $"{value:N0} sản phẩm"
                    : $"{value:N0} đ";

                chartToolTip.SetToolTip(formsPlotStatistics,
                    $"{label}\n{valueStr}");
            }
        }

        private void SetupDgv()
        {
            //dgvTopProducts.Columns.Clear();
            //dgvTopProducts.AutoGenerateColumns = false;
            //dgvTopProducts.AllowUserToAddRows = false;
            //dgvTopProducts.AllowUserToDeleteRows = false;
            //dgvTopProducts.ReadOnly = true;
            //dgvTopProducts.MultiSelect = false;
            //dgvTopProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //dgvTopProducts.RowHeadersVisible = false;
            //dgvTopProducts.BackgroundColor = Color.White;
            //dgvTopProducts.BorderStyle = BorderStyle.None;
            //dgvTopProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //dgvTopProducts.EnableHeadersVisualStyles = false;
            //dgvTopProducts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            //dgvTopProducts.ColumnHeadersHeight = 35;
            //dgvTopProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(47, 128, 237);
            //dgvTopProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //dgvTopProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            //dgvTopProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dgvTopProducts.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            //dgvTopProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            //dgvTopProducts.DefaultCellStyle.SelectionForeColor = Color.Black;
            //dgvTopProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void LoadDgvData()
        {
            try
            {
                string fromDate = dtpFromDate.Value.ToString("yyyy-MM-dd");
                string toDate = dtpToDate.Value.ToString("yyyy-MM-dd");
                string selected = cbDgvMode.SelectedItem?.ToString() ?? "";

                dgvTopProducts.Columns.Clear();
                DataTable dt;

                if (selected == "Tổng hợp theo ngày")
                {
                    dt = LoadDailySum(fromDate, toDate);
                    dgvTopProducts.Columns.Add(MakeCol("ReportDate", "Ngày", "ReportDate", 20, "dd/MM/yyyy"));
                    dgvTopProducts.Columns.Add(MakeCol("OrderCount", "Số đơn", "OrderCount", 15));
                    dgvTopProducts.Columns.Add(MakeCol("Revenue", "Doanh thu", "Revenue", 25, "N0", DataGridViewContentAlignment.MiddleRight));
                    dgvTopProducts.Columns.Add(MakeCol("Profit", "Lợi nhuận", "Profit", 25, "N0", DataGridViewContentAlignment.MiddleRight));
                }
                else if (selected == "Danh sách hóa đơn")
                {
                    dt = LoadInvoiceList(fromDate, toDate);
                    dgvTopProducts.Columns.Add(MakeCol("SaleId", "Mã HĐ", "SaleId", 10));
                    dgvTopProducts.Columns.Add(MakeCol("SaleDate", "Ngày", "SaleDate", 20, "dd/MM/yyyy HH:mm"));
                    dgvTopProducts.Columns.Add(MakeCol("CustomerName", "Khách hàng", "CustomerName", 25));
                    dgvTopProducts.Columns.Add(MakeCol("SubTotal", "Tạm tính", "SubTotal", 15, "N0", DataGridViewContentAlignment.MiddleRight));
                    dgvTopProducts.Columns.Add(MakeCol("Discount", "Giảm giá", "Discount", 13, "N0", DataGridViewContentAlignment.MiddleRight));
                    dgvTopProducts.Columns.Add(MakeCol("Total", "Tổng tiền", "Total", 17, "N0", DataGridViewContentAlignment.MiddleRight));
                }
                else if (selected == "Top bán chạy")
                {
                    dt = LoadTopByQty(fromDate, toDate);
                    dgvTopProducts.Columns.Add(MakeCol("Name", "Tên sản phẩm", "Name", 40));
                    dgvTopProducts.Columns.Add(MakeCol("CardCode", "Mã thẻ", "CardCode", 20));
                    dgvTopProducts.Columns.Add(MakeCol("TotalQty", "Đã bán", "TotalQty", 15, alignment: DataGridViewContentAlignment.MiddleCenter));
                    dgvTopProducts.Columns.Add(MakeCol("Revenue", "Doanh thu", "Revenue", 25, "N0", DataGridViewContentAlignment.MiddleRight));
                }
                else // Top lợi nhuận cao
                {
                    dt = LoadTopByProfit(fromDate, toDate);
                    dgvTopProducts.Columns.Add(MakeCol("Name", "Tên sản phẩm", "Name", 40));
                    dgvTopProducts.Columns.Add(MakeCol("CardCode", "Mã thẻ", "CardCode", 20));
                    dgvTopProducts.Columns.Add(MakeCol("TotalQty", "Đã bán", "TotalQty", 15, alignment: DataGridViewContentAlignment.MiddleCenter));
                    dgvTopProducts.Columns.Add(MakeCol("Profit", "Lợi nhuận", "Profit", 25, "N0", DataGridViewContentAlignment.MiddleRight));
                }

                dgvTopProducts.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load bảng: " + ex.Message);
            }
        }

        private DataTable LoadDailySum(string from, string to)
        {
            string sql = $@"
                    SELECT
                        CAST(si.SaleDate AS DATE) AS ReportDate,
                        COUNT(si.SaleId) AS OrderCount,
                        ISNULL(SUM(si.Total), 0) AS Revenue,
                        ISNULL(SUM(sd.LineTotal - (sd.UnitCostPrice * sd.Qty)), 0) AS Profit
                    FROM SalesInvoices si
                    LEFT JOIN SalesDetails sd ON si.SaleId = sd.SaleId
                    WHERE CAST(si.SaleDate AS DATE) >= '{from}'
                      AND CAST(si.SaleDate AS DATE) <= '{to}'
                    GROUP BY CAST(si.SaleDate AS DATE)
                    ORDER BY CAST(si.SaleDate AS DATE) DESC";
            return DbHelper.Query(sql);
        }

        private DataTable LoadInvoiceList(string from, string to)
        {
            string sql = $@"
                    SELECT
                        si.SaleId,
                        si.SaleDate,
                        ISNULL(c.Name, N'Khách lẻ') AS CustomerName,
                        si.SubTotal,
                        si.Discount,
                        si.Total
                    FROM SalesInvoices si
                    LEFT JOIN Customers c ON si.CustomerId = c.CustomerId
                    WHERE CAST(si.SaleDate AS DATE) >= '{from}'
                      AND CAST(si.SaleDate AS DATE) <= '{to}'
                    ORDER BY si.SaleDate DESC";
            return DbHelper.Query(sql);
        }

        private DataTable LoadTopByQty(string from, string to)
        {
            string sql = $@"
                        SELECT TOP 10
                            p.Name, p.CardCode,
                            SUM(sd.Qty) AS TotalQty,
                            ISNULL(SUM(sd.LineTotal), 0) AS Revenue
                        FROM SalesDetails sd
                        INNER JOIN Products p ON sd.ProductId = p.ProductId
                        INNER JOIN SalesInvoices si ON sd.SaleId = si.SaleId
                        WHERE CAST(si.SaleDate AS DATE) >= '{from}'
                          AND CAST(si.SaleDate AS DATE) <= '{to}'
                        GROUP BY p.ProductId, p.Name, p.CardCode
                        ORDER BY SUM(sd.Qty) DESC";
            return DbHelper.Query(sql);
        }

        private DataTable LoadTopByProfit(string from, string to)
        {
            string sql = $@"
                    SELECT TOP 10
                        p.Name, p.CardCode,
                        SUM(sd.Qty) AS TotalQty,
                        ISNULL(SUM(sd.LineTotal - (sd.UnitCostPrice * sd.Qty)), 0) AS Profit
                    FROM SalesDetails sd
                    INNER JOIN Products p ON sd.ProductId = p.ProductId
                    INNER JOIN SalesInvoices si ON sd.SaleId = si.SaleId
                    WHERE CAST(si.SaleDate AS DATE) >= '{from}'
                      AND CAST(si.SaleDate AS DATE) <= '{to}'
                    GROUP BY p.ProductId, p.Name, p.CardCode
                    ORDER BY SUM(sd.LineTotal - sd.UnitCostPrice * sd.Qty) DESC";
            return DbHelper.Query(sql);
        }

        private DataGridViewTextBoxColumn MakeCol(
            string name, string header, string dataProp,
            int fillWeight, string format = "",
            DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                DataPropertyName = dataProp,
                FillWeight = fillWeight
            };
            if (!string.IsNullOrEmpty(format) || alignment != DataGridViewContentAlignment.MiddleLeft)
            {
                col.DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = format,
                    Alignment = alignment
                };
            }
            return col;
        }

        private void LoadStatistics()
        {
            try
            {
                string sqlRevenueToday = @"
                                        SELECT ISNULL(SUM(Total), 0) FROM SalesInvoices
                                        WHERE CAST(SaleDate AS DATE) = CAST(GETDATE() AS DATE)";

                string sqlRevenueMonth = @"
                                        SELECT ISNULL(SUM(Total), 0) FROM SalesInvoices
                                        WHERE MONTH(SaleDate) = MONTH(GETDATE()) AND YEAR(SaleDate) = YEAR(GETDATE())";

                string sqlProfitToday = @"
                                        SELECT ISNULL(SUM(sd.LineTotal - (sd.UnitCostPrice * sd.Qty)), 0)
                                        FROM SalesDetails sd
                                        INNER JOIN SalesInvoices si ON sd.SaleId = si.SaleId
                                        WHERE CAST(si.SaleDate AS DATE) = CAST(GETDATE() AS DATE)";

                string sqlProfitMonth = @"
                                        SELECT ISNULL(SUM(sd.LineTotal - (sd.UnitCostPrice * sd.Qty)), 0)
                                        FROM SalesDetails sd
                                        INNER JOIN SalesInvoices si ON sd.SaleId = si.SaleId
                                        WHERE MONTH(si.SaleDate) = MONTH(GETDATE()) AND YEAR(si.SaleDate) = YEAR(GETDATE())";

                decimal revenueToday = Convert.ToDecimal(DbHelper.Query(sqlRevenueToday).Rows[0][0]);
                decimal revenueMonth = Convert.ToDecimal(DbHelper.Query(sqlRevenueMonth).Rows[0][0]);
                decimal profitToday = Convert.ToDecimal(DbHelper.Query(sqlProfitToday).Rows[0][0]);
                decimal profitMonth = Convert.ToDecimal(DbHelper.Query(sqlProfitMonth).Rows[0][0]);

                lblRevenueToday.Text = revenueToday.ToString("N0") + " đ";
                lblRevenueMonth.Text = revenueMonth.ToString("N0") + " đ";
                lblProfitToday.Text = profitToday.ToString("N0") + " đ";
                lblProfitMonth.Text = profitMonth.ToString("N0") + " đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load thống kê: " + ex.Message);
            }
        }

        private void RefreshAll()
        {
            if (dtpFromDate.Value.Date > dtpToDate.Value.Date)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                return;
            }
            LoadChartData();
            LoadDgvData();
        }

        private void UpdateLastUpdated()
        {
            lblLastUpdated.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void btnRefreshStatistics_Click(object sender, EventArgs e)
        {
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            LoadStatistics();
            RefreshAll();
        }

        private void cbChartMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChartData();
        }

        private void cbDgvMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDgvData();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e) { }
        private void dtpToDate_ValueChanged(object sender, EventArgs e) { }
    }
}
