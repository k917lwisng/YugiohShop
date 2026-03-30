using Guna.UI2.WinForms;
//using ScottPlot;
using ScottPlot.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace YugiohShop
{
    public partial class FormStatistics : Form
    {

        // ── Colors ────────────────────────────────────────────
        private static readonly Color CLR_ACTIVE_BG = Color.FromArgb(65, 105, 225);  // RoyalBlue
        private static readonly Color CLR_ACTIVE_FORE = Color.White;
        private static readonly Color CLR_ACTIVE_BORDER = Color.FromArgb(65, 105, 225);
        private static readonly Color CLR_INACTIVE_BG = Color.FromArgb(245, 247, 250);
        private static readonly Color CLR_INACTIVE_FORE = Color.FromArgb(80, 96, 120);
        private static readonly Color CLR_INACTIVE_BORDER = Color.FromArgb(203, 213, 225);

        private static readonly ScottPlot.Color SP_REVENUE = new ScottPlot.Color(249, 115, 22);   // Cam
        private static readonly ScottPlot.Color SP_PROFIT = new ScottPlot.Color(16, 185, 129);   // Emerald

        private Guna2Button _activeQuickRange;
        private Guna2Button _activeChartType;
        private Guna2Button _activeSeriesMode;

        private string _chartType = "line";  // "line" | "bar"
        private string _seriesMode = "both";  // "both" | "revenue" | "profit"
        private FormsPlot _formsPlot;

        // ── Constructor ───────────────────────────────────────
        public FormStatistics()
        {
            InitializeComponent();
            InitFormsPlot();
            InitButtonStyles();

            SetQuickRange(btnThangNay);      // mặc định Tháng này
            SetChartType(btnLine);           // mặc định Line
            SetSeriesMode(btnBoth);          // mặc định cả hai
            UpdateLastUpdated();
            LoadChartData();
        }

        // ── FormsPlot init ────────────────────────────────────
        private void InitFormsPlot()
        {
            _formsPlot = new FormsPlot { Dock = DockStyle.Fill };
            PanelChart.Controls.Add(_formsPlot);
        }

        // ── Button helpers ────────────────────────────────────
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
            btn.FillColor = Color.White;
            btn.ForeColor = Color.FromArgb(65, 105, 225);  // RoyalBlue text
            btn.BorderColor = Color.FromArgb(65, 105, 225);
            btn.BorderThickness = 1;
            btn.Font = new Font(btn.Font, FontStyle.Bold);
        }

        private void StyleChartTypeInactive(Guna2Button btn)
        {
            btn.FillColor = Color.FromArgb(245, 247, 250);
            btn.ForeColor = Color.FromArgb(80, 96, 120);
            btn.BorderColor = Color.FromArgb(203, 213, 225);
            btn.BorderThickness = 1;
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

            // Nút Xem - primary style
            StyleActive(btnXem);

            // Nút Refresh - ghost style
            btnRefreshStatistics.FillColor = Color.White;
            btnRefreshStatistics.ForeColor = Color.FromArgb(80, 96, 120);
            btnRefreshStatistics.BorderColor = Color.FromArgb(203, 213, 225);
            btnRefreshStatistics.BorderThickness = 1;
        }

        // ── Quick Range ───────────────────────────────────────
        private void SetQuickRange(Guna2Button active)
        {
            foreach (var b in new[] { btnHomNay, btnBayNgay, btnThangNay, btnThangTruoc })
                StyleInactive(b);
            StyleActive(active);
        }

        private void btnHomNay_Click(object sender, EventArgs e)
        {
            dtpFromDate.Value = DateTime.Today;
            dtpToDate.Value = DateTime.Today;
            SetQuickRange(btnHomNay);
            LoadChartData();
        }

        private void btnBayNgay_Click(object sender, EventArgs e)
        {
            dtpFromDate.Value = DateTime.Today.AddDays(-6);
            dtpToDate.Value = DateTime.Today;
            SetQuickRange(btnBayNgay);
            LoadChartData();
        }

        private void btnThangNay_Click(object sender, EventArgs e)
        {
            dtpFromDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpToDate.Value = DateTime.Today;
            SetQuickRange(btnThangNay);
            LoadChartData();
        }

        private void btnThangTruoc_Click(object sender, EventArgs e)
        {
            var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            dtpFromDate.Value = first;
            dtpToDate.Value = first.AddMonths(1).AddDays(-1);
            SetQuickRange(btnThangTruoc);
            LoadChartData();
        }

        // ── Chart Type (Line / Bar) ───────────────────────────
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

        // ── Series Mode (Both / Revenue / Profit) ─────────────
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

        // ── Refresh / Reset ───────────────────────────────────
        private void btnRefreshStatistics_Click(object sender, EventArgs e)
        {
            dtpFromDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpToDate.Value = DateTime.Today;
            SetQuickRange(btnThangNay);
            UpdateLastUpdated();
            LoadChartData();
        }

        // ── Last Updated label ────────────────────────────────
        private void UpdateLastUpdated()
        {
            lblLastUpdated.Text = $"Cập nhật: {DateTime.Now:HH:mm dd/MM/yyyy}";
        }

        // ── Load & Render ─────────────────────────────────────
        private void LoadChartData()
        {
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.",
                    "Lỗi khoảng thời gian", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // TODO: Thay mock data bằng query DB thật
            // LoadStatsCards();  ← gọi hàm update lblRevenueToday, lblRevenueMonth...
            RenderChart();
            UpdateLastUpdated();
        }

        private void RenderChart()
        {
            _formsPlot.Plot.Clear();

            int days = Math.Max(1, (dtpToDate.Value - dtpFromDate.Value).Days + 1);
            double[] xs = new double[days];
            double[] revenue = new double[days];
            double[] profit = new double[days];

            var rnd = new Random(42);
            for (int i = 0; i < days; i++)
            {
                xs[i] = i;
                revenue[i] = rnd.Next(3_000_000, 15_000_000);
                profit[i] = revenue[i] * (0.15 + rnd.NextDouble() * 0.25);
            }

            bool showRevenue = _seriesMode == "both" || _seriesMode == "revenue";
            bool showProfit = _seriesMode == "both" || _seriesMode == "profit";

            if (_chartType == "line")
            {
                if (showRevenue)
                {
                    var s = _formsPlot.Plot.Add.Scatter(xs, revenue);
                    s.Color = SP_REVENUE; s.LineWidth = 2.5f; s.MarkerSize = 6;
                    s.LegendText = "Doanh thu";
                }
                if (showProfit)
                {
                    var s = _formsPlot.Plot.Add.Scatter(xs, profit);
                    s.Color = SP_PROFIT; s.LineWidth = 2.5f; s.MarkerSize = 6;
                    s.LegendText = "Lợi nhuận";
                }
            }
            else
            {
                double bw = 0.35;
                for (int i = 0; i < days; i++) { xs[i] = i; }

                if (showRevenue && showProfit)
                {
                    double[] xR = new double[days], xP = new double[days];
                    for (int i = 0; i < days; i++) { xR[i] = xs[i] - bw / 2; xP[i] = xs[i] + bw / 2; }
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

            _formsPlot.Plot.ShowLegend();
            _formsPlot.Plot.Title($"{dtpFromDate.Value:dd/MM/yyyy} – {dtpToDate.Value:dd/MM/yyyy}");
            _formsPlot.Refresh();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e) { }
        private void FormStatistics_Load(object sender, EventArgs e) { }
    }
}