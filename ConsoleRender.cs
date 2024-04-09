namespace FinanceCalculator;

public record ConsoleRenderInfo(string[] Columns, IEnumerable<IEnumerable<string>> Rows);

public class ConsoleRender
{
	private readonly int _remainder;
	private readonly int _colLength;
	private readonly int _maxRowLength;
	private readonly ConsoleRenderInfo _consoleRenderInfo;

	public ConsoleRender(ConsoleRenderInfo consoleRenderInfo)
	{
		_consoleRenderInfo = consoleRenderInfo;
		_maxRowLength = GetMaxRowLength();
		var rowLengthWithoutSeparator = _maxRowLength - _consoleRenderInfo.Columns.Length;
		_remainder = rowLengthWithoutSeparator % _consoleRenderInfo.Columns.Length;
		_colLength = rowLengthWithoutSeparator / _consoleRenderInfo.Columns.Length;
	}

	public void Render()
	{
		Console.WriteLine("_".PadRight(_maxRowLength, '_'));
		RenderArray(_consoleRenderInfo.Columns);
		Console.WriteLine("-".PadRight(_maxRowLength, '-'));

		foreach (var row in _consoleRenderInfo.Rows)
		{
			RenderArray(row);
		}

		Console.WriteLine("-".PadRight(_maxRowLength, '-'));
	}

	private void RenderArray(IEnumerable<string> array)
	{
		var strings = array as string[] ?? array.ToArray();
		var columns = new List<string>(strings.Length)
		{
			AlignCenter(strings.First(), _colLength + _remainder)
		};
		columns.AddRange(strings.Skip(1).Select(col => AlignCenter(col, _colLength) + ' '));
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

	private int GetMaxRowLength()
	{
		var maxColumnLength = _consoleRenderInfo.Columns.Max(e => e.Length);
		var maxValueLength = _consoleRenderInfo.Rows.Max(e => e.Max(w => w.Length));
		var max = maxColumnLength >= maxValueLength
			? maxColumnLength 
			: maxValueLength;

		var maxRemainder = max % 10;

		if (maxRemainder > 0)
		{
			max += maxRemainder;
		}

		return max * _consoleRenderInfo.Columns.Length;
	}
}