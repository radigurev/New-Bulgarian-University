namespace Calculator
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.tlpDivision = new System.Windows.Forms.TableLayoutPanel();
            this.pEquation = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.bBackspace = new System.Windows.Forms.Button();
            this.bPlus = new System.Windows.Forms.Button();
            this.bMinus = new System.Windows.Forms.Button();
            this.bDivide = new System.Windows.Forms.Button();
            this.bMultiply = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.bEnter = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.bOne = new System.Windows.Forms.Button();
            this.bTwo = new System.Windows.Forms.Button();
            this.bThree = new System.Windows.Forms.Button();
            this.bFour = new System.Windows.Forms.Button();
            this.bFive = new System.Windows.Forms.Button();
            this.bSix = new System.Windows.Forms.Button();
            this.bSeven = new System.Windows.Forms.Button();
            this.bEight = new System.Windows.Forms.Button();
            this.bNine = new System.Windows.Forms.Button();
            this.bZero = new System.Windows.Forms.Button();
            this.bComma = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.bLeftBracket = new System.Windows.Forms.Button();
            this.bRightBracket = new System.Windows.Forms.Button();
            this.tlpDivision.SuspendLayout();
            this.pEquation.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpDivision
            // 
            this.tlpDivision.ColumnCount = 1;
            this.tlpDivision.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDivision.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpDivision.Controls.Add(this.pEquation, 0, 0);
            this.tlpDivision.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tlpDivision.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDivision.Location = new System.Drawing.Point(0, 0);
            this.tlpDivision.Name = "tlpDivision";
            this.tlpDivision.RowCount = 2;
            this.tlpDivision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
            this.tlpDivision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
            this.tlpDivision.Size = new System.Drawing.Size(525, 633);
            this.tlpDivision.TabIndex = 0;
            // 
            // pEquation
            // 
            this.pEquation.Controls.Add(this.textBox1);
            this.pEquation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pEquation.Location = new System.Drawing.Point(3, 3);
            this.pEquation.Name = "pEquation";
            this.pEquation.Size = new System.Drawing.Size(519, 174);
            this.pEquation.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.07692F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 183);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(519, 447);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Controls.Add(this.bBackspace, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.bPlus, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.bMinus, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.bDivide, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.bMultiply, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(513, 121);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // bBackspace
            // 
            this.bBackspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bBackspace.Location = new System.Drawing.Point(411, 3);
            this.bBackspace.Name = "bBackspace";
            this.bBackspace.Size = new System.Drawing.Size(99, 115);
            this.bBackspace.TabIndex = 0;
            this.bBackspace.Text = "<-";
            this.bBackspace.UseVisualStyleBackColor = true;
            this.bBackspace.Click += new System.EventHandler(this.bBackspace_Click);
            // 
            // bPlus
            // 
            this.bPlus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bPlus.Location = new System.Drawing.Point(309, 3);
            this.bPlus.Name = "bPlus";
            this.bPlus.Size = new System.Drawing.Size(96, 115);
            this.bPlus.TabIndex = 1;
            this.bPlus.Text = "+";
            this.bPlus.UseVisualStyleBackColor = true;
            // 
            // bMinus
            // 
            this.bMinus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bMinus.Location = new System.Drawing.Point(207, 3);
            this.bMinus.Name = "bMinus";
            this.bMinus.Size = new System.Drawing.Size(96, 115);
            this.bMinus.TabIndex = 2;
            this.bMinus.Text = "-";
            this.bMinus.UseVisualStyleBackColor = true;
            // 
            // bDivide
            // 
            this.bDivide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bDivide.Location = new System.Drawing.Point(105, 3);
            this.bDivide.Name = "bDivide";
            this.bDivide.Size = new System.Drawing.Size(96, 115);
            this.bDivide.TabIndex = 3;
            this.bDivide.Text = "/";
            this.bDivide.UseVisualStyleBackColor = true;
            // 
            // bMultiply
            // 
            this.bMultiply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bMultiply.Location = new System.Drawing.Point(3, 3);
            this.bMultiply.Name = "bMultiply";
            this.bMultiply.Size = new System.Drawing.Size(96, 115);
            this.bMultiply.TabIndex = 4;
            this.bMultiply.Text = "*";
            this.bMultiply.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.bEnter, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 130);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(513, 314);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // bEnter
            // 
            this.bEnter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bEnter.Location = new System.Drawing.Point(387, 3);
            this.bEnter.Name = "bEnter";
            this.bEnter.Size = new System.Drawing.Size(123, 308);
            this.bEnter.TabIndex = 1;
            this.bEnter.Text = "Enter";
            this.bEnter.UseVisualStyleBackColor = true;
            this.bEnter.Click += new System.EventHandler(this.bEnter_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Controls.Add(this.bOne, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.bTwo, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.bThree, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.bFour, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.bFive, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.bSix, 2, 1);
            this.tableLayoutPanel4.Controls.Add(this.bSeven, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.bEight, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.bNine, 2, 2);
            this.tableLayoutPanel4.Controls.Add(this.bZero, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.bComma, 2, 3);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 3);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(378, 308);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // bOne
            // 
            this.bOne.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bOne.Location = new System.Drawing.Point(3, 3);
            this.bOne.Name = "bOne";
            this.bOne.Size = new System.Drawing.Size(120, 71);
            this.bOne.TabIndex = 0;
            this.bOne.Text = "1";
            this.bOne.UseVisualStyleBackColor = true;
            // 
            // bTwo
            // 
            this.bTwo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bTwo.Location = new System.Drawing.Point(129, 3);
            this.bTwo.Name = "bTwo";
            this.bTwo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.bTwo.Size = new System.Drawing.Size(120, 71);
            this.bTwo.TabIndex = 1;
            this.bTwo.Text = "2";
            this.bTwo.UseVisualStyleBackColor = true;
            // 
            // bThree
            // 
            this.bThree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bThree.Location = new System.Drawing.Point(255, 3);
            this.bThree.Name = "bThree";
            this.bThree.Size = new System.Drawing.Size(120, 71);
            this.bThree.TabIndex = 2;
            this.bThree.Text = "3";
            this.bThree.UseVisualStyleBackColor = true;
            // 
            // bFour
            // 
            this.bFour.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bFour.Location = new System.Drawing.Point(3, 80);
            this.bFour.Name = "bFour";
            this.bFour.Size = new System.Drawing.Size(120, 71);
            this.bFour.TabIndex = 3;
            this.bFour.Text = "4";
            this.bFour.UseVisualStyleBackColor = true;
            // 
            // bFive
            // 
            this.bFive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bFive.Location = new System.Drawing.Point(129, 80);
            this.bFive.Name = "bFive";
            this.bFive.Size = new System.Drawing.Size(120, 71);
            this.bFive.TabIndex = 4;
            this.bFive.Text = "5";
            this.bFive.UseVisualStyleBackColor = true;
            // 
            // bSix
            // 
            this.bSix.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bSix.Location = new System.Drawing.Point(255, 80);
            this.bSix.Name = "bSix";
            this.bSix.Size = new System.Drawing.Size(120, 71);
            this.bSix.TabIndex = 5;
            this.bSix.Text = "6";
            this.bSix.UseVisualStyleBackColor = true;
            // 
            // bSeven
            // 
            this.bSeven.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bSeven.Location = new System.Drawing.Point(3, 157);
            this.bSeven.Name = "bSeven";
            this.bSeven.Size = new System.Drawing.Size(120, 71);
            this.bSeven.TabIndex = 6;
            this.bSeven.Text = "7";
            this.bSeven.UseVisualStyleBackColor = true;
            // 
            // bEight
            // 
            this.bEight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bEight.Location = new System.Drawing.Point(129, 157);
            this.bEight.Name = "bEight";
            this.bEight.Size = new System.Drawing.Size(120, 71);
            this.bEight.TabIndex = 7;
            this.bEight.Text = "8";
            this.bEight.UseVisualStyleBackColor = true;
            // 
            // bNine
            // 
            this.bNine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bNine.Location = new System.Drawing.Point(255, 157);
            this.bNine.Name = "bNine";
            this.bNine.Size = new System.Drawing.Size(120, 71);
            this.bNine.TabIndex = 8;
            this.bNine.Text = "9";
            this.bNine.UseVisualStyleBackColor = true;
            // 
            // bZero
            // 
            this.bZero.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bZero.Location = new System.Drawing.Point(129, 234);
            this.bZero.Name = "bZero";
            this.bZero.Size = new System.Drawing.Size(120, 71);
            this.bZero.TabIndex = 9;
            this.bZero.Text = "0";
            this.bZero.UseVisualStyleBackColor = true;
            // 
            // bComma
            // 
            this.bComma.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bComma.Location = new System.Drawing.Point(255, 234);
            this.bComma.Name = "bComma";
            this.bComma.Size = new System.Drawing.Size(120, 71);
            this.bComma.TabIndex = 10;
            this.bComma.Text = ",";
            this.bComma.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(519, 174);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.bRightBracket, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.bLeftBracket, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 234);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(120, 71);
            this.tableLayoutPanel5.TabIndex = 11;
            // 
            // bLeftBracket
            // 
            this.bLeftBracket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLeftBracket.Location = new System.Drawing.Point(3, 3);
            this.bLeftBracket.Name = "bLeftBracket";
            this.bLeftBracket.Size = new System.Drawing.Size(54, 65);
            this.bLeftBracket.TabIndex = 0;
            this.bLeftBracket.Text = "(";
            this.bLeftBracket.UseVisualStyleBackColor = true;
            // 
            // bRightBracket
            // 
            this.bRightBracket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bRightBracket.Location = new System.Drawing.Point(63, 3);
            this.bRightBracket.Name = "bRightBracket";
            this.bRightBracket.Size = new System.Drawing.Size(54, 65);
            this.bRightBracket.TabIndex = 1;
            this.bRightBracket.Text = ")";
            this.bRightBracket.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 633);
            this.Controls.Add(this.tlpDivision);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tlpDivision.ResumeLayout(false);
            this.pEquation.ResumeLayout(false);
            this.pEquation.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpDivision;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel pEquation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button bEnter;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button bBackspace;
        private System.Windows.Forms.Button bPlus;
        private System.Windows.Forms.Button bMinus;
        private System.Windows.Forms.Button bDivide;
        private System.Windows.Forms.Button bMultiply;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button bOne;
        private System.Windows.Forms.Button bTwo;
        private System.Windows.Forms.Button bThree;
        private System.Windows.Forms.Button bFour;
        private System.Windows.Forms.Button bFive;
        private System.Windows.Forms.Button bSix;
        private System.Windows.Forms.Button bSeven;
        private System.Windows.Forms.Button bEight;
        private System.Windows.Forms.Button bNine;
        private System.Windows.Forms.Button bZero;
        private System.Windows.Forms.Button bComma;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Button bLeftBracket;
        private System.Windows.Forms.Button bRightBracket;
    }
}

