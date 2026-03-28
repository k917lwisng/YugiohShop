namespace YugiohShop
{
    partial class FormProducts
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProducts));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlTop = new Guna.UI2.WinForms.Guna2Panel();
            btnClearFilter = new Guna.UI2.WinForms.Guna2Button();
            cbCategoryFilter = new Guna.UI2.WinForms.Guna2ComboBox();
            txtSearchProduct = new Guna.UI2.WinForms.Guna2TextBox();
            flpProducts = new FlowLayoutPanel();
            LeftPanel = new Panel();
            btnRefreshProduct = new Guna.UI2.WinForms.Guna2Button();
            btnEditProduct = new Guna.UI2.WinForms.Guna2Button();
            btnAddProduct = new Guna.UI2.WinForms.Guna2Button();
            btnDeleteProduct = new Guna.UI2.WinForms.Guna2Button();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            lblMenuProducts = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlTop.SuspendLayout();
            LeftPanel.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.White;
            pnlTop.Controls.Add(btnClearFilter);
            pnlTop.Controls.Add(cbCategoryFilter);
            pnlTop.Controls.Add(txtSearchProduct);
            pnlTop.CustomizableEdges = customizableEdges7;
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(225, 15);
            pnlTop.Margin = new Padding(2);
            pnlTop.Name = "pnlTop";
            pnlTop.ShadowDecoration.CustomizableEdges = customizableEdges8;
            pnlTop.Size = new Size(635, 60);
            pnlTop.TabIndex = 1;
            // 
            // btnClearFilter
            // 
            btnClearFilter.BorderColor = Color.Empty;
            btnClearFilter.BorderRadius = 8;
            btnClearFilter.Cursor = Cursors.Hand;
            btnClearFilter.CustomizableEdges = customizableEdges1;
            btnClearFilter.DisabledState.BorderColor = Color.DarkGray;
            btnClearFilter.DisabledState.CustomBorderColor = Color.DarkGray;
            btnClearFilter.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnClearFilter.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnClearFilter.FillColor = Color.Empty;
            btnClearFilter.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClearFilter.ForeColor = Color.White;
            btnClearFilter.Image = (Image)resources.GetObject("btnClearFilter.Image");
            btnClearFilter.Location = new Point(435, 11);
            btnClearFilter.Margin = new Padding(2);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnClearFilter.Size = new Size(32, 36);
            btnClearFilter.TabIndex = 3;
            btnClearFilter.Click += btnClearFilter_Click;
            // 
            // cbCategoryFilter
            // 
            cbCategoryFilter.BackColor = Color.Transparent;
            cbCategoryFilter.BorderColor = Color.FromArgb(26, 115, 232);
            cbCategoryFilter.BorderRadius = 3;
            cbCategoryFilter.BorderThickness = 2;
            cbCategoryFilter.Cursor = Cursors.Hand;
            cbCategoryFilter.CustomizableEdges = customizableEdges3;
            cbCategoryFilter.DrawMode = DrawMode.OwnerDrawFixed;
            cbCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCategoryFilter.FocusedColor = Color.FromArgb(94, 148, 255);
            cbCategoryFilter.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cbCategoryFilter.Font = new Font("Segoe UI", 10F);
            cbCategoryFilter.ForeColor = Color.FromArgb(148, 163, 184);
            cbCategoryFilter.ItemHeight = 30;
            cbCategoryFilter.Items.AddRange(new object[] { "Tất cả", "SingleCard", "Pack", "FullBox" });
            cbCategoryFilter.ItemsAppearance.ForeColor = Color.FromArgb(30, 30, 45);
            cbCategoryFilter.Location = new Point(315, 11);
            cbCategoryFilter.Margin = new Padding(2);
            cbCategoryFilter.Name = "cbCategoryFilter";
            cbCategoryFilter.ShadowDecoration.CustomizableEdges = customizableEdges4;
            cbCategoryFilter.Size = new Size(101, 36);
            cbCategoryFilter.TabIndex = 2;
            cbCategoryFilter.SelectedIndexChanged += cbCategoryFilter_SelectedIndexChanged;
            // 
            // txtSearchProduct
            // 
            txtSearchProduct.BackColor = Color.Transparent;
            txtSearchProduct.BorderColor = Color.FromArgb(26, 115, 232);
            txtSearchProduct.BorderRadius = 3;
            txtSearchProduct.BorderThickness = 2;
            txtSearchProduct.Cursor = Cursors.Hand;
            txtSearchProduct.CustomizableEdges = customizableEdges5;
            txtSearchProduct.DefaultText = "";
            txtSearchProduct.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtSearchProduct.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtSearchProduct.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtSearchProduct.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtSearchProduct.FillColor = Color.FromArgb(241, 245, 249);
            txtSearchProduct.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearchProduct.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtSearchProduct.ForeColor = Color.FromArgb(30, 41, 59);
            txtSearchProduct.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearchProduct.IconLeft = (Image)resources.GetObject("txtSearchProduct.IconLeft");
            txtSearchProduct.Location = new Point(16, 11);
            txtSearchProduct.Name = "txtSearchProduct";
            txtSearchProduct.PlaceholderText = "";
            txtSearchProduct.SelectedText = "";
            txtSearchProduct.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtSearchProduct.ShadowDecoration.Shadow = new Padding(0, 2, 0, 2);
            txtSearchProduct.Size = new Size(282, 36);
            txtSearchProduct.TabIndex = 0;
            txtSearchProduct.TextChanged += txtSearchProduct_TextChanged;
            // 
            // flpProducts
            // 
            flpProducts.AutoScroll = true;
            flpProducts.BackColor = Color.White;
            flpProducts.Dock = DockStyle.Fill;
            flpProducts.Location = new Point(225, 75);
            flpProducts.Margin = new Padding(2);
            flpProducts.Name = "flpProducts";
            flpProducts.Size = new Size(635, 349);
            flpProducts.TabIndex = 2;
            // 
            // LeftPanel
            // 
            LeftPanel.BackColor = Color.White;
            LeftPanel.Controls.Add(btnRefreshProduct);
            LeftPanel.Controls.Add(btnEditProduct);
            LeftPanel.Controls.Add(btnAddProduct);
            LeftPanel.Controls.Add(btnDeleteProduct);
            LeftPanel.Dock = DockStyle.Left;
            LeftPanel.Location = new Point(15, 15);
            LeftPanel.Margin = new Padding(2);
            LeftPanel.Name = "LeftPanel";
            LeftPanel.Padding = new Padding(10, 36, 10, 12);
            LeftPanel.Size = new Size(210, 409);
            LeftPanel.TabIndex = 1;
            // 
            // btnRefreshProduct
            // 
            btnRefreshProduct.BorderRadius = 5;
            btnRefreshProduct.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            btnRefreshProduct.Cursor = Cursors.Hand;
            btnRefreshProduct.CustomizableEdges = customizableEdges9;
            btnRefreshProduct.DisabledState.BorderColor = Color.DarkGray;
            btnRefreshProduct.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRefreshProduct.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRefreshProduct.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRefreshProduct.FillColor = Color.Empty;
            btnRefreshProduct.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRefreshProduct.ForeColor = Color.Black;
            btnRefreshProduct.HoverState.FillColor = Color.FromArgb(59, 130, 246);
            btnRefreshProduct.Image = (Image)resources.GetObject("btnRefreshProduct.Image");
            btnRefreshProduct.ImageAlign = HorizontalAlignment.Left;
            btnRefreshProduct.Location = new Point(45, 199);
            btnRefreshProduct.Margin = new Padding(2);
            btnRefreshProduct.Name = "btnRefreshProduct";
            btnRefreshProduct.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnRefreshProduct.Size = new Size(116, 21);
            btnRefreshProduct.TabIndex = 3;
            btnRefreshProduct.Text = "Làm mới";
            btnRefreshProduct.TextAlign = HorizontalAlignment.Left;
            btnRefreshProduct.Click += btnRefreshProduct_Click;
            // 
            // btnEditProduct
            // 
            btnEditProduct.BorderColor = Color.Empty;
            btnEditProduct.BorderRadius = 5;
            btnEditProduct.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            btnEditProduct.Cursor = Cursors.Hand;
            btnEditProduct.CustomizableEdges = customizableEdges11;
            btnEditProduct.DisabledState.BorderColor = Color.DarkGray;
            btnEditProduct.DisabledState.CustomBorderColor = Color.DarkGray;
            btnEditProduct.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnEditProduct.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnEditProduct.FillColor = Color.Empty;
            btnEditProduct.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEditProduct.ForeColor = Color.Black;
            btnEditProduct.HoverState.FillColor = Color.FromArgb(59, 130, 246);
            btnEditProduct.Image = (Image)resources.GetObject("btnEditProduct.Image");
            btnEditProduct.ImageAlign = HorizontalAlignment.Left;
            btnEditProduct.Location = new Point(45, 129);
            btnEditProduct.Margin = new Padding(2);
            btnEditProduct.Name = "btnEditProduct";
            btnEditProduct.ShadowDecoration.CustomizableEdges = customizableEdges12;
            btnEditProduct.Size = new Size(116, 21);
            btnEditProduct.TabIndex = 2;
            btnEditProduct.Text = "Sửa SP";
            btnEditProduct.TextAlign = HorizontalAlignment.Left;
            btnEditProduct.Click += btnEditProduct_Click;
            // 
            // btnAddProduct
            // 
            btnAddProduct.BorderColor = Color.Empty;
            btnAddProduct.BorderRadius = 5;
            btnAddProduct.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            btnAddProduct.Cursor = Cursors.Hand;
            btnAddProduct.CustomizableEdges = customizableEdges13;
            btnAddProduct.DisabledState.BorderColor = Color.DarkGray;
            btnAddProduct.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAddProduct.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAddProduct.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAddProduct.FillColor = Color.Empty;
            btnAddProduct.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddProduct.ForeColor = Color.Black;
            btnAddProduct.HoverState.FillColor = Color.FromArgb(59, 130, 246);
            btnAddProduct.Image = (Image)resources.GetObject("btnAddProduct.Image");
            btnAddProduct.ImageAlign = HorizontalAlignment.Left;
            btnAddProduct.Location = new Point(45, 92);
            btnAddProduct.Margin = new Padding(2);
            btnAddProduct.Name = "btnAddProduct";
            btnAddProduct.ShadowDecoration.CustomizableEdges = customizableEdges14;
            btnAddProduct.Size = new Size(116, 21);
            btnAddProduct.TabIndex = 0;
            btnAddProduct.Text = "Thêm SP";
            btnAddProduct.TextAlign = HorizontalAlignment.Left;
            btnAddProduct.Click += btnAddProduct_Click;
            // 
            // btnDeleteProduct
            // 
            btnDeleteProduct.BorderRadius = 5;
            btnDeleteProduct.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            btnDeleteProduct.Cursor = Cursors.Hand;
            btnDeleteProduct.CustomizableEdges = customizableEdges15;
            btnDeleteProduct.DisabledState.BorderColor = Color.DarkGray;
            btnDeleteProduct.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDeleteProduct.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDeleteProduct.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDeleteProduct.FillColor = Color.Empty;
            btnDeleteProduct.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteProduct.ForeColor = Color.Black;
            btnDeleteProduct.HoverState.FillColor = Color.FromArgb(59, 130, 246);
            btnDeleteProduct.Image = (Image)resources.GetObject("btnDeleteProduct.Image");
            btnDeleteProduct.ImageAlign = HorizontalAlignment.Left;
            btnDeleteProduct.Location = new Point(45, 164);
            btnDeleteProduct.Margin = new Padding(2);
            btnDeleteProduct.Name = "btnDeleteProduct";
            btnDeleteProduct.ShadowDecoration.CustomizableEdges = customizableEdges16;
            btnDeleteProduct.Size = new Size(116, 21);
            btnDeleteProduct.TabIndex = 1;
            btnDeleteProduct.Text = "Xóa SP";
            btnDeleteProduct.TextAlign = HorizontalAlignment.Left;
            btnDeleteProduct.Click += btnDeleteProduct_Click;
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 200;
            // 
            // lblMenuProducts
            // 
            lblMenuProducts.BackColor = Color.Transparent;
            lblMenuProducts.Location = new Point(105, 60);
            lblMenuProducts.Margin = new Padding(2);
            lblMenuProducts.Name = "lblMenuProducts";
            lblMenuProducts.Size = new Size(61, 17);
            lblMenuProducts.TabIndex = 4;
            lblMenuProducts.Text = "Chức năng";
            // 
            // FormProducts
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(875, 439);
            Controls.Add(flpProducts);
            Controls.Add(pnlTop);
            Controls.Add(LeftPanel);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(2);
            Name = "FormProducts";
            Padding = new Padding(15);
            Text = "ca";
            WindowState = FormWindowState.Maximized;
            Load += FormProducts_Load;
            pnlTop.ResumeLayout(false);
            LeftPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Guna.UI2.WinForms.Guna2Panel pnlTop;
        private FlowLayoutPanel flpProducts;
        private Guna.UI2.WinForms.Guna2Button btnClearFilter;
        private Guna.UI2.WinForms.Guna2TextBox txtSearchProduct;
        private Guna.UI2.WinForms.Guna2ComboBox cbCategoryFilter;
        private Panel LeftPanel;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Button btnRefreshProduct;
        private Guna.UI2.WinForms.Guna2Button btnEditProduct;
        private Guna.UI2.WinForms.Guna2Button btnAddProduct;
        private Guna.UI2.WinForms.Guna2Button btnDeleteProduct;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblMenuProducts;
    }
}