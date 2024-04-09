namespace FinanceCalculator.Templates;

public static class OnlyCreditTable
{
	public static void Run()
	{
		var credCalc = new CreditCalculator();
		var result = credCalc.ClearCalculate();
		credCalc.RenderTable(result);
	}
}