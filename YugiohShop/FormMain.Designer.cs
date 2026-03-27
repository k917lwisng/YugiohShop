namespace YugiohShop
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            leftPanel = new Panel();
            btnLogout = new Guna.UI2.WinForms.Guna2Button();
            btnSales = new Guna.UI2.WinForms.Guna2Button();
            btnStatistics = new Guna.UI2.WinForms.Guna2Button();
            btnCustomers = new Guna.UI2.WinForms.Guna2Button();
            btnProducts = new Guna.UI2.WinForms.Guna2Button();
            btnUser = new Guna.UI2.WinForms.Guna2Button();
            rightPanel = new Panel();
            lblCurrentUser = new Label();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            leftPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            guna2Panel1.SuspendLayout();
            SuspendLayout();
            // 
            // leftPanel
            // 
            leftPanel.BackColor = Color.FromArgb(224, 224, 224);
            leftPanel.Controls.Add(btnLogout);
            leftPanel.Controls.Add(btnSales);
            leftPanel.Controls.Add(btnStatistics);
            leftPanel.Controls.Add(btnCustomers);
            leftPanel.Controls.Add(btnProducts);
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Location = new Point(0, 46);
            leftPanel.Margin = new Padding(2);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(174, 224);
            leftPanel.TabIndex = 0;
            leftPanel.Click += btnSales_Click;
            leftPanel.Paint += leftPanel_Paint;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.Transparent;
            btnLogout.BorderRadius = 5;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.CustomizableEdges = customizableEdges1;
            btnLogout.DisabledState.BorderColor = Color.DarkGray;
            btnLogout.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLogout.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLogout.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLogout.FillColor = Color.Empty;
            btnLogout.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogout.ForeColor = Color.Black;
            btnLogout.Image = (Image)resources.GetObject("btnLogout.Image");
            btnLogout.ImageAlign = HorizontalAlignment.Left;
            btnLogout.Location = new Point(12, 165);
            btnLogout.Name = "btnLogout";
            btnLogout.Padding = new Padding(10, 0, 0, 0);
            btnLogout.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnLogout.Size = new Size(119, 23);
            btnLogout.TabIndex = 9;
            btnLogout.Text = "Đăng xuất";
            btnLogout.TextAlign = HorizontalAlignment.Left;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnSales
            // 
            btnSales.BackColor = Color.Transparent;
            btnSales.BorderRadius = 5;
            btnSales.Cursor = Cursors.Hand;
            btnSales.CustomizableEdges = customizableEdges3;
            btnSales.DisabledState.BorderColor = Color.DarkGray;
            btnSales.DisabledState.CustomBorderColor = Color.DarkGray;
            btnSales.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSales.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnSales.FillColor = Color.Empty;
            btnSales.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSales.ForeColor = Color.Black;
            btnSales.Image = (Image)resources.GetObject("btnSales.Image");
            btnSales.ImageAlign = HorizontalAlignment.Left;
            btnSales.Location = new Point(12, 74);
            btnSales.Name = "btnSales";
            btnSales.Padding = new Padding(10, 0, 0, 0);
            btnSales.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnSales.Size = new Size(119, 23);
            btnSales.TabIndex = 7;
            btnSales.Text = "Bán hàng";
            btnSales.TextAlign = HorizontalAlignment.Left;
            btnSales.Click += btnSales_Click;
            // 
            // btnStatistics
            // 
            btnStatistics.BackColor = Color.Transparent;
            btnStatistics.BorderRadius = 5;
            btnStatistics.Cursor = Cursors.Hand;
            btnStatistics.CustomizableEdges = customizableEdges5;
            btnStatistics.DisabledState.BorderColor = Color.DarkGray;
            btnStatistics.DisabledState.CustomBorderColor = Color.DarkGray;
            btnStatistics.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnStatistics.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnStatistics.FillColor = Color.Empty;
            btnStatistics.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStatistics.ForeColor = Color.Black;
            btnStatistics.Image = (Image)resources.GetObject("btnStatistics.Image");
            btnStatistics.ImageAlign = HorizontalAlignment.Left;
            btnStatistics.Location = new Point(12, 103);
            btnStatistics.Name = "btnStatistics";
            btnStatistics.Padding = new Padding(10, 0, 0, 0);
            btnStatistics.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnStatistics.Size = new Size(119, 23);
            btnStatistics.TabIndex = 8;
            btnStatistics.Text = "Thống kê";
            btnStatistics.TextAlign = HorizontalAlignment.Left;
            btnStatistics.Click += btnStatistics_Click;
            // 
            // btnCustomers
            // 
            btnCustomers.BackColor = Color.Transparent;
            btnCustomers.BorderRadius = 5;
            btnCustomers.Cursor = Cursors.Hand;
            btnCustomers.CustomizableEdges = customizableEdges7;
            btnCustomers.DisabledState.BorderColor = Color.DarkGray;
            btnCustomers.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCustomers.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCustomers.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCustomers.FillColor = Color.Empty;
            btnCustomers.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCustomers.ForeColor = Color.Black;
            btnCustomers.Image = (Image)resources.GetObject("btnCustomers.Image");
            btnCustomers.ImageAlign = HorizontalAlignment.Left;
            btnCustomers.Location = new Point(12, 45);
            btnCustomers.Name = "btnCustomers";
            btnCustomers.Padding = new Padding(10, 0, 0, 0);
            btnCustomers.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnCustomers.Size = new Size(119, 23);
            btnCustomers.TabIndex = 6;
            btnCustomers.Text = "Khách hàng        ";
            btnCustomers.TextAlign = HorizontalAlignment.Left;
            btnCustomers.Click += btnCustomers_Click;
            // 
            // btnProducts
            // 
            btnProducts.BackColor = Color.Transparent;
            btnProducts.BorderRadius = 5;
            btnProducts.Cursor = Cursors.Hand;
            btnProducts.CustomizableEdges = customizableEdges9;
            btnProducts.DisabledState.BorderColor = Color.DarkGray;
            btnProducts.DisabledState.CustomBorderColor = Color.DarkGray;
            btnProducts.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnProducts.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnProducts.FillColor = Color.Empty;
            btnProducts.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnProducts.ForeColor = Color.Black;
            btnProducts.Image = (Image)resources.GetObject("btnProducts.Image");
            btnProducts.ImageAlign = HorizontalAlignment.Left;
            btnProducts.Location = new Point(12, 16);
            btnProducts.Name = "btnProducts";
            btnProducts.Padding = new Padding(10, 0, 0, 0);
            btnProducts.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnProducts.Size = new Size(119, 23);
            btnProducts.TabIndex = 5;
            btnProducts.Text = "Sản phẩm";
            btnProducts.TextAlign = HorizontalAlignment.Left;
            btnProducts.Click += btnProducts_Click;
            // 
            // btnUser
            // 
            btnUser.BackColor = Color.Transparent;
            btnUser.BorderRadius = 5;
            btnUser.Cursor = Cursors.Hand;
            btnUser.CustomizableEdges = customizableEdges11;
            btnUser.DisabledState.BorderColor = Color.DarkGray;
            btnUser.DisabledState.CustomBorderColor = Color.DarkGray;
            btnUser.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnUser.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnUser.FillColor = Color.Empty;
            btnUser.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnUser.ForeColor = Color.Black;
            btnUser.Image = (Image)resources.GetObject("btnUser.Image");
            btnUser.ImageAlign = HorizontalAlignment.Left;
            btnUser.Location = new Point(405, 12);
            btnUser.Name = "btnUser";
            btnUser.Padding = new Padding(10, 0, 0, 0);
            btnUser.ShadowDecoration.CustomizableEdges = customizableEdges12;
            btnUser.Size = new Size(127, 24);
            btnUser.TabIndex = 10;
            btnUser.Text = "Người dùng";
            btnUser.TextAlign = HorizontalAlignment.Left;
            btnUser.Click += btnUser_Click;
            // 
            // rightPanel
            // 
            rightPanel.BackColor = SystemColors.ButtonHighlight;
            rightPanel.Controls.Add(lblCurrentUser);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(174, 46);
            rightPanel.Margin = new Padding(2);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(386, 224);
            rightPanel.TabIndex = 1;
            // 
            // lblCurrentUser
            // 
            lblCurrentUser.AutoSize = true;
            lblCurrentUser.Location = new Point(22, 5);
            lblCurrentUser.Margin = new Padding(2, 0, 2, 0);
            lblCurrentUser.Name = "lblCurrentUser";
            lblCurrentUser.Size = new Size(0, 15);
            lblCurrentUser.TabIndex = 0;
            // 
            // guna2Panel1
            // 
            guna2Panel1.Controls.Add(guna2HtmlLabel1);
            guna2Panel1.Controls.Add(btnUser);
            guna2Panel1.CustomizableEdges = customizableEdges13;
            guna2Panel1.Dock = DockStyle.Top;
            guna2Panel1.Location = new Point(0, 0);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges14;
            guna2Panel1.Size = new Size(560, 46);
            guna2Panel1.TabIndex = 1;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = Color.Black;
            guna2HtmlLabel1.Location = new Point(59, 12);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(150, 22);
            guna2HtmlLabel1.TabIndex = 1;
            guna2HtmlLabel1.Text = "HỆ THỐNG QUẢN LÝ";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(560, 270);
            Controls.Add(rightPanel);
            Controls.Add(leftPanel);
            Controls.Add(guna2Panel1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(2);
            Name = "FormMain";
            Text = "Yugioh's Shop";
            WindowState = FormWindowState.Maximized;
            Load += FormMain_Load;
            leftPanel.ResumeLayout(false);
            rightPanel.ResumeLayout(false);
            rightPanel.PerformLayout();
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel leftPanel;
        private Panel rightPanel;
        private Label lblCurrentUser;
        private Guna.UI2.WinForms.Guna2Button btnProducts;
        private Guna.UI2.WinForms.Guna2Button btnSales;
        private Guna.UI2.WinForms.Guna2Button btnCustomers;
        private Guna.UI2.WinForms.Guna2Button btnLogout;
        private Guna.UI2.WinForms.Guna2Button btnStatistics;
        private Guna.UI2.WinForms.Guna2Button btnUser;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
    }
}
