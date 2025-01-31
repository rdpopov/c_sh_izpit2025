// Фн 112490
// Росен Димитров Попов
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace PictureBoxBalls
{
    public class Ball
    {
        public float x;
        public float y;
        public float d;
        public float dx;
        public float dy;
        public Brush b;

        public Ball(float _x , float _y, float _d, float _dx , float _dy, Brush _b) { 
            x = _x;
            y = _y;
            d = _d;
            dx = _dx;
            dy = _dy;
            b = _b; ;
        } 

        public void moveBall(float maxX, float maxY,Random r)
        {
            x += dx;
            if ((x > maxX - 2*d) || x <= d)
            {
                dx = - dx;
            }
            y += dy;
            if ((y > maxY - 2*d) || y <= d)
            {
                dy = - dy;
            }
        }
        public void  drawBall(Graphics g)
        {
            g.FillEllipse(b, x, y, d, d);
        }

        public bool ballCollision(List<Ball> lb) 
        {
            foreach (Ball b in lb) {
                if (b != this)
                {
                    float dst = (b.x - this.x) * (b.x - this.x) + (b.y - this.y) * (b.y - this.y);
                    float rs = b.d + this.d;

                    if (dst <= rs * rs) {
                        b.dy = -b.dy;
                        this.dx = -this.dx;
                        return true;

                    }
                }
            }
            return false;
        }
    }

    public class MainForm : Form
    {
        private PictureBox pictureBox;
        private Button btnLoadImage;
        private Button btnClearImage;
        private Button btnExit;
        private Timer timer;
        private TrackBar tackBalls;
        private TrackBar tackSize;
        private Random random = new Random();
        private int hasCollission = 0;

        private List<Ball> balls = new List<Ball> {
           new Ball(40, 40, 10, 10, 10, new SolidBrush(Color.Red))
        };
        public void timer_tick(object sender, EventArgs e)
        {
            foreach (Ball b in balls) {
                b.moveBall(pictureBox.Width, pictureBox.Height, random);
                if (b.ballCollision(balls)) {
                    hasCollission = 10;
                }
            }
            
            pictureBox.Invalidate();
        }
        public void trackBalls_Count_event(object sender, System.EventArgs e) {
            int set_count_to = tackBalls.Value;
            if (set_count_to < balls.Count)
            {
                while (balls.Count !=  set_count_to) {
                    balls.RemoveAt(balls.Count - 1);
                }
            }
            else
            {
                float sz = balls[0].d;
                for (int i = balls.Count; i != set_count_to; i++)
                {
                    int x = random.Next(200,400);
                    int y = random.Next(200,400);
                    int dx = random.Next(10,15);
                    int dy = random.Next(10,15);
                    balls.Add(new Ball(x, y, sz, dx, dy, Brushes.BlueViolet));
                }
            }
            pictureBox.Invalidate();
        }

        public void trackBalls_Size_event(object sender, System.EventArgs e) {
            int set_size_to = tackSize.Value;
            foreach (Ball b in balls)
            {
                b.d = set_size_to;
            }
            pictureBox.Invalidate();
        }

        public void picture_paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            gr.SmoothingMode = SmoothingMode.HighQuality;
            if (hasCollission > 0) {
                hasCollission --;
                string text = "kaboom!";
                Font font = new Font("Arial", 16, FontStyle.Bold);
                Brush brush = Brushes.DarkBlue;

                // Draw text in the center
                SizeF textSize = gr.MeasureString(text, font);
                PointF textLocation = new PointF(
                    (pictureBox.Width - textSize.Width) / 2,
                    (pictureBox.Height - textSize.Height) / 2
                );

                gr.DrawString(text, font, brush, textLocation);
            }
            foreach (Ball b in balls)
            {
                b.drawBall(gr);
            }
        }

        public MainForm()
        {
            // Initialize Form settings
            this.Text = "Balls";
            this.Size = new Size(800, 650);

            // Initialize PictureBox
            pictureBox = new PictureBox
            {
                Size = new Size(680, 450),
                Location = new Point(50, 20),
                BackColor = System.Drawing.Color.Ivory,
                BorderStyle = BorderStyle.Fixed3D,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            pictureBox.Paint += picture_paint;

            timer = new Timer();
            timer.Interval = 40;
            timer.Tick += timer_tick;

            // Initialize Buttons
            btnLoadImage = new Button
            {
                Text = "Start",
                Location = new Point(200, 500)
            };
            btnLoadImage.Click += BtnStart_Click;

            btnClearImage = new Button
            {
                Text = "Stop",
                Location = new Point(400, 500)
            };
            btnClearImage.Click += BtnStop_Click;

            btnExit = new Button
            {
                Text = "Exit",
                Location = new Point(600, 500)
            };
            btnExit.Click += BtnExit_Click;

            tackBalls = new TrackBar
            {
                Location = new Point(50, 550),
                Size = new System.Drawing.Size(200, 45),
                Maximum = 10,
                Minimum = 1,
                TickFrequency = 1,
                Value = 1,
            };
            tackBalls.Scroll += trackBalls_Count_event;

            tackSize = new TrackBar
            {
                Location = new Point(400,550),
                Size = new System.Drawing.Size(200, 45),
                Maximum = 35,
                Minimum = 10,
                TickFrequency = 2,
                LargeChange = 4,
                SmallChange = 3,
                Value = 10,
            };
            tackSize.Scroll += trackBalls_Size_event;

            // Add Controls to Form
            this.Controls.Add(pictureBox);
            this.Controls.Add(btnLoadImage);
            this.Controls.Add(btnClearImage);
            this.Controls.Add(btnExit);
            this.Controls.Add(tackBalls);
            this.Controls.Add(tackSize);

            this.DoubleBuffered = true;
            this.Load += new System.EventHandler(this.MainForm_Load);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
