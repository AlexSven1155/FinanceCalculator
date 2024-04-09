namespace FinanceCalculator.Utils;

public static class FileSaver
{
	private static readonly JsonSerializerOptions _defJsonOptions = new()
	{
		WriteIndented = true
	};

	public static void SaveCalcResult(object obj, string fileName)
	{
		var path = Path.Combine(@"D:\Projects\FinanceCalculator", fileName);
		using var file = new FileStream(path, FileMode.OpenOrCreate);
		using var fileWriter = new StreamWriter(file);
		fileWriter.Write(JsonSerializer.Serialize(obj, _defJsonOptions));
	}
}