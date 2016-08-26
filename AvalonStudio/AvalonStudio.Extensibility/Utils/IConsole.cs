namespace AvalonStudio.Utils
{
	public interface IConsole
	{
		void WriteLine(string data);

		void WriteLine();

		void OverWrite(string data);

		void Write(string data);

		void Write(char data);

        char ReadKey();

        string ReadLine();

		void Clear();
	}
}