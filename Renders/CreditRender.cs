namespace FinanceCalculator.Renders;

public static class CreditRender
{
	private static readonly string[] _summaryColumns =
	[
		"Общ сумма",
		"Первая дата",
		"Прошло месяцев",
		"Осталось месяцев",
		"Последняя дата",
		"Выплачено процентов",
		"Сумма вычета по %"
	];

	private static readonly string[] _tableColumns =
	[
		"Сумма",
		"Дата",
		"Долг",
		"Проценты",
		"Оплачен",
		"Остаток кредита",
		"Остаток осн долга",
		"Остаток %",
		"Сумма вычета по %"
	];

	private static readonly string[] _withoutPercentColumns =
	[
		"Первая дата",
		"Прошло месяцев",
		"Осталось месяцев",
		"Последняя дата",
		"Выплачено долга"
	];

	public static void RenderSummary(this List<CreditMonthPaymentModel> payments)
	{
		ConsoleRender.Render(
			new TableModel(_summaryColumns, new RowModel(
					new List<string>
					{
						$"{Math.Round(payments.Sum(pay => pay.PaySum), 2):N}", // Общая выплаченная сумма
						$"{payments.First().PayDate:dd-MM-yyyy}", // Первая дата
						$"{Math.Round((double)(DateTime.Now - payments.First().PayDate).Days / 30)}", // Прошло месяцев
						$"{Math.Round((double)(payments.Last().PayDate - DateTime.Now).Days / 30)}", // Осталось месяцев
						$"{payments.Last().PayDate:dd-MM-yyyy}", // Последняя дата
						$"{Math.Round(payments.Sum(pay => pay.PercentSum), 2):N}", // Выплачено процентов
						$"{Math.Round(payments.Sum(pay => pay.PercentSum) * 0.13m, 2):N}" // Сумма вычета по процентам
					}
				)
			));
	}

	public static void RenderTable(this List<CreditMonthPaymentModel> payments)
	{
		var tempPercentSum = 0m;
		var currYear = payments.First().PayDate.Year;
		var rows = new List<RowModel>();

		foreach (var payment in payments)
		{
			if (currYear != payment.PayDate.Year)
			{
				currYear = payment.PayDate.Year;
				rows.Add(new SeparatorModel());
			}

			tempPercentSum += payment.PercentSum;
			var percentTaxDeduction = tempPercentSum * 0.13m;

			rows.Add(new RowModel(new List<string>
			{
				$"{payment.PaySum:N}", // Сумма
				$"{payment.PayDate:dd-MM-yyyy}", // Дата
				$"{Math.Round(payment.DebtSum, 2):N}", // Долг
				$"{Math.Round(payment.PercentSum, 2):N}", // Проценты
				$"{(payment.IsPay ? "+" : "-")}", // Оплачен
				$"{Math.Round(payment.TotalRemainder, 2):N}", // Остаток кредита
				$"{Math.Round(payment.DebtRemainder, 2):N}", // Остаток основного долга
				$"{Math.Round(payment.PercentRemainder, 2):N}", // Остаток процентов
				$"{Math.Round(percentTaxDeduction, 2):N}" // Сумма вычета по процентам
			}));
		}

		ConsoleRender.Render(new TableModel(_tableColumns, rows));
		Console.WriteLine($"Итого сумма процентов: {Math.Round(tempPercentSum, 2):N}");
		Console.WriteLine($"Итого сумма ипотеки: {Math.Round(payments.First().TotalRemainder, 2):N}");
	}

	public static void RenderWithoutPercent(this List<CreditMonthPaymentModel> payments)
	{
		ConsoleRender.Render(
			new TableModel(_withoutPercentColumns, new RowModel(
					new List<string>
					{
						$"{payments.First().PayDate:dd-MM-yyyy}", // Первая дата
						$"{Math.Round((double)(DateTime.Now - payments.First().PayDate).Days / 30)}", // Прошло месяцев
						$"{Math.Round((double)(payments.Last().PayDate - DateTime.Now).Days / 30)}", // Осталось месяцев
						$"{payments.Last().PayDate:dd-MM-yyyy}", // Последняя дата
						$"{Math.Round(payments.Sum(pay => pay.DebtSum), 2):N}" // Выплачено долга
					}
				)
			));
	}
}