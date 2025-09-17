using ContosoPets.Infrastructure.AssemblyReferences;

namespace ContosoPets.Application.Ports
{
    public interface ILinePrinter : IAssemblyReference
    {
        void PrintLine(string message);
        void PrintLine();
        void Write(string message);
        string? ReadLine();
        void Clear();
        ConsoleKeyInfo ReadKey();

    }
}
