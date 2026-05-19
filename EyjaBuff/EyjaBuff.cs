using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;
using EyjaTrinket.ProjectileTAS;
using StardewValley.Tools;
using System.Reflection.Emit;
using EyjaTrinket;
namespace EyjaTrinket.EyjaBuff
{
    public class ReplenishingMistBuff : Buff
    {
        private float _healTimer;
        private float HealInterval = 1000f; // 1秒
        private int _currentLevel = 1; // 当前层数
        private int MaxLevel = 3; // 最大层数
        private bool _isDowngrading = false; // 标记是否正在降级

        // 粒子效果相关
        private float _particleTimer;
        private float ParticleInterval = 550f; // 粒子生成间隔

        // 存储从 ChunAi 传递过来的 baseHeal
        private int _baseHeal;
        private Random rand = new Random(); 
        // 各层级的治疗效果和防御加成
        private (int Defense, int HealAmount)[] _levelData = new[]
        {
            (1, 3),    // 等级1
            (2, 6), // 等级2  
            (3, 9)          // 等级3
        };

        public int CurrentLevel => _currentLevel;
        // 基础持续时间（毫秒）
        private int BaseDuration = 7000;
        // 降级后的持续时间（毫秒）- 3秒
        private int DowngradeDuration = 3000;
        private static Texture2D CustomIconTexture => Game1.content.Load<Texture2D>("Mods/EyjaReplenishingMistBuffIcon");
        public ReplenishingMistBuff(int level = 1, bool isDowngrading = false, int baseHeal = 15)
            : base(
                id: "ReplenishingMistBuff",
                source: "ChunAi",
                displaySource: I18n.VolcanoWatchers(),
                duration: isDowngrading ? 3000 : 7000,//基础7s
                iconTexture: CustomIconTexture,
                iconSheetIndex: 0,
                effects: CreateEffects(level),// 初始防御+1
                isDebuff: false,
                displayName: GetDisplayName(level),
                description: GetDescription(level, isDowngrading, baseHeal) 
            )
        {
            _currentLevel = Math.Clamp(level, 1, MaxLevel);
            _isDowngrading = isDowngrading;
            _baseHeal = baseHeal; // 存储传递的值
            _healTimer = 0f;
            _particleTimer = 0f;
            customFields["level"] = _currentLevel.ToString();
            customFields["isDowngrading"] = _isDowngrading.ToString();
            customFields["baseHeal"] = _baseHeal.ToString(); 
        }
        private static BuffEffects CreateEffects(int level)
        {
            var defense = level switch
            {
                1 => 1,
                2 => 2,
                3 => 3,
                _ => 1
            };
            return new BuffEffects { Defense = { Value = defense } };
        }
        private static string GetDisplayName(int level)
        {
            return level switch
            {
                1 => I18n.ReplenishingMist() + " I",
                2 => I18n.ReplenishingMist() + " II",
                3 => I18n.ReplenishingMist() + " III",
                _ => I18n.ReplenishingMist()
            };
        }
        private static string GetDescription(int level, bool isDowngrading, int baseHeal)
        {
            var healAmount = CalculateHealAmount(level, baseHeal);
            var durationText = isDowngrading ? "3秒" : "7秒";
            return string.Format(I18n.VERegenerates(), healAmount); ;
        }
        private static int CalculateHealAmount(int level, int baseHeal)
        {
            return level switch
            {
                1 => (int)(baseHeal * 0.1f), 
                2 => (int)(baseHeal * 0.1f)+ (int)(baseHeal * 0.1f), 
                3 => (int)(baseHeal * 0.1f)+ (int)(baseHeal * 0.1f)+ (int)(baseHeal * 0.1f), 
                _ => (int)(baseHeal * 0.2f)
            };
        }
        public void Upgrade()
        {
            if (_currentLevel < MaxLevel)
            {
                _currentLevel++;
            }

            // 刷新持续时间（使用完整时间）
            millisecondsDuration = BaseDuration;
            totalMillisecondsDuration = BaseDuration;
            _isDowngrading = false;

            // 更新效果
            effects.Defense.Value = _currentLevel;

            // 更新显示信息
            displayName = GetDisplayName(_currentLevel);
            description = GetDescription(_currentLevel, false, _baseHeal);

            // 更新自定义字段
            customFields["level"] = _currentLevel.ToString();
            customFields["isDowngrading"] = _isDowngrading.ToString();
        }
        //降级
        public void Downgrade()
        {
            if (_currentLevel > 1)
            {
                _currentLevel--;
                _isDowngrading = true;

                // 设置降级后的持续时间
                millisecondsDuration = DowngradeDuration;
                totalMillisecondsDuration = DowngradeDuration;

                // 更新效果
                effects.Defense.Value = _currentLevel;

                // 更新显示信息
                displayName = GetDisplayName(_currentLevel);
                description = GetDescription(_currentLevel, true, _baseHeal);

                // 更新自定义字段
                customFields["level"] = _currentLevel.ToString();
                customFields["isDowngrading"] = _isDowngrading.ToString();

            }
        }
        /// 刷新持续时间（用于3级时的刷新）
        public void Refresh()
        {
            millisecondsDuration = 7000;
            totalMillisecondsDuration = 7000;
        }

        public override bool update(GameTime time)
        {
            // 调用父类更新持续时间
            bool shouldRemove = base.update(time);

            // 如果Buff时间结束
            if (shouldRemove)
            {
                // 如果不是降级状态且不是1级，则降级而不是完全移除
                if (_currentLevel > 1)
                {
                    // 创建降级Buff
                    CreateDowngradeBuff();
                    return false; // 移除当前Buff
                }
                return true; // 移除1级Buff或降级后的Buff
            }

            float elapsed = (float)time.ElapsedGameTime.TotalMilliseconds;

            // 更新治疗计时器
            _healTimer += elapsed;
            if (_healTimer >= HealInterval)
            {
                ApplyHealing();
                _healTimer = 0f;
            }

            // 更新粒子计时器
            _particleTimer += elapsed;
            if (_particleTimer >= (ParticleInterval+ rand.Next(-100, 101)))
            {
                CreateHealingParticles();
                _particleTimer = 0f;
            }

            return false;
        }
        private void CreateDowngradeBuff()
        {
            if (Game1.player != null)
            {
                var downgradeBuff = new ReplenishingMistBuff(_currentLevel - 1, true, _baseHeal);
                Game1.player.buffs.Apply(downgradeBuff);
            }
        }

        private void ApplyHealing()
        {
            if (Game1.player != null && Game1.player.health < Game1.player.maxHealth && Game1.shouldTimePass())
            {
                // 使用存储的 _baseHeal 计算治疗量
                int healAmount = CalculateHealAmount(_currentLevel, _baseHeal);
                Game1.player.health = Math.Min(Game1.player.health + healAmount, Game1.player.maxHealth);

                Game1.player.currentLocation.debris.Add(new Debris(
                    healAmount,
                    Game1.player.getStandingPosition(),
                    Color.Lime,
                    1f,
                    Game1.player
                ));
                Game1.playSound("EyjaReplenishingMistSound");
            }
        }
        private float GetCompensationSpeedByScaleChange(float scaleChange)
        {
            
            return Math.Abs(scaleChange) * 36f;
        }
        private int GetWeightedRandomValue()
        {
            int randomNum = rand.Next(10);

            if (randomNum < 8)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private int GetWeightedRandomValue1()
        {
            int randomNum = rand.Next(10);

            if (randomNum < 6)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private void CreateHealingParticles()
        {
            if (Game1.player?.currentLocation == null)
                return;

            Vector2 playerPosition = Game1.player.getStandingPosition();
            string textureName = "Mods/EyjaStarLightAnimation";
            string textureName1 = "Mods/EyjaStarLightAnimation";
            int random1 = GetWeightedRandomValue();
            int random2 = rand.Next(4) + 9;

            for (int i = 0; i < random1; i++)//十字粒子
            {
                int random3 = GetWeightedRandomValue1();//粒子出现频率
                Vector2 heartPosition = playerPosition + new Vector2(0f, 20f) + new Vector2(
                rand.Next(-50, 50),
                rand.Next(-60, 10)
            );
                int random = rand.Next(4);
                int YZrandom = rand.Next(2);
                Vector2 Shuihua = new Vector2(25f, -105f);
                float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值

                float scale = (float)(rand.NextDouble() * 0.4 + 0.1);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                float scaleChange = (float)(rand.NextDouble() * 0.008 + 0.002);
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
                switch (YZrandom)
                {
                    case 0:
                        textureName1 = "Mods/EyjaBlueLeaf";
                        break;
                    case 1:
                        textureName1 = "Mods/EyjaPinkLeaf";
                        break;
                    default:
                        textureName1 = "Mods/EyjaBlueLeaf";
                        break;
                }
                var particle1 = new EyjaTAS(
                    "垂直叶子",//id
                    1200f,//多少毫秒后开始动态变化
                    textureName1,
                    sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: heartPosition,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: scaleChange,
                    rotation: initialRotation,
                    rotationChange: rotationSpeed
                            );
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "垂直星星",//id
                    1200f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 3000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: heartPosition,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: scaleChange,
                    rotation: 0f,
                    rotationChange: 0f
                );
                float speed = GetCompensationSpeedByScaleChange(scaleChange);
                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    -(float)Math.Sin(angle) * speed 
                );
                particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    -(float)Math.Sin(angle) * speed 
                );
                particle1.acceleration = new Vector2(0, -0.0075f);
                particle.acceleration = new Vector2(0, -0.0075f);
                if (random3 == 1)
                {
                    Game1.player.currentLocation.temporarySprites.Add(particle);
                }
                else if (random3 == 2)
                {
                    Game1.player.currentLocation.temporarySprites.Add(particle1);
                }

            }

        }

        public override void OnAdded()
        {
            base.OnAdded();
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
        }
    }
}
