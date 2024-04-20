namespace FinanceCalculator.Models.RenderModels;

public class TableModel
{
	public string[] Columns { get; set; }
	public IEnumerable<RowModel> Rows { get; set; }

	public TableModel(string[] columns, params RowModel[] rows)
	{
		Columns = columns;
		Rows = rows;
	}

	public TableModel(string[] columns, IEnumerable<RowModel> rows)
	{
		Columns = columns;
		Rows = rows;
	}
}