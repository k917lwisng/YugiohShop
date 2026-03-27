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

        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                // Khi rỗng: tắt ẩn để hiện placeholder text
                txtPassword.UseSystemPasswordChar = false;
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
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Text = "";
            txtPassword.ForeColor = Color.Black;
        }
    }
}
