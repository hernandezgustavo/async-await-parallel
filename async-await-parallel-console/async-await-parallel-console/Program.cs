using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace async_await_parallel_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ExecuteSync() 1st");
            ExecuteSync();
            Console.WriteLine("\nExecuteSync() 2nd to avoid cache");
            ExecuteSync();

            Console.WriteLine("\nExecuteAsync()");
            ExecuteAsync().GetAwaiter().GetResult();


            Console.WriteLine("\nExecuteParallelAsync()");
            ExecuteParallelAsync().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        public static void ExecuteSync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Total execution time: {elapsedMs}");
        }

        private static void RunDownloadSync()
        {
            List<string> websites = PrepareData();

            foreach (var website in websites)
            {
                WebsiteDataModel results = DownloadWebsite(website);
                ReportWebSiteInfo(results);
            }
        }

        public static async Task ExecuteAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Total execution time: {elapsedMs}");
        }

        private static async Task RunDownloadAsync()
        {
            List<string> websites = PrepareData();

            foreach (var website in websites)
            {
                WebsiteDataModel results = await Task.Run(() => DownloadWebsite(website));
                ReportWebSiteInfo(results);
            }
        }

        public static async Task ExecuteParallelAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Total execution time: {elapsedMs}");
        }

        private static async Task RunDownloadParallelAsync()
        {
            List<string> websites = PrepareData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (var website in websites)
            {
                tasks.Add(Task.Run(() => DownloadWebsite(website)));
            }
            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                ReportWebSiteInfo(result);
            }
        }

        private static List<string> PrepareData()
        {
            List<string> output = new List<string>();
            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.w3schools.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://www.elpais.com.uy");
            return output;
        }

        private static void ReportWebSiteInfo(WebsiteDataModel data)
        {
            if (data == null)
            {
                return;
            }
            Console.WriteLine($"{ data.WebsiteUrl, 25 } \tdownloaded: { data.WebsiteData.Length, 10 } characters long.");
        }

        private static WebsiteDataModel DownloadWebsite(string website)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = website;
            output.WebsiteData = client.DownloadString(website);

            return output;
        }
    }
    
}
