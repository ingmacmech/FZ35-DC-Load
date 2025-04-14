using System;
using System.Windows.Controls;
using System.Windows;

namespace LineCharts
{
    public class ChartStyle
    {
        private Canvas _chartCanvas;

        private double _xMin = 0;
        private double _xMax = 10;
        private double _yMin = 0;
        private double _yMax = 10;


        public Canvas ChartCanvas
        {
            get { return _chartCanvas; }
            set { _chartCanvas = value; }
        }

        public double XMin
        {
            get { return _xMin; }
            set { _xMin = value; }
        }

        public double XMax
        {
            get { return _xMax; }
            set { _xMax = value; }
        }

        public double YMin
        {
            get { return _yMin; }
            set { _yMin = value; }
        }

        public double YMax
        {
            get { return _yMax; }
            set { _yMax = value; }
        }

        public Point NormalizePoint(Point point)
        {
            Point result = new Point();

            if(ChartCanvas.Width.ToString() == "NaN")
            {
                ChartCanvas.Width = 270;
            }
            if (ChartCanvas.Height.ToString() == "NaN")
            {
                ChartCanvas.Height = 250;
            }
            result.X = (point.X - XMin) * ChartCanvas.Width / (XMax - XMin);
            result.Y = ChartCanvas.Height - (point.Y - YMin) * ChartCanvas.Height / (YMax - YMin);

            return result;
        }

    }
}
