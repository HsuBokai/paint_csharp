using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{
    class Curve : DynamicDrawing
    {
        Point _startPoint, _control1, _control2, _endPoint;
        Point _preStartPoint, _preControl1, _preControl2, _preEndPoint;
        int _PointsNum = 0;
        //Bitmap _buf_buf;

        public Curve(Form1 f)
            : base(f)
        {
            //_pictureBox = f.pictureBox; _pen = f._pen;
        }

        public Curve(Curve c)
            : base((DynamicDrawing)c) 
        {
            //_preStartPoint = c._preStartPoint; _preControl1 = c._preControl1; _preControl2 = c._preControl2; _preEndPoint = c._preEndPoint;
            _startPoint = c._startPoint; _control1 = c._control1; _control2 = c._control2; _endPoint = c._endPoint;
            _PointsNum = c._PointsNum;
        }

        override public AblePaint copy()
        {
            AblePaint temp = new Curve(this);
            return temp;
        }

        override public void mouseDown(ref MouseEventArgs e)
        {
            Console.WriteLine("call mouseDown" + _form1._mode);
            _firstPoint = e.Location;
            //_preNewPoint = _newPoint;
            //_newPoint = e.Location;
            if (_form1._mode == EditMode.choose)
            {
                if (_zoom.mouseDown(_firstPoint))
                {
                    updateDrawPoints();
                    _form1._mode = EditMode.Zoom;
                    return;
                }
                else if (isInChooseArea(_firstPoint)) updateDrawPoints();
                else endChoosing();
            }
            if (_form1._mode == EditMode.normal)
            {
                _PointsNum = 0;
                ++_PointsNum;
                //_mode = EditMode.drawing;// 此處有bug 如果先點一下就放開， 沒有移動 mode會跑到drawing 但應該要是normal
                _buf = new Bitmap(_form1._img);
                _startPoint = e.Location;
                //_preNewPoint = e.Location;
            }
            else if (_form1._mode == EditMode.drawing)
            {
                ++_PointsNum;
                //keepDrawing();
                if (_PointsNum == 2) _control1 = e.Location;
                else if (_PointsNum == 3) _control2 = e.Location;
                drawShape();
            }
        }

        override public void updatePosition()
        {
            _left = Math.Min(Math.Min(_startPoint.X, _endPoint.X), Math.Min(_control1.X, _control2.X));
            _up = Math.Min(Math.Min(_startPoint.Y, _endPoint.Y), Math.Min(_control1.Y, _control2.Y));
            _width = Math.Max(Math.Max(_startPoint.X, _endPoint.X), Math.Max(_control1.X, _control2.X)) - _left;
            _height = Math.Max(Math.Max(_startPoint.Y, _endPoint.Y), Math.Max(_control1.Y, _control2.Y)) - _up;
        }

        override public void mouseUp(ref MouseEventArgs e)
        {
            Console.WriteLine("call mouseUp");
            if (_form1._mode == EditMode.normal) _PointsNum = 0;
            if (_PointsNum == 3)
            {
                updatePosition();
                if (_form1._mode == EditMode.drawing)
                {
                    endDrawing();
                    //_PointsNum = 0;
                    //if (_width <= 0 || _height <= 0) endChoosing();
                    //chooseShape();
                    _zoom.addZoomPoints();
                }
                else if (_form1._mode == EditMode.choose)
                {
                    _zoom.updateZoomPoints();
                }
                else if (_form1._mode == EditMode.Zoom)
                {
                    _zoom.updateZoomPoints();
                    //chooseShape();
                    _form1._mode = EditMode.choose;
                }
                else MessageBox.Show("Error at MouseUp()!!");
                showZoomPoints();
            }
            //_form1._img = _buf; // it's bug
             //_buf = _buf_buf;
        }
        
        public override void drawShape()
        {
            Console.WriteLine("call drawShape");
            dynamicDrawing();
            if (_form1._mode == EditMode.Zoom)
            {
                double rateX = _zoom.getRateX(), rateY = _zoom.getRateY();
                switch (_zoom.getMoveIndex())
                {
                    case 0: 
                        _startPoint.X = (int)((_preStartPoint.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _startPoint.Y = (int)((_preStartPoint.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _control1.X = (int)((_preControl1.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _control1.Y = (int)((_preControl1.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _control2.X = (int)((_preControl2.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _control2.Y = (int)((_preControl2.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _endPoint.X = (int)((_preEndPoint.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _endPoint.Y = (int)((_preEndPoint.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        break;
                    case 1: 
                        _startPoint.Y = (int)((_preStartPoint.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _control1.Y = (int)((_preControl1.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _control2.Y = (int)((_preControl2.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _endPoint.Y = (int)((_preEndPoint.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        break;
                    case 2:
                        _startPoint.Y = (int)((_preStartPoint.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _control1.Y = (int)((_preControl1.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _control2.Y = (int)((_preControl2.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _endPoint.Y = (int)((_preEndPoint.Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                        _startPoint.X = (int)((_preStartPoint.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _control1.X = (int)((_preControl1.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _control2.X = (int)((_preControl2.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _endPoint.X = (int)((_preEndPoint.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        break;
                    case 3: 
                        _startPoint.X = (int)((_preStartPoint.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _control1.X = (int)((_preControl1.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _control2.X = (int)((_preControl2.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _endPoint.X = (int)((_preEndPoint.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        break;
                    case 4:
                        _startPoint.X = (int)((_preStartPoint.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _startPoint.Y = (int)((_preStartPoint.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _control1.X = (int)((_preControl1.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _control1.Y = (int)((_preControl1.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _control2.X = (int)((_preControl2.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _control2.Y = (int)((_preControl2.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _endPoint.X = (int)((_preEndPoint.X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                        _endPoint.Y = (int)((_preEndPoint.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        break;
                    case 5: 
                        _startPoint.Y = (int)((_preStartPoint.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _control1.Y = (int)((_preControl1.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _control2.Y = (int)((_preControl2.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _endPoint.Y = (int)((_preEndPoint.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        break;
                    case 6:
                        _startPoint.Y = (int)((_preStartPoint.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _control1.Y = (int)((_preControl1.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _control2.Y = (int)((_preControl2.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _endPoint.Y = (int)((_preEndPoint.Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                        _startPoint.X = (int)((_preStartPoint.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _control1.X = (int)((_preControl1.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _control2.X = (int)((_preControl2.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _endPoint.X = (int)((_preEndPoint.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        break;
                    case 7: 
                        _startPoint.X = (int)((_preStartPoint.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _control1.X = (int)((_preControl1.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _control2.X = (int)((_preControl2.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        _endPoint.X = (int)((_preEndPoint.X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                        break;
                }
            }

            //_graphics.DrawBezier(_form1._pen, new Point(_startPoint.X + _changeX, _startPoint.Y + _changeY), new Point(_control1.X + _changeX, _control1.Y + _changeY), new Point(_control2.X + _changeX, _control2.Y + _changeY), new Point(_endPoint.X + _changeX, _endPoint.Y + _changeY));
            _graphics.DrawBezier(_form1._pen, _startPoint, _control1, _control2, _endPoint);
            endDynamicDrawing();
        }

        //Point zoomPoint(Point fix, Point move)
        //{

        //}

        protected override void setCornor(ref MouseEventArgs e)
        {
            Console.WriteLine("call setCornor");
            if (_PointsNum == 1)
            {
                _control1 = _control2 = _startPoint;
                _endPoint = e.Location;
                //_buf_buf = new Bitmap(_buf);
                //_graphics = Graphics.FromImage(_buf_buf);
                //_pictureBox.Image = _buf_buf;

                //keepDrawing()
                //_graphics.DrawLine(_pen, _preNewPoint, e.Location);
                //_newPoint = e.Location;
            }
            else if (_PointsNum == 2) _control1 = e.Location;
            else if (_PointsNum == 3) _control2 = e.Location;
            else MessageBox.Show("curve::setCornor error" + _PointsNum);
        }
        public override void moveShape(int changeX, int changeY)
        {
            //_changeX = changeX;
            //_changeY = changeY;
            Console.WriteLine("call MoveShape!!");
            _startPoint.X = _preStartPoint.X + changeX;
            _startPoint.Y = _preStartPoint.Y + changeY;
            _control1.X = _preControl1.X + changeX;
            _control1.Y = _preControl1.Y + changeY;
            _control2.X = _preControl2.X + changeX;
            _control2.Y = _preControl2.Y + changeY;
            _endPoint.X = _preEndPoint.X + changeX;
            _endPoint.Y = _preEndPoint.Y + changeY;
            drawShape();
        }
        protected override void updateDrawPoints()
        {
            _preStartPoint = _startPoint;
            _preControl1 = _control1;
            _preControl2 = _control2;
            _preEndPoint = _endPoint;
        }

        /*protected override void updateZoomPoints()
        {
            _left = Math.Min(Math.Min(_startPoint.X, _endPoint.X), Math.Min(_control1.X, _control2.X));
            _up = Math.Min(Math.Min(_startPoint.Y, _endPoint.Y), Math.Min(_control1.Y, _control2.Y));
            _width = Math.Max(Math.Max(_startPoint.X, _endPoint.X), Math.Max(_control1.X, _control2.X)) - _left;
            _height = Math.Max(Math.Max(_startPoint.Y, _endPoint.Y), Math.Max(_control1.Y, _control2.Y)) - _up;
            base.updateZoomPoints();
        }*/
        /*private void keepDrawing()
        {
            _buf_buf = new Bitmap(_buf);
            _graphics = Graphics.FromImage(_buf);
            _pictureBox.Image = _buf;
        }*/
        /*override protected void endDrawing()
        {
            base.endDrawing(ref img);
        }*/
    }
}
