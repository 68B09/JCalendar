namespace JCalendar
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radJSON = new System.Windows.Forms.RadioButton();
			this.radNormal = new System.Windows.Forms.RadioButton();
			this.btnMakeList = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.cmbYearTo = new System.Windows.Forms.ComboBox();
			this.txtResult = new System.Windows.Forms.TextBox();
			this.cmbYear = new System.Windows.Forms.ComboBox();
			this.chkZeroPad = new System.Windows.Forms.CheckBox();
			this.chkAddLine = new System.Windows.Forms.CheckBox();
			this.chkComma = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.cmbYearTo);
			this.groupBox1.Controls.Add(this.txtResult);
			this.groupBox1.Controls.Add(this.cmbYear);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(640, 758);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Year";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chkComma);
			this.groupBox2.Controls.Add(this.chkAddLine);
			this.groupBox2.Controls.Add(this.chkZeroPad);
			this.groupBox2.Controls.Add(this.radJSON);
			this.groupBox2.Controls.Add(this.radNormal);
			this.groupBox2.Controls.Add(this.btnMakeList);
			this.groupBox2.Location = new System.Drawing.Point(6, 47);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(628, 33);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			// 
			// radJSON
			// 
			this.radJSON.AutoSize = true;
			this.radJSON.Location = new System.Drawing.Point(175, 11);
			this.radJSON.Name = "radJSON";
			this.radJSON.Size = new System.Drawing.Size(53, 17);
			this.radJSON.TabIndex = 6;
			this.radJSON.Text = "JSON";
			this.radJSON.UseVisualStyleBackColor = true;
			// 
			// radNormal
			// 
			this.radNormal.AutoSize = true;
			this.radNormal.Checked = true;
			this.radNormal.Location = new System.Drawing.Point(116, 11);
			this.radNormal.Name = "radNormal";
			this.radNormal.Size = new System.Drawing.Size(53, 17);
			this.radNormal.TabIndex = 5;
			this.radNormal.TabStop = true;
			this.radNormal.Text = "通常";
			this.radNormal.UseVisualStyleBackColor = true;
			// 
			// btnMakeList
			// 
			this.btnMakeList.Location = new System.Drawing.Point(6, 9);
			this.btnMakeList.Name = "btnMakeList";
			this.btnMakeList.Size = new System.Drawing.Size(93, 21);
			this.btnMakeList.TabIndex = 3;
			this.btnMakeList.Text = "Make List";
			this.btnMakeList.UseVisualStyleBackColor = true;
			this.btnMakeList.Click += new System.EventHandler(this.btnMakeList_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(76, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(21, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "～";
			// 
			// cmbYearTo
			// 
			this.cmbYearTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbYearTo.FormattingEnabled = true;
			this.cmbYearTo.Location = new System.Drawing.Point(103, 19);
			this.cmbYearTo.Name = "cmbYearTo";
			this.cmbYearTo.Size = new System.Drawing.Size(64, 21);
			this.cmbYearTo.TabIndex = 2;
			// 
			// txtResult
			// 
			this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtResult.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.txtResult.Location = new System.Drawing.Point(6, 86);
			this.txtResult.Multiline = true;
			this.txtResult.Name = "txtResult";
			this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtResult.Size = new System.Drawing.Size(628, 666);
			this.txtResult.TabIndex = 4;
			this.txtResult.WordWrap = false;
			// 
			// cmbYear
			// 
			this.cmbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbYear.FormattingEnabled = true;
			this.cmbYear.Location = new System.Drawing.Point(6, 19);
			this.cmbYear.Name = "cmbYear";
			this.cmbYear.Size = new System.Drawing.Size(64, 21);
			this.cmbYear.TabIndex = 0;
			this.cmbYear.SelectedIndexChanged += new System.EventHandler(this.cmbYear_SelectedIndexChanged);
			// 
			// chkZeroPad
			// 
			this.chkZeroPad.AutoSize = true;
			this.chkZeroPad.Checked = true;
			this.chkZeroPad.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkZeroPad.Location = new System.Drawing.Point(246, 12);
			this.chkZeroPad.Name = "chkZeroPad";
			this.chkZeroPad.Size = new System.Drawing.Size(82, 17);
			this.chkZeroPad.TabIndex = 7;
			this.chkZeroPad.Text = "/1/=/01/";
			this.chkZeroPad.UseVisualStyleBackColor = true;
			// 
			// chkAddLine
			// 
			this.chkAddLine.AutoSize = true;
			this.chkAddLine.Checked = true;
			this.chkAddLine.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAddLine.Location = new System.Drawing.Point(334, 11);
			this.chkAddLine.Name = "chkAddLine";
			this.chkAddLine.Size = new System.Drawing.Size(82, 17);
			this.chkAddLine.TabIndex = 8;
			this.chkAddLine.Text = "空行追加";
			this.chkAddLine.UseVisualStyleBackColor = true;
			// 
			// chkComma
			// 
			this.chkComma.AutoSize = true;
			this.chkComma.Checked = true;
			this.chkComma.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkComma.Location = new System.Drawing.Point(422, 10);
			this.chkComma.Name = "chkComma";
			this.chkComma.Size = new System.Drawing.Size(75, 17);
			this.chkComma.TabIndex = 9;
			this.chkComma.Text = ",区切り";
			this.chkComma.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(664, 782);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox cmbYear;
		private System.Windows.Forms.TextBox txtResult;
		private System.Windows.Forms.Button btnMakeList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cmbYearTo;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radJSON;
		private System.Windows.Forms.RadioButton radNormal;
		private System.Windows.Forms.CheckBox chkZeroPad;
		private System.Windows.Forms.CheckBox chkAddLine;
		private System.Windows.Forms.CheckBox chkComma;
	}
}

