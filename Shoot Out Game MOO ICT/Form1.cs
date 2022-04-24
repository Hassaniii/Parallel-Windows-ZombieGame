using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Shoot_Out_Game_MOO_ICT
{
    public partial class Form1 : Form
    {

        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up";
        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 3;
        List<string> users = new List<string>();
        List<PictureBox> players = new List<PictureBox>();
        List<Point> pos = new List<Point>();
        public int globalport = 11000;
        Random randNum = new Random();
        int score;
        List<PictureBox> zombiesList = new List<PictureBox>();
         string[] ips = "116,121,126,127,99,98,97,102,137,124,103,107,129,105,106".Split(',');



        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                player.Image = Properties.Resources.dead;
                GameTimer.Stop();
            }

            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;

            if (goLeft == true && player.Left > 0)
            {
                player.Left -= speed;
            }
            if (goRight == true && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }
            if (goUp == true && player.Top > 45)
            {
                player.Top -= speed;
            }
            if (goDown == true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }



            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;

                    }
                }


                if (x is PictureBox && (string)x.Tag == "zombie")
                {

                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 1;
                    }


                    //if (x.Left > player.Left)
                    //{
                    //    x.Left -= zombieSpeed;
                    //    ((PictureBox)x).Image = Properties.Resources.zleft;
                    //}
                    //if (x.Left < player.Left)
                    //{
                    //    x.Left += zombieSpeed;
                    //    ((PictureBox)x).Image = Properties.Resources.zright;
                    //}
                    //if (x.Top > player.Top)
                    //{
                    //    x.Top -= zombieSpeed;
                    //    ((PictureBox)x).Image = Properties.Resources.zup;
                    //}
                    //if (x.Top < player.Top)
                    //{
                    //    x.Top += zombieSpeed;
                    //    ((PictureBox)x).Image = Properties.Resources.zdown;
                    //}

                }



                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie" )
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            zombiesList.Remove(((PictureBox)x));

                            for (int i = 0; i < ips.Length; i++)
                            {
                                MakeZombies(Convert.ToInt32(ips[i]), i);
                            }
                            Thread s = new Thread(move);
                            s.Start();
                        }
                    }
                }
                //for (int i = 0; i < ips.Length; i++)
                //{
                //    MakeZombies(Convert.ToInt32(ips[i]), i);
                //}

                //Thread s = new Thread(move);
                //s.Start();

            }


        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {

            if (gameOver == true)
            {
                return;
            }

            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.down;
            }



        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(facing);


                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }

        }

        //private void Form1_Load(object sender, EventArgs e)
        //{

        //    for (int i = 0; i < ips.Length; i++)
        //    {
        //        MakeZombies(Convert.ToInt32(ips[i]), i);
        //    }

        //    Thread s = new Thread(move);
        //    s.Start();
        //}

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);
        }

        /// <summary>
        /// Funtion <c>MakeZombies</c> MakeZombies work as spwan code 
        /// </summary>
        private void MakeZombies(int i, int id)
        {
            //192.168.3.106
            //users.Add("172.16.1." + i);
            users.Add("192.168.3." + i);
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;
            zombie.Left = randNum.Next(0, 900);
            zombie.Top = randNum.Next(0, 800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            zombiesList.Add(zombie);
            this.Controls.Add(zombie);
            player.BringToFront();
            Thread n = new Thread(() => read(id, i));
            n.Start();

        }
        public void read(int id, int ipp)
        {
            bool done = false;
            int listenPort = globalport;
            using (UdpClient listener = new UdpClient(ipp * 10))
            {

                while (!done)
                {



                    IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse(users[id]), ipp * 10);
                    byte[] receivedData = listener.Receive(ref listenEndPoint);
                    string n = Encoding.Unicode.GetString(receivedData);

                    pos[id] = new Point(Convert.ToInt32(n.Split(',')[0]), Convert.ToInt32(n.Split(',')[1]));
                    //should be "Hello World" sent from above client
                }
            }

        }
        private void DropAmmo()
        {

            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = randNum.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = randNum.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";
            this.Controls.Add(ammo);

            ammo.BringToFront();
            player.BringToFront();



        }
        public void set()
        {
            if (this.InvokeRequired == true)
            {
                this.Invoke(new MethodInvoker(set));

            }
            else
            {
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].Left = pos[i].X;
                    players[i].Top = pos[i].Y;
                }
            }
        }
        public void move()
        {
            while (true)
            {
                try
                {
                    set();
                    Thread.Sleep(1);
                }
                catch (Exception e)
                {

                }
            }

        }
        private void RestartGame()
        {
            player.Image = Properties.Resources.up;

            foreach (PictureBox i in zombiesList)
            {
                this.Controls.Remove(i);
            }

            zombiesList.Clear();

            //for (int i = 0; i < 3; i++)
            //{
            //    MakeZombies();
            //}

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            ammo = 10;

            GameTimer.Start();
        }

    }
}
