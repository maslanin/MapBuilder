namespace MapBuilder
{
    partial class FormTeleport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTeleport));
            this.storona = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.kuda = new System.Windows.Forms.TextBox();
            this.otkuda = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tele = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // storona
            // 
            this.storona.FormattingEnabled = true;
            this.storona.Items.AddRange(new object[] {
            "UP",
            "DOWN",
            "LEFT",
            "RIGHT"});
            this.storona.Location = new System.Drawing.Point(12, 103);
            this.storona.Name = "storona";
            this.storona.Size = new System.Drawing.Size(127, 21);
            this.storona.TabIndex = 5;
            this.storona.Text = "UP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Сторона шага:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Куда:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Откуда:";
            // 
            // kuda
            // 
            this.kuda.Location = new System.Drawing.Point(12, 64);
            this.kuda.Name = "kuda";
            this.kuda.ReadOnly = true;
            this.kuda.Size = new System.Drawing.Size(127, 20);
            this.kuda.TabIndex = 7;
            // 
            // otkuda
            // 
            this.otkuda.Location = new System.Drawing.Point(12, 25);
            this.otkuda.Name = "otkuda";
            this.otkuda.ReadOnly = true;
            this.otkuda.Size = new System.Drawing.Size(127, 20);
            this.otkuda.TabIndex = 6;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 170);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(124, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Скрывать в поиске";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Добавить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // tele
            // 
            this.tele.FormattingEnabled = true;
            this.tele.Items.AddRange(new object[] {
            "Doratia_Farm",
            "Freedonia",
            "Lybera_Fishing_Village",
            "Lybera_Hornet_Cave",
            "Vargos_5th_Floor",
            "Vargos_Basement"});
            this.tele.Location = new System.Drawing.Point(12, 143);
            this.tele.Name = "tele";
            this.tele.Size = new System.Drawing.Size(127, 21);
            this.tele.TabIndex = 13;
            this.tele.Text = "Doratia_Farm";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Имя телепорта:";
            // 
            // FormTeleport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(152, 226);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tele);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.storona);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.kuda);
            this.Controls.Add(this.otkuda);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(168, 264);
            this.MinimumSize = new System.Drawing.Size(168, 264);
            this.Name = "FormTeleport";
            this.Text = "Teleport";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox storona;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox kuda;
        private System.Windows.Forms.TextBox otkuda;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox tele;
        private System.Windows.Forms.Label label4;
    }
}