using MachineLearning.PredictStock;

public class Program
{
  public static void Main(string[] args)
  {
    var predictor = new PredictStock();
    predictor.DoIt();
  }
}