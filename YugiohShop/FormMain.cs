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
            ApplyRoundedClip(rightPanel, 15);

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, ev) => UpdateDateNow();
            timer.Start();

            UpdateDateNow();

            OpenChildForm(new FormDashboard(), "Dashboard", btnDashboard);

        }

        private void ApplyRoundedClip(Control ctrl, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(ctrl.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(ctrl.Width - radius * 2, ctrl.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, ctrl.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseAllFigures();
            ctrl.Region = new Region(path);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (rightPanel != null)
                ApplyRoundedClip(rightPanel, 15);
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
            Guna2Button[] navBtns = { btnProducts, btnCustomers, btnSales, btnStatistics, btnDashboard };

            // Reset tất cả về trạng thái ban đầu
            foreach (var b in navBtns)
            {
                b.FillColor = Color.Transparent;
                b.ForeColor = Color.DimGray;
                b.Image = GetIcon(b, isActive: false);
            }

            // Active button
            _activeBtn = btn;
            btn.FillColor = Color.FromArgb(235, 242, 255);        // xanh nhạt nền
            btn.ForeColor = Color.RoyalBlue;                       // chữ royal blue
            btn.Image = GetIcon(btn, isActive: true);              // icon đổi sang royal blue

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
            => OpenChildForm(new FormProducts(), "Sản phẩm", btnProducts);

        private void btnCustomers_Click(object sender, EventArgs e)
            => OpenChildForm(new FormCustomers(), "Khách hàng", btnCustomers);

        private void btnSales_Click(object sender, EventArgs e)
            => OpenChildForm(new FormSales(), "Bán hàng", btnSales);

        private void btnStatistics_Click(object sender, EventArgs e)
            => OpenChildForm(new FormStatistics(), "Thống kê", btnStatistics);

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
                MessageBox.Show("Chức năng đổi mật khẩu (chưa làm)"));
            menu.Items.Add("Đăng xuất", null, (s, ev) => {
                this.Hide();
                new FormLogin().Show();
            });
            menu.Show(btnUser, 0, btnUser.Height);
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
    }
}
