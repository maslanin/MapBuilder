using System;
using System.Windows.Forms;

namespace MapBuilder
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            mob1.Text = "";
            mob2.Text = "";
            lut1.Text = "";
            lut2.Text = "";
            lut3.Text = "";
            lut4.Text = "";
            lut5.Text = "";
            lut6.Text = "";
            lut7.Text = "";
            lut8.Text = "";
            lut9.Text = "";
        }

        public void AddSettings(string[] sets)
        {
            mob1.Text = sets[0];
            mob2.Text = sets[1];
            lut1.Text = sets[2];
            lut2.Text = sets[3];
            lut3.Text = sets[4];
            lut4.Text = sets[5];
            lut5.Text = sets[6];
            lut6.Text = sets[7];
            lut7.Text = sets[8];
            lut8.Text = sets[9];
            lut9.Text = sets[10];
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (mob1.Text == "" && mob2.Text == "")
            {
                MessageBox.Show("You need to enter at least one mob!");
                return;
            }
            if (mob1.Text == "" && mob2.Text != "")
            {
                // если ввели второго моба только
                mob1.Text = mob2.Text;
                mob2.Text = "";
            }
            int search = 1;
            if (mob1.Text != "" && mob2.Text != "")
            {
                search = 2;
            }
            string sett = "vini2vzgljada=" + search.ToString() + Environment.NewLine;
            sett += "vini1monstr=" + mob1.Text + Environment.NewLine;
            sett += "vini2monstr=" + mob2.Text + Environment.NewLine;
            sett += "vinives41=" + lut1.Text + Environment.NewLine;
            sett += "vinives42=" + lut2.Text + Environment.NewLine;
            sett += "vinives43=" + lut3.Text + Environment.NewLine;
            sett += "vinives44=" + lut4.Text + Environment.NewLine;
            sett += "vinives45=" + lut5.Text + Environment.NewLine;
            sett += "vinives46=" + lut6.Text + Environment.NewLine;
            sett += "vinives47=" + lut7.Text + Environment.NewLine;
            sett += "vinives48=" + lut8.Text + Environment.NewLine;
            sett += "vinives49=" + lut9.Text;
            MainForm.settings = sett;
            MessageBox.Show("Settings saved successfully!");
            this.Close();
        }
    }
}
