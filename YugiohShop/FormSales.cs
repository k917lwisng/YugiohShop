using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using PayOS;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YugiohShop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Drawing.Printing;

namespace YugiohShop
{
    public partial class FormSales : Form
    {
        private List<CartItem> cart = new List<CartItem>();

        private int? selectedCartProductId = null;
        private int? selectedCustomerId = null;
        private int currentCustomerPoints = 0;
        private string currentCustomerName = "";
        private string currentCustomerPhone = "";

        private bool isRefreshing = false;

        public FormSales()
        {
            InitializeComponent();
        }

        private void FormSales_Load(object sender, EventArgs e)
        {
            SetupCartGrid();
            LoadSalesProducts();
            LoadAllFilters();
            ResetCustomerInfo();
            UpdateCartSummary();

            StyleCartButtons();
        }

        private void SetupCartGrid()
        {
            dgvCart.Columns.Clear();
            dgvCart.AutoGenerateColumns = false;
            dgvCart.AllowUserToAddRows = false;
            dgvCart.AllowUserToDeleteRows = false;
            dgvCart.ReadOnly = true;
            dgvCart.MultiSelect = false;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.RowHeadersVisible = false;
            dgvCart.BackgroundColor = Color.White;
            dgvCart.BorderStyle = BorderStyle.None;
            dgvCart.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCart.EnableHeadersVisualStyles = false;
            dgvCart.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvCart.ColumnHeadersHeight = 38;
            dgvCart.RowTemplate.Height = 34;
            dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvCart.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(47, 128, 237);
            dgvCart.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Be Vietnam Pro", 10, FontStyle.Bold);
            dgvCart.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvCart.DefaultCellStyle.Font = new Font("Be Vietnam Pro", 10);
            dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvCart.DefaultCellStyle.BackColor = Color.White;
            dgvCart.DefaultCellStyle.ForeColor = Color.Black;
            dgvCart.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductId",
                DataPropertyName = "ProductId",
                Visible = false
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Sản phẩm",
                DataPropertyName = "Name",
                FillWeight = 40
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VariantId",
                DataPropertyName = "VariantId",
                Visible = false
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Rarity",
                HeaderText = "Độ hiếm",
                DataPropertyName = "Rarity",
                FillWeight = 20
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Giá bán",
                DataPropertyName = "SellPrice",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Qty",
                HeaderText = "SL",
                DataPropertyName = "Quantity",
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Thành tiền",
                DataPropertyName = "LineTotal",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCart.CellClick -= dgvCart_CellClick;
            dgvCart.CellClick += dgvCart_CellClick;

            dgvCart.SelectionChanged -= dgvCart_SelectionChanged;
            dgvCart.SelectionChanged += dgvCart_SelectionChanged;

            dgvCart.CellDoubleClick -= dgvCart_CellDoubleClick;
            dgvCart.CellDoubleClick += dgvCart_CellDoubleClick;

        }

        private void dgvCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            selectedCartProductId = Convert.ToInt32(
                dgvCart.Rows[e.RowIndex].Cells["ProductId"].Value
            );
        }

        private void dgvCart_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count > 0)
            {
                selectedCartProductId = Convert.ToInt32(
                    dgvCart.SelectedRows[0].Cells["ProductId"].Value
                );
            }
        }

        private void LoadAllFilters()
        {
            cbSalesCategoryFilter.Items.Clear();
            cbSalesCategoryFilter.Items.Add("Tất cả");
            cbSalesCategoryFilter.Items.Add("SingleCard");
            cbSalesCategoryFilter.Items.Add("Pack");
            cbSalesCategoryFilter.Items.Add("FullBox");
            cbSalesCategoryFilter.SelectedIndex = 0;

            cbRarityFilter.Items.Clear();
            cbRarityFilter.Items.Add("Tất cả");
            cbRarityFilter.Items.Add("Common");
            cbRarityFilter.Items.Add("Rare");
            cbRarityFilter.Items.Add("Super Rare");
            cbRarityFilter.Items.Add("Ultra Rare");
            cbRarityFilter.Items.Add("Secret Rare");
            cbRarityFilter.Items.Add("Prismatic Secret Rare");
            cbRarityFilter.SelectedIndex = 0;

            cbAttributeFilter.Items.Clear();
            cbAttributeFilter.Items.Add("Tất cả");
            cbAttributeFilter.Items.Add("DARK");
            cbAttributeFilter.Items.Add("LIGHT");
            cbAttributeFilter.Items.Add("FIRE");
            cbAttributeFilter.Items.Add("WATER");
            cbAttributeFilter.Items.Add("EARTH");
            cbAttributeFilter.Items.Add("WIND");
            cbAttributeFilter.Items.Add("DIVINE");
            cbAttributeFilter.SelectedIndex = 0;

            cbCardTypeFilter.Items.Clear();
            cbCardTypeFilter.Items.Add("Tất cả");
            cbCardTypeFilter.Items.Add("Monster");
            cbCardTypeFilter.Items.Add("Spell");
            cbCardTypeFilter.Items.Add("Trap");
            cbCardTypeFilter.SelectedIndex = 0;
        }

        private void LoadSalesProducts(
    string keyword = "",
    string category = "Tất cả",
    string rarity = "Tất cả",
    string attribute = "Tất cả",
    string cardType = "Tất cả")
        {
            try
            {
                flpSalesProducts.Controls.Clear();

                using var conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string sql = @"
            SELECT DISTINCT
                p.ProductId, p.Code, p.CardCode, p.Name, p.Category, p.ImagePath,
                ISNULL(SUM(v.Stock), 0) AS TotalStock,
                ISNULL(MIN(v.SellPrice), 0) AS MinPrice
            FROM Products p
            INNER JOIN ProductVariants v ON p.ProductId = v.ProductId
            WHERE p.IsActive = 1
            AND v.Stock > 0
            AND (p.Name LIKE @keyword OR p.CardCode LIKE @keyword)";

                if (category != "Tất cả")
                    sql += " AND p.Category = @category";

                if (rarity != "Tất cả")
                    sql += " AND v.Rarity = @rarity";

                if (attribute != "Tất cả")
                    sql += " AND p.Attribute = @attribute";

                if (cardType != "Tất cả")
                    sql += " AND p.CardType = @cardType";

                sql += @" GROUP BY p.ProductId, p.Code, p.CardCode, p.Name, p.Category, p.ImagePath
                  ORDER BY p.Name";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                if (category != "Tất cả") cmd.Parameters.AddWithValue("@category", category);
                if (rarity != "Tất cả") cmd.Parameters.AddWithValue("@rarity", rarity);
                if (attribute != "Tất cả") cmd.Parameters.AddWithValue("@attribute", attribute);
                if (cardType != "Tất cả") cmd.Parameters.AddWithValue("@cardType", cardType);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    int productId = Convert.ToInt32(row["ProductId"]);
                    string code = row["Code"].ToString();
                    string cardCode = row["CardCode"].ToString();
                    string name = row["Name"].ToString();
                    string cardCat = row["Category"].ToString();
                    decimal minPrice = Convert.ToDecimal(row["MinPrice"]);
                    int totalStock = Convert.ToInt32(row["TotalStock"]);
                    string imagePath = row["ImagePath"] == DBNull.Value ? "" : row["ImagePath"].ToString();

                    flpSalesProducts.Controls.Add(
                        CreateSalesProductCard(productId, code, cardCode, name, cardCat, minPrice, totalStock, imagePath)
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load sản phẩm bán hàng: " + ex.Message);
            }
        }

        private Panel CreateSalesProductCard(
    int productId, string code, string cardCode, string name, string category,
    decimal minPrice, int totalStock, string imagePath)
        {
            Panel card = new Panel();
            card.Width = 180;
            card.Height = 270;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Margin = new Padding(10);
            card.BackColor = Color.White;
            card.Tag = productId;

            PictureBox pic = new PictureBox();
            pic.Width = 160;
            pic.Height = 140;
            pic.Location = new Point(10, 10);
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            pic.BackColor = Color.Gainsboro;

            string fullPath = Path.Combine(Application.StartupPath, imagePath ?? "");
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(fullPath))
            {
                pic.Image = Image.FromFile(fullPath);
            }

            Label lblName = new Label();
            lblName.Text = name;
            lblName.Location = new Point(10, 160);
            lblName.Size = new Size(160, 35);
            lblName.Font = new Font("Be Vietnam Pro", 10, FontStyle.Bold);

            Label lblCode = new Label();
            lblCode.Text = string.IsNullOrWhiteSpace(cardCode) ? category : cardCode;
            lblCode.Location = new Point(10, 195);
            lblCode.Size = new Size(160, 20);
            lblCode.ForeColor = Color.DimGray;

            Label lblPrice = new Label();
            lblPrice.Text = "Từ: " + minPrice.ToString("N0") + " đ";
            lblPrice.Location = new Point(10, 218);
            lblPrice.Size = new Size(160, 20);
            lblPrice.ForeColor = Color.Red;
            lblPrice.Font = new Font("Be Vietnam Pro", 9, FontStyle.Bold);

            Label lblStock = new Label();
            lblStock.Text = "Tồn: " + totalStock + " (tất cả loại)";
            lblStock.Location = new Point(10, 240);
            lblStock.Size = new Size(160, 20);

            card.Controls.Add(pic);
            card.Controls.Add(lblName);
            card.Controls.Add(lblCode);
            card.Controls.Add(lblPrice);
            card.Controls.Add(lblStock);

            card.Click += SalesProductCard_Click;
            pic.Click += SalesProductCard_Click;
            lblName.Click += SalesProductCard_Click;
            lblCode.Click += SalesProductCard_Click;
            lblPrice.Click += SalesProductCard_Click;
            lblStock.Click += SalesProductCard_Click;

            return card;
        }

        private void SalesProductCard_Click(object sender, EventArgs e)
        {
            Control c = sender as Control;
            Panel card = c is Panel ? (Panel)c : c.Parent as Panel;
            if (card == null) return;

            int productId = (int)card.Tag;
            ShowVariantPicker(productId);
        }

        private void ShowVariantPicker(int productId)
        {
            try
            {
                using var conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string sql = @"
            SELECT v.VariantId, v.Rarity, v.SellPrice, v.CostPrice, v.Stock,
                   p.Name, p.Code, p.CardCode
            FROM ProductVariants v
            INNER JOIN Products p ON p.ProductId = v.ProductId
            WHERE v.ProductId = @productId AND v.Stock > 0
            ORDER BY v.SellPrice";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@productId", productId);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Sản phẩm này đã hết tất cả các loại!");
                    return;
                }

                if (dt.Rows.Count == 1)
                {
                    var r = dt.Rows[0];
                    AddToCart(new CartItem
                    {
                        ProductId = productId,
                        VariantId = Convert.ToInt32(r["VariantId"]),
                        Rarity = r["Rarity"].ToString(),
                        Name = r["Name"].ToString(),
                        Code = r["Code"].ToString(),
                        CardCode = r["CardCode"].ToString(),
                        SellPrice = Convert.ToDecimal(r["SellPrice"]),
                        CostPrice = Convert.ToDecimal(r["CostPrice"]),
                        Stock = Convert.ToInt32(r["Stock"]),
                        Quantity = 1
                    }, 1);
                    return;
                }

                using Form popup = new Form();
                popup.Text = "Chọn độ hiếm";
                popup.Size = new Size(360, 80 + dt.Rows.Count * 46);
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.MaximizeBox = false;

                int y = 12;
                foreach (DataRow row in dt.Rows)
                {
                    int variantId = Convert.ToInt32(row["VariantId"]);
                    string rarity = row["Rarity"].ToString();
                    decimal sellPrice = Convert.ToDecimal(row["SellPrice"]);
                    decimal costPrice = Convert.ToDecimal(row["CostPrice"]);
                    int stock = Convert.ToInt32(row["Stock"]);
                    string name = row["Name"].ToString();
                    string code = row["Code"].ToString();
                    string cardCode = row["CardCode"].ToString();

                    Button btn = new Button();
                    btn.Text = $"{rarity}  —  {sellPrice:N0} đ  (Tồn: {stock})";
                    btn.Location = new Point(12, y);
                    btn.Size = new Size(320, 38);
                    btn.Font = new Font("Be Vietnam Pro", 10);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = Color.FromArgb(235, 242, 255);
                    btn.ForeColor = Color.RoyalBlue;
                    btn.FlatAppearance.BorderColor = Color.RoyalBlue;

                    btn.Click += (s, ev) =>
                    {
                        AddToCart(new CartItem
                        {
                            ProductId = productId,
                            VariantId = variantId,
                            Rarity = rarity,
                            Name = name,
                            Code = code,
                            CardCode = cardCode,
                            SellPrice = sellPrice,
                            CostPrice = costPrice,
                            Stock = stock,
                            Quantity = 1
                        }, 1);
                        popup.Close();
                    };

                    popup.Controls.Add(btn);
                    y += 46;
                }

                popup.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi chọn variant: " + ex.Message);
            }
        }

        private void AddToCart(CartItem product, int quantity)
        {
            if (quantity <= 0) return;

            var existing = cart.FirstOrDefault(x => x.VariantId == product.VariantId);

            if (existing != null)
            {
                if (existing.Quantity + quantity > existing.Stock)
                {
                    MessageBox.Show("Số lượng vượt quá tồn kho!");
                    return;
                }

                existing.Quantity += quantity;
            }
            else
            {
                if (quantity > product.Stock)
                {
                    MessageBox.Show("Số lượng vượt quá tồn kho!");
                    return;
                }

                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    VariantId = product.VariantId,
                    Rarity = product.Rarity,
                    Code = product.Code,
                    CardCode = product.CardCode,
                    Name = product.Name,
                    SellPrice = product.SellPrice,
                    CostPrice = product.CostPrice,
                    Quantity = quantity,
                    Stock = product.Stock
                });
            }

            BindCart();
        }

        private void BindCart()
        {
            dgvCart.DataSource = null;
            dgvCart.DataSource = cart.Select(x => new
            {
                x.ProductId,
                x.VariantId,
                x.Name,
                x.Rarity,
                x.SellPrice,
                x.Quantity,
                x.LineTotal
            }).ToList();

            if (dgvCart.Rows.Count > 0)
            {
                dgvCart.ClearSelection();
                dgvCart.Rows[0].Selected = true;
                selectedCartProductId = Convert.ToInt32(dgvCart.Rows[0].Cells["ProductId"].Value);
            }
            else
            {
                selectedCartProductId = null;
            }

            UpdateCartSummary();
        }

        private void txtSearchSalesProduct_TextChanged(object sender, EventArgs e)
        {
            ApplySalesFilter();
        }

        private void cbSalesCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySalesFilter();
        }

        private void btnRefreshSalesProducts_Click(object sender, EventArgs e)
        {
            isRefreshing = true;

            txtSearchSalesProduct.Text = "";
            cbSalesCategoryFilter.SelectedIndex = 0;
            cbRarityFilter.SelectedIndex = 0;
            cbAttributeFilter.SelectedIndex = 0;
            cbCardTypeFilter.SelectedIndex = 0;

            isRefreshing = false;

            ApplySalesFilter();
        }

        private void btnIncreaseQty_Click(object sender, EventArgs e)
        {
            var item = GetSelectedCartItem();

            if (item == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm trong giỏ!");
                return;
            }

            if (item.Quantity + 1 > item.Stock)
            {
                MessageBox.Show("Số lượng vượt quá tồn kho!");
                return;
            }

            item.Quantity++;
            BindCart();
        }

        private void btnDecreaseQty_Click(object sender, EventArgs e)
        {
            var item = GetSelectedCartItem();

            if (item == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm trong giỏ!");
                return;
            }

            item.Quantity--;

            if (item.Quantity <= 0)
            {
                cart.Remove(item);
            }

            BindCart();
        }

        private void btnRemoveCartItem_Click(object sender, EventArgs e)
        {
            var item = GetSelectedCartItem();

            if (item == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm trong giỏ!");
                return;
            }

            cart.Remove(item);
            BindCart();
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0) return;

            if (MessageBox.Show("Xóa toàn bộ giỏ hàng?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cart.Clear();
                selectedCartProductId = null;
                BindCart();
            }
        }

        private void ResetCustomerInfo()
        {
            selectedCustomerId = null;
            currentCustomerPoints = 0;
            currentCustomerName = "";
            currentCustomerPhone = "";

            lblCustomerName.Text = "Tên: Khách lẻ";
            lblCustomerPoints.Text = "Điểm hiện tại: 0";

            txtUsePoints.Text = "0";
            txtUsePoints.Enabled = false;

            UpdateCartSummary();
        }

        private void btnFindCustomer_Click(object sender, EventArgs e)
        {
            string phone = txtCustomerPhone.Text.Trim();

            if (phone == "")
            {
                ResetCustomerInfo();
                MessageBox.Show("Để trống số điện thoại sẽ tính là khách lẻ.");
                return;
            }

            try
            {
                string sql = $@"
            SELECT CustomerId, Name, Points
            FROM Customers
            WHERE Phone = N'{phone}'";

                DataTable dt = DbHelper.Query(sql);

                if (dt.Rows.Count > 0)
                {
                    selectedCustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
                    currentCustomerPoints = Convert.ToInt32(dt.Rows[0]["Points"]);
                    currentCustomerName = dt.Rows[0]["Name"].ToString();
                    currentCustomerPhone = phone;

                    lblCustomerName.Text = "Tên: " + dt.Rows[0]["Name"].ToString();
                    lblCustomerPoints.Text = "Điểm hiện tại: " + currentCustomerPoints;

                    txtUsePoints.Enabled = true;
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        "Không tìm thấy khách. Bạn có muốn mở danh sách khách để thêm/chọn khách không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        OpenCustomerSelection();
                    }
                    else
                    {
                        ResetCustomerInfo();
                    }
                }

                UpdateCartSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm khách: " + ex.Message);
            }
        }

        private void OpenCustomerSelection(string phone = "")
        {
            using (FormCustomers f = new FormCustomers())
            {
                f.IsSelectMode = true;
                f.ShowDialog();

                if (f.DialogResult == DialogResult.OK)
                {
                    selectedCustomerId = f.SelectedCustomerId;
                    currentCustomerPoints = f.SelectedCustomerPoints;
                    currentCustomerName = f.SelectedCustomerName;
                    currentCustomerPhone = f.SelectedCustomerPhone;

                    txtCustomerPhone.Text = f.SelectedCustomerPhone;
                    lblCustomerName.Text = "Tên: " + f.SelectedCustomerName;
                    lblCustomerPoints.Text = "Điểm hiện tại: " + currentCustomerPoints;

                    txtUsePoints.Enabled = true;
                    txtUsePoints.Text = "0";

                    UpdateCartSummary();
                }
            }
        }

        private void UpdateCartSummary()
        {
            decimal subTotal = cart.Sum(x => x.LineTotal);

            decimal manualDiscount = 0;
            decimal.TryParse(txtDiscount.Text.Trim(), out manualDiscount);

            if (manualDiscount < 0) manualDiscount = 0;
            if (manualDiscount > subTotal) manualDiscount = subTotal;

            decimal afterManualDiscount = subTotal - manualDiscount;

            int usedPoints = 0;
            int.TryParse(txtUsePoints.Text.Trim(), out usedPoints);

            if (selectedCustomerId == null)
            {
                usedPoints = 0;
                if (txtUsePoints.Text != "0")
                    txtUsePoints.Text = "0";
            }

            if (usedPoints < 0) usedPoints = 0;
            if (usedPoints > currentCustomerPoints) usedPoints = currentCustomerPoints;
            if (usedPoints > afterManualDiscount) usedPoints = (int)afterManualDiscount;

            if (txtUsePoints.Text != usedPoints.ToString())
                txtUsePoints.Text = usedPoints.ToString();

            decimal pointDiscount = usedPoints; 
            decimal finalTotal = afterManualDiscount - pointDiscount;

            if (finalTotal < 0) finalTotal = 0;

            int earnedPoints = 0;
            if (selectedCustomerId != null && finalTotal >= 100000)
            {
                earnedPoints = (int)(finalTotal / 1000);
            }

            lblSubTotalValue.Text = subTotal.ToString("N0") + " đ";
            lblFinalTotalValue.Text = finalTotal.ToString("N0") + " đ";
            lblEarnedPointsValue.Text = earnedPoints.ToString();
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            UpdateCartSummary();
        }

        private void dgvCart_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int productId = Convert.ToInt32(dgvCart.Rows[e.RowIndex].Cells["ProductId"].Value);
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item == null) return;

            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập số lượng mới:",
                "Sửa số lượng",
                item.Quantity.ToString());

            if (int.TryParse(input, out int newQty))
            {
                if (newQty <= 0)
                {
                    cart.Remove(item);
                }
                else if (newQty > item.Stock)
                {
                    MessageBox.Show("Số lượng vượt quá tồn kho!");
                    return;
                }
                else
                {
                    item.Quantity = newQty;
                }

                BindCart();
            }
        }

        private CartItem GetSelectedCartItem()
        {
            if (selectedCartProductId == null)
                return null;

            return cart.FirstOrDefault(x => x.ProductId == selectedCartProductId.Value);
        }

        private void lblDiscountText_Click(object sender, EventArgs e)
        {

        }

        private void txtUsePoints_TextChanged(object sender, EventArgs e)
        {
            UpdateCartSummary();
        }

        private void cbRarityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySalesFilter();
        }

        private void cbCardTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySalesFilter();
        }

        private void cbAttributeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySalesFilter();
        }

        private void ApplySalesFilter()
        {
            if (isRefreshing) return;

            string keyword = txtSearchSalesProduct.Text.Trim();
            string category = cbSalesCategoryFilter.SelectedIndex <= 0 ? "Tất cả" : cbSalesCategoryFilter.Text;
            string rarity = cbRarityFilter.SelectedIndex <= 0 ? "Tất cả" : cbRarityFilter.Text;
            string attribute = cbAttributeFilter.SelectedIndex <= 0 ? "Tất cả" : cbAttributeFilter.Text;
            string cardType = cbCardTypeFilter.SelectedIndex <= 0 ? "Tất cả" : cbCardTypeFilter.Text;

            LoadSalesProducts(keyword, category, rarity, attribute, cardType);
        }

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            ShowBarcodeScanPopup();
        }

        private void ShowBarcodeScanPopup()
        {
            Form popup = new Form();
            popup.Text = "Quét barcode";
            popup.Size = new Size(400, 220);
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.FormBorderStyle = FormBorderStyle.FixedDialog;
            popup.MaximizeBox = false;
            popup.MinimizeBox = false;

            Label lblIcon = new Label();
            lblIcon.Text = "📷";
            lblIcon.Font = new Font("Be Vietnam Pro", 28);
            lblIcon.Location = new Point(160, 15);
            lblIcon.Size = new Size(60, 50);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            Label lblStatus = new Label();
            lblStatus.Text = "Đang chờ quét mã...";
            lblStatus.Font = new Font("Be Vietnam Pro", 11);
            lblStatus.ForeColor = Color.RoyalBlue;
            lblStatus.Location = new Point(20, 75);
            lblStatus.Size = new Size(350, 25);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            TextBox txtScan = new TextBox();
            txtScan.Location = new Point(-500, -500);
            txtScan.Size = new Size(1, 1);

            Label lblResult = new Label();
            lblResult.Font = new Font("Be Vietnam Pro", 10);
            lblResult.Location = new Point(20, 105);
            lblResult.Size = new Size(350, 45);
            lblResult.TextAlign = ContentAlignment.MiddleCenter;

            Button btnClose = new Button();
            btnClose.Text = "Đóng";
            btnClose.Location = new Point(140, 155);
            btnClose.Size = new Size(100, 35);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += (s, ev) => popup.Close();

            txtScan.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode != Keys.Enter) return;

                string barcode = txtScan.Text.Trim();
                txtScan.Clear();
                if (string.IsNullOrEmpty(barcode)) return;

                try
                {
                    using var conn = new SqlConnection(DbConfig.ConnectionString);
                    conn.Open();

                    string sql = @"
                SELECT DISTINCT p.ProductId, p.Name
                FROM Products p
                INNER JOIN ProductVariants v ON p.ProductId = v.ProductId
                WHERE p.IsActive = 1 AND v.Stock > 0
                AND (p.CardCode = @barcode OR p.Code = @barcode)";

                    using var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@barcode", barcode);

                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        lblIcon.Text = "❌";
                        lblStatus.Text = $"Không tìm thấy: {barcode}";
                        lblStatus.ForeColor = Color.Red;
                        lblResult.Text = "Sản phẩm không có trong hệ thống";
                        lblResult.ForeColor = Color.Red;

                        var timers = new System.Windows.Forms.Timer();
                        timers.Interval = 2000;
                        timers.Tick += (ts, te) =>
                        {
                            timers.Stop();
                            lblIcon.Text = "📷";
                            lblStatus.Text = "Đang chờ quét mã...";
                            lblStatus.ForeColor = Color.RoyalBlue;
                            lblResult.Text = "";
                            txtScan.Focus();
                        };
                        timers.Start();
                        return;
                    }

                    int productId = Convert.ToInt32(dt.Rows[0]["ProductId"]);
                    string productName = dt.Rows[0]["Name"].ToString();

                    lblIcon.Text = "✅";
                    lblStatus.Text = $"Tìm thấy: {productName}";
                    lblStatus.ForeColor = Color.Green;
                    lblResult.Text = "Đã thêm vào giỏ!";
                    lblResult.ForeColor = Color.SeaGreen;

                    ShowVariantPicker(productId);

                    var timer = new System.Windows.Forms.Timer();
                    timer.Interval = 1500;
                    timer.Tick += (ts, te) =>
                    {
                        timer.Stop();
                        lblIcon.Text = "📷";
                        lblStatus.Text = "Đang chờ quét mã...";
                        lblStatus.ForeColor = Color.RoyalBlue;
                        lblResult.Text = "";
                        txtScan.Focus(); 
                    };
                    timer.Start();
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Lỗi: " + ex.Message;
                    lblStatus.ForeColor = Color.Red;
                }

                ev.Handled = true;
                ev.SuppressKeyPress = true;
            };

            popup.Controls.Add(lblIcon);
            popup.Controls.Add(lblStatus);
            popup.Controls.Add(lblResult);
            popup.Controls.Add(txtScan);
            popup.Controls.Add(btnClose);

            popup.Shown += (s, ev) => txtScan.Focus();
            popup.ShowDialog(this);
        }

        private async Task<bool> ShowQRPaymentPopupAsync(decimal amount)
        {
            int orderCode = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            string description = "YGO " + orderCode.ToString();

            PayOSClient payOS = new PayOSClient("53afb4a2-7ebf-45f3-b0ec-c83bb456ba18", "11ddd648-e0e7-42bc-9dc1-740a9b8257c3", "44d4ae3970a346056f6564fb63b076d2bf806438c21c9052986d7848a854d1e3");

            try
            {
                CreatePaymentLinkRequest paymentData = new CreatePaymentLinkRequest
                {
                    OrderCode = orderCode,
                    Amount = (int)amount,
                    Description = description,
                    CancelUrl = "http://localhost/cancel", 
                    ReturnUrl = "http://localhost/success"
                };

                var createPayment = await payOS.PaymentRequests.CreateAsync(paymentData);

                Form popup = new Form();
                popup.Text = "Quét mã QR để thanh toán";
                popup.Size = new Size(350, 450);
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.FormBorderStyle = FormBorderStyle.Sizable;
                popup.BackColor = Color.White;
                popup.MaximizeBox = false;
                popup.MinimizeBox = false;

                PictureBox picQR = new PictureBox();
                picQR.Size = new Size(250, 250);
                picQR.Location = new Point(40, 20);
                picQR.SizeMode = PictureBoxSizeMode.Zoom;

                string qrData = createPayment.QrCode;
                picQR.LoadAsync($"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={Uri.EscapeDataString(qrData)}");

                Label lblAmount = new Label();
                lblAmount.Text = $"Số tiền: {amount:N0} đ";
                lblAmount.Font = new Font("Be Vietnam Pro", 14, FontStyle.Bold);
                lblAmount.ForeColor = Color.RoyalBlue;
                lblAmount.TextAlign = ContentAlignment.MiddleCenter;
                lblAmount.Location = new Point(10, 280);
                lblAmount.Size = new Size(310, 30);

                Label lblStatus = new Label();
                lblStatus.Text = "Đang chờ khách quét mã...";
                lblStatus.Font = new Font("Be Vietnam Pro", 10, FontStyle.Italic);
                lblStatus.ForeColor = Color.DimGray;
                lblStatus.TextAlign = ContentAlignment.MiddleCenter;
                lblStatus.Location = new Point(10, 320);
                lblStatus.Size = new Size(310, 30);

                Button btnCancel = new Button();
                btnCancel.Text = "Hủy giao dịch QR";
                btnCancel.Location = new Point(100, 360);
                btnCancel.Size = new Size(130, 35);
                btnCancel.Click += (s, ev) => popup.Close();

                popup.Controls.Add(picQR);
                popup.Controls.Add(lblAmount);
                popup.Controls.Add(lblStatus);
                popup.Controls.Add(btnCancel);

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 3000;
                bool isPaid = false;

                timer.Tick += async (s, e) =>
                {
                    try
                    {
                        var info = await payOS.PaymentRequests.GetAsync(orderCode);

                        lblStatus.Text = $"Trạng thái: {info.Status}...";

                        string currentStatus = info.Status.ToString().ToUpper();

                        if (currentStatus == "PAID" || currentStatus == "SUCCESS")
                        {
                            timer.Stop();
                            isPaid = true;
                            lblStatus.Text = "✅ ĐÃ NHẬN TIỀN THÀNH CÔNG!";
                            lblStatus.ForeColor = Color.Green;

                            await Task.Delay(1000);
                            popup.DialogResult = DialogResult.OK;
                            popup.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = "Lỗi check: " + ex.Message;
                        lblStatus.ForeColor = Color.Red;
                    }
                };

                timer.Start();
                popup.FormClosed += (s, e) => timer.Stop();

                popup.ShowDialog(this);
                return isPaid;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo mã QR PayOS: " + ex.Message);
                return false;
            }
        }

        private async void btnQR_Click(object sender, EventArgs e)
        {
            await ProcessOrder(true);
        }

        private async void btnCash_Click(object sender, EventArgs e)
        {
            await ProcessOrder(false);
        }

        private async Task ProcessOrder(bool isQR)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string paymentMethod = isQR ? "Quét mã QR" : "Tiền mặt";
            DialogResult confirm = MessageBox.Show(
                $"Xác nhận thanh toán đơn hàng này bằng hình thức {paymentMethod}?",
                "Xác nhận thanh toán",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            decimal subTotal = cart.Sum(x => x.LineTotal);

            decimal manualDiscount = 0;
            decimal.TryParse(txtDiscount.Text.Trim(), out manualDiscount);

            if (manualDiscount < 0) manualDiscount = 0;
            if (manualDiscount > subTotal) manualDiscount = subTotal;

            decimal afterManualDiscount = subTotal - manualDiscount;

            int usedPoints = 0;
            int.TryParse(txtUsePoints.Text.Trim(), out usedPoints);

            if (selectedCustomerId == null)
            {
                usedPoints = 0;
            }

            if (usedPoints < 0) usedPoints = 0;
            if (usedPoints > currentCustomerPoints) usedPoints = currentCustomerPoints;
            if (usedPoints > afterManualDiscount) usedPoints = (int)afterManualDiscount;

            decimal pointDiscount = usedPoints;
            decimal finalTotal = afterManualDiscount - pointDiscount;

            if (finalTotal < 0) finalTotal = 0;

            int earnedPoints = 0;
            if (selectedCustomerId != null && finalTotal >= 100000)
            {
                earnedPoints = (int)(finalTotal / 1000);
            }

            if (isQR && finalTotal > 0)
            {
                bool isPaid = await ShowQRPaymentPopupAsync(finalTotal);

                if (!isPaid)
                {
                    MessageBox.Show("Giao dịch QR đã bị hủy. Đơn hàng chưa được lưu.", "Thông báo");
                    return;
                }
            }

            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();

            try
            {
                string sqlInvoice = @"
                                    INSERT INTO SalesInvoices
                                    (SaleDate, CustomerId, CustomerName, CustomerPhone, UserId, SubTotal, Discount, Total, PointsEarned, Note)
                                    VALUES
                                    (GETDATE(), @CustomerId, @CustomerName, @CustomerPhone, @UserId, @SubTotal, @Discount, @Total, @PointsEarned, @Note);
                                    SELECT SCOPE_IDENTITY();";

                using SqlCommand cmdInvoice = new SqlCommand(sqlInvoice, conn, tran);
                cmdInvoice.Parameters.AddWithValue("@CustomerId", (object?)selectedCustomerId ?? DBNull.Value);
                cmdInvoice.Parameters.AddWithValue("@UserId", CurrentUser.UserId);
                cmdInvoice.Parameters.AddWithValue("@CustomerName", (object?)currentCustomerName ?? DBNull.Value);
                cmdInvoice.Parameters.AddWithValue("@CustomerPhone", (object?)currentCustomerPhone ?? DBNull.Value);
                cmdInvoice.Parameters.AddWithValue("@SubTotal", subTotal);
                cmdInvoice.Parameters.AddWithValue("@Discount", manualDiscount + pointDiscount);
                cmdInvoice.Parameters.AddWithValue("@Total", finalTotal);
                cmdInvoice.Parameters.AddWithValue("@PointsEarned", earnedPoints);
                cmdInvoice.Parameters.AddWithValue("@Note", isQR ? "Thanh toán QR PayOS" : "Thanh toán Tiền mặt");

                int saleId = Convert.ToInt32(cmdInvoice.ExecuteScalar());

                foreach (var item in cart)
                {
                    string sqlDetail = @"
                                    INSERT INTO SalesDetails
                                    (SaleId, ProductId, Qty, UnitSellPrice, UnitCostPrice, LineTotal)
                                    VALUES
                                    (@SaleId, @ProductId, @Qty, @UnitSellPrice, @UnitCostPrice, @LineTotal)";

                    using SqlCommand cmdDetail = new SqlCommand(sqlDetail, conn, tran);
                    cmdDetail.Parameters.AddWithValue("@SaleId", saleId);
                    cmdDetail.Parameters.AddWithValue("@ProductId", item.ProductId);
                    cmdDetail.Parameters.AddWithValue("@Qty", item.Quantity);
                    cmdDetail.Parameters.AddWithValue("@UnitSellPrice", item.SellPrice);
                    cmdDetail.Parameters.AddWithValue("@UnitCostPrice", item.CostPrice);
                    cmdDetail.Parameters.AddWithValue("@LineTotal", item.LineTotal);
                    cmdDetail.ExecuteNonQuery();

                    string sqlStock = @"
                                    UPDATE ProductVariants
                                    SET Stock = Stock - @Qty
                                    WHERE VariantId = @VariantId AND Stock >= @Qty";

                    using SqlCommand cmdStock = new SqlCommand(sqlStock, conn, tran);
                    cmdStock.Parameters.AddWithValue("@Qty", item.Quantity);
                    cmdStock.Parameters.AddWithValue("@VariantId", item.VariantId);

                    int stockUpdated = cmdStock.ExecuteNonQuery();
                    if (stockUpdated == 0)
                    {
                        throw new Exception($"'{item.Name} - {item.Rarity}' không đủ tồn kho!");
                    }
                }

                if (selectedCustomerId != null)
                {
                    int newBalance = currentCustomerPoints - usedPoints + earnedPoints;

                    string sqlUpdateCustomer = @"
                                                UPDATE Customers
                                                SET Points = @NewPoints
                                                WHERE CustomerId = @CustomerId";

                    using SqlCommand cmdUpdateCustomer = new SqlCommand(sqlUpdateCustomer, conn, tran);
                    cmdUpdateCustomer.Parameters.AddWithValue("@NewPoints", newBalance);
                    cmdUpdateCustomer.Parameters.AddWithValue("@CustomerId", selectedCustomerId.Value);
                    cmdUpdateCustomer.ExecuteNonQuery();

                    if (usedPoints > 0)
                    {
                        string sqlRedeem = @"
                                            INSERT INTO PointTransactions
                                            (CustomerId, SaleId, TxnDate, Type, PointsChange, BalanceAfter, Note)
                                            VALUES
                                            (@CustomerId, @SaleId, GETDATE(), @Type, @PointsChange, @BalanceAfter, @Note)";

                        using SqlCommand cmdRedeem = new SqlCommand(sqlRedeem, conn, tran);
                        cmdRedeem.Parameters.AddWithValue("@CustomerId", selectedCustomerId.Value);
                        cmdRedeem.Parameters.AddWithValue("@SaleId", saleId);
                        cmdRedeem.Parameters.AddWithValue("@Type", "Redeem");
                        cmdRedeem.Parameters.AddWithValue("@PointsChange", -usedPoints);
                        cmdRedeem.Parameters.AddWithValue("@BalanceAfter", currentCustomerPoints - usedPoints);
                        cmdRedeem.Parameters.AddWithValue("@Note", "Sử dụng điểm để giảm giá hóa đơn");
                        cmdRedeem.ExecuteNonQuery();
                    }

                    if (earnedPoints > 0)
                    {
                        string sqlEarn = @"
                                        INSERT INTO PointTransactions
                                        (CustomerId, SaleId, TxnDate, Type, PointsChange, BalanceAfter, Note)
                                        VALUES
                                        (@CustomerId, @SaleId, GETDATE(), @Type, @PointsChange, @BalanceAfter, @Note)";

                        using SqlCommand cmdEarn = new SqlCommand(sqlEarn, conn, tran);
                        cmdEarn.Parameters.AddWithValue("@CustomerId", selectedCustomerId.Value);
                        cmdEarn.Parameters.AddWithValue("@SaleId", saleId);
                        cmdEarn.Parameters.AddWithValue("@Type", "Earn");
                        cmdEarn.Parameters.AddWithValue("@PointsChange", earnedPoints);
                        cmdEarn.Parameters.AddWithValue("@BalanceAfter", newBalance);
                        cmdEarn.Parameters.AddWithValue("@Note", "Tích điểm từ hóa đơn bán hàng");
                        cmdEarn.ExecuteNonQuery();
                    }
                }

                tran.Commit();

                DialogResult printRes = MessageBox.Show("Thanh toán thành công! Bạn có muốn in hóa đơn không?",
                                                        "In Hóa Đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (printRes == DialogResult.Yes)
                {
                    PrintReceipt(saleId, finalTotal, manualDiscount + pointDiscount, earnedPoints);
                }

                cart.Clear();

                MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                cart.Clear();
                selectedCartProductId = null;
                txtDiscount.Text = "0";
                txtUsePoints.Text = "0";
                txtCustomerPhone.Text = "";
                ResetCustomerInfo();
                BindCart();
                LoadSalesProducts(txtSearchSalesProduct.Text.Trim(), cbSalesCategoryFilter.Text);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                MessageBox.Show("Thanh toán thất bại: " + ex.Message, "Lỗi");
            }
        }

        private void PrintReceipt(int saleId, decimal finalTotal, decimal totalDiscount, int earnedPoints)
        {
            PrintDocument pd = new PrintDocument();

            pd.DefaultPageSettings.PaperSize = new PaperSize("POS Receipt", 315, 600);

            var printItems = cart.ToList();
            string custName = currentCustomerName == "" ? "Khách lẻ" : currentCustomerName;
            DateTime printDate = DateTime.Now;

            pd.PrintPage += (sender, e) =>
            {
                Graphics g = e.Graphics;
                Font fontTitle = new Font("Courier New", 14, FontStyle.Bold);
                Font fontRegular = new Font("Courier New", 9, FontStyle.Regular);
                Font fontBold = new Font("Courier New", 9, FontStyle.Bold);
                Brush brush = Brushes.Black;

                int y = 10;
                int left = 10;
                int center = pd.DefaultPageSettings.PaperSize.Width / 2;

                StringFormat formatCenter = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString("VUATROICHO - YUGIOH SHOP", fontTitle, brush, center, y, formatCenter);
                y += 25;
                g.DrawString("ĐC: TP.Hồ Chí Minh", fontRegular, brush, center, y, formatCenter);
                y += 20;
                g.DrawString("HÓA ĐƠN BÁN HÀNG", fontBold, brush, center, y, formatCenter);
                y += 25;

                g.DrawString($"Mã HĐ: {saleId}", fontRegular, brush, left, y);
                y += 15;
                g.DrawString($"Ngày : {printDate:dd/MM/yyyy HH:mm}", fontRegular, brush, left, y);
                y += 15;
                g.DrawString($"Khách: {custName}", fontRegular, brush, left, y);
                y += 20;

                g.DrawString("---------------------------------", fontRegular, brush, left, y);
                y += 15;

                foreach (var item in printItems)
                {
                    string name = item.Name.Length > 20 ? item.Name.Substring(0, 20) + "..." : item.Name;
                    g.DrawString(name, fontBold, brush, left, y);
                    y += 15;

                    string qtyPrice = $"{item.Quantity} x {item.SellPrice:N0}";
                    g.DrawString(qtyPrice, fontRegular, brush, left, y);

                    string lineTotal = item.LineTotal.ToString("N0");
                    g.DrawString(lineTotal, fontRegular, brush, pd.DefaultPageSettings.PaperSize.Width - 15, y, new StringFormat { Alignment = StringAlignment.Far });
                    y += 20;
                }

                g.DrawString("---------------------------------", fontRegular, brush, left, y);
                y += 20;

                g.DrawString("Giảm giá:", fontRegular, brush, left, y);
                g.DrawString("-" + totalDiscount.ToString("N0"), fontRegular, brush, pd.DefaultPageSettings.PaperSize.Width - 15, y, new StringFormat { Alignment = StringAlignment.Far });
                y += 20;

                g.DrawString("TỔNG CỘNG:", fontTitle, brush, left, y);
                g.DrawString(finalTotal.ToString("N0"), fontTitle, brush, pd.DefaultPageSettings.PaperSize.Width - 15, y, new StringFormat { Alignment = StringAlignment.Far });
                y += 30;

                if (earnedPoints > 0)
                {
                    g.DrawString($"Điểm tích lũy được: {earnedPoints}", fontRegular, brush, center, y, formatCenter);
                    y += 20;
                }

                g.DrawString("Cảm ơn quý khách và hẹn gặp lại!", fontRegular, brush, center, y, formatCenter);
            };

            try
            {
                PrintPreviewDialog ppd = new PrintPreviewDialog();
                ppd.Document = pd;
                ppd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi in hóa đơn: " + ex.Message);
            }
        }

        private void StyleCartButtons()
        {
            Guna.UI2.WinForms.Guna2Button[] btns = { btnIncreaseQty, btnDecreaseQty, btnRemoveCartItem, btnClearCart };

            int spacing = 10; 
            int totalSpacing = spacing * (btns.Length - 1);
            int btnWidth = (dgvCart.Width - totalSpacing) / btns.Length;

            int startX = dgvCart.Location.X;

            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].Width = btnWidth;
                btns[i].Height = 40; 
                btns[i].BorderRadius = 6; 
                btns[i].Font = new Font("Be Vietnam Pro", 9.5F, FontStyle.Bold);
                btns[i].ForeColor = Color.White;
                btns[i].Cursor = Cursors.Hand;

                btns[i].Left = startX + (btnWidth + spacing) * i;
            }

            btnIncreaseQty.FillColor = Color.FromArgb(16, 185, 129);    
            btnDecreaseQty.FillColor = Color.FromArgb(245, 158, 11);    
            btnRemoveCartItem.FillColor = Color.FromArgb(239, 68, 68);  
            btnClearCart.FillColor = Color.FromArgb(185, 28, 28);        
        }

        private void BlockInvalidInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }
    }
}
