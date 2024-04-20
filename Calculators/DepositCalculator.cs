namespace FinanceCalculator.Calculators;

/// <summary>
/// Калькулятор для расчета вклада.
/// </summary>
public class DepositCalculator
{
	/// <summary>
	/// Дата начала рассчетов.
	/// </summary>
	private readonly DateTime _startDate;

	/// <inheritdoc cref="DepositCalculator"/>
	public DepositCalculator()
	{
		var currDate = DateTime.Now;
		_startDate = new DateTime(currDate.Year, currDate.Month, DepositConstants.IncrementDay);
	}

	/// <summary>
	/// Делает рассчет за указанный период.
	/// </summary>
	/// <param name="monthlyPay">Ежемесячная сумма пополнения вклада.</param>
	/// <param name="monthCount">Срок вклада в месяцах.</param>
	/// <returns>Результат рассчета.</returns>
	public List<DepositMonthModel> CalculateWithPeriod(decimal monthlyPay, int monthCount)
	{
		var result = new List<DepositMonthModel>();
		var totalSum = DepositConstants.CurrentSum;
		var currentDate = _startDate;
		var currMonthCount = 0;

		while (result.Count == 0 || ++currMonthCount <= monthCount)
		{
			var monthlyPercentSum = 0m;
			currentDate = currentDate.AddDays(1);

			while (currentDate.Day != DepositConstants.IncrementDay)
			{
				var dailyPercent = PercentUtils.GetDailyRate(currentDate, DepositConstants.Percent);
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

	/// <summary>
	/// Делает рассчет до указанной суммы.
	/// </summary>
	/// <param name="monthlyPay">Ежемесячная сумма пополнения вклада.</param>
	/// <param name="targetSum">Сумма которую требуется накопить.</param>
	/// <returns>Результат рассчета.</returns>
	public List<DepositMonthModel> CalculateWithTargetSum(decimal monthlyPay, decimal targetSum)
	{
		var result = new List<DepositMonthModel>();
		var totalSum = DepositConstants.CurrentSum;
		var currentDate = _startDate;

		while (result.Count == 0 || totalSum < targetSum)
		{
			var monthlyPercentSum = 0m;
			currentDate = currentDate.AddDays(1);

			while (currentDate.Day != DepositConstants.IncrementDay)
			{
				var dailyPercent = PercentUtils.GetDailyRate(currentDate, DepositConstants.Percent);
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

	/// <summary>
	/// Делает рассчет исходя из результата рассчета ипотеки.
	/// Вычисляет до тех пор, пока не накопится достаточно для закрытия основного долга по ипотеке.
	/// </summary>
	/// <param name="monthlyPay">Ежемесячная сумма пополнения вклада.</param>
	/// <param name="creditResult">Результат рассчета ипотеки.</param>
	/// <returns>Результат рассчета.</returns>
	public List<DepositMonthModel> CalculateWithCreditResult(decimal monthlyPay, List<CreditMonthPaymentModel> creditResult)
	{
		var result = new List<DepositMonthModel>();
		var totalSum = DepositConstants.CurrentSum;
		var currentDate = _startDate;

		while (true)
		{
			var monthlyPercentSum = 0m;
			currentDate = currentDate.AddDays(1);

			while (currentDate.Day != DepositConstants.IncrementDay)
			{
				var dailyPercent = PercentUtils.GetDailyRate(currentDate, DepositConstants.Percent);
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

			if (totalSum >= creditResult.FirstOrDefault(pay => pay.PayDate > currentDate)?.DebtRemainder)
			{
				break;
			}
		}

		return result;
	}
}