using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Timer = System.Windows.Forms.Timer;

namespace YugiohShop
{
    public partial class FormMain : Form
    {
        private Form currentForm = null;
        private Guna2Button _activeBtn = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, ev) => UpdateDateNow();
            timer.Start();

            UpdateDateNow();

            if (cboDateRange.Items.Count == 0)
            {
                cboDateRange.Items.AddRange(new[] { "Hôm nay", "7 ngày", "Tháng này", "Tháng trước" });
                cboDateRange.SelectedIndex = 2; 
            }


            if (CurrentUser.Role != null && (CurrentUser.Role.ToLower() == "staff"))
            {
                OpenChildForm(new FormSales(), "Bán hàng", btnSales);
                cboDateRange.Visible = false;
            }
            else
            {
                OpenChildForm(new FormDashboard(), "Dashboard", btnDashboard);
                cboDateRange.Visible = true;
            }
        }

        private void UpdateDateNow()
        {
            lblDateNow.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy - HH:mm", new System.Globalization.CultureInfo("vi-VN"));
        }

        private void OpenChildForm(Form childForm, string title, Guna2Button sender = null)
        {
            if (currentForm != null) currentForm.Close();

            currentForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            rightPanel.Controls.Clear();
            rightPanel.Controls.Add(childForm);
            childForm.Show();

            lblTitleControl.Text = title;

            if (sender != null) SetActiveButton(sender);
        }

        private void SetActiveButton(Guna2Button btn)
        {
            Guna2Button[] navBtns = { btnProducts, btnCustomers, btnSales, btnOrderHistory, btnStatistics, btnDashboard };

            foreach (var b in navBtns)
            {
                b.FillColor = Color.Transparent;
                b.ForeColor = Color.DimGray;
                b.Image = GetIcon(b, isActive: false);
            }

            _activeBtn = btn;
            btn.FillColor = Color.FromArgb(235, 242, 255);       
            btn.ForeColor = Color.RoyalBlue;                       
            btn.Image = GetIcon(btn, isActive: true);              

            leftPanel.Invalidate();
        }

        private Image GetIcon(Guna2Button btn, bool isActive)
        {
            string iconDir = Path.GetFullPath(
        Path.Combine(Application.StartupPath, "..", "..", "..", "icons"));
            string suffix = isActive ? "RoyalBlue" : "DimGray";

            string fileName = btn.Name switch
            {
                "btnProducts" => $"box_{suffix}.png",
                "btnCustomers" => $"contact_{suffix}.png",
                "btnSales" => $"shopping_cart_{suffix}.png",
                "btnOrderHistory" => $"receipt_long_{suffix}.png",
                "btnStatistics" => $"report_{suffix}.png",
                "btnLogout" => $"logout_{suffix}.png",
                "btnDashboard" => $"dashboard_{suffix}.png",
                _ => ""
            };

            string fullPath = Path.Combine(iconDir, fileName);
            return File.Exists(fullPath) ? Image.FromFile(fullPath) : null;
        }

        private void leftPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_activeBtn == null) return;
            using var brush = new SolidBrush(Color.White);
            int y = _activeBtn.Top + 10;
            int h = _activeBtn.Height - 20;
            e.Graphics.FillRectangle(brush, 0, y, 4, h);
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role != null && (CurrentUser.Role.ToLower() == "staff" || CurrentUser.Role.ToLower() == "nhân viên"))
            {
                MessageBox.Show("Rất tiếc! Bạn không có quyền truy cập vào chức năng Quản lý Sản phẩm.", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            cboDateRange.Visible = false;

            OpenChildForm(new FormProducts(), "Sản phẩm", btnProducts);
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            cboDateRange.Visible = false;

            OpenChildForm(new FormCustomers(), "Khách hàng", btnCustomers);
        }
        private void btnSales_Click(object sender, EventArgs e)
        {
            cboDateRange.Visible = false;

            OpenChildForm(new FormSales(), "Bán hàng", btnSales);
        }
        private void btnStatistics_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role != null && (CurrentUser.Role.ToLower() == "staff" || CurrentUser.Role.ToLower() == "nhân viên"))
            {
                MessageBox.Show("Rất tiếc! Bạn không có quyền truy cập vào chức năng Thống kê.", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            cboDateRange.Visible = false;

            OpenChildForm(new FormStatistics(), "Thống kê", btnStatistics);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất không?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                new FormLogin().Show();
                this.Hide();
            }
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add("Đổi mật khẩu", null, (s, ev) =>
            {
                new FormChangePassword().ShowDialog();
            });

            menu.Items.Add("Đăng xuất", null, (s, ev) =>
            {
                this.Hide();
                new FormLogin().Show();
            });

            menu.Show(btnUser, 0, btnUser.Height);
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role != null && (CurrentUser.Role.ToLower() == "staff" || CurrentUser.Role.ToLower() == "nhân viên"))
            {
                MessageBox.Show("Rất tiếc! Bạn không có quyền truy cập vào chức năng Dashboard.", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblTitleControl.Text = "Dashboard";

            cboDateRange.Visible = true;

            OpenChildForm(new FormDashboard(), "Dashboard");
            SetActiveButton(btnDashboard);
        }

        private void cboDateRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dashboard = rightPanel.Controls
                .OfType<FormDashboard>()
                .FirstOrDefault();

            dashboard?.OnDateRangeChanged(cboDateRange.SelectedItem?.ToString());
        }

        private void btnOrderHistory_Click(object sender, EventArgs e)
        {
            cboDateRange.Visible = false; 
            OpenChildForm(new FormOrderHistory(), "Hóa đơn", btnOrderHistory);
        }
    }
}
