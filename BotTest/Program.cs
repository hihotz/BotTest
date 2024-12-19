using BotTest.Main;
using Microsoft.Extensions.Configuration;

#region appsettings.json에서 데이터 불러오기
// 봇 토큰
IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory()) // 현재 디렉토리 기준
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
string botToken = _configuration["DiscordConnectionStrings:BotToken"] ?? string.Empty;
#endregion

var main = new Main(botToken);

await main.RunBotAsync();











