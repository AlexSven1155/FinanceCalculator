namespace FinanceCalculator.Constants;

/// <summary>
/// Контанты для расчета ипотеки.
/// </summary>
public static class CreditConstants
{
	/// <summary>
	/// Срок ипотеки в годах
	/// </summary>
	public const decimal Time = 0;

	/// <summary>
	/// Процент ипотеки
	/// </summary>
	public const decimal Percent = 0;

	/// <summary>
	/// Сумма ипотеки
	/// </summary>
	public const decimal Sum = 0;

	/// <summary>
	/// Дата старта.
	/// </summary>
	public static readonly DateTime StartDate;

	/// <summary>
	/// Досрочные платежи.
	/// </summary>
	public static readonly CreditMonthPaymentModel[] SpecialPayments;
}