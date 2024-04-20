namespace FinanceCalculator.Templates;

public static class DiffOverCreditVsDeposit
{
	public static void Run()
	{
		const decimal paySum = 120_000;

		try
		{
			Console.WriteLine($"Ипотека: {CreditConstants.Sum:N}");
			var credCalc = new CreditCalculator();
			Console.WriteLine("Если не гасить досрочно:");
			var defCredResult = credCalc.CalculateDefault();
			defCredResult.RenderSummary();
			Console.WriteLine("Если гасить досрочно:");
			Console.WriteLine($"Ежемесячный расход: {paySum:N}");
			credCalc.CalculateWithOverPay(paySum).RenderSummary();
			var depCalc = new DepositCalculator();
			Console.WriteLine($"Если класть на накопительный под {DepositConstants.Percent:N}%, то вот за какой период будет накоплено для погашения ипотеки:");
			Console.WriteLine($"Стартовая сумма {DepositConstants.CurrentSum:N}");
			depCalc.CalculateWithCreditResult(paySum, defCredResult).RenderSummary();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}
}