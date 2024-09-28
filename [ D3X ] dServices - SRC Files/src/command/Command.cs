using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using static dServices.dServices;

namespace dServices
{
    public static class Command
    {
        public static void Load()
        {
            var config = Config.config?.Settings;
            if (config == null) return;

            foreach (var command in SplitCommands(config.Services_Commands))
            {
                Instance.AddCommand($"css_{command}", "Services Command", Command_Services);
            }
        }

        private static IEnumerable<string> SplitCommands(string commands)
        {
            return commands.Split(',').Select(c => c.Trim());
        }

        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public static void Command_Services(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;

            var config = Config.config.Settings;
            var menu = Instance._api.NewMenu($"<font color='{config.Menu_Title_Color}'>{config.Menu_Title}</font>");

            foreach (var serviceName in Config.config.Uslugi.Keys)
            {
                menu.AddMenuOption($"<font color='#ffd500'>{serviceName}</font>", (player, option) =>
                {
                    if (Config.config.Uslugi.TryGetValue(serviceName, out var serviceInfo))
                    {
                        if (serviceInfo != null)
                        {
                            if (config.DisplayMode == "menu" || config.DisplayMode == "both")
                            {
                                var infoMenu = Instance._api.NewMenu($"<font color='{config.Menu_Title_Color}'>{serviceInfo.Title}</font>");
                                foreach (var menuOption in serviceInfo.MenuOptions)
                                {
                                    infoMenu.AddMenuOption($"<font color='#ffd500'>{menuOption}</font>", (player, option) => { }, true);
                                }
                                infoMenu.Open(player);
                            }

                            if (config.DisplayMode == "chat" || config.DisplayMode == "both")
                            {
                                foreach (var menuOption in serviceInfo.MenuOptions)
                                {
                                    player.PrintToChat($" {ChatColors.DarkRed}► {ChatColors.Green}[{ChatColors.DarkRed} {serviceName} {ChatColors.Green}] {ChatColors.Green}✔ {ChatColors.Lime}{menuOption}");
                                }

                                if (config.DisplayMode == "chat")
                                {
                                    Instance._api.CloseMenu(player);
                                }
                            }
                        }
                    }
                });
            }

            menu.Open(player);
        }
    }
}