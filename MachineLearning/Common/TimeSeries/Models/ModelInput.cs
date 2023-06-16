using Microsoft.ML.Data;

namespace MachineLearning.Common.TimeSeries.Models;

public class ModelInput
{
  [LoadColumn(0)]
  public string Ticker { get; set; }
  [LoadColumn(1)]
  public string Date { get; set; }
  [LoadColumn(2)]
  public float Open { get; set; }
  [LoadColumn(3)]
  public float High { get; set; }
  [LoadColumn(4)]
  public float Low { get; set; }
  [LoadColumn(5)]
  public float Close { get; set; }
  [LoadColumn(6)]
  public long Volume { get; set; }
}