using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Projectiles.BasicProjectile;
using StardewValley.TerrainFeatures;
using StardewValley.Extensions;

namespace EyjaTrinket.ProjectileTAS
{
    public class HealingProjectile : BasicProjectile
    {
        public readonly NetInt healAmount = new NetInt();
        public readonly NetRef<Character> _firer = new NetRef<Character>();
        private readonly NetFloat trackStrength = new NetFloat(0.1f);
        private readonly NetFloat maxTrackSpeed = new NetFloat(8f);

        // 添加无参数构造函数
        public HealingProjectile() : base()
        {
            InitNetFields();
        }

        public HealingProjectile(
            int healAmount,
            int spriteIndex,
            int bouncesTillDestruct,
            int tailLength,
            float rotationVelocity,
            float xVelocity,
            float yVelocity,
            Vector2 startingPosition,
            string collisionSound,
            string firingSound,
            GameLocation location,
            Character firer,
            onCollisionBehavior collisionBehavior = null,
            float trackStrength = 0.1f,
            float maxTrackSpeed = 8f)
            : base(
                damageToFarmer: 0,
                spriteIndex: spriteIndex,
                bouncesTillDestruct: bouncesTillDestruct,
                tailLength: tailLength,
                rotationVelocity: rotationVelocity,
                xVelocity: xVelocity,
                yVelocity: yVelocity,
                startingPosition: startingPosition,
                collisionSound: collisionSound,
                bounceSound: null,
                firingSound: null,
                explode: false,
                damagesMonsters: false,
                location: location,
                firer: firer,
                collisionBehavior: collisionBehavior)
        {
            this.healAmount.Value = healAmount;
            this._firer.Value = firer;
            this.trackStrength.Value = trackStrength;
            this.maxTrackSpeed.Value = maxTrackSpeed;
            InitNetFields();
        }

        protected override void InitNetFields()
        {
            base.InitNetFields();
            base.NetFields.AddField(healAmount, "healAmount")
                         .AddField(_firer, "_firer")
                         .AddField(trackStrength, "trackStrength")
                         .AddField(maxTrackSpeed, "maxTrackSpeed");
        }


        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
            if (player == _firer.Value)
            {
                // 治疗
                player.health = Math.Min(player.maxHealth, player.health + healAmount.Value);

                // 触发碰撞行为
                if (this.collisionBehavior != null)
                {
                    this.collisionBehavior(location, (int)this.position.X, (int)this.position.Y, player);
                }
                piercesLeft.Value = 0;
                // 销毁弹射物
                //location.projectiles.Remove(this);
            }
        }
        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location)
        {
                return;
        }

        public override bool update(GameTime time, GameLocation location)
        {
            // 跟踪逻辑
            if (_firer.Value != null && _firer.Value.currentLocation == location)
            {
                // 计算朝向玩家的方向
                Vector2 direction = _firer.Value.Position - this.position.Value;
                if (direction != Vector2.Zero)
                {
                    direction.Normalize();

                    // 应用跟踪力
                    xVelocity.Value = MathHelper.Lerp(xVelocity.Value, direction.X * maxTrackSpeed.Value, trackStrength.Value);
                    yVelocity.Value = MathHelper.Lerp(yVelocity.Value, direction.Y * maxTrackSpeed.Value, trackStrength.Value);

                    // 限制最大速度
                    float currentSpeed = (float)Math.Sqrt(xVelocity.Value * xVelocity.Value + yVelocity.Value * yVelocity.Value);
                    if (currentSpeed > maxTrackSpeed.Value)
                    {
                        xVelocity.Value = xVelocity.Value / currentSpeed * maxTrackSpeed.Value;
                        yVelocity.Value = yVelocity.Value / currentSpeed * maxTrackSpeed.Value;
                    }
                }
            }

            // 调用基类的update方法并返回其结果
            return base.update(time, location);
        }
    }
}
