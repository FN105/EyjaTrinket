using System;
using System.Reflection.Metadata;
using EyjaTrinket;
using EyjaTrinket.EyjaEffects;
using EyjaTrinket.ProjectileTAS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;
using static StardewValley.Minigames.TargetGame;

namespace StardewValley.Companions;

public enum ParticleType
{
    Petal,
    Water,WhiteWater,
    PinkStar,YellowStar,
    OtherWater1,OtherWater2,OtherWater3,
    OtherPetal1, OtherPetal2, OtherPetal3,
}

// 粒子配置类
public class ParticleConfig
{
    public ParticleType Type { get; set; }
    public float Size { get; set; }
    public float PhaseOffset { get; set; }
    public float RotationSpeed { get; set; }
    public float Scale { get; set; }
    public float Alpha { get; set; }
    public int TextureVariant { get; set; }
    public float SizeVariation { get; set; } // 大小变化幅度
    public float SpeedFactor { get; set; } // 运动速度因子
    public Vector2 CenterOffice { get; set; } // 中心偏移
    public Vector2 JiaoZhengOffice { get; set; } // 矫正偏移
    public int Rectangle { get; set; }//大小框架
    public float ElapsedTime { get; set; }//no
    public int isReverse { get; set; }//用来随机方向
    public float MinAlpha { get; set; } = 0f; // 顶部最小透明度
    public float MaxAlpha { get; set; } = 1.0f; // 底部最大透明度
    public float AlphaCurve { get; set; } = 1f; // 透明度曲线强度
    public float SpawnDelay { get; set; }     // 粒子生成延迟时间 (0-3000ms)
    public bool IsActive { get; set; }        // 粒子是否激活
    public float LifeProgress { get; set; }   // 粒子生命周期进度 (0.0-1.0)

    public bool VStar { get; set; } = false;  //火山前后
    public int VStarShunXu { get; set; }  //星星顺序

    public float StartPositionT { get; set; } // 粒子在轨迹上的初始位置 (0~1)
}
public class WaterEyjaCompanion : Companion
{
    
    public const int VARIANT_FAIRY = 0;

    public const int VARIANT_PARROT = 1;

    private float flitTimer;

    private Vector2 extraPosition;

    private Vector2 extraPositionMotion;

    private Vector2 extraPositionAcceleration;

    private bool floatup;

    private int flapAnimationLength = 4;

    private int currentSidewaysFlap;

    private bool hasLight = true;

    private string lightId;

    private NetInt whichSubVariant = new NetInt(-1);

    private NetInt startingYForVariant = new NetInt(0);

    private bool perching;

    private float timeSinceLastZeroLerp;

    private float parrot_squawkTimer;

    private float parrot_squatTimer;

    private readonly NetVector2 netPosition = new NetVector2();
    private readonly NetBool netIsCasting = new NetBool(false);
    private readonly NetInt netCastEffectFrame = new NetInt(0);
    private readonly NetBool netYellowC = new NetBool(false);
    private readonly NetBool netYellow1 = new NetBool(false);
    private readonly NetBool netYellow2 = new NetBool(false);
    private readonly NetBool netYellow3 = new NetBool(false);
    private readonly NetBool _netMultiAttackMode = new NetBool(false);
    private readonly NetFloat _netMultiAttackTimer = new NetFloat(0f);
    private readonly NetFloat _netCooldownTimer = new NetFloat(0f);
    private readonly NetBool _netShowSkillEffect = new NetBool(false);

    private float _fireballTimer = 1600f;
    private float _fireballDelay = 1600f;
    private float _specialAttackTimer = 6500f;
    private const float SpecialAttackInterval = 5000f;
    private const int SpecialAttackMultiplier = 3;
    private int BaseDamage = 100;
    private int Damage=100;

    private Vector2 _fireballOffset = new Vector2(-50f, -148f);//如果要改这个，记得在WaterEyjaHitEffect里也改了

    private float _hoverTimer=300f;
    private readonly float _hoverSpeed = 5f;
    private readonly float _hoverAmplitude = 15f;
    private Vector2 _customOffset;

    private float _castEffectTimer;
    private int _castEffectFrame;
    private bool _isCasting;
    private const float FrameDuration = 60f;

    private float rotationAngle;
    private float WFBRotationAngle;
    private float FZrotationAngle;
    private float elapsedMilliseconds;
    private float elapsedMilliseconds1;
    private float elapsedMilliseconds2;
    private float elapsedMilliseconds3;

    private bool YellowC;
    private bool Yellow1;
    private bool Yellow2;
    private bool Yellow3;

    public bool _multiAttackMode = false;
    private float _multiAttackTimer = 0f;
    private float _cooldownTimer = 50000f;
    private bool _wasCtrl1Pressed = false;          // 防止重复触发
    private bool _showSkillEffect = false;          // 显示技能特效
    private float _skillEffectTimer = 0f;           // 技能特效计时器

    private Vector2 lsPosition;
    private Vector2 VolcanoZiDanPosition;
    private float _dailyParticleTimer;

    private readonly NetBool _netManualMode = new NetBool(false); // 手动模式状态
    private bool _wasCtrlLPressed; // 防止Ctrl+L重复触发

    // 粒子相关
    private List<ParticleConfig> _particleConfigs = new List<ParticleConfig>();
    private float _eightFigureMasterTimer;
    private const float EightFigureDuration = 3000f;//
    private Dictionary<ParticleType, Texture2D> _particleTextures;
    private float _totalRunningTime = 0f;
    private float HuanLiuAlpha = 1f;
    private bool VFenXing=false;
    private bool VHuangXing = false;
    private bool VDone = true;
    private float tishi = 500f;
    private bool VYinXiao;

    //
    private bool isTiQuCd=false;
    private WaterEyjyHitEffect particleManager;

    // 初始化粒子
    private void InitializeParticleSystem()
    {
        Random rand = new Random();

        // 粒子纹理
        _particleTextures = new Dictionary<ParticleType, Texture2D>
        {
            { ParticleType.Petal, Game1.content.Load<Texture2D>("Mods/EyjaPinkPetals") },
            { ParticleType.Water, Game1.content.Load<Texture2D>("Mods/EyjaBlueBlock") },
            { ParticleType.WhiteWater, Game1.content.Load<Texture2D>("Mods/EyjaWhiteBlock") },
            { ParticleType.OtherWater1, Game1.content.Load<Texture2D>("Mods/EyjaBlueShine") },
            { ParticleType.OtherWater2, Game1.content.Load<Texture2D>("Mods/EyjaThinBlueShine") },
            { ParticleType.PinkStar, Game1.content.Load<Texture2D>("Mods/EyjaPinkStar") },
            { ParticleType.YellowStar, Game1.content.Load<Texture2D>("Mods/EyjaYellowStar") },
            { ParticleType.OtherWater3, Game1.content.Load<Texture2D>("Mods/EyjaWFlower") },
        };

        // 清空现有配置
        _particleConfigs.Clear();

        // 添加花瓣粒子 (10个)
        for (int i = 0; i < 10; i++)
        {
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.Petal,
                PhaseOffset = MathHelper.TwoPi * i / 10,
                Size = 50f + (float)rand.NextDouble() * 60f,
                RotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.005f,//旋转
                Scale = 0.8f + (float)rand.NextDouble() * 0.6f,
                Alpha = 0.7f + (float)rand.NextDouble() * 0.3f,
                TextureVariant = rand.Next(0, 11),
                SizeVariation = 0.2f, // 花瓣大小变化较小
                SpeedFactor = 1.0f,   // 正常速度
                CenterOffice= new Vector2(0, 0),
                JiaoZhengOffice= new Vector2(0, 0),
                Rectangle=16,
                isReverse= rand.Next(2)
            });
        }

        // 添加水流粒子
        for (int i = 0; i < 300; i++)
        {
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.Water,//图标
                PhaseOffset = MathHelper.TwoPi * i / 300,//偏移
                Size = 60f + (float)rand.NextDouble() * 10f, // 略小
                RotationSpeed = 0f,//旋转
                Scale = 0.8f + (float)rand.NextDouble() * 0.5f,//大小
                Alpha = 0.5f + (float)rand.NextDouble() * 0.2f,//透明度
                TextureVariant = rand.Next(0, 3),//种类
                SizeVariation = 0.4f, // 水流大小变化更大
                SpeedFactor = 1.2f,    // 速度
                CenterOffice = new Vector2((float)(rand.NextDouble()) * 16f, (float)(rand.NextDouble() ) * 16f),
                JiaoZhengOffice = new Vector2(0,-15),
                Rectangle = 32,
                isReverse = rand.Next(2)
            });
        }
        // 添加水流粒子
        for (int i = 0; i < 100; i++)
        {
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.WhiteWater,//图标
                PhaseOffset = MathHelper.TwoPi * i / 100,//偏移
                Size = 60f + (float)rand.NextDouble() * 10f, // 略小
                RotationSpeed = 0f,//旋转
                Scale = 0.8f + (float)rand.NextDouble() * 0.5f,//大小
                Alpha = 0.5f + (float)rand.NextDouble() * 0.2f,//透明度
                TextureVariant = rand.Next(0, 3),//种类
                SizeVariation = 0.4f, // 水流大小变化更大
                SpeedFactor = 1.2f,    // 速度
                CenterOffice = new Vector2((float)(rand.NextDouble()) * 16f, (float)(rand.NextDouble()) * 16f),
                JiaoZhengOffice = new Vector2(0, -15),
                Rectangle = 32,
                isReverse = rand.Next(2)
            });
        }
        for (int i = 0; i < 1; i++)
        {
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.OtherWater1,//图标
                PhaseOffset = MathHelper.TwoPi * i / 5,//偏移
                Size = 70f + (float)rand.NextDouble() *10f, // 略小
                RotationSpeed = 0f,//旋转
                Scale = 0.3f + (float)rand.NextDouble() * 0.3f,//大小
                Alpha = 0.1f + (float)rand.NextDouble() * 0.1f,//透明度
                TextureVariant = rand.Next(0, 3),//种类
                SizeVariation = 0.4f, // 水流大小变化更大
                SpeedFactor = 1.2f,    // 速度
                CenterOffice = new Vector2((float)(rand.NextDouble()) * 16f, (float)(rand.NextDouble()) * 16f),
                JiaoZhengOffice = new Vector2(0, -15),
                Rectangle = 64,
                isReverse = rand.Next(2)
            });
        }
        //水流
        for (int i = 0; i <500; i++)
        {
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.OtherWater2,//图标
                PhaseOffset = MathHelper.TwoPi * i / 500,//偏移
                Size = 80f + (float)rand.NextDouble() * 5f, // 略小
                RotationSpeed = 0f,//旋转
                Scale = 0.2f + (float)rand.NextDouble() * 0.2f,//大小
                Alpha = 0.2f + (float)rand.NextDouble() * 0.1f,//透明度
                TextureVariant = 0,//种类
                SizeVariation = 0.4f, // 水流大小变化更大
                SpeedFactor = 2f,    // 速度
                CenterOffice = new Vector2((float)(rand.NextDouble()) * 16f, (float)(rand.NextDouble()) * 16f),
                JiaoZhengOffice = new Vector2(0, -15),
                Rectangle = 64,
                isReverse = rand.Next(2)
            });
        }
        // 星星
        for (int i = 0; i < 12; i++)
        {
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.PinkStar,//图标
                PhaseOffset = MathHelper.TwoPi * i / 5,//偏移
                Size = 80f + (float)rand.NextDouble() * 20f, // 略小
                RotationSpeed = (float)(rand.NextDouble()+0.1) * 0.01f,//旋转
                Scale = 0.2f + (float)rand.NextDouble() * 0.4f,//大小
                Alpha = 1,//透明度
                TextureVariant = 0,//种类
                SizeVariation = 0.4f, // 水流大小变化更大
                SpeedFactor = 1.3f,    // 速度
                CenterOffice = new Vector2((float)(rand.NextDouble()) * 16f, (float)(rand.NextDouble()) * 16f),
                JiaoZhengOffice = new Vector2(-10, -15),
                Rectangle = 64,
                isReverse = rand.Next(2)
            });
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.YellowStar,//图标
                PhaseOffset = MathHelper.TwoPi * i / 5,//偏移
                Size = 80f + (float)rand.NextDouble() * 20f, // 略小
                RotationSpeed = (float)(rand.NextDouble()+0.1) * 0.01f,//旋转
                Scale = 0.2f + (float)rand.NextDouble() * 0.4f,//大小
                Alpha =1,//透明度
                TextureVariant = 0,//种类
                SizeVariation = 0.4f, // 水流大小变化更大
                SpeedFactor = 1.3f,    // 速度
                CenterOffice = new Vector2((float)(rand.NextDouble()) * 16f, (float)(rand.NextDouble()) * 16f),
                JiaoZhengOffice = new Vector2(-15, -5),
                Rectangle = 64,
                isReverse = rand.Next(2),
                MaxAlpha=1.0f,
                MinAlpha=0.1f,
                AlphaCurve=1.0f
            });
        }
        for (int i = 0; i < 1; i++)
        {
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.PinkStar,//图标
                PhaseOffset = MathHelper.TwoPi * 1,//偏移
                Size = 40f + (float)rand.NextDouble() * 20f, // 略小
                RotationSpeed = (float)(rand.NextDouble() * 0.0025f+0.002f),//旋转
                Scale = 0.3f + (float)rand.NextDouble() * 0.4f,//大小
                Alpha = 1,//透明度
                TextureVariant = 0,//种类
                SizeVariation = 0.4f, // 水流大小变化更大
                SpeedFactor = 0.7f,    // 速度
                CenterOffice = new Vector2((float)(rand.NextDouble()) * 16f, (float)(rand.NextDouble()) * 16f),
                JiaoZhengOffice = new Vector2(-10, -7),
                Rectangle = 48,
                isReverse = rand.Next(2),
                VStar=true,
                StartPositionT = (float)rand.NextDouble(), // 随机起始位置 (0~1)
                VStarShunXu = 0
            });
            _particleConfigs.Add(new ParticleConfig
            {
                Type = ParticleType.YellowStar,//图标
                PhaseOffset = MathHelper.TwoPi * 1,//偏移
                Size = 40f + (float)rand.NextDouble() * 20f, // 略小
                RotationSpeed = (float)(rand.NextDouble() * 0.0025f+ 0.002f) ,//旋转
                Scale = 0.3f + (float)rand.NextDouble() * 0.4f,//大小
                Alpha = 1,//透明度
                TextureVariant = 0,//种类
                SizeVariation = 0.4f, // 水流大小变化更大
                SpeedFactor = 1f,    // 速度
                CenterOffice = new Vector2((float)(rand.NextDouble()) * 16f, (float)(rand.NextDouble()) * 16f),
                JiaoZhengOffice = new Vector2(-10, -7),
                Rectangle = 48,
                isReverse = rand.Next(2),
                VStar = true,
                StartPositionT = (float)rand.NextDouble(), // 随机起始位置 (0~1)
                VStarShunXu = 1
            });
            
        }
       
        }

    public WaterEyjaCompanion()
    {
        lightId = $"{"FlyingCompanion1"}_{Game1.random.Next()}";
        netPosition.Interpolated(false, false); // 禁用插值，立即更新（没记错是仙女的代码

    }

    public WaterEyjaCompanion(int whichVariant, int whichSubVariant = -1)
        : this()
    {
        base.whichVariant.Value = whichVariant;
        this.whichSubVariant.Value = whichSubVariant;
        if (whichVariant == 1)
        {
            startingYForVariant.Value = 160;
            hasLight = false;
        }
    }

    public override void InitNetFields()
    {
        base.InitNetFields(); // 基类初始化
        NetFields
            .AddField(netPosition, "netPosition") // 位置同步字段
            .AddField(whichSubVariant, "whichSubVariant")
            .AddField(startingYForVariant, "startingYForVariant")
            .AddField(netIsCasting, "netIsCasting")
            .AddField(netCastEffectFrame, "netCastEffectFrame")
            .AddField(netYellowC, "netYellowC")
            .AddField(netYellow1, "netYellow1")
            .AddField(netYellow2, "netYellow2")
            .AddField(netYellow3, "netYellow3")
            .AddField(_netMultiAttackMode, "_netMultiAttackMode")
            .AddField(_netMultiAttackTimer, "_netMultiAttackTimer")
            .AddField(_netCooldownTimer, "_netCooldownTimer")
            .AddField(_netShowSkillEffect, "_netShowSkillEffect")
            .AddField(_netManualMode, "_netManualMode");
    }
    public static List<Monster> findMonstersWithinRange(
    GameLocation location,
    Vector2 center,
    float radius,
    int maxCount)
    {
        return location.characters
            .OfType<Monster>()
            .Where(m => Vector2.Distance(center, m.Position) <= radius)
            .OrderBy(m => Vector2.Distance(center, m.Position))
            .Take(maxCount)
            .ToList();
    }

    private void SaveCooldownToModData()
    {
        if (Owner != null)
        {
            Owner.modData["EyjaTrinket.CooldownTimer"] = _cooldownTimer.ToString();
            Owner.modData["EyjaTrinket.MultiAttackMode"] = _multiAttackMode.ToString();
            Owner.modData["EyjaTrinket.MultiAttackTimer"] = _multiAttackTimer.ToString();
        }
    }
    public override void Update(GameTime time, GameLocation location)
    {
        base.Update(time, location);
        // 确保粒子管理器有正确的location
        if (particleManager == null || particleManager.location != location)
        {
            particleManager = new WaterEyjyHitEffect(location);
        }
        Random rand = new Random();
        base.hopEvent.Poll(); // 
        //Damage = Game1.random.Next(BaseDamage - (int)(BaseDamage * 0.2), BaseDamage + (int)(BaseDamage * 0.2) + 1);
        // 悬浮
        _hoverTimer += (float)time.ElapsedGameTime.TotalSeconds;
        float verticalOffset = (float)Math.Sin(_hoverTimer * _hoverSpeed) * _hoverAmplitude;
        _customOffset = new Vector2(0, verticalOffset);
        _dailyParticleTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
        if (_dailyParticleTimer >= 1500f)
        {
            //Game1.addHUDMessage(new HUDMessage("日常特效"));
            particleManager.AddCasualParticles(location, Position);
            if (this.Owner?.IsLocalPlayer ?? false)
            {
                ModEntry.StaticHelper.Multiplayer.SendMessage(
                    message: new ModEntry.WaterHitEffectMessage
                    {
                        EffectType = "AddCasualParticles",
                        X = Position.X,
                        Y = Position.Y,
                        LocationName = location.Name
                    },
                    messageType: "EyjaTrinket/PlayWaterHitEffect"
                );
            }
            _dailyParticleTimer = 0f;
        }
        if (Owner != null && Owner.IsLocalPlayer)
        {
            // 特效状态
            netIsCasting.Value = _isCasting;
            netCastEffectFrame.Value = _castEffectFrame;
            netPosition.Value = Position;
            netYellowC.Value = YellowC;
            netYellow1.Value = Yellow1;
            netYellow2.Value = Yellow2;
            netYellow3.Value = Yellow3;

            // 同步技能状态
            _netMultiAttackMode.Value = _multiAttackMode;
            _netMultiAttackTimer.Value = _multiAttackTimer;
            _netCooldownTimer.Value = _cooldownTimer;
            _netShowSkillEffect.Value = _showSkillEffect;

            //&&IsLocal
            if (Game1.shouldTimePass())
            {
                elapsedMilliseconds += (float)time.ElapsedGameTime.TotalMilliseconds;
                elapsedMilliseconds1 += (float)time.ElapsedGameTime.TotalMilliseconds;

                // 每10ms转一次（黄圈）
                if (elapsedMilliseconds >= 10)
                {
                    rotationAngle -= 0.03f;
                    elapsedMilliseconds -= 10;
                    rotationAngle %= MathHelper.TwoPi;
                }
                if (elapsedMilliseconds1 >= 2)
                {
                    WFBRotationAngle -= 0.12f;
                    elapsedMilliseconds1 -= 2;
                    rotationAngle %= MathHelper.TwoPi;
                }

                // 点燃存储次数
                if (_specialAttackTimer >= 5000f && _specialAttackTimer < 10000f)
                {
                    YellowC = true;
                    Yellow1 = true;
                    Yellow2 = false;
                    Yellow3 = false;
                }
                else if (_specialAttackTimer >= 10000f && _specialAttackTimer < 15000f)
                {
                    Yellow1 = false;
                    Yellow2 = true;
                    Yellow3 = false;
                }
                else if (_specialAttackTimer == 15000f)
                {
                    Yellow1 = false;
                    Yellow2 = false;
                    Yellow3 = true;
                }
                else
                {
                    YellowC = Yellow1 = Yellow2 = Yellow3 = false;
                }

                // 技能计时器
                float elapsedMs = (float)time.ElapsedGameTime.TotalMilliseconds;

                // 计时器（火山
                if (_multiAttackMode)//如果开火山
                {
                    _multiAttackTimer -= elapsedMs;//_multiAttackTimer火山持续时间
                    if (_multiAttackTimer <= 0f)
                    {
                        _multiAttackMode = false;
                        _multiAttackTimer = 0f;
                        _cooldownTimer = 60000f;//冷却
                    }
                }

                // 冷却计时器(火山cd)
                if (_cooldownTimer > 0f)
                {
                    _cooldownTimer -= elapsedMs;//这个是火山cd
                    if (_cooldownTimer < 0f) _cooldownTimer = 0f;
                }
                //Game1.addHUDMessage(new HUDMessage("Open"+ _cooldownTimer));
                // 更新技能特效计时器
                if (_showSkillEffect)
                {
                    _skillEffectTimer += elapsedMs;
                    if (_skillEffectTimer >= 2000f) // 特效显示2秒
                    {
                        _showSkillEffect = false;
                        _skillEffectTimer = 0f;
                    }
                }
                bool isCtrlPressed111111 = Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftControl) && Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftShift) && Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftAlt) && Game1.input.GetKeyboardState().IsKeyDown(Keys.V);
                if (isCtrlPressed111111)
                {
                    _cooldownTimer = 0f;
                }
                // 检测技能按键 (Ctrl+L)
                bool isCtrlPressed = Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftControl) ||
                                     Game1.input.GetKeyboardState().IsKeyDown(Keys.RightControl);
                bool is1Pressed = Game1.input.GetKeyboardState().IsKeyDown(Keys.L);
                tishi += (float)time.ElapsedGameTime.TotalMilliseconds;
                if (is1Pressed && isCtrlPressed && !_multiAttackMode && _cooldownTimer != 0)
                {

                    if (tishi > 300) { Game1.addHUDMessage(new HUDMessage(I18n.EyjaVolcanoCD() + ((int)_cooldownTimer / 1000 + 1), 2)); tishi = 0; }
                    if (Game1.hudMessages.Count >= 2)
                    {
                        Game1.hudMessages.RemoveAt(0); // 移除最旧的消息
                    }


                }
                if (_cooldownTimer == 0 && VDone && !_multiAttackMode)
                {
                    Game1.addHUDMessage(new HUDMessage(I18n.EyjaVolcanoDone(), 1));
                    VDone = false;
                }
                if (!isTiQuCd)
                {
                    string? cdValue = Owner.modData.ContainsKey("EyjaTrinket._cooldownTimer")
                        ? Owner.modData["EyjaTrinket._cooldownTimer"]
                        : null;
                    if (cdValue != null && float.TryParse(cdValue, out float _cooldownTimer1))
                    {
                        _cooldownTimer = _cooldownTimer1;
                    }
                    isTiQuCd = true;
                }
                Owner.modData["EyjaTrinket._cooldownTimer"] = _cooldownTimer.ToString();
                if (isCtrlPressed && is1Pressed)
                {
                    if (!_wasCtrl1Pressed && _cooldownTimer <= 0f && !_multiAttackMode)
                    {
                        // 开技能
                        _multiAttackMode = true;
                        _multiAttackTimer = 7000f;
                        _showSkillEffect = true;
                        _skillEffectTimer = 0f;
                        VDone = true;
                        _cooldownTimer = 60000f;
                        //VYinXiao = true;
                        // 播放音效
                        //Game1.addHUDMessage(new HUDMessage("开火山"));
                        location.playSound("EyjaHuoShanQiDong");
                    }
                    _wasCtrl1Pressed = true;
                }
                else
                {
                    _wasCtrl1Pressed = false;

                }
                bool isCtrlPressed1 = Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftControl) ||
                                    Game1.input.GetKeyboardState().IsKeyDown(Keys.RightControl);
                bool isLPressed1 = Game1.input.GetKeyboardState().IsKeyDown(Keys.K);
                if (isCtrlPressed1 && isLPressed1)
                {
                    if (!_wasCtrlLPressed)
                    {
                        // 切换手动模式
                        _netManualMode.Value = !_netManualMode.Value;

                        // 提示信息
                        if (_netManualMode.Value)
                            Game1.addHUDMessage(new HUDMessage(I18n.EyjaShouDongOpen(), 2));
                        else
                            Game1.addHUDMessage(new HUDMessage(I18n.EyjaShouDongClose(), 2));
                        if (Game1.hudMessages.Count >= 2)
                        {
                            Game1.hudMessages.RemoveAt(0); // 移除最旧的消息
                        }
                    }
                    _wasCtrlLPressed = true;
                }
                else
                {
                    _wasCtrlLPressed = false;
                }
                //if (Game1.IsMasterGame)
                {
                    List<Monster> targets;

                    // 根据模式选择目标
                    if (_multiAttackMode)
                    {
                        //火山
                        targets = findMonstersWithinRange(location, Owner.Position, 850, 6);

                        Damage = (int)(BaseDamage * 2.3);

                        _fireballDelay = 400f;
                    }
                    else if (_netManualMode.Value)
                    {
                        Monster singleTarget = Utility.findClosestMonsterWithinRange(location, Owner.Position, 850);
                        targets = singleTarget != null ? new List<Monster> { singleTarget } : new List<Monster>();
                        Damage = BaseDamage;
                        _fireballDelay = 800f;
                    }
                    else
                    {
                        // 普通
                        Monster singleTarget = Utility.findClosestMonsterWithinRange(location, Owner.Position, 850);
                        targets = singleTarget != null ? new List<Monster> { singleTarget } : new List<Monster>();
                        Damage = BaseDamage;
                        _fireballDelay = 1600f;
                    }

                    if (!_netManualMode.Value)//自动（有无火山）
                    {
                        if (targets == null || targets.Count == 0)
                        {
                            // 无目标时累积特殊攻击计时器
                            _specialAttackTimer += elapsedMs;
                            if (_specialAttackTimer > 15000f)
                            {
                                _specialAttackTimer = 15000f;
                            }
                        }
                        else
                        {
                            _fireballTimer += elapsedMs;
                            _specialAttackTimer += elapsedMs;
                            if (_specialAttackTimer >= 15000f)
                            {
                                _specialAttackTimer = 15000f;
                            }

                            // 特殊攻击条件
                            if (_specialAttackTimer >= SpecialAttackInterval && _fireballTimer >= _fireballDelay && !_multiAttackMode && Game1.player.currentLocation.Name != "Slime Hutch")
                            {
                                // 对每个目标发射特殊火球
                                foreach (Monster target in targets)
                                {
                                    //自动点燃
                                    SpecialShootFireball(location, Owner, target);
                                }
                                _fireballTimer = 0f;
                                _specialAttackTimer -= 5000f;
                            }
                            // 普通攻击条件
                            else if (_fireballTimer >= _fireballDelay)
                            {
                                // 对每个目标发射普通火球
                                if (_multiAttackMode)
                                {
                                    foreach (Monster target in targets)
                                    {
                                        //Game1.addHUDMessage(new HUDMessage("自动攻击=" + _fireballDelay));
                                        //火山
                                        VShootFireball(location, Owner, target);
                                    }
                                    //VolcanoZiDanPosition = new(-25, 25);
                                }
                                else
                                {
                                    foreach (Monster target in targets)
                                    {
                                        //Game1.addHUDMessage(new HUDMessage("自动攻击=" + _fireballDelay));
                                        //自动普工
                                        ShootFireball(location, Owner, target);
                                    }
                                    VolcanoZiDanPosition = new(0, 0);
                                }


                                _fireballTimer = 0f;
                            }
                        }
                    }
                    else if (_netManualMode.Value)//手动（）
                    {
                        // 检测鼠标左键按下
                        bool isLeftClickPressed = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
                        _fireballTimer += elapsedMs;
                        _specialAttackTimer += elapsedMs;
                        if (_specialAttackTimer >= 15000f)
                        {
                            _specialAttackTimer = 15000f;
                        }
                        if (_fireballTimer >= 1600f)
                        {
                            _fireballTimer = 1600f;
                        }
                        //Game1.addHUDMessage(new HUDMessage("isLeftClickPressed="+ isLeftClickPressed));
                        if (_multiAttackMode && _fireballTimer >= _fireballDelay)//(有火山)
                        {

                            //VolcanoZiDanPosition = new(-25, 25);

                            foreach (Monster target in targets)
                            {
                                //Game1.addHUDMessage(new HUDMessage("当前速度=" + _fireballDelay));
                                VShootFireball(location, Owner, target);
                            }
                            _fireballTimer = 0f;


                        }
                        else if (isLeftClickPressed && !_multiAttackMode)
                        {
                            // 获取鼠标位置（转换为游戏世界坐标）
                            Vector2 mouseScreenPos = new Vector2(Game1.getMouseX(), Game1.getMouseY());
                            Vector2 mouseWorldPos = new Vector2(
                                mouseScreenPos.X + Game1.viewport.X,
                                mouseScreenPos.Y + Game1.viewport.Y
                            );


                            // 特殊攻击条件
                            if (_specialAttackTimer >= SpecialAttackInterval && _fireballTimer >= _fireballDelay && !_multiAttackMode)
                            {
                                //带s的是手动
                                SSpecialShootFireball(location, Owner, mouseWorldPos);

                                _fireballTimer = 0f;
                                _specialAttackTimer -= 5000f;
                            }
                            else if (_fireballTimer >= _fireballDelay)
                            {
                                VolcanoZiDanPosition = new(0, 0);
                                SShootFireball(location, Owner, mouseWorldPos);
                                _fireballTimer = 0f;
                            }
                        }


                    }
                }
            }
        }
        else
        {
            _isCasting = netIsCasting.Value;
            _castEffectFrame = netCastEffectFrame.Value;
            YellowC = netYellowC.Value;
            Yellow1 = netYellow1.Value;
            Yellow2 = netYellow2.Value;
            Yellow3 = netYellow3.Value;
            _multiAttackMode = _netMultiAttackMode.Value;
            _multiAttackTimer = _netMultiAttackTimer.Value;
            _cooldownTimer = _netCooldownTimer.Value;
            _showSkillEffect = _netShowSkillEffect.Value;
        }

        // 施法特效更新
        if (_isCasting && !_multiAttackMode)
        {
            _castEffectTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            if (_castEffectTimer >= FrameDuration)
            {
                _castEffectTimer = 0f;
                _castEffectFrame++;
                if (_castEffectFrame >= 4)
                {
                    _isCasting = false;
                    _castEffectFrame = 0;
                }
            }
        }
        if (_multiAttackMode)
        {
            WaterEyjaTrinket.Huoshanyisu = true;
            //火山粒子
            elapsedMilliseconds2 += (float)time.ElapsedGameTime.TotalMilliseconds;
            if (elapsedMilliseconds2 >= 5)
            {
                Vector2 ls = new Vector2(65, 160);
                lsPosition = base.Position + _customOffset + ls;
                particleManager.AddVolcanoParticles(location, lsPosition);
                if (this.Owner?.IsLocalPlayer ?? false)
                {
                    ModEntry.StaticHelper.Multiplayer.SendMessage(
                        message: new ModEntry.WaterHitEffectMessage
                        {
                            EffectType = "AddVolcanoParticles",
                            X = lsPosition.X,
                            Y = lsPosition.Y,
                            LocationName = location.Name
                        },
                        messageType: "EyjaTrinket/PlayWaterHitEffect"
                    );
                }
                elapsedMilliseconds2 -= 5;
            }
            if (_multiAttackTimer <= 1000)
            {
                elapsedMilliseconds3 += (float)time.ElapsedGameTime.TotalMilliseconds;
                HuanLiuAlpha -= elapsedMilliseconds3 / 10000f; // 每秒减少 1.0

                if (HuanLiuAlpha < 0.0f)
                {
                    HuanLiuAlpha = 0.0f;
                    elapsedMilliseconds3 = 0f;
                }


            }
            if (_multiAttackTimer > 1000)
            {
                HuanLiuAlpha = 1f;
            }
        }
        else
        {
            WaterEyjaTrinket.Huoshanyisu = false;
        }
        if (_cooldownTimer <= 30000f) VFenXing = true; else VFenXing = false;
        if (_cooldownTimer == 0) VHuangXing = true; else VHuangXing = false;
       // Game1.addHUDMessage(new HUDMessage("isLeftClickPressed=" + VHuangXing));
    }

    public override void Draw(SpriteBatch b)
    {
        if (base.Owner?.currentLocation == null ||
            (base.Owner.currentLocation.DisplayName == "Temp" && !Game1.isFestival()))
        {
            return;
        }

        Texture2D texture = Game1.content.Load<Texture2D>("Mods/EyjafjallaWaterSplash1");
        Texture2D _YellowC = Game1.content.Load<Texture2D>("Mods/YellowCircle");
        Texture2D _Yellow1 = Game1.content.Load<Texture2D>("Mods/Yellow1");
        Texture2D _Yellow2 = Game1.content.Load<Texture2D>("Mods/Yellow2");
        Texture2D _Yellow3 = Game1.content.Load<Texture2D>("Mods/Yellow3");

        Vector2 YellowOffset = new Vector2(-10f, -45f);
        Vector2 ls = new Vector2(0, -100);
        Vector2 finalPosition = base.Position + _customOffset + ls;
        SpriteEffects effect = SpriteEffects.None;

        // 绘制法杖本体
        b.Draw(
            texture,
            Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
            new Rectangle(0, 0, 16, 16),
            Color.White,
            FZrotationAngle,
            new Vector2(8f, 8f),
            4f,
            effect,
            Position.Y / 10000f
        );

        //  绘制黄圈（就那个点燃层数）
        if (YellowC)
        {
            b.Draw(
                _YellowC,
                Game1.GlobalToLocal(finalPosition + YellowOffset + base.Owner.drawOffset + new Vector2(0f, -height * 4f - height)),
                new Rectangle(0, 0, 31, 31),
                Color.White,
                rotationAngle,
                new Vector2(15f, 15f),
                1.1f,
                effect,
                Position.Y / 10000f
            );
        }
        if (Yellow1)
        {
            //这是1
            b.Draw(
           _Yellow1,
           Game1.GlobalToLocal(finalPosition + YellowOffset + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
           new Rectangle(0, 0, 16, 16),
           Color.White,
           0f,
           new Vector2(8f, 8f),
           2f,
           effect,
           _position.Y / 10000f
       );
        }
        //这是2
        if (Yellow2)
        {
            b.Draw(
            _Yellow2,
            Game1.GlobalToLocal(finalPosition + YellowOffset + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
            new Rectangle(0, 0, 16, 16),
            Color.White,
            0f,
            new Vector2(8f, 8f),
            2f,
            effect,
            _position.Y / 10000f
               );
        }
        if (Yellow3)
        {
            //这是3
            b.Draw(
           _Yellow3,
           Game1.GlobalToLocal(finalPosition + YellowOffset + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
           new Rectangle(0, 0, 16, 16),
           Color.White,
           0f,
           new Vector2(8f, 8f),
           2f,
           effect,
           _position.Y / 10000f
       );
        }



        // 绘制阴影
        b.Draw(
            Game1.shadowTexture,
            Game1.GlobalToLocal(Position + base.Owner.drawOffset),
            Game1.shadowTexture.Bounds,
            Color.White,
            0f,
            new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
            3f * Utility.Lerp(1f, 0.8f, Math.Min(height, 1f)),
            SpriteEffects.None,
            (Position.Y - 8f) / 10000f - 2E-06f
        );

        // 绘制施法特效
        if (_isCasting && !_multiAttackMode)
        {
            Texture2D castTexture = Game1.content.Load<Texture2D>("Mods/EyjaWaterCast");
            Rectangle sourceRect = new Rectangle(_castEffectFrame * 16, 0, 16, 16);
            Vector2 effectPosition = new Vector2(20f, -30f);

            b.Draw(
                castTexture,
                Game1.GlobalToLocal(finalPosition + effectPosition + base.Owner.drawOffset),
                sourceRect,
                Color.White,
                0f,
                new Vector2(8f, 8f),
                3.5f,
                SpriteEffects.None,
                (Position.Y + 32f) / 10000f
            );
        }

        if (_multiAttackMode)
        {
            DrawEightFigureEffect(b);
            Texture2D castTexture = Game1.content.Load<Texture2D>("Mods/EyjaWFlower");
            Texture2D castTexture1 = Game1.content.Load<Texture2D>("Mods/EyjaBigWaterBall");
            Rectangle sourceRect = new Rectangle(0, 0, 48, 48);
            Vector2 effectPosition = new Vector2(30f, -40f);

            b.Draw(
                castTexture,
                Game1.GlobalToLocal(finalPosition + effectPosition + base.Owner.drawOffset),
                sourceRect,
                Color.White * 0.75f * HuanLiuAlpha,
                WFBRotationAngle ,
                new Vector2(24f, 24f),
                2f,
                SpriteEffects.None,
                (Position.Y + 33f) / 10000f
            );
            //b.Draw(
            //    castTexture1,
            //    Game1.GlobalToLocal(finalPosition + effectPosition + base.Owner.drawOffset),
            //    sourceRect,
            //    Color.White * 0.45f,
            //    WFBRotationAngle,
            //    new Vector2(24f, 24f),
            //    1.5f,
            //    SpriteEffects.None,
            //    (Position.Y + 32f) / 10000f
            //);
        }
        else
        {
            _totalRunningTime += (float)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;

            
            foreach (var config in _particleConfigs)
            {
                if (config.VStar == true && VFenXing == true)
                {
                    if(config.VStarShunXu==0)
                     {
                        float actualPeriod = EightFigureDuration / config.SpeedFactor;
                        float t = ((_totalRunningTime / actualPeriod) + config.StartPositionT) % 1.0f;
                        if (t < 0) t += 1.0f;
                        float angle = t * MathHelper.TwoPi + config.PhaseOffset;
                        float originalX = config.Size * (float)Math.Sin(angle);
                        float originalY = config.Size * (float)Math.Sin(2 * angle) / 2;
                        float TiltAngle = -MathHelper.PiOver4;
                        float CosTilt = (float)Math.Cos(TiltAngle);
                        float SinTilt = (float)Math.Sin(TiltAngle);
                        float rotatedX = originalX * CosTilt - originalY * SinTilt;
                        float rotatedY = originalX * SinTilt + originalY * CosTilt;
                        Vector2 finalPosition1 = this.Position + _customOffset + ls;
                        Vector2 drawOffset = base.Owner?.drawOffset ?? Vector2.Zero;
                        float heightOffset = (0f - height) * 4f + (0f - height);
                        Vector2 center = finalPosition1 + drawOffset + new Vector2(0f, heightOffset);
                        Vector2 effectPosition1 = center + new Vector2(rotatedX, rotatedY) + config.CenterOffice + config.JiaoZhengOffice;
                        float rawVerticalPosition = originalX;
                        float normalizedX = -(rawVerticalPosition / (config.Size * 0.5f));
                        float alphaFactor = CalculateAlphaFromVerticalPosition(config, normalizedX);
                        float finalAlpha = config.Alpha * alphaFactor;
                        float rotation = _totalRunningTime * config.RotationSpeed;
                        Vector2 screenPosition = Game1.GlobalToLocal(effectPosition1);
                        Texture2D Startexture = _particleTextures[config.Type];//粒子图标
                        b.Draw(
                            Startexture,
                            screenPosition,
                            new Rectangle(config.TextureVariant * 16, 0, config.Rectangle, config.Rectangle),
                            Color.White,
                            rotation,
                            new Vector2(config.Rectangle / 2, config.Rectangle / 2),
                            config.Scale,
                            SpriteEffects.None,
                            (Position.Y + 32f) / 10000f
                        );
                    }
                }if (config.VStar == true && VHuangXing == true)
                {
                    if (config.VStarShunXu == 1)
                    {
                        float actualPeriod = EightFigureDuration / config.SpeedFactor;
                        float t = ((_totalRunningTime / actualPeriod) + config.StartPositionT) % 1.0f;
                        if (t < 0) t += 1.0f;
                        float angle = t * MathHelper.TwoPi + config.PhaseOffset;
                        float originalX = config.Size * (float)Math.Sin(angle);
                        float originalY = config.Size * (float)Math.Sin(2 * angle) / 2;
                        float TiltAngle = -MathHelper.PiOver4;
                        float CosTilt = (float)Math.Cos(TiltAngle);
                        float SinTilt = (float)Math.Sin(TiltAngle);
                        float rotatedX = originalX * CosTilt - originalY * SinTilt;
                        float rotatedY = originalX * SinTilt + originalY * CosTilt;
                        Vector2 finalPosition1 = this.Position + _customOffset + ls;
                        Vector2 drawOffset = base.Owner?.drawOffset ?? Vector2.Zero;
                        float heightOffset = (0f - height) * 4f + (0f - height);
                        Vector2 center = finalPosition1 + drawOffset + new Vector2(0f, heightOffset);
                        Vector2 effectPosition1 = center + new Vector2(rotatedX, rotatedY) + config.CenterOffice + config.JiaoZhengOffice;
                        float rawVerticalPosition = originalX;
                        float normalizedX = -(rawVerticalPosition / (config.Size * 0.5f));
                        float alphaFactor = CalculateAlphaFromVerticalPosition(config, normalizedX);
                        float finalAlpha = config.Alpha * alphaFactor;
                        float rotation = _totalRunningTime * config.RotationSpeed;
                        Vector2 screenPosition = Game1.GlobalToLocal(effectPosition1);
                        Texture2D Startexture = _particleTextures[config.Type];//粒子图标
                        b.Draw(
                            Startexture,
                            screenPosition,
                            new Rectangle(config.TextureVariant * 16, 0, config.Rectangle, config.Rectangle),
                            Color.White,
                            rotation,
                            new Vector2(config.Rectangle / 2, config.Rectangle / 2),
                            config.Scale,
                            SpriteEffects.None,
                            (Position.Y + 32f) / 10000f
                        );
                    }
                }
            }
        }
    }
   
    private void DrawEightFigureEffect(SpriteBatch b)
    {
        // 绘制特效
        _totalRunningTime += (float)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;


        foreach (var config in _particleConfigs)
        {
            // 计算粒子运动的周期
            // 实际周期 = 基础周期 / 速度因子（速度越快，周期越短）
            float actualPeriod = EightFigureDuration / config.SpeedFactor;

            // 计算归一化时间t（0~1，代表在当前周期中的进度）
            // 用总时间除以实际周期，再取模1，确保t始终在0~1平滑循环
            float t = (float)Math.IEEERemainder(_totalRunningTime / actualPeriod, 1.0);
            // 处理负数
            if (t < 0) t += 1.0f;

            // 基于t计算角度（此时t的循环完全匹配粒子运动周期）
            float angle = t * MathHelper.TwoPi + config.PhaseOffset;
            if (config.isReverse==1)
            {
                angle = -angle;
            }
            // 原始8字坐标（水平方向）
            float originalX = config.Size * (float)Math.Sin(angle);
            float originalY = config.Size * (float)Math.Sin(2 * angle) / 2;
            float TiltAngle = -MathHelper.PiOver4; // π/4 弧度 = 45度
                                                   // 预计算旋转所需的正弦和余弦值（提升性能）
            float CosTilt = (float)Math.Cos(TiltAngle);
            float SinTilt = (float)Math.Sin(TiltAngle);
            // 核心：将原始坐标旋转45度（关键步骤）
            float rotatedX = originalX * CosTilt - originalY * SinTilt;
            float rotatedY = originalX * SinTilt + originalY * CosTilt;

            // 确定8字轨迹的中心位置
            Vector2 ls = new Vector2(0, -100);
            Vector2 finalPosition1 = this.Position + _customOffset + ls;
            Vector2 drawOffset = base.Owner?.drawOffset ?? Vector2.Zero;
            float heightOffset = (0f - height) * 4f + (0f - height);
            Vector2 center = finalPosition1 + drawOffset + new Vector2(0f, heightOffset);

            // 计算特效最终位置（中心 + 旋转后的8字偏移）
            Vector2 effectPosition1 = center + new Vector2(rotatedX, rotatedY)+ config.CenterOffice+ config.JiaoZhengOffice;

            //根据垂直位置计算透明度
            // 获取原始Y坐标（未旋转的垂直位置）
            float rawVerticalPosition = originalX;

            //计算Y坐标在8字形中的相对位置（-1到1）
            float normalizedX = -(rawVerticalPosition / (config.Size * 0.5f)); // 因为原始Y范围是 [-size/2, size/2]

            // 映射到透明度曲线（底部=1，顶部=0）
            float alphaFactor = CalculateAlphaFromVerticalPosition(config,normalizedX);

            // 应用基础透明度
            float finalAlpha = config.Alpha * alphaFactor;

            float rotation = _totalRunningTime * config.RotationSpeed;

            Vector2 screenPosition = Game1.GlobalToLocal(effectPosition1);
            Texture2D texture = _particleTextures[config.Type];//粒子图标
            b.Draw(
                texture,
                screenPosition,
                new Rectangle(config.TextureVariant * 16, 0, config.Rectangle, config.Rectangle),
                Color.White * finalAlpha* HuanLiuAlpha,
                rotation,
                new Vector2(config.Rectangle/2, config.Rectangle / 2),
                config.Scale,
                SpriteEffects.None,
                (Position.Y + 32f) / 10000f
            );
        }
    }
    // 透明度计算
    private float CalculateAlphaFromVerticalPosition(ParticleConfig config, float normalizedX)
    {
        // 使用S形曲线（sigmoid）实现平滑过渡
        float t = (normalizedX + 2f) * 0.5f; // 归一化到[0,1]

        // 曲线函数
        float alpha = (float)(1f / (1f + Math.Exp(-12f * (t - 0.5f)))); // S形曲线

        // 曲线强度参数
        alpha = (float)Math.Pow(alpha, config.AlphaCurve);

        // 映射到配置的透明度范围
        alpha = config.MinAlpha + (config.MaxAlpha - config.MinAlpha) * alpha;

        return MathHelper.Clamp(alpha, 0f, 1f);
    }

    public override void InitializeCompanion(Farmer farmer)
    {
        base.InitializeCompanion(farmer);
        InitializeParticleSystem(); // 初始化粒子数据
        if (hasLight)
        {
            Game1.currentLightSources.Add(new LightSource(lightId, 1, base.Position, 2f, Color.Black, LightSource.LightContext.None, 0L));
        }
        if (whichSubVariant.Value == -1)
        {
            Random r = Utility.CreateRandom(farmer.uniqueMultiplayerID.Value);
            whichSubVariant.Value = r.Next(4);
            if (whichVariant.Value == 0 && r.NextDouble() < 0.5)
            {
                startingYForVariant.Value += 176;
            }
        }
    }

    public override void CleanupCompanion()
    {
        base.CleanupCompanion();
        if (hasLight)
        {
            Utility.removeLightSource(lightId);
        }
    }

    public override void OnOwnerWarp()
    {
        base.OnOwnerWarp();
        extraPosition = Vector2.Zero;
        extraPositionMotion = Vector2.Zero;
        extraPositionAcceleration = Vector2.Zero;

        netPosition.Value = Position;

        if (hasLight)
        {
            Game1.currentLightSources.Add(new LightSource(lightId, 1, base.Position, 2f, Color.Black, LightSource.LightContext.None, 0L));
        }
    }

    public override void Hop(float amount)
    {
    }
    
    
    private void SpecialShootFireball(GameLocation location, Farmer farmer, Monster target)
    {
        if (target == null) return;
        //Game1.addHUDMessage(new HUDMessage($"Current Location: {Game1.player.currentLocation.Name}", 3));

        _isCasting = true;
        _castEffectTimer = 0f;
        _castEffectFrame = 0;
        Vector2 ZidanQishiOffset = new Vector2(40f, -10f);
        int damage = Game1.random.Next(Damage - (int)(Damage * 0.2), Damage + (int)(Damage * 0.2) + 1) * SpecialAttackMultiplier;

        // 计算朝向目标的向量
        Vector2 velocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset,
            target.Position,
            10f
        );

        WaterEyjaProjectile SpecialFireball = new WaterEyjaProjectile(
            damageToFarmer: damage,
            spriteIndex: 37,
            bouncesTillDestruct: 0,
            tailLength: 3,
            rotationVelocity: 0.15f,
            xVelocity: velocity.X,
            yVelocity: velocity.Y,
            startingPosition: Position + _fireballOffset + ZidanQishiOffset,
            collisionSound: null,
            bounceSound: null,
            firingSound: "Kawaillusion.EyjafjallaForYou_Sound",
            explode: true,
            damagesMonsters: true,
            location: location,
            firer: farmer,
            collisionBehavior: OnSpecialFireballCollision,
            target: target // 传递目标怪物
        );

        // 弹射物属性设置
        SpecialFireball.maxVelocity.Value = 20f;
        SpecialFireball.acceleration.Value = velocity * 0.1f;
        SpecialFireball.light.Value = true;
        SpecialFireball.IgnoreLocationCollision = true;
        SpecialFireball.ignoreObjectCollisions.Value = true;

        location.projectiles.Add(SpecialFireball);
    }

    private void ShootFireball(GameLocation location, Farmer farmer, Monster target)
    {
        if (target == null) return;
        if (Game1.player.currentLocation.Name == "Slime Hutch") return;
        _isCasting = true;
        _castEffectTimer = 0f;
        _castEffectFrame = 0;
        Vector2 ZidanQishiOffset = new Vector2(40f, -15f);

        // 计算初始速度方向
        Vector2 velocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition,
            target.Position,
            8f  // 较低的初始速度，让跟踪效果更明显Damage
        );

        WaterEyjaProjectile fireball = new WaterEyjaProjectile(
            damageToFarmer: Game1.random.Next(Damage - (int)(Damage * 0.2), Damage + (int)(Damage * 0.2) + 1),
            spriteIndex: 38,
            bouncesTillDestruct: 0,
            tailLength: 3,
            rotationVelocity: 0.1f,
            xVelocity: velocity.X,
            yVelocity: velocity.Y,
            startingPosition: Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition,
            collisionSound: null,
            bounceSound: null,
            firingSound: "Kawaillusion.EyjafjallaForYou_Sound",
            explode: true,
            damagesMonsters: true,
            location: location,
            firer: farmer,
            collisionBehavior: OnFireballCollision,
            target: target  // 传递目标怪物
        );

        // 弹射物属性设置
        fireball.maxVelocity.Value = 20f;
        fireball.acceleration.Value = velocity * 0.05f;
        fireball.light.Value = true;
        fireball.IgnoreLocationCollision = true;
        fireball.ignoreObjectCollisions.Value = true;

        location.projectiles.Add(fireball);
    }
     //火山子弹
    private void VShootFireball(GameLocation location, Farmer farmer, Monster target)
    {
        if (target == null) return;
        if (Game1.player.currentLocation.Name == "Slime Hutch") return;
        _isCasting = true;
        _castEffectTimer = 0f;
        _castEffectFrame = 0;
        Vector2 ZidanQishiOffset = new Vector2(40f, -15f);

        // 计算初始速度方向
        Vector2 velocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition,
            target.Position,
            8f  // 较低的初始速度
        );

        WaterEyjaProjectile fireball = new WaterEyjaProjectile(
            damageToFarmer: Game1.random.Next(Damage - (int)(Damage * 0.2), Damage + (int)(Damage * 0.2) + 1),
            spriteIndex: 36,
            bouncesTillDestruct: 0,
            tailLength: 3,
            rotationVelocity: 0.1f,
            xVelocity: velocity.X,
            yVelocity: velocity.Y,
            startingPosition: Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition,
            collisionSound: null,
            bounceSound: null,
            firingSound: "Kawaillusion.EyjafjallaForYou_Sound",
            explode: true,
            damagesMonsters: true,
            location: location,
            firer: farmer,
            collisionBehavior: OnVFireballCollision,
            target: target  // 传递目标怪物
        );

        // 弹射物属性设置
        fireball.maxVelocity.Value = 20f;
        fireball.acceleration.Value = velocity * 0.05f;
        fireball.light.Value = true;
        fireball.IgnoreLocationCollision = true;
        fireball.ignoreObjectCollisions.Value = true;

        location.projectiles.Add(fireball);
    }

    //ss的是给手动的
    private void SSpecialShootFireball(GameLocation location, Farmer farmer, Vector2 targetPosition)
    {

        //Game1.addHUDMessage(new HUDMessage($"Current Location: {Game1.player.currentLocation.Name}", 3));
        _isCasting = true;
        _castEffectTimer = 0f;
        _castEffectFrame = 0;
        Vector2 ZidanQishiOffset = new Vector2(40f, -10f);
        Vector2 MinZhongOffset1 = new Vector2(25f, 30f);
        int damage = Game1.random.Next(Damage - (int)(Damage * 0.2), Damage + (int)(Damage * 0.2) + 1) * SpecialAttackMultiplier;

        Vector2 velocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition + MinZhongOffset1,
            targetPosition, // 使用传入的目标位置
            10f
        );

        BasicProjectile SpecialFireball = new BasicProjectile(
            damageToFarmer: damage,
            spriteIndex: 37,
            bouncesTillDestruct: 0,
            tailLength: 3,
            rotationVelocity: 0.15f,
            xVelocity: velocity.X,
            yVelocity: velocity.Y,
            startingPosition: Position + _fireballOffset + ZidanQishiOffset,
            collisionSound: null,
            bounceSound: null,
            firingSound: "Kawaillusion.EyjafjallaForYou_Sound",
            explode: true,
            damagesMonsters: true,
            location: location,
            firer: farmer,
            collisionBehavior: OnSpecialFireballCollision
        );

        // 弹射物属性设置
        SpecialFireball.maxVelocity.Value = 20f;
        SpecialFireball.acceleration.Value = velocity * 0.1f;
        SpecialFireball.light.Value = true;
        SpecialFireball.IgnoreLocationCollision = true;
        SpecialFireball.ignoreObjectCollisions.Value = true;

        location.projectiles.Add(SpecialFireball);
    }

    private void SShootFireball(GameLocation location, Farmer farmer, Vector2 targetPosition)
    {
        _isCasting = true;
        _castEffectTimer = 0f;
        _castEffectFrame = 0;
        Vector2 ZidanQishiOffset = new Vector2(40f, -15f);
        Vector2 MinZhongOffset1 = new Vector2(25f, 30f);
        Vector2 velocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition + MinZhongOffset1,
            targetPosition, // 使用传入的目标位置
            10f
        );

        BasicProjectile fireball = new BasicProjectile(
            damageToFarmer: Game1.random.Next(Damage - (int)(Damage * 0.2), Damage + (int)(Damage * 0.2) + 1),
            spriteIndex: 38,
            bouncesTillDestruct: 0,
            tailLength: 3,
            rotationVelocity: 0.1f,
            xVelocity: velocity.X,
            yVelocity: velocity.Y,
            startingPosition: Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition,
            collisionSound: null,
            bounceSound: null,
            firingSound: "Kawaillusion.EyjafjallaForYou_Sound",
            explode: true,
            damagesMonsters: true,
            location: location,
            firer: farmer,
            collisionBehavior: OnFireballCollision
        );

        // 弹射物属性设置
        fireball.maxVelocity.Value = 20f;
        fireball.acceleration.Value = velocity * 0.1f;
        fireball.light.Value = true;
        fireball.IgnoreLocationCollision = true;
        fireball.ignoreObjectCollisions.Value = true;

        location.projectiles.Add(fireball);

    }
    //火球碰撞后
    private void OnFireballCollision(GameLocation location, int x, int y, Character who)
    {
        Vector2 explosionCenter = new Vector2(x, y);
        particleManager.CreateExplosion(location, explosionCenter);
        if (this.Owner?.IsLocalPlayer ?? false)
        {
            ModEntry.StaticHelper.Multiplayer.SendMessage(
                message: new ModEntry.WaterHitEffectMessage
                {
                    EffectType = "CreateExplosion",
                    X = explosionCenter.X,
                    Y = explosionCenter.Y,
                    LocationName = location.Name
                },
                messageType: "EyjaTrinket/PlayWaterHitEffect"
            );
        }

    }
    private void OnVFireballCollision(GameLocation location, int x, int y, Character who)
    {
        Vector2 explosionCenter = new Vector2(x, y);
        particleManager.VCreateExplosion(location, explosionCenter);
        if (this.Owner?.IsLocalPlayer ?? false)
        {
            ModEntry.StaticHelper.Multiplayer.SendMessage(
                message: new ModEntry.WaterHitEffectMessage
                {
                    EffectType = "VCreateExplosion",
                    X = explosionCenter.X,
                    Y = explosionCenter.Y,
                    LocationName = location.Name
                },
                messageType: "EyjaTrinket/PlayWaterHitEffect"
            );
        }

    }
    private void OnSpecialFireballCollision(GameLocation location, int x, int y, Character who)
    {
        Vector2 explosionCenter = new Vector2(x, y);
        particleManager.CreateSpecialExplosion(location, explosionCenter);
        if (this.Owner?.IsLocalPlayer ?? false)
        {
            ModEntry.StaticHelper.Multiplayer.SendMessage(
                message: new ModEntry.WaterHitEffectMessage
                {
                    EffectType = "CreateSpecialExplosion",
                    X = explosionCenter.X,
                    Y = explosionCenter.Y,
                    LocationName = location.Name
                },
                messageType: "EyjaTrinket/PlayWaterHitEffect"
            );
        }
        Vector2 explosionTile = new Vector2(x / 64, y / 64);
        int damage = (Game1.random.Next(Damage - (int)(Damage * 0.2), Damage + (int)(Damage * 0.2) + 1) * 3) / 2;
        Farmer damageSource = Owner ?? Game1.player;
        // 爆炸
        location?.explode(
            explosionTile,
            2,
            Game1.player,                  // 伤害来源设为玩家
            damageFarmers: false,   // 不伤害玩家自己
            damage,
            false
            //!(location is Farm) && !(location is SlimeHutch)&& !(location is Cellar) && !(location is FarmHouse)//到时候直接写个false
        );
    }
    
    
}

