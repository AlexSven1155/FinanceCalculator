namespace FinanceCalculator.Calculators;

/// <summary>
/// Калькулятор ипотеки.
/// </summary>
public class CreditCalculator
{
	/// <summary>
	/// Дата на момент рассчетов.
	/// </summary>
	private readonly DateTime _startDate = new(2024, 4, 5);

	/// <summary>
	/// Размер ежемесячного платежа.
	/// </summary>
	public readonly decimal EveryMonthPayment;

	/// <inheritdoc cref="CreditCalculator"/>
	public CreditCalculator()
	{
		const decimal monthlyRate = CreditConstants.Percent / 12 / 100;
		var totalRate = (decimal)Math.Pow(1 + (double)monthlyRate, (double)CreditConstants.Time * 12);
		EveryMonthPayment = Math.Round(CreditConstants.Sum * monthlyRate * totalRate / (totalRate - 1m), 2);
	}

	/// <summary>
	/// Рассчет графика платежей с учетом досрочных платежей <see cref="CreditConstants.SpecialPayments"/>.
	/// </summary>
	/// <returns>Коллекция платежей по ипотеке.</returns>
	public List<CreditMonthPaymentModel> CalculateDefault()
	{
		var payments = new List<CreditMonthPaymentModel>();

		var totalPercentSum = 0m;
		var totalDebt = CreditConstants.Sum;
		var currentDate = CreditConstants.StartDate;

		do
		{
			var percentSum = 0m;
			var gotSpecPay = false;

			do
			{
				var dailyRate = GetDailyRate(currentDate);
				var specPay = GetSpecialPayment(currentDate);

				if (specPay != null)
				{
					gotSpecPay = true;
					payments.Add(specPay);
					totalDebt -= specPay.DebtSum = specPay.PaySum - percentSum;
					totalPercentSum += specPay.PercentSum = percentSum;
					percentSum = 0m;
				}

				if (totalDebt <= 0)
				{
					break;
				}

				percentSum += totalDebt * dailyRate;
				currentDate = currentDate.AddDays(1);
			} while (currentDate.Day != CreditConstants.StartDate.Day);

			var newPayment = new CreditMonthPaymentModel();
			newPayment.PayDate = currentDate;
			newPayment.DebtSum = gotSpecPay ? 0m : EveryMonthPayment - percentSum;

			if (totalDebt - newPayment.DebtSum < 0)
			{
				newPayment.DebtSum -= newPayment.DebtSum - totalDebt;
			}
			else
			{
				totalDebt -= newPayment.DebtSum;
			}

			totalPercentSum += newPayment.PercentSum = percentSum;
			newPayment.PaySum = newPayment.DebtSum + newPayment.PercentSum;
			payments.Add(newPayment);
		} while (payments.Sum(pay => pay.DebtSum) < CreditConstants.Sum);

		var finalCreditSum = totalPercentSum + CreditConstants.Sum;
		var tempCreditSum = 0m;
		var tempDebtSum = 0m;
		var tempPercentSum = 0m;

		foreach (var payment in payments)
		{
			tempDebtSum += payment.DebtSum;
			tempPercentSum += payment.PercentSum;
			tempCreditSum += payment.DebtSum + payment.PercentSum;
			payment.IsPay = DateTime.Now >= payment.PayDate;

			payment.TotalRemainder = finalCreditSum - tempCreditSum;
			payment.DebtRemainder = CreditConstants.Sum - tempDebtSum;
			payment.PercentRemainder = totalPercentSum - tempPercentSum;
		}

		return payments;
	}

	/// <summary>
	/// Рассчет при условии что в день выплаты будет досрочное погашение в счет сокращения срока.
	/// </summary>
	/// <param name="overPay">Размер суммы досрочного погашения.</param>
	/// <returns>Коллекция платежей по ипотеке.</returns>
	public List<CreditMonthPaymentModel> CalculateWithOverPay(decimal overPay)
	{
		var payments = new List<CreditMonthPaymentModel>();

		var totalPercentSum = 0m;
		var totalDebt = CreditConstants.Sum;
		var currentDate = CreditConstants.StartDate;

		do
		{
			var percentSum = 0m;
			var gotSpecPay = false;

			do
			{
				var dailyRate = GetDailyRate(currentDate);
				var specPay = GetSpecialPayment(currentDate);

				if (specPay != null)
				{
					gotSpecPay = true;
					payments.Add(specPay);
					totalDebt -= specPay.DebtSum = specPay.PaySum - percentSum;
					totalPercentSum += specPay.PercentSum = percentSum;
					percentSum = 0m;
				}

				if (totalDebt <= 0)
				{
					break;
				}

				percentSum += totalDebt * dailyRate;
				currentDate = currentDate.AddDays(1);
			} while (currentDate.Day != CreditConstants.StartDate.Day);

			var newPayment = new CreditMonthPaymentModel();
			newPayment.PayDate = currentDate;
			newPayment.DebtSum = gotSpecPay ? 0m : EveryMonthPayment - percentSum;

			if (totalDebt - newPayment.DebtSum < 0)
			{
				newPayment.DebtSum -= newPayment.DebtSum - totalDebt;
				totalDebt = 0;
			}
			else
			{
				totalDebt -= newPayment.DebtSum;
			}

			totalPercentSum += newPayment.PercentSum = percentSum;
			newPayment.PaySum = newPayment.DebtSum + newPayment.PercentSum;
			payments.Add(newPayment);

			if (currentDate >= _startDate && totalDebt > 0)
			{
				var newOverPayment = new CreditMonthPaymentModel
				{
					PayDate = currentDate,
					DebtSum = overPay,
					PaySum = overPay
				};
				payments.Add(newOverPayment);

				if (totalDebt - overPay < 0)
				{
					newOverPayment.DebtSum = totalDebt;
					newOverPayment.PaySum = totalDebt;
					totalDebt = 0;
				}
				else
				{
					totalDebt -= overPay;
				}
			}
		} while (payments.Sum(pay => pay.DebtSum) < CreditConstants.Sum);

		var finalCreditSum = totalPercentSum + CreditConstants.Sum;
		var tempCreditSum = 0m;
		var tempDebtSum = 0m;
		var tempPercentSum = 0m;

		foreach (var payment in payments)
		{
			tempDebtSum += payment.DebtSum;
			tempPercentSum += payment.PercentSum;
			tempCreditSum += payment.DebtSum + payment.PercentSum;
			payment.IsPay = DateTime.Now >= payment.PayDate;

			payment.TotalRemainder = finalCreditSum - tempCreditSum;
			payment.DebtRemainder = CreditConstants.Sum - tempDebtSum;
			payment.PercentRemainder = totalPercentSum - tempPercentSum;
		}

		return payments;
	}

	/// <summary>
	/// Рассчет чистый, без досрочных платежей.
	/// </summary>
	/// <returns>Коллекция платежей по ипотеке.</returns>
	public List<CreditMonthPaymentModel> ClearCalculate()
	{
		var payments = new List<CreditMonthPaymentModel>();

		var totalPercentSum = 0m;
		var totalDebt = CreditConstants.Sum;
		var currentDate = CreditConstants.StartDate;

		do
		{
			var percentSum = 0m;

			do
			{
				var dailyRate = GetDailyRate(currentDate);
				percentSum += totalDebt * dailyRate;
				currentDate = currentDate.AddDays(1);
			} while (currentDate.Day != CreditConstants.StartDate.Day || totalDebt == 0);

			var newPayment = new CreditMonthPaymentModel();
			newPayment.PayDate = currentDate;

			if (totalDebt - (EveryMonthPayment - percentSum) < 0)
			{
				newPayment.DebtSum -= newPayment.DebtSum - totalDebt;
			}
			else
			{
				newPayment.DebtSum = EveryMonthPayment - percentSum;
				totalDebt -= newPayment.DebtSum;
			}

			totalPercentSum += newPayment.PercentSum = percentSum;
			newPayment.PaySum = newPayment.DebtSum + newPayment.PercentSum;
			payments.Add(newPayment);
		} while (payments.Sum(pay => pay.DebtSum) < CreditConstants.Sum);

		var tempDebtSum = 0m;
		var tempCreditSum = 0m;
		var tempPercentSum = 0m;
		var finalCreditSum = totalPercentSum + CreditConstants.Sum;

		foreach (var payment in payments)
		{
			tempDebtSum += payment.DebtSum;
			tempPercentSum += payment.PercentSum;
			payment.IsPay = DateTime.Now >= payment.PayDate;
			tempCreditSum += payment.DebtSum + payment.PercentSum;
			payment.TotalRemainder = finalCreditSum - tempCreditSum;
			payment.DebtRemainder = CreditConstants.Sum - tempDebtSum;
			payment.PercentRemainder = totalPercentSum - tempPercentSum;
		}

		return payments;
	}

	/// <summary>
	/// Рассчет на опреденное количество месяцев. Отсчет начинается с даты <see cref="_startDate"/>.
	/// </summary>
	/// <param name="monthCount">Количество месяцев.</param>
	/// <returns>Коллекция платежей по ипотеке.</returns>
	public List<CreditMonthPaymentModel> CalculatePeriod(int monthCount)
	{
		var payments = new List<CreditMonthPaymentModel>();

		var totalPercentSum = 0m;
		var totalDebt = CreditConstants.Sum;
		var currentDate = CreditConstants.StartDate;

		do
		{
			var percentSum = 0m;
			var gotSpecPay = false;

			do
			{
				var dailyRate = GetDailyRate(currentDate);
				var specPay = GetSpecialPayment(currentDate);

				if (specPay != null)
				{
					gotSpecPay = true;
					payments.Add(specPay);
					totalDebt -= specPay.DebtSum = specPay.PaySum - percentSum;
					totalPercentSum += specPay.PercentSum = percentSum;
					percentSum = 0m;
				}

				if (totalDebt <= 0)
				{
					break;
				}

				percentSum += totalDebt * dailyRate;
				currentDate = currentDate.AddDays(1);
			} while (currentDate.Day != CreditConstants.StartDate.Day);

			var newPayment = new CreditMonthPaymentModel();
			newPayment.PayDate = currentDate;
			newPayment.DebtSum = gotSpecPay ? 0m : EveryMonthPayment - percentSum;

			if (totalDebt - newPayment.DebtSum < 0)
			{
				newPayment.DebtSum -= newPayment.DebtSum - totalDebt;
			}
			else
			{
				totalDebt -= newPayment.DebtSum;
			}

			totalPercentSum += newPayment.PercentSum = percentSum;
			newPayment.PaySum = newPayment.DebtSum + newPayment.PercentSum;
			payments.Add(newPayment);

			if (currentDate > _startDate)
			{
				monthCount--;
			}
		} while (payments.Sum(pay => pay.DebtSum) < CreditConstants.Sum && monthCount >= 0);

		var finalCreditSum = totalPercentSum + CreditConstants.Sum;
		var tempCreditSum = 0m;
		var tempDebtSum = 0m;
		var tempPercentSum = 0m;

		foreach (var payment in payments)
		{
			tempDebtSum += payment.DebtSum;
			tempPercentSum += payment.PercentSum;
			tempCreditSum += payment.DebtSum + payment.PercentSum;
			payment.IsPay = DateTime.Now >= payment.PayDate;

			payment.TotalRemainder = finalCreditSum - tempCreditSum;
			payment.DebtRemainder = CreditConstants.Sum - tempDebtSum;
			payment.PercentRemainder = totalPercentSum - tempPercentSum;
		}

		return payments;
	}

	public void RenderSummary(List<CreditMonthPaymentModel> payments)
	{
		var rows = new List<List<string>>();
		var consoleRenderInfo = new ConsoleRenderInfo([
			"Общ сумма",
			"Первая дата",
			"Кол-во месяцев",
			"Кол-во месяцев с тек даты",
			"Последняя дата",
			"Выплачено долга",
			"Выплачено процентов",
			"Сумма вычета по процентам",
			"Остаток основного долга"
		], rows);

		var row = new List<string>();
		row.Add($"{Math.Round(payments.Sum(pay => pay.PaySum), 2):N}"); // Общая выплаченная сумма
		row.Add($"{payments.First().PayDate:dd-MM-yyyy}"); // Первая дата
		row.Add($"{Math.Round((double)(payments.Last().PayDate - payments.First().PayDate).Days / 30)}"); // Количество месяцев
		row.Add($"{Math.Round((double)(payments.Last().PayDate - _startDate).Days / 30)}"); // Кол-во месяцев с тек даты
		row.Add($"{payments.Last().PayDate:dd-MM-yyyy}"); // Последняя дата
		row.Add($"{Math.Round(payments.Sum(pay => pay.DebtSum), 2):N}"); // Выплачено долга
		row.Add($"{Math.Round(payments.Sum(pay => pay.PercentSum), 2):N}"); // Выплачено процентов
		row.Add($"{Math.Round(payments.Sum(pay => pay.PercentSum) * 0.13m, 2):N}"); // Сумма вычета по процентам
		row.Add($"{Math.Round(CreditConstants.Sum - payments.Sum(pay => pay.DebtSum), 2):N}"); // Остаток основного долга
		rows.Add(row);

		new ConsoleRender(consoleRenderInfo).Render();
	}

	public void RenderTable(List<CreditMonthPaymentModel> payments)
	{
		var rows = new List<List<string>>();
		var consoleRenderInfo = new ConsoleRenderInfo([
			"Сумма",
			"Дата",
			"Долг",
			"Проценты",
			"Оплачен",
			"Остаток кредита",
			"Остаток основного долга",
			"Остаток процентов",
			"Сумма вычета по процентам"
		], rows);

		var tempPercentSum = 0m;

		foreach (var payment in payments)
		{
			var row = new List<string>();
			tempPercentSum += payment.PercentSum;
			var percentTaxDeduction = tempPercentSum * 0.13m;

			row.Add($"{payment.PaySum:N}"); // Сумма
			row.Add($"{payment.PayDate:dd-MM-yyyy}"); // Дата
			row.Add($"{Math.Round(payment.DebtSum, 2):N}"); // Долг
			row.Add($"{Math.Round(payment.PercentSum, 2):N}"); // Проценты
			row.Add($"{(payment.IsPay ? "+" : "-")}"); // Оплачен
			row.Add($"{Math.Round(payment.TotalRemainder, 2):N}"); // Остаток кредита
			row.Add($"{Math.Round(payment.DebtRemainder, 2):N}"); // Остаток основного долга
			row.Add($"{Math.Round(payment.PercentRemainder, 2):N}"); // Остаток процентов
			row.Add($"{Math.Round(percentTaxDeduction, 2):N}"); // Сумма вычета по процентам
			rows.Add(row);
		}

		new ConsoleRender(consoleRenderInfo).Render();
		Console.WriteLine();
		Console.WriteLine($"Итого сумма процентов: {Math.Round(tempPercentSum, 2):N}");
		Console.WriteLine($"Итого сумма ипотеки: {Math.Round(payments.First().TotalRemainder, 2):N}");
	}

	/// <summary>
	/// Возвращает первый досрочный платеж следующий на указанной датой в том же месяце.
	/// </summary>
	/// <param name="currentDate">Текущая дата.</param>
	/// <returns>Досрочный платёж.</returns>
	private static CreditMonthPaymentModel GetSpecialPayment(DateTime currentDate) => CreditConstants.SpecialPayments
		.FirstOrDefault(specialPayment => specialPayment.PayDate == currentDate);

	/// <summary>
	/// Дневной процент в зависимости от высокоскосности года.
	/// </summary>
	/// <param name="currentDate">Текущая дата.</param>
	/// <returns>Дневной процент.</returns>
	private static decimal GetDailyRate(DateTime currentDate)
	{
		return (new DateTime(currentDate.Year, 3, 1) - new DateTime(currentDate.Year, 2, 1)).Days == 28
			? CreditConstants.Percent / 365 / 100
			: CreditConstants.Percent / 366 / 100;
	}
}