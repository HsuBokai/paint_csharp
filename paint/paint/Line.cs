using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{        
    class Line : DynamicDrawing
    {
        Point _endPoint,_startPoint;
        public Line(Form1 f)
            :base(f)
        {
            //_pictureBox = f.pictureBox; _pen = f._pen;
        }

        public Line(Line e)
            : base((DynamicDrawing)e) 
        {
            _endPoint = e._endPoint; _startPoint = e._startPoint;
        }

        override public AblePaint copy()
        {
            AblePaint temp = new Line(this);
            return temp;
        }

        override public void mouseUp(ref MouseEventArgs e)
        {
            Console.WriteLine("call mouseUp");
            if (_form1._mode == EditMode.normal) return;
            if (_form1._mode == EditMode.drawing)
            {
                endDrawing();
                if (_width <= 0 || _height <= 0) endChoosing();
                addZoomPoints();
            }
            else if (_form1._mode == EditMode.choose)
            {
                updateZoomPoints();
                updatePosition();
            }
            else if (_form1._mode == EditMode.Zoom)
            {
                updateZoomPoints();
                updatePosition();
                //chooseShape();
                _form1._mode = EditMode.choose;
            }
            else MessageBox.Show("Error at MouseUp()!!");
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
                drawShape();
            }
            else if (_form1._mode == EditMode.choose)
            {
                moveShape(e.Location.X - _firstPoint.X, e.Location.Y - _firstPoint.Y);
            }
            else if (_form1._mode == EditMode.Zoom)
            {
                zoomMouseMove(ref e);
            }
        }
        void zoomMouseMove(ref MouseEventArgs e)
        {
            int moveIndex = _zoom.whichZoom(ref _firstPoint);
            switch (moveIndex)
            {
                case 0:
                    _startPoint = e.Location;
                    break;
                case 1:
                    _endPoint = e.Location;
                    break;
            }
            drawShape();
        }
        public override void drawShape()
        {
            dynamicDrawing();
            if (_form1._mode == EditMode.choose) _graphics.DrawLine(_form1._pen, _startPoint, _endPoint);
            else _graphics.DrawLine(_form1._pen, _startPoint, _endPoint);
            endDynamicDrawing();
        }
        protected override void setCornor(ref MouseEventArgs e)
        {
            base.setCornor(ref e);
            _startPoint = _firstPoint;
            _endPoint = e.Location;
        }
        protected override void showZoomPoints()
        {
            //dynamicDrawing();
            Bitmap bufbuf = new Bitmap(_buf);
            _graphics = Graphics.FromImage(bufbuf);
            dashRectangle(Color.Blue, 6);
            drawSquare(_startPoint.X, _startPoint.Y, bufbuf); // 0
            drawSquare(_endPoint.X, _endPoint.Y, bufbuf); // 1
            _form1.pictureBox.Image = bufbuf;
            //_form1.pictureBoxChoose.Image = _buf;
        }
        override public void moveTo0()
        {
            updateDrawPoints();
            moveShape(0 - _left, 0 - _up);
            updatePosition();
            updateZoomPoints();
            showZoomPoints();
        }

        public override void moveShape(int changeX, int changeY)
        {
            Console.WriteLine("call MoveShape!!");
            _startPoint.X = _zoom._zoomPoints[0].X + changeX;
            _startPoint.Y = _zoom._zoomPoints[0].Y + changeY;
            _endPoint.X = _zoom._zoomPoints[1].X + changeX;
            _endPoint.Y = _zoom._zoomPoints[1].Y + changeY;
            drawShape();
            //endDynamicDrawing();
        }

        protected void updateZoomPoints()
        {
            _zoom._zoomPoints[0] = new Point(_startPoint.X, _startPoint.Y); //0
            _zoom._zoomPoints[1] = new Point(_endPoint.X, _endPoint.Y); // 1
        }

        public override void updatePosition()
        {
            _left = Math.Min(_startPoint.X, _endPoint.X);
            _up = Math.Min(_startPoint.Y, _endPoint.Y);
            _width = Math.Max(_startPoint.X, _endPoint.X) - _left;
            _height = Math.Max(_startPoint.Y, _endPoint.Y) - _up;
        }

        protected void addZoomPoints()
        {
            Point s = _startPoint , e = _endPoint;
            _zoom._zoomPoints.Add(s); // 0
            _zoom._zoomPoints.Add(e); // 1 
        }
    }
}
