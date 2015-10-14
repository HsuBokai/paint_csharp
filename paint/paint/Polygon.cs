using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{
    class Polygon : DynamicDrawing
    {
        Point _newPoint , _preNewPoint;
        Bitmap _img2;
        List<Point> _cornor = new List<Point>(), _preCornor = new List<Point>();
        protected int _changeX = 0, _changeY = 0;

        public Polygon(Form1 f)
            :base(f)
        {
            //_pictureBox = f.pictureBox; _pen = f._pen;
        }

        public Polygon(Polygon e)
            : base((DynamicDrawing)e) 
        {
            _cornor = new List<Point>(e._cornor);
            _img2 = new Bitmap(e._img2);
            _newPoint = e._newPoint;
        }

        override public AblePaint copy()
        {
            AblePaint temp = new Polygon(this);
            return temp;
        }

        override public void mouseDown(ref MouseEventArgs e)
        {
            _firstPoint = e.Location;
            _preNewPoint = _newPoint;
            _newPoint = e.Location;
            if (_form1._mode == EditMode.choose)
            {
                if (_zoom.mouseDown(_firstPoint))
                {
                    updateDrawPoints();
                    _form1._mode = EditMode.Zoom;
                    return;
                }
                else if(isInChooseArea(_firstPoint)) updateDrawPoints();
                else endChoosing();
            }
            if (_form1._mode == EditMode.normal)
            {
                _cornor.Clear();
                _changeX = _changeY = 0;
                _cornor.Add(new Point(_firstPoint.X, _firstPoint.Y));
                _img2 = new Bitmap(_form1._img);
                _preNewPoint = e.Location;
            }
            else if (_form1._mode == EditMode.drawing)
            {
                dynamicDrawing(); // it's bug
                _graphics.DrawLine(_form1._pen, _preNewPoint, e.Location);
                endDynamicDrawing();
            }
        }
        protected override void updateDrawPoints()
        {
            _preCornor = new List<Point>(_cornor);
        }
        public override void updatePosition()
        {
            _left += _changeX;
            _up += _changeY;
        }

        override public void mouseUp(ref MouseEventArgs e)
        {
            Console.WriteLine("call mouseUp");
            if (_form1._mode == EditMode.normal) return;
            if (_form1._mode == EditMode.drawing)
            {
                _newPoint = e.Location;
                _cornor.Add(new Point(_newPoint.X, _newPoint.Y));
                _form1._img = _buf;
            }
            else if (_form1._mode == EditMode.choose)
            {
                updatePosition();
                _zoom.updateZoomPoints();
                showZoomPoints();
            }
            else if (_form1._mode == EditMode.Zoom)
            {
                _zoom.updateZoomPoints();
                showZoomPoints();
                //chooseShape();
                _form1._mode = EditMode.choose;
            }
            else MessageBox.Show("Error at MouseUp()!!");
        }

        public override void mouseMove(ref MouseEventArgs e)
        {
            //Console.Write("call mouseMove " + _form1._mode);
            _changeX = e.Location.X - _firstPoint.X;
            _changeY = e.Location.Y - _firstPoint.Y;
            if (_form1._mode == EditMode.normal) _form1._mode = EditMode.drawing;
            if (_form1._mode == EditMode.drawing)
            {
                //if (_width <= 0 || _height <= 0) return;
                dynamicDrawing();
                _graphics.DrawLine(_form1._pen, _preNewPoint, e.Location);
                endDynamicDrawing();
            }
            else if (_form1._mode == EditMode.choose)
            {
                moveShape(_changeX, _changeY);
            }
            else if (_form1._mode == EditMode.Zoom)
            {
                _zoom.mouseMove(ref e);
                drawShape();
            }
        }

        override public void drawShape()
        {
            dynamicDrawing();
            
                        if (_form1._mode == EditMode.Zoom)
                        {
                            int x, y;
                            double rateX = _zoom.getRateX(), rateY = _zoom.getRateY();
                            switch (_zoom.getMoveIndex())
                            {
                                case 0:
                                    for (int i = 0; i < _cornor.Count; ++i )
                                    {
                                        x = (int)((_preCornor[i].X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                                        y = (int)((_preCornor[i].Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                                        _cornor[i] = new Point(x,y);
                                    }
                                    break;
                                    
                                case 1:
                                    for (int i = 0; i < _cornor.Count; ++i )
                                    {
                                        //x = (int)((_preCornor[i].X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                                        y = (int)((_preCornor[i].Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                                        _cornor[i] = new Point(_cornor[i].X, y);
                                    } 
                                    break;
                                case 2:
                                    for (int i = 0; i < _cornor.Count; ++i)
                                    {
                                        x = (int)((_preCornor[i].X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                                        y = (int)((_preCornor[i].Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                                        _cornor[i] = new Point(x, y);
                                    }
                                    break;
                                case 3:
                                    for (int i = 0; i < _cornor.Count; ++i)
                                    {
                                        x = (int)((_preCornor[i].X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                                        //y = (int)((_preCornor[i].Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                                        _cornor[i] = new Point(x, _cornor[i].Y);
                                    }
                                    break;
                                case 4:
                                    for (int i = 0; i < _cornor.Count; ++i)
                                    {
                                        x = (int)((_preCornor[i].X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                                        y = (int)((_preCornor[i].Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                                        _cornor[i] = new Point(x, y);
                                    }
                                    break;
                                case 5:
                                    for (int i = 0; i < _cornor.Count; ++i)
                                    {
                                        //x = (int)((_preCornor[i].X - _zoom._zoomPoints[0].X) * _zoom.getRateX() + _zoom._zoomPoints[0].X);
                                        y = (int)((_preCornor[i].Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                                        _cornor[i] = new Point(_cornor[i].X, y);
                                    }
                                    break;
                                case 6:
                                    for (int i = 0; i < _cornor.Count; ++i)
                                    {
                                        x = (int)((_preCornor[i].X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                                        y = (int)((_preCornor[i].Y - _zoom._zoomPoints[0].Y) * _zoom.getRateY() + _zoom._zoomPoints[0].Y);
                                        _cornor[i] = new Point(x, y);
                                    }
                                    break;
                                case 7:
                                    for (int i = 0; i < _cornor.Count; ++i)
                                    {
                                        x = (int)((_preCornor[i].X - _zoom._zoomPoints[4].X) * _zoom.getRateX() + _zoom._zoomPoints[4].X);
                                        //y = (int)((_preCornor[i].Y - _zoom._zoomPoints[4].Y) * _zoom.getRateY() + _zoom._zoomPoints[4].Y);
                                        _cornor[i] = new Point(x, _cornor[i].Y);
                                    }
                                    break;
                            }
                        }
            if (_form1.checkBoxFill.Checked == true) _graphics.FillPolygon(_form1._brush, _cornor.ToArray());
            if (_form1.checkBoxFramed.Checked == true || _form1.checkBoxFill.Checked == false) _graphics.DrawPolygon(_form1._pen,_cornor.ToArray());
            endDynamicDrawing();
            //_buf_buf = new Bitmap(_buf);
            //_graphics = Graphics.FromImage(_buf_buf);
            //_pictureBox.Image = _buf_buf;
        }

        override public void mouseDoubleClick(ref MouseEventArgs e)
        {
            _form1._img = _img2;
            drawShape();
            int maxX = _left = e.Location.X;
            int maxY = _up = e.Location.Y;
            foreach (Point p in _cornor)
            {
                _left = Math.Min(_left, p.X);
                _up = Math.Min(_up, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }
            _width = maxX - _left;
            _height = maxY - _up;
            _zoom._zoomPoints.Clear();
            _zoom.addZoomPoints();
            showZoomPoints();
            _form1._mode = EditMode.choose;
        }
        override public void moveShape(int changeX, int changeY)
        {
            _changeX = changeX;
            _changeY = changeY;
            Console.WriteLine("call MoveShape!!");
            for(int i = 0; i<_cornor.Count; ++i)
            {
                _cornor[i] = new Point(_preCornor[i].X + changeX, _preCornor[i].Y + changeY);
            }
            drawShape();
        }
        protected override void finish()
        {
            _form1._img = _img2;
            drawShape();
        }
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
