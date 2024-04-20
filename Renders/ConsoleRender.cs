namespace FinanceCalculator.Renders;

public static class ConsoleRender
{
	public static void Render(TableModel tableModel)
	{
		var maxRowLength = GetMaxRowLength(tableModel);
		var rowLengthWithoutSeparator = maxRowLength - tableModel.Columns.Length;
		var remainder = rowLengthWithoutSeparator % tableModel.Columns.Length;
		var colLength = rowLengthWithoutSeparator / tableModel.Columns.Length;

		Console.WriteLine("_".PadRight(maxRowLength, '_'));
		RenderArray(tableModel.Columns, colLength, remainder);
		Console.WriteLine("-".PadRight(maxRowLength, '-'));

		foreach (var row in tableModel.Rows)
		{
			if (row is SeparatorModel)
			{
				Console.WriteLine("-".PadRight(maxRowLength, '-'));
				Console.WriteLine("_".PadRight(maxRowLength, '_'));
			}
			else
			{
				RenderArray(row.CellValues.ToArray(), colLength, remainder);
			}
		}

		Console.WriteLine("-".PadRight(maxRowLength, '-'));
	}

	private static void RenderArray(string[] cells, int colLength, int remainder)
	{
		var columns = new List<string>(cells.Length)
		{
			AlignCenter(cells.First(), colLength + remainder)
		};
		columns.AddRange(cells.Skip(1).Select(col => AlignCenter(col, colLength) + ' '));
		Console.WriteLine($"|{string.Join("|", columns)}|");
	}

	private static string AlignCenter(string val, int length)
	{
		var valCenter = val.Length / 2;
		var colCenter = length / 2;
		var leftStr = new string(' ', colCenter - valCenter);
		var rightStr = new string(' ', colCenter - (val.Length - valCenter));
		var result = leftStr + val + rightStr;
		return result;
	}

	private static int GetMaxRowLength(TableModel tableModel)
	{
		var maxColumnLength = tableModel.Columns.Max(e => e.Length);
		var maxValueLength = tableModel.Rows
			.Where(e => e is not SeparatorModel)
			.Max(e => e.CellValues.Max(w => w.Length));
		var max = maxColumnLength >= maxValueLength
			? maxColumnLength
			: maxValueLength;

		var maxRemainder = max % 10;

		if (maxRemainder > 0)
		{
			max += maxRemainder;
		}

		return max * tableModel.Columns.Length;
	}
}