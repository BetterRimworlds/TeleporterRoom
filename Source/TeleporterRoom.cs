using UnityEngine;
using Verse;

namespace BetterRimworlds.TeleporterRoom;

public class TeleporterRoom : Mod
{
    public static Settings Settings;

    public TeleporterRoom(ModContentPack content) : base(content)
    {
        Settings = GetSettings<Settings>() ?? new Settings();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Settings.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Teleporter Room";
    }
}
