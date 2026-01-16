using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace InvisiblePlayerModel;

[Menu("Invisible Player Model")]
public class InvisiblePlayerModelConfig : ConfigFile
{
    [Toggle( "Hide Player Model ", Tooltip = "Hide the player model"), OnChange(nameof(OnHideModelChanged))]
    public bool hideModel = true;
    
    [Toggle( "Hide Player Shadows", Tooltip = "Hide the player shadows"), OnChange(nameof(OnHideModelChanged))]
    public bool hidePlayerShadows = true;

    public void OnHideModelChanged()
    {
        PlayerModelModifier.UpdatePlayerModel();
    }
}
