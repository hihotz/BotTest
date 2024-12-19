using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;


namespace BotTest.Main
{
    internal class Main
    {
        private DiscordSocketClient? _client;
        private readonly string botToken;

        public Main(string _botToken)
        {
            botToken = _botToken;
        }
        
        public async Task RunBotAsync()
        {
            // 클라이언트 설정
            // _client = new DiscordSocketClient();


            // Intents 설정
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });

            // 로그 메시지 처리
            _client.Log += LogAsync;

            // 메시지 수신 처리
            _client.MessageReceived += MessageReceivedAsync;

            // 봇 로그인 및 시작
            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();

            // 봇이 종료되지 않도록 대기
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // 봇 자신의 메시지는 무시
            if (message.Author.IsBot) return;

            // 특정 명령어에 반응
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("애옹!");
            }
        }
    }


}
