using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineCharts
{
    class ChartStyleGridlines : ChartStyle
    {
        private string _title;
        private string _xLabel;
        private string _yLabel;

        private Canvas _textCanvas;
        private bool _isXGrid = true;
        private bool _isYGrid = true;
        private Brush _gridlineColor = Brushes.LightGray;
        private double _xTick = 1;
        private double _yTick = 0.5;
        private GridlinePatternEnum _gridlinePatern;
        private double _leftOffset = 20;
        private double _bottomOffset = 15;
        private double _rightOffset = 10;
        private Line _gridline = new Line();


        public ChartStyleGridlines()
        {
            _title = "Title";
            _xLabel = "X Axis";
            _yLabel = "Y Axis";
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string XLabel
        {
            get { return _xLabel; }
            set { _xLabel = value; }
        }

        public string YLabel
        {
            get { return _yLabel; }
            set { _yLabel = value; }
        }

        public GridlinePatternEnum GridlinePattern
        {
            get { return _gridlinePatern; }
            set { _gridlinePatern = value; }
        }

        public double XTick
        {
            get { return _xTick; }
            set { _xTick = value; }
        }
        public double YTick
        {
            get { return _yTick; }
            set { _yTick = value; }
        }
        public Brush GridlineColor
        {
            get { return _gridlineColor; }
            set { _gridlineColor = value; }
        }
        public Canvas TextCanvas
        {
            get { return _textCanvas; }
            set { _textCanvas = value; }
        }
        public bool IsXGrid
        {
            get { return _isXGrid; }
            set { _isXGrid = value; }
        }
        public bool IsYGrid
        {
            get { return _isYGrid; }
            set { _isYGrid = value; }
        }

        public void AddChartStyle(TextBlock tbTitle, TextBlock tbXLabel, TextBlock tbYLabel)
        {
            Point pt = new Point();
            Line tick = new Line();
            double offset = 0;
            double dx, dy;
            TextBlock tb = new TextBlock();
            
            // determine right offset:
            tb.Text = XMax.ToString();
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size size = tb.DesiredSize;
            _rightOffset = size.Width / 2 + 2;
            
            // Determine left offset:
            for (dy = YMin; dy <= YMax; dy += YTick)
            {
                pt = NormalizePoint(new Point(XMin, dy));
                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.TextAlignment = TextAlignment.Right;
                tb.Measure(new Size(Double.PositiveInfinity,
                Double.PositiveInfinity));
                size = tb.DesiredSize;
                if (offset < size.Width)
                    offset = size.Width;
            }

            _leftOffset = offset + 5;

            Canvas.SetLeft(ChartCanvas, _leftOffset);
            Canvas.SetBottom(ChartCanvas, _bottomOffset);
            ChartCanvas.Width = Math.Abs(TextCanvas.Width - _leftOffset - _rightOffset);
            ChartCanvas.Height = Math.Abs(TextCanvas.Height - _bottomOffset - size.Height / 2);
            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = ChartCanvas.Width;
            chartRect.Height = ChartCanvas.Height;
            ChartCanvas.Children.Add(chartRect);

            // Create vertical gridlines:
            if (IsYGrid == true)
            {
                for (dx = XMin + XTick; dx < XMax; dx += XTick)
                {
                    _gridline = new Line();
                    AddLinePattern();
                    _gridline.X1 = NormalizePoint(new Point(dx, YMin)).X;
                    _gridline.Y1 = NormalizePoint(new Point(dx, YMin)).Y;
                    _gridline.X2 = NormalizePoint(new Point(dx, YMax)).X;
                    _gridline.Y2 = NormalizePoint(new Point(dx, YMax)).Y;
                    ChartCanvas.Children.Add(_gridline);
                }


            }

            // Create horizontal gridlines:
            if (IsXGrid == true)
            {
                for (dy = YMin + YTick; dy < YMax; dy += YTick)
                {
                    _gridline = new Line();
                    AddLinePattern();
                    _gridline.X1 = NormalizePoint(new Point(XMin, dy)).X;
                    _gridline.Y1 = NormalizePoint(new Point(XMin, dy)).Y;
                    _gridline.X2 = NormalizePoint(new Point(XMax, dy)).X;
                    _gridline.Y2 = NormalizePoint(new Point(XMax, dy)).Y;
                    ChartCanvas.Children.Add(_gridline);
                }
            }

            // Create x-axis tick marks:
            for (dx = XMin; dx <= XMax; dx += _xTick)
            {
                pt = NormalizePoint(new Point(dx, YMin));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X;
                tick.Y2 = pt.Y - 5;
                ChartCanvas.Children.Add(tick);
                tb = new TextBlock();
                tb.Text = dx.ToString();
                tb.Measure(new Size(Double.PositiveInfinity,
                Double.PositiveInfinity));
                size = tb.DesiredSize;
                TextCanvas.Children.Add(tb);
                Canvas.SetLeft(tb, _leftOffset + pt.X - size.Width / 2);
                Canvas.SetTop(tb, pt.Y + 2 + size.Height / 2);
            }

            // Create y-axis tick marks:
            for (dy = YMin; dy <= YMax; dy += YTick)
            {
                pt = NormalizePoint(new Point(XMin, dy));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X + 5;
                tick.Y2 = pt.Y;
                ChartCanvas.Children.Add(tick);
                tb = new TextBlock();
                tb.Text = dy.ToString();

                tb.Measure(new Size(Double.PositiveInfinity,
                Double.PositiveInfinity));
                size = tb.DesiredSize;
                TextCanvas.Children.Add(tb);
                Canvas.SetRight(tb, ChartCanvas.Width + 10);
                Canvas.SetTop(tb, pt.Y);
            }

            // Add title and labels:
            tbTitle.Text = Title;
            tbXLabel.Text = XLabel;
            tbYLabel.Text = YLabel;
            tbXLabel.Margin = new Thickness(_leftOffset + 2, 2, 2, 2);
            tbTitle.Margin = new Thickness(_leftOffset + 2, 2, 2, 2);

        }

        public void AddLinePattern()
        {
            _gridline.Stroke = GridlineColor;
            _gridline.StrokeThickness = 1;
            switch (GridlinePattern)
            {
                case GridlinePatternEnum.Dash:
                    _gridline.StrokeDashArray =
                    new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case GridlinePatternEnum.Dot:
                    _gridline.StrokeDashArray =
                    new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case GridlinePatternEnum.DashDot:
                    _gridline.StrokeDashArray =
                    new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }

        public enum GridlinePatternEnum
        {
            Solid = 1,
            Dash = 2,
            Dot = 3,
            DashDot = 4
        }

    }

}
