using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineCharts
{
    class DataCollection
    {
        private List<DataSeries> _dataList;

        public DataCollection()
        {
            _dataList = new List<DataSeries>();
        }

        public List<DataSeries> DataList
        {
            get { return _dataList; }
            set { _dataList = value; }
        }


        public void AddLines(ChartStyle chartStyle)
        {
            int j = 0;

            foreach (DataSeries dataSeries in DataList)
            {
                if (dataSeries.SeriesName == "Default Name")
                {
                    dataSeries.SeriesName = "DataSeries" + j.ToString();
                }

                dataSeries.AddLinePattern();

                for (int i = 0; i < dataSeries.LineSeries.Points.Count; i++)
                {
                    dataSeries.LineSeries.Points[i] = chartStyle.NormalizePoint(dataSeries.LineSeries.Points[i]);
                }
                chartStyle.ChartCanvas.Children.Add(dataSeries.LineSeries);
            }
        }
    }
}
