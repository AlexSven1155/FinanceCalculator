namespace FinanceCalculator.Models;

public class DepositMonthModel
{
	public DateTime IncrementDate { get; set; }
	public decimal PercentSum { get; set; }
	public decimal TotalSum { get; set; }
	public decimal MonthlySum { get; set; }
	public decimal MonthlyPay { get; set; }
}