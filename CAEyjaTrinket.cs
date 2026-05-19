using System;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Companions;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Objects.Trinkets;
using StardewValley.Projectiles;
using xTile;
using static System.Net.Mime.MediaTypeNames;


namespace EyjaTrinket
{
    public class CAEyjaTrinket : TrinketEffect
    {
        public float HealTimer;
        public float MaxHP;
        public float HealDelay = 2800f;
        public float Power = 0.25f;
        public int DamageSinceLastHeal;
        public bool IsVolcanicEchoesActive = false;
        public Companion Companion1;

        /// <inheritdoc />
        public CAEyjaTrinket(Trinket trinket)
            : base(trinket)
        {

        }
        /// <inheritdoc />
        public override void OnDamageMonster(Farmer farmer, Monster monster, int damageAmount, bool isBomb, bool isCriticalHit)
        {
            DamageSinceLastHeal += damageAmount;
            base.OnDamageMonster(farmer, monster, damageAmount, isBomb, isCriticalHit);
        }

        /// <inheritdoc />
        public override void OnReceiveDamage(Farmer farmer, int damageAmount)
        {
            DamageSinceLastHeal += damageAmount;
            base.OnReceiveDamage(farmer, damageAmount);
        }

        /// <inheritdoc />
        public override void Update(Farmer farmer, GameTime time, GameLocation location)
        {
            base.Update(farmer, time, location);


            if (Companion1 == null || !farmer.companions.Contains(Companion1))
            {
                Apply(farmer);
            }
            else
            {
                // 更新火山回响状态
                UpdateVolcanicEchoesState();
            }
        }

        private void UpdateVolcanicEchoesState()
        {
            if (Companion1 is ChunAi chunAi)
            {
                bool newState = chunAi.isVolcanicEchoes;

                // 如果状态发生变化
                if (newState != IsVolcanicEchoesActive)
                {
                    IsVolcanicEchoesActive = newState;
                    ApplyHealthBonus(Game1.player);
                }
            }
        }

        private void ApplyHealthBonus(Farmer farmer)
        {
            if (farmer.modData.ContainsKey("EyjaTrinket.MaxHP"))
            {
                if (float.TryParse(farmer.modData["EyjaTrinket.MaxHP"], out float savedMaxHP))
                {
                    MaxHP = savedMaxHP;//MaxHP是玩家基础生命值
                }
            }
            else
            {
                MaxHP = Game1.player.maxHealth;
                farmer.modData["EyjaTrinket.MaxHP"] = MaxHP.ToString();
            }

            //火山回响状态
            float bonusMultiplier = IsVolcanicEchoesActive ? 0.5f : 0.1f;
            int addAmount = (int)(MaxHP * bonusMultiplier);

            // 记录当前生命值比例，以便在调整后保持相同比例
            float healthRatio = (float)Game1.player.health / Game1.player.maxHealth;

            Game1.player.maxHealth = (int)MaxHP + addAmount;
            if(Game1.player.health== MaxHP || IsVolcanicEchoesActive)
            {
                Game1.player.health = Math.Min((int)(Game1.player.maxHealth * healthRatio), Game1.player.maxHealth);
            }
        }
        public override void Apply(Farmer farmer)
        {
            HealTimer = 0f;
            DamageSinceLastHeal = 0;
            IsVolcanicEchoesActive = false;

            Companion1 = new ChunAi(0);
            if (Game1.gameMode == 3)
            {
                farmer.AddCompanion(Companion1);
            }

            // 应用初始生命值加成
            ApplyHealthBonus(farmer);

            base.Apply(farmer);
        }

        /// <inheritdoc />
        public override void Unapply(Farmer farmer)
        {
            if (Companion1 != null)
            {
                farmer.RemoveCompanion(Companion1);
            }

            // 从 modData 恢复 MaxHP
            if (farmer.modData.ContainsKey("EyjaTrinket.MaxHP"))
            {
                if (float.TryParse(farmer.modData["EyjaTrinket.MaxHP"], out float savedMaxHP))
                {
                    Game1.player.maxHealth = (int)savedMaxHP;
                    // 确保不超过新的最大血量
                    Game1.player.health = Math.Min(Game1.player.health, Game1.player.maxHealth);
                }
            }

            // 重置状态
            IsVolcanicEchoesActive = false;

            base.Unapply(farmer);
        }
    }
}