namespace FinanceCalculator.Models;

/// <summary>
/// Данные платежа по ипотеке.
/// </summary>
public class CreditMonthPaymentModel
{
	private decimal _paySum;
	private decimal _debtSum;
	private decimal _percentSum;
	private decimal _totalRemainder;
	private decimal _debtRemainder;
	private decimal _percentRemainder;

	/// <summary>
	/// Оплачен.
	/// </summary>
	public bool IsPay { get; set; }

	/// <summary>
	/// Сумма оплаты.
	/// </summary>
	public decimal PaySum
	{
		get => _paySum;
		set => _paySum = Math.Round(value, 2);
	}

	/// <summary>
	/// Дата отплаты.
	/// </summary>
	public DateTime PayDate { get; set; }

	/// <summary>
	/// Сумма погашения основного долга.
	/// </summary>
	public decimal DebtSum
	{
		get => _debtSum;
		set => _debtSum = Math.Round(value, 2);
	}

	/// <summary>
	/// Сумма погашенения процентов.
	/// </summary>
	public decimal PercentSum
	{
		get => _percentSum;
		set => _percentSum = Math.Round(value, 2);
	}

	/// <summary>
	/// Общая задолженность.
	/// </summary>
	public decimal TotalRemainder
	{
		get => _totalRemainder;
		set => _totalRemainder = Math.Round(value, 2);
	}

	/// <summary>
	/// Остаток основного долга.
	/// </summary>
	public decimal DebtRemainder
	{
		get => _debtRemainder;
		set => _debtRemainder = Math.Round(value, 2);
	}

	/// <summary>
	/// Остаток долга по процентам.
	/// </summary>
	public decimal PercentRemainder
	{
		get => _percentRemainder;
		set => _percentRemainder = Math.Round(value, 2);
	}
}