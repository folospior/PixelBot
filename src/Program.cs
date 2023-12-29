    /*
 * CHANGE YOUR TOKEN ON LINE 25
 * BEFORE STARTING THE BOT
 *
 * THE BOT WILL NOT WORK WITHOUT THE TOKEN
 * INSTRUCTIONS TO GET TOKEN ARE IN THE README
 */

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace PixelBot
{
    class Program
    {
        public static async Task Main()
        {
            if (Environment.GetEnvironmentVariable("DISCORD_TOKEN") == null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("It seems that this is the first time you are running the bot.");
                Console.WriteLine("A Discord bot token is needed for further continuing.");
                Console.WriteLine("Instructions on getting the token are in the README.md file.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You will need to open the bot executable once again after inputting the token.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please enter your bot token: ");
                Environment.SetEnvironmentVariable("DISCORD_TOKEN", Console.ReadLine());
                return;
            }
            
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
            discord.GuildMemberAdded += MemberAddedMessage;
            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<SlashCommands>();
            SlashCommandsExtension slashCommandsExtension = slash;
            slashCommandsExtension.SlashCommandErrored += async (s, e) =>
            {
                await e.Context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithDescription(e.Exception.Message)
                            .WithColor(DiscordColor.DarkRed)
                            .WithTimestamp(DateTime.Now)
                            .WithFooter("If you believe this is a bug, please contact fo1o.")
                        )
                );
            };
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
       private static async Task MemberAddedMessage(DiscordClient s, GuildMemberAddEventArgs e)
        {
            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(e.Member.DisplayName, null, e.Member.AvatarUrl)
                    .WithColor(e.Member.Color)
                    .WithTimestamp(DateTime.Now)
                    .WithTitle($"Welcome to {e.Guild.Name}!")
                    .WithDescription($"You are our member #{e.Guild.MemberCount}. Make yourself feel at home.")
                    .WithFooter(e.Guild.Name, e.Guild.IconUrl)
                )
                .WithContent(e.Member.Mention);
            var channel = await s.GetChannelAsync(1155216445211422794);
            await s.SendMessageAsync(channel, message);
        }
    }
    public class GetIDPostData
    {
        public IList<string> usernames { get; set; }
        public bool excludeBannedUsers { get; set; }
    }

}
