using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace paint
{
    class Spray : AblePaint
    {
        Graphics _graphics;
        //PictureBox _pictureBox;
        //Pen _pen;
        //Bitmap _img;

        public Spray(Form1 f)
            :base(f)
        {
            //_pictureBox = f.pictureBox; _pen = f._pen; _img = f._img; 
            _graphics = Graphics.FromImage(_form1._img);
        }


        override public void mouseDown(ref MouseEventArgs e)
        {
            _mousePosition = e.Location;
            _isSprayRunning = true;
            new Thread(new ThreadStart(runSpray)).Start();
        }

        override public void mouseUp(ref MouseEventArgs e)
        {
            _isSprayRunning = false;
            _form1.addBitmap(_form1._img);
        }

        override public void mouseMove(ref MouseEventArgs e)
        {
            _mousePosition = e.Location;
        }

        Point _mousePosition;
        bool _isSprayRunning = false;
        private void runSpray()
        {
            try
            {
                Random _random = new Random();
                while (_isSprayRunning)
                {
                    for (int i = 0; i < 30; ++i)
                    {
                        double radius = _random.NextDouble() * (_form1._pen.Width + 5);
                        double theta = _random.NextDouble() * 2 * Math.PI;
                        double x = _mousePosition.X + radius * Math.Cos(theta);
                        double y = _mousePosition.Y + radius * Math.Sin(theta);
                        if (0 < x && x < _form1._img.Width && 0 < y && y < _form1._img.Height) _form1._img.SetPixel((int)x, (int)y, _form1._pen.Color);
                    }
                    _form1.pictureBox.Image = _form1._img;
                    //Console.WriteLine("position "+ _mousePosition.ToString());
                    Thread.Sleep(20);
                }
            }
            catch
            {
            }
        }

    }
}
