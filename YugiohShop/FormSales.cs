using Microsoft.Data.SqlClient;
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
using Microsoft.VisualBasic;

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
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvCart.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvCart.DefaultCellStyle.Font = new Font("Segoe UI", 10);
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
            // Category
            cbSalesCategoryFilter.Items.Clear();
            cbSalesCategoryFilter.Items.Add("Tất cả");
            cbSalesCategoryFilter.Items.Add("SingleCard");
            cbSalesCategoryFilter.Items.Add("Pack");
            cbSalesCategoryFilter.Items.Add("FullBox");
            cbSalesCategoryFilter.SelectedIndex = 0;

            // Rarity
            cbRarityFilter.Items.Clear();
            cbRarityFilter.Items.Add("Tất cả");
            cbRarityFilter.Items.Add("Common");
            cbRarityFilter.Items.Add("Rare");
            cbRarityFilter.Items.Add("Super Rare");
            cbRarityFilter.Items.Add("Ultra Rare");
            cbRarityFilter.Items.Add("Secret Rare");
            cbRarityFilter.Items.Add("Prismatic Secret Rare");
            cbRarityFilter.SelectedIndex = 0;

            // Attribute
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

            // CardType
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
            lblName.Font = new Font("Segoe UI", 10, FontStyle.Bold);

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
            lblPrice.Font = new Font("Segoe UI", 9, FontStyle.Bold);

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

                // Nếu chỉ có 1 variant → add thẳng không cần popup
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

                // Nhiều variant → hiện popup chọn
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
                    btn.Font = new Font("Segoe UI", 10);
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
            txtSearchSalesProduct.Text = "";
            cbSalesCategoryFilter.SelectedIndex = 0;
            cbRarityFilter.SelectedIndex = 0;
            cbAttributeFilter.SelectedIndex = 0;
            cbCardTypeFilter.SelectedIndex = 0;
            LoadSalesProducts(); // reset về mặc định
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

            decimal pointDiscount = usedPoints; // 1 điểm = 1đ
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

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống!");
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
                cmdInvoice.Parameters.AddWithValue("@Note", "");

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

                MessageBox.Show("Thanh toán thành công!");

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
                MessageBox.Show("Thanh toán thất bại: " + ex.Message);
            }
        }
        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0) return;

            if (MessageBox.Show("Hủy đơn hiện tại?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cart.Clear();
                selectedCartProductId = null;
                txtDiscount.Text = "0";
                txtCustomerPhone.Text = "";
                ResetCustomerInfo();
                BindCart();
            }
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

            // Icon trạng thái
            Label lblIcon = new Label();
            lblIcon.Text = "📷";
            lblIcon.Font = new Font("Segoe UI", 28);
            lblIcon.Location = new Point(160, 15);
            lblIcon.Size = new Size(60, 50);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            // Label trạng thái
            Label lblStatus = new Label();
            lblStatus.Text = "Đang chờ quét mã...";
            lblStatus.Font = new Font("Segoe UI", 11);
            lblStatus.ForeColor = Color.RoyalBlue;
            lblStatus.Location = new Point(20, 75);
            lblStatus.Size = new Size(350, 25);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            // Ô ẩn nhận barcode
            TextBox txtScan = new TextBox();
            txtScan.Location = new Point(-500, -500); // ẩn ra ngoài màn hình
            txtScan.Size = new Size(1, 1);

            // Label kết quả
            Label lblResult = new Label();
            lblResult.Font = new Font("Segoe UI", 10);
            lblResult.Location = new Point(20, 105);
            lblResult.Size = new Size(350, 45);
            lblResult.TextAlign = ContentAlignment.MiddleCenter;

            // Nút đóng
            Button btnClose = new Button();
            btnClose.Text = "Đóng";
            btnClose.Location = new Point(140, 155);
            btnClose.Size = new Size(100, 35);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += (s, ev) => popup.Close();

            // Xử lý khi nhận barcode
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
                        // Không tìm thấy
                        lblIcon.Text = "❌";
                        lblStatus.Text = $"Không tìm thấy: {barcode}";
                        lblStatus.ForeColor = Color.Red;
                        lblResult.Text = "Sản phẩm không có trong hệ thống";
                        lblResult.ForeColor = Color.Red;

                        // Reset lại sau 2 giây
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

                    // Tìm thấy
                    int productId = Convert.ToInt32(dt.Rows[0]["ProductId"]);
                    string productName = dt.Rows[0]["Name"].ToString();

                    lblIcon.Text = "✅";
                    lblStatus.Text = $"Tìm thấy: {productName}";
                    lblStatus.ForeColor = Color.Green;
                    lblResult.Text = "Đã thêm vào giỏ!";
                    lblResult.ForeColor = Color.SeaGreen;

                    // Thêm vào giỏ ngay, không đóng popup
                    ShowVariantPicker(productId);

                    // Reset lại sau 1.5 giây để quét tiếp
                    var timer = new System.Windows.Forms.Timer();
                    timer.Interval = 1500;
                    timer.Tick += (ts, te) =>
                    {
                        timer.Stop();
                        lblIcon.Text = "📷";
                        lblStatus.Text = "Đang chờ quét mã...";
                        lblStatus.ForeColor = Color.RoyalBlue;
                        lblResult.Text = "";
                        txtScan.Focus(); // sẵn sàng quét tiếp
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

            popup.Shown += (s, ev) => txtScan.Focus(); // focus ngay khi popup mở
            popup.ShowDialog(this);
        }

        private void lblCustomerTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
