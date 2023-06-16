using MachineLearning.Common.TimeSeries;
using MachineLearning.PredictStock;

public class Program
{
  public static void Main(string[] args)
  {
    var predictor = new StockPricePredictor();
    predictor.SetBuilder(new StockPriceTimeSeriesBuilder());
    
    predictor.Predict();
  }
}