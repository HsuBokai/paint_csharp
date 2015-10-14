using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;
namespace paint
{
    class Ellipse : DynamicDrawing
    {
        public Ellipse(Form1 f)
            : base(f)
        {
            //_pen = f._pen; _brush = f._brush; _checkBoxFill = f.checkBoxFill; _checkBoxFramed = f.checkBoxFramed; _pictureBox = f.pictureBox;        
        }

        public Ellipse(Ellipse e)
            : base((DynamicDrawing)e) {}

        override public AblePaint copy()
        {
            AblePaint temp = new Ellipse(this);
            return temp;
        }

        override public void drawShape()
        {
            dynamicDrawing();
            //setCornor(ref e);
            if (_form1.checkBoxFill.Checked == true) _graphics.FillEllipse(_form1._brush, _left, _up, _width, _height);
            if (_form1.checkBoxFramed.Checked == true || _form1.checkBoxFill.Checked == false) _graphics.DrawEllipse(_form1._pen, _left, _up, _width, _height);
            endDynamicDrawing();
        }
    }
}
