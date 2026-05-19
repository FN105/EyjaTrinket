using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EyjaTrinket.ProjectileTAS
{
    public class EyjaTAS : TemporaryAnimatedSprite
    {
        private float growDuration;
        private float shrinkDuration;
        private bool isGrowing = true;
        private float totalTimer;
        private string textureName1;
        private float scaleChange1;//用来存原始数值
        //闪烁状态
        private int flashCount = 0; // 记录已经完成的闪烁次数
        private bool isFirstFlash = true; // 是否是第一次闪烁
        private float originalScaleChange; // 保存原始的scaleChange值
        public string ParticleId { get; }
        private Dictionary<string, bool> canTrigger = new Dictionary<string, bool>();

        // 闪烁星星2的特定字段
        private int flashCount2 = 0; // 记录闪烁星星2的闪烁次数
        private bool isGrowing2 = true; // 记录当前是否处于变大阶段
        private bool shouldScaleChangeInvert = false; // 标记是否需要反转scaleChange
        private float targetScale = 0f; // 目标放大尺寸
        private int flashCount3 = 0; //用来只执行一次变化大小
        private int VELeafCount = 0; //用来只执行一次变化大小
        //没有闪烁，反转
        public EyjaTAS(string LiZiId, float growDur, string textureName, Rectangle sourceRect, float animationInterval, int animationLength, int numberOfLoops, Vector2 position, float layerDepth, float alphaFade, Color color, float scale, float scaleChange, float rotation, float rotationChange)
            : base(textureName, sourceRect, animationInterval, animationLength, numberOfLoops, position, false, false, layerDepth, alphaFade, color, scale, scaleChange, rotation, rotationChange)
        {
            ParticleId = LiZiId;
            growDuration = growDur; // 多少毫秒开始变
            canTrigger[ParticleId] = true;
            if (ParticleId == "垂直叶子" || ParticleId == "垂直星星" || ParticleId == "闪烁星星1" || ParticleId == "白色闪烁光团" || ParticleId == "闪烁星星2"|| ParticleId == "火山回响线性光")
            {
                scaleChange1 = this.scaleChange; // 反转传入的 scaleChange
            }
        }


        //闪烁星星2用（垂直下降火山回响用的）

        public override bool update(GameTime time)
        {

            totalTimer += (float)time.ElapsedGameTime.TotalMilliseconds;

            if (ParticleId == "蓝色光柱")
            {
                float fadeInTime = 200f;    
                float holdTime = 100f;       
                float fadeOutTime = 200f;
                float Alpha = 0.7f;

                if (totalTimer <= fadeInTime)
                {
                    float progress = totalTimer / fadeInTime;
                    float alpha = MathHelper.Lerp(0f, Alpha, progress);
                    color = Color.White * alpha;
                }
                else if (totalTimer <= fadeInTime + holdTime)
                {
                    color = Color.White * Alpha;
                }
                else if (totalTimer <= fadeInTime + holdTime + fadeOutTime)
                {
                    float progress = (totalTimer - fadeInTime - holdTime) / fadeOutTime;
                    float alpha = MathHelper.Lerp(Alpha, 0f, progress);
                    color = Color.White * alpha;
                }
                else
                {
                    color = Color.White * 0f;
                }

                return base.update(time);
            }
            if (ParticleId == "白色光柱")
            {
                float fadeInTime = 250f;
                float holdTime = 50f;
                float fadeOutTime = 100f;
                float Alpha = 0.7f;

                if (totalTimer <= fadeInTime)
                {
                    float progress = totalTimer / fadeInTime;
                    float alpha = MathHelper.Lerp(0f, Alpha, progress);
                    color = Color.White * alpha;
                }
                else if (totalTimer <= fadeInTime + holdTime)
                {
                    color = Color.White * Alpha;
                }
                else if (totalTimer <= fadeInTime + holdTime + fadeOutTime)
                {
                    float progress = (totalTimer - fadeInTime - holdTime) / fadeOutTime;
                    float alpha = MathHelper.Lerp(Alpha, 0f, progress);
                    color = Color.White * alpha;
                }
                else
                {
                    color = Color.White * 0f;
                }

                return base.update(time);
            }
            if (ParticleId == "闪烁星星2")
            {
                
                if (totalTimer <= growDuration)//还没到目标时间，先记录和1变大
                {
                    isGrowing2 = true;
                    targetScale = scale;
                }//然后就是totalTimer >= growDuration
                else//到时间
                {
                    //1变小
                    if (isGrowing2 && flashCount2 < 2)
                    {
                        scaleChange = -scaleChange1;
                        if (flashCount3 == 0)//这是会反复执行的，所以要这么干，只运行一次
                        {
                            flashCount3++;
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = (0.08f + Math.Abs(scaleChange) * 30f) * 2;
                            motion -= new Vector2((float)Math.Cos(angle) * speed, -(float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }

                        if (scale <= 0.1f)
                        {
                            isGrowing2 = false;
                            flashCount2++;
                        }
                    }
                    if (!isGrowing2 && flashCount2 < 2)//2变大
                    {
                        scaleChange = scaleChange1;
                        if (flashCount3 == 1)//这是会反复执行的，所以要这么干，只运行一次
                        {
                            flashCount3++;
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = (0.08f + Math.Abs(scaleChange) * 30f) * 2;
                            motion += new Vector2((float)Math.Cos(angle) * speed, -(float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                        if (scale >= targetScale)
                        {
                            isGrowing2 = true;
                            flashCount2++;
                        }
                    }
                    if (flashCount2 >= 2)
                    {
                        scaleChange = -scaleChange1;
                        if (flashCount3 == 2)//这是会反复执行的，所以要这么干，只运行一次
                        {
                            flashCount3++;
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = (0.08f + Math.Abs(scaleChange) * 30f) * 2;
                            motion -= new Vector2((float)Math.Cos(angle) * speed, -(float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                    }
                     return base.update(time);
                }
            }
            if (ParticleId == "火山回响叶子")
            {
                float fadeInTime = 250f;
                float holdTime = 50f;
                float fadeOutTime = 200f;
                float Alpha = 1f;

                if (totalTimer <= fadeInTime)
                {
                    float progress = totalTimer / fadeInTime;
                    float alpha = MathHelper.Lerp(0f, Alpha, progress);
                    color = Color.White * alpha;
                }
                else if (totalTimer <= fadeInTime + holdTime)
                {
                    color = Color.White * Alpha;
                    scaleChange = 0.005f;
                    if (VELeafCount == 0)
                    {
                        VELeafCount++;
                        float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                        float speed = Math.Abs(scaleChange) * 80f;
                        motion = new Vector2((float)Math.Cos(angle) * speed,-(float)Math.Sin(angle) * speed);
                        canTrigger[ParticleId] = false;
                    }
                }
                else if (totalTimer <= fadeInTime + holdTime + fadeOutTime)
                {
                    float progress = (totalTimer - fadeInTime - holdTime) / fadeOutTime;
                    float alpha = MathHelper.Lerp(Alpha, 0f, progress);
                    color = Color.White * alpha;
                }
                else
                {
                    color = Color.White * 0f;
                }

                return base.update(time);
            }
            if (ParticleId == "白色闪烁光团")
            {
                float A = growDuration;
                float B = 75f;//额外消失时间
                               // 第一次闪烁
                if (scale <= 0.1f&&flashCount <= 2)
                {
                    scale = 0.1f; // 设置最小尺寸
                }
                //else if(flashCount >= 3)//还有总时间长度
                //{
                //    scale = 0f;
                //}
                if (totalTimer >= A && flashCount == 0)
                {
                    scaleChange = -scaleChange1; // 变小
                    flashCount = 1;
                    if (canTrigger[ParticleId])
                    {
                        float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                        float speed = Math.Abs(scaleChange) * 40f;
                        motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                        canTrigger[ParticleId] = false;
                    }

                }
                // 第二次闪烁
                else if (totalTimer >= A * 2 + B && flashCount == 1)
                {
                    scaleChange = scaleChange1; // 再次变大
                    flashCount = 2;
                    canTrigger[ParticleId] = true;
                    if (canTrigger[ParticleId])
                    {
                        float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                        float speed = Math.Abs(scaleChange) * 40f;
                        motion += new Vector2((float)Math.Cos(angle) * speed, -(float)Math.Sin(angle) * speed);
                        canTrigger[ParticleId] = false;
                    }
                }
                // 第三次变化
                else if (totalTimer >= A * 3 + B && flashCount == 2)
                {
                    scaleChange = -scaleChange1; // 最终变小
                    flashCount = 3;
                    canTrigger[ParticleId] = true;
                    if (canTrigger[ParticleId])
                    {
                        float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                        float speed = Math.Abs(scaleChange) * 40f;
                        motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                        canTrigger[ParticleId] = false;
                    }
                    
                }
                //}else if (flashCount == 3 && totalTimer >= A * 4 + B)
                //{
                //    scale = 0f;
                //}
            }
            else
            {

                if (totalTimer >= growDuration)
                {
                    if (ParticleId == "叶子")
                    {
                        scaleChange = -0.02f;
                        if (canTrigger[ParticleId])//这个if判断是为了只运行一次下面的内容
                        {
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = 0.65f;
                            motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                    }
                    if (ParticleId == "星星")
                    {
                        scaleChange = -0.02f;
                        if (canTrigger[ParticleId])
                        {
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = 0.65f;
                            motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                    }
                    if (ParticleId == "蓝光团")
                    {
                        alphaFade = 0.02f;
                    }
                    if (ParticleId == "水花")
                    {
                        alphaFade = 0.05f;
                    }
                    if (ParticleId == "白色闪烁光团1")
                    {
                        alphaFade = 0.03f;
                    }
                    if (ParticleId == "16小叶子")
                    {
                        alphaFade = 0.03f;
                    }
                    if (ParticleId == "火山回响水环")
                    {
                        alphaFade = 0.1f;
                    }
                    if (ParticleId == "火山回响光环")
                    {
                        alphaFade = 0.1f;
                    }
                    if (ParticleId == "火山回响线性光")
                    {
                        alphaFade = 0.03f;
                        scaleChange = -scaleChange1;
                        if (canTrigger[ParticleId])
                        {
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = Math.Abs(scaleChange) * 85f*2;
                            motion -= new Vector2((float)Math.Cos(angle) * speed, -(float)Math.Sin(angle) * speed); 
                            canTrigger[ParticleId] = false;
                        }
                    }
                    if (ParticleId == "光斑")
                    {
                        alphaFade = 0.03f;
                    }
                    if (ParticleId == "白光团" || ParticleId == "EyjaBeams")
                    {
                        alphaFade = 0.03f;
                        scaleChange = -0.06f;
                        if (canTrigger[ParticleId])
                        {
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = 4.1f;
                            motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                    }
                    if (ParticleId == "白光团1")
                    {
                        alphaFade = 0.03f;
                        scaleChange = -0.03f;
                        if (canTrigger[ParticleId])
                        {
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = 2f;
                            motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                    }
                    if (ParticleId == "垂直叶子" || ParticleId == "垂直星星")
                    {
                        scaleChange = -scaleChange1;
                        if (canTrigger[ParticleId])
                        {
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = Math.Abs(scaleChange) * 72f;
                            motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                    }
                    if (ParticleId == "闪烁星星1")
                    {
                        scaleChange = -scaleChange1;
                        if (canTrigger[ParticleId])
                        {
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = 0.16f + Math.Abs(scaleChange) * 60f;
                            motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                    }
                    if (ParticleId == "白色1闪烁光团")
                    {
                        scaleChange = -scaleChange1;
                        alphaFade = 0.01f;
                        if (canTrigger[ParticleId])
                        {
                            float angle = (float)(Math.PI * 3 / 4); // 135度的弧度值
                            float speed = 0.16f + Math.Abs(scaleChange) * 60f;
                            motion += new Vector2(-(float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
                            canTrigger[ParticleId] = false;
                        }
                    }
                }

            }
            return base.update(time);
        }
    }
}
