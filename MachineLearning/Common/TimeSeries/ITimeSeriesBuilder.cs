namespace MachineLearning.Common.TimeSeries;

public interface ITimeSeriesBuilder
{
  ITimeSeriesBuilder Load();
  ITimeSeriesBuilder Train();
  ITimeSeriesBuilder Evaluate();
  ITimeSeriesBuilder CheckPoint();
  void Predict();
}