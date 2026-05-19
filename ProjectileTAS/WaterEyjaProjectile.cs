using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Projectiles.BasicProjectile;

namespace EyjaTrinket.ProjectileTAS
{
    //跟踪子弹类
    public class WaterEyjaProjectile : BasicProjectile
    {
        private Monster _target;
        private readonly NetVector2 _targetPosition = new NetVector2();
        private readonly NetFloat _trackStrength = new NetFloat(0.5f);
        private readonly NetFloat _maxTrackSpeed = new NetFloat(15f);
        private readonly NetBool _isTrackingPosition = new NetBool(false);
        private readonly NetFloat _timeAfterKill = new NetFloat(0f);
        private float TimeToLiveAfterKill = 2000f; // 怪物死亡后子弹存活的时间（毫秒）TrackParticlesTimer
        private float TrackParticlesTimer;
        public WaterEyjaProjectile(
            int damageToFarmer,
            int spriteIndex,
            int bouncesTillDestruct,
            int tailLength,
            float rotationVelocity,
            float xVelocity,
            float yVelocity,
            Vector2 startingPosition,
            string collisionSound,
            string bounceSound,
            string firingSound,
            bool explode,
            bool damagesMonsters,
            GameLocation location,
            Character firer,
            onCollisionBehavior collisionBehavior,
            Monster target = null,
            Vector2? targetPosition = null)
            : base(damageToFarmer, spriteIndex, bouncesTillDestruct, tailLength, rotationVelocity,
                  xVelocity, yVelocity, startingPosition, collisionSound, bounceSound,
                  firingSound, explode, damagesMonsters, location, firer, collisionBehavior)
        {
            if (target != null)
            {
                this._target = target;
                _isTrackingPosition.Value = false;
            }
            else if (targetPosition.HasValue)
            {
                _targetPosition.Value = targetPosition.Value;
                _isTrackingPosition.Value = true;
            }

            InitNetFields();
        }

        protected override void InitNetFields()
        {
            base.InitNetFields();
            base.NetFields
                         .AddField(_targetPosition, "_targetPosition")
                         .AddField(_trackStrength, "_trackStrength")
                         .AddField(_maxTrackSpeed, "_maxTrackSpeed")
                         .AddField(_timeAfterKill, "_timeAfterKill")
                         .AddField(_isTrackingPosition, "_isTrackingPosition");
        }

        public override bool update(GameTime time, GameLocation location)
        {

            TrackParticlesTimer += time.ElapsedGameTime.Milliseconds;
            if (TrackParticlesTimer >= 30f)
            {
                TrackParticlesTimer = 0;
                TrackParticles(location);
            }
            // 如果已经标记为死亡后计时，则更新计时器
            if (_timeAfterKill.Value > 0f)
            {
                _timeAfterKill.Value -= (float)time.ElapsedGameTime.TotalMilliseconds;

                // 销毁子弹
                if (_timeAfterKill.Value <= 0f)
                {
                    destroyMe = true;
                    return true;
                }

                // 继续正常移动，但不跟踪
                return base.update(time, location);
            }

            // 检查目标是否还存在且是有效的怪物
            if (!_isTrackingPosition.Value && _target!= null && _target.currentLocation == location)
            {
                // 检查目标是否已经死亡
                if (_target.Health <= 0)
                {
                    // 目标已死亡，启动死亡后计时器
                    _timeAfterKill.Value = TimeToLiveAfterKill;
                    _target = null; // 清除目标引用

                    // 降低速度
                    xVelocity.Value *= 0.5f;
                    yVelocity.Value *= 0.5f;

                    return base.update(time, location);
                }

                // 原有的跟踪逻辑
                Vector2 direction = _target.Position - this.position.Value;

                if (direction != Vector2.Zero) // 避免在非常接近目标时过度调整
                {
                    direction.Normalize();
                    // 应用跟踪力
                    xVelocity.Value = MathHelper.Lerp(xVelocity.Value, direction.X * _maxTrackSpeed.Value, _trackStrength.Value);
                    yVelocity.Value = MathHelper.Lerp(yVelocity.Value, direction.Y * _maxTrackSpeed.Value, _trackStrength.Value);

                    // 限制最大速度
                    float currentSpeed = (float)Math.Sqrt(xVelocity.Value * xVelocity.Value + yVelocity.Value * yVelocity.Value);
                    if (currentSpeed > _maxTrackSpeed.Value)
                    {
                        xVelocity.Value = xVelocity.Value / currentSpeed * _maxTrackSpeed.Value;
                        yVelocity.Value = yVelocity.Value / currentSpeed * _maxTrackSpeed.Value;
                    }

                }
            }
            else if (_isTrackingPosition.Value)
            {
                // 位置跟踪逻辑
                Vector2 direction = _targetPosition.Value - this.position.Value;
                float distance = direction.Length();

                if (direction != Vector2.Zero)
                {
                    direction.Normalize();

                    // 跟踪
                    xVelocity.Value = MathHelper.Lerp(xVelocity.Value, direction.X * _maxTrackSpeed.Value, _trackStrength.Value);
                    yVelocity.Value = MathHelper.Lerp(yVelocity.Value, direction.Y * _maxTrackSpeed.Value, _trackStrength.Value);

                    // 限制最大速度
                    float currentSpeed = (float)Math.Sqrt(xVelocity.Value * xVelocity.Value + yVelocity.Value * yVelocity.Value);
                    if (currentSpeed > _maxTrackSpeed.Value)
                    {
                        xVelocity.Value = xVelocity.Value / currentSpeed * _maxTrackSpeed.Value;
                        yVelocity.Value = yVelocity.Value / currentSpeed * _maxTrackSpeed.Value;
                    }
                }
            }

            return base.update(time, location);
        }
        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {

            base.behaviorOnCollisionWithMonster(n, location);

            // 检查目标是否已经死亡
            if (_target != null && _target.Health <= 0)
            {
                // 启动死亡后计时器
                _timeAfterKill.Value = TimeToLiveAfterKill;

                // 停止跟踪
                _target = null;

                // 降低速度
                xVelocity.Value *= 0.5f;
                yVelocity.Value *= 0.5f;
            }
        }
        private void TrackParticles(GameLocation location)
        {
            Vector2 Dianhu = new Vector2(0, 0);

            Vector2 _bulletFlyDir = new Vector2(0, -1); //修改左右飞行偏移
            Vector2 smallOffset = _bulletFlyDir * -20f; // 

            Vector2 _bulletFlyDir1 = new Vector2(-1, 0); // 修改上下飞行偏移
            Vector2 smallOffset1 = _bulletFlyDir1 * -30f; // 乘倍数
            Vector2 particlePosition = this.position.Value + Dianhu + smallOffset + smallOffset1;
            Random rand = new Random();
            int randomNumber = rand.Next(0, 3);

            string textureName = "Mods/EyjaBubble";

            for (int i = 0; i < rand.Next(1, 3); i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 0.9 + 0.3);
                float rotationChange = (float)(0.03 + rand.NextDouble() * 0.05);
                float scale = (float)(0.25 + rand.NextDouble() * 0.3);

                var particle = new TemporaryAnimatedSprite(
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: particlePosition,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.015f,
                    color: Color.White,
                    scale: scale,
                    scaleChange: 0f,
                    rotation: 0f,
                    rotationChange: rotationChange
                );

                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                location.temporarySprites.Add(particle);
            }



        }
    }
}
