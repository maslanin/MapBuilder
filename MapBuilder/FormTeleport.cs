using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapBuilder
{
    public partial class FormTeleport : Form
    {
        public FormTeleport()
        {
            InitializeComponent();
        }
        int X = 0;
        int Y = 0;
        private void Button1_Click(object sender, EventArgs e)
        {
            if (this.Owner is MainForm f)
            {

                int hide = (checkBox1.Checked == true) ? 1 : 0;
                if (f.idx != -1)
                {
                    f.waypoints.Insert(f.idx + 1, new Node
                    {
                        Obrabot4ik = 3,
                        PunktNazna4enija = kuda.Text,
                        StoronaWaga = storona.Text,
                        ImjaTeleporta = tele.Text,
                        ZapretPoiska = hide,
                        X = X,
                        Y = Y
                    });
                }
                else
                {
                    f.waypoints.Add(new Node
                    {
                        Obrabot4ik = 3,
                        PunktNazna4enija = kuda.Text,
                        StoronaWaga = storona.Text,
                        ImjaTeleporta = tele.Text,
                        ZapretPoiska = hide,
                        X = X,
                        Y = Y
                    });
                }

                f.Remap(f.waypoints, MainForm.way);
                MessageBox.Show("Teleport successfully added!");
                this.Close();
            }
        }

        public void AddCoords(int X1, int Y1, int X2, int Y2)
        {
            otkuda.Text = (X1 + 1).ToString() + "m" + (Y1 + 1).ToString();
            kuda.Text = (X2 + 1).ToString() + "m" + (Y2 + 1).ToString();
            this.X = X2;
            this.Y = Y2;
        }
    }
}
