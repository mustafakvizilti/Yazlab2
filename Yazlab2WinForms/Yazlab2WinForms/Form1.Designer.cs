namespace Yazlab2WinForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.txtNodeName = new System.Windows.Forms.TextBox();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.cmbRelType = new System.Windows.Forms.ComboBox();
            this.numWeight = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).BeginInit();
            this.SuspendLayout();
            // 
            // txtNodeName
            // 
            this.txtNodeName.Location = new System.Drawing.Point(0, 0);
            this.txtNodeName.Name = "txtNodeName";
            this.txtNodeName.Size = new System.Drawing.Size(100, 22);
            this.txtNodeName.TabIndex = 0;
            this.txtNodeName.Text = "Düğüm adı";
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(0, 28);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(100, 22);
            this.txtSource.TabIndex = 1;
            this.txtSource.Text = "Kaynak";
            // 
            // txtTarget
            // 
            this.txtTarget.Location = new System.Drawing.Point(0, 56);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(100, 22);
            this.txtTarget.TabIndex = 2;
            this.txtTarget.Text = "Hedef";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 113);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Düğüm Ekle";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnAddNode_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 84);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Bağlantı Ekle";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnAddEdge_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(-3, 199);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(105, 16);
            this.lblInfo.TabIndex = 6;
            this.lblInfo.Text = "Bilgi bekleniyor..";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(242, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(557, 426);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(0, 173);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "En Kısa Yol";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnShortestPath_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(0, 142);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(146, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "En Güçlü Düğümü Bul";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cmbRelType
            // 
            this.cmbRelType.FormattingEnabled = true;
            this.cmbRelType.Location = new System.Drawing.Point(106, 83);
            this.cmbRelType.Name = "cmbRelType";
            this.cmbRelType.Size = new System.Drawing.Size(91, 24);
            this.cmbRelType.TabIndex = 10;
            // 
            // numWeight
            // 
            this.numWeight.Location = new System.Drawing.Point(203, 83);
            this.numWeight.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numWeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numWeight.Name = "numWeight";
            this.numWeight.Size = new System.Drawing.Size(41, 22);
            this.numWeight.TabIndex = 11;
            this.numWeight.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.numWeight);
            this.Controls.Add(this.cmbRelType);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.txtNodeName);
            this.Name = "Form1";
            this.Text = "Graf Çizim Uygulaması";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.TextBox txtNodeName;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ComboBox cmbRelType;
        private System.Windows.Forms.NumericUpDown numWeight;
    }
}
