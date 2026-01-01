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
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.numAktiflik = new System.Windows.Forms.NumericUpDown();
            this.buttonUpdateNode = new System.Windows.Forms.Button();
            this.buttonAStar = new System.Windows.Forms.Button();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.Algoritma = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNodeCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Yol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Maliyet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAktiflik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
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
            this.txtSource.Location = new System.Drawing.Point(97, 0);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(54, 22);
            this.txtSource.TabIndex = 1;
            this.txtSource.Text = "Kaynak";
            // 
            // txtTarget
            // 
            this.txtTarget.Location = new System.Drawing.Point(97, 28);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(54, 22);
            this.txtTarget.TabIndex = 2;
            this.txtTarget.Text = "Hedef";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(-2, 71);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 51);
            this.button1.TabIndex = 3;
            this.button1.Text = "Düğüm Ekle";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnAddNode_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(-2, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 50);
            this.button2.TabIndex = 4;
            this.button2.Text = "Bağlantı Ekle";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnAddEdge_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(12, 203);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(105, 16);
            this.lblInfo.TabIndex = 6;
            this.lblInfo.Text = "Bilgi bekleniyor..";
            this.lblInfo.Click += new System.EventHandler(this.lblInfo_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(242, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(557, 426);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(120, 247);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(124, 31);
            this.button3.TabIndex = 8;
            this.button3.Text = "Dijkstra";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnShortestPath_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(120, 307);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(124, 64);
            this.button4.TabIndex = 9;
            this.button4.Text = "En Güçlü Düğümü Bul";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cmbRelType
            // 
            this.cmbRelType.FormattingEnabled = true;
            this.cmbRelType.Location = new System.Drawing.Point(97, 56);
            this.cmbRelType.Name = "cmbRelType";
            this.cmbRelType.Size = new System.Drawing.Size(77, 24);
            this.cmbRelType.TabIndex = 10;
            // 
            // numWeight
            // 
            this.numWeight.Location = new System.Drawing.Point(97, 86);
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
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(120, 276);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(124, 35);
            this.button5.TabIndex = 12;
            this.button5.Text = "Top5Node";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(0, 307);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(124, 34);
            this.button6.TabIndex = 13;
            this.button6.Text = "Düğüm Sil";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.btnDeleteNode_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(-2, 368);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(126, 32);
            this.button7.TabIndex = 14;
            this.button7.Text = "Bağlantı Sil";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.btnDeleteEdge_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(-2, 397);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(126, 32);
            this.button8.TabIndex = 15;
            this.button8.Text = "Boya";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.btnColoring_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(-2, 276);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(126, 35);
            this.button9.TabIndex = 16;
            this.button9.Text = "Bfs";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.btnBFS_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(0, 247);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(124, 31);
            this.button10.TabIndex = 17;
            this.button10.Text = "Dfs";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.btnDFS_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(120, 368);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(124, 32);
            this.button11.TabIndex = 18;
            this.button11.Text = "Kaydet";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(120, 397);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(124, 32);
            this.button12.TabIndex = 19;
            this.button12.Text = "Yükle";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // numAktiflik
            // 
            this.numAktiflik.DecimalPlaces = 1;
            this.numAktiflik.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numAktiflik.Location = new System.Drawing.Point(97, 114);
            this.numAktiflik.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAktiflik.Name = "numAktiflik";
            this.numAktiflik.Size = new System.Drawing.Size(41, 22);
            this.numAktiflik.TabIndex = 20;
            this.numAktiflik.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // buttonUpdateNode
            // 
            this.buttonUpdateNode.Location = new System.Drawing.Point(0, 338);
            this.buttonUpdateNode.Name = "buttonUpdateNode";
            this.buttonUpdateNode.Size = new System.Drawing.Size(124, 33);
            this.buttonUpdateNode.TabIndex = 21;
            this.buttonUpdateNode.Text = "Düğümü Güncelle";
            this.buttonUpdateNode.UseVisualStyleBackColor = true;
            this.buttonUpdateNode.Click += new System.EventHandler(this.buttonUpdateNode_Click);
            // 
            // buttonAStar
            // 
            this.buttonAStar.Location = new System.Drawing.Point(120, 222);
            this.buttonAStar.Name = "buttonAStar";
            this.buttonAStar.Size = new System.Drawing.Size(124, 31);
            this.buttonAStar.TabIndex = 22;
            this.buttonAStar.Text = "A*";
            this.buttonAStar.UseVisualStyleBackColor = true;
            this.buttonAStar.Click += new System.EventHandler(this.buttonAStar_Click);
            // 
            // dgvResults
            // 
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Algoritma,
            this.colNodeCount,
            this.Yol,
            this.colTime,
            this.Maliyet});
            this.dgvResults.Location = new System.Drawing.Point(805, 12);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowHeadersWidth = 51;
            this.dgvResults.RowTemplate.Height = 24;
            this.dgvResults.Size = new System.Drawing.Size(579, 426);
            this.dgvResults.TabIndex = 23;
            this.dgvResults.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResults_CellContentClick);
            // 
            // Algoritma
            // 
            this.Algoritma.HeaderText = "Algoritma Adı";
            this.Algoritma.MinimumWidth = 6;
            this.Algoritma.Name = "Algoritma";
            this.Algoritma.Width = 125;
            // 
            // colNodeCount
            // 
            this.colNodeCount.HeaderText = "Düğüm Sayısı";
            this.colNodeCount.MinimumWidth = 6;
            this.colNodeCount.Name = "colNodeCount";
            this.colNodeCount.Width = 125;
            // 
            // Yol
            // 
            this.Yol.HeaderText = "İzlenen Yol";
            this.Yol.MinimumWidth = 6;
            this.Yol.Name = "Yol";
            this.Yol.Width = 125;
            // 
            // colTime
            // 
            this.colTime.HeaderText = "Süre";
            this.colTime.MinimumWidth = 6;
            this.colTime.Name = "colTime";
            this.colTime.Width = 125;
            // 
            // Maliyet
            // 
            this.Maliyet.HeaderText = "Toplam Dinamik Maliyet";
            this.Maliyet.MinimumWidth = 6;
            this.Maliyet.Name = "Maliyet";
            this.Maliyet.Width = 125;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1386, 450);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.buttonAStar);
            this.Controls.Add(this.buttonUpdateNode);
            this.Controls.Add(this.numAktiflik);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
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
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Graf Çizim Uygulaması";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAktiflik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
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
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.NumericUpDown numAktiflik;
        private System.Windows.Forms.Button buttonUpdateNode;
        private System.Windows.Forms.Button buttonAStar;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn Algoritma;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNodeCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Yol;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Maliyet;
    }
}
