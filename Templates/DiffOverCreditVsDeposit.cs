using System.Globalization;

namespace FinanceCalculator.Templates;

public static class DiffOverCreditVsDeposit
{
	public static void Run()
	{
		const decimal paySum = 120_000;

		try
		{
			Console.WriteLine($"Ежемесячный расход: {paySum}");
			var credCalc = new CreditCalculator();
			var depCalc = new DepositCalculator();
			var defCreditPayments = credCalc.CalculateDefault();
			Console.WriteLine("График платежей по ипотеке:");
			credCalc.RenderTable(defCreditPayments);
			Console.Write("Рассчитаем вклад, целевая сумма: ");
			var targetDepSum = Convert.ToDecimal(Console.ReadLine(), CultureInfo.InvariantCulture);
			var depResult = depCalc.Calculate(paySum, targetDepSum);
			Console.WriteLine($"Стартовая сумма вклада: {DepositInfo.CurrentSum:N}");
			depCalc.RenderSummary(depResult);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}
}