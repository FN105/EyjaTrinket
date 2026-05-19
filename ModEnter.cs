using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.GameData;
using StardewValley.Objects.Trinkets;
using StardewValley;
using static EyjaTrinket.EyjaTrinket;
using Microsoft.Xna.Framework.Graphics;

namespace EyjaTrinket
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            I18n.Init(helper.Translation);

        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
           
        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            
        }
    }
}