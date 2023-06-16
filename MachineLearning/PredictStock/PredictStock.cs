using MachineLearning.PredictStock.Models;
using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace MachineLearning.PredictStock;

public class PredictStock
{
  public void DoIt()
  {
    var fileName = "C:\\DotnetProjects\\MachineLearning\\MachineLearning\\FPT.csv";
    var testFileName = "C:\\DotnetProjects\\MachineLearning\\MachineLearning\\test_FPT.csv";
    var modelPath = "C:\\DotnetProjects\\MachineLearning\\MachineLearning\\predict_stock_model.zip";
    
    var mlContext = new MLContext();
    IDataView data = mlContext.Data.LoadFromTextFile<ModelInput>(fileName, separatorChar: ',', hasHeader: true);
    IDataView testData = mlContext.Data.LoadFromTextFile<ModelInput>(testFileName, separatorChar: ',', hasHeader: true);
    
    var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
      outputColumnName: "ForecastedValues",
      inputColumnName: "Close",
      windowSize: 7,
      seriesLength: 30,
      trainSize: 365,
      horizon: 7,
      confidenceLevel: 0.95f,
      confidenceLowerBoundColumn: "LowerBoundValues",
      confidenceUpperBoundColumn: "UpperBoundValues");
    
    SsaForecastingTransformer forecaster = forecastingPipeline.Fit(data);
    
    Evaluate(testData, forecaster, mlContext);
    
    var forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
    forecastEngine.CheckPoint(mlContext, modelPath);
    
    Forecast(testData, 7, forecastEngine, mlContext);
  }
  
  void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
  {
    IDataView predictions = model.Transform(testData);
    IEnumerable<float> actual =
      mlContext.Data.CreateEnumerable<ModelInput>(testData, true)
        .Select(observed => observed.Close);
    IEnumerable<float> forecast =
      mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true)
        .Select(prediction => prediction.ForecastedValues[0]);
    var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);
    var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
    var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error
    Console.WriteLine("Evaluation Metrics");
    Console.WriteLine("---------------------");
    Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
    Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");
  }
  
  void Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecaster, MLContext mlContext)
  {
    ModelOutput forecast = forecaster.Predict();
    IEnumerable<string> forecastOutput =
      mlContext.Data.CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
        .Take(horizon)
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