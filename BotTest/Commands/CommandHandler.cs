using Discord.WebSocket;
using Discord;
using BotTest.YouTube;

namespace BotTest.Commands
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly YouTubeSearcher searcher;
        public CommandHandler(DiscordSocketClient _client)
        {
            client = _client;
            searcher = new YouTubeSearcher();
        }

        public void RegisterCommands()
        {
            client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            try
            {
                // 봇의 메시지 또는 문자열이 비어있는 경우 무시
                if (message.Author.IsBot || string.IsNullOrEmpty(message.Content.ToString())) return;

                if (message.Content.StartsWith("http://"))
                {
                    string content = message.Content.Substring(7);
                    await message.Channel.SendMessageAsync(content);
                    return;
                }

                if (message.Content.StartsWith("p "))
                {
                    searcher.YoutubeMusic(message.Content.Substring(2));
                    return;
                }

                switch (message.Content.ToLower())
                {
                    case "!ping":
                        await message.Channel.SendMessageAsync("애옹!");
                        break;
                    case "!hello":
                        await message.Channel.SendMessageAsync($"안녕하세요, {message.Author.Username}님!");
                        break;
                    case "!help":
                        await message.Channel.SendMessageAsync("사용 가능한 명령어:\n!ping - 핑 테스트\n!hello - 인사\n!time - 현재 시간\n!repeat [문자열] - 문자열 반복");
                        break;
                    case "!time":
                        await message.Channel.SendMessageAsync($"현재 서버 시간: {DateTime.Now}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"명령어 처리 중 오류 발생: {ex.Message}");
                await message.Channel.SendMessageAsync("명령어 처리 중 오류가 발생했습니다.");
            }
        }

    }
}
