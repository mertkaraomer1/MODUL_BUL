namespace MODUL_BUL
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button1 = new Button();
            advancedDataGridView1 = new Zuby.ADGV.AdvancedDataGridView();
            button2 = new Button();
            fontDialog1 = new FontDialog();
            toolStrip1 = new ToolStrip();
            toolStripButton1 = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)advancedDataGridView1).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(19, 46);
            label1.Name = "label1";
            label1.Size = new Size(109, 20);
            label1.TabIndex = 0;
            label1.Text = "PROJE KODU :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(15, 85);
            label2.Name = "label2";
            label2.Size = new Size(113, 20);
            label2.TabIndex = 1;
            label2.Text = "PARÇA KODU :";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            textBox1.Location = new Point(126, 43);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(162, 27);
            textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            textBox2.Location = new Point(126, 82);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(162, 27);
            textBox2.TabIndex = 3;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button1.Location = new Point(312, 43);
            button1.Name = "button1";
            button1.Size = new Size(94, 71);
            button1.TabIndex = 4;
            button1.Text = "GETİR";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // advancedDataGridView1
            // 
            advancedDataGridView1.AllowUserToAddRows = false;
            advancedDataGridView1.AllowUserToDeleteRows = false;
            advancedDataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            advancedDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            advancedDataGridView1.BackgroundColor = Color.White;
            advancedDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            advancedDataGridView1.FilterAndSortEnabled = true;
            advancedDataGridView1.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            advancedDataGridView1.Location = new Point(28, 133);
            advancedDataGridView1.MaxFilterButtonImageHeight = 23;
            advancedDataGridView1.Name = "advancedDataGridView1";
            advancedDataGridView1.RightToLeft = RightToLeft.No;
            advancedDataGridView1.RowHeadersWidth = 51;
            advancedDataGridView1.Size = new Size(1746, 845);
            advancedDataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            advancedDataGridView1.TabIndex = 5;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button2.Location = new Point(465, 43);
            button2.Name = "button2";
            button2.Size = new Size(94, 71);
            button2.TabIndex = 6;
            button2.Text = "KAYIT ET";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = Color.DimGray;
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1902, 27);
            toolStrip1.TabIndex = 9;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(29, 24);
            toolStripButton1.Text = "toolStripButton1";
            toolStripButton1.Click += toolStripButton1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gray;
            ClientSize = new Size(1902, 1033);
            Controls.Add(toolStrip1);
            Controls.Add(button2);
            Controls.Add(advancedDataGridView1);
            Controls.Add(button1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MODUL BUL";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)advancedDataGridView1).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button1;
        private Zuby.ADGV.AdvancedDataGridView advancedDataGridView1;
        private Button button2;
        private FontDialog fontDialog1;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton1;
    }
}
