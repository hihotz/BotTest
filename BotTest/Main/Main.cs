using Discord.WebSocket;
using Discord;
using BotTest.Commands;

namespace BotTest.Main
{
    internal class Main
    {
        private DiscordSocketClient? client;
        private readonly CommandHandler commandHandler;
        private readonly string botToken;


        public Main(string _botToken)
        {
            botToken = _botToken;

            // Intents 설정
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });
            client.Log += LogAsync;// 로그 메시지 처리
            client.Ready += ReadyAsync; // 메시지 수신 처리

            commandHandler = new CommandHandler(client);
            commandHandler.RegisterCommands();
        }
        
        public async Task RunBotAsync()
        {
            // 봇 로그인 및 시작
            await client!.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();

            // 봇이 종료되지 않도록 대기
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine("봇이 준비되었습니다!");
            return Task.CompletedTask;
        }
    }
}
