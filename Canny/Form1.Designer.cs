namespace Canny
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
            this.originalPicBox = new System.Windows.Forms.PictureBox();
            this.openPictureDialog = new System.Windows.Forms.OpenFileDialog();
            this.resultPicBox = new System.Windows.Forms.PictureBox();
            this.ChoosePicBtn = new System.Windows.Forms.Button();
            this.MatrixSizeNUD = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SmoothTypeListBox = new System.Windows.Forms.ComboBox();
            this.ExecuteBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.originalPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MatrixSizeNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // originalPicBox
            // 
            this.originalPicBox.Location = new System.Drawing.Point(12, 41);
            this.originalPicBox.Name = "originalPicBox";
            this.originalPicBox.Size = new System.Drawing.Size(385, 384);
            this.originalPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.originalPicBox.TabIndex = 0;
            this.originalPicBox.TabStop = false;
            // 
            // openPictureDialog
            // 
            this.openPictureDialog.FileName = "Picture";
            this.openPictureDialog.Filter = "Pictures (*.bmp)|*.bmp";
            // 
            // resultPicBox
            // 
            this.resultPicBox.Location = new System.Drawing.Point(403, 41);
            this.resultPicBox.Name = "resultPicBox";
            this.resultPicBox.Size = new System.Drawing.Size(385, 384);
            this.resultPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.resultPicBox.TabIndex = 1;
            this.resultPicBox.TabStop = false;
            // 
            // ChoosePicBtn
            // 
            this.ChoosePicBtn.Location = new System.Drawing.Point(12, 12);
            this.ChoosePicBtn.Name = "ChoosePicBtn";
            this.ChoosePicBtn.Size = new System.Drawing.Size(75, 23);
            this.ChoosePicBtn.TabIndex = 2;
            this.ChoosePicBtn.Text = "Choose";
            this.ChoosePicBtn.UseVisualStyleBackColor = true;
            this.ChoosePicBtn.Click += new System.EventHandler(this.ChoosePicBtn_Click);
            // 
            // MatrixSizeNUD
            // 
            this.MatrixSizeNUD.Location = new System.Drawing.Point(182, 12);
            this.MatrixSizeNUD.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.MatrixSizeNUD.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.MatrixSizeNUD.Name = "MatrixSizeNUD";
            this.MatrixSizeNUD.Size = new System.Drawing.Size(36, 23);
            this.MatrixSizeNUD.TabIndex = 3;
            this.MatrixSizeNUD.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Размер ядра -";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Метод сглаживания";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 427);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(164, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // SmoothTypeListBox
            // 
            this.SmoothTypeListBox.FormattingEnabled = true;
            this.SmoothTypeListBox.Location = new System.Drawing.Point(347, 12);
            this.SmoothTypeListBox.Name = "SmoothTypeListBox";
            this.SmoothTypeListBox.Size = new System.Drawing.Size(133, 23);
            this.SmoothTypeListBox.TabIndex = 8;
            // 
            // ExecuteBtn
            // 
            this.ExecuteBtn.Location = new System.Drawing.Point(486, 11);
            this.ExecuteBtn.Name = "ExecuteBtn";
            this.ExecuteBtn.Size = new System.Drawing.Size(75, 23);
            this.ExecuteBtn.TabIndex = 9;
            this.ExecuteBtn.Text = "Execute";
            this.ExecuteBtn.UseVisualStyleBackColor = true;
            this.ExecuteBtn.Click += new System.EventHandler(this.ExecuteBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ExecuteBtn);
            this.Controls.Add(this.SmoothTypeListBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MatrixSizeNUD);
            this.Controls.Add(this.ChoosePicBtn);
            this.Controls.Add(this.resultPicBox);
            this.Controls.Add(this.originalPicBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.originalPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MatrixSizeNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox originalPicBox;
        private System.Windows.Forms.OpenFileDialog openPictureDialog;
        private System.Windows.Forms.PictureBox resultPicBox;
        private System.Windows.Forms.Button ChoosePicBtn;
        private System.Windows.Forms.NumericUpDown MatrixSizeNUD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox SmoothTypeListBox;
        private System.Windows.Forms.Button ExecuteBtn;
    }
}

