namespace YugiohShop
{
    partial class UCLowStockItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlContainer = new Guna.UI2.WinForms.Guna2Panel();
            progressStock = new Guna.UI2.WinForms.Guna2ProgressBar();
            lblStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblQuantity = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblProductName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlContainer.SuspendLayout();
            SuspendLayout();
            // 
            // pnlContainer
            // 
            pnlContainer.BackColor = Color.Transparent;
            pnlContainer.Controls.Add(progressStock);
            pnlContainer.Controls.Add(lblStatus);
            pnlContainer.Controls.Add(lblQuantity);
            pnlContainer.Controls.Add(lblProductName);
            pnlContainer.CustomizableEdges = customizableEdges3;
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.FillColor = Color.White;
            pnlContainer.Location = new Point(0, 0);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlContainer.Size = new Size(354, 60);
            pnlContainer.TabIndex = 0;
            // 
            // progressStock
            // 
            progressStock.CustomizableEdges = customizableEdges1;
            progressStock.Font = new Font("Be Vietnam Pro", 8.25F, FontStyle.Bold);
            progressStock.ForeColor = Color.Black;
            progressStock.Location = new Point(121, 30);
            progressStock.Name = "progressStock";
            progressStock.ShadowDecoration.CustomizableEdges = customizableEdges2;
            progressStock.Size = new Size(140, 6);
            progressStock.TabIndex = 3;
            progressStock.Text = "guna2ProgressBar1";
            progressStock.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Be Vietnam Pro", 8.25F, FontStyle.Bold);
            lblStatus.ForeColor = Color.Black;
            lblStatus.Location = new Point(286, 16);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(62, 19);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Trạng thái";
            // 
            // lblQuantity
            // 
            lblQuantity.BackColor = Color.Transparent;
            lblQuantity.Font = new Font("Be Vietnam Pro", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblQuantity.ForeColor = SystemColors.ControlDarkDark;
            lblQuantity.Location = new Point(13, 30);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new Size(54, 19);
            lblQuantity.TabIndex = 1;
            lblQuantity.Text = "Còn lại: 0";
            // 
            // lblProductName
            // 
            lblProductName.BackColor = Color.Transparent;
            lblProductName.Font = new Font("Be Vietnam Pro", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblProductName.Location = new Point(13, 3);
            lblProductName.Name = "lblProductName";
            lblProductName.Size = new Size(92, 21);
            lblProductName.TabIndex = 0;
            lblProductName.Text = "TenSanPham";
            // 
            // UCLowStockItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlContainer);
            Name = "UCLowStockItem";
            Size = new Size(354, 60);
            pnlContainer.ResumeLayout(false);
            pnlContainer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlContainer;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblProductName;
        private Guna.UI2.WinForms.Guna2ProgressBar progressStock;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStatus;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblQuantity;
    }
}
