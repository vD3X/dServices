using CounterStrikeSharp.API.Core; 
using CounterStrikeSharp.API.Core.Capabilities;
using MenuManager;

namespace dServices; 
 
public class dServices : BasePlugin 
{  
    public override string ModuleName => "[CS2] D3X - [ Opis Usług ]";
    public override string ModuleAuthor => "D3X";
    public override string ModuleDescription => " Plugin umożliwia stworzenie listy usług na serwerze.";
    public override string ModuleVersion => "1.0.0";

    public static dServices Instance { get; private set; } = new dServices();

    public IMenuApi? _api;
    private readonly PluginCapability<IMenuApi?> _pluginCapability = new("menu:nfcore");

    public override void Load(bool hotReload)
    {
        Instance = this;
        Config.Initialize();
        Command.Load();
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _api = _pluginCapability.Get();
        if (_api == null) Console.WriteLine("MenuManager Core nie znaleziono...");
    }
} 
