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
using static System.Net.Mime.MediaTypeNames;


namespace EyjaTrinket
{
    public class EyjaTrinket : TrinketEffect
    {

        public float HealTimer;
        public float HealDelay = 4000f;
        public float Power = 0.25f;
        public int DamageSinceLastHeal;

        public Companion Companion1;

        /// <inheritdoc />
        public EyjaTrinket(Trinket trinket)
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

            // 炎息buff
            Texture2D buffIcon = Game1.content.Load<Texture2D>("Mods/EyjaYanXi");
            Buff attackBuff = new Buff(
                id: "Eyjafjalla_attack_buff",
               displayName: I18n.Buff_YanXi_Name(),  //
                  description: I18n.Buff_YanXi_Description(),  //
                iconTexture: buffIcon,
                iconSheetIndex: 0,
                duration: 40,
                effects: new BuffEffects { Attack = { 2 } }
            );
            farmer.applyBuff(attackBuff);
            if (Companion1 == null || !farmer.companions.Contains(Companion1))
            {
                Apply(farmer);
            }
        }

        /// <inheritdoc />
        public override void Apply(Farmer farmer)
        {
            HealTimer = 0f;
            DamageSinceLastHeal = 0;

            Companion1 = new FlyingCompanion1(0);
            if (Game1.gameMode == 3)
            {
                farmer.AddCompanion(Companion1);
            }

            base.Apply(farmer);
        }

        /// <inheritdoc />
        public override void Unapply(Farmer farmer)
        {
            farmer.RemoveCompanion(Companion1);
        }
    }
}
