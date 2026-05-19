using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.GameData;
using StardewValley.Objects.Trinkets;
using StardewValley;
using static EyjaTrinket.EyjaTrinket;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EyjaTrinket.EyjaEffects;

namespace EyjaTrinket
{
    public class ModEntry : Mod
    {
        public static IModHelper StaticHelper;

        public class WaterHitEffectMessage
        {
            public string EffectType { get; set; } // 填id
            public float X { get; set; }           // 特效X坐标
            public float Y { get; set; }           // 特效Y坐标
            public string LocationName { get; set; }
        }

        public override void Entry(IModHelper helper)
        {
            StaticHelper = helper;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            I18n.Init(helper.Translation);
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type == "EyjaTrinket/PlayWaterHitEffect")
            {
                WaterHitEffectMessage msg = e.ReadAs<WaterHitEffectMessage>();
                Vector2 position = new Vector2(msg.X, msg.Y);
                GameLocation location = Game1.currentLocation;
                if (location == null || location.Name != msg.LocationName)
                    return;

                var hitEffect = new WaterEyjyHitEffect(location);
                if (msg.EffectType == "AddCasualParticles")
                {
                    hitEffect.AddCasualParticles(location, position);
                }
                else if (msg.EffectType == "AddVolcanoParticles")
                {
                    hitEffect.AddVolcanoParticles(location, position);
                }
                else if (msg.EffectType == "CreateExplosion")
                {
                    hitEffect.CreateExplosion(location, position);
                }
                else if (msg.EffectType == "VCreateExplosion")
                {
                    hitEffect.VCreateExplosion(location, position);
                }
                else if (msg.EffectType == "CreateSpecialExplosion")
                {
                    hitEffect.CreateSpecialExplosion(location, position);
                }
            }
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