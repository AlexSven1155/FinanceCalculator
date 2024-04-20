namespace FinanceCalculator.Templates;

public static class OnlyCreditTable
{
	public static void Run() => new CreditCalculator().ClearCalculate().RenderTable();
}