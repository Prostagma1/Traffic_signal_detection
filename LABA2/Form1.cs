using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LABA2
{
    public partial class Form1 : Form
    {
        List<string> paths = new List<string> { };
        Bitmap currentBitmap, bitmapWithRect, mask;
        int[,] gridHistogram;
        bool[,] gridSkip;
        Point[] myRectangle = new Point[2];
        float[] coeff = { 1, 1, 1 };
        int gridWidth, gridHeight, countClasters = 0;
        public Form1()
        {
            InitializeComponent();
        }
        public void Gistogram(Bitmap bitmap)
        {
            var a = (Bitmap)bitmap.Clone();
            int gridSize = 10;
            gridWidth = (int)Math.Ceiling(bitmap.Width / (double)gridSize);
            gridHeight = (int)Math.Ceiling(bitmap.Height / (double)gridSize);
            gridHistogram = new int[10, 10];
            gridSkip = new bool[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int x = gridWidth * i; x < gridWidth * (i + 1); x++)
                    {
                        var n = 0;
                        if (x >= bitmap.Width) break;
                        for (int y = gridHeight * j; y < gridHeight * (j + 1); y++)
                        {
                            if (y >= bitmap.Height) break;

                            var currentPixel = bitmap.GetPixel(x, y);

                            if (currentPixel == Color.FromArgb(255, 255, 255))
                            {
                                n++;
                                gridHistogram[i, j] += 1;

                            }
                        }
                        if (n >= gridHeight)
                        {
                            gridSkip[i, j] = true;
                        }
                    }
                }
            }

            var g = Graphics.FromImage(a);
            for (int x = gridWidth; x < bitmap.Width; x += gridWidth)
            {
                g.DrawLine(new Pen(Color.Red), x, 0, x, bitmap.Height);
            }
            for (int y = gridHeight; y < bitmap.Height; y += gridHeight)
            {
                g.DrawLine(new Pen(Color.Red), 0, y, bitmap.Width, y);
            }
            pictureBox1.Image = a;
            g.Dispose();

        }
        private void GrayWorld()
        {
            float meanR = 0, meanG = 0, meanB = 0, avg, n = 0;

            for (int y = myRectangle[0].Y; y < myRectangle[1].Y; y++)
            {
                for (int x = myRectangle[0].X; x < myRectangle[1].X; x++)
                {
                    var currentPixel = currentBitmap.GetPixel(x, y);
                    n++;
                    meanR += currentPixel.R;
                    meanG += currentPixel.G;
                    meanB += currentPixel.B;

                }
            }
            meanR = meanR / n;
            meanG = meanG / n;
            meanB = meanB / n;

            avg = (meanR + meanG + meanB) / 3;

            coeff[0] = avg / meanR;
            coeff[1] = avg / meanG;
            coeff[2] = avg / meanB;
        }
        private void PrintRectangle()
        {
            var g = Graphics.FromImage(bitmapWithRect);
            int x0, y0, width, height;

            width = Math.Abs(myRectangle[0].X - myRectangle[1].X);
            height = Math.Abs(myRectangle[0].Y - myRectangle[1].Y);

            if (myRectangle[0].X < myRectangle[1].X) x0 = myRectangle[0].X;
            else x0 = myRectangle[1].X;

            if (myRectangle[0].Y < myRectangle[1].Y) y0 = myRectangle[0].Y;
            else y0 = myRectangle[1].Y;

            g.DrawRectangle(new Pen(Color.Red), x0, y0, width, height);
            g.Dispose();

            pictureBox1.Image = bitmapWithRect;
            myRectangle = new Point[] { new Point(x0, y0), new Point(x0 + width, y0 + height) };

        }
        FolderBrowserDialog folder;
        private void button1_Click(object sender, EventArgs e)
        {
            folder = new FolderBrowserDialog();
            folder.SelectedPath = @"D:\Study\4 sem\TechnicalVision\TrafficLightsDataSet";

            if (folder.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                paths.Clear();

                string[] files = Directory.GetFiles(folder.SelectedPath);

                foreach (var file in files)
                {
                    var ext = Path.GetExtension(file);

                    if (ext == ".bmp" || ext == ".png" || ext == ".jpg")
                    {
                        paths.Add(file);
                        listBox1.Items.Add(Path.GetFileName(file));
                    }
                }
            }
            folder.Dispose();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RGB_Pixel_Label.Text = String.Empty;
            currentBitmap = new Bitmap(paths[listBox1.SelectedIndex]);
            bitmapWithRect = (Bitmap)currentBitmap.Clone();
            radioButton1.Visible = true; radioButton2.Visible = true; button2.Visible = true; button3.Visible = true;
            button4.Visible = true; label1.Visible = true; label2.Visible = true; label3.Visible = true;
            textBox1.Visible = true; textBox2.Visible = true; textBox3.Visible = true; textBox4.Visible = true;
            textBox5.Visible = true; textBox6.Visible = true; button6.Visible = true;
            pictureBox1.Height = currentBitmap.Height;
            pictureBox1.Width = currentBitmap.Width;
            pictureBox1.Image = currentBitmap;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            myRectangle[0] = e.Location;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RGB_Pixel_Label.Text = String.Empty;
            coeff_labal.Text = String.Empty;
            label4.Text = String.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GrayWorld();
            coeff_labal.Text = $"R {coeff[0]} G {coeff[1]} B{coeff[2]}";
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < currentBitmap.Height; y++)
            {
                for (int x = 0; x < currentBitmap.Width; x++)
                {
                    var currentPixel = currentBitmap.GetPixel(x, y);
                    var R = currentPixel.R * coeff[0];
                    if (R > 255) R = 255;
                    var G = currentPixel.G * coeff[1];
                    if (G > 255) G = 255;
                    var B = currentPixel.B * coeff[2];
                    if (B > 255) B = 255;
                    currentBitmap.SetPixel(x, y, Color.FromArgb((int)R, (int)G, (int)B));
                }
            }
            pictureBox1.Image = currentBitmap;
        }
        bool[,] visited;
        public Bitmap DoMask(byte[] RGB)
        {
            mask = new Bitmap(currentBitmap.Width, currentBitmap.Height);
            var g = Graphics.FromImage(mask);
            visited = new bool[mask.Height, mask.Width];
            g.Clear(Color.White);
            g.Dispose();
            for (int y = 0; y < currentBitmap.Height; y++)
            {
                for (int x = 0; x < currentBitmap.Width; x++)
                {
                    var currentPixel = currentBitmap.GetPixel(x, y);
                    visited[y, x] = false;
                    if (currentPixel.R < RGB[0] || currentPixel.R > RGB[1] ||
                        currentPixel.G < RGB[2] || currentPixel.G > RGB[3] ||
                        currentPixel.B < RGB[4] || currentPixel.B > RGB[5])
                    {
                        // currentBitmap.SetPixel(x, y, Color.Black);
                        mask.SetPixel(x, y, Color.Black);
                    }

                }
            }
            return mask;

        }

        private void button4_Click(object sender, EventArgs e)
        {

            byte[] cr = new byte[6];
            if (byte.TryParse(textBox1.Text, out cr[0]) && byte.TryParse(textBox2.Text, out cr[1]) && byte.TryParse(textBox3.Text, out cr[2]) &&
                byte.TryParse(textBox4.Text, out cr[3]) && byte.TryParse(textBox5.Text, out cr[4]) && byte.TryParse(textBox6.Text, out cr[5]))
            {
                mask = DoMask(cr);

            }
            else
            {
                MessageBox.Show("Ошибка ввода данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            checkBox1.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            pictureBox1.Image = currentBitmap;
        }

        private string CheckColor(int i)
        {
            int R = 0, G = 0, B = 0, N = 0;

            for (int x = trueClust[i].X0; x < trueClust[i].X1; x++)
            {
                for (int y = trueClust[i].Y0; y < trueClust[i].Y1; y++)
                {
                    var currentPixel = currentBitmap.GetPixel(x, y);
                    R += currentPixel.R;
                    G += currentPixel.G;
                    B += currentPixel.B;
                    N++;
                }
            }

            if (N == 0) { return ""; }
            R /= N; G /= N; B /= N;
            if (Math.Min(R, Math.Min(G, B)) == R) return "Зелёный цвет";
            if (G >= 90) return "Жёлтый цвет";
            return "Красный цвет";
        }
        byte[] green = new byte[] { 0, 130, 200, 255, 0, 255 };
        byte[] redAndYellow = new byte[] { 200, 255, 0, 255, 0, 80 };
        private void button6_Click(object sender, EventArgs e)
        {
            label4.Text = string.Empty;
            for (int j = 0; j < 2; j++)
            {
                clust = new List<myPoint>();
                trueClust = new List<myPoint>();
                countClasters = 0;
                if (j == 0) { mask = DoMask(redAndYellow); }
                else if (j == 1) { mask = DoMask(green); }
                for (int y = 0; y < mask.Height; y++)
                {
                    for (int x = 0; x < mask.Width; x++)
                    {
                        if (mask.GetPixel(x, y) == Color.FromArgb(0, 0, 0))
                        {
                            continue;
                        }
                        if (visited[y, x] == false)
                        {
                            clust.Add(new myPoint { X0 = int.MaxValue, X1 = 0, Y0 = int.MaxValue, Y1 = 0 });
                            ff = 0;
                            SearchAround(x, y);

                            countClasters++;
                        }
                    }
                }
                var g = Graphics.FromImage(currentBitmap);
                CheckVolume();

                for (int i = 0; i < trueClust.Count; i++)
                {
                    label4.Text += CheckColor(i) + "\n";
                    g.DrawString(CheckColor(i), new Font("Consolas", 8), new SolidBrush(Color.Red), trueClust[i].X0- 10, trueClust[i].Y0 + trueClust[i].Height);
                    g.DrawRectangle(new Pen(Color.Red, 2), trueClust[i].RetRect());
                }
                g.Dispose();
                pictureBox1.Image = currentBitmap;
            }
        }

        public void CheckVolume()
        {
            for (int i = 0; i < clust.Count; i++)
            {
                if (clust[i].CheckValue() && clust[i].Diff() <= clust[i].Height * 0.3 && clust[i].Diff() <= clust[i].Width * 0.3)
                {
                    trueClust.Add(clust[i]);
                }
            }
        }
        public bool CheckFill(int i)
        {
            int m = 0;
            for (int x = clust[i].X0; x < clust[i].X1; x++)
            {
                for (int  y = clust[i].Y0; y < clust[i].Y1; y++)
                {
                    if (mask.GetPixel(x,y) != Color.FromArgb(0, 0, 0))
                    {
                        m++;
                    }
                }
            }
            if (m / clust[i].Area() >= 0.3) return true;
            return false;
        }
        public class myPoint
        {
            public int X0 { get; set; }
            public int Y0 { get; set; }
            public int X1 { get; set; }
            public int Y1 { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public bool CheckValue(int w = 20, int h = 100)
            {
                return Width >= w && Height <= h;
            }
            public void ChangeCoords(int x, int y)
            {
                if (X0 > x)
                {
                    X0 = x;
                }
                else if (X1 < x)
                {
                    X1 = x;
                }
                if (Y0 > y)
                {
                    Y0 = y;
                }
                else if (Y1 < y)
                {
                    Y1 = y;
                }
                Width = Math.Abs(X1 - X0);
                Height = Math.Abs(Y1 - Y0);
            }
            public int Diff()
            {
                return Math.Abs(Width - Height);
            }
            public Rectangle RetRect()
            {
                return new Rectangle(X0, Y0, Width, Height);
            }
            public double Area()
            {
                return (double)Width * (double)Height;
            }

        }
        List<myPoint> clust, trueClust;

        private void button7_Click(object sender, EventArgs e)
        {

        }
        long ff = 0;
        private void SearchAround(int x0, int y0, int n = 3)
        {
            if (ff <5000)
            {
                for (int y = y0 - n; y < y0 + n; y++)
                {
                    for (int x = x0 - n; x < x0 + n; x++)
                    {
                        if (x >= 0 && x < currentBitmap.Width && y >= 0 && y < currentBitmap.Height)
                        {
                            if (mask.GetPixel(x, y) == Color.FromArgb(255, 255, 255))
                            {
                                if (visited[y, x] == false)
                                {
                                    visited[y, x] = true;
                                    clust[countClasters].ChangeCoords(x, y);
                                    ff++;
                                    SearchAround(x, y);

                                }
                            }
                        }

                    }
                }
            }
            
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Gistogram(mask);
            var a = new Form2(gridHistogram, gridWidth, gridHeight);
            a.Show();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                pictureBox1.Image = mask;
            }
            else
            {
                pictureBox1.Image = currentBitmap;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            myRectangle[1] = e.Location;
            if (radioButton1.Checked && radioButton1.Visible)
            {
                bitmapWithRect = (Bitmap)currentBitmap.Clone();
                button2.Enabled = true;
                PrintRectangle();

            }

            else if (radioButton2.Checked)
            {
                RGB_Pixel_Label.Text = $"{currentBitmap.GetPixel(e.X, e.Y)}";
                RGB_Pixel_Label.Location = new Point(e.X + 450, e.Y + 51);
            }
        }
    }
}
