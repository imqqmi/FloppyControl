namespace FloppyControlApp
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.PathToRecoveredDisksTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SaveSettingsButton = new System.Windows.Forms.Button();
            this.MicroSteppingUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxBaud = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.MicroSteppingUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // PathToRecoveredDisksTextBox
            // 
            this.PathToRecoveredDisksTextBox.Location = new System.Drawing.Point(24, 139);
            this.PathToRecoveredDisksTextBox.Name = "PathToRecoveredDisksTextBox";
            this.PathToRecoveredDisksTextBox.Size = new System.Drawing.Size(356, 20);
            this.PathToRecoveredDisksTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Path to recovered disks folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(310, 91);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // SaveSettingsButton
            // 
            this.SaveSettingsButton.Location = new System.Drawing.Point(308, 301);
            this.SaveSettingsButton.Name = "SaveSettingsButton";
            this.SaveSettingsButton.Size = new System.Drawing.Size(75, 23);
            this.SaveSettingsButton.TabIndex = 3;
            this.SaveSettingsButton.Text = "Save";
            this.SaveSettingsButton.UseVisualStyleBackColor = true;
            this.SaveSettingsButton.Click += new System.EventHandler(this.SaveSettingsButton_Click);
            // 
            // MicroSteppingUpDown
            // 
            this.MicroSteppingUpDown.Location = new System.Drawing.Point(152, 233);
            this.MicroSteppingUpDown.Name = "MicroSteppingUpDown";
            this.MicroSteppingUpDown.Size = new System.Drawing.Size(46, 20);
            this.MicroSteppingUpDown.TabIndex = 4;
            this.MicroSteppingUpDown.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 235);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "StepStick Microstepping";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(266, 39);
            this.label4.TabIndex = 1;
            this.label4.Text = "If you\'ve replaced the stepper motor driver, you can set\r\nmicrostepping here. 1 i" +
    "s no microstepping, normally\r\nconnected floppydrive.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(128, 264);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Baudrate:";
            // 
            // comboBoxBaud
            // 
            this.comboBoxBaud.FormattingEnabled = true;
            this.comboBoxBaud.Items.AddRange(new object[] {
            "9600",
            "115200",
            "250000",
            "500000",
            "1000000",
            "2000000",
            "4000000"});
            this.comboBoxBaud.Location = new System.Drawing.Point(131, 280);
            this.comboBoxBaud.Name = "comboBoxBaud";
            this.comboBoxBaud.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBaud.TabIndex = 15;
            this.comboBoxBaud.Text = "5000000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 262);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Ports:";
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Location = new System.Drawing.Point(27, 278);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(98, 21);
            this.comboBoxPort.TabIndex = 14;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 336);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBoxBaud);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.comboBoxPort);
            this.Controls.Add(this.MicroSteppingUpDown);
            this.Controls.Add(this.SaveSettingsButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PathToRecoveredDisksTextBox);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.MicroSteppingUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PathToRecoveredDisksTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SaveSettingsButton;
        private System.Windows.Forms.NumericUpDown MicroSteppingUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxBaud;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxPort;
        private System.IO.Ports.SerialPort serialPort1;
    }
}