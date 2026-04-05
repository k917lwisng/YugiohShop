using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using ZXing;
using ZXing.Common;
using System.Drawing.Imaging;

namespace YugiohShop
{
    public partial class FormProductEditor : Form
    {
        private int? editingProductId = null;

        public FormProductEditor()
        {
            InitializeComponent();
        }

        public FormProductEditor(int productId)
        {
            InitializeComponent();
            editingProductId = productId;
        }

        private void FormProductEditor_Load(object sender, EventArgs e)
        {
            cbCategory.Items.Clear();
            cbCategory.Items.Add("SingleCard");
            cbCategory.Items.Add("Pack");
            cbCategory.Items.Add("FullBox");
            cbCategory.SelectedIndex = 0;

            cbAttribute.Items.AddRange(new[] { "DARK", "LIGHT", "FIRE", "WATER", "EARTH", "WIND", "DIVINE" });
            cbAttribute.SelectedIndex = -1;

            cbCardType.Items.AddRange(new[] { "Monster", "Spell", "Trap" });
            cbCardType.SelectedIndex = -1;

            cbRarity.Items.AddRange(new[] {
                "Common", "Rare", "Super Rare", "Ultra Rare",
                "Secret Rare", "Prismatic Secret Rare"
            });
            cbRarity.SelectedIndex = -1;

            SetupVariantGrid();

            chkIsActive.Checked = true;

            dgvVariants.BringToFront();

            if (editingProductId != null)
            {
                this.Text = "Sửa sản phẩm";
                LoadProductForEdit();
            }
            else
            {
                this.Text = "Thêm sản phẩm";
            }
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtImagePath.Text = ofd.FileName;

                var oldImage = picProduct.Image;
                picProduct.Image = null;
                oldImage?.Dispose();

                var ms = new System.IO.MemoryStream(File.ReadAllBytes(ofd.FileName));
                picProduct.Image = Image.FromStream(ms);
                picProduct.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dgvVariants.EndEdit();

                string code = txtCode.Text.Trim().Replace("'", "''");
                string cardCode = txtCardCode.Text.Trim().Replace("'", "''");
                string name = txtName.Text.Trim().Replace("'", "''");
                string category = cbCategory.Text;
                string attribute = cbAttribute.SelectedIndex == -1 ? "" : cbAttribute.Text;
                string cardType = cbCardType.SelectedIndex == -1 ? "" : cbCardType.Text;
                string imagePath = txtImagePath.Text.Trim().Replace("'", "''");
                string note = txtNote.Text.Trim().Replace("'", "''");
                int isActive = chkIsActive.Checked ? 1 : 0;

                if (code == "" || name == "")
                {
                    MessageBox.Show("Vui lòng nhập mã và tên sản phẩm!");
                    return;
                }

                int variantCount = dgvVariants.Rows
                    .Cast<DataGridViewRow>()
                    .Count(r => !r.IsNewRow);

                if (variantCount == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất 1 variant!");
                    return;
                }

                foreach (DataGridViewRow row in dgvVariants.Rows)
                {
                    if (row.IsNewRow) continue;

                    decimal sellPrice = decimal.TryParse(row.Cells["SellPrice"].Value?.ToString(), out var sp) ? sp : 0;
                    decimal costPrice = decimal.TryParse(row.Cells["CostPrice"].Value?.ToString(), out var cp) ? cp : 0;
                    int stock = int.TryParse(row.Cells["Stock"].Value?.ToString(), out var st) ? st : 0;

                    if (sellPrice < 0 || costPrice < 0 || stock < 0)
                    {
                        MessageBox.Show("Giá bán, Giá vốn và Tồn kho KHÔNG ĐƯỢC LÀ SỐ ÂM!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; 
                    }
                }

                int productId;
                if (editingProductId == null)
                {
                    string sqlInsert = $@"
                INSERT INTO Products (Code, CardCode, Name, Category, Attribute, CardType, IsActive, CreatedAt, ImagePath, Note)
                OUTPUT INSERTED.ProductId
                VALUES (N'{code}', N'{cardCode}', N'{name}', N'{category}',
                        N'{attribute}', N'{cardType}', {isActive}, GETDATE(), N'{imagePath}', N'{note}')";

                    productId = Convert.ToInt32(DbHelper.Scalar(sqlInsert));
                }
                else
                {
                    productId = editingProductId.Value;
                    DbHelper.Execute($@"
                UPDATE Products SET
                    Code = N'{code}', CardCode = N'{cardCode}', Name = N'{name}',
                    Category = N'{category}', Attribute = N'{attribute}', CardType = N'{cardType}',
                    IsActive = {isActive}, ImagePath = N'{imagePath}', Note = N'{note}'
                WHERE ProductId = {productId}");

                    DbHelper.Execute($"DELETE FROM ProductVariants WHERE ProductId = {productId}");
                }

                foreach (DataGridViewRow row in dgvVariants.Rows)
                {
                    string rarity = row.Cells["Rarity"].Value?.ToString() ?? "";
                    decimal sellPrice = decimal.TryParse(row.Cells["SellPrice"].Value?.ToString(), out var sp) ? sp : 0;
                    decimal costPrice = decimal.TryParse(row.Cells["CostPrice"].Value?.ToString(), out var cp) ? cp : 0;
                    int stock = int.TryParse(row.Cells["Stock"].Value?.ToString(), out var st) ? st : 0;

                    DbHelper.Execute($@"
                INSERT INTO ProductVariants (ProductId, Rarity, SellPrice, CostPrice, Stock)
                VALUES ({productId}, N'{rarity}',
                        {sellPrice.ToString(CultureInfo.InvariantCulture)},
                        {costPrice.ToString(CultureInfo.InvariantCulture)},
                        {stock})");
                }

                MessageBox.Show(editingProductId == null ? "Thêm thành công!" : "Sửa thành công!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadProductForEdit()
        {
            try
            {
                string sql = $@"
            SELECT *
            FROM Products
            WHERE ProductId = {editingProductId}";

                DataTable dt = DbHelper.Query(sql);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm!");
                    this.Close();
                    return;
                }

                DataRow row = dt.Rows[0];

                txtCode.Text = row["Code"].ToString();
                txtCardCode.Text = row["CardCode"].ToString();
                txtName.Text = row["Name"].ToString();
                cbCategory.Text = row["Category"].ToString();
                txtNote.Text = row["Note"] == DBNull.Value ? "" : row["Note"].ToString();
                chkIsActive.Checked = Convert.ToBoolean(row["IsActive"]);

                cbAttribute.Text = row["Attribute"] == DBNull.Value ? "" : row["Attribute"].ToString();
                cbCardType.Text = row["CardType"] == DBNull.Value ? "" : row["CardType"].ToString();

                txtImagePath.Text = row["ImagePath"].ToString();

                string imagePath = row["ImagePath"].ToString();
                string fullPath = Path.Combine(Application.StartupPath, imagePath);

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(fullPath))
                {
                    using var ms = new System.IO.MemoryStream(File.ReadAllBytes(fullPath));
                    picProduct.Image = Image.FromStream(ms);
                    picProduct.SizeMode = PictureBoxSizeMode.Zoom;
                }

                DataTable dtV = DbHelper.Query($"SELECT * FROM ProductVariants WHERE ProductId = {editingProductId} ORDER BY VariantId");
                foreach (DataRow v in dtV.Rows)
                {
                    dgvVariants.Rows.Add(v["VariantId"], v["Rarity"], v["SellPrice"], v["CostPrice"], v["Stock"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu sản phẩm: " + ex.Message);
            }
        }

        private void btnRemoveImage_Click(object sender, EventArgs e)
        {
            txtImagePath.Text = "";
            picProduct.Image = null;
        }

        private void txtCardCode_TextChanged(object sender, EventArgs e)
        {
            string cardCode = txtCardCode.Text.Trim();

            if (string.IsNullOrEmpty(cardCode))
            {
                picBarcode.Image = null;
                return;
            }

            try
            {
                picBarcode.Image = GenerateBarcode(cardCode);
                picBarcode.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch { }
        }

        private Bitmap GenerateBarcode(string content)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 80,
                    Width = 300,
                    Margin = 4,
                    PureBarcode = false 
                }
            };

            var pixelData = writer.Write(content);

            var bitmap = new Bitmap(pixelData.Width, pixelData.Height,
                PixelFormat.Format32bppRgb);

            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                bitmap.PixelFormat);

            System.Runtime.InteropServices.Marshal.Copy(
                pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        private void btnSaveBarcode_Click(object sender, EventArgs e)
        {
            string cardCode = txtCardCode.Text.Trim();

            if (string.IsNullOrEmpty(cardCode) || picBarcode.Image == null)
            {
                MessageBox.Show("Vui lòng nhập CardCode trước!");
                return;
            }

            try
            {
                string projectDir = Path.GetFullPath(
                    Path.Combine(Application.StartupPath, "..", "..", ".."));

                string barcodeDir = Path.Combine(projectDir, "barcodes");
                Directory.CreateDirectory(barcodeDir);

                string fileName = $"barcode_{cardCode}.png";
                string fullPath = Path.Combine(barcodeDir, fileName);

                picBarcode.Image.Save(fullPath, ImageFormat.Png);

                MessageBox.Show($"Đã lưu barcode!\nĐường dẫn: {fullPath}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu barcode: " + ex.Message);
            }
        }

        private void SetupVariantGrid()
        {
            dgvVariants.AllowUserToAddRows = false;
            dgvVariants.Columns.Clear();
            dgvVariants.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VariantId",
                HeaderText = "ID",
                Visible = false
            });

            dgvVariants.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Rarity",
                HeaderText = "Độ hiếm",
                Width = 160,
                ReadOnly = true
            });

            dgvVariants.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SellPrice",
                HeaderText = "Giá bán",
                Width = 120
            });

            dgvVariants.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CostPrice",
                HeaderText = "Giá vốn",
                Width = 120
            });

            dgvVariants.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Stock",
                HeaderText = "Tồn kho",
                Width = 90
            });

            dgvVariants.ClearSelection();

            dgvVariants.CellValidating -= dgvVariants_CellValidating;
            dgvVariants.CellValidating += dgvVariants_CellValidating;
        }

        private void dgvVariants_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvVariants.Rows[e.RowIndex].IsNewRow) return;

            string colName = dgvVariants.Columns[e.ColumnIndex].Name;

            if (colName == "Stock" || colName == "SellPrice" || colName == "CostPrice")
            {
                if (decimal.TryParse(e.FormattedValue.ToString(), out decimal val) && val < 0)
                {
                    MessageBox.Show("Cấm nhập số âm! Vui lòng sửa lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; 
                }
            }
        }

        private void btnAddVariant_Click(object sender, EventArgs e)
        {
            string category = cbCategory.Text.Trim();
            bool needRarity = category == "SingleCard";

            string rarity = cbRarity.SelectedIndex == -1 ? "" : cbRarity.Text.Trim();

            if (needRarity && string.IsNullOrWhiteSpace(rarity))
            {
                MessageBox.Show("SingleCard phải chọn độ hiếm trước!");
                return;
            }

            if (!needRarity && string.IsNullOrWhiteSpace(rarity))
            {
                rarity = "Default";
            }

            foreach (DataGridViewRow r in dgvVariants.Rows)
            {
                if (r.IsNewRow) continue;

                string existingRarity = r.Cells["Rarity"].Value?.ToString() ?? "";
                if (existingRarity == rarity)
                {
                    MessageBox.Show($"Đã có variant '{rarity}' rồi!");
                    return;
                }
            }

            dgvVariants.Rows.Add(null, rarity, 0, 0, 0);
            cbRarity.SelectedIndex = -1;
        }

        private void btnDeleteVariant_Click(object sender, EventArgs e)
        {
            if (dgvVariants.Rows.Count == 0 || dgvVariants.CurrentRow == null || dgvVariants.CurrentRow.Index < 0)
            {
                MessageBox.Show("Chưa có dòng nào để xóa!");
                return;
            }

            dgvVariants.Rows.RemoveAt(dgvVariants.CurrentRow.Index);
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isSingleCard = cbCategory.Text == "SingleCard";

            cbRarity.Enabled = isSingleCard;
            cbAttribute.Enabled = isSingleCard;
            cbCardType.Enabled = isSingleCard;

            if (!isSingleCard)
            {
                cbRarity.SelectedIndex = -1;
                cbAttribute.SelectedIndex = -1;
                cbCardType.SelectedIndex = -1;
            }
        }
    }
}
