using System;
using System.Reflection;
using System.Reflection.Emit;
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
    // ============================== 饰品效果类 ==============================
    public class WaterEyjaTrinket : TrinketEffect
    {
        /// <summary>The number of milliseconds until the fairy next heals the player.</summary>
        public float HealTimer;
        public static bool Huoshanyisu = false;
        /// <summary>The number of milliseconds between each heal.</summary>
        public float HealDelay = 4000f;

        /// <summary>The power rating applied to the heal amount.</summary>
        public float Power = 0.25f;

        /// <summary>The amount of damage taken by the player since the last heal.</summary>
        public int DamageSinceLastHeal;

        public Companion Companion1;
        /// <inheritdoc />
        public WaterEyjaTrinket(Trinket trinket)
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
                id: "Eyjafjalla_Attack_buff",
                source: "Eyjafjalla_Attack_buff",
                displaySource: I18n.Buff_YanXi_Description(),
                duration: 40,
                iconTexture: buffIcon,
                iconSheetIndex: 0,
                effects: new BuffEffects { Attack = { 2 } },
                isDebuff: false,
                displayName: null,
                description: null

           //     id: "Eyjafjalla_attack_buff",
           //displayName: I18n.Buff_YanXi_Name(),  //
           //       description: I18n.Buff_YanXi_Description(),  //
           //     iconTexture: buffIcon,
           //     iconSheetIndex: 0,
           //     duration: 40,
           //     effects: new BuffEffects { Attack = { 2 } }
            );
            if (Companion1 == null || !farmer.companions.Contains(Companion1))
            {
                Apply(farmer);
            }
            else
            {
                if (Companion1 is WaterEyjaCompanion WaterEyjaCompanion)
                {
                    bool newState = WaterEyjaCompanion._multiAttackMode;//_multiAttackMode=true的时候，火山进行时

                    if (newState)
                    {
                        // 速度+1
                        Texture2D buffIcon1 = Game1.content.Load<Texture2D>("TileSheets\\BuffsIcons");
                        Buff speedBuff = new Buff(
                        id: "Eyjafjalla_Speed_buff",
                        source: "Eyjafjalla_Speed_buff",
                        displaySource: I18n.Buff_YanXi_Description(),
                        duration: 40,
                        iconTexture: buffIcon1,
                        iconSheetIndex: 9,
                        effects: new BuffEffects { Speed = { 2 } },
                        isDebuff: false,
                        displayName: null,
                        description: null
                        );
                        farmer.applyBuff(speedBuff);
                    }
                }
            }
            farmer.applyBuff(attackBuff);

        }

        /// <inheritdoc />
        public override void Apply(Farmer farmer)
        {
            HealTimer = 0f;
            DamageSinceLastHeal = 0;

            Companion1 = new WaterEyjaCompanion(0);
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
