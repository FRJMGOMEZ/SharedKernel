using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace BankAccounts.Api
{
    /// <summary> Program start. </summary>
    public class Program
    {
        /// <summary> Main </summary>
        [ExcludeFromCodeCoverage]
        public static async Task<int> Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
            return 0;
        }

        /// <summary> CreateHostBuilder </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
