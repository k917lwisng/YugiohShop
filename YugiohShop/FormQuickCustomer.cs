using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YugiohShop
{
    public partial class FormQuickCustomer : Form
    {
        public string CustomerName => txtQuickCustomerName.Text.Trim();
        public string CustomerPhone => txtQuickCustomerPhone.Text.Trim();

        public FormQuickCustomer(string phone = "")
        {
            InitializeComponent();
            txtQuickCustomerPhone.Text = phone;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void btnQuickSave_Click(object sender, EventArgs e)
        {
            if (txtQuickCustomerName.Text.Trim() == "" || txtQuickCustomerPhone.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập tên và số điện thoại!");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnQuickCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
