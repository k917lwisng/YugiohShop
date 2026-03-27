using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;

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
            OpenChildForm(new FormProducts(), btnProducts);
        }

        private void OpenChildForm(Form childForm, Guna2Button sender = null)
        {
            if (currentForm != null) currentForm.Close();

            currentForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            rightPanel.Controls.Clear();
            rightPanel.Controls.Add(childForm);
            childForm.Show();

            if (sender != null) SetActiveButton(sender);
        }

        private void SetActiveButton(Guna2Button btn)
        {
            // Reset tất cả
            Guna2Button[] navBtns = { btnProducts, btnCustomers, btnSales, btnStatistics };
            foreach (var b in navBtns)
            {
                b.FillColor = Color.Transparent;
                b.ForeColor = Color.Black;
                b.Image = GetIcon(b, isActive: false);
            }

            // Set active
            _activeBtn = btn;
            btn.FillColor = Color.FromArgb(26, 115, 232);
            btn.ForeColor = Color.White;
            btn.Image = GetIcon(btn, isActive: true);

            leftPanel.Invalidate(); // vẽ lại thanh indicator
        }

        private Image GetIcon(Guna2Button btn, bool isActive)
        {
            string iconDir = Path.GetFullPath(
        Path.Combine(Application.StartupPath, "..", "..", "..", "icons"));
            string suffix = isActive ? "white" : "black";

            string fileName = btn.Name switch
            {
                "btnProducts" => $"package_{suffix}.png",
                "btnCustomers" => $"contact_{suffix}.png",
                "btnSales" => $"sale_{suffix}.png",
                "btnStatistics" => $"analytics_{suffix}.png",
                "btnLogout" => $"logout_{suffix}.png",
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
            => OpenChildForm(new FormProducts(), btnProducts);

        private void btnCustomers_Click(object sender, EventArgs e)
            => OpenChildForm(new FormCustomers(), btnCustomers);

        private void btnSales_Click(object sender, EventArgs e)
            => OpenChildForm(new FormSales(), btnSales);

        private void btnStatistics_Click(object sender, EventArgs e)
            => OpenChildForm(new FormStatistics(), btnStatistics);

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
