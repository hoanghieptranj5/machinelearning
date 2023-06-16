using MachineLearning.Common.TimeSeries.Models;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace MachineLearning.Common.TimeSeries;

public class StockPriceTimeSeriesBuilder : ITimeSeriesBuilder
{
  private MLContext _mlContext;
  private IDataView _data;
  private IDataView _testData;
  private SsaForecastingTransformer _forecastingTransformer;

  private string _dataPath;
  private string _testDataPath;
  private string _modelPath;

  public ITimeSeriesBuilder Load()
  {
    _dataPath = "C:\\DotnetProjects\\MachineLearning\\MachineLearning\\FPT.csv";
    _testDataPath = "C:\\DotnetProjects\\MachineLearning\\MachineLearning\\test_FPT.csv";
    _modelPath = "C:\\DotnetProjects\\MachineLearning\\MachineLearning\\predict_stock_model.zip";

    _mlContext = new MLContext();
    _data = _mlContext.Data.LoadFromTextFile<ModelInput>(_dataPath, separatorChar: ',', hasHeader: true);
    _testData = _mlContext.Data.LoadFromTextFile<ModelInput>(_testDataPath, separatorChar: ',', hasHeader: true);

    return this;
  }

  public ITimeSeriesBuilder Train()
  {
    var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
      outputColumnName: "ForecastedValues",
      inputColumnName: "Close",
      windowSize: 7,
      seriesLength: 30,
      trainSize: 365,
      horizon: 7,
      confidenceLevel: 0.95f,
      confidenceLowerBoundColumn: "LowerBoundValues",
      confidenceUpperBoundColumn: "UpperBoundValues");
    
    _forecastingTransformer = forecastingPipeline.Fit(_data);

    return this;
  }

  public ITimeSeriesBuilder Evaluate()
  {
    IDataView predictions = _forecastingTransformer.Transform(_testData);
    IEnumerable<float> actual =
      _mlContext.Data.CreateEnumerable<ModelInput>(_testData, true)
        .Select(observed => observed.Close);
    IEnumerable<float> forecast =
      _mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true)
        .Select(prediction => prediction.ForecastedValues[0]);
    var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);
    var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
    var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error
    Console.WriteLine("Evaluation Metrics");
    Console.WriteLine("---------------------");
    Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
    Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");

    return this;
  }

  public ITimeSeriesBuilder CheckPoint()
  {
    var forecastEngine = _forecastingTransformer.CreateTimeSeriesEngine<ModelInput, ModelOutput>(_mlContext);
    forecastEngine.CheckPoint(_mlContext, _modelPath);

    return this;
  }

  public void Predict()
  {
    var forecastEngine = _forecastingTransformer.CreateTimeSeriesEngine<ModelInput, ModelOutput>(_mlContext);

    ModelOutput forecast = forecastEngine.Predict();
    IEnumerable<string> forecastOutput =
      _mlContext.Data.CreateEnumerable<ModelInput>(_testData, reuseRowObject: false)
        .Take(7)
        .Select((ModelInput rental, int index) =>
        {
          string rentalDate = rental.Date;
          float actualRentals = rental.Close;
          float lowerEstimate = Math.Max(0, forecast.LowerBoundValues[index]);
          float estimate = forecast.ForecastedValues[index];
          float upperEstimate = forecast.UpperBoundValues[index];
          return $"Date: {rentalDate}\n" +
                 $"Actual Rentals: {actualRentals}\n" +
                 $"Lower Estimate: {lowerEstimate}\n" +
                 $"Forecast: {estimate}\n" +
                 $"Upper Estimate: {upperEstimate}\n";
        });
    
    Console.WriteLine("Rental Forecast");
    Console.WriteLine("---------------------");
    foreach (var prediction in forecastOutput)
    {
      Console.WriteLine(prediction);
    }
  }
}