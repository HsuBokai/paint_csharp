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
    class Brush : AblePaint
    {
        Graphics _graphics;
        int _preColorARGB;
        //Point _nowPosition;

        public Brush(Form1 f)
            : base(f)
        {
            //_pictureBox = f.pictureBox; _pen = f._pen; _img = f._img; 
            _graphics = Graphics.FromImage(_form1._img);
        }

        override public void mouseDown(ref MouseEventArgs e)
        {
            Color penColor = _form1._pen.Color;
            _preColorARGB = _form1._img.GetPixel(e.Location.X, e.Location.Y).ToArgb();
            //penColor.ToArgb();
            bool b = penColor.ToArgb() == _preColorARGB;
            Console.WriteLine("brush " + b.ToString());
            if (b) return;
            Bitmap img = new Bitmap(_form1._img);
            Stack<Point> myStack = new Stack<Point>();
            Point p = e.Location;
            myStack.Push(p);
            while (myStack.Count > 0)
            {
                //Console.WriteLine(b);
                p = myStack.Pop();
                int x = p.X, y = p.Y;
                img.SetPixel(x, y, penColor);
                if (x + 1 < img.Width && img.GetPixel(x + 1, y).ToArgb() == _preColorARGB) myStack.Push(new Point(x + 1, y));
                if (y + 1 < img.Height && img.GetPixel(x, y + 1).ToArgb() == _preColorARGB) myStack.Push(new Point(x, y + 1));
                if (x - 1 >= 0 && img.GetPixel(x - 1, y).ToArgb() == _preColorARGB) myStack.Push(new Point(x - 1, y));
                if (y - 1 >= 0 && img.GetPixel(x, y - 1).ToArgb() == _preColorARGB) myStack.Push(new Point(x, y - 1));
            }
            _form1._img = img;    
            _form1.pictureBox.Image = img;
        }

        void fill(int x, int y)
        {
            Bitmap img = _form1._img;
            img.SetPixel(x, y,_form1._pen.Color);
            if (x + 1 < img.Width && img.GetPixel(x + 1, y).ToArgb() == _preColorARGB) fill(x + 1, y);
            if (y + 1 < img.Height && img.GetPixel(x, y + 1).ToArgb() == _preColorARGB) fill(x, y + 1);
            if (x - 1 >= 0 && img.GetPixel(x - 1, y).ToArgb() == _preColorARGB) fill(x - 1, y);
            if (y - 1 >= 0 && img.GetPixel(x, y - 1).ToArgb() == _preColorARGB) fill(x + 1, y - 1);
        }
        /*
        override public void mouseUp(ref MouseEventArgs e)
        {
            _isSprayRunning = false;
            _form1.addBitmap(_form1._img);
        }

        Point _mousePosition;
        bool _isSprayRunning = false;
        
        private void runBrush()
        {
            try
            {
                //bool[,] temp = new bool[_form1._img.Width, _form1._img.Height];
                Queue<int> queueX = new Queue<int>(), queueY = new Queue<int>();
                //queueX.Enqueue(_nowPosition.X); queueY.Enqueue(_nowPosition.Y);
                while (queueX.Count > 0)
                {
                    int x = queueX.Dequeue(), y = queueY.Dequeue();
                    _form1._img.SetPixel(x, y, _form1._pen.Color);
                    //temp[x, y] = true;
                    if (x + 1 < _form1._img.Width && _form1._img.GetPixel(x + 1, y).Equals(_preColor))
                    {
                        queueX.Enqueue(x + 1); queueY.Enqueue(y);
                    }
                    if (y + 1 < _form1._img.Height && _form1._img.GetPixel(x, y + 1).Equals(_preColor))
                    {
                        queueX.Enqueue(x); queueY.Enqueue(y + 1);
                    }
                    if (x - 1 >= 0 && _form1._img.GetPixel(x - 1, y).Equals(_preColor))
                    {
                        queueX.Enqueue(x - 1); queueY.Enqueue(y);
                    }
                    if (y - 1 >= 0 && _form1._img.GetPixel(x, y - 1).Equals(_preColor))
                    {
                        queueX.Enqueue(x); queueY.Enqueue(y - 1);
                    }
                }
            }
            catch
            {
            }
        }
        */
    }

}
