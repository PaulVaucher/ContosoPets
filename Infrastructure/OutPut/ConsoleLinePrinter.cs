using ContosoPets.Application.Ports;

namespace ContosoPets.Infrastructure.Output
{
    public class ConsoleLinePrinter : ILinePrinter
    {
        public void PrintLine(string message)
        {
            Console.WriteLine(message);
        }
        public void PrintLine()
        {
            Console.WriteLine();
        }
        public void Write(string message)
        {
            Console.Write(message);
        }
        public string? ReadLine()
        {
            return Console.ReadLine();
        }
        public void Clear()
        {
            Console.Clear();
        }
        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
    }    
}
