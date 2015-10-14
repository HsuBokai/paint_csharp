using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{
    class Text : DynamicDrawing
    {
        public Text(Form1 f)
            : base(f)
        {
        }

        public Text(Text e)
            : base((DynamicDrawing)e) {}

        override public AblePaint copy()
        {
            AblePaint temp = new Text(this);
            return temp;
        }

        private void updateTextBox()
        {
            _form1.richTextBox.Location = new Point(_left + _form1.pictureBox.Location.X + 1, _up + _form1.pictureBox.Location.Y + 3);
            _form1.richTextBox.Size = new Size(_width - 3, _height - 3);
        }

        override public void drawShape()
        {
            updateTextBox();
            dynamicDrawing();
            dashRectangle(Color.Black, 6);
            endDynamicDrawing();
        }

        override public void mouseUp(ref MouseEventArgs e)
        {
            Console.WriteLine("call mouseUp " + _form1._mode);
            if (_form1._mode == EditMode.normal) return;
            if (_form1._mode == EditMode.drawing)
            {
                endDrawing();
                if (_width <= 0 || _height <= 0) endChoosing();
                _zoom.addZoomPoints();

                updateTextBox();
                _form1.richTextBox.Visible = true;
            }
            if (_form1._mode == EditMode.choose)
            {
                _zoom.updateZoomPoints();
            }
            else if (_form1._mode == EditMode.Zoom)
            {
                _zoom.updateZoomPoints();
                _form1._mode = EditMode.choose;
            }
            //MessageBox.Show("Error at MouseUp()!!");
            showZoomPoints();
        }

        override public void endChoosing()
        {
            Console.WriteLine("call Text endChoosing " + _form1._mode);
            if (_form1._mode == EditMode.choose)
            {
                RichTextBox textBox = _form1.richTextBox;
                _graphics = Graphics.FromImage(_form1._img);
                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Center;
                strFormat.LineAlignment = StringAlignment.Center;
                //_graphics.DrawString(textBox.Text, textBox.Font, new SolidBrush(textBox.ForeColor), new Rectangle(_left, _up, _width, _height), new StringFormat());
                _graphics.DrawString(textBox.Text, textBox.Font, new SolidBrush(textBox.ForeColor), _left, _up + 2);
                textBox.Text = "";
                textBox.Visible = false;

                _form1.pictureBox.Image = _form1._img;
                _zoom._zoomPoints.Clear();
                //_form1._img = _buf;
                _form1.addBitmap(_form1._img);
                _form1._mode = EditMode.normal;
            }
        }
    }
}
