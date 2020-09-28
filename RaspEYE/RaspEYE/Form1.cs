using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace RaspEYE
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        Bitmap image;

        int y = 0;
        int x = 0;
        int wid = 0;
        Bitmap bmp2;
        int widz = 0;

        //открытие
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog nf = new OpenFileDialog();
            nf.Filter = "Image Files(*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG)|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG|Все файлы(*.*)|*.*";
            if (nf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(nf.FileName);
                    button2.Visible = true;
                    
                    image = new Bitmap(pictureBox1.Image);
                    kk = 0;
                    pictureBox2.Visible = false;
                    radioButton1.Checked = true;
                    if (bmp2 != null)
                    { bmp2.Dispose(); }
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть файл");
                }
            }
            button2.Enabled = false;
            button3.Enabled = false;


        }
       
        int kk = 0;
        //отрисовка на оригинале
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (kk >= 2)
            {
                MessageBox.Show("Изображение уже обработано");
            }
            else
            {
                pictureBox3.Image = pictureBox1.Image;
                if (wid == widz && radioButton2.Checked == true)
                { MessageBox.Show("Вы не выбрали параметры для радужки глаза"); kk = 1; }
                else { paint3(); }

                if (wid != widz && radioButton2.Checked == true)
                {
                    kk = 2;
                }
                if (radioButton1.Checked==true)
                {
                    kk = 1;
                    radioButton2.Checked = true;
                    Bitmap bmp1 = pictureBox3.Image as Bitmap;
                    wid *= 4;
                    bmp2 = bmp1.Clone(new Rectangle(x - wid / 2, y - wid / 2, wid, wid), bmp1.PixelFormat);
                    paint4();
                    widz = wid;
                }


            }
            button3.Enabled = true;


        }

        public void paint4()
        {
            var im1 = new Bitmap(pictureBox3.Image);

            Graphics g = Graphics.FromImage(im1);

            Bitmap bmp1 = new Bitmap(bmp2, wid, wid);       //подгоняем размер картинки под размер прямоугольника     
            g.Clear(Color.White);
            g.DrawImage(bmp1, x - wid / 2, y - wid / 2);      //рисуем картинку
            g.DrawRectangle(Pens.White, new Rectangle(x - wid / 2, y - wid / 2, wid, wid)); //рисуем прямоугольник         
            pictureBox4.Image = im1;

        }

        public void paint3()
        {
            var im1 = new Bitmap(pictureBox1.Image);

            Graphics g = Graphics.FromImage(im1);

            g.DrawEllipse(new Pen(Color.Red, 3), new Rectangle(x - wid / 2, y - wid / 2, wid, wid));
            pictureBox1.Image = im1;

        }

        public void paint()
        {
            var im2 = new Bitmap(pictureBox2.Image);

            Graphics g1 = Graphics.FromImage(im2);
            g1.DrawLine(new Pen(Color.Red, 3), new Point(0, y), new Point(im2.Width, y));
            g1.DrawLine(new Pen(Color.Red, 3), new Point(x, 0), new Point(x, im2.Height));
            g1.DrawEllipse(new Pen(Color.Red, 3), new Rectangle(x - wid / 2, y - wid / 2, wid, wid));
            pictureBox2.Image = im2;
        }

        //откат
        private void button3_Click(object sender, EventArgs e)
        {

            kk--;
            if (kk == 0)
            {
                radioButton1.Checked = true;
                pictureBox1.Image = image;
                button3.Enabled = false;
            }
            else if (kk == 1)
            {
                pictureBox1.Image = pictureBox3.Image;
            }
        }

        // отрисовка на чб
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int t = Convert.ToInt16(textBox1.Text);
                if (t>255)
                { MessageBox.Show("Слишком большое значение бинеризации"); textBox1.Text = "150"; return; }
                button2.Enabled = true;
                pictureBox2.Visible = true;
                var bmp = new Bitmap(image);            
                if (radioButton1.Checked == false)
                { bmp.Dispose(); bmp = new Bitmap(pictureBox4.Image); }

                for (int i = 0; i < bmp.Width; i++)
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        int R = bmp.GetPixel(i, j).R;
                        int G = bmp.GetPixel(i, j).G;
                        int B = bmp.GetPixel(i, j).B;
                        int Gray = Convert.ToInt32(0.2125 * R + 0.7154 * G + 0.0721 * B);
                        if (Gray < t)
                        {
                            R = G = B = 0;
                        }
                        else
                        {
                            R = G = B = 255;
                        }
                        Color RGB = Color.FromArgb(255, R, G, B);
                        bmp.SetPixel(i, j, RGB);
                    }

                pictureBox2.Image = bmp;

                Bitmap b = new Bitmap(pictureBox2.Width, pictureBox2.Height);
                b = (Bitmap)pictureBox2.Image;

                int[] counts = new int[b.Height];
                for (int i = 0; i < b.Height; i++)
                {
                    counts[i] = 0;
                }
                int maxxx = 0;
                for (int i = 0; i < b.Height; i++)
                {
                    maxxx = 0;
                    for (int j = 0; j < b.Width; j++)
                    {
                        int r = b.GetPixel(j, i).R;
                        if (r == 0)
                        { counts[i] += 1; maxxx++; }
                    }

                }

                int max = counts[0];
                int maxid = 0;

                for (int i = 1; i < counts.Length; i++)
                {
                    if (counts[i] > max)
                    {
                        max = counts[i];
                        maxid = i;
                    }
                }
                wid = max;
                if (radioButton1.Checked == true)
                { y = maxid; }

                //--------------------------------------------------------------------------

                counts = new int[b.Width];
                for (int i = 0; i < b.Width; i++)
                {
                    counts[i] = 0;
                }
                maxxx = 0;
                for (int i = 0; i < b.Width; i++)
                {
                    maxxx = 0;
                    for (int j = 0; j < b.Height; j++)
                    {
                        int r = b.GetPixel(i, j).R;
                        if (r == 0)
                        { counts[i] += 1; maxxx++; }
                    }
                }

                max = counts[0];
                maxid = 0;

                for (int i = 1; i < counts.Length; i++)
                {
                    if (counts[i] > max)
                    {
                        max = counts[i];
                        maxid = i;
                    }
                }
                if (radioButton1.Checked == true)
                { x = maxid; }
                 
               
                paint();
                
            }
            catch
            {
                MessageBox.Show("Не введен параметр t");
            }
        }
        

        private void radioButton2_MouseClick(object sender, MouseEventArgs e)
        {
            if (kk == 0)
            {
                radioButton1.Checked = true;
                MessageBox.Show("Не опознан зрачок глаза");

            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }

            
        }
    }
}
