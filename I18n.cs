using System;
using System.CodeDom.Compiler;
using StardewModdingAPI;

namespace EyjaTrinket;

[GeneratedCode("TextTemplatingFileGenerator", "1.0.0")]
internal static class I18n
{
    private static ITranslationHelper? Translations;

    public static void Init(ITranslationHelper translations)
    {
        Translations = translations;
    }

    public static string Buff_YanXi_Name()
    {
        return GetByKey("Eyja.YanXi");
    }

    public static string Buff_YanXi_Description()
    {
        return GetByKey("Eyja.YanXiDescription");
    }
    public static string EyjaShouDongOpen()
    {
        return GetByKey("Eyja.ShouDongOpen");
    }
    public static string EyjaShouDongClose()
    {
        return GetByKey("Eyja.ShouDongClose");
    }
    public static string EyjaVolcanoCD()
    {
        return GetByKey("Eyja.VolcanoCD");
    }
    public static string EyjaVolcanoDone()
    {
        return GetByKey("Eyja.VolcanoDone");
    }
    public static string VolcanicEchoesCD()
    {
        return GetByKey("Eyja.VolcanicEchoesCD");
    }
    public static string VolcanicEchoesDone()
    {
        return GetByKey("Eyja.VolcanicEchoesDone");
    }
    public static string ReplenishingMist()
    {
        return GetByKey("Eyja.ReplenishingMist");
    }
    public static string VolcanoWatchers()
    {
        return GetByKey("Eyja.VolcanoWatchers");
    }
    public static string VERegenerates()
    {
        return GetByKey("Eyja.VERegenerates");
    }
    //    "Eyja.VolcanicEchoesCD": "火山回响CD：",
    //"Eyja.VolcanicEchoesDone": "火山回响已就绪!",
    //"Eyja.ReplenishingMist": "氤氲",
    //"Eyja.VolcanoWatchers": "火山预警花",
    //"Eyja.VERegenerates": "每秒回复{0}点生命值"
    public static Translation GetByKey(string key, object? tokens = null)
    {
        if (Translations == null)
        {
            throw new InvalidOperationException("You must call I18n.Init from the mod's entry method before reading translations.");
        }
        return Translations.Get(key, tokens);
    }
}
