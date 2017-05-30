using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectionManagement;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Contacts : Form
    {
        private String username;
        private Image picture;
        private String pathToSend;
        private static System.Timers.Timer aTimer;
        private NotifyIcon myNotifyIcon = new NotifyIcon();


        public delegate void updatePathDelegate(String path); // delegate type 
        public updatePathDelegate updatePath; // delegate object

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void menuOpen_Click(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void menuChangeIncognito_Click(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void menuSettings_Click(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void myNotifyIcon_Click(object sender, System.EventArgs e)
        {
            System.Drawing.Size windowSize =
                SystemInformation.PrimaryMonitorMaximizedWindowSize;
            System.Drawing.Point menuPoint =
                new System.Drawing.Point(windowSize.Width - 180,
                windowSize.Height - 5);
            menuPoint = this.PointToClient(menuPoint);

            myNotifyIcon.ContextMenu.Show(this, menuPoint);
        }

        private void Contacts_Resize(object sender, EventArgs e)
        {
            myNotifyIcon.BalloonTipTitle = "File sharing on LAN";
            myNotifyIcon.BalloonTipText = "Sto continuando in background...";

            if (FormWindowState.Minimized == this.WindowState)
            {
                myNotifyIcon.Visible = true;
                myNotifyIcon.ShowBalloonTip(500);
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                myNotifyIcon.Visible = false;
            }
        }

        void updatePath1(string str) {
            label1.Text = str;
            pathToSend = str;
        }

        public Contacts()
        {
            InitializeComponent();
            progressBar1.Visible = false;
            groupBox1.Visible = false;
            updatePath = new updatePathDelegate(updatePath1);

            myNotifyIcon.Icon = new Icon(this.Icon, 40, 40);
            MenuItem[] menuList = new MenuItem[]{new MenuItem("Apri"),
                new MenuItem("Vai in incognito"), new MenuItem("Impostazioni")};
            menuList[0].Click += new System.EventHandler(this.menuOpen_Click);
            menuList[1].Click += new System.EventHandler(this.menuChangeIncognito_Click);
            menuList[2].Click += new System.EventHandler(this.menuSettings_Click);
            ContextMenu clickMenu = new ContextMenu(menuList);
            myNotifyIcon.ContextMenu = clickMenu;
            myNotifyIcon.Click += new System.EventHandler(myNotifyIcon_Click);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            aTimer.Stop();
            groupBox1.Visible = true;
            progressBar1.Visible = true;
            this.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Marquee;
            await Task.Run(() => Program.CM.stopIOServ());
            groupBox1.Visible = false;
            progressBar1.Visible = true;
            Program.CM = null;
            this.Enabled = true;
            Login form = (Login)Application.OpenForms["Login"];
            form.Show();
        }

        public void prepareToShow(String username, Image picture)
        {
            this.username = username;
            this.picture = picture;
        }

        private void Contacts_Load(object sender, EventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox2.Width - 3, pictureBox2.Height - 3);
            Region rg = new Region(gp);
            label2.Text = username;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = picture;
            pictureBox2.Region = rg;

            aTimer = new System.Timers.Timer(5000);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 3000;
            aTimer.Enabled = true;
            aTimer.SynchronizingObject = this;
            

            //Thread.Sleep(10000);

            ConnectionManagement.Host[] hosts = Program.CM.getConnectedHosts();
            String[] hostnames = new String[10];
            ImageList hostPics = new ImageList();


            for (int i = 0; i < hosts.Length; i++)
            {
                Image img;
                if (i == 0)
                {
                    img = CropToCircle(Properties.Resources.incognito_mode, Color.GhostWhite);
                } else
                {
                    Image raw = Image.FromFile(hosts[i].getImagePath());
                    img = CropToCircle(raw, Color.GhostWhite);
                }
                
                hostPics.Images.Add(img);
                hostnames[i] = hosts[i].getName();
                //hostnames[i] = "prova";
            }

            hostPics.ImageSize = new Size(128, 128);
            listView1.LargeImageList = hostPics;
            listView1.CheckBoxes = true;

            for (int i = 0; i < hosts.Length; i++)
            {
                listView1.Items.Add(new ListViewItem { ImageIndex = i, Text = hostnames[i] });
            }


        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("ciao");
        }

        public static Image CropToCircle(Image srcImage, Color backGround)
        {
            Image dstImage = new Bitmap(srcImage.Width, srcImage.Height, srcImage.PixelFormat);
            Graphics g = Graphics.FromImage(dstImage);
            using (Brush br = new SolidBrush(backGround))
            {
                g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
            }
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, dstImage.Width, dstImage.Height);
            g.SetClip(path);
            //g.DrawImage(srcImage, 0, 0);
            g.DrawImage(srcImage, new Rectangle(0, 0, srcImage.Width, srcImage.Height));

            return dstImage;
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("STO AGGIORNANDO");
            ConnectionManagement.Host[] hosts = Program.CM.getConnectedHosts();
            String[] hostnames = new String[10];
            ImageList hostPics = new ImageList();

            for (int i = 0; i < hosts.Length; i++)
            {
                Console.WriteLine(i);
                Console.WriteLine(hosts[i].getImagePath());
                Image img;
                if (i == 0)
                {
                    img = CropToCircle(Properties.Resources.incognito_mode, Color.GhostWhite);
                }
                else
                {
                    Image raw;
                    try
                    {
                        raw = Image.FromFile(hosts[i].getImagePath());
                    } catch
                    {
                        raw = Properties.Resources.incognito_mode;
                    }
                    img = CropToCircle(raw, Color.GhostWhite);
                }

                hostPics.Images.Add(img);
                hostnames[i] = hosts[i].getName();
                //hostnames[i] = "prova";
            }

            hostPics.ImageSize = new Size(128, 128);
            listView1.LargeImageList = hostPics;
            listView1.CheckBoxes = true;
            listView1.Items.Clear();

            for (int i = 0; i < hosts.Length; i++)
            {
                listView1.Items.Add(new ListViewItem { ImageIndex = i, Text = hostnames[i] });
            }
        } 

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
