using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineCharts
{
    public class DataSeries
    {
        private Polyline _lineSeries = new Polyline();
        private Brush _lineColor;
        private double _lineThickness = 1;
        private LinePatternEnum _linePattern;
        private string _seriesName = "Default Name";
        //private Symbols _symbols;

        public DataSeries()
        {
            LineColor = Brushes.Black;
            //_symbols = new Symbols();
        }

        public Brush LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        public Polyline LineSeries
        {
            get { return _lineSeries; }
            set { _lineSeries = value; }
        }

        public double LineThickness
        {
            get { return _lineThickness; }
            set { _lineThickness = value; }
        }

        public LinePatternEnum LinePattern
        {
            get { return _linePattern; }
            set { _linePattern = value; }
        }

        public string SeriesName
        {
            get { return _seriesName; }
            set { _seriesName = value; }
        }

        public void AddLinePattern()
        {
            LineSeries.Stroke = LineColor;
            LineSeries.StrokeThickness = LineThickness;

            switch (LinePattern)
            {
                case LinePatternEnum.Solid:
                    // Default for Polyline
                    break;
                case LinePatternEnum.Dash:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case LinePatternEnum.Dot:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case LinePatternEnum.DashDot:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
                case LinePatternEnum.None:
                    LineSeries.Stroke = Brushes.Transparent;
                    break;
                default:
                    break;
            }
        }

        public enum LinePatternEnum
        {
            Solid = 1,
            Dash = 2,
            Dot = 3,
            DashDot = 4,
            None = 5
        }
    }
}
