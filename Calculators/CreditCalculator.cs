using FinanceCalculator.Utils;

namespace FinanceCalculator.Calculators;

/// <summary>
/// Калькулятор ипотеки.
/// </summary>
public class CreditCalculator
{
	/// <summary>
	/// Размер ежемесячного платежа.
	/// </summary>
	private readonly decimal _everyMonthPayment;

	/// <inheritdoc cref="CreditCalculator"/>
	public CreditCalculator()
	{
		const decimal monthlyRate = CreditConstants.Percent / 12 / 100;
		var totalRate = (decimal)Math.Pow(1 + (double)monthlyRate, (double)CreditConstants.Time * 12);
		_everyMonthPayment = Math.Round(CreditConstants.Sum * monthlyRate * totalRate / (totalRate - 1m), 2);
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
				var dailyRate = PercentUtils.GetDailyRate(currentDate, CreditConstants.Percent);
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
			newPayment.DebtSum = gotSpecPay ? 0m : _everyMonthPayment - percentSum;

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
				var dailyRate = PercentUtils.GetDailyRate(currentDate, CreditConstants.Percent);
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
			newPayment.DebtSum = gotSpecPay ? 0m : _everyMonthPayment - percentSum;

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

			if (currentDate >= DateTime.Now && totalDebt > 0)
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
				var dailyRate = PercentUtils.GetDailyRate(currentDate, CreditConstants.Percent);
				percentSum += totalDebt * dailyRate;
				currentDate = currentDate.AddDays(1);
			} while (currentDate.Day != CreditConstants.StartDate.Day || totalDebt == 0);

			var newPayment = new CreditMonthPaymentModel();
			newPayment.PayDate = currentDate;

			if (totalDebt - (_everyMonthPayment - percentSum) < 0)
			{
				newPayment.DebtSum -= newPayment.DebtSum - totalDebt;
			}
			else
			{
				newPayment.DebtSum = _everyMonthPayment - percentSum;
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
	/// Рассчет на опреденное количество месяцев. Отсчет начинается с текущей даты.
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
				var dailyRate = PercentUtils.GetDailyRate(currentDate, CreditConstants.Percent);
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
			newPayment.DebtSum = gotSpecPay ? 0m : _everyMonthPayment - percentSum;

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

			if (currentDate > DateTime.Now)
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

	/// <summary>
	/// Возвращает первый досрочный платеж следующий на указанной датой в том же месяце.
	/// </summary>
	/// <param name="currentDate">Текущая дата.</param>
	/// <returns>Досрочный платёж.</returns>
	private static CreditMonthPaymentModel GetSpecialPayment(DateTime currentDate) => CreditConstants.SpecialPayments
		.FirstOrDefault(specialPayment => specialPayment.PayDate == currentDate);
}