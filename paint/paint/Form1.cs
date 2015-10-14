using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//adding
using System.Drawing.Drawing2D;
using System.Threading;

namespace paint
{
    enum Tool { normal, brush, choose, curve, ellipse, line, pencil, polygon, program, rectangle, spray, text, erase, pick}

    public partial class Form1 : Form
    {
        AblePaint _target, _buffer;
        bool _isMouseDown = false;
        Tool _tool;
        internal EditMode _mode;
        internal Bitmap _img;
        internal Pen _pen = new Pen(Color.Black, 1);
        internal SolidBrush _brush = new SolidBrush(Color.Yellow);
        //internal List<Bitmap> _history = new List<Bitmap>();
        const int _maxHistoryNum = 100;
        internal Bitmap[] _history = new Bitmap[_maxHistoryNum];
        internal int _bitmapIndex = 0, _maxBitmapIndex = 0;
        //Graphics _graphics;
        //Brush _brush = new HatchBrush(HatchStyle.Cross, Color.Magenta,Color.LightBlue);

        public Form1()
        {
            InitializeComponent();
            init();
            buttonColor2.BackColor = Color.Yellow;
            //_target = new AblePaint(this);
            //_graphics = Graphics.FromImage(_img);
        }

        private void init()
        {
            _mode = EditMode.normal;
            _tool = Tool.normal;
            _target = null;
            _img = new Bitmap(pictureBox.Width, pictureBox.Height);
            for (int i = 0; i < _img.Width; ++i)
                for (int j = 0; j < _img.Height; ++j)
                    _img.SetPixel(i, j, pictureBox.BackColor);
            addBitmap(_img);
            pictureBox.Image = _img;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
            if (_tool == Tool.pick) changePenColor(_img.GetPixel(e.X, e.Y));
            if(_target!=null) _target.mouseDown(ref e);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
            if (_target != null) _target.mouseUp(ref e);
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_target == null) return;
            if (_isMouseDown)
            {
                _target.mouseMove(ref e);
            }
            else
            {
                if (_mode == EditMode.choose)
                {
                    /// <summary>
                    ///     0-----1-----2
                    ///     -           -
                    ///     7           3
                    ///     -           -
                    ///     6-----5-----4
                    /// </summary>
                    switch (_target.whichZoom(e.Location))
                    {
                        case 0: Cursor = Cursors.SizeNWSE; break;
                        case 1: Cursor = Cursors.SizeNS; break;
                        case 2: Cursor = Cursors.SizeNESW; break;
                        case 3: Cursor = Cursors.SizeWE; break;
                        case 4: Cursor = Cursors.SizeNWSE; break;
                        case 5: Cursor = Cursors.SizeNS; break;
                        case 6: Cursor = Cursors.SizeNESW; break;
                        case 7: Cursor = Cursors.SizeWE; break;
                        default:
                            if (_target.isInChooseArea(e.Location)) Cursor = Cursors.SizeAll;
                            else Cursor = Cursors.Default;
                            break;
                    }
                }
            }
        }

        private void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_target != null) _target.mouseDoubleClick(ref e);
        }

        private void pictureBox_MouseEnter(object sender, EventArgs e)
        {
            //Console.WriteLine(Application.StartupPath + "   " + _tool.ToString());
            try
            {
                switch (_tool)
                {
                    case Tool.pencil: Cursor = new Cursor(Application.StartupPath + @"\..\..\Icon\pencil.ico"); break;
                    case Tool.erase: Cursor = new Cursor(Application.StartupPath + @"\..\..\Icon\erase.ico"); break;
                    case Tool.spray: Cursor = new Cursor(Application.StartupPath + @"\..\..\Icon\spary.ico"); break;
                    case Tool.brush: Cursor = new Cursor(Application.StartupPath + @"\..\..\Icon\brush.ico"); break;
                    case Tool.text: Cursor = Cursors.IBeam; break;
                    case Tool.choose:
                    case Tool.line:
                    case Tool.rectangle:
                    case Tool.ellipse:
                    case Tool.polygon:
                    case Tool.curve:
                        Cursor = Cursors.Cross;
                        break;
                    default: Cursor = Cursors.Default; break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MouseEnter" + ex.ToString());
            }
        }

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void numericUpDownWidth_ValueChanged(object sender, EventArgs e)
        {
            _pen.Width = (float)numericUpDownWidth.Value;
            if (_mode == EditMode.choose) _target.drawShape();
        }


        private void buttonColor_Click(object sender, EventArgs e)
        {
            colorDialogPen.ShowDialog();
            changePenColor(colorDialogPen.Color);
            if (_mode == EditMode.choose) _target.drawShape();
        }
        private void changePenColor(Color c)
        {
            buttonColor.BackColor = c;
            _pen.Color = buttonColor.BackColor;
        }

        private void buttonColor2_Click(object sender, EventArgs e)
        {
            colorDialogPen.ShowDialog();
            buttonColor2.BackColor = colorDialogPen.Color;
            _brush.Color = buttonColor2.BackColor;
            if (_mode == EditMode.choose) _target.drawShape();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        _img.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        _img.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        _img.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }

                fs.Close();
            }

            //saveFileDialog1.ShowDialog();
            //if (_img != null) _img.Save(@"C:\Users\user\Documents\test.bmp");
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            openFileDialog1.Title = "Open an Image File";
            openFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (openFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                   (System.IO.FileStream)openFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                _img = new Bitmap(fs);
                /*
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        _img.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        _img.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        _img.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }
                 */
                pictureBox.Image = _img;
                fs.Close();
            }
            //openFileDialog1.ShowDialog();
            //_img
        }

        static public void pasteBitmap(Bitmap destBitmap, int left, int up, Bitmap srcBitmap)
        {
            Rectangle srcArea = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
            System.Drawing.Imaging.BitmapData srcData = srcBitmap.LockBits(srcArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, destBitmap.PixelFormat);
            Rectangle destArea = new Rectangle(left, up, srcBitmap.Width, srcBitmap.Height);
            System.Drawing.Imaging.BitmapData destData = destBitmap.LockBits(destArea, System.Drawing.Imaging.ImageLockMode.WriteOnly, destBitmap.PixelFormat);

            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;
            byte[] buffer = new byte[srcData.Stride];
            for (int i = 0; i < srcData.Height; ++i)
            {
                System.Runtime.InteropServices.Marshal.Copy(srcPtr, buffer, 0, buffer.Length);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, destPtr, buffer.Length);

                srcPtr += srcData.Stride;
                destPtr += destData.Stride;
            }
            srcBitmap.UnlockBits(srcData);
            destBitmap.UnlockBits(destData);
        }

        private void buttonClear_MouseDown(object sender, MouseEventArgs e)
        {
            init();
        }

        private void checkBoxFill_CheckedChanged(object sender, EventArgs e)
        {
            if (_mode == EditMode.choose) _target.drawShape();
        }

        private void checkBoxFramed_CheckedChanged(object sender, EventArgs e)
        {
            if (_mode == EditMode.choose) _target.drawShape();
        }

        private void toolStripButtonRectangle_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Rectangular(this);
            _tool = Tool.rectangle;
        }

        private void toolStripButtonBrush_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Brush(this);
            _tool = Tool.brush;
        }

        private void toolStripButtonLine_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Line(this);
            _tool = Tool.line;
        }

        private void toolStripButtonPencil_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Pencil(this);
            _tool = Tool.pencil;
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Curve(this);
            _tool = Tool.curve;
        }

        private void toolStripButtonErase_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Erase(this);
            _tool = Tool.erase;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Spray(this);
            _tool = Tool.spray;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Text(this);
            _tool = Tool.text;
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Polygon(this);
            _tool = Tool.polygon;
        }

        private void toolStripButtonEllipse_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Ellipse(this);
            _tool = Tool.ellipse;
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _target = new Choose(this);
            _tool = Tool.choose;
        }

        private void toolStripButtonPick_Click(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            _tool = Tool.pick;
        }

        private void comboBoxShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_target != null) _target.endChoosing();
            switch (comboBoxShape.Text)
            {
                case "line": _target = new Line(this); break;
                case "rectangular": _target = new Rectangular(this); break;
                case "ellipse": _target = new Ellipse(this); break;
                case "polygon": _target = new Polygon(this); break;
                case "curve": _target = new Curve(this); break;
            }
        }   //已過時
        private void comboBoxTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("call tool selected!!");
            if (_target != null) _target.endChoosing();
            switch (comboBoxTool.Text)
            {
                case "pen": _target = new Pencil(this); break;
                case "erase": _target = new Erase(this); break;
                case "spray": _target = new Spray(this); break;
                case "brush": Cursor = Cursors.PanNE; break;
                case "text": _target = new Text(this); break;
                case "choose": _target = new Choose(this); break;
                default:
                    //_img.SetPixel(100, 100, _pen.Color);
                    break;
            }
        }   //已過時

        private void Copy()
        {
            if (_mode == EditMode.choose)
            {
                _buffer = _target.copy();
            }
        }

        private void Cut()
        {
            if (_mode == EditMode.choose)
            {
                _buffer = _target.copy();
                if (_tool == Tool.choose) _target.clearChoose();
                else
                {
                    if (_target != null) _target.endChoosing();
                    Recovery();
                }
                
            }
        }

        private void Paste()
        {
            if (_buffer == null) return;
            if (_target != null) _target.endChoosing();
            _target = _buffer.copy();
            _target.moveTo0();
            _mode = EditMode.choose;
            //Bitmap buf = new Bitmap(_img);
            //pasteBitmap(buf, 0, 0, _buffer);
            //pictureBox.Image = buf;
        }

        private void Recovery()
        {
            if (_target != null) _target.endChoosing();
            if (!(_bitmapIndex > 1)) return;
            --_bitmapIndex;
            pictureBox.Image = _history[_bitmapIndex - 1];
            _img = new Bitmap(pictureBox.Image);
            Console.WriteLine("the _history.count is " + _maxBitmapIndex);
            Console.WriteLine("the _bitmapIndex is " + _bitmapIndex);
        }

        private void Repeat()
        {
            if (_target != null) _target.endChoosing();
            if (!(_bitmapIndex < _maxBitmapIndex)) return;
            pictureBox.Image = _history[_bitmapIndex];
            _img = new Bitmap(pictureBox.Image);
            ++_bitmapIndex;
            Console.WriteLine("the _history.count is " + _maxBitmapIndex);
            Console.WriteLine("the _bitmapIndex is " + _bitmapIndex);
        }


        public void addBitmap(Bitmap img)
        {
            //if (0 < _bitmapIndex && _bitmapIndex < _history.Count) _history.RemoveRange(_bitmapIndex - 1, _history.Count - _bitmapIndex);
            //_history.Add(img);
            _history[_bitmapIndex] = new Bitmap(img);
            _bitmapIndex = (_bitmapIndex + 1) % _maxHistoryNum;
            _maxBitmapIndex = _bitmapIndex;
            Console.WriteLine("the _history.count is " + _maxBitmapIndex);
            Console.WriteLine("the _bitmapIndex is " + _bitmapIndex);
        }

        private void toolStripButtonCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }
        private void toolStripButtonCut_Click(object sender, EventArgs e)
        {
            Cut();
        }
        private void toolStripButtonPaste_Click(object sender, EventArgs e)
        {
            Paste();
        }
        private void toolStripButtonRecovery_Click(object sender, EventArgs e)
        {
            Recovery();
        }
        private void toolStripButtonRepeat_Click(object sender, EventArgs e)
        {
            Repeat();
        }
        private void recoveryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Recovery();
        }
        private void repeatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Repeat();
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

    }

    enum EditMode { normal, drawing, choose, Zoom };

    class AblePaint
    {
        protected Form1 _form1;

        public AblePaint(Form1 f)
        {
            _form1 = f;
        }

        virtual public void mouseDown(ref MouseEventArgs e) { }

        virtual public void mouseUp(ref MouseEventArgs e) {
            _form1.addBitmap(_form1._img); 
        }

        virtual public void mouseMove(ref MouseEventArgs e) { }

        virtual public void mouseDoubleClick(ref MouseEventArgs e) { }

        virtual public void endChoosing() {
            Console.WriteLine("call AblePaint::endChoosing " + _form1._mode);
        }

        virtual public void drawShape() { }

        virtual public void setPosition(int left, int up) { }
        virtual public void clearChoose() { }
        virtual public bool isInChooseArea(Point p) { return false; }
        virtual public int whichZoom(Point p) { return -1; }
        virtual public AblePaint copy() { return this; }
        virtual public void showShape() { }
        virtual public void moveTo0() { }
    }
}



/*
// Lock the bitmap's bits.  
Rectangle rect = new Rectangle(left, up, material.Width, material.Height);
System.Drawing.Imaging.BitmapData bufData =
    target.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
    //_buf.PixelFormat);
    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

// Get the address of the first line.
IntPtr ptr = bufData.Scan0;

// Declare an array to hold the bytes of the bitmap.
//int bytes = Math.Abs(bufData.Stride) * _buf.Height * 3;
int bytes = material.Width * material.Height * 3;
byte[] rgbValues = new byte[bytes];

// Copy the RGB values into the array.
System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
// Set every third value to 255. A 24bpp bitmap will look red.
int count = 1;
for (int i = 0; i < _width; ++i)
{
    for (int j = 0; j < _height; ++j)
    {
        //Console.WriteLine(rgbValues[count]);
        //Console.WriteLine(_choose.GetPixel(i, j).B);
        //rgbValues[count++] = _choose.GetPixel(i, j).B;
        //rgbValues[count++] = _choose.GetPixel(i, j).G;
        //rgbValues[count++] = _choose.GetPixel(i, j).R;
        rgbValues[count] = 255;
        rgbValues[count + 1] = 255;
        //rgbValues[count + 2] = 255;
        count += 3;
    }
}
//for (int counter = 0; counter < rgbValues.Length; ++counter)
//    rgbValues[counter] = _choose[counter];

// Copy the RGB values back to the bitmap
System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

// Unlock the bits.
_buf.UnlockBits(bufData);*/



/*
// Lock the bitmap's bits.  
//ectangle rect = new Rectangle(_left, _up, _width, _height);
Rectangle rect = new Rectangle(0, 0, _buf.Width, _buf.Height);
System.Drawing.Imaging.BitmapData bufData =
_buf.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
//_buf.PixelFormat);
System.Drawing.Imaging.PixelFormat.Format24bppRgb);
//System.Drawing.Imaging.PixelFormat.Format32bppRgb);

// Get the address of the first line.
IntPtr ptr = bufData.Scan0;

// Declare an array to hold the bytes of the bitmap.
int bytes = Math.Abs(bufData.Stride) * _buf.Height;
//int bytes = _width * _height * 3;
_choose = new byte[bytes];

// Copy the RGB values into the array.
System.Runtime.InteropServices.Marshal.Copy(ptr, _choose, , bytes);
//System.Runtime.InteropServices.Marshal.Copy(ptr, 0, _choose, bytes);

// Set every third value to 255. A 24bpp bitmap will look red.  
for (int counter = 2; counter < _choose.Length; counter += 3)
_choose[counter] = 255;

// Copy the RGB values back to the bitmap
System.Runtime.InteropServices.Marshal.Copy(_choose, 0, ptr, bytes);

// Unlock the bits.
_buf.UnlockBits(bufData);*/