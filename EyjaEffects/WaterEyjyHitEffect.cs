using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Companions.WaterEyjaCompanion;

namespace EyjaTrinket.EyjaEffects
{
    public class WaterEyjyHitEffect
    {
        private Random rand;
        public GameLocation location;
        public WaterEyjyHitEffect(GameLocation location)
        {
            rand = new Random();
            this.location = location;
        }

        public void CreateExplosion(GameLocation location, Vector2 epicenter)
        {
            Random rand = new Random();
            //float drop = 0.03f;
            //float drop = 0.03f;
            for (int i = 0; i < 5; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

                //  （泡泡
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBubble",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.015f,
                    color: Color.White,
                    scale: 1.2f,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );

                // 设置运动参数
                particle.motion = new Vector2(
                     (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 3; i++)
            {
                //随机生成一个角度（0到360度）
                float angle = (float)(Game1.random.NextDouble() * Math.PI * 2);

                //根据角度计算运动方向（速度向量）
                float speed = 4f; // 粒子移动速度
                Vector2 motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                // 计算粒子旋转角度（使头部对准运动方向）
                float rotation = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2;

                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(1 + rand.NextDouble());//大小()
                                                              //水滴
                var particle = new TemporaryAnimatedSprite(
                     textureName: "Mods/EyjaWaterDroplet",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.025f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: rotation,
                    rotationChange: 0
                    );
                // 5. 设置粒子运动方向和加速度
                particle.motion = motion;
                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 6; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                float scale1 = (float)(0.2 + rand.NextDouble() * 1.5);//大小()
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

                //  （十字，蓝块，白块）
                var particle = new TemporaryAnimatedSprite(
                    "Mods/EyjaBlueShiZi", new Rectangle(0, 0, 32, 32), 600f, 1, 1, epicenter, false, false, 0.9f, 0.02f, Color.White, scale1, 0f, 0f, 0.2f
                );
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 5; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float scale1 = (float)(0.6 + rand.NextDouble() * 2.5);//大小()
                                                                      //  
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaWhiteBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 600f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBlueBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 600f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle2 = new TemporaryAnimatedSprite(
                   textureName: "Mods/EyjaWaterAnimation",
                   sourceRect: new Rectangle(0, 0, 7, 7),
                   animationInterval: 50f,
                   animationLength: 15,
                   numberOfLoops: 1,
                   position: epicenter,
                   flicker: false,
                   flipped: false,
                   layerDepth: 0.9f,
                   alphaFade: 0.011f,
                   color: Color.White,
                   scale: scale1,
                   scaleChange: -0.015f,
                   rotation: 0f,
                   rotationChange: 0.05f
               );
                // 
                particle.motion = particle2.motion = particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(particle);
                location.temporarySprites.Add(particle2);
            }
            for (int i = 0; i < 18; i++)//水花
            {
                Vector2 Shuihua = new Vector2(-20f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 4);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float scale1 = (float)(0.6 + rand.NextDouble() * 2.5);//大小()
                                                                      //  
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaWhiteBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 800f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.018f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBlueBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 800f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.018f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                // 
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed - 7f
                );
                particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed - 7f
                );
                particle.acceleration = new Vector2(0, 0.3f);  // 增加下落加速度
                particle1.acceleration = new Vector2(0, 0.3f);  // 增加下落加速度
                                                                //particle.acceleration = new Vector2(0, 0.3f);
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(particle);
            }

            location.playSound("EyjaFireBoom");

        }
        public void CreateSpecialExplosion(GameLocation location, Vector2 epicenter)
        {
            Random rand = new Random();
            //float drop = 0.03f;
            //float drop = 0.03f;
            for (int i = 0; i < 10; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 1);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

                //  （泡泡
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBubble",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1500f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.009f,
                    color: Color.White,
                    scale: (float)(0.7 + rand.NextDouble()),
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );

                // 设置运动参数
                particle.motion = new Vector2(
                     (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                // 设置加速度（重力）
                //particle.acceleration = new Vector2(0, 0.3f);  // 增加下落加速度
                //particle.fireworkType =2;
                // 空气阻力？
                //particle.accelerationChange = new Vector2(0.01f, 0.01f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 3; i++)
            {
                float angle = (float)(Game1.random.NextDouble() * Math.PI * 2);

                // 根据角度计算运动方向（速度向量）
                float speed = 4f; // 粒子移动速度
                Vector2 motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                //使头部对准运动方向
                float rotation = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2;

                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(1 + rand.NextDouble());//大小()
                                                              //水滴
                var particle = new TemporaryAnimatedSprite(
                     textureName: "Mods/EyjaWaterDroplet",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.025f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: rotation,
                    rotationChange: 0
                    );

                particle.motion = motion; // 向外运动

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 6; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                float scale1 = (float)(0.2 + rand.NextDouble() * 1.5);//大小()
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

                //  （十字，蓝块，白块）
                var particle = new TemporaryAnimatedSprite(
                    "Mods/EyjaBlueShiZi", new Rectangle(0, 0, 32, 32), 600f, 1, 1, epicenter, false, false, 0.9f, 0.02f, Color.White, scale1, 0f, 0f, 0.2f
                );
                // 
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 5; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float scale1 = (float)(0.6 + rand.NextDouble() * 2.5);//大小()
                                                                      //  
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaWhiteBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 600f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBlueBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 600f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                // 
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 18; i++)//向上水花
            {
                Vector2 Shuihua = new Vector2(-20f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 4);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float scale1 = (float)(0.6 + rand.NextDouble() * 2.5);//大小()
                                                                      //  
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaWhiteBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1500f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.011f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBlueBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1500f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.011f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle2 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaWaterAnimation",
                    sourceRect: new Rectangle(0, 0, 7, 7),
                    animationInterval: 50f,
                    animationLength: 15,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.011f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                // 
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed - 10f
                );
                particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed - 10f
                );
                particle2.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed - 10f
                );
                particle.acceleration = new Vector2(0, 0.3f);  // 增加下落加速度
                particle1.acceleration = new Vector2(0, 0.3f);  // 增加下落加速度
                particle2.acceleration = new Vector2(0, 0.3f);
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(particle);
                location.temporarySprites.Add(particle2);
            }
            for (int i = 0; i < 8; i++)//星星
            {
                Vector2 Shuihua = new Vector2(-50f, -50f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2.5 + 1.5);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                float scale1;//大小()
                float Big = (float)(rand.NextDouble());
                if (Big < 0.2)
                {
                    scale1 = (float)(2 + rand.NextDouble() * 1);//0.6+2.7==3.3,-2.3
                }
                else
                {
                    scale1 = (float)(0.6 + rand.NextDouble() * 1);//0.6+2.7==0.6-2.6
                }
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaPinkStar",
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 2000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.05f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );

                // 设置运动参数

                particle1.motion = new Vector2(
                     (float)Math.Cos(angle) * speed + 1f,
                    (float)Math.Sin(angle) * speed + 1f
                );



                location.temporarySprites.Add(particle1);
            }
            for (int i = 0; i < 8; i++)//星星
            {
                Vector2 Shuihua = new Vector2(-50f, -50f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2.5 + 1.5);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                float scale1;//大小()

                float Big = (float)(rand.NextDouble());
                if (Big < 0.2)
                {
                    scale1 = (float)(2 + rand.NextDouble() * 1);//0.6+2.7==3.3,-2.3
                }
                else
                {
                    scale1 = (float)(0.6 + rand.NextDouble() * 1);//0.6+2.7==0.6-2.6
                }
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaYellowStar",
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 2000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.05f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed + 1f,
                   (float)Math.Sin(angle) * speed + 1f
               );
                location.temporarySprites.Add(particle);
            }
            // 增加粒子数量
            int particleCount = rand.Next(8, 12);

            for (int i = 0; i < particleCount; i++)
            {
                // 随机角度和速度
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 0.4 + 0.1); // 0.1-0.5
                float alphaFade = ((float)(rand.NextDouble() * 0.0035 + 0.0025));
                // 初始运动向量（向上飘）
                Vector2 initialMotion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                   -(float)(rand.NextDouble() * 0.5 + 0.5)
                );
                Vector2 initialAcceleration = new Vector2(
                   (float)(rand.NextDouble() * 0.05 - 0.025), // 随机水平风力 (-0.005 到 +0.005)
                   (float)(rand.NextDouble() * 0.05 - 0.025) // 重力加速度（调低使下落更慢）
               );
                // 随机花瓣类型
                int HuaBanZhen = (rand.Next(0, 11) * 16);

                // 花瓣粒子
                var particle = new FloatingPetalParticle(
                    textureName: "Mods/EyjaPinkPetals",
                    sourceRect: new Rectangle(HuaBanZhen, 0, 16, 16),
                    animationInterval: 5000,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    alphaFade: alphaFade, // 缓慢消失
                    scale: (float)(1 + rand.NextDouble() * 2), // 0.7-1.3
                    rotation: (float)(rand.NextDouble() * Math.PI * 2),
                    rotationChange: (float)(0.03 + rand.NextDouble() * 0.03),
                    initialMotion: initialMotion,
                    initialAcceleration: initialAcceleration,
                    random: rand
                );

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 ZOffset18 = new Vector2(-110, -120);
                Vector2 ZOffset = new Vector2(10, 10);
                Vector2 ZOffset1 = new Vector2(-18, -15);
                var particle3 = new TemporaryAnimatedSprite(
                               textureName: "Mods/EyjaPurpleShine",
                               sourceRect: new Rectangle(0, 0, 64, 64),
                               animationInterval: 1500f,
                               animationLength: 1,
                               numberOfLoops: 1,
                               position: epicenter + ZOffset18 + ZOffset,
                               flicker: false,
                               flipped: false,
                               layerDepth: 0.6f,
                               alphaFade: 0.015f,
                               color: Color.White,
                               scale: 3f,
                               scaleChange: 0f,
                               rotation: 0f,
                               rotationChange: 0f
                           );

                var particle4 = new TemporaryAnimatedSprite(
                   textureName: "Mods/EyjaYellowShineGroup",
                   sourceRect: new Rectangle(0, 0, 64, 64),
                   animationInterval: 50,
                   animationLength: 8,
                   numberOfLoops: 1,
                   position: epicenter + ZOffset18 + ZOffset1,
                   flicker: false,
                   flipped: false,
                   layerDepth: 0.7f,
                   alphaFade: 0.02f,
                   color: Color.White,
                   scale: 4f,
                   scaleChange: 0f,
                   rotation: 0f,
                   rotationChange: 0f
               );
                var particle5 = new TemporaryAnimatedSprite(
                   textureName: "Mods/EyjaYellowShineGroup",
                   sourceRect: new Rectangle(64 * 6, 0, 64, 64),
                   animationInterval: 1000,
                   animationLength: 1,
                   numberOfLoops: 1,
                   position: epicenter + ZOffset18 + ZOffset1,
                   flicker: false,
                   flipped: false,
                   layerDepth: 0.7f,
                   alphaFade: 0.03f,
                   color: Color.White,
                   scale: 4f,
                   scaleChange: 0f,
                   rotation: 0f,
                   rotationChange: 0f
               );

                particle3.motion = new Vector2(
                0, 0);
                particle4.motion = new Vector2(
                0, 0);
                location.temporarySprites.Add(particle4);
            }
            location.playSound("EyjaFireBoom");
        }
        //火山水环特效
        public void AddVolcanoParticles(GameLocation location, Vector2 position)
        {
            Vector2 ZOffset18 = new Vector2(8, 8);
            Vector2 SOffset = new Vector2(-32, -32);
            Vector2 Offset36 = new Vector2(-36, -36);
            Vector2 Offset48 = new Vector2(-48, -48);
            Vector2 Offset64 = new Vector2(-64, -64);
            Vector2 newSOffset = new Vector2(-96, -96);
            Vector2 newShine7fOffset = new Vector2(-96, -96);//64
            Vector2 JiaoZheng = new Vector2(-45, -210);
            Vector2 Dianhu = new Vector2(50, 50);
            Vector2 _fireballOffset = new Vector2(-50f, -148f);
            Vector2 effectPosition = position + _fireballOffset + JiaoZheng; // 叠加偏移
            Random rand = new Random();
            int randomNumber = rand.Next(0, 4);
            int randomNumber1 = rand.Next(0, 2);
            int HuaBan = rand.Next(0, 25);
            if (randomNumber == 1)
            {
                for (int i = 0; i < randomNumber1; i++)
                {
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                    float speed = (float)(rand.NextDouble() * 0.9 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                    float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                    Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                    float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                    float scale1 = (float)(1.5 + rand.NextDouble() * 0.5);//大小()
                                                                          // 蓝色光球
                    var particle = new TemporaryAnimatedSprite(
                        textureName: "Mods/EyjaBlueBlock",
                        sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                        animationInterval: 2000f,//动画间隔：40f
                        animationLength: 1,//动画长度：15
                        numberOfLoops: 1,//循环次数：1
                        position: effectPosition + Dianhu,//位置：爆炸中心
                        flicker: false,//闪烁：否
                        flipped: false,//翻转：否
                        layerDepth: 0.9f,//层深度：0.9f
                        alphaFade: 0.01f,// （透明度渐变）：0.001f
                        color: Color.White,//颜色：白色
                        scale: scale1,//缩放：3.5f
                        scaleChange: 0f,//缩放变化：0f
                        rotation: 0f,//旋转：0f
                        rotationChange: rotationChange1//旋转变化：0.05f

                    );

                    // 
                    particle.motion = new Vector2(
                        (float)Math.Cos(angle) * speed,//x坐标（-7，7）-往左偏，+往右偏，非常简单的坐标系
                        (float)Math.Sin(angle) * speed //y坐标（-10，4）去掉-3f就能减少向下的偏移（-下+上）
                    );
                    //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                    location.temporarySprites.Add(particle);
                }
            }
            else if (randomNumber == 2)
            {
                for (int i = 0; i < randomNumber1; i++)
                {
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                    float speed = (float)(rand.NextDouble() * 0.9 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                    float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                    Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                    float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                    float scale1 = (float)(0.3 + rand.NextDouble() * 0.3);//大小()
                                                                          // 蓝色光球
                    var particle = new TemporaryAnimatedSprite(
                        textureName: "Mods/EyjaBubble",
                        sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                        animationInterval: 2000f,//动画间隔：40f
                        animationLength: 1,//动画长度：15
                        numberOfLoops: 1,//循环次数：1
                        position: effectPosition + Dianhu,//位置：爆炸中心
                        flicker: false,//闪烁：否
                        flipped: false,//翻转：否
                        layerDepth: 0.9f,//层深度：0.9f
                        alphaFade: 0.01f,// （透明度渐变）：0.001f
                        color: Color.White,//颜色：白色
                        scale: scale1,//缩放：3.5f
                        scaleChange: 0f,//缩放变化：0f
                        rotation: 0f,//旋转：0f
                        rotationChange: rotationChange1//旋转变化：0.05f

                    );

                    // 
                    particle.motion = new Vector2(
                        (float)Math.Cos(angle) * speed,//x坐标（-7，7）-往左偏，+往右偏，非常简单的坐标系
                        (float)Math.Sin(angle) * speed //y坐标（-10，4）去掉-3f就能减少向下的偏移（-下+上）
                    );
                    //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                    location.temporarySprites.Add(particle);
                }
            }
            else if (randomNumber == 3)
            {
                for (int i = 0; i < randomNumber1; i++)
                {
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                    float speed = (float)(rand.NextDouble() * 0.9 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                    float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                    Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                    float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                    float scale1 = (float)(1.5 + rand.NextDouble() * 1);//大小()
                                                                        // 蓝色光球
                    var particle = new TemporaryAnimatedSprite(
                        textureName: "Mods/EyjaWaterAnimation",
                        sourceRect: new Rectangle(0, 0, 7, 7),//源矩形：新的矩形(0, 0, 7, 7)
                        animationInterval: 80f,//动画间隔：40f
                        animationLength: 11,//动画长度：15
                        numberOfLoops: 1,//循环次数：1
                        position: effectPosition + Dianhu,//位置：爆炸中心
                        flicker: false,//闪烁：否
                        flipped: false,//翻转：否
                        layerDepth: 0.9f,//层深度：0.9f
                        alphaFade: 0.01f,// （透明度渐变）：0.001f
                        color: Color.White,//颜色：白色
                        scale: scale1,//缩放：3.5f
                        scaleChange: 0f,//缩放变化：0f
                        rotation: 0f,//旋转：0f
                        rotationChange: rotationChange1//旋转变化：0.05f

                    );

                    // 
                    particle.motion = new Vector2(
                        (float)Math.Cos(angle) * speed,//x坐标（-7，7）-往左偏，+往右偏，非常简单的坐标系
                        (float)Math.Sin(angle) * speed //y坐标（-10，4）去掉-3f就能减少向下的偏移（-下+上）
                    );
                    //particle.acceleration = new Vector2(0, 0.3f);//y轴加速度(+粒子向下飘,-粒子向上飘)

                    location.temporarySprites.Add(particle);
                }
            }
            if (HuaBan == 10)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 1 + 0.5);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.03);//角度()
                float scale1 = (float)(1.5 + rand.NextDouble() * 0.5);//大小()
                int HuaBanZhen = (rand.Next(0, 11) * 16);
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaPinkPetals",
                    sourceRect: new Rectangle(HuaBanZhen, 0, 16, 16),//源矩形：新的矩形(0, 0, 7, 7)16 32
                    animationInterval: 5000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: effectPosition + Dianhu,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.005f,// （透明度渐变）：0.001f
                    color: Color.White,//颜色：白色
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: 0f,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f

                );

                // 花瓣飘落参数
                float horizontalVariation = (float)(rand.NextDouble() * 0.4 - 0.2); // -0.2 到 +0.2
                float verticalFloat = (float)(rand.NextDouble() * 0.15 + 0.1); // 0.2-0.5

                // 运动参数 - 更自然的飘落
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,//
                    (float)Math.Sin(angle) * speed  // 负值使花瓣向上飘再下落
                );

                //// 关键改进：添加空气阻力/风力效果
                particle.acceleration = new Vector2(
                    (float)(rand.NextDouble() * 0.005 - 0.0025), // 随机水平风力 (-0.005 到 +0.005)
                    (float)(rand.NextDouble() * 0.005 - 0.0025) // 重力加速度（调低使下落更慢）
                );

                location.temporarySprites.Add(particle);

            }
        }
        //日常粉色花瓣特效
        public void AddCasualParticles(GameLocation location, Vector2 position)
        {
            Vector2 ZOffset18 = new Vector2(32, 50);
            Vector2 SOffset = new Vector2(-32, -32);
            Vector2 Offset36 = new Vector2(-36, -36);
            Vector2 Offset48 = new Vector2(-48, -48);
            Vector2 Offset64 = new Vector2(-64, -64);
            Vector2 newSOffset = new Vector2(-96, -96);
            Vector2 newShine7fOffset = new Vector2(-96, -96);//64
            Vector2 _fireballOffset = new Vector2(-50f, -148f);
            Vector2 Dianhu = new Vector2(50, 50);
            Vector2 effectPosition = position + _fireballOffset + ZOffset18; // 叠加偏移
            Random rand = new Random();
            int randomNumber = rand.Next(1, 3);
            int randomNumber1 = rand.Next(0, 10);

            for (int i = 0; i < randomNumber; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 1 + 0.5);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.03);//角度()
                float scale1 = (float)(1.5 + rand.NextDouble() * 0.5);//大小()
                int HuaBanZhen = (rand.Next(0, 11) * 16);
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaPinkPetals",
                    sourceRect: new Rectangle(HuaBanZhen, 0, 16, 16),//源矩形：新的矩形(0, 0, 7, 7)16 32
                    animationInterval: 5000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: effectPosition,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.005f,// （透明度渐变）：0.001f
                    color: Color.White,//颜色：白色
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: 0f,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f

                );

                // 花瓣飘落
                float horizontalVariation = (float)(rand.NextDouble() * 0.4 - 0.2); // -0.2 到 +0.2
                float verticalFloat = (float)(rand.NextDouble() * 0.15 + 0.1); // 0.2-0.5

                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,//
                    (float)Math.Sin(angle) * speed  // 负值使花瓣向上飘再下落
                );

                particle.acceleration = new Vector2(
                    (float)(rand.NextDouble() * 0.005 - 0.0025),
                    (float)(rand.NextDouble() * 0.005 - 0.0025)
                );

                location.temporarySprites.Add(particle);

            }

        }
        //火山子弹弹幕
        public void VCreateExplosion(GameLocation location, Vector2 epicenter)
        {
            Random rand = new Random();
            //float drop = 0.03f;
            //float drop = 0.03f;
            for (int i = 0; i < 5; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

                //  （泡泡
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBubble",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.015f,
                    color: Color.White,
                    scale: 1.2f,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );

                particle.motion = new Vector2(
                     (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 3; i++)
            {
                float angle = (float)(Game1.random.NextDouble() * Math.PI * 2);

                float speed = 4f; // 粒子移动速度
                Vector2 motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                float rotation = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2;

                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(1 + rand.NextDouble());//大小()
                                                              //水滴
                var particle = new TemporaryAnimatedSprite(
                     textureName: "Mods/EyjaWaterDroplet",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.025f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: rotation,
                    rotationChange: 0
                    );

                particle.motion = motion;
                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 6; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                float scale1 = (float)(0.2 + rand.NextDouble() * 1.5);//大小()
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

                //  （十字，蓝块，白块）
                var particle = new TemporaryAnimatedSprite(
                    "Mods/EyjaBlueShiZi", new Rectangle(0, 0, 32, 32), 600f, 1, 1, epicenter, false, false, 0.9f, 0.02f, Color.White, scale1, 0f, 0f, 0.2f
                );
                // 
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 5; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float scale1 = (float)(0.6 + rand.NextDouble() * 2.5);//大小()
                                                                      //  
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaWhiteBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 600f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBlueBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 600f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle2 = new TemporaryAnimatedSprite(
                   textureName: "Mods/EyjaWaterAnimation",
                   sourceRect: new Rectangle(0, 0, 7, 7),
                   animationInterval: 50f,
                   animationLength: 15,
                   numberOfLoops: 1,
                   position: epicenter,
                   flicker: false,
                   flipped: false,
                   layerDepth: 0.9f,
                   alphaFade: 0.011f,
                   color: Color.White,
                   scale: scale1,
                   scaleChange: -0.015f,
                   rotation: 0f,
                   rotationChange: 0.05f
               );
                // 
                particle.motion = particle2.motion = particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(particle);
                location.temporarySprites.Add(particle2);
            }
            for (int i = 0; i < 18; i++)//水花
            {
                Vector2 Shuihua = new Vector2(-20f, 0f);
                float angle = (float)(rand.NextDouble() * Math.PI * 4);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float scale1 = (float)(0.6 + rand.NextDouble() * 2.5);//大小()
                                                                      //  
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaWhiteBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 800f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.018f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaBlueBlock",
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 800f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.018f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.015f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                // 
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed - 7f
                );
                particle1.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed - 7f
                );
                particle.acceleration = new Vector2(0, 0.3f);  // 增加下落加速度
                particle1.acceleration = new Vector2(0, 0.3f);  // 增加下落加速度
                                                                //particle.acceleration = new Vector2(0, 0.3f);
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 4; i++)//星星
            {
                Vector2 Shuihua = new Vector2(-30f, -40f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2.5 + 1.5);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                float scale1;//大小()
                float Big = (float)(rand.NextDouble());

                scale1 = (float)(0.6 + rand.NextDouble() * 1);//0.6+2.7==0.6-2.6

                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaPinkStar",
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 2000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.03f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );

                // 设置运动参数

                particle1.motion = new Vector2(
                     (float)Math.Cos(angle) * speed + 1f,
                    (float)Math.Sin(angle) * speed + 1f
                );

                location.temporarySprites.Add(particle1);
            }
            for (int i = 0; i < 4; i++)//星星
            {
                Vector2 Shuihua = new Vector2(-30f, -40f);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2.5 + 1.5);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                float scale1;//大小()

                float Big = (float)(rand.NextDouble());

                scale1 = (float)(0.6 + rand.NextDouble() * 1);//0.6+2.7==0.6-2.6

                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaYellowStar",
                    sourceRect: new Rectangle(0, 0, 48, 48),
                    animationInterval: 2000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter + Shuihua,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0f,
                    color: Color.White,
                    scale: scale1,
                    scaleChange: -0.03f,
                    rotation: 0f,
                    rotationChange: 0.05f
                );
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed + 1f,
                   (float)Math.Sin(angle) * speed + 1f
               );
                location.temporarySprites.Add(particle);
            }
            // 增加粒子数量
            int particleCount = rand.Next(1, 3);

            for (int i = 0; i < particleCount; i++)
            {
                // 随机角度和速度
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 0.4 + 0.1); // 0.1-0.5
                float alphaFade = ((float)(rand.NextDouble() * 0.0035 + 0.0025));
                // 初始运动向量（向上飘）
                Vector2 initialMotion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                   -(float)(rand.NextDouble() * 0.5 + 0.5)  // 负值=向上
                );
                Vector2 initialAcceleration = new Vector2(
                   (float)(rand.NextDouble() * 0.05 - 0.025), // 随机水平风力 (-0.005 到 +0.005)
                   (float)(rand.NextDouble() * 0.05 - 0.025) // 重力加速度（调低使下落更慢）
               );
                // 随机花瓣类型
                int HuaBanZhen = (rand.Next(0, 11) * 16);

                // 创建自定义花瓣粒子
                var particle = new FloatingPetalParticle(
                    textureName: "Mods/EyjaPinkPetals",
                    sourceRect: new Rectangle(HuaBanZhen, 0, 16, 16),
                    animationInterval: 5000,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: epicenter,
                    alphaFade: alphaFade, // 缓慢消失
                    scale: (float)(1 + rand.NextDouble() * 2), // 0.7-1.3
                    rotation: (float)(rand.NextDouble() * Math.PI * 2),
                    rotationChange: (float)(0.03 + rand.NextDouble() * 0.03),
                    initialMotion: initialMotion,
                    initialAcceleration: initialAcceleration,
                    random: rand
                );

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 ZOffset18 = new Vector2(-64, -67);
                var particle3 = new TemporaryAnimatedSprite(
                               textureName: "Mods/EyjaPurpleShine",
                               sourceRect: new Rectangle(0, 0, 64, 64),
                               animationInterval: 1500f,
                               animationLength: 1,
                               numberOfLoops: 1,
                               position: epicenter + ZOffset18,
                               flicker: false,
                               flipped: false,
                               layerDepth: 0.7f,
                               alphaFade: 0.018f,
                               color: Color.White,
                               scale: 4f,
                               scaleChange: 0f,
                               rotation: 0f,
                               rotationChange: 0f
                           );

                var particle4 = new TemporaryAnimatedSprite(
                   textureName: "Mods/EyjaYellowShineGroup",
                   sourceRect: new Rectangle(0, 0, 64, 64),
                   animationInterval: 50,
                   animationLength: 8,
                   numberOfLoops: 1,
                   position: epicenter + ZOffset18,
                   flicker: false,
                   flipped: false,
                   layerDepth: 1f,
                   alphaFade: 0.02f,
                   color: Color.White * 0.5f,
                   scale: 2f,
                   scaleChange: 0f,
                   rotation: 0f,
                   rotationChange: 0f
               );

                particle3.motion = new Vector2(
                0, 0);
                particle4.motion = new Vector2(
                0, 0);
                //location.temporarySprites.Add(particle3);
                location.temporarySprites.Add(particle4);
            }

            location.playSound("EyjaFireBoom");

        }
        //花瓣粒子
        public class FloatingPetalParticle : TemporaryAnimatedSprite
        {
            private Random rand;
            private Vector2 baseMotion;
            private Vector2 baseAcceleration;
            private float windForce;
            private float floatTimer;
            private float floatDuration;
            private float rotationSpeed;
            private float windForceX;
            private float windForceY;

            public FloatingPetalParticle(
                string textureName,
                Rectangle sourceRect,
                float animationInterval,
                int animationLength,
                int numberOfLoops,
                Vector2 position,
                float alphaFade,
                float scale,
                float rotation,
                float rotationChange,
                Vector2 initialMotion,
                Vector2 initialAcceleration,
                Random random)
                : base(textureName, sourceRect, animationInterval, animationLength, numberOfLoops,
                      position, false, false, 0.9f, alphaFade, Color.White,
                      scale, 0f, rotation, rotationChange)
            {
                rand = random;
                baseMotion = initialMotion;
                baseAcceleration = initialAcceleration;
                // 初始化随机参数
                windForce = (float)(rand.NextDouble() * 0.01f - 0.005f);
                floatDuration = (float)(rand.NextDouble() * 2 + 1.5); // 2-5秒的浮动周期
                rotationSpeed = rotationChange;

                // 初始运动
                motion = initialMotion;
                acceleration = initialAcceleration;
                // 添加随机偏移
                position += new Vector2(
                    (float)(rand.NextDouble() * 20 - 10),
                    (float)(rand.NextDouble() * 20 - 10)
                );
            }
            public class FigureEightParticle : TemporaryAnimatedSprite
            {
                private Vector2 center;
                private float elapsedTime;
                private readonly float duration;
                private readonly float size;


                public override bool update(GameTime time)
                {
                    elapsedTime += (float)time.ElapsedGameTime.TotalMilliseconds;

                    
                    float t = elapsedTime / duration;
                    if (t >= 1f) return true; // 动画结束

                    float angle = t * MathHelper.TwoPi;
                    float x = size * (float)Math.Sin(angle);
                    float y = size * (float)Math.Sin(2 * angle) / 2;

                    // 更新粒子位置
                    Position = center + new Vector2(x, y);

                    return base.update(time);
                }
            }
            public override bool update(GameTime time)
            {

                floatTimer += (float)time.ElapsedGameTime.TotalMilliseconds;

                // 每过一段时间改变浮动方向
                if (floatTimer > floatDuration)
                {
                    floatTimer = 0;
                    floatDuration = (float)(rand.NextDouble() * 500 + 1000); // 新周期
                }

                float acceleration111 = (float)(rand.NextDouble() * 0.2 + 0.2);//0-1

                acceleration = new Vector2(
                    (float)(rand.NextDouble() * acceleration111 - acceleration111 * 0.5), // 随机水平风力
                   (float)(rand.NextDouble() * acceleration111 - acceleration111 * 0.5));

                // 缓慢缩小
                scale = Math.Max(0.1f, scale - 0.001f);

                return base.update(time);
            }
        }
    }
}
