using ClosedXML.Excel;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace YugiohShop
{
    public partial class FormOrderHistory : Form
    {
        public FormOrderHistory()
        {
            InitializeComponent();
        }

        private void FormOrderHistory_Load(object sender, EventArgs e)
        {
            SetupGrids();
            LoadFilters();

            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            LoadOrders();
        }

        private void SetupGrids()
        {
            dgvOrders.Columns.Clear();
            dgvOrders.AutoGenerateColumns = false;
            dgvOrders.AllowUserToAddRows = false;
            dgvOrders.ReadOnly = true;
            dgvOrders.RowHeadersVisible = false;
            dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvOrders.ColumnHeadersHeight = 40;
            dgvOrders.RowTemplate.Height = 40;
            dgvOrders.EnableHeadersVisualStyles = false;
            dgvOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(47, 128, 237);
            dgvOrders.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Be Vietnam Pro", 10, FontStyle.Bold);

            dgvOrders.DefaultCellStyle.Font = new Font("Be Vietnam Pro", 10);
            dgvOrders.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvOrders.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvOrders.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "SaleId", DataPropertyName = "SaleId", Visible = false });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "SaleDate", HeaderText = "Thời gian", DataPropertyName = "SaleDate", FillWeight = 35, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerName", HeaderText = "Khách hàng", DataPropertyName = "CustomerName", FillWeight = 40 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Tổng tiền", DataPropertyName = "Total", FillWeight = 30, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "Note", HeaderText = "Thanh toán", DataPropertyName = "Note", FillWeight = 35 });

            DataGridViewButtonColumn btnPrintCol = new DataGridViewButtonColumn();
            btnPrintCol.Name = "btnPrint";
            btnPrintCol.HeaderText = "";
            btnPrintCol.Text = "🖨️ In";
            btnPrintCol.UseColumnTextForButtonValue = true;
            btnPrintCol.FillWeight = 15;
            dgvOrders.Columns.Add(btnPrintCol);

            dgvOrders.CellClick -= DgvOrders_CellClick;
            dgvOrders.CellClick += DgvOrders_CellClick;


            dgvOrderDetails.Columns.Clear();
            dgvOrderDetails.AutoGenerateColumns = false;
            dgvOrderDetails.AllowUserToAddRows = false;
            dgvOrderDetails.ReadOnly = true;
            dgvOrderDetails.RowHeadersVisible = false;
            dgvOrderDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvOrderDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvOrderDetails.ColumnHeadersHeight = 40;
            dgvOrderDetails.RowTemplate.Height = 40;  
            dgvOrderDetails.EnableHeadersVisualStyles = false;
            dgvOrderDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(47, 128, 237);
            dgvOrderDetails.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvOrderDetails.ColumnHeadersDefaultCellStyle.Font = new Font("Be Vietnam Pro", 10, FontStyle.Bold);

            dgvOrderDetails.DefaultCellStyle.Font = new Font("Be Vietnam Pro", 10); 
            dgvOrderDetails.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvOrderDetails.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvOrderDetails.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProductName", HeaderText = "Sản phẩm", DataPropertyName = "Name", FillWeight = 50 });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qty", HeaderText = "SL", DataPropertyName = "Qty", FillWeight = 15, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "LineTotal", HeaderText = "Thành tiền", DataPropertyName = "LineTotal", FillWeight = 35, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
        }

        private void LoadFilters()
        {
            cbPaymentType.Items.Clear();
            cbPaymentType.Items.Add("Tất cả");
            cbPaymentType.Items.Add("Thanh toán Tiền mặt");
            cbPaymentType.Items.Add("Thanh toán qua QR");
            cbPaymentType.SelectedIndex = 0;
        }

        private void LoadOrders()
        {
            try
            {
                using var conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string sql = @"
                    SELECT 
                        s.SaleId, 
                        s.SaleDate, 
                        ISNULL(c.Name, N'Khách lẻ') AS CustomerName, 
                        s.Total, 
                        s.Note
                    FROM SalesInvoices s
                    LEFT JOIN Customers c ON s.CustomerId = c.CustomerId
                    WHERE 1=1 ";

                string keyword = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    sql += " AND (c.Name LIKE @keyword OR c.Phone LIKE @keyword OR CAST(s.SaleId AS VARCHAR) = @keyword)";
                }

                if (cbPaymentType.SelectedIndex > 0)
                {
                    sql += " AND s.Note = @paymentType";
                }

                sql += " AND CAST(s.SaleDate AS DATE) >= @fromDate AND CAST(s.SaleDate AS DATE) <= @toDate";

                sql += " ORDER BY s.SaleDate DESC";

                using var cmd = new SqlCommand(sql, conn);

                if (!string.IsNullOrEmpty(keyword))
                    cmd.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                if (cbPaymentType.SelectedIndex > 0)
                    cmd.Parameters.AddWithValue("@paymentType", cbPaymentType.Text);

                cmd.Parameters.AddWithValue("@fromDate", dtpFromDate.Value.Date);
                cmd.Parameters.AddWithValue("@toDate", dtpToDate.Value.Date);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                dgvOrders.DataSource = dt;

                dgvOrderDetails.DataSource = null;
                lblDetailTitle.Text = "Chọn một đơn hàng để xem chi tiết";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách đơn hàng: " + ex.Message);
            }
        }

        private void LoadOrderDetails(int saleId)
        {
            try
            {
                using var conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string sql = @"
                    SELECT p.Name, d.Qty, d.LineTotal
                    FROM SalesDetails d
                    INNER JOIN Products p ON d.ProductId = p.ProductId
                    WHERE d.SaleId = @SaleId";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SaleId", saleId);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                dgvOrderDetails.DataSource = dt;

                lblDetailTitle.Text = $"Chi tiết đơn hàng #{saleId}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết đơn: " + ex.Message);
            }
        }

        private void DgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int saleId = Convert.ToInt32(dgvOrders.Rows[e.RowIndex].Cells["SaleId"].Value);

            if (dgvOrders.Columns[e.ColumnIndex].Name == "btnPrint")
            {
                ReprintBill(saleId);
            }
            else
            {
                LoadOrderDetails(saleId);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            cbPaymentType.SelectedIndex = 0;
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvOrders.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime fromDate = dtpFromDate.Value;
            DateTime toDate = dtpToDate.Value;

            string fileName = $"LichSuDonHang_{fromDate:ddMMyyyy}_{toDate:ddMMyyyy}.xlsx";

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Workbook|*.xlsx";
            sfd.FileName = fileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("Lịch sử Đơn hàng");

                        ws.Cell("A1").Value = "BÁO CÁO LỊCH SỬ ĐƠN HÀNG";
                        ws.Range("A1:D1").Merge().Style.Font.SetBold().Font.FontSize = 16;
                        ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("A2").Value = $"Từ ngày: {fromDate:dd/MM/yyyy} - Đến ngày: {toDate:dd/MM/yyyy}";
                        ws.Range("A2:D2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range("A2:D2").Style.Font.SetItalic();

                        int excelCol = 1;
                        for (int i = 1; i < dgvOrders.Columns.Count - 1; i++)
                        {
                            ws.Cell(4, excelCol).Value = dgvOrders.Columns[i].HeaderText;
                            ws.Cell(4, excelCol).Style.Font.Bold = true;
                            ws.Cell(4, excelCol).Style.Fill.BackgroundColor = XLColor.RoyalBlue;
                            ws.Cell(4, excelCol).Style.Font.FontColor = XLColor.White;
                            excelCol++;
                        }

                        for (int i = 0; i < dgvOrders.Rows.Count; i++)
                        {
                            excelCol = 1;
                            for (int j = 1; j < dgvOrders.Columns.Count - 1; j++)
                            {
                                ws.Cell(i + 5, excelCol).Value = dgvOrders.Rows[i].Cells[j].Value?.ToString() ?? "";
                                excelCol++;
                            }
                        }

                        ws.Columns().AdjustToContents();
                        workbook.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi");
                }
            }
        }

        private void ReprintBill(int saleId)
        {
            DataTable invoiceInfo = new DataTable();
            DataTable detailsInfo = new DataTable();

            using (var conn = new SqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM SalesInvoices WHERE SaleId = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", saleId);
                    new SqlDataAdapter(cmd).Fill(invoiceInfo);
                }

                using (var cmd = new SqlCommand("SELECT p.Name, d.Qty, d.UnitSellPrice, d.LineTotal FROM SalesDetails d JOIN Products p ON d.ProductId = p.ProductId WHERE d.SaleId = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", saleId);
                    new SqlDataAdapter(cmd).Fill(detailsInfo);
                }
            }

            if (invoiceInfo.Rows.Count == 0) return;

            DataRow inv = invoiceInfo.Rows[0];
            string custName = inv["CustomerName"] == DBNull.Value ? "Khách lẻ" : inv["CustomerName"].ToString();
            DateTime saleDate = Convert.ToDateTime(inv["SaleDate"]);
            decimal total = Convert.ToDecimal(inv["Total"]);
            decimal discount = Convert.ToDecimal(inv["Discount"]);
            int earnedPoints = Convert.ToInt32(inv["PointsEarned"]);

            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("POS Receipt", 315, 600);

            pd.PrintPage += (sender, e) =>
            {
                Graphics g = e.Graphics;
                Font fontTitle = new Font("Courier New", 14, FontStyle.Bold);
                Font fontRegular = new Font("Courier New", 9, FontStyle.Regular);
                Font fontBold = new Font("Courier New", 9, FontStyle.Bold);
                Brush brush = Brushes.Black;
                int y = 10;
                int left = 10;
                int center = pd.DefaultPageSettings.PaperSize.Width / 2;
                StringFormat formatCenter = new StringFormat { Alignment = StringAlignment.Center };
                StringFormat formatRight = new StringFormat { Alignment = StringAlignment.Far };

                g.DrawString("VUATROICHO - YUGIOH SHOP", fontTitle, brush, center, y, formatCenter); y += 25;
                g.DrawString("HÓA ĐƠN BÁN HÀNG (BẢN SAO)", fontBold, brush, center, y, formatCenter); y += 25;
                g.DrawString($"Mã HĐ: {saleId}", fontRegular, brush, left, y); y += 15;
                g.DrawString($"Ngày : {saleDate:dd/MM/yyyy HH:mm}", fontRegular, brush, left, y); y += 15;
                g.DrawString($"Khách: {custName}", fontRegular, brush, left, y); y += 20;
                g.DrawString("---------------------------------", fontRegular, brush, left, y); y += 15;

                foreach (DataRow row in detailsInfo.Rows)
                {
                    string name = row["Name"].ToString();
                    name = name.Length > 20 ? name.Substring(0, 20) + "..." : name;
                    g.DrawString(name, fontBold, brush, left, y); y += 15;

                    g.DrawString($"{row["Qty"]} x {Convert.ToDecimal(row["UnitSellPrice"]):N0}", fontRegular, brush, left, y);
                    g.DrawString(Convert.ToDecimal(row["LineTotal"]).ToString("N0"), fontRegular, brush, pd.DefaultPageSettings.PaperSize.Width - 15, y, formatRight);
                    y += 20;
                }

                g.DrawString("---------------------------------", fontRegular, brush, left, y); y += 20;
                g.DrawString("Giảm giá:", fontRegular, brush, left, y);
                g.DrawString("-" + discount.ToString("N0"), fontRegular, brush, pd.DefaultPageSettings.PaperSize.Width - 15, y, formatRight); y += 20;
                g.DrawString("TỔNG CỘNG:", fontTitle, brush, left, y);
                g.DrawString(total.ToString("N0"), fontTitle, brush, pd.DefaultPageSettings.PaperSize.Width - 15, y, formatRight); y += 30;
                g.DrawString("Cảm ơn quý khách và hẹn gặp lại!", fontRegular, brush, center, y, formatCenter);
            };

            PrintPreviewDialog ppd = new PrintPreviewDialog();
            ppd.Document = pd;
            ppd.ShowDialog();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void cbPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            LoadOrders();
        }
    }
}