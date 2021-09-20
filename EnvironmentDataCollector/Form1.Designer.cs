
namespace EnvironmentDataCollector
{
    partial class EnvDataForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.FileBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.FromPiker = new System.Windows.Forms.DateTimePicker();
            this.ToPicker = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ExportBtn = new System.Windows.Forms.Button();
            this.DataGrid = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.SearchBtn = new System.Windows.Forms.Button();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.InputMinTemp = new System.Windows.Forms.NumericUpDown();
            this.InputMaxTemp = new System.Windows.Forms.NumericUpDown();
            this.InputMaxHum = new System.Windows.Forms.NumericUpDown();
            this.InputMinHum = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputMinTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputMaxTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputMaxHum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputMinHum)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Carica Dati";
            // 
            // FileBtn
            // 
            this.FileBtn.Location = new System.Drawing.Point(143, 12);
            this.FileBtn.Name = "FileBtn";
            this.FileBtn.Size = new System.Drawing.Size(216, 46);
            this.FileBtn.TabIndex = 1;
            this.FileBtn.Text = "Upload";
            this.FileBtn.UseVisualStyleBackColor = true;
            this.FileBtn.Click += new System.EventHandler(this.FileBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(12, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 31);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ricerca Dati";
            // 
            // FromPiker
            // 
            this.FromPiker.Location = new System.Drawing.Point(68, 127);
            this.FromPiker.Name = "FromPiker";
            this.FromPiker.Size = new System.Drawing.Size(311, 30);
            this.FromPiker.TabIndex = 3;
            this.FromPiker.ValueChanged += new System.EventHandler(this.CleanFilters);
            // 
            // ToPicker
            // 
            this.ToPicker.Location = new System.Drawing.Point(68, 172);
            this.ToPicker.Name = "ToPicker";
            this.ToPicker.Size = new System.Drawing.Size(311, 30);
            this.ToPicker.TabIndex = 4;
            this.ToPicker.ValueChanged += new System.EventHandler(this.CleanFilters);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(12, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 31);
            this.label3.TabIndex = 5;
            this.label3.Text = "Da";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(12, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 31);
            this.label4.TabIndex = 6;
            this.label4.Text = "A";
            // 
            // ExportBtn
            // 
            this.ExportBtn.Location = new System.Drawing.Point(12, 691);
            this.ExportBtn.Name = "ExportBtn";
            this.ExportBtn.Size = new System.Drawing.Size(216, 46);
            this.ExportBtn.TabIndex = 7;
            this.ExportBtn.Text = "Esporta";
            this.ExportBtn.UseVisualStyleBackColor = true;
            this.ExportBtn.Click += new System.EventHandler(this.ExportBtn_Click);
            // 
            // DataGrid
            // 
            this.DataGrid.AllowUserToAddRows = false;
            this.DataGrid.AllowUserToDeleteRows = false;
            this.DataGrid.AllowUserToOrderColumns = true;
            this.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGrid.Location = new System.Drawing.Point(429, 51);
            this.DataGrid.Name = "DataGrid";
            this.DataGrid.ReadOnly = true;
            this.DataGrid.Size = new System.Drawing.Size(929, 686);
            this.DataGrid.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(429, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 31);
            this.label5.TabIndex = 9;
            this.label5.Text = "Risultati";
            // 
            // SearchBtn
            // 
            this.SearchBtn.Location = new System.Drawing.Point(12, 639);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(216, 46);
            this.SearchBtn.TabIndex = 10;
            this.SearchBtn.Text = "Cerca";
            this.SearchBtn.UseVisualStyleBackColor = true;
            this.SearchBtn.Click += new System.EventHandler(this.SearchBtn_Click);
            // 
            // FileDialog
            // 
            this.FileDialog.Title = "Carica un file";
            // 
            // InputMinTemp
            // 
            this.InputMinTemp.Location = new System.Drawing.Point(60, 252);
            this.InputMinTemp.Name = "InputMinTemp";
            this.InputMinTemp.Size = new System.Drawing.Size(133, 30);
            this.InputMinTemp.TabIndex = 11;
            this.InputMinTemp.TextChanged += new System.EventHandler(this.CleanFilters);
            // 
            // InputMaxTemp
            // 
            this.InputMaxTemp.Location = new System.Drawing.Point(234, 252);
            this.InputMaxTemp.Name = "InputMaxTemp";
            this.InputMaxTemp.Size = new System.Drawing.Size(145, 30);
            this.InputMaxTemp.TabIndex = 12;
            this.InputMaxTemp.TextChanged += new System.EventHandler(this.CleanFilters);
            // 
            // InputMaxHum
            // 
            this.InputMaxHum.Location = new System.Drawing.Point(234, 330);
            this.InputMaxHum.Name = "InputMaxHum";
            this.InputMaxHum.Size = new System.Drawing.Size(145, 30);
            this.InputMaxHum.TabIndex = 14;
            this.InputMaxHum.TextChanged += new System.EventHandler(this.CleanFilters);
            // 
            // InputMinHum
            // 
            this.InputMinHum.Location = new System.Drawing.Point(60, 330);
            this.InputMinHum.Name = "InputMinHum";
            this.InputMinHum.Size = new System.Drawing.Size(133, 30);
            this.InputMinHum.TabIndex = 13;
            this.InputMinHum.TextChanged += new System.EventHandler(this.CleanFilters);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(12, 216);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 31);
            this.label6.TabIndex = 15;
            this.label6.Text = "Temperatura";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(12, 247);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 31);
            this.label7.TabIndex = 16;
            this.label7.Text = "Da";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label8.Location = new System.Drawing.Point(199, 247);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 31);
            this.label8.TabIndex = 17;
            this.label8.Text = "A";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label9.Location = new System.Drawing.Point(12, 330);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 31);
            this.label9.TabIndex = 18;
            this.label9.Text = "Da";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label10.Location = new System.Drawing.Point(199, 330);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 31);
            this.label10.TabIndex = 19;
            this.label10.Text = "A";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(12, 294);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 31);
            this.label11.TabIndex = 20;
            this.label11.Text = "Umidità";
            // 
            // EnvDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 749);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.InputMaxHum);
            this.Controls.Add(this.InputMinHum);
            this.Controls.Add(this.InputMaxTemp);
            this.Controls.Add(this.InputMinTemp);
            this.Controls.Add(this.SearchBtn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DataGrid);
            this.Controls.Add(this.ExportBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ToPicker);
            this.Controls.Add(this.FromPiker);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FileBtn);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.Name = "EnvDataForm";
            this.Text = "Raccolta Dati Ambiente";
            ((System.ComponentModel.ISupportInitialize)(this.DataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputMinTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputMaxTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputMaxHum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputMinHum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button FileBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker FromPiker;
        private System.Windows.Forms.DateTimePicker ToPicker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ExportBtn;
        private System.Windows.Forms.DataGridView DataGrid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button SearchBtn;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.NumericUpDown InputMinTemp;
        private System.Windows.Forms.NumericUpDown InputMaxTemp;
        private System.Windows.Forms.NumericUpDown InputMaxHum;
        private System.Windows.Forms.NumericUpDown InputMinHum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}