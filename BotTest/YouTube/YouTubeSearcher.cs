using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTest.YouTube
{
    internal class YouTubeSearcher
    {
        public List<(string Title, string Url)> SearchVideos(string query, int resultCount = 5)
        {
            var videoResults = new List<(string Title, string Url)>();

            // ChromeDriver 설정
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // 헤드리스 모드 (필요 없으면 제거)
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-webgl"); // WebGL 경고 제거
            options.AddArgument("--log-level=3");  // 로그 레벨 최소화

            using (var driver = new ChromeDriver(options))
            {
                // YouTube 검색 URL
                string searchUrl = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(query)}";
                driver.Navigate().GoToUrl(searchUrl);

                // 페이지 로드 대기
                Task.Delay(1000).Wait();

                // 동영상 요소 찾기
                var videoElements = driver.FindElements(By.CssSelector("a#video-title"));
                foreach (var element in videoElements)
                {
                    string title = element.GetDomAttribute("title"); // 제목 가져오기
                    string url = "https://www.youtube.com" + element.GetDomAttribute("href"); // URL 가져오기

                    videoResults.Add((title, url));

                    if (videoResults.Count >= resultCount)
                        break;
                }
            }

            return videoResults;
        }

        public void YoutubeMusic(string song)
        {
            var videoResults = SearchVideos(song);

            if (videoResults.Count > 0)
            {
                Console.WriteLine("Top 5 Results:");
                for (int i = 0; i < videoResults.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {videoResults[i].Title} - {videoResults[i].Url}");
                }

                Console.WriteLine("Enter the number of the video to open (1-5):");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= videoResults.Count)
                {
                    string selectedUrl = videoResults[choice - 1].Url;
                    Console.WriteLine($"Opening: {selectedUrl}");

                    // 브라우저에서 열기
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = selectedUrl,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
            else
            {
                Console.WriteLine("No videos found.");
            }
        }
    }
}