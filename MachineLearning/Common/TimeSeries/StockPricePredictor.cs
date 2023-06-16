namespace MachineLearning.Common.TimeSeries;

public class StockPricePredictor : ITimeSeriesPredictor
{
  private ITimeSeriesBuilder _builder = new StockPriceTimeSeriesBuilder();
  
  public void SetBuilder(ITimeSeriesBuilder builder)
  {
    _builder = builder;
  }

  public void Predict()
  {
    _builder
      .Load()
      .Train()
      .Evaluate()
      .CheckPoint()
      .Predict();
  }
}