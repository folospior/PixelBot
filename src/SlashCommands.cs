using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using DSharpPlus.SlashCommands.Attributes;

namespace PixelBot
{
    public class SlashCommands : ApplicationCommandModule
    {
        private static readonly HttpClient userClient = new() { BaseAddress = new Uri("https://users.roblox.com/") };
        private static readonly HttpClient groupClient = new() { BaseAddress = new Uri("https://groups.roblox.com/") };
        private static readonly HttpClient friendsClient = new() { BaseAddress = new Uri("https://friends.roblox.com/") };
        private static readonly HttpClient badgeClient = new() { BaseAddress = new Uri("https://www.roblox.com/") };
        private static readonly HttpClient thumbnailClient = new() { BaseAddress = new Uri("https://thumbnails.roblox.com/") };
        private static readonly HttpClient catClient = new() { BaseAddress = new Uri("https://api.thecatapi.com/") };
        private static readonly HttpClient dogClient = new() { BaseAddress = new Uri("https://api.thedogapi.com/") };
        private static readonly HttpClient factClient = new() { BaseAddress = new Uri("https://api.api-ninjas.com/") };

        [SlashCommand("ban", "Ban the specified user.")]
        public async Task BanCommand
        (
            InteractionContext ctx,
            [Option("member", "Member to ban")] 
            DiscordUser user,
            [Option("reason", "Reason for ban")] 
            string reason,
            [Choice("None", 0)]
            [Choice("1 Day", 1)]
            [Choice("3 Days", 3)]
            [Choice("1 Week", 7)]
            [Option("deletedays", "Number of days of message history to delete")]
            long deletedays
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (!ctx.Member.Roles.Contains(ctx.Guild.GetRole(1177946702339112970)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                        .WithColor(DiscordColor.DarkRed)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | Unauthorised!")
                        .WithDescription("You are not authorised to run this command!")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter("If you believe this is a bug, contact fo1o.")
                    )
                );
                return;
            }
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            await ctx.Guild.BanMemberAsync(member.Id, (int)deletedays, reason);
            var banServerMessage =
                new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(member.Username, null, member.AvatarUrl)
                        .WithColor(DiscordColor.DarkRed)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} | Successfully banned {member.Username}!")
                        .WithDescription($"Reason: {reason}")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter(ctx.User.Username, ctx.User.AvatarUrl)
                    );
            if (!member.IsBot)
            {
                try
                {
                    await member.SendMessageAsync(new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | You have been banned from {ctx.Guild.Name}!")
                            .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                            .WithDescription($"Reason for your ban: {reason}")
                            .WithTimestamp(DateTime.Now)
                            .WithColor(DiscordColor.Red)
                            .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                        )
                    );
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("403"))
                    {
                        banServerMessage = banServerMessage.WithContent("*Could not DM!*");
                    }
                }
            }
            else
            {
                banServerMessage = banServerMessage.WithContent("*Could not DM!*");
            }
            await ctx.EditResponseAsync(banServerMessage);
        }

        [SlashCommand("unban", "Unban the specified user")]
        public async Task UnbanCommand
        (
            InteractionContext ctx,
            [Option("id", "User ID")]
            string id,
            [Option("reason", "Reason for unban")]
            string reason
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (!ctx.Member.Roles.Contains(ctx.Guild.GetRole(1177946702339112970)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                        .WithColor(DiscordColor.DarkRed)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | Unauthorised!")
                        .WithDescription("You are not authorised to run this command!")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter("If you believe this is a bug, contact fo1o.")
                    )
                );
                return;
            }
            DiscordUser user = await ctx.Client.GetUserAsync(Convert.ToUInt64(id));
            await user.UnbanAsync(ctx.Guild, reason);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(user.Username, null, user.AvatarUrl)
                    .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} | Successfully unbanned {user.Username}!")
                    .WithDescription($"Reason: {reason}")
                    .WithTimestamp(DateTime.Now)
                    .WithColor(DiscordColor.LightGray)
                    .WithFooter(ctx.User.Username, ctx.User.AvatarUrl)
                )
            );
        }

        [SlashCommand("timeout", "Times-out the specified user")]
        public async Task TimeoutCommand(
        InteractionContext ctx,
            [Option("user", "User to timeout")]
        DiscordUser user,
            [Option("reason", "Reason for timeout")]
            string reason,
            [Choice("1m", 1)]
            [Choice("5m", 5)]
            [Choice("10m", 10)]
            [Choice("30m", 30)]
            [Choice("1h", 60)]
            [Choice("3h", 180)]
            [Choice("6h", 360)]
            [Choice("12h", 720)]
            [Choice("1d", 1440)]
            [Choice("2d", 2880)]
            [Choice("7d", 10080)]
            [Option("time", "How long to timeout the user for")]
            double time
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (!ctx.Member.Roles.Contains(ctx.Guild.GetRole(1177946702339112970)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                        .WithColor(DiscordColor.DarkRed)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | Unauthorised!")
                        .WithDescription("You are not authorised to run this command!")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter("If you believe this is a bug, contact fo1o.")
                    )
                );
                return;
            }
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            await member.TimeoutAsync(DateTimeOffset.Now.AddMinutes(time), reason);
            var timeoutServerMessage = new DiscordWebhookBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithAuthor(member.Username, null, member.AvatarUrl)
                            .WithColor(DiscordColor.Green)
                            .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} | Successfully timed-out {member.Username} for {time} minute{((time > 1) ? "s" : "")}!")
                            .WithDescription($"Reason: {reason}")
                            .WithTimestamp(DateTime.Now)
                            .WithFooter(ctx.User.Username, ctx.User.AvatarUrl)
                        );
            if (!member.IsBot)
            {
                try
                {
                    await member.SendMessageAsync(new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                            .WithColor(DiscordColor.Red)
                            .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | You have been timed-out in {ctx.Guild.Name} for {time} minute{((time > 1) ? "s" : "")}!")
                            .WithDescription($"Reason: {reason}")
                            .WithTimestamp(DateTime.Now)
                            .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                        )
                    );
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("403"))
                    {
                        timeoutServerMessage = timeoutServerMessage.WithContent("*Could not DM!*");
                    }
                }
            }
            else
            {
                timeoutServerMessage = timeoutServerMessage.WithContent("*Could not DM!*");
            }
            await ctx.EditResponseAsync(timeoutServerMessage);
        }

        [SlashCommand("endtimeout", "Ends the timeout for a specified user")]
        public async Task EndTimeoutCommand(
        InteractionContext ctx,
            [Option("user", "User for ending timeout")]
        DiscordUser user,
            [Option("reason", "Reason for ending timeout")]
            string reason
            )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (!ctx.Member.Roles.Contains(ctx.Guild.GetRole(1177946702339112970)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                        .WithColor(DiscordColor.DarkRed)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | Unauthorised!")
                        .WithDescription("You are not authorised to run this command!")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter("If you believe this is a bug, contact fo1o.")
                    )
                );
                return;
            }
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            if (member.CommunicationDisabledUntil == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(member.Username, null, member.AvatarUrl)
                        .WithColor(DiscordColor.Red)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | User is not timed out!")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter(ctx.User.Username, ctx.User.AvatarUrl)
                    )
                );
                return;
            }
            await member.TimeoutAsync(null, reason);
            var endTimeoutServerMessage = new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(member.Username, null, member.AvatarUrl)
                    .WithColor(DiscordColor.Green)
                    .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} | Successfully ended the timeout of {member.Username}")
                    .WithDescription($"Reason: {reason}")
                    .WithTimestamp(DateTime.Now)
                    .WithFooter(ctx.User.Username, ctx.User.AvatarUrl)
            );
            if (!member.IsBot)
            {
                try
                {
                    var endTimeoutDM = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                            .WithColor(DiscordColor.Green)
                            .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} | Your timeout in {ctx.Guild.Name} has ended prematurely!")
                            .WithDescription($"Reason: {reason}")
                            .WithTimestamp(DateTime.Now)
                            .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                        );
                    await member.SendMessageAsync(endTimeoutDM);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("403"))
                    {
                        endTimeoutServerMessage = endTimeoutServerMessage.WithContent("*Could not DM!*");
                    }
                }
            }
            else
            {
                endTimeoutServerMessage = endTimeoutServerMessage.WithContent("*Could not DM!*");
            }
            await ctx.EditResponseAsync(endTimeoutServerMessage);
        }

        [SlashCommand("kick", "Kick the specified user.")]
        public async Task KickCommand(
            InteractionContext ctx,
            [Option("user", "User to kick")]
            DiscordUser user,
            [Option("reason", "Reason for kick")]
            string reason
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (!ctx.Member.Roles.Contains(ctx.Guild.GetRole(1177946702339112970)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                        .WithColor(DiscordColor.DarkRed)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | Unauthorised!")
                        .WithDescription("You are not authorised to run this command!")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter("If you believe this is a bug, contact fo1o.")
                    )
                );
                return;
            }
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            await member.RemoveAsync(reason);
            var kickServerMessage = new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(member.Username, null, member.AvatarUrl)
                    .WithColor(DiscordColor.Green)
                    .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} | Successfully kicked {member.Username}!")
                    .WithDescription($"Reason: {reason}")
                    .WithTimestamp(DateTime.Now)
                    .WithFooter(ctx.User.Username, ctx.User.AvatarUrl)
                );
            if (!member.IsBot)
            {
                try
                {
                    await member.SendMessageAsync(new DiscordMessageBuilder()
                        .WithEmbed(new DiscordEmbedBuilder()
                            .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                            .WithColor(DiscordColor.Red)
                            .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | You have been kicked from {ctx.Guild.Name}!")
                            .WithDescription($"Reason: {reason}")
                            .WithTimestamp(DateTime.Now)
                            .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                        )
                    );
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("403"))
                    {
                        kickServerMessage = kickServerMessage.WithContent("*Could not DM!*");
                    }
                }
            }
            else
            {
                kickServerMessage = kickServerMessage.WithContent("*Could not DM!*");
            }
            await ctx.EditResponseAsync(kickServerMessage);
        }

        [SlashCommand("softban", "Softbans the specified user")]
        public async Task SoftbanCommand
        (
            InteractionContext ctx,
            [Option("user", "User to softban")]
            DiscordUser user,
            [Option("reason", "Reason for softban")]
            string reason,
            [Choice("None", 0)]
            [Choice("1 Day", 1)]
            [Choice("3 Days", 3)]
            [Choice("1 Week", 7)]
            [Option("deletedays", "Number of days of message history to delete")]
            long deletedays
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (!ctx.Member.Roles.Contains(ctx.Guild.GetRole(1177946702339112970)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                        .WithColor(DiscordColor.DarkRed)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | Unauthorised!")
                        .WithDescription("You are not authorised to run this command!")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter("If you believe this is a bug, contact fo1o.")
                    )
                );
                return;
            }
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            var softbanServerMessage = new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(user.Username, null, user.AvatarUrl)
                    .WithColor(DiscordColor.Green)
                    .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} | Successfully softbanned {user.Username}!")
                    .WithDescription($"Reason: {reason}")
                    .WithTimestamp(DateTime.Now)
                    .WithFooter(ctx.User.Username, ctx.User.AvatarUrl)
            );
            if (!member.IsBot)
            {
                try
                {
                    await member.SendMessageAsync(new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                            .WithColor(DiscordColor.Red)
                            .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | You have been softbanned from {ctx.Guild.Name}")
                            .WithDescription($"Reason: {reason}")
                            .WithTimestamp(DateTime.Now)
                            .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                        )
                    );
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("403"))
                    {
                        softbanServerMessage = softbanServerMessage.WithContent("*Could not DM!*");
                    }
                }
            }
            else
            {
                softbanServerMessage = softbanServerMessage.WithContent("*Could not DM!*");
            }
            await member.BanAsync((int)deletedays, reason + " Softban.");
            await user.UnbanAsync(ctx.Guild, reason + " Softban.");
            await ctx.EditResponseAsync(softbanServerMessage);
        }

        [SlashCommand("avatar", "Gets the avatar of the specfied user")]
        public async Task AvatarCommand
        (
            InteractionContext ctx,
            [Option("user", "User to get the avatar from")]
            DiscordUser user
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await ctx.EditResponseAsync(
                new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(user.Username, null, user.AvatarUrl)
                        .WithImageUrl(user.AvatarUrl)
                        .WithTimestamp(DateTime.Now)
                        .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                        .WithColor(DiscordColor.MidnightBlue)
                    )
                );
        }

        [SlashCommand("check", "Background check a Roblox account")]
        public async Task CheckCommand
        (
            InteractionContext ctx,
            [Option("user", "User to background check")]
            DiscordUser? user = null
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (user == null)
            {
                user = ctx.User;
            }

            bool inCUSA = false;
            string inventoryVisibility = "";
            string badgesState = "";
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);

            if (member.Nickname == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(user.Username, null, user.AvatarUrl)
                        .WithColor(DiscordColor.DarkRed)
                        .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | Not verified!")
                        .WithDescription("Please run **<@508968886998269962> verify** or **<@508968886998269962> update** and follow the instructions the bot gives you." +
                        "\nIf you wished to background check a person other than yourself, please instruct them to do it themselves.")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter("If you believe this is a bug, please contact fo1o.")
                    )
                );
                return;
            }
            string username = member.Nickname.Split(' ').Last();

            GetIDPostData IdPostData = new()
            {
                usernames = new List<string>
                {
                    username
                },
                excludeBannedUsers = true
            };

            StringContent IdPostDataString = new(
                JsonConvert.SerializeObject(IdPostData, Formatting.Indented),
                Encoding.UTF8,
                "application/json"
            );

            var IDResponse = await userClient.PostAsync("v1/usernames/users", IdPostDataString);
            string IDResponseString = await IDResponse.Content.ReadAsStringAsync();
            JObject IDResponseParsed = JObject.Parse(IDResponseString);

            string userID = "";

            try
            {
                userID = (string)IDResponseParsed["data"][0]["id"];
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Index was out of range."))
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithAuthor(username, null, user.AvatarUrl)
                            .WithColor(DiscordColor.DarkRed)
                            .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":x:")} | Invalid user!")
                            .WithDescription($"The user {username} does not exist or is banned on Roblox.")
                            .WithTimestamp(DateTime.Now)
                            .WithFooter("If you believe this is a bug, contact fo1o.")
                        )
                    );
                    return;
                }
            }

            var groupResponse = await groupClient.GetAsync($"v1/users/{userID}/groups/roles");
            var friendsResponse = await friendsClient.GetAsync($"v1/users/{userID}/friends/count");
            var followersResponse = await friendsClient.GetAsync($"v1/users/{userID}/followers/count");
            var ageResponse = await userClient.GetAsync($"v1/users/{userID}");
            var badgeResponse = await badgeClient.GetAsync($"users/inventory/list-json?assetTypeId=21&cursor=&itemsPerPage=100&userId={userID}");
            var thumbnailResponse = await thumbnailClient.GetAsync($"v1/users/avatar-headshot?userIds={userID}&size=150x150&format=Png&isCircular=false");

            string groupResponseString = await groupResponse.Content.ReadAsStringAsync();
            if (groupResponseString.Contains("4219097"))
            {
                inCUSA = true;
            }

            string friendsResponseString = await friendsResponse.Content.ReadAsStringAsync();
            JObject friendsResponseParsed = JObject.Parse(friendsResponseString);
            string friendsCount = (string)friendsResponseParsed["count"];

            string followersResponseString = await followersResponse.Content.ReadAsStringAsync();
            JObject followersResponseParsed = JObject.Parse(followersResponseString);
            string followersCount = (string)followersResponseParsed["count"];

            string ageResponseString = await ageResponse.Content.ReadAsStringAsync();
            JObject ageResponseParsed = JObject.Parse(ageResponseString);
            string createdOnString = (string)ageResponseParsed["created"];
            DateTime createdOnDateTime = DateTime.ParseExact(createdOnString, "MM/dd/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.RoundtripKind);
            TimeSpan difference = DateTime.Today - createdOnDateTime;
            int accountAge = (int)Math.Round(difference.TotalDays);

            string badgeResponseString = await badgeResponse.Content.ReadAsStringAsync();
            JObject badgeResponseParsed = JObject.Parse(badgeResponseString);
            int badgeCount = (int)badgeResponseParsed["Data"]["End"];

            if (badgeCount == 99)
            {
                inventoryVisibility = "Public";
                badgesState = "More than 100 or exactly 100 badges.";
            }
            else if (badgeCount == -1)
            {
                inventoryVisibility = "Private";
                badgesState = "Private inventory.";
            }
            else
            {
                inventoryVisibility = "Public";
                badgesState = "Less than 100 badges.";
            }

            string thumbnailResponseString = await thumbnailResponse.Content.ReadAsStringAsync();
            JObject thumbnailResponseParsed = JObject.Parse(thumbnailResponseString);
            var thumbnailURL = (string)thumbnailResponseParsed["data"][0]["imageUrl"];

            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                    .WithColor(DiscordColor.Azure)
                    .WithTitle($"Background Check - {username}")
                    .WithThumbnail(thumbnailURL)
                    .WithFooter($"ID: {ctx.User.Id}")
                    .AddField("Roblox ID:", $"```{userID}```", true)
                    .AddField("Is in CUSA?", (inCUSA) ? "```Yes```" : "```No```", true)
                    .AddField("Roblox inventory:", $"```{inventoryVisibility}```", true)
                    .AddField("Roblox account age:", $"```{accountAge} days```", true)
                    .AddField("Roblox followers:", $"```{followersCount}```", true)
                    .AddField("Roblox friends:", $"```{friendsCount}```", true)
                    .AddField("Roblox badge count:", $"```{badgesState}```", false)
                    .AddField("Discord username:", $"```{user.Username}```", true)
                    .AddField("Discord ID:", $"```{Convert.ToString(user.Id)}```", true)
                )
            );
        }

        [SlashCommand("cat", "Returns a random cat image")]
        public async Task CatCommand
        (
            InteractionContext ctx
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var catResponse = await catClient.GetAsync("v1/images/search");
            string catResponseString = await catResponse.Content.ReadAsStringAsync();
            JArray catResponseParsed = JArray.Parse(catResponseString);
            string catImageLink = (string)catResponseParsed[0]["url"];
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Teal)
                    .WithImageUrl(catImageLink)
                    .WithTimestamp(DateTime.Now)
                    .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":cat:")} | Peek-a-boo!")
                )
            );
        }

        [SlashCommand("dog", "Returns a random dog image")]
        public async Task DogCommand
        (
            InteractionContext ctx
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var dogResponse = await dogClient.GetAsync("v1/images/search");
            string dogResponseString = await dogResponse.Content.ReadAsStringAsync();
            JArray dogResponseParsed = JArray.Parse(dogResponseString);
            string catImageLink = (string)dogResponseParsed[0]["url"];
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Brown)
                    .WithImageUrl(catImageLink)
                    .WithTimestamp(DateTime.Now)
                    .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":dog:")} | Woof!")
                )
            );
        }

        [SlashCommand("fact", "Returns a random fact")]
        public async Task FactCommand
        (
            InteractionContext ctx
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (!factClient.DefaultRequestHeaders.Contains("X-Api-Key"))
            {
                factClient.DefaultRequestHeaders.Add("X-Api-Key", "fTik0VLOxu8ttx7ieclFMQ==Qfl1cDjMGHc52z8n");
            }
            var factResponse = await factClient.GetAsync("v1/facts?limit=1");
            string factResponseString = await factResponse.Content.ReadAsStringAsync();
            JArray factResponseParsed = JArray.Parse(factResponseString);
            string fact = (string)factResponseParsed[0]["fact"];
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"{DiscordEmoji.FromName(ctx.Client, ":thinking:")} | This is true!")
                    .WithDescription(fact + ((fact[^1].ToString() != ".") ? "." : ""))
                    .WithColor(DiscordColor.CornflowerBlue)
                    .WithTimestamp(DateTime.Now)
                )
            );
        }

        [SlashCommand("setnick", "Sets the nickname of the specified user to the specified nickname")]
        [SlashRequirePermissions(Permissions.ChangeNickname)]
        public static async Task SetNickCommand(
            InteractionContext ctx,
            [Option("user", "User for which to change the nickname")]
            DiscordUser? user = null,
            [Option("nickname", "Nickname for the user. Leave empty to set nickname to the user's display name.")]
            string? nickname = null
        )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (user == null)
            {
                user = ctx.User;
            }
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            await member.ModifyAsync(x => x.Nickname = nickname);
            var nicknameChangeServerMessage = new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(user.Username, null, user.AvatarUrl)
                    .WithColor(DiscordColor.Green)
                    .WithTitle((nickname == null) ?
                        $"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} | Successfully removed {user.Username}{((user.Username.EndsWith('s')) ? "\'" : "\'")} nickname." :
                        $"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} |" +
                        $" Successfully set {user.Username}{((user.Username.EndsWith('s')) ? "\'" : "\'s")} nickname to {nickname}")
                    .WithTimestamp(DateTime.Now)
                    .WithFooter(ctx.User.Username, ctx.User.AvatarUrl)
            );
            if (!member.IsBot)
            {
                try
                {
                    await member.SendMessageAsync(new DiscordEmbedBuilder()
                        .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                        .WithColor(DiscordColor.Teal)
                        .WithTitle((nickname == null) ? $"Your nickname in {ctx.Guild.Name} has been reset!" : $"Your nickname in {ctx.Guild.Name} has been set to {nickname}!")
                        .WithTimestamp(DateTime.Now)
                        .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                    );
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("403"))
                    {
                        nicknameChangeServerMessage = nicknameChangeServerMessage.WithContent("*Could not DM!*");
                    }
                }
            }
            else
            {
                nicknameChangeServerMessage = nicknameChangeServerMessage.WithContent("*Could not DM!*");
            }
            await ctx.EditResponseAsync(nicknameChangeServerMessage);
        }
    }
}
