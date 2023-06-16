namespace MachineLearning.Common.TimeSeries.Models;

public class ModelOutput
{
  public float[] ForecastedValues { get; set; }

  public float[] LowerBoundValues { get; set; }

  public float[] UpperBoundValues { get; set; }
}