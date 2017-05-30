using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ConnectionManagement;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Login : Form
    {
        [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username, UInt32 whatever, // 0x80000000
                                                  StringBuilder picpath, int maxLength);

        private Boolean incognito = false;
        private Image usr;
        UdpServerWrapper cm;

        public static string GetUserTilePath(string username)
        {   // username: use null for current user
            var sb = new StringBuilder(1000);
            GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
            return sb.ToString();
        }

        public static Image GetUserTile(string username)
        {
            return Image.FromFile(GetUserTilePath(username));
        }

        public Login()
        {
            InitializeComponent();
        }

        
        private void Form1_Load_1(object sender, EventArgs e)
        {
            usr = GetUserTile(null);
            this.Controls.Add(pictureBox2);
            this.Controls.Add(label2);
            this.Controls.Add(label3);

            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox2.Width - 3, pictureBox2.Height - 3);
            Region rg = new Region(gp);
            pictureBox2.Left = (this.ClientSize.Width - pictureBox2.Width) / 2;
            pictureBox2.Region = rg;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = usr;

            label2.Text = Environment.UserName;
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label2.Left = (this.ClientSize.Width - label2.Width) / 2;
            label3.Left = (this.ClientSize.Width - label3.Width) / 2;
            label4.Left = (this.ClientSize.Width - label4.Width) / 2;
            label5.Left = (this.ClientSize.Width - label5.Width) / 2;
            label6.Left = (this.ClientSize.Width - label6.Width) / 2;
            button2.Left = (this.ClientSize.Width - button2.Width) / 2;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void startCM()
        {
            Program.CM.startIOServ();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.CM = new UdpServerWrapper(incognito);
            Thread newThread = new Thread(startCM);
            newThread.Start();
            Contacts segue = new Contacts();
            this.Hide();
            if(incognito == false)
            {
                segue.prepareToShow(Environment.UserName, usr);
            } else
            {
                segue.prepareToShow("Incognito", Properties.Resources.incognito_mode);
            }
            segue.Show();
            this.Hide();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {
            if(incognito == false)
            {
                pictureBox2.Image = Properties.Resources.incognito_mode;
                label2.Text = "Incognito";
                label5.Text = "Se desideri ricevere file,";
                label6.Text = "rendi visibile il tuo account";
                label2.Left = (this.ClientSize.Width - label2.Width) / 2;
                label5.Left = (this.ClientSize.Width - label5.Width) / 2;
                label6.Left = (this.ClientSize.Width - label6.Width) / 2;
                incognito = true;
            } else
            {
                pictureBox2.Image = usr;
                label2.Text = Environment.UserName;
                label5.Text = "Se non desideri ricevere file,";
                label6.Text = "passa alla Modalità Incognito";
                label2.Left = (this.ClientSize.Width - label2.Width) / 2;
                label5.Left = (this.ClientSize.Width - label5.Width) / 2;
                label6.Left = (this.ClientSize.Width - label6.Width) / 2;
                incognito = false;
            }
            
        }
    }
}
