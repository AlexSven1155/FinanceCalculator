namespace FinanceCalculator.Renders;

public static class DepositRender
{
	private static readonly string[] _tableColumns =
	[
		"Дата",
		"Сумма накопленний",
		"Потраченная сумма",
		"Сумма процентов"
	];

	private static readonly string[] _summaryColumns =
	[
		"Общая сумма",
		"Сумма без старта",
		"Накопленная сумма",
		"Потраченная сумма",
		"Первая дата",
		"Количество месяцев",
		"Последняя дата"
	];

	public static void RenderTable(this IEnumerable<DepositMonthModel> depositList)
	{
		ConsoleRender.Render(new TableModel(_tableColumns, depositList.Select(deposit => new RowModel(new List<string>
		{
			$"{deposit.IncrementDate:dd-MM-yyyy}", // Дата
			$"{Math.Round(deposit.TotalSum, 2):N}", // Сумма накопленний
			$"{Math.Round(deposit.MonthlyPay, 2):N}", // Потраченная сумма
			$"{Math.Round(deposit.PercentSum, 2)}" // Сумма процентов
		}))));
	}

	public static void RenderSummary(this List<DepositMonthModel> depositList)
	{
		ConsoleRender.Render(new TableModel(_summaryColumns, new RowModel(new List<string>
		{
			$"{Math.Round(depositList.Sum(dep => dep.MonthlySum) + DepositConstants.CurrentSum, 2):N}", // Общая сумма
			$"{Math.Round(depositList.Sum(dep => dep.MonthlySum), 2):N}", // Сумма без старта
			$"{Math.Round(depositList.Sum(dep => dep.PercentSum), 2):N}", // Накопленная сумма
			$"{Math.Round(depositList.Sum(dep => dep.MonthlyPay), 2):N}", // Потраченная сумма
			$"{depositList.First().IncrementDate:dd-MM-yyyy}", // Первая дата
			$"{Math.Round((double)(depositList.Last().IncrementDate - depositList.First().IncrementDate).Days / 30)}", // Количество месяцев
			$"{depositList.Last().IncrementDate:dd-MM-yyyy}" // Последняя дата
		})));
	}
}