using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using Netcode;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace EyjaTrinket.ProjectileTAS
{
    public class CustomHealingProjectile : HealingProjectile
    {
        // 自定义贴图属性
        Random rand = new Random();
        public Texture2D CustomTexture { get; set; }
        public Rectangle? CustomSourceRect { get; set; }
        public float CustomScale { get; set; } = 2f;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        private Vector2? _targetPosition;
        private int MaxTailLength1 = 12; // 拖尾线段数量
        private List<Vector2> _tailPositions = new List<Vector2>(); // 原始轨迹点（无偏移）
        private Vector2 _tailOffset = new Vector2(30f, 30f);
        private Vector2 LiziOffset;
        private Vector2 WhiteLiziOffset;

        // 拖尾颜色
        private Color _headColor = new Color(255, 129, 190); // 靠近子弹的颜色
        private Color _tailColor = new Color(75, 178, 255); // 远离子弹的颜色（好像写反了）

        // 偏移参数
        private float _minOffset = 8f; // 最后一个线段的偏移量
        private float _maxOffset = 1f; // 第一个线段的偏移量
        private float _tailThickness = 6f;
        private float _particleSpawnTimer;


        // 模糊层相关字段
        private Texture2D _blurTexture;
        private float _blurScale = 1.2f; // 模糊层比原始拖尾稍大
        private Color _blurColor = new Color(255, 200, 255, 100); // 半透明的模糊颜色

        // 模糊层的偏移
        private float _blurMinOffset = 8f;
        private float _blurMaxOffset = 4f;
        private float _blurThickness = 8f;

        // 模糊层的颜色
        private Color _blurHeadColor = new Color(255, 129, 190, 80);
        private Color _blurTailColor = new Color(75, 178, 255, 60);

        // 只添加视觉相关的状态字段
        private bool _isVisualFinished = false;
        private float _fadeOutTimer = 0f;
        private float _fadeOutDuration = 0.5f;
        private Vector2? _finalTarget;
        private Vector2 _fixedTarget; //子弹消失位置
        public CustomHealingProjectile(
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
            float maxTrackSpeed = 8f
        ) : base(
            healAmount, spriteIndex, bouncesTillDestruct, tailLength, rotationVelocity,
            xVelocity, yVelocity, startingPosition, collisionSound, firingSound,
            location, firer, collisionBehavior, trackStrength, maxTrackSpeed
        )
        {
            InitNetFields();
            // 初始化贴图
            CustomTexture = Game1.content.Load<Texture2D>("Mods/EyjaZhiLiaoBall");
            CustomSourceRect = new Rectangle(0, 0, 48, 48);
            Origin = new Vector2(
                CustomSourceRect.Value.Width / 2,
                CustomSourceRect.Value.Height / 2
            );
            CustomScale = 1.5f;
            _blurTexture = Game1.content.Load<Texture2D>("Mods/EyjaWhiteShine1");
        }
        protected override void InitNetFields()
        {
            // 调用基类的InitNetFields
            base.InitNetFields();

            //NetFields.AddField(, "");
        }

        public override bool update(GameTime time, GameLocation location)
        {
            // 如果视觉上已完成，只更新消失动画
            if (_isVisualFinished)
            {
                return UpdateVisualFinishState(time, location);
            }

            // 原有的拖尾记录逻辑
            Vector2 currentPos = position.Value + _tailOffset;

            // 只有当拖尾为空，或当前位置与最后一个点的距离超过阈值时，才添加新点
            if (_tailPositions.Count == 0 ||
                Vector2.Distance(currentPos, _tailPositions.Last()) > 0f)
            {
                _tailPositions.Add(currentPos);
            }

            // 限制拖尾长度
            while (_tailPositions.Count > MaxTailLength1)
            {
                _tailPositions.RemoveAt(0);
            }

            _particleSpawnTimer += time.ElapsedGameTime.Milliseconds;
            if (_particleSpawnTimer >= 25f)
            {
                _particleSpawnTimer = 0;
                SpawnDirectionalParticles(location);
                CreateWhiteLightBall(location);
            }

            var target = GetTargetPosition(location);
            if (target.HasValue)
                rotation = CalculateTargetAngle(target.Value);

            // 调用基类update，如果返回true表示需要销毁
            bool shouldDestroy = base.update(time, location);

            // 当基类需要销毁时，我们只标记视觉完成
            if (shouldDestroy && !_isVisualFinished)
            {
                StartVisualFinishSequence(location);
                return false; // 返回false表示不立即销毁
            }

            return false;
        }

        //轨迹粒子
        private void SpawnDirectionalParticles(GameLocation location)
        {
            // 获取子弹当前速度向量（飞行方向）Vector2 direction = _firer.Value.Position - this.position.Value;
            Vector2 bulletVelocity = _firer.Value.Position - this.position.Value;
            if (bulletVelocity.LengthSquared() < 0.1f) // 避免零速度时生成粒子
                return;

            // 计算子弹飞行方向的角度（弧度）
            float baseAngle = (float)Math.Atan2(bulletVelocity.Y, bulletVelocity.X);

            // 粒子生成位置（子弹当前位置±5像素随机偏移）
            Vector2 spawnPos = position.Value + new Vector2(
                Game1.random.Next(-5, 6),
                Game1.random.Next(-5, 6)
            );

            Random rand = new Random();
            //float drop = 0.03f;
            //float drop = 0.03f;
            string textureName = "Mods/EyjaStarLightAnimation";
            int random1 = rand.Next(3) + 3;
            int random2 = rand.Next(4) + 9;
            int random3 = rand.Next(10);
            if (random3 == 1)
            {
                for (int i = 0; i < 1; i++)//十字粒子
                {
                    // 随机偏移角度：-30° 到 30°（转换为弧度）
                    float angleOffset = MathHelper.ToRadians(Game1.random.Next(-10, 11));
                    float particleAngle = baseAngle + angleOffset;
                    // 随机粒子速度(float)(rand.NextDouble() * 2 + 0.3);
                    float particleSpeed = (float)(Game1.random.NextDouble() * 3 + 0.6);
                    int random = rand.Next(4);
                    Vector2 Shuihua = new Vector2(30f, 30f);
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                    float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                    float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                    float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                    Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                    float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                    switch (random)
                    {
                        case 0:
                            textureName = "Mods/EyjaStarLight";
                            break;
                        case 1:
                            textureName = "Mods/EyjaStarLightPink";
                            break;
                        case 2:
                            textureName = "Mods/EyjaStarLightBlue";
                            break;
                        case 3:
                            textureName = "Mods/EyjaStarLightPurplePink";
                            break;
                    }
                    var particle = new EyjaTAS(
                        "星星",//id
                        1000f,//多少毫秒后开始动态变化
                        textureName: textureName,
                        sourceRect: new Rectangle(0, 0, 48, 48),
                        animationInterval: 3000f,
                        animationLength: 1,
                        numberOfLoops: 1,
                        position: LiziOffset + Shuihua,
                        layerDepth: 0.9f,
                        alphaFade: 0f,
                        color: Color.White,
                        scale: scale,
                        scaleChange: 0f,
                        rotation: initialRotation,
                        rotationChange: rotationSpeed
                    );

                    particle.motion = new Vector2(
                        (float)Math.Cos(particleAngle) * particleSpeed,
                        (float)Math.Sin(particleAngle) * particleSpeed
                    );
                    //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                    location.temporarySprites.Add(particle);
                }
            }
            else if (random3 == 2)
            {
                for (int i = 0; i < 1; i++)//叶子
                {
                    // 随机偏移角度：-30° 到 30°（转换为弧度）
                    float angleOffset = MathHelper.ToRadians(Game1.random.Next(-30, 31));
                    float particleAngle = baseAngle + angleOffset;
                    // 随机粒子速度(float)(rand.NextDouble() * 2 + 0.3);
                    float particleSpeed = (float)(Game1.random.NextDouble() * 3 + 0.6);
                    int random = rand.Next(2);
                    Vector2 Shuihua = new Vector2(30f, 30f);
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                    float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                    float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                    float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                    Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                    float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                    switch (random)
                    {
                        case 0:
                            textureName = "Mods/EyjaBlueLeaf";
                            break;
                        case 1:
                            textureName = "Mods/EyjaPinkLeaf";
                            break;
                        default:
                            textureName = "Mods/EyjaBlueLeaf";
                            break;
                    }
                    var particle1 = new EyjaTAS(
                        "叶子",//id
                        1000f,//多少毫秒后开始动态变化
                        textureName,
                        sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                        animationInterval: 3000f,//动画间隔：40f
                        animationLength: 1,//动画长度：15
                        numberOfLoops: 1,//循环次数：1
                        position: LiziOffset + Shuihua,//位置：爆炸中心
                        layerDepth: 0.9f,
                        alphaFade: 0f,
                        color: Color.White,
                        scale: scale,
                        scaleChange: 0f,
                        rotation: initialRotation,
                        rotationChange: rotationSpeed
                                );
                    // 手动设置运动参数
                    particle1.motion = new Vector2(
                        (float)Math.Cos(particleAngle) * particleSpeed,
                        (float)Math.Sin(particleAngle) * particleSpeed
                    );
                    //particle.acceleration = new Vector2(0, 0.3f);

                    location.temporarySprites.Add(particle1);
                }
            }
        }
        public void CreateWhiteLightBall(GameLocation location)
        {
            //

            string textureName = "Mods/EyjaWhiteLightBall";
            // 获取子弹当前速度向量（飞行方向）Vector2 direction = _firer.Value.Position - this.position.Value;
            Vector2 bulletVelocity = _firer.Value.Position - this.position.Value;
            if (bulletVelocity.LengthSquared() < 0.1f) // 避免零速度时生成粒子
                return;

            // 计算子弹飞行方向的角度（弧度）
            float baseAngle = (float)Math.Atan2(bulletVelocity.Y, bulletVelocity.X);
            float angleOffset = MathHelper.ToRadians(Game1.random.Next(-30, 31));
            float particleAngle = baseAngle + angleOffset;

            int random3 = rand.Next(10);
            //if (random3 > 3)
            for (int i = 0; i < rand.Next(1, 3); i++)//白色闪烁光团
            {

                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float angle1 = (float)(Math.PI * 3 / 4);
                float particleSpeed = (float)(Game1.random.NextDouble() * 2 + 0.4);
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.02 + 0.02);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "白色闪烁光团",//id
                    200f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 6000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: WhiteLiziOffset + Shuihua,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: scaleChange,
                    rotation: 0,
                    rotationChange: 0
                );
                float speed1 = Math.Abs(scaleChange) * 20f;
                particle.motion = new Vector2(
                    (float)Math.Cos(angle1) * speed1,//
                    -(float)Math.Sin(angle1) * speed1 //
                );
                particle.motion += new Vector2(
                        (float)Math.Cos(particleAngle) * particleSpeed,
                        (float)Math.Sin(particleAngle) * particleSpeed
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
        public override void draw(SpriteBatch b)
        {
            if (CustomTexture == null)
            {
                base.draw(b);
                return;
            }

            //拖尾
            DrawGradientTailWithOffset(b);
            //子弹头
            if (!_isVisualFinished)
            {
                DrawSquareStackedBulletHead(b);
            }

            //模糊
            DrawBlurTrail(b);
        }
        public override void updatePosition(GameTime time)
        {
            // 在视觉完成状态下，完全停止位置更新
            if (_isVisualFinished)
            {
                return;
            }

            // 否则调用基类逻辑
            base.updatePosition(time);
        }
        // 拖尾
        private void DrawGradientTailWithOffset(SpriteBatch b)
        {
            if (_tailPositions.Count < 2) return;

            int segmentCount = _tailPositions.Count - 1;

            for (int i = 0; i < segmentCount; i++)
            {
                float progress = (float)i / segmentCount;
                float currentOffset = MathHelper.Lerp(_minOffset, _maxOffset, progress);

                Vector2 start = _tailPositions[i];
                Vector2 end = _tailPositions[i + 1];

                // 获取拖尾方向并计算偏移（使用与子弹头相同的方法）
                Vector2 segmentDir = end - start;
                if (segmentDir.LengthSquared() < 0.1f) continue;
                segmentDir.Normalize();

                // 使用与子弹头相同的偏移计算逻辑
                Vector2 perpendicularDir = new Vector2(-segmentDir.Y, segmentDir.X);
                Vector2 leftDir = new Vector2(segmentDir.Y, -segmentDir.X);

                // 与子弹头相同的偏移参数
                float sideOffsetDistance = 6.5f;
                float forwardOffsetDistance = 0f;
                Vector2 offset = leftDir * sideOffsetDistance + segmentDir * forwardOffsetDistance;

                // 应用偏移到拖尾点
                Vector2 offsetStart = start + offset + perpendicularDir * currentOffset;
                Vector2 offsetEnd = end + offset + perpendicularDir * currentOffset;

                Color currentColor = LerpColor(_headColor, _tailColor, progress);
                currentColor = new Color((byte)currentColor.R, (byte)currentColor.G, (byte)currentColor.B, (byte)255);

                float thickness = _tailThickness * (0.2f + progress * 2f);

                DrawWideLine(
                    b,
                    offsetStart,
                    offsetEnd,
                    currentColor,
                    thickness
                );
            }
        }

        // 绘制加宽线段（复用逻辑，使用偏移后的位置）
        private void DrawWideLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            if (length < 0.1f) return;

            float angle = (float)Math.Atan2(direction.Y, direction.X);
            Vector2 startScreen = Game1.GlobalToLocal(Game1.viewport, start);

            // 使用1x1像素纹理绘制宽线段
            Texture2D tailTexture = Game1.staminaRect;
            Rectangle? sourceRect = null;

            spriteBatch.Draw(
                tailTexture,
                position: startScreen,
                sourceRectangle: sourceRect,
                color: color,
                rotation: angle,
                origin: Vector2.Zero,
                scale: new Vector2(length, thickness), // 长度=线段长度，宽度=thickness
                effects: SpriteEffects.None,
                layerDepth: (start.Y + 92f) / 10000f
            );
        }

        // 绘制子弹头
        private void DrawSquareStackedBulletHead(SpriteBatch b)
        {
            if (_tailPositions.Count == 0) return;
            Vector2 headPosition = _tailPositions.Last();

            // 计算拖尾前进方向（前后方向）
            Vector2 forwardDir = GetTailDirection();

            // 计算垂直于前进方向的向量（左右方向）
            // 公式：垂直于(x,y)的向量为(-y, x) 或 (y, -x)，二者方向相反
            //Vector2 rightDir = new Vector2(-forwardDir.Y, forwardDir.X); // 右方向
            Vector2 leftDir = new Vector2(forwardDir.Y, -forwardDir.X); // 左方向（与右方向相反）

            // 左右偏移距离（正数向右，负数向左，可调整）
            float sideOffsetDistance = -6f;
            // （可选）前后偏移距离（正数向前，负数向后）
            float forwardOffsetDistance = 0f;

            // 最终偏移 = 左右偏移 + （可选）前后偏移
            Vector2 offset = leftDir * sideOffsetDistance + forwardDir * forwardOffsetDistance;
            Vector2 adjustedHeadPos = headPosition;//+ offset

            Vector2 screenPos = Game1.GlobalToLocal(Game1.viewport, adjustedHeadPos);

            float sideOffsetDistance1 = 0f;
            float forwardOffsetDistance1 = 0f;
            Vector2 offset1 = leftDir * sideOffsetDistance1 + forwardDir * forwardOffsetDistance1;
            Vector2 _tailOffset1 = new Vector2(5f, 5f);
            Vector2 _bulletFlyDir = new Vector2(0, -1); //修改左右飞行偏移
            Vector2 smallOffset = _bulletFlyDir * 10f; // 

            Vector2 _bulletFlyDir1 = new Vector2(-1, 0); // 修改上下飞行偏移
            Vector2 smallOffset1 = _bulletFlyDir1 * 14f; // 乘倍数

            Vector2 smallOffset2 = _bulletFlyDir * -40f; // 

            Vector2 smallOffset3 = _bulletFlyDir1 * -40f; // 乘倍数

            LiziOffset = _tailPositions.Last() - _tailOffset - _tailOffset1 + smallOffset + smallOffset1;
            WhiteLiziOffset = (_tailPositions.Last()) * 0.995f - _tailOffset - _tailOffset1 + smallOffset2 + smallOffset3;
            b.Draw(
                CustomTexture,
                screenPos,
                CustomSourceRect,
                color.Value * alpha.Value,
                rotation,
                Origin,
                CustomScale * localScale,
                SpriteEffects.None,
                (screenPos.Y + 96f) / 10000f
            );
        }

        // 获取拖尾方向
        private Vector2 GetTailDirection()
        {
            if (_tailPositions.Count < 2)
            {
                return new Vector2(0, -1);
            }

            Vector2 prevPosition = _tailPositions[_tailPositions.Count - 2];
            Vector2 headPosition = _tailPositions.Last();
            Vector2 direction = headPosition - prevPosition;

            if (direction.LengthSquared() > 0)
            {
                direction.Normalize();
            }

            return direction;
        }

        // 颜色插值
        private Color LerpColor(Color a, Color b, float t)
        {
            return new Color(
                (byte)MathHelper.Lerp(a.R, b.R, t),
                (byte)MathHelper.Lerp(a.G, b.G, t),
                (byte)MathHelper.Lerp(a.B, b.B, t),
                (byte)MathHelper.Lerp(a.A, b.A, t)
            );
        }

        // 角度计算和目标位置获取
        private float CalculateTargetAngle(Vector2 targetPos)
        {
            Vector2 direction = targetPos - position.Value;
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            angle -= MathHelper.PiOver2;
            return angle;
        }

        private Vector2? GetTargetPosition(GameLocation location)
        {
            if (_firer.Value != null)
                return _firer.Value.Position + new Vector2(_firer.Value.Sprite.SpriteWidth / 2, _firer.Value.Sprite.SpriteWidth / 2);
            foreach (var farmer in location.farmers)
                return farmer.Position + new Vector2(farmer.Sprite.SpriteWidth / 2, farmer.Sprite.SpriteWidth / 2);
            return null;
        }
        // 同样修改模糊拖尾的方法
        private void DrawBlurTrail(SpriteBatch b)
        {
            if (_tailPositions.Count < 2 || _blurTexture == null)
                return;

            int segmentCount = _tailPositions.Count - 1;

            for (int i = 0; i < segmentCount; i++)
            {
                float progress = (float)i / segmentCount;
                float currentOffset = MathHelper.Lerp(_blurMinOffset, _blurMaxOffset, progress);

                Vector2 start = _tailPositions[i];
                Vector2 end = _tailPositions[i + 1];

                Vector2 segmentDir = end - start;
                if (segmentDir.LengthSquared() < 0.1f) continue;
                segmentDir.Normalize();

                // 使用与子弹头相同的偏移计算逻辑
                Vector2 perpendicularDir = new Vector2(-segmentDir.Y, segmentDir.X);
                Vector2 leftDir = new Vector2(segmentDir.Y, -segmentDir.X);

                float sideOffsetDistance = 5.5f;
                float forwardOffsetDistance = 0f;
                Vector2 offset = leftDir * sideOffsetDistance + segmentDir * forwardOffsetDistance;

                Vector2 offsetStart = start + offset + perpendicularDir * currentOffset;
                Vector2 offsetEnd = end + offset + perpendicularDir * currentOffset;

                Color currentColor = LerpColor(_blurHeadColor, _blurTailColor, progress);
                float thickness = _blurThickness * (0.2f + progress * 1f);

                DrawBlurLine(
                    b,
                    offsetStart,
                    offsetEnd,
                    currentColor,
                    thickness
                );
            }
        }

        // 使用模糊纹理绘制线段
        private void DrawBlurLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            if (length < 0.1f) return;

            float angle = (float)Math.Atan2(direction.Y, direction.X);

            // 计算线段的中点
            Vector2 segmentCenter = (start + end) / 2f;
            Vector2 centerScreen = Game1.GlobalToLocal(Game1.viewport, segmentCenter);

            // 计算缩放
            float scaleX = length * 4f / _blurTexture.Width;
            float scaleY = thickness * 4f / _blurTexture.Height;

            // 使用纹理中心作为原点
            Vector2 origin = new Vector2(_blurTexture.Width / 2.3f, _blurTexture.Height / 2.3f);

            spriteBatch.Draw(
                _blurTexture,
                position: centerScreen,
                sourceRectangle: null,
                color: color,
                rotation: angle,
                origin: origin,
                scale: new Vector2(scaleX, scaleY),
                effects: SpriteEffects.None,
                layerDepth: (start.Y + 101f) / 10000f
            );
        }
        private void StartVisualFinishSequence(GameLocation location)
        {
            _isVisualFinished = true;
            // 使用子弹头当前位置作为固定目标，而不是玩家位置
            _fixedTarget = position.Value + _tailOffset;
            _fadeOutTimer = 0f;

            // 停止物理运动，但不影响碰撞逻辑
            xVelocity.Value = 0f;
            yVelocity.Value = 0f;
            rotationVelocity.Value = 0f;

            // 停止生成新粒子
            _particleSpawnTimer = 0;
            // 确保拖尾从当前位置开始
            if (_tailPositions.Count > 0)
            {
                // 用当前位置替换最后一个拖尾点
                _tailPositions[_tailPositions.Count - 1] = _fixedTarget;
            }
            else
            {
                // 如果没有拖尾点，添加当前位置
                _tailPositions.Add(_fixedTarget);
            }
        }

        private bool UpdateVisualFinishState(GameTime time, GameLocation location)
        {
            float deltaTime = (float)time.ElapsedGameTime.TotalSeconds;
            _fadeOutTimer += deltaTime;

            // 链条式运动：每段都朝着它前面的段移动
            ChainMoveTailSegments(deltaTime);

            // 当所有段都到达目标后返回true表示可以销毁
            return _tailPositions.Count <= 1;
        }

        private void ChainMoveTailSegments(float deltaTime)
        {
            if (_tailPositions.Count < 2) return;

            float baseSpeed = 300f;

            // 从最后一段开始向前移动（保持链条顺序）
            for (int i = _tailPositions.Count - 1; i >= 0; i--)
            {
                Vector2 targetPosition;

                if (i == _tailPositions.Count - 1)
                {
                    // 最后一段朝着固定目标移动
                    targetPosition = _fixedTarget;
                }
                else
                {
                    // 其他段朝着它后面的段移动（保持链条连接）
                    targetPosition = _tailPositions[i + 1];
                }

                Vector2 direction = targetPosition - _tailPositions[i];
                float distance = direction.Length();

                if (distance <= 1f)
                {
                    // 如果已经到达目标，移除这个段（除了最后一段）
                    if (i < _tailPositions.Count - 1)
                    {
                        _tailPositions.RemoveAt(i);
                    }
                    continue;
                }

                // 计算移动速度
                float speed = baseSpeed * (1.0f + (i * 0.4f)); // 后面的段移动更快

                // 移动段
                float moveDistance = Math.Min(speed * deltaTime, distance);
                direction.Normalize();
                _tailPositions[i] += direction * moveDistance;
            }
        }
        public Vector2 HeadPosition
        {
            get
            {
                return _tailPositions.Count > 0 ? _tailPositions.Last() : position.Value;
            }
        }

    }
}