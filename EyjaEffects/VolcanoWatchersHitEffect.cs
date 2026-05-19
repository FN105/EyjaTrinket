using EyjaTrinket.ProjectileTAS;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
namespace EyjaTrinket.EyjaEffects
{
    public class VolcanoWatchersHitEffect
    {
        private Random rand;
        public GameLocation location;

        // 初始化列表
        private List<StarSequenceData> activeStarSequences = new List<StarSequenceData>();

        private struct StarSequenceData
        {
            public Vector2 BasePosition;
            public int StarsToGenerate;
            public int StarsGenerated;
            public float Timer;
            public bool IsActive;
            public string SequenceType;//
        }

        public VolcanoWatchersHitEffect(GameLocation location)
        {
            rand = new Random();
            this.location = location;
        }
        // 开始一个新的星星生成序列
        public void StartStarSequence(Vector2 position, int minStars, int maxStars)
        {
            // 安全检查：确保列表已初始化
            if (activeStarSequences == null)
                activeStarSequences = new List<StarSequenceData>();

            StarSequenceData sequence = new StarSequenceData
            {
                BasePosition = position,
                StarsToGenerate = rand.Next(minStars, maxStars + 1),
                StarsGenerated = 0,
                Timer = 0f,
                IsActive = true,
                SequenceType = "回响光柱底闪烁星"
            };

            activeStarSequences.Add(sequence);
        }
        public void StartVolcanicEchoesThinStar2Sequence(Vector2 position, int minStars, int maxStars)
        {
            if (activeStarSequences == null)
                activeStarSequences = new List<StarSequenceData>();

            StarSequenceData sequence = new StarSequenceData
            {
                BasePosition = position,
                StarsToGenerate = rand.Next(minStars, maxStars + 1),
                StarsGenerated = 0,
                Timer = 0f,
                IsActive = true,
                SequenceType = "回响光柱内闪烁星"
            };

            activeStarSequences.Add(sequence);
        }
        // 更新所有活跃的星星序列
        public void UpdateStarSequences(float elapsedTime)
        {
            if (activeStarSequences == null || activeStarSequences.Count == 0)
                return;

            // 从后往前遍历
            for (int i = activeStarSequences.Count - 1; i >= 0; i--)
            {
                // 获取当前序列的副本
                StarSequenceData sequence = activeStarSequences[i];

                if (!sequence.IsActive) continue;

                sequence.Timer += elapsedTime;


                if (sequence.Timer >= 150f && sequence.StarsGenerated < sequence.StarsToGenerate && sequence.SequenceType == "回响光柱底闪烁星")
                {
                    // 生成一个星星
                    Vector2 starPosition = sequence.BasePosition + new Vector2(
                        rand.Next(-30, 31),
                        rand.Next(-60, 21)
                    );

                    CreateExplosion1(location, starPosition);

                    sequence.Timer = 0f;
                    sequence.StarsGenerated++;

                    // 检查是否完成
                    if (sequence.StarsGenerated >= sequence.StarsToGenerate)
                    {
                        sequence.IsActive = false;
                    }
                }
                if (sequence.Timer >= 250f && sequence.StarsGenerated < sequence.StarsToGenerate && sequence.SequenceType == "回响光柱内闪烁星")
                {

                    VolcanicEchoesThinStar2(location, sequence.BasePosition);

                    sequence.Timer = 0f;
                    sequence.StarsGenerated++;

                    // 检查是否完成
                    if (sequence.StarsGenerated >= sequence.StarsToGenerate)
                    {
                        sequence.IsActive = false;
                    }
                }
                // 更新列表中的元素
                activeStarSequences[i] = sequence;
            }

            // 移除已完成的序列
            activeStarSequences.RemoveAll(s => !s.IsActive);
        }

        // 治疗命中特效
        public void CreateHealHitEffect(Vector2 epicenter, int healAmount)
        {
            // 添加治疗数字
            location.debris.Add(new Debris(healAmount, epicenter, Color.Lime, 1f, Game1.player));

            // 播放音效


            // 创建粒子
            CreateStarParticles(location, epicenter);
            CreateWhiteLightBall(location, epicenter);
        }

        //火山回响粒子
        public void VolcanicEchoesHit(Vector2 epicenter, int healAmount)
        {
            // 添加治疗数字
            location.debris.Add(new Debris(healAmount, epicenter, Color.Lime, 1f, Game1.player));
            VolcanicEchoesRibbonAnimation(location, epicenter); VolcanicEchoesBlueLightBeam(location, epicenter);
            VolcanicEchoesShuiHuan(location, epicenter); VolcanicEchoesYeZi(location, epicenter);
            VolcanicEchoesThinStar(location, epicenter); VolcanicEchoesWhiteLightBall(location, epicenter);
            VolcanicEchoesLeaf(location, epicenter); CreateWhiteLightBall2(location, epicenter);

            //VolcanicEchoesThinStar2(location, epicenter);
        }

        //普攻爆炸星星叶子
        public void CreateStarParticles(GameLocation location, Vector2 epicenter)
        {

            //float drop = 0.03f;
            //float drop = 0.03f;
            string textureName = "Mods/EyjaStarLightAnimation";
            int random1 = rand.Next(2) + 2;
            int random2 = rand.Next(3) + 6;
            for (int i = 0; i < random1; i++)//十字粒子
            {
                int random = rand.Next(4);
                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 1 + 0.5);//（2,4）的速度（NextDouble()是[0，1））
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
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "星星",//id
                    400f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 3000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: scale,
                    scaleChange: 0f,
                    rotation: 0f,
                    rotationChange: 0f
                );

                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < random2; i++)//叶子
            {
                int random = rand.Next(2);
                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 1.4 + 0.5);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.5 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
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
                    700f,//多少毫秒后开始动态变化
                    textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Shuihua,//位置：爆炸中心
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
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle1);
            }
            for (int i = 0; i < 1; i++)//水花
            {
                Vector2 Shuihua = new Vector2(27f, 27f);
                Vector2 Shuihua1 = new Vector2(20f, 20f);
                Vector2 Shuihua11 = new Vector2(10f, 15f);
                float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                float speed = 2f; // 根据需要调整速度大小
                float speed1 = 2.6f; // 蓝光团
                float speed2 = 2f; // 蓝光团
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);

                var particle1 = new EyjaTAS(
                    "蓝光团",//id
                    800f,//多少毫秒后开始动态变化
                    "Mods/CAEyjaBlueShine",
                    sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Shuihua,//位置：爆炸中心
                    layerDepth: 0.6f,
                    alphaFade: 0f,
                    color: Color.White * 0.7f,
                    scale: 0.01f,
                    scaleChange: 0.08f,
                    rotation: 0f,
                    rotationChange: 0f
                            );
                var WhiteShine = new EyjaTAS(
                    "白光团",//id
                    600f,//多少毫秒后开始动态变化
                    "Mods/EyjaShine",
                    sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Shuihua,//位置：爆炸中心
                    layerDepth: 0.7f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: 0.06f,
                    rotation: 0f,
                    rotationChange: 0f
                            );
                var EyjaBeams = new EyjaTAS(
                    "EyjaBeams",//id 光刺
                    500f,//多少毫秒后开始动态变化
                    "Mods/EyjaBeams",
                    sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Shuihua,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: 0.06f,
                    rotation: initialRotation,
                    rotationChange: 0f
                            );
                // 手动设置运动参数

                particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed1,//-往左偏，+往右偏
                    -(float)Math.Sin(angle) * speed1 //（-下+上）
                );
                WhiteShine.motion = new Vector2(
                    (float)Math.Cos(angle) * speed2,//-往左偏，+往右偏
                    -(float)Math.Sin(angle) * speed2 //（-下+上）
                );
                EyjaBeams.motion = new Vector2(
                    (float)Math.Cos(angle) * speed2,//-往左偏，+往右偏
                    -(float)Math.Sin(angle) * speed2 //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(WhiteShine);
                location.temporarySprites.Add(EyjaBeams);
            }
        }
        //施法特效粒子
        public void PingAiLiZi(GameLocation location, Vector2 finalPosition1)
        {
            Random rand = new Random();
            //float drop = 0.03f;
            //float drop = 0.03f;
            string textureName = "Mods/EyjaStarLightAnimation";
            int random1 = rand.Next(3) + 3;
            int random2 = rand.Next(4) + 5;
            Vector2 Shuihua = new Vector2(-10f, -135f);//(25f, -105f)
            for (int i = 0; i < random1; i++)//十字粒子
            {
                int random = rand.Next(4);
                
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.4 + 0.1);//（2,4）的速度（NextDouble()是[0，1））
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
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "星星",//id
                    700f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 3000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: finalPosition1 + Shuihua,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: scale,
                    scaleChange: 0f,
                    rotation: initialRotation,
                    rotationChange: rotationSpeed
                );

                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed 
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < random2; i++)//叶子
            {
                int random = rand.Next(2);
                Vector2 Shuihua1 = new Vector2(20f, 20f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.4 + 0.1);//（2,4）的速度（NextDouble()是[0，1））
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
                    700f,//多少毫秒后开始动态变化
                    textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: finalPosition1 + Shuihua,//位置：爆炸中心
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
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed 
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle1);
            }
            //for (int i = 0; i < 1; i++)
            //{
            //    int random = rand.Next(2);
            //    Vector2 Shuihua = new Vector2(30f, -90f);
            //    float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
            //    float speed2 = 1f; // 蓝光团
            //    var WhiteShine = new EyjaTAS(
            //        "白光团1",//id
            //        400f,//多少毫秒后开始动态变化
            //        "Mods/EyjaShine",
            //        sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
            //        animationInterval: 3000f,//动画间隔：40f
            //        animationLength: 1,//动画长度：15
            //        numberOfLoops: 1,//循环次数：1
            //        position: finalPosition1 + Shuihua,//位置：爆炸中心
            //        layerDepth: 0.7f,
            //        alphaFade: 0f,
            //        color: Color.White,
            //        scale: 0.1f,
            //        scaleChange: 0.03f,
            //        rotation: 0f,
            //        rotationChange: 0f
            //          );

            //    WhiteShine.motion = new Vector2(
            //        (float)Math.Cos(angle) * speed2,
            //        -(float)Math.Sin(angle) * speed2 
            //    );

            //    location.temporarySprites.Add(WhiteShine);
            //}

        }
        //施法特效粒子
        public void VEPingAiLiZi(GameLocation location, Vector2 finalPosition1)
        {
            Random rand = new Random();
            string textureName = "Mods/EyjaStarLightAnimation";
            int random1 = rand.Next(3) + 4;
            int random2 = rand.Next(4) + 8;
            Vector2 Shuihua = new Vector2(-10f, -135f);//(25f, -105f)

            for (int i = 0; i < random1; i++)
            {
                int random = rand.Next(2);
                Vector2 Shuihua1 = new Vector2(-17f, -145f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 0.5 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.35 + 0.35);//（2,4）的速度（NextDouble()是[0，1））
                float Alpha = (float)(rand.NextDouble() * 0.3+0.7);//（2,4）的速度（NextDouble()是[0，1））
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                textureName = "Mods/EyjaLightSpot";
                var particle1 = new EyjaTAS(
                    "光斑",//id
                    1300f,//多少毫秒后开始动态变化
                    textureName,
                    sourceRect: new Rectangle(0, 0, 112, 112),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: finalPosition1 + Shuihua1,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White * Alpha,
                    scale: scale,
                    scaleChange: 0f,
                    rotation: initialRotation,
                    rotationChange: 0f
                            );
                // 手动设置运动参数
                particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle1);
            }
            for (int i = 0; i < random2; i++)//叶子
            {
                int random = rand.Next(2);
                Vector2 Shuihua1 = new Vector2(20f, 20f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.45 + 0.15);//（2,4）的速度（NextDouble()是[0，1））
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
                    800f,//多少毫秒后开始动态变化
                    textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: finalPosition1 + Shuihua,//位置：爆炸中心
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
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle1);
            }

        }
        //细十字光
        private int GetWeightedRandomValue()
        {
            int randomNum = rand.Next(100);

            if (randomNum <= 40)
            {
                return 0;
            }
            else if (randomNum > 40 && randomNum <= 80)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private float GetCompensationSpeedByScaleChange(float scaleChange)
        {
            // 简单线性关系：speed = 0.05 + scaleChange × 10
            return 0.08f + Math.Abs(scaleChange) * 30f;
        }
        //依次出现组
        public void CreateExplosion1(GameLocation location, Vector2 epicenter)
        {
            string textureName = "Mods/EyjaStarLightAnimation";
            for (int i = 0; i < 1; i++)//十字粒子
            {
                int random = GetWeightedRandomValue();
                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值

                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.03 + 0.015);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                switch (random)
                {
                    case 0:
                        textureName = "Mods/EyjaThinStarLightWhite";
                        break;
                    case 1:
                        textureName = "Mods/EyjaThinStarLightBlue";
                        break;
                    case 2:
                        textureName = "Mods/EyjaThinStarLight";
                        break;
                    case 3:
                        textureName = "Mods/EyjaStarLightPurplePink";
                        break;
                }
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "闪烁星星1",//id
                    350f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 3000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: scaleChange,
                    rotation: 0,
                    rotationChange: 0
                );
                float speed = GetCompensationSpeedByScaleChange(scaleChange);
                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    -(float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
        //依次水波纹(2)
        public void CreateShuiBoWen(GameLocation location, Vector2 epicenter)
        {
            for (int i = 0; i < 1; i++)
            {
                Vector2 Shuihua = new Vector2(20f, 20f);
                Vector2 Shuihua1 = new Vector2(20f, 20f);
                Vector2 Shuihua11 = new Vector2(10f, 15f);
                float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                float speed = 1.7f; // 根据需要调整速度大小
                float speed1 = 2.6f; // 蓝光团
                float speed2 = 2f; // 蓝光团
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                var particle = new EyjaTAS(
                    "水花",//id
                    500f,//多少毫秒后开始动态变化
                    "Mods/EyjaShuiHuan",
                    sourceRect: new Rectangle(0, 0, 112, 112),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Shuihua + Shuihua11,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.01f,
                    scaleChange: 0.02f,
                    rotation: initialRotation,
                    rotationChange: 0f
                            );
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                -(float)Math.Sin(angle) * speed //y坐标（-10，4）
                    );
                location.temporarySprites.Add(particle);
            }
        }

        //白色闪烁光团
        public void CreateWhiteLightBall(GameLocation location, Vector2 epicenter)
        {
            //
            string textureName = "Mods/EyjaWhiteLightBall";
            for (int i = 0; i < rand.Next(7, 10); i++)//白色闪烁光团
            {

                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float angle1 = (float)(Math.PI * 3 / 4);
                float speed = (float)(rand.NextDouble() * 1 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.03 + 0.01);//（2,4）的速度（NextDouble()是[0，1））
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
                    position: epicenter + Shuihua,
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
                    (float)Math.Cos(angle1) * speed1,//-往左偏，+往右偏
                    -(float)Math.Sin(angle1) * speed1 //（-下+上）
                );
                particle.motion += new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
        
        private int GetLeafRandomValue()
        {
            if (rand.Next(200) == 0)
            {
                return 3; 
            }
            else
            {
                return rand.Next(3);
            }
        }
        private Color LerpColor(Color a, Color b, float t)
        {
            return new Color(
                (byte)MathHelper.Lerp(a.R, b.R, t),
                (byte)MathHelper.Lerp(a.G, b.G, t),
                (byte)MathHelper.Lerp(a.B, b.B, t),
                (byte)MathHelper.Lerp(a.A, b.A, t)
            );
        }
        //日常粒子
        public void CreateRiChang(GameLocation location, Vector2 epicenter)
        {
            string textureName = "Mods/EyjaWhiteLightBall";

            for (int i = 0; i < rand.Next(1, 3); i++)//叶子
            {
                int random = rand.Next(4);
                Vector2 Shuihua = new Vector2(-40f, -25f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 1 + 0.5);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.5+ 1.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(0.03 + rand.NextDouble() * 0.03);//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                int random1 = GetLeafRandomValue();
                switch (random1)
                {
                    case 0:
                        textureName = "Mods/EyjaBasicLeaf1";
                        break;
                    case 1:
                        textureName = "Mods/EyjaBasicLeaf2";
                        break;
                    case 2:
                        textureName = "Mods/EyjaBasicLeaf3";
                        break;
                    case 3:
                        textureName = "Mods/EyjaTSLeaf";
                        break;
                }
                Color LeafColor;
                Color _headColor = new Color(255, 120, 226); // 靠近子弹的颜色
                Color _tailColor = new Color(84, 212, 255); // 远离子弹的颜色（好像写反了）
                if (random1 == 3)
                {
                    
                    LeafColor = Color.White;
                }
                else
                {
                    float t = (float)(rand.NextDouble()*0.7);
                    LeafColor =LerpColor(_headColor, _tailColor, t);
                }
                var particle1 = new EyjaTAS(
                    "16小叶子",//id
                    2500f,//多少毫秒后开始动态变化
                    textureName,
                    sourceRect: new Rectangle(0, 0, 16, 16),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Shuihua,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: LeafColor,
                    scale: scale,
                    scaleChange: 0f,
                    rotation: initialRotation,
                    rotationChange: rotationSpeed
                            );
               
                particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );
                particle1.acceleration = new Vector2(
                    (float)(rand.NextDouble() * 0.005 - 0.0025),
                    (float)(rand.NextDouble() * 0.005 - 0.0025)
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle1);
            }
        }


        //火山回响 预警花本体用的粒子
        public void CreateWhiteLightBall1(GameLocation location, Vector2 epicenter)
        {
            //
            string textureName = "Mods/EyjaWhiteLightBall";
            for (int i = 0; i < rand.Next(1, 2); i++)//白色闪烁光团
            {

                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float angle1 = (float)(Math.PI * 3 / 4);
                float speed = (float)(rand.NextDouble() * 0.3 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.01 + 0.01);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "白色1闪烁光团",//id
                    200f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 6000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.05f,
                    scaleChange: scaleChange,
                    rotation: 0,
                    rotationChange: 0
                );
                float speed1 = Math.Abs(scaleChange) * 12f;
                particle.motion = new Vector2(
                    (float)Math.Cos(angle1) * speed1,//-往左偏，+往右偏
                    -(float)Math.Sin(angle1) * speed1 //（-下+上）
                );
                particle.motion += new Vector2(
                    0,//-往左偏，+往右偏
                    speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.005f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
        public void VEThinStarLight(GameLocation location, Vector2 epicenter)
        {
            string textureName = "Mods/EyjaThinStarLightWhite";
            for (int i = 0; i < 1; i++)//十字粒子
            {
                int random = GetWeightedRandomValue();
                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值

                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.03 + 0.015);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
              
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "闪烁星星1",//id
                    250f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 3000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: scaleChange,
                    rotation: 0,
                    rotationChange: 0
                );
                float speed = GetCompensationSpeedByScaleChange(scaleChange);
                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    -(float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
        public void VolcanicEchoesRibbonAnimation(GameLocation location, Vector2 epicenter)
        {
            string textureName = "Mods/EyjaRibbonAnimation";
            string textureName1 = "Mods/EyjaRibbonAnimation1";
            int random = GetWeightedRandomValue();
            //Vector2 Shuihua = new Vector2(-16f, -270f);//2
            Vector2 Shuihua = new Vector2(-28f, -370f);
            // 创建粒子对象
            var particle = new EyjaTAS(
                    "丝带",//id
                    0f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 48, 176),
                    animationInterval: 24f,
                    animationLength: 18,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    layerDepth: 0.95f,
                    alphaFade: 0f,
                    color: Color.White*0.75f,
                    scale: 2.4f,
                    scaleChange: 0f,
                    rotation: 0,
                    rotationChange: 0
                );
            var particle1 = new TemporaryAnimatedSprite(
                textureName: textureName1,
                sourceRect: new Rectangle(0, 0, 48, 176),
                animationInterval: 24f,
                animationLength: 18,
                numberOfLoops: 1,
                position: epicenter + Shuihua,
                flicker: false,
                flipped: false,
                layerDepth: 0.95f,
                alphaFade: 0f,
                    color: Color.White * 0.75f,
                    scale: 2.4f,
                    scaleChange: 0f,
                    rotation: 0,
                    rotationChange: 0
            );
                location.temporarySprites.Add(particle);
            location.temporarySprites.Add(particle1);

        }
        public void VolcanicEchoesBlueLightBeam(GameLocation location, Vector2 epicenter)
        {
            string textureName = "Mods/EyjaBlueLightBeam";
            string textureName1 = "Mods/EyjaWhiteLightBeam";
            int random = GetWeightedRandomValue();
            //Vector2 Shuihua = new Vector2(-20f, -330f);//1
            //Vector2 Shuihua1 = new Vector2(-10f, -310f);//0.8
            Vector2 Shuihua = new Vector2(-30f, -430f);//1.3
            Vector2 Shuihua1 = new Vector2(-15f, -400f);//1
            // 创建粒子对象
            var particle = new EyjaTAS(
                "蓝色光柱",//id
                0f,//多少毫秒后开始动态变化
                textureName: textureName,
                sourceRect: new Rectangle(0, 0, 96, 448),
                animationInterval: 1200f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter + Shuihua,
                layerDepth: 0.9f,
                alphaFade: 0f,
                color: Color.White * 0f,
                scale: 1.3f,
                scaleChange: 0f,
                rotation: 0,
                rotationChange: 0
            );
            location.temporarySprites.Add(particle);
            var particle1 = new EyjaTAS(
                "白色光柱",//id
                0f,//多少毫秒后开始动态变化
                textureName: textureName1,
                sourceRect: new Rectangle(0, 0, 96, 448),
                animationInterval: 1200f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter + Shuihua1,
                layerDepth: 0.91f,
                alphaFade: 0f,
                color: Color.White * 0f,
                scale: 1f,
                scaleChange: 0f,
                rotation: 0,
                rotationChange: 0
            );
            location.temporarySprites.Add(particle1);

        }
        public void VolcanicEchoesShuiHuan(GameLocation location, Vector2 epicenter)
        {
            string textureName = "Mods/EyjaShuiGuangHuan";
            string textureName1 = "Mods/EyjaShuiHuan1";
            string textureName2 = "Mods/EyjaShuiHuan2";
            int random = GetWeightedRandomValue();
            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
            Vector2 Shuihua = new Vector2(-12f, 21f);
            Vector2 Shuihua1 = new Vector2(10f, 40f);
            Vector2 Shuihua2 = new Vector2(7f, 10f);
            // 创建粒子对象
            var particle = new EyjaTAS(
                "火山回响光环",//id
                300f,//多少毫秒后开始动态变化
                textureName: textureName,
                sourceRect: new Rectangle(0, 0, 288, 288),
                animationInterval: 3000f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter + Shuihua,
                layerDepth: 0f,
                alphaFade: 0f,
                color: Color.White * 0.7f,
                scale: 0.3f,
                scaleChange: 0.005f,
                rotation: 0,
                rotationChange: 0
            );
            particle.motion = new Vector2(
                (float)Math.Cos(angle) * 1f,
                -(float)Math.Sin(angle) * 1f 
            );
            location.temporarySprites.Add(particle);
            var particle1 = new EyjaTAS(
                "火山回响水环",//id
                400f,//多少毫秒后开始动态变化
                textureName: textureName1,
                sourceRect: new Rectangle(0, 0, 288, 288),
                animationInterval: 3000f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter + Shuihua1,
                layerDepth: 0.5f,
                alphaFade: 0f,
                color: Color.White * 0.7f,
                scale: 0.15f,
                scaleChange: 0.015f,
                rotation: 0,
                rotationChange: 0
            );
            particle1.motion = new Vector2(
                (float)Math.Cos(angle) * 3f,
                -(float)Math.Sin(angle) * 3f
            );
            location.temporarySprites.Add(particle1);
            var particle2 = new EyjaTAS(
                "火山回响水环",//id
                400f,//多少毫秒后开始动态变化
                textureName: textureName2,
                sourceRect: new Rectangle(0, 0, 288, 288),
                animationInterval: 3000f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter + Shuihua1+ Shuihua2,
                layerDepth: 0.5f,
                alphaFade: 0f,
                color: Color.White * 0.5f,
                scale: 0.1f,
                scaleChange: 0.01f,
                rotation: 0,
                rotationChange: 0
            );
            particle2.motion = new Vector2(
                (float)Math.Cos(angle) * 2f,
                -(float)Math.Sin(angle) * 2f
            );
            location.temporarySprites.Add(particle2);
        }
        public void VolcanicEchoesYeZi(GameLocation location, Vector2 epicenter)
        {

            //float drop = 0.03f;
            //float drop = 0.03f;
            string textureName = "Mods/EyjaStarLightAnimation";
            int random1 = rand.Next(2) + 2;
            int random2 = rand.Next(2) + 4;
            
            for (int i = 0; i < random2; i++)//叶子
            {
                int random = rand.Next(2);
                Vector2 Shuihua = new Vector2(19f, 35f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 1.4 + 0.5);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.3 + 0.25);//（2,4）的速度（NextDouble()是[0，1））
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
                    400f,//多少毫秒后开始动态变化
                    textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 3000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Shuihua,//位置：爆炸中心
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
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );

                location.temporarySprites.Add(particle1);
            }
        }
        private int GetWeightedRandomNumber()
        {
            int probabilityRand = rand.Next(100); 

            if (probabilityRand < 70)
            {
                return rand.Next(0, 4); 
            }
            else
            {
                return rand.Next(4, 8);
            }
        }
        public void VolcanicEchoesThinStar(GameLocation location, Vector2 epicenter)
        {
            string textureName = "Mods/EyjaStarLightAnimation";
            for (int i = 0; i < rand.Next(2) + 1; i++)//十字粒子
            {
                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                float speed1 = (float)(rand.NextDouble() * 3 + 3);///////////////////////////2.52.5 3 3
                Vector2 Position = new Vector2(
                        rand.Next(-10, 50),//x
                        rand.Next(-420, -120));//350，-130
                float y = Position.Y;
                if (y >= -200)
                {
                    float t = (y - (-150)) / (-200 - (-150)); // 0~1
                    speed1 -= 1.5f*t;
                }
                float growDur= (float)(rand.NextDouble() * 300 + 150);//350 150
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);
                float scaleChange = (float)(rand.NextDouble() * 0.035 + 0.03);/////////////////////////////0.025 0.02  003  0025
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                switch (GetWeightedRandomNumber())
                {
                    case 0:
                        textureName = "Mods/EyjaThinStarLight";
                        break;
                    case 1:
                        textureName = "Mods/EyjaThinStarLightBlue";
                        break;
                    case 2:
                        textureName = "Mods/EyjaThinStarLightPink";
                        break;
                    case 3:
                        textureName = "Mods/EyjaThinStarLightPurplePink";
                        break;
                    case 4:
                        textureName = "Mods/EyjaStarLight";
                        break;
                    case 5:
                        textureName = "Mods/EyjaStarLightBlue";
                        break;
                    case 6:
                        textureName = "Mods/EyjaStarLightPurplePink";
                        break;
                    case 7:
                        textureName = "Mods/EyjaStarLightPink";
                        break;
                }
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "闪烁星星2",//id
                    growDur,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 6000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua+ Position,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: scaleChange,
                    rotation: 0,
                    rotationChange: 0
                );
                float speed = 0.08f + Math.Abs(scaleChange) * 30f;
                // 手动设置运动参数
                particle.motion = new Vector2(
                   0f,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed1 //（-下+上）
                );
                particle.motion += new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    -(float)Math.Sin(angle) * speed //（-下+上）
                );

                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
        public void VolcanicEchoesThinStar2(GameLocation location, Vector2 epicenter)
        {
            string textureName = "Mods/EyjaStarLightAnimation";
            for (int i = 0; i < 1; i++)//十字粒子
            {
                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                float speed1 = (float)(rand.NextDouble() * 3 + 3);///////////////////////////2.52.5
                Vector2 Position = new Vector2(
                        rand.Next(-10, 50),//x
                        rand.Next(-420, -100));//350，-130
                float y = Position.Y;
                if (y >= -200)
                {
                    float t = (y - (-150)) / (-200 - (-150)); // 0~1
                    speed1 -= 1.5f * t;
                }
                float growDur = (float)(rand.NextDouble() * 350 + 150);
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);
                float scaleChange = (float)(rand.NextDouble() * 0.03 + 0.025);/////////////////////////////0.025 0.02
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                switch (GetWeightedRandomNumber())
                {
                    case 0:
                        textureName = "Mods/EyjaThinStarLight";
                        break;
                    case 1:
                        textureName = "Mods/EyjaThinStarLightBlue";
                        break;
                    case 2:
                        textureName = "Mods/EyjaThinStarLightPink";
                        break;
                    case 3:
                        textureName = "Mods/EyjaThinStarLightPurplePink";
                        break;
                    case 4:
                        textureName = "Mods/EyjaStarLight";
                        break;
                    case 5:
                        textureName = "Mods/EyjaStarLightBlue";
                        break;
                    case 6:
                        textureName = "Mods/EyjaStarLightPurplePink";
                        break;
                    case 7:
                        textureName = "Mods/EyjaStarLightPink";
                        break;
                }
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "闪烁星星2",//id
                    growDur,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 6000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua + Position,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.1f,
                    scaleChange: scaleChange,
                    rotation: 0,
                    rotationChange: 0
                );
                float speed = 0.08f + Math.Abs(scaleChange) * 30f;
                // 手动设置运动参数
                particle.motion = new Vector2(
                   0f,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed1 //（-下+上）
                );
                particle.motion += new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    -(float)Math.Sin(angle) * speed //（-下+上）
                );

                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
        public void VolcanicEchoesWhiteLightBall(GameLocation location, Vector2 epicenter)
        {
            //
            string textureName = "Mods/EyjaWhiteLightBall";
            for (int i = 0; i < rand.Next(20, 25); i++)//白色闪烁光团
            {
                Vector2 Position = new Vector2(
                rand.Next(-10, 60),//x 50
                rand.Next(-370, -50));//300
                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float angle1 = (float)(Math.PI * 3 / 4);
                float speed = (float)(rand.NextDouble() * 1 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.03 + 0.01);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "白色闪烁光团",//id
                    150f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 6000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua+ Position,
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
                    (float)Math.Cos(angle1) * speed1,//-往左偏，+往右偏
                    -(float)Math.Sin(angle1) * speed1 //（-下+上）
                );
                particle.motion += new Vector2(
                    0f,
                    speed 
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
            string textureName1 = "Mods/EyjaLittleWhiteLightBeam";
            for (int i = 0; i < rand.Next(2)+3; i++)//白色火山回响线性光22
            {
                Vector2 Position = new Vector2(
                rand.Next(-18, 53),//x -10 50
                rand.Next(-450, -50));//370
                Vector2 Shuihua = new Vector2(5f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float angle1 = (float)(Math.PI * 3 / 4);
                float speed = (float)(rand.NextDouble() * 4 + 3);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.03 + 0.01);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "火山回响线性光",//id
                    230f,//多少毫秒后开始动态变化
                    textureName: textureName1,
                    sourceRect: new Rectangle(0, 0, 112, 112),
                    animationInterval: 6000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua + Position,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: 0.2f,
                    scaleChange: scaleChange,
                    rotation: 0,
                    rotationChange: 0
                );
                float speed1 = Math.Abs(scaleChange) * 85f;
                particle.motion = new Vector2(
                    (float)Math.Cos(angle1) * speed1,//-往左偏，+往右偏
                    -(float)Math.Sin(angle1) * speed1 //（-下+上）
                );
                particle.motion += new Vector2(
                    0f,
                    speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }

        //光柱用
        public void VolcanicEchoesLeaf(GameLocation location, Vector2 epicenter)
        {
            //
            string textureName = "Mods/EyjaVELeaf";
            for (int i = 0; i < 1; i++)//白色闪烁光团
            {
                Vector2 Position = new Vector2(
                rand.Next(-10, 50),//x
                rand.Next(-300, -50));
                Vector2 Shuihua = new Vector2(26f, 55f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float angle1 = (float)(Math.PI * 3 / 4);
                float speed = (float)(rand.NextDouble() * 1 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.03 + 0.01);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() * 0.06 + 0.02f);//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float initialRotation = (float)(rand.NextDouble() * Math.PI * 2);
                // 创建粒子对象
                var particle = new EyjaTAS(
                    "火山回响叶子",//id
                    400f,//多少毫秒后开始动态变化
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 112, 112),
                    animationInterval: 4000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White*0f,
                    scale: 0.1f,
                    scaleChange: 0.02f,//07是55 1是80(scaleChange*80f)
                    rotation: 0,
                    rotationChange: 0
                );
                float speed1 = 1.6f;
                particle.motion = new Vector2(
                    (float)Math.Cos(angle1) * speed1,//-往左偏，+往右偏
                    -(float)Math.Sin(angle1) * speed1 //（-下+上）
                );
                particle.motion += new Vector2(
                    0f,
                    -0.2f
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }

        //火山回响光柱底下用
        public void CreateWhiteLightBall2(GameLocation location, Vector2 epicenter)
        {
            //
            string textureName = "Mods/EyjaWhiteLightBall";
            for (int i = 0; i < rand.Next(7, 10); i++)//白色闪烁光团
            {

                Vector2 Shuihua = new Vector2(30f, 50f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float angle1 = (float)(Math.PI * 3 / 4);
                float speed = (float)(rand.NextDouble() * 0.75 + 0.15);//（2,4）的速度（NextDouble()是[0，1））
                float scale = (float)(rand.NextDouble() * 0.7 + 0.2);//（2,4）的速度（NextDouble()是[0，1））
                float scaleChange = (float)(rand.NextDouble() * 0.03 + 0.01);//（2,4）的速度（NextDouble()是[0，1））
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
                    position: epicenter + Shuihua,
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
                    (float)Math.Cos(angle1) * speed1,//-往左偏，+往右偏
                    -(float)Math.Sin(angle1) * speed1 //（-下+上）
                );
                particle.motion += new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
    }
}
