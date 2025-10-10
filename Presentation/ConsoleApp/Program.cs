using ContosoPets.Domain.Constants;
using ContosoPets.Presentation.ConsoleApp.Configuration;

namespace ContosoPets.Presentation.ConsoleApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using var serviceProvider = ServiceContainer.ConfigureServices();                             
                ApplicationRunner.RunApplication(serviceProvider);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(AppConstants.ApplicationStartupErrorFormat, ex.Message));
                Console.WriteLine(AppConstants.ApplicationExitingMessage);
                Environment.ExitCode = 1;
            }
        }        
    }
}