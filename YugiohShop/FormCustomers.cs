using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace YugiohShop
{
    public partial class FormCustomers : Form
    {
        private int? selectedCustomerId = null;
        private bool _isLoading = false;
        private bool _suppressSearch = false;
        public bool IsSelectMode { get; set; } = false;

        public int SelectedCustomerId { get; private set; }
        public string SelectedCustomerName { get; private set; }
        public string SelectedCustomerPhone { get; private set; }
        public int SelectedCustomerPoints { get; private set; }

        public FormCustomers()
        {
            InitializeComponent();
        }

        private void FormCustomers_Load(object sender, EventArgs e)
        {
            _suppressSearch = true;

            txtSearchCustomer.Text = "";
            txtSearchCustomer.ForeColor = Color.Black;
            txtPhone.Text = "";
            txtPhone.ForeColor = Color.Black;
            txtName.Text = "";
            txtName.ForeColor = Color.Black;

                LoadCustomers("");
            ClearCustomerInputs();

            _suppressSearch = false;
        }

        private void LoadCustomers(string keyword = "")
        {
            try
            {
                _isLoading = true;

                keyword = keyword.Trim().Replace("'", "''");

                string sql = @"
            SELECT CustomerId, Phone, Name, Points, CreatedAt
            FROM Customers
            WHERE 1 = 1";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    sql += $@"
                AND (
                    Phone LIKE N'%{keyword}%'
                    OR Name LIKE N'%{keyword}%'
                )";
                }

                sql += " ORDER BY CustomerId DESC";

                DataTable dt = DbHelper.Query(sql);
                dgvCustomers.DataSource = dt;
                dgvCustomers.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load khách hàng: " + ex.Message);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void ClearCustomerInputs()
        {
            selectedCustomerId = null;
            _suppressSearch = true;
            txtPhone.Text = "";
            txtName.Text = "";
            lblPoints.Text = "0";
            dgvCustomers.ClearSelection();
            _suppressSearch = false;
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_isLoading || e.RowIndex < 0) return;

            DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];

            // Kiểm tra null trước khi convert
            if (row.Cells["CustomerId"].Value == null || row.Cells["CustomerId"].Value == DBNull.Value) return;

            selectedCustomerId = Convert.ToInt32(row.Cells["CustomerId"].Value);
            txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";
            txtName.Text = row.Cells["Name"].Value?.ToString() ?? "";
            lblPoints.Text = row.Cells["Points"].Value?.ToString() ?? "0";
        }

        private void dgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (IsSelectMode)
            {
                SelectCurrentCustomer();
                return;
            }

            dgvCustomers_CellClick(sender, e);
        }

        private void txtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
            if (_suppressSearch || _isLoading) return;
            LoadCustomers(txtSearchCustomer.Text.Trim());
        }

        private void btnEditCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedCustomerId == null)
                {
                    MessageBox.Show("Vui lòng click chọn khách hàng cần sửa!");
                    return;
                }

                string phone = txtPhone.Text.Trim().Replace("'", "''");
                string name = txtName.Text.Trim().Replace("'", "''");

                if (phone == "" || name == "")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                int count = Convert.ToInt32(DbHelper.Query(
                    $"SELECT COUNT(*) FROM Customers WHERE Phone=N'{phone}' AND CustomerId<>{selectedCustomerId.Value}"
                ).Rows[0][0]);

                if (count > 0)
                {
                    MessageBox.Show("SĐT đã thuộc khách hàng khác!");
                    return;
                }

                if (DbHelper.Execute(
                    $"UPDATE Customers SET Phone=N'{phone}', Name=N'{name}' WHERE CustomerId={selectedCustomerId.Value}"
                ) > 0)
                {
                    MessageBox.Show("Sửa thành công!");
                    _suppressSearch = true;
                    txtSearchCustomer.Text = "";
                    _suppressSearch = false;
                    LoadCustomers("");
                    ClearCustomerInputs();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                string phone = txtPhone.Text.Trim().Replace("'", "''");
                string name = txtName.Text.Trim().Replace("'", "''");

                if (phone == "" || name == "")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                int count = Convert.ToInt32(DbHelper.Query(
                    $"SELECT COUNT(*) FROM Customers WHERE Phone=N'{phone}'"
                ).Rows[0][0]);

                if (count > 0) { MessageBox.Show("SĐT đã tồn tại!"); return; }

                if (DbHelper.Execute(
                    $"INSERT INTO Customers(Phone,Name,Points,CreatedAt) VALUES(N'{phone}',N'{name}',0,GETDATE())"
                ) > 0)
                {
                    MessageBox.Show("Thêm thành công!");
                    _suppressSearch = true;
                    txtSearchCustomer.Text = "";
                    _suppressSearch = false;
                    LoadCustomers("");
                    ClearCustomerInputs();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void SelectCurrentCustomer()
        {
            if (selectedCustomerId == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!");
                return;
            }

            SelectedCustomerId = selectedCustomerId.Value;
            SelectedCustomerPhone = txtPhone.Text.Trim();
            SelectedCustomerName = txtName.Text.Trim();

            int points = 0;
            int.TryParse(lblPoints.Text.Trim(), out points);
            SelectedCustomerPoints = points;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}