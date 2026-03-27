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

            txtSellPrice.Maximum = 1000000000;
            txtSellPrice.Minimum = 0;
            txtSellPrice.ThousandsSeparator = true;

            txtCostPrice.Maximum = 1000000000;
            txtCostPrice.Minimum = 0;
            txtCostPrice.ThousandsSeparator = true;

            chkIsActive.Checked = true;

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
                string code = txtCode.Text.Trim().Replace("'", "''");
                string cardCode = txtCardCode.Text.Trim().Replace("'", "''");
                string name = txtName.Text.Trim().Replace("'", "''");
                string category = cbCategory.Text.Trim().Replace("'", "''");
                string imagePath = txtImagePath.Text.Trim().Replace("'", "''");
                string note = txtNote.Text.Trim().Replace("'", "''");

                if (code == "" || name == "")
                {
                    MessageBox.Show("Vui lòng nhập mã sản phẩm và tên sản phẩm!");
                    return;
                }

                decimal sellPrice = txtSellPrice.Value;
                decimal costPrice = txtCostPrice.Value;
                int stock = 0;
                int.TryParse(txtStock.Text.Trim(), out stock);
                int isActive = chkIsActive.Checked ? 1 : 0;

                string sellPriceSql = sellPrice.ToString(CultureInfo.InvariantCulture);
                string costPriceSql = costPrice.ToString(CultureInfo.InvariantCulture);

                string sql = "";

                if (editingProductId == null)
                {
                    sql = $@"
                INSERT INTO Products
                (Code, CardCode, Name, Category, SellPrice, CostPrice, Stock, IsActive, CreatedAt, ImagePath)
                VALUES
                (N'{code}', N'{cardCode}', N'{name}', N'{category}', {sellPriceSql}, {costPriceSql}, {stock}, {isActive}, GETDATE(), N'{imagePath}')";
                }
                else
                {
                    sql = $@"
                UPDATE Products
                SET Code = N'{code}',
                    CardCode = N'{cardCode}',
                    Name = N'{name}',
                    Category = N'{category}',
                    SellPrice = {sellPriceSql},
                    CostPrice = {costPriceSql},
                    Stock = {stock},
                    IsActive = {isActive},
                    ImagePath = N'{imagePath}'
                WHERE ProductId = {editingProductId}";
                }

                int result = DbHelper.Execute(sql);

                if (result > 0)
                {
                    MessageBox.Show(editingProductId == null
                        ? "Thêm sản phẩm thành công!"
                        : "Sửa sản phẩm thành công!");

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu nào được lưu.");
                }
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
                txtSellPrice.Value = Convert.ToDecimal(row["SellPrice"]);
                txtCostPrice.Value = Convert.ToDecimal(row["CostPrice"]);
                txtStock.Text = row["Stock"].ToString();
                txtImagePath.Text = row["ImagePath"].ToString();

                string imagePath = row["ImagePath"].ToString();
                string fullPath = Path.Combine(Application.StartupPath, imagePath);

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(fullPath))
                {
                    var oldImage = picProduct.Image;
                    picProduct.Image = null;
                    oldImage?.Dispose();

                    var ms = new System.IO.MemoryStream(File.ReadAllBytes(fullPath));
                    picProduct.Image = Image.FromStream(ms);
                    picProduct.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    picProduct.Image = null; 
                }

                chkIsActive.Checked = Convert.ToBoolean(row["IsActive"]); 

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu sản phẩm: " + ex.Message);
            }
        }

        private void btnCancel_TextChanged(object sender, EventArgs e)
        {
            this.Close();
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
            catch {}
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
                    PureBarcode = false // hiển thị text bên dưới barcode
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
                // Lưu vào thư mục project thay vì bin\Debug
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

    }
}
