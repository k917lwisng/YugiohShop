using Guna.UI2.WinForms;
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
    public partial class FormProducts : Form
    {
        private List<int> selectedProductIds = new List<int>();
        private List<Panel> selectedCards = new List<Panel>();
        private Guna.UI2.WinForms.Guna2Button _activeNavBtn = null;

        public FormProducts()
        {
            InitializeComponent();
        }

        private void FormProducts_Load(object sender, EventArgs e)
        {
            cbCategoryFilter.Items.Clear();
            cbCategoryFilter.Items.Add("Tất cả");
            cbCategoryFilter.Items.Add("SingleCard");
            cbCategoryFilter.Items.Add("Pack");
            cbCategoryFilter.Items.Add("FullBox");
            cbCategoryFilter.SelectedIndex = 0;

            InitNavButtons();
            LoadProductsFromDb();
        }

        private Panel CreateProductCard(int productId, string name, string category, decimal price, int stock, string imagePath, bool isActive)
        {
            Panel card = new Panel();
            card.Width = 180;
            card.Height = 285;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Margin = new Padding(10);
            card.BackColor = isActive ? Color.White : Color.LightGray;
            card.Tag = productId;

            PictureBox pic = new PictureBox();
            pic.Width = 160;
            pic.Height = 140;
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            pic.Location = new Point(10, 10);
            pic.BackColor = Color.Gainsboro;

            string fullPath = Path.Combine(Application.StartupPath, imagePath);

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(fullPath))
            {
                pic.Image = Image.FromFile(fullPath);
            }

            Label lblName = new Label();
            lblName.Text = name;
            lblName.Location = new Point(10, 160);
            lblName.Width = 160;
            lblName.Height = 35;
            lblName.Font = new Font("Be Vietnam Pro", 10, FontStyle.Bold);

            Label lblCategory = new Label();
            lblCategory.Text = "Loại: " + category;
            lblCategory.Location = new Point(10, 195);
            lblCategory.Width = 160;
            lblCategory.Height = 20;

            Label lblPrice = new Label();
            lblPrice.Text = "Giá: " + price.ToString("N0") + " đ";
            lblPrice.Location = new Point(10, 215);
            lblPrice.Width = 160;
            lblPrice.Height = 20;
            lblPrice.ForeColor = Color.Red;
            lblPrice.Font = new Font("Be Vietnam Pro", 9, FontStyle.Bold);

            Label lblStock = new Label();
            lblStock.Text = "Tồn kho: " + stock;
            lblStock.Location = new Point(10, 235);
            lblStock.Width = 160;
            lblStock.Height = 20;

            Label lblStatus = new Label();
            lblStatus.Text = isActive ? "Trạng thái: Đang bán" : "Trạng thái: Ngừng bán";
            lblStatus.Location = new Point(10, 255);
            lblStatus.Width = 160;
            lblStatus.Height = 20;
            lblStatus.ForeColor = isActive ? Color.Green : Color.DarkRed;

            card.Controls.Add(pic);
            card.Controls.Add(lblName);
            card.Controls.Add(lblCategory);
            card.Controls.Add(lblPrice);
            card.Controls.Add(lblStock);
            card.Controls.Add(lblStatus);

            card.Click += ProductCard_Click;
            pic.Click += ProductCard_Click;
            lblName.Click += ProductCard_Click;
            lblCategory.Click += ProductCard_Click;
            lblPrice.Click += ProductCard_Click;
            lblStock.Click += ProductCard_Click;
            lblStatus.Click += ProductCard_Click;

            return card;
        }

        private void LoadProductsFromDb(string keyword = "", string category = "Tất cả")
        {
            try
            {
                ClearProductSelection();
                flpProducts.Controls.Clear();

                string sql = @"
                    SELECT 
                        p.ProductId, p.Code, p.Name, p.CardCode, p.Category, 
                        ISNULL(p.ImagePath, '') AS ImagePath,
                        p.IsActive,
                        ISNULL(SUM(v.Stock), 0)     AS TotalStock,
                        ISNULL(MIN(v.SellPrice), 0) AS MinPrice
                    FROM Products p
                    LEFT JOIN ProductVariants v ON p.ProductId = v.ProductId
                    WHERE 1 = 1";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Replace("'", "''");
                    sql += $" AND (p.Name LIKE N'%{keyword}%' OR p.CardCode LIKE '%{keyword}%')";
                }

                if (!string.IsNullOrWhiteSpace(category) && category != "Tất cả")
                {
                    category = category.Replace("'", "''");
                    sql += $" AND p.Category = N'{category}'";
                }

                sql += @" GROUP BY p.ProductId, p.Code, p.Name, p.CardCode, p.Category, p.ImagePath, p.IsActive";
                sql += @" ORDER BY p.Name ASC";

                DataTable dt = DbHelper.Query(sql);

                foreach (DataRow row in dt.Rows)
                {
                    int productId = Convert.ToInt32(row["ProductId"]);
                    string name = row["Name"].ToString();
                    string cardCategory = row["Category"].ToString();
                    decimal price = row["MinPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["MinPrice"]);
                    int stock = row["TotalStock"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalStock"]);
                    string imagePath = row["ImagePath"] == DBNull.Value ? "" : row["ImagePath"].ToString();
                    bool isActive = Convert.ToBoolean(row["IsActive"]);

                    flpProducts.Controls.Add(
                        CreateProductCard(productId, name, cardCategory, price, stock, imagePath, isActive)
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load sản phẩm: " + ex.Message);
            }
        }

        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearchProduct.Text.Trim();
            string category = cbCategoryFilter.Text;

            LoadProductsFromDb(keyword, category);
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearchProduct.Text = "";
            cbCategoryFilter.SelectedIndex = 0;
            LoadProductsFromDb();
        }

        private void btnRefreshProduct_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnRefreshProduct);
            txtSearchProduct.Text = "";
            cbCategoryFilter.SelectedIndex = 0;

            ClearProductSelection();
            LoadProductsFromDb("", "Tất cả");
        }

        private void cbCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string keyword = txtSearchProduct.Text.Trim();
            string category = cbCategoryFilter.Text;

            LoadProductsFromDb(keyword, category);
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnAddProduct);

            FormProductEditor f = new FormProductEditor();

            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadProductsFromDb(txtSearchProduct.Text.Trim(), cbCategoryFilter.Text);
            }

            SetActiveNavButton(null);
        }

        private void btnEditProduct_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnEditProduct);

            if (selectedProductIds.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!");
                return;
            }

            if (selectedProductIds.Count > 1)
            {
                MessageBox.Show("Chỉ được chọn 1 sản phẩm để sửa!");
                return;
            }

            FormProductEditor f = new FormProductEditor(selectedProductIds[0]);

            if (f.ShowDialog() == DialogResult.OK)
            {
                ClearProductSelection();
                LoadProductsFromDb(txtSearchProduct.Text.Trim(), cbCategoryFilter.Text);
            }

            SetActiveNavButton(null);
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            SetActiveNavButton(btnDeleteProduct);

            if (selectedProductIds.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm!");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn ngừng bán {selectedProductIds.Count} sản phẩm đã chọn không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (int productId in selectedProductIds)
                    {
                        string sql = $@"
                    UPDATE Products
                    SET IsActive = 0
                    WHERE ProductId = {productId}";

                        DbHelper.Execute(sql);
                    }

                    MessageBox.Show("Đã ngừng bán các sản phẩm đã chọn!");

                    selectedProductIds.Clear();
                    selectedCards.Clear();

                    LoadProductsFromDb(txtSearchProduct.Text.Trim(), cbCategoryFilter.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa sản phẩm: " + ex.Message);
                }
            }

            SetActiveNavButton(null);
        }

        private void ProductCard_Click(object sender, EventArgs e)
        {
            Control clickedControl = sender as Control;
            Panel card = clickedControl is Panel ? (Panel)clickedControl : clickedControl.Parent as Panel;

            if (card == null) return;

            int productId = Convert.ToInt32(card.Tag);
            bool alreadySelected = selectedProductIds.Contains(productId);

            if ((ModifierKeys & Keys.Control) != Keys.Control)
            {
                if (alreadySelected && selectedProductIds.Count == 1)
                {
                    ClearProductSelection(); 
                    return;
                }

                ClearProductSelection();
                selectedProductIds.Add(productId);
                selectedCards.Add(card);
                card.BackColor = Color.LightBlue;
                return;
            }

            if (alreadySelected)
            {
                selectedProductIds.Remove(productId);
                selectedCards.Remove(card);
                card.BackColor = Color.White;
            }
            else
            {
                selectedProductIds.Add(productId);
                selectedCards.Add(card);
                card.BackColor = Color.LightBlue;
            }
        }

        private void ClearProductSelection()
        {
            foreach (var card in selectedCards)
            {
                if (card != null && !card.IsDisposed)
                {
                    card.BackColor = Color.White;
                }
            }

            selectedCards.Clear();
            selectedProductIds.Clear();
        }

        private void SetActiveNavButton(Guna2Button btn)
        {
            var navBtns = new[] { btnAddProduct, btnEditProduct, btnDeleteProduct, btnRefreshProduct };

            foreach (var b in navBtns)
            {
                b.FillColor = Color.Transparent;
                b.ForeColor = Color.DimGray;
                b.Image = GetNavIcon(b, isActive: false);
            }

            if (btn != null)
            {
                _activeNavBtn = btn;
                btn.FillColor = Color.FromArgb(235, 242, 255);
                btn.ForeColor = Color.RoyalBlue;               
                btn.Image = GetNavIcon(btn, isActive: true);  
            }
        }

        private Image GetNavIcon(Guna2Button btn, bool isActive)
        {
            string iconDir = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "..", "icons"));
            string suffix = isActive ? "RoyalBlue" : "DimGray";

            string fileName = btn.Name switch
            {
                "btnAddProduct" => $"add_box_{suffix}.png",
                "btnEditProduct" => $"edit_{suffix}.png",
                "btnDeleteProduct" => $"delete_{suffix}.png",
                "btnRefreshProduct" => $"refresh_{suffix}.png",
                _ => ""
            };

            string fullPath = Path.Combine(iconDir, fileName);
            return File.Exists(fullPath) ? Image.FromFile(fullPath) : null;
        }

        private void InitNavButtons()
        {
            var navBtns = new[] { btnAddProduct, btnEditProduct, btnDeleteProduct, btnRefreshProduct };

            foreach (var b in navBtns)
            {
                b.CheckedState.FillColor = Color.Empty;
                b.CheckedState.ForeColor = Color.Empty;

                b.HoverState.FillColor = Color.FromArgb(242, 245, 250);
                b.HoverState.ForeColor = Color.DimGray;

                b.FillColor = Color.Transparent;
                b.ForeColor = Color.DimGray;
                b.BorderThickness = 0;
                b.BorderColor = Color.Transparent;
                b.Image = GetNavIcon(b, isActive: false);
            }
        }
    }
}
