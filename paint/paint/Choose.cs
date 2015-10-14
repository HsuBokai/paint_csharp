using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{
    class Choose : DynamicDrawing
    {
        Bitmap _choose;

        public Choose(Form1 f)
            :base(f)
        {
            //_pen = f._pen; _brush = f._brush; _checkBoxFill = f.checkBoxFill; _checkBoxFramed = f.checkBoxFramed; _pictureBox = f.pictureBox;
        }

        public Choose(Choose c)
            : base((DynamicDrawing)c) 
        {
            _choose = new Bitmap(c._choose);
        }

        override public AblePaint copy()
        {
            AblePaint temp = new Choose(this);
            return temp;
        }

        protected override void chooseShape()
        {
            try
            {
                _choose = new Bitmap(_width, _height);
                _choose = _form1._img.Clone(new Rectangle(_left, _up, _width, _height), _form1._img.PixelFormat);
                Bitmap white = new Bitmap(_width, _height);
                for (int i = 0; i < white.Width; ++i)
                    for (int j = 0; j < white.Height; ++j)
                        white.SetPixel(i, j, _form1.pictureBox.BackColor);
                Form1.pasteBitmap(_form1._img, _left, _up, white);
                //_form1.addBitmap(_form1._img);
            }
            catch { }
        }

        override public void drawShape()
        {
            dynamicDrawing();
            //setCornor(ref e);
            dashRectangle(Color.Black, 6);
            endDynamicDrawing();
        }

        override public void moveShape(int changeX, int changeY)
        {
            _left = _zoom._zoomPoints[0].X + changeX;
            _up = _zoom._zoomPoints[0].Y + changeY;
            dynamicDrawing();
            int l = 0, u = 0 , ll = 0, uu = 0;
            if (_left < 0) l = _left;
            else if (_left + _width > _form1._img.Width) ll = _form1._img.Width - _left - _width;
            if (_up < 0) u = _up;
            else if (_up + _height > _form1._img.Height) uu = _form1._img.Height - _up - _height;
            Form1.pasteBitmap(_buf, _left - l, _up - u, _choose.Clone(new Rectangle(-l, -u, _choose.Width + l + ll, _choose.Height + u + uu), _choose.PixelFormat));
            endDynamicDrawing();
        }

        override public void endChoosing()
        {
            Console.WriteLine("call Choose endChoosing " + _form1._mode);
            if (_form1._mode == EditMode.choose)
            {
                Form1.pasteBitmap(_form1._img, _left, _up, _choose);
                _form1.pictureBox.Image = _form1._img;
                _zoom._zoomPoints.Clear();
                //_form1._img = _buf;
                _form1.addBitmap(_form1._img);
                _form1._mode = EditMode.normal;
            }
        }

        override public void clearChoose()
        {
            Console.WriteLine("call clearChoose");
            for (int i = 0; i < _choose.Width; ++i)
                for (int j = 0; j < _choose.Height; ++j)
                    _choose.SetPixel(i, j, _form1.pictureBox.BackColor);
            endChoosing();
        }

        /*override protected bool isInChooseArea(ref Point p)
        {
            return _left < p.X && p.X < _left + _width && _up < p.Y && p.Y < _up + _height;
        }*/
    }
}

