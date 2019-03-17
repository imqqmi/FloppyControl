using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace FloppyControlApp
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            getAvailablePorts();

            PathToRecoveredDisksTextBox.Text = Properties.Settings.Default["PathToRecoveredDisks"].ToString();
            MicroSteppingUpDown.Value = (decimal)Properties.Settings.Default["StepStickMicrostepping"];
            comboBoxBaud.Text = Properties.Settings.Default["DefaultBaud"].ToString();
            comboBoxPort.SelectedItem = Properties.Settings.Default["DefaultPort"];

        }

        private void SaveSettingsButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["PathToRecoveredDisks"] = PathToRecoveredDisksTextBox.Text;
            Properties.Settings.Default["StepStickMicrostepping"] = MicroSteppingUpDown.Value;
            Properties.Settings.Default["DefaultPort"] = comboBoxPort.SelectedItem;
            Properties.Settings.Default["DefaultBaud"] = int.Parse( comboBoxBaud.Text );
            Properties.Settings.Default.Save();

            this.Hide();
        }

        private void getAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            comboBoxPort.Items.AddRange(ports);
        }

        private void MicroSteppingUpDown_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
