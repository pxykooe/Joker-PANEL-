using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyAuth;

namespace Joker_PANEL___
{
    public partial class stt : Form
    {
        public static api keyAuthApp = new api(
   name: "Pxykoyt's Application",
   ownerid: "oVtdj6ggqc",
   secret: "1cc5cbe20a9224e6ef32b32740a26fa5d4727341de4f3352a8bbe542b066533b",
   version: "1.0");

        private const int ParticleCount = 100;
        private const int DrawCount = 100;
        private readonly Random _random = new Random();
        private readonly PointF[] _particlePositions = new PointF[ParticleCount];
        private readonly PointF[] _particleTargetPositions = new PointF[ParticleCount];
        private readonly float[] _particleSpeeds = new float[ParticleCount];
        private readonly float[] _particleSizes = new float[ParticleCount];
        private readonly float[] _particleRadii = new float[ParticleCount];
        private readonly float[] _particleRotations = new float[ParticleCount];
        private readonly PointF[] _vertices = new PointF[3];

        private Color _particleColor = Color.Green;

        private Color _glowColor = Color.Green;
        private bool _particlesEnabled = true;

        public static api KeyAuthApp { get => keyAuthApp; set => keyAuthApp = value; }

        public stt()
        {
            InitializeComponent();
            KeyAuthApp.init();
            DoubleBuffered = true;

            Timer timer = new Timer
            {
                Interval = 3 // Roughly 60 FPS
            };
            timer.Tick += (sender, args) =>
            {
                UpdateParticles();
                Invalidate();
            };
            timer.Start();
        }

        // Initialize Particles
        private void InitializeParticles()
        {
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            for (int i = 0; i < ParticleCount; i++)
            {
                _particlePositions[i] = new PointF(0, 0);
                _particleTargetPositions[i] = new PointF(_random.Next(screenSize.Width), screenSize.Height * 2);
                _particleSpeeds[i] = 1 + _random.Next(25);
                _particleSizes[i] = _random.Next(8);
                _particleRadii[i] = _random.Next(4);
                _particleRotations[i] = 0;
            }
        }

        // Update Particles
        private void UpdateParticles()
        {
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            for (int i = 0; i < ParticleCount; i++)
            {
                if (_particlePositions[i].X == 0 || _particlePositions[i].Y == 0)
                {
                    _particlePositions[i] = new PointF(_random.Next(screenSize.Width + 1), 15f);
                    _particleSpeeds[i] = 1 + _random.Next(25);
                    _particleRadii[i] = _random.Next(4);
                    _particleSizes[i] = _random.Next(8);
                    _particleTargetPositions[i] = new PointF(_random.Next(screenSize.Width), screenSize.Height * 2);
                }

                float deltaTime = 2.5f / 60;
                _particlePositions[i] = Lerp(_particlePositions[i], _particleTargetPositions[i], deltaTime * (_particleSpeeds[i] / 60));
                _particleRotations[i] += deltaTime;

                if (_particlePositions[i].Y > screenSize.Height)
                {
                    _particlePositions[i] = new PointF(0, 0);
                    _particleRotations[i] = 0;
                }
            }
        }

        // Linear interpolation (Lerp)
        private PointF Lerp(PointF start, PointF end, float t)
        {
            return new PointF(start.X + (end.X - start.X) * t, start.Y + (end.Y - start.Y) * t);
        }

        // Handle drawing the particles and glow effect
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Add this check to skip drawing
            if (!_particlesEnabled)
                return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (int i = 0; i < DrawCount; i++)
            {
                DrawTriangleWithGlow(e.Graphics, _particlePositions[i], _particleSizes[i], _particleRotations[i]);
            }
        }


        // Drawing the triangle with glow effect
        private void DrawTriangleWithGlow(Graphics graphics, PointF position, float size, float rotation)
        {
            float angle = (float)(Math.PI * 2 / 3);
            PointF[] vertices = new PointF[3];

            for (int i = 0; i < 3; i++)
            {
                vertices[i] = new PointF(
                    position.X + size * (float)Math.Cos(rotation + i * angle),
                    position.Y + size * (float)Math.Sin(rotation + i * angle)
                );
            }

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw glow effect
            int maxGlowLayers = 10;
            for (int j = 0; j < maxGlowLayers; j++)
            {
                int alpha = 25 - 2 * j;
                using (Brush glowBrush = new SolidBrush(Color.FromArgb(alpha, _glowColor))) // Using glow color
                {
                    float glowSize = size + j * 4;
                    graphics.FillEllipse(glowBrush, position.X - glowSize / 2, position.Y - glowSize / 2, glowSize, glowSize);
                }
            }

            // Draw triangle
            using (Brush brush = new SolidBrush(_particleColor)) // Using particle color
            {
                graphics.FillPolygon(brush, vertices);
            }
        }
       

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            KeyAuthApp.login(username.Text, password.Text);

            if (KeyAuthApp.response.success)
            {
                LoginSucces();
                Text = KeyAuthApp.response.message;
                Console.Beep(400, 300);
            }
            else
            {
                Text = KeyAuthApp.response.message;
                Console.Beep(400, 300);
            }
        }
        private void LoginSucces()
        {
            this.Hide();
            Form2 frm = new Form2();
            frm.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
