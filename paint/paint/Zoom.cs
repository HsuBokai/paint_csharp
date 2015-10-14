using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//adding
using System.Drawing;
using System.Windows.Forms;

namespace paint
{
    class Zoom
    {
        int _radius = 100 , _moveIndex;
        internal List<Point> _zoomPoints = new List<Point>();
        DynamicDrawing _target;
        double _rateX, _rateY;

        public Zoom(DynamicDrawing dd)
        {
            _target = dd;
        }

        public Zoom(Zoom z , DynamicDrawing dd)
        {
            _zoomPoints = new List<Point>(z._zoomPoints);
            _target = dd;
        }
        /// <summary>
        ///     0-----1-----2
        ///     -           -
        ///     7           3
        ///     -           -
        ///     6-----5-----4
        /// </summary>

        virtual public void updateZoomPoints()
        {
            int _left = _target._left, _up = _target._up, _width = _target._width, _height = _target._height;
            _zoomPoints[0] = new Point(_left, _up); //0
            _zoomPoints[1] = new Point(_left + _width / 2, _up); // 1
            _zoomPoints[2] = new Point(_left + _width, _up); // 2
            _zoomPoints[3] = new Point(_left + _width, _up + _height / 2); // 3
            _zoomPoints[4] = new Point(_left + _width, _up + _height); // 4
            _zoomPoints[5] = new Point(_left + _width / 2, _up + _height); // 5
            _zoomPoints[6] = new Point(_left, _up + _height); // 6
            _zoomPoints[7] = new Point(_left, _up + _height / 2); // 7
        }


        virtual public void addZoomPoints()
        {
            int _left = _target._left, _up = _target._up, _width = _target._width, _height = _target._height;
            _zoomPoints.Add(new Point(_left, _up)); // 0
            _zoomPoints.Add(new Point(_left + _width / 2, _up)); // 1
            _zoomPoints.Add(new Point(_left + _width, _up)); // 2
            _zoomPoints.Add(new Point(_left + _width, _up + _height / 2)); // 3
            _zoomPoints.Add(new Point(_left + _width, _up + _height)); // 4
            _zoomPoints.Add(new Point(_left + _width / 2, _up + _height)); // 5
            _zoomPoints.Add(new Point(_left, _up + _height)); // 6
            _zoomPoints.Add(new Point(_left, _up + _height / 2)); // 7
        }

        private bool isAround(Point p1, Point p2)
        {
            int diffX = p1.X - p2.X;
            int diffY = p1.Y - p2.Y;
            return (diffX * diffX + diffY * diffY) < _radius;
        }

        public bool mouseDown(Point firstPoint)
        {
            _moveIndex = whichZoom(ref firstPoint);
            return _moveIndex != -1;
        }

        public int whichZoom(ref Point firstPoint)
        {
            for (int i = 0; i < _zoomPoints.Count; ++i)
            {
                if (isAround(firstPoint, _zoomPoints[i])) return i;
            }
            return -1;
        }

        public void mouseMove(ref MouseEventArgs e)
        {
            int right = _target._left + _target._width;
            int down = _target._up + _target._height;
            switch (_moveIndex)
            {
                case 0:
                    _rateX = (double)(e.Location.X - _zoomPoints[4].X) / (_zoomPoints[0].X - _zoomPoints[4].X);
                    _rateY = (double)(e.Location.Y - _zoomPoints[4].Y) / (_zoomPoints[0].Y - _zoomPoints[4].Y);
                    _target._left = e.Location.X;
                    _target._up = e.Location.Y;
                    _target._width = right - e.Location.X;
                    _target._height = down - e.Location.Y;
                    break;
                case 1:
                    _rateX = 1;
                    _rateY = (double)(e.Location.Y - _zoomPoints[4].Y) / (_zoomPoints[0].Y - _zoomPoints[4].Y);
                    _target._up = e.Location.Y;
                    _target._height = (down - e.Location.Y);
                    break;
                case 2:
                    _rateX = (double)(e.Location.X - _zoomPoints[0].X) / (_zoomPoints[4].X - _zoomPoints[0].X);
                    _rateY = (double)(e.Location.Y - _zoomPoints[4].Y) / (_zoomPoints[0].Y - _zoomPoints[4].Y);
                    _target._up = e.Location.Y;
                    _target._height = (down - e.Location.Y);
                    _target._width = (e.Location.X - _target._left);
                    break;
                case 3:
                    _rateX = (double)(e.Location.X - _zoomPoints[0].X) / (_zoomPoints[4].X - _zoomPoints[0].X);
                    _rateY = 1;
                    _target._width = e.Location.X - _target._left;
                    break;
                case 4:
                    _rateX = (double)(e.Location.X - _zoomPoints[0].X) / (_zoomPoints[4].X - _zoomPoints[0].X);
                    _rateY = (double)(e.Location.Y - _zoomPoints[0].Y) / (_zoomPoints[4].Y - _zoomPoints[0].Y);
                    _target._width = e.Location.X - _target._left;
                    _target._height = e.Location.Y - _target._up;
                    break;
                case 5:
                    _rateX = 1;
                    _rateY = (double)(e.Location.Y - _zoomPoints[0].Y) / (_zoomPoints[4].Y - _zoomPoints[0].Y);
                    _target._height = e.Location.Y - _target._up;
                    break;
                case 6:
                    _rateX = (double)(e.Location.X - _zoomPoints[4].X) / (_zoomPoints[0].X - _zoomPoints[4].X);
                    _rateY = (double)(e.Location.Y - _zoomPoints[0].Y) / (_zoomPoints[4].Y - _zoomPoints[0].Y);
                    _target._left = e.Location.X;
                    _target._height = e.Location.Y - _target._up;
                    _target._width = right - e.Location.X;
                    break;
                case 7:
                    _rateX = (double)(e.Location.X - _zoomPoints[4].X) / (_zoomPoints[0].X - _zoomPoints[4].X);
                    _rateY = 1;
                    _target._left = e.Location.X;
                    _target._width = right - e.Location.X;
                    break;
            }
            Console.WriteLine(_rateX + "   " + _rateY);
        }

        public double getRateX() { return _rateX; }
        public double getRateY() { return _rateY; }
        public int getMoveIndex() { return _moveIndex; }

    }
}
