using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{
    abstract class DynamicDrawing : AblePaint
    {
        /*protected CheckBox _checkBoxFill, _checkBoxFramed;
        protected PictureBox _pictureBox;
        protected Pen _pen;
        protected SolidBrush _brush;*/

        protected Bitmap _buf;
        protected Zoom _zoom;
        internal protected Point _firstPoint;
        //protected int _changeX, _changeY;
        protected Graphics _graphics;
        internal protected int _up, _left, _width, _height;

        public DynamicDrawing(Form1 f)
            : base(f)
        {
            _zoom = new Zoom(this);
            //_pictureBox = f.pictureBox; _pen = f._pen; _brush = f._brush; _checkBoxFill = f.checkBoxFill; _checkBoxFramed = f.checkBoxFramed;
        }

        public DynamicDrawing(DynamicDrawing dd)
            : base(dd._form1)
        {
            _zoom = new Zoom(dd._zoom, this);
            _left = dd._left; _up = dd._up; _width = dd._width; _height = dd._height; _buf = new Bitmap(dd._buf); 
        }

        override public AblePaint copy() { return this; }

        override public void mouseDown(ref MouseEventArgs e)
        {
            Console.WriteLine("call mouseDown   " + _form1._mode);
            _firstPoint = e.Location;
            if (_form1._mode == EditMode.choose)
            {
                if (_zoom.mouseDown(_firstPoint))
                {
                    _form1._mode = EditMode.Zoom;
                    return;
                }
                else if (isInChooseArea(_firstPoint)) updateDrawPoints();
                else endChoosing();
            }
            else if (_form1._mode == EditMode.normal) { 
                //_img2 = new Bitmap(_form1._img); 
            }
            else if (_form1._mode == EditMode.drawing)
            {
                //_img2 = new Bitmap(_form1._img);
                //endChoosing(ref img);
            }
        }

        override public void mouseUp(ref MouseEventArgs e)
        {
            Console.WriteLine("call mouseUp");
            if (_form1._mode == EditMode.normal) return;
            if (_form1._mode == EditMode.drawing)
            {
                endDrawing();
                if (_width <= 0 || _height <= 0) endChoosing();
                _zoom.addZoomPoints();
                chooseShape();
            }
            if (_form1._mode == EditMode.choose)
            {
                _zoom.updateZoomPoints();
            }
            else if(_form1._mode == EditMode.Zoom) {
                _zoom.updateZoomPoints();
                _form1._mode = EditMode.choose;
            }
            //else MessageBox.Show("Error at MouseUp()!!");
            showZoomPoints();
        }

        override public void mouseMove(ref MouseEventArgs e)
        {
            //Console.Write("call mouseMove " + _form1._mode);
            if (_form1._mode == EditMode.normal) _form1._mode = EditMode.drawing;
            if (_form1._mode == EditMode.drawing)
            {
                //if (_width <= 0 || _height <= 0) return;
                setCornor(ref e);
                //dynamicDrawing();
                drawShape();
                //endDynamicDrawing();
            }
            else if (_form1._mode == EditMode.choose)
            {
                moveShape(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
            }
            else if (_form1._mode == EditMode.Zoom)
            {
                _zoom.mouseMove(ref e);
                drawShape();
            }
        }

        virtual protected void showZoomPoints() {
            Console.WriteLine("call showZoomPoints");
            //dynamicDrawing();
            Bitmap bufbuf = new Bitmap(_buf);
            _graphics = Graphics.FromImage(bufbuf);
            dashRectangle(Color.Blue, 6);
            drawSquare(_left, _up,bufbuf); // 0
            drawSquare(_left + _width / 2, _up,bufbuf); // 1
            drawSquare(_left + _width, _up, bufbuf); // 2
            drawSquare(_left + _width, _up + _height / 2, bufbuf); // 3
            drawSquare(_left + _width, _up + _height, bufbuf); // 4
            drawSquare(_left + _width / 2, _up + _height, bufbuf); // 5
            drawSquare(_left, _up + _height, bufbuf); // 6
            drawSquare(_left, _up + _height / 2, bufbuf); // 7
            _form1.pictureBox.Image = bufbuf;
            //_form1.pictureBoxChoose.Image = _buf;
        }
        virtual protected void chooseShape() { }

        override public void drawShape() { }
        
        override public void moveTo0() 
        {
            updateDrawPoints();
            moveShape(0 - _zoom._zoomPoints[0].X, 0 - _zoom._zoomPoints[0].Y);
            updatePosition();
            _zoom.updateZoomPoints();
            showZoomPoints();
        }
        virtual public void moveShape(int changeX, int changeY) 
        {
            Console.WriteLine("call MoveShape!!");
            //dynamicDrawing();
            _left = _zoom._zoomPoints[0].X + changeX;
            _up = _zoom._zoomPoints[0].Y + changeY;
            drawShape();
            //endDynamicDrawing();
        }

        virtual protected void setCornor(ref MouseEventArgs e)
        {
            Console.WriteLine("call setCornor");
            _width = Math.Abs(e.Location.X - _firstPoint.X);
            _height = Math.Abs(e.Location.Y - _firstPoint.Y);
            _left = (e.Location.X > _firstPoint.X) ? _firstPoint.X : e.Location.X;
            _up = (e.Location.Y > _firstPoint.Y) ? _firstPoint.Y : e.Location.Y;
            Console.WriteLine("_left is " + _left + "    up is " + _up + "   width is " + _width + "     _height is " + _height);
        }
        protected void dynamicDrawing()
        {
            //_buf = new Bitmap(_form1._img.Clone(new Rectangle(_left-_scale, _up-_scale, _width+_scale*2, _height+_scale*2), _form1._img.PixelFormat));
            _buf = new Bitmap(_form1._img);
            _graphics = Graphics.FromImage(_buf);
            
        }
        protected void endDynamicDrawing()
        {
            //_form1.pictureBoxChoose.Location = new Point(_left + _form1.pictureBox.Location.X, _up - _scale + _form1.pictureBox.Location.Y-_scale);
            //_form1.pictureBoxChoose.Size = new Size(_width+10, _height+10);
            //_form1.pictureBoxChoose.Image = _buf;
            _form1.pictureBox.Image = _buf;
        }
        virtual protected void endDrawing()
        {
            _form1._mode = EditMode.choose;
        }
        override public void endChoosing()
        {
            Console.WriteLine("call Dynamic endChoosing  " + _form1._mode);
            if (_form1._mode == EditMode.normal) return;
            if (_form1._mode == EditMode.drawing) finish();
            _form1._img = _buf;
            //Form1.pasteBitmap(_form1._img, _left, _up, _form1._choose);

            //byte[] tempByte = ImageToBuffer(_buf, System.Drawing.Imaging.ImageFormat.Jpeg);
            //_form1.pictureBox.Image = BufferToImage(tempByte);
            _form1.pictureBox.Image = _form1._img;
            _zoom._zoomPoints.Clear();

            _form1.addBitmap(_form1._img);
            _form1._mode = EditMode.normal;
        }
        virtual protected void finish() { }

        public static byte[] ImageToBuffer(Image Image, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            if (Image == null) { return null; }
            byte[] data = null;
            using (System.IO.MemoryStream oMemoryStream = new System.IO.MemoryStream())
            {
                //建立副本
                using (Bitmap oBitmap = new Bitmap(Image))
                {
                    //儲存圖片到 MemoryStream 物件，並且指定儲存影像之格式
                    oBitmap.Save(oMemoryStream, imageFormat);
                    //設定資料流位置
                    oMemoryStream.Position = 0;
                    //設定 buffer 長度
                    data = new byte[oMemoryStream.Length];
                    //將資料寫入 buffer
                    oMemoryStream.Read(data, 0, Convert.ToInt32(oMemoryStream.Length));
                    //將所有緩衝區的資料寫入資料流
                    oMemoryStream.Flush();
                }
            }
            return data;
        }
        public static Image BufferToImage(byte[] Buffer)
        {
            if (Buffer == null || Buffer.Length == 0) { return null; }
            byte[] data = null;
            Image oImage = null;
            Bitmap oBitmap = null;
            //建立副本
            data = (byte[])Buffer.Clone();
            try
            {
                System.IO.MemoryStream oMemoryStream = new System.IO.MemoryStream(Buffer);
                //設定資料流位置
                oMemoryStream.Position = 0;
                oImage = System.Drawing.Image.FromStream(oMemoryStream);
                //建立副本
                oBitmap = new Bitmap(oImage);
            }
            catch
            {
                throw;
            }
            //return oImage;
            return oBitmap;
        }

        protected void drawSquare(int centerX, int centerY, Bitmap buf)
        {
            _graphics = Graphics.FromImage(buf);
            if (0 < centerX && centerX < buf.Width && 0 < centerY && centerY < buf.Height) buf.SetPixel(centerX, centerY, _form1.pictureBox.BackColor);
            _graphics.DrawRectangle(new Pen(Color.Blue), centerX - 2, centerY - 2, 4, 4);
            _graphics.DrawRectangle(new Pen(_form1.pictureBox.BackColor), centerX - 1, centerY - 1, 2, 2);
        }

        protected void dashRectangle(Color c, int dense)
        {
            Pen p = new Pen(c, 1);
            for (int i = 0; i < _width; i += dense)
            {
                _graphics.DrawLine(p, new Point(_left + i, _up), new Point(_left + i + 3, _up));
                _graphics.DrawLine(p, new Point(_left + i, _up + _height), new Point(_left + i + 3, _up + _height));
            }
            for (int i = 0; i < _height; i += dense)
            {
                _graphics.DrawLine(p, new Point(_left, _up + i), new Point(_left, _up + i + 3));
                _graphics.DrawLine(p, new Point(_left + _width, _up + i), new Point(_left + _width, _up + i + 3));
            }
        }

        override public bool isInChooseArea(Point p)
        {
            return _left < p.X && p.X < _left + _width && _up < p.Y && p.Y < _up + _height;
        }

        override public void setPosition(int left, int up)
        {
            _left = left;
            _up = up;
            /*
            dynamicDrawing();
            Form1.pasteBitmap(_buf, _left, _up, _choose);
            endDynamicDrawing();

            _zoom._zoomPoints.Clear();
            _zoom.addZoomPoints();
            showZoomPoints();
            _form1._mode = EditMode.choose;*/
            //MessageBox.Show("show: " + _left + ","+ _up + ","+ _width + ","+ _height + "," + _firstPoint.ToString() + _form1._mode + isInChooseArea(new Point(10,10)).ToString());
        }

        override public void showShape()
        {
            drawShape();
            _zoom._zoomPoints.Clear();
            _zoom.addZoomPoints();
            showZoomPoints();
            _form1._mode = EditMode.choose;
            //MessageBox.Show("show: " + _left + ","+ _up + ","+ _width + ","+ _height + "," + _firstPoint.ToString() + _form1._mode + isInChooseArea(new Point(10,10)).ToString());
        }

        override public void clearChoose()
        {
            Console.WriteLine("call clearChoose");
            for (int i = _left; i < _width; ++i)
                for (int j = _up; j < _height; ++j)
                    _buf.SetPixel(i, j, _form1.pictureBox.BackColor);
            endChoosing();
        }

        protected virtual void updateDrawPoints() { }
        virtual public void updatePosition() { }
        override public int whichZoom(Point p) { return _zoom.whichZoom(ref p); }
    }

}
