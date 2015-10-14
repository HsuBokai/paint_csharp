using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{
    class Pencil : AblePaint
    {
        protected List<Point> _points = new List<Point>();
        protected Graphics _graphics;
        //protected PictureBox _pictureBox;
        //protected Pen _pen;

        public Pencil(Form1 f):base(f)
        {
            //_pictureBox = f.pictureBox; _pen = f._pen;
        }

        override public void mouseDown(ref MouseEventArgs e)
        {
            _form1._img.SetPixel(e.Location.X, e.Location.Y, _form1._pen.Color);
            _form1.pictureBox.Image = _form1._img;
            _points.Clear();
            _points.Add(e.Location);
        }

        /*override public void mouseUp(ref MouseEventArgs e)
        {
        }*/

        override public void mouseMove(ref MouseEventArgs e)
        {
            _points.Add(e.Location);
            _graphics = Graphics.FromImage(_form1._img);
            _graphics.DrawLine(_form1._pen, _points[_points.Count - 2], e.Location);
            _form1.pictureBox.Image = _form1._img;
        }
    }

    class Erase : Pencil
    {
        public Erase(Form1 f) : base(f)
        {
        }

        override public void mouseMove(ref MouseEventArgs e)
        {
            _points.Add(e.Location);
            _graphics = Graphics.FromImage(_form1._img);
            _graphics.DrawLine(new Pen(_form1.pictureBox.BackColor, _form1._pen.Width), _points[_points.Count - 2], e.Location);
            _form1.pictureBox.Image = _form1._img;
        }
    }
}
