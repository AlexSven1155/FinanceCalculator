namespace FinanceCalculator.Utils;

public static class PercentUtils
{
	/// <summary>
	/// Дневной процент в зависимости от высокоскосности года.
	/// </summary>
	/// <param name="currentDate">Текущая дата.</param>
	/// <param name="percent">Годовой процент.</param>
	/// <returns>Дневной процент.</returns>
	public static decimal GetDailyRate(DateTime currentDate, decimal percent)
	{
		return (new DateTime(currentDate.Year, 3, 1) - new DateTime(currentDate.Year, 2, 1)).Days == 28
			? percent / 365 / 100
			: percent / 366 / 100;
	}
}