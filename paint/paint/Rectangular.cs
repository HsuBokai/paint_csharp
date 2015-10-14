using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{
    class Rectangular : DynamicDrawing
    {
        public Rectangular(Form1 f)
            : base(f)
        {
            //_pen = f._pen; _brush = f._brush; _checkBoxFill = f.checkBoxFill; _checkBoxFramed = f.checkBoxFramed; _pictureBox = f.pictureBox;
        }

        public Rectangular(Rectangular r)
            : base((DynamicDrawing)r) {}

        override public AblePaint copy()
        {
            AblePaint temp = new Rectangular(this);
            return temp;
        }

        override public void drawShape()
        {
            //setCornor(ref e);
            dynamicDrawing();
            if (_form1.checkBoxFill.Checked == true) _graphics.FillRectangle(_form1._brush, _left, _up, _width, _height);
            if (_form1.checkBoxFramed.Checked == true || _form1.checkBoxFill.Checked == false) _graphics.DrawRectangle(_form1._pen, _left, _up, _width, _height);
            //if (_form1.checkBoxFill.Checked == true) _graphics.FillRectangle(_form1._brush, _scale, _scale, _width, _height);
            //if (_form1.checkBoxFramed.Checked == true || _form1.checkBoxFill.Checked == false) _graphics.DrawRectangle(_form1._pen, _scale, _scale, _width, _height);
            //Form1.pasteBitmap(_form1._img, _left, _up, _buf);
            endDynamicDrawing();
        }

        /*override protected void chooseShape()
        {
            //int scale = (int)_form1._pen.Width;
            //_form1._choose = new Bitmap(_width + scale, _height + scale);
            //_form1._choose = _buf.Clone(new Rectangle(_left - scale / 2-1, _up - scale / 2-1, _width + scale, _height + scale), _buf.PixelFormat);
            
            _form1._choose = new Bitmap(_buf.Clone(new Rectangle(_left, _up, _width, _height), _buf.PixelFormat));
        }
        */
    }
}
