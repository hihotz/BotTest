using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

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

        public async Task YoutubeMusicAsync(string song)
        {
            var videoResults = SearchVideos(song);

            if (videoResults.Count > 0)
            {
                Console.WriteLine("Top 5 Results:");
                for (int i = 0; i < videoResults.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {videoResults[i].Title} - {videoResults[i].Url}");
                }

                Console.WriteLine("Enter the number of the video to play (1-5):");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= videoResults.Count)
                {
                    string selectedUrl = videoResults[choice - 1].Url;
                    Console.WriteLine($"Selected: {selectedUrl}");

                    await PlayAudioFromUrl(selectedUrl);
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


        public async Task PlayAudioFromUrl(string url)
        {
            var youtube = new YoutubeClient();

            try
            {
                // URL에서 동영상 ID 추출
                string videoId = ExtractVideoId(url);

                // 스트림 정보 가져오기
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                if (audioStreamInfo != null)
                {
                    Console.WriteLine($"Streaming audio from: {audioStreamInfo.Url}");

                    // 오디오 재생 로직 (시스템의 기본 미디어 플레이어를 사용하여 재생)
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = audioStreamInfo.Url,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("No audio stream found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing audio: {ex.Message}");
            }
        }

        public static string ExtractVideoId(string url)
        {
            Uri uri = new Uri(url);

            // `v` 파라미터 추출
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (query.AllKeys.Contains("v"))
            {
                return query["v"] ?? throw new ArgumentException("YouTube URL does not contain a valid video ID.");
            }

            // 짧은 URL 형태 (https://youtu.be/VIDEO_ID)
            if (uri.Host.Contains("youtu.be"))
            {
                return uri.AbsolutePath.Trim('/') ?? throw new ArgumentException("YouTube URL does not contain a valid video ID.");
            }

            throw new ArgumentException("Invalid YouTube URL.");
        }


    }
}