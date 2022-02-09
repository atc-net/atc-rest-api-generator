using Spectre.Console;

namespace Atc.Rest.ApiGenerator.CLI
{
    public static class ConsoleHelper
    {
        public static void WriteHeader()
            => AnsiConsole.Write(new FigletText("API Generator").Color(Color.CornflowerBlue));
    }
}