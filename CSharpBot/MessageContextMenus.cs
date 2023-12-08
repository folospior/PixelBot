using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace PixelBot
{
    public class MessageContextMenus : ApplicationCommandModule
    {
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Translate")]
        public async Task TranslateMenu(ContextMenuContext ctx)
        {

        }
    }
}
