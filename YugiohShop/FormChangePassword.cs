using System;
using System.Data;
using System.Windows.Forms;

namespace YugiohShop
{
    public partial class FormChangePassword : Form
    {
        public FormChangePassword()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void FormChangePassword_Load(object sender, EventArgs e)
        {
            string pathMatNham = @"D:\YuGiOhShop\YugiohShop\YugiohShop\icons\visibility_off_darkgray.png";

            Control[] passBoxes = { txtOldPass, txtNewPass, txtConfirmPass };
            foreach (Guna.UI2.WinForms.Guna2TextBox txt in passBoxes)
            {
                txt.UseSystemPasswordChar = true;
                txt.PasswordChar = '\0';
                txt.IconRight = Image.FromFile(pathMatNham);
                txt.IconRightCursor = Cursors.Hand;
            }
        }

        private void TogglePassword(Guna.UI2.WinForms.Guna2TextBox txt)
        {
            string pathMatMo = @"D:\YuGiOhShop\YugiohShop\YugiohShop\icons\visibility_darkgray.png";
            string pathMatNham = @"D:\YuGiOhShop\YugiohShop\YugiohShop\icons\visibility_off_darkgray.png";

            if (txt.UseSystemPasswordChar == true)
            {
                txt.UseSystemPasswordChar = false;
                txt.PasswordChar = '\0';
                txt.IconRight = Image.FromFile(pathMatMo);
            }
            else
            {
                txt.UseSystemPasswordChar = true;
                txt.IconRight = Image.FromFile(pathMatNham);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string oldPass = txtOldPass.Text.Trim();
            string newPass = txtNewPass.Text.Trim();
            string confirmPass = txtConfirmPass.Text.Trim();

            if (oldPass == "" || newPass == "" || confirmPass == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string sqlCheck = $"SELECT PasswordHash FROM Users WHERE UserId = {CurrentUser.UserId}";
                DataTable dt = DbHelper.Query(sqlCheck);

                if (dt.Rows.Count > 0)
                {
                    string currentDbPass = dt.Rows[0]["PasswordHash"].ToString();

                    if (currentDbPass != oldPass)
                    {
                        MessageBox.Show("Mật khẩu cũ không chính xác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string sqlUpdate = $"UPDATE Users SET PasswordHash = N'{newPass}' WHERE UserId = {CurrentUser.UserId}";
                    if (DbHelper.Execute(sqlUpdate) > 0)
                    {
                        MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtOldPass_IconRightClick(object sender, EventArgs e)
        {
            TogglePassword(txtOldPass);
        }

        private void txtNewPass_IconRightClick(object sender, EventArgs e)
        {
            TogglePassword(txtNewPass);
        }

        private void txtConfirmPass_IconRightClick(object sender, EventArgs e)
        {
            TogglePassword(txtConfirmPass);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}