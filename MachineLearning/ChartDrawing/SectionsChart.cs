using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace MachineLearning.ChartDrawing;

public class SectionsChart
{
  public void Draw()
  {
    var seriesCollection = new SeriesCollection()
    {
      new LineSeries
      {
        Values = new ChartValues<ObservableValue>()
        {
          new ObservableValue(1),
          new ObservableValue(2),
          new ObservableValue(3),
          new ObservableValue(4),
          new ObservableValue(6),
          new ObservableValue(17),
          new ObservableValue(121),
          new ObservableValue(2),
        },
        PointGeometrySize = 0,
        StrokeThickness = 4,
      }
    };
  }
}