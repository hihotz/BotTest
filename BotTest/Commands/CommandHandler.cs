using Discord.WebSocket;
using Discord;
using BotTest.YouTube;
using Discord.Commands;

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

        private Task HandleCommandAsync(SocketMessage message)
        {
            _ = Task.Run(async () =>
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
                        await searcher.YoutubeMusicAsync(message.Content.Substring(2));
                        return;
                    }

                    if (message.Content.StartsWith("!join") || message.Content.StartsWith("!leave"))
                    {
                        var userMessage = message as SocketUserMessage;
                        var context = new SocketCommandContext(client, userMessage);
                        if (message.Content.StartsWith("!join"))
                            await JoinVoiceChannelAsync(context);
                        else
                            await LeaveVoiceChannelAsync(context);
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
            });
            return Task.CompletedTask;
        }

        private async Task JoinVoiceChannelAsync(SocketCommandContext context)
        {
            // 사용자가 음성 채널에 있는지 확인
            var voiceChannel = (context.User as SocketGuildUser)?.VoiceChannel;
            if (voiceChannel == null)
            {
                await context.Channel.SendMessageAsync("음성 채널에 들어가 있지 않습니다.");
                return;
            }

            // 음성 채널에 연결
            try
            {
                var audioClient = await voiceChannel.ConnectAsync();
                await context.Channel.SendMessageAsync($"'{voiceChannel.Name}' 음성 채널에 연결되었습니다!");
            }
            catch (Exception ex)
            {
                await context.Channel.SendMessageAsync($"음성 채널에 연결할 수 없습니다: {ex.Message}");
            }
        }

        private async Task LeaveVoiceChannelAsync(SocketCommandContext context)
        {
            var voiceChannel = (context.User as SocketGuildUser)?.VoiceChannel;
            if (voiceChannel == null)
            {
                await context.Channel.SendMessageAsync("봇이 음성 채널에 연결되어 있지 않습니다.");
                return;
            }

            await voiceChannel.DisconnectAsync();
            await context.Channel.SendMessageAsync("음성 채널에서 나왔습니다.");
        }
    }
}
