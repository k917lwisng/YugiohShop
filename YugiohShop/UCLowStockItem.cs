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
    public partial class UCLowStockItem : UserControl
    {
        public UCLowStockItem()
        {
            InitializeComponent();
        }

        public void SetData(string productName, int stock, int threshold)
        {
            lblProductName.Text = productName;
            lblQuantity.Text = $"Còn lại: {stock}";

            int half = threshold / 2;
            bool isDanger = stock <= half;

            if (isDanger)
            {
                progressStock.ProgressColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "⚠ Nguy hiểm";
                lblStatus.ForeColor = Color.FromArgb(185, 28, 28);
                lblStatus.BackColor = Color.FromArgb(254, 226, 226);
            }
            else
            {
                progressStock.ProgressColor = Color.FromArgb(249, 115, 22);
                lblStatus.Text = "⚠ Cảnh báo";
                lblStatus.ForeColor = Color.FromArgb(180, 75, 10);
                lblStatus.BackColor = Color.FromArgb(255, 237, 213);
            }

            int maxRef = Math.Max(threshold * 2, 1);
            int val = (int)Math.Min(100.0, (double)stock / maxRef * 100);
            progressStock.Value = Math.Max(1, val);
        }
    }
}
