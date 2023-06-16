namespace MachineLearning.Common.TimeSeries;

public interface ITimeSeriesPredictor
{
  void SetBuilder(ITimeSeriesBuilder builder);
  void Predict();
}