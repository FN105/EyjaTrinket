using System;
using EyjaTrinket;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;

namespace StardewValley.Companions;

public class FlyingCompanion1 : Companion
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


    // ===================== 自定义字段 =====================
    private float _fireballTimer = 1600f;
    private float _fireballDelay = 1600f;
    private float _specialAttackTimer = 6500f;
    private const float SpecialAttackInterval = 5000f;
    private const int SpecialAttackMultiplier = 3;
    private int _minDamage = 70;
    private int _maxDamage = 120;
    private Vector2 _fireballOffset = new Vector2(-50f, -148f);

    private float _hoverTimer;
    private readonly float _hoverSpeed = 5f;
    private readonly float _hoverAmplitude = 15f;
    private Vector2 _customOffset;

    private float _castEffectTimer;
    private int _castEffectFrame;
    private bool _isCasting;
    private const float FrameDuration = 60f;

    private float rotationAngle;
    private float FZrotationAngle;
    private float elapsedMilliseconds;
    private float elapsedMilliseconds1;
    private float elapsedMilliseconds2;

    private bool YellowC;
    private bool Yellow1;
    private bool Yellow2;
    private bool Yellow3;

    private bool _multiAttackMode = false;
    private float _multiAttackTimer = 0f;
    private float _cooldownTimer = 50000f;
    private bool _wasCtrl1Pressed = false;          // 防止重复触发
    private bool _showSkillEffect = false;          // 显示技能特效
    private float _skillEffectTimer = 0f;           // 技能特效计时器
    private Vector2 lsPosition;
    private Vector2 VolcanoZiDanPosition;
    private float tishi=500f;
    private bool VDone = true;
    private readonly NetBool _netManualMode = new NetBool(false); // 手动模式状态
    private bool _wasCtrlLPressed; // 防止Ctrl+L重复触发
    private float _dailyParticleTimer;

    public FlyingCompanion1()
    {
        lightId = $"{"FlyingCompanion1"}_{Game1.random.Next()}";
        netPosition.Interpolated(false, false); //
    }

    public FlyingCompanion1(int whichVariant, int whichSubVariant = -1)
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
        base.InitNetFields(); // 调用初始化
        NetFields
            .AddField(netPosition, "netPosition") // 同步字段
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


    public override void Update(GameTime time, GameLocation location)
    {
        base.Update(time, location);

        base.hopEvent.Poll(); 

        // 悬浮
        _hoverTimer += (float)time.ElapsedGameTime.TotalSeconds;
        float verticalOffset = (float)Math.Sin(_hoverTimer * _hoverSpeed) * _hoverAmplitude;
        _customOffset = new Vector2(0, verticalOffset);

        // 更新特效状态
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

        if (Game1.shouldTimePass())
        {
            elapsedMilliseconds += (float)time.ElapsedGameTime.TotalMilliseconds;
            _dailyParticleTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            if (_dailyParticleTimer >= 150f)
            {
                //Game1.addHUDMessage(new HUDMessage("日常特效"));
                //AddCasualParticles(location, Position);
                _dailyParticleTimer = 0f;
            }
            // 每10ms转一次（黄圈）
            if (elapsedMilliseconds >= 10)
            {
                rotationAngle -= 0.03f;
                elapsedMilliseconds -= 10;
                rotationAngle %= MathHelper.TwoPi;
            }
            if (_multiAttackMode) //（开大旋转）
            {
                elapsedMilliseconds1 += (float)time.ElapsedGameTime.TotalMilliseconds;
                // 每1ms转一次
                if (elapsedMilliseconds1 >= 1)
                {
                    FZrotationAngle -= 0.5f;
                    elapsedMilliseconds1 -= 1;
                    FZrotationAngle %= MathHelper.TwoPi;
                }
                elapsedMilliseconds2 += (float)time.ElapsedGameTime.TotalMilliseconds;
                // 每1ms转一次（粒子）
                if (elapsedMilliseconds2 >= 100)
                {

                    elapsedMilliseconds -= 10;
                    Vector2 ls = new Vector2(42, 205);
                    lsPosition = base.Position + _customOffset + ls;
                    AddVolcanoParticles(location, lsPosition);
                }
            }
            else
            {
                FZrotationAngle = 0f;
            }

            // 这是黄圈
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

            // 火山计时器
            if (_multiAttackMode)
            {
                _multiAttackTimer -= elapsedMs;
                if (_multiAttackTimer <= 0f)
                {
                    _multiAttackMode = false;
                    _multiAttackTimer = 0f;
                    _cooldownTimer = 60000f;
                }
            }

            // 冷却计时器
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= elapsedMs;
                if (_cooldownTimer < 0f) _cooldownTimer = 0f;
            }

            // 技能特效计时器
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
            // 检测技能按键 (Ctrl+1)
            bool isCtrlPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) ||
                                 Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);
            bool is1Pressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.L);
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
            if (isCtrlPressed && is1Pressed)
            {
                if (!_wasCtrl1Pressed && _cooldownTimer <= 0f && !_multiAttackMode)
                {
                    // 激活火山
                    _multiAttackMode = true;
                    _multiAttackTimer = 7000;
                    _showSkillEffect = true;
                    _skillEffectTimer = 0f;
                    VDone = true;
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
                    // 切换手动
                    _netManualMode.Value = !_netManualMode.Value;

                    // 提示信息
                    if (_netManualMode.Value)
                        Game1.addHUDMessage(new HUDMessage(I18n.EyjaShouDongOpen(),2));
                    else
                        Game1.addHUDMessage(new HUDMessage(I18n.EyjaShouDongClose(),2));
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
            if (Game1.IsMasterGame)
            {
                List<Monster> targets;

                // 根据模式选择目标
                if (_multiAttackMode)
                {
                    //火山
                    targets = findMonstersWithinRange(location, Owner.Position, 850, 6);
                    _minDamage = (int)(70 * 2.3);
                    _maxDamage = (int)(140 * 2.3);
                    _fireballDelay = 400f;
                }
                else if (_netManualMode.Value)
                {
                    Monster singleTarget = Utility.findClosestMonsterWithinRange(location, Owner.Position, 850);
                    targets = singleTarget != null ? new List<Monster> { singleTarget } : new List<Monster>();
                    _minDamage = 70;
                    _maxDamage = 120;
                    _fireballDelay = 800f;
                }
                else
                {
                    // 普通
                    Monster singleTarget = Utility.findClosestMonsterWithinRange(location, Owner.Position, 850);
                    targets = singleTarget != null ? new List<Monster> { singleTarget } : new List<Monster>();
                    _minDamage = 70;
                    _maxDamage = 120;
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
                                VolcanoZiDanPosition = new(-25, 25);
                            }
                            else
                            {
                                VolcanoZiDanPosition = new(0, 0);
                            }

                            foreach (Monster target in targets)
                            {
                                ShootFireball(location, Owner, target);
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

                        VolcanoZiDanPosition = new(-25, 25);

                        foreach (Monster target in targets)
                        {
                            //Game1.addHUDMessage(new HUDMessage("当前速度=" + _fireballDelay));
                            ShootFireball(location, Owner, target);
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
    }

    public override void Draw(SpriteBatch b)
    {
        if (base.Owner?.currentLocation == null ||
            (base.Owner.currentLocation.DisplayName == "Temp" && !Game1.isFestival()))
        {
            return;
        }

        Texture2D texture = Game1.content.Load<Texture2D>("Mods/EyjafjallaSpark");
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

        // 绘制黄圈（就那个点燃层数）
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
            Texture2D castTexture = Game1.content.Load<Texture2D>("Mods/Eyjacast");
            Rectangle sourceRect = new Rectangle(_castEffectFrame * 16, 0, 16, 16);
            Vector2 effectPosition = new Vector2(20f, -33f);

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
    }

    public override void InitializeCompanion(Farmer farmer)
    {
        base.InitializeCompanion(farmer);
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

        _isCasting = true;
        _castEffectTimer = 0f;
        _castEffectFrame = 0;
        Vector2 ZidanQishiOffset = new Vector2(40f, -10f);
        int damage = Game1.random.Next(_minDamage, _maxDamage + 1) * SpecialAttackMultiplier;

        // 计算朝向目标的向量
        Vector2 velocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset,
            target.Position,
            10f
        );

        BasicProjectile SpecialFireball = new BasicProjectile(
            damageToFarmer: damage,
            spriteIndex: 10,
            bouncesTillDestruct: 0,
            tailLength: 3,
            rotationVelocity: 0.1f,
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

    private void ShootFireball(GameLocation location, Farmer farmer, Monster target)
    {
        if (target == null) return;
        if (Game1.player.currentLocation.Name == "Slime Hutch") return;
        _isCasting = true;
        _castEffectTimer = 0f;
        _castEffectFrame = 0;
        Vector2 ZidanQishiOffset = new Vector2(40f, -15f);

        // 计算朝向目标的向量
        Vector2 velocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition,
            target.Position,
            10f
        );

        BasicProjectile fireball = new BasicProjectile(
            damageToFarmer: Game1.random.Next(_minDamage, _maxDamage + 1),
            spriteIndex: 39,
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
    private void SSpecialShootFireball(GameLocation location, Farmer farmer, Vector2 targetPosition)
    {


        _isCasting = true;
        _castEffectTimer = 0f;
        _castEffectFrame = 0;
        Vector2 ZidanQishiOffset = new Vector2(40f, -10f);
        Vector2 MinZhongOffset1 = new Vector2(25f, 30f);
        int damage = Game1.random.Next(_minDamage, _maxDamage + 1) * SpecialAttackMultiplier;

        Vector2 velocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset + VolcanoZiDanPosition + MinZhongOffset1,
            targetPosition, // 使用传入的目标位置
            10f
        );

        BasicProjectile SpecialFireball = new BasicProjectile(
            damageToFarmer: damage,
            spriteIndex: 10,
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
            damageToFarmer: Game1.random.Next(_minDamage, _maxDamage + 1),
            spriteIndex: 39,
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
    private void OnFireballCollision(GameLocation location, int x, int y, Character who)
    {
        // 调用饰品效果中的爆炸方法
        Vector2 explosionCenter = new Vector2(x, y);
        CreateExplosion(location, explosionCenter);

    }
    private void OnSpecialFireballCollision(GameLocation location, int x, int y, Character who)
    {
        // 调用饰品效果中的爆炸方法
        Vector2 explosionCenter = new Vector2(x, y);
        CreateSpecialExplosion(location, explosionCenter);
        Vector2 explosionTile = new Vector2(x / 64, y / 64);
        int damage = (Game1.random.Next(_minDamage, _maxDamage + 1) * 3) / 2;
        Farmer damageSource = Owner ?? Game1.player;
        // 触发爆炸效果
        location?.explode(
            explosionTile,
            2,
            Game1.player,                  // 伤害来源设为玩家
            damageFarmers: false,   // 不伤害玩家自己
            damage,
            !(location is Farm) && !(location is SlimeHutch) && !(location is Cellar) && !(location is FarmHouse)
        );
    }
    public void CreateExplosion(GameLocation location, Vector2 epicenter)
    {
        Random rand = new Random();
        //float drop = 0.03f;
        //float drop = 0.03f;
        for (int i = 0; i < 11; i++)
        {
            float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
            float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
            float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
            Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

            // 创建粒子对象
            var particle = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaFireAnimation",
                sourceRect: new Rectangle(0, 0, 7, 7),
                animationInterval: 40f,
                animationLength: 15,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.9f,
                alphaFade: 0.001f,
                color: Color.White,
                scale: 3.5f,
                scaleChange: 0f,
                rotation: 0f,
                rotationChange: 0.05f
            );

            //  
            particle.motion = new Vector2(
                (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                (float)Math.Sin(angle) * speed //（-下+上）
            );
            //particle.acceleration = new Vector2(0, 0.3f);//(+粒子向下飘,-粒子向上飘)

            location.temporarySprites.Add(particle);
        }
        for (int i = 0; i < 9; i++)
        {
            float angle = (float)(rand.NextDouble() * Math.PI * 2);
            float speed = (float)(rand.NextDouble() * 2 + 0.3);
            float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
            Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

            // 创建粒子对象
            var particle = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaSmallFire",
                sourceRect: new Rectangle(0, 0, 5, 5),
                animationInterval: 600f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.9f,
                alphaFade: 0.001f,
                color: Color.White,
                scale: 3.5f,
                scaleChange: 0f,
                rotation: 0f,
                rotationChange: 0.05f
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

            // 创建粒子对象
            var particle = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaFire",
                sourceRect: new Rectangle(0, 0, 5, 5),
                animationInterval: 600f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.9f,
                alphaFade: 0.001f,
                color: Color.White,
                scale: 3.5f,
                scaleChange: 0f,
                rotation: 0f,
                rotationChange: 0.05f
            );

            //  
            particle.motion = new Vector2(
                (float)Math.Cos(angle) * speed,
                (float)Math.Sin(angle) * speed
            );
            //particle.acceleration = new Vector2(0, 0.3f);

            location.temporarySprites.Add(particle);
        }

        //烟雾效果
        for (int i = 0; i < 8; i++)
        {
            var smoke = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaSmoke",
                sourceRect: new Rectangle(0, 0, 8, 8),
                animationInterval: 1000f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.8f,
                alphaFade: 0.01f,
                color: Color.Lerp(Color.Gray, Color.Black, (float)rand.NextDouble()),
                scale: 3f,
                scaleChange: 0.1f,
                rotation: 0f,
                rotationChange: 0f
            );

            smoke.motion = new Vector2(
                (float)(rand.NextDouble() - 0.5) * 2f,
                (float)(rand.NextDouble() - 0.5) * 2f
            );
            smoke.acceleration = new Vector2(0, -0.04f);

            location.temporarySprites.Add(smoke);
        }
        for (int i = 0; i < 3; i++)
        {
            var Bigsmoke = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaBigSmoke",
                sourceRect: new Rectangle(0, 0, 32, 32),
                animationInterval: 1000f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.8f,
                alphaFade: 0.01f,
                color: Color.White,
                scale: 2f,
                scaleChange: -0.01f,
                rotation: 0f,
                rotationChange: 0.05f
            );

            Bigsmoke.motion = new Vector2(
                (float)(rand.NextDouble() - 0.5) * 2f,
                (float)(rand.NextDouble() - 0.5) * 2f
            );
            Bigsmoke.acceleration = new Vector2(0, -0.04f);

            location.temporarySprites.Add(Bigsmoke);
        }

        location.playSound("EyjaFireBoom");

    }
    public void CreateSpecialExplosion(GameLocation location, Vector2 epicenter)
    {
        Random rand = new Random();
        //float drop = 0.03f;
        //float drop = 0.03f;
        for (int i = 0; i < 20; i++)
        {
            float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
            float speed = (float)(rand.NextDouble() * 5 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
            float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
            Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

            // 创建粒子对象
            var particle = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaFireAnimation",
                sourceRect: new Rectangle(0, 0, 7, 7),
                animationInterval: 40f,
                animationLength: 15,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.9f,
                alphaFade: 0.001f,
                color: Color.White,
                scale: 3.5f,
                scaleChange: 0f,
                rotation: 0f,
                rotationChange: 0.05f
            );

            //  
            particle.motion = new Vector2(
                (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                (float)Math.Sin(angle) * speed //（-下+上）
            );
            //particle.acceleration = new Vector2(0, 0.3f);//(+粒子向下飘,-粒子向上飘)

            location.temporarySprites.Add(particle);
        }
        for (int i = 0; i < 22; i++)
        {
            float angle = (float)(rand.NextDouble() * Math.PI * 2);
            float speed = (float)(rand.NextDouble() * 5 + 0.3);
            float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
            Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

            // 创建粒子对象
            var particle = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaSmallFire",
                sourceRect: new Rectangle(0, 0, 5, 5),
                animationInterval: 600f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.9f,
                alphaFade: 0.001f,
                color: Color.White,
                scale: 3.5f,
                scaleChange: 0f,
                rotation: 0f,
                rotationChange: 0.05f
            );

            //  
            particle.motion = new Vector2(
                (float)Math.Cos(angle) * speed,
                (float)Math.Sin(angle) * speed
            );
            //particle.acceleration = new Vector2(0, 0.3f);

            location.temporarySprites.Add(particle);
        }
        for (int i = 0; i < 12; i++)
        {
            float angle = (float)(rand.NextDouble() * Math.PI * 2);
            float speed = (float)(rand.NextDouble() * 5 + 0.3);
            float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
            Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());

            // 创建粒子对象
            var particle = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaFire",
                sourceRect: new Rectangle(0, 0, 5, 5),
                animationInterval: 600f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.9f,
                alphaFade: 0.001f,
                color: Color.White,
                scale: 3.5f,
                scaleChange: 0f,
                rotation: 0f,
                rotationChange: 0.05f
            );

            //  
            particle.motion = new Vector2(
                (float)Math.Cos(angle) * speed,
                (float)Math.Sin(angle) * speed
            );
            //particle.acceleration = new Vector2(0, 0.3f);

            location.temporarySprites.Add(particle);
        }

        //烟雾效果
        for (int i = 0; i < 8; i++)
        {
            var smoke = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaSmoke",
                sourceRect: new Rectangle(0, 0, 8, 8),
                animationInterval: 1000f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.8f,
                alphaFade: 0.01f,
                color: Color.Lerp(Color.Gray, Color.Black, (float)rand.NextDouble()),
                scale: 4f,
                scaleChange: 0.1f,
                rotation: 0f,
                rotationChange: 0f
            );

            smoke.motion = new Vector2(
                (float)(rand.NextDouble() - 0.5) * 2f,
                (float)(rand.NextDouble() - 0.5) * 2f
            );
            smoke.acceleration = new Vector2(0, -0.06f);

            location.temporarySprites.Add(smoke);
        }
        for (int i = 0; i < 3; i++)
        {
            float angle = (float)(rand.NextDouble() * Math.PI * 2);
            float speed = (float)(rand.NextDouble() * 1 + 0.3);
            float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
            var Bigsmoke = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaBigSmoke",
                sourceRect: new Rectangle(0, 0, 32, 32),
                animationInterval: 1000f,
                animationLength: 1,
                numberOfLoops: 1,
                position: epicenter,
                flicker: false,
                flipped: false,
                layerDepth: 0.8f,
                alphaFade: 0.01f,
                color: Color.White,
                scale: 2.5f,
                scaleChange: -0.01f,
                rotation: 0f,
                rotationChange: 0.05f
            );

            Bigsmoke.motion = new Vector2(
                //(float)(rand.NextDouble() - 0.5) * 2f,
                //(float)(rand.NextDouble() - 0.5) * 2f
                (float)Math.Cos(angle) * speed,
                (float)Math.Sin(angle) * speed - 2f
            );
            Bigsmoke.acceleration = new Vector2(0, -0.03f);

            location.temporarySprites.Add(Bigsmoke);
        }


        location.playSound("EyjaFireBoom");

    }
    private void AddVolcanoParticles(GameLocation location, Vector2 position)
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
        Vector2 effectPosition = position + _fireballOffset + JiaoZheng; // 叠加偏移
        Random rand = new Random();
        int randomNumber = rand.Next(0, 3);
        int randomNumber1 = rand.Next(0, 2);
        if (randomNumber == 1)
        {
            for (int i = 0; i < randomNumber1; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 0.9 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(2.5 + rand.NextDouble() * 0.5);//大小()
                                                                      // 蓝色光球
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaFireAnimation",
                    sourceRect: new Rectangle(0, 0, 7, 7),//源矩形：新的矩形(0, 0, 7, 7)
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
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//(+粒子向下飘,-粒子向上飘)

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
                float scale1 = (float)(2.5 + rand.NextDouble() * 0.5);//大小()
                                                                      // 蓝色光球
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaSmallFire",
                    sourceRect: new Rectangle(0, 0, 5, 5),//源矩形：新的矩形(0, 0, 7, 7)
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
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
        else
        {
            for (int i = 0; i < randomNumber1; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 0.9 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(2.5 + rand.NextDouble() * 0.5);//大小()
                                                                      // 蓝色光球
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/EyjaFire",
                    sourceRect: new Rectangle(0, 0, 5, 5),//源矩形：新的矩形(0, 0, 7, 7)
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
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );
                //particle.acceleration = new Vector2(0, 0.3f);//(+粒子向下飘,-粒子向上飘)

                location.temporarySprites.Add(particle);
            }
        }
    }
    //日常特效
    private void AddCasualParticles(GameLocation location, Vector2 position)
    {
        Vector2 ZOffset18 = new Vector2(32, 50);
        Vector2 SOffset = new Vector2(-32, -32);
        Vector2 Offset36 = new Vector2(-36, -36);
        Vector2 Offset48 = new Vector2(-48, -48);
        Vector2 Offset64 = new Vector2(-64, -64);
        Vector2 newSOffset = new Vector2(32, -32);
        Vector2 newShine7fOffset = new Vector2(-96, -96);//64

        Vector2 Dianhu = new Vector2(50, 50);
        Vector2 effectPosition = position + _fireballOffset + ZOffset18+ newSOffset; // 叠加偏移
        Random rand = new Random();
        int randomNumber = rand.Next(10, 30);
        int randomNumber1 = rand.Next(0, 10);

        for (int i = 0; i < randomNumber; i++)
        {
            float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
            float speed = (float)(rand.NextDouble() * 1 + 0.5);//（2,4）的速度（NextDouble()是[0，1））
            float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
            Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
            float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.03);//角度()
            float scale1 = (float)(4 + rand.NextDouble() * 0.5);//大小()
            int HuaBanZhen = (rand.Next(0, 11) * 16);
            var particle = new TemporaryAnimatedSprite(
                textureName: "Mods/EyjaSmallFire",
                sourceRect: new Rectangle(HuaBanZhen, 0, 5, 5),//源矩形：新的矩形(0, 0, 7, 7)16 32
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

            // 花瓣飘落参数
            float horizontalVariation = (float)(rand.NextDouble() * 0.4 - 0.2); // -0.2 到 +0.2
            float verticalFloat = (float)(rand.NextDouble() * 0.15 + 0.1); // 0.2-0.5

            // 飘落
           particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,//-往左偏，+往右偏
                    (float)Math.Sin(angle) * speed //（-下+上）
                );


            location.temporarySprites.Add(particle);

        }

    }
}

