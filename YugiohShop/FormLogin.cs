using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace YugiohShop
{
    public partial class FormLogin : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private const int EM_SETCUEBANNER = 0x1501;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Tên đăng nhập")
            {
                txtUsername.Text = "";
                txtUsername.ForeColor = Color.Black;
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "Tên đăng nhập";
                txtUsername.ForeColor = Color.Gray;
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Mật khẩu")
            {
                txtPassword.Text = "";
                txtPassword.ForeColor = Color.Black;
                // Bật che mật khẩu bằng dấu chấm tròn
                txtPassword.PasswordChar = '●';
                txtPassword.UseSystemPasswordChar = false;
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                // Tắt che mật khẩu để hiện chữ mờ
                txtPassword.PasswordChar = '\0';
                txtPassword.Text = "Mật khẩu";
                txtPassword.ForeColor = Color.Gray;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (username == "" || username == "Tên đăng nhập" || password == "" || password == "Mật khẩu")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!");
                    return;
                }

                string sql = $@"
                                SELECT UserId, Username, Role, IsActive
                                FROM Users
                                WHERE Username = N'{username}'
                                AND PasswordHash = N'{password}'";

                DataTable dt = DbHelper.Query(sql);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!");
                    return;
                }

                bool isActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);

                if (isActive == false)
                {
                    MessageBox.Show("Tài khoản này đã bị khóa!");
                    return;
                }

                CurrentUser.UserId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                CurrentUser.Username = dt.Rows[0]["Username"].ToString();
                CurrentUser.Role = dt.Rows[0]["Role"].ToString();

                MessageBox.Show("Đăng nhập thành công!");

                FormMain main = new FormMain();
                main.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = false;
            txtPassword.PasswordChar = '\0';           
            txtPassword.Text = "Mật khẩu";
            txtPassword.ForeColor = Color.Gray;

            string pathMatNham = @"D:\YuGiOhShop\YugiohShop\YugiohShop\icons\visibility_off_darkgray.png";
            txtPassword.IconRight = Image.FromFile(pathMatNham);
            txtPassword.IconRightCursor = Cursors.Hand;

            PanelLogin.Location = new Point(
                (this.ClientSize.Width - PanelLogin.Width) / 2,
                (this.ClientSize.Height - PanelLogin.Height) / 2

            );
            PanelLoginDepth.Location = new Point(
                (this.ClientSize.Width - PanelLogin.Width) / 2,
                (this.ClientSize.Height - PanelLogin.Height) / 2

            );
        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private void txtPassword_IconRightClick(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Mật khẩu") return;

            string pathMatMo = @"D:\YuGiOhShop\YugiohShop\YugiohShop\icons\visibility_darkgray.png";
            string pathMatNham = @"D:\YuGiOhShop\YugiohShop\YugiohShop\icons\visibility_off_darkgray.png";

            if (txtPassword.PasswordChar == '●')
            {
                txtPassword.PasswordChar = '\0';
                txtPassword.IconRight = Image.FromFile(pathMatMo);
            }
            else
            {
                txtPassword.PasswordChar = '●';
                txtPassword.IconRight = Image.FromFile(pathMatNham);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát hoàn toàn phần mềm?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
