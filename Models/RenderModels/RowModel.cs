namespace FinanceCalculator.Models.RenderModels;

public class RowModel
{
	public IEnumerable<string> CellValues { get; init; }

	public RowModel() { }

	public RowModel(IEnumerable<string> cellValues)
	{
		CellValues = cellValues;
	}
}