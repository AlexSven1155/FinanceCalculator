namespace FinanceCalculator.Calculators;

public class DepositCalculator
{
	private readonly DateTime _startDate;

	public List<DepositMonthModel> Calculate(decimal monthlyPay, decimal targetSum)
	{
		var result = new List<DepositMonthModel>();
		var dailyPercent = DepositInfo.Percent / 365;
		var totalSum = DepositInfo.CurrentSum;
		var currentDate = _startDate;

		while (!result.Any() || totalSum < targetSum)
		{
			var monthlyPercentSum = 0m;
			currentDate = currentDate.AddDays(1);

			while (currentDate.Day != DepositInfo.IncrementDay)
			{
				monthlyPercentSum += totalSum * dailyPercent;
				currentDate = currentDate.AddDays(1);
			}

			var monthlySum = monthlyPay + monthlyPercentSum;
			totalSum += monthlySum;

			result.Add(new DepositMonthModel
			{
				PercentSum = monthlyPercentSum,
				IncrementDate = currentDate,
				TotalSum = totalSum,
				MonthlySum = monthlySum,
				MonthlyPay = monthlyPay
			});
		}

		return result;
	}

	public void RenderTable(List<DepositMonthModel> depositList)
	{
		var rows = new List<List<string>>();
		var consoleRenderInfo = new ConsoleRenderInfo([
			"Дата",
			"Сумма накопленний",
			"Потраченная сумма",
			"Сумма процентов"
		], rows);

		foreach (var deposit in depositList)
		{
			var row = new List<string>();
			row.Add($"{deposit.IncrementDate:dd-MM-yyyy}"); // Дата
			row.Add($"{Math.Round(deposit.TotalSum, 2):N}"); // Сумма накопленний
			row.Add($"{Math.Round(deposit.MonthlyPay, 2):N}"); // Потраченная сумма
			row.Add($"{Math.Round(deposit.PercentSum, 2)}"); // Сумма процентов
			rows.Add(row);
		}

		new ConsoleRender(consoleRenderInfo).Render();
	}

	public void RenderSummary(List<DepositMonthModel> depositList)
	{
		var rows = new List<List<string>>();
		var consoleRenderInfo = new ConsoleRenderInfo([
			"Общая накопленная сумма",
			"Потраченная сумма",
			"Первая дата",
			"Количество месяцев",
			"Последняя дата"
		], rows);

		var row = new List<string>();
		row.Add($"{Math.Round(depositList.Sum(dep => dep.MonthlySum) + DepositInfo.CurrentSum, 2):N}"); // Общая накопленная сумма
		row.Add($"{Math.Round(depositList.Sum(dep => dep.MonthlyPay), 2):N}"); // Потраченная сумма
		row.Add($"{depositList.First().IncrementDate:dd-MM-yyyy}"); // Первая дата
		row.Add($"{Math.Round((double)(depositList.Last().IncrementDate - depositList.First().IncrementDate).Days / 30)}"); // Количество месяцев
		row.Add($"{depositList.Last().IncrementDate:dd-MM-yyyy}"); // Последняя дата
		rows.Add(row);

		new ConsoleRender(consoleRenderInfo).Render();
	}
}