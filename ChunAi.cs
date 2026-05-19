using System;
using EyjaTrinket.ProjectileTAS;
using EyjaTrinket.EyjaBuff;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;
using EyjaTrinket.EyjaEffects;
using Microsoft.Xna.Framework.Input;
using EyjaTrinket;
using StardewValley.Objects;
using xTile;

namespace StardewValley.Companions;

public class ChunAi : Companion
{
    private class BeamData
    {
        public float AlphaTimer = 0f;
        public float Alpha = 0f;
        public float Rotation = 0f;
        public float Scale = 0.3f;
        public float Delay = 0f; // 延迟启动时间
        public bool IsActive = false;

        // 重置为初始状态
        public void Reset()
        {
            AlphaTimer = 0f;
            Alpha = 0f;
            Rotation = (float)new Random().NextDouble() * MathHelper.TwoPi;
            Scale = (float)new Random().NextDouble() * 0.15f + 0.35f;
            Delay = (float)new Random().NextDouble() * 250f; 
            IsActive = true;
        }
    }
    private const int Max_Beams = 12; 
    private BeamData[] _beams = new BeamData[Max_Beams];
    private float _beamSpawnTimer = 0f;
    private float _beamSpawnInterval = 250f; // 每隔x毫秒生成一个
    private int _activeBeamCount = 0;

    //普通的
    private Random rand = new Random();
    public const int VARIANT_FAIRY = 0;

    public const int VARIANT_PARROT = 1;
    
    private bool hasLight = true;

    private string lightId;

    private NetInt whichSubVariant = new NetInt(-1);

    private NetInt startingYForVariant = new NetInt(0);

    // 治疗量范围
    private int _baseHeal = 15;
    private int _Heal;

    // 治疗特效相关
    private float _healEffectTimer = 0f;
    private float _healEffectTimer1=0f;
    private bool _showHealEffect = false;//攻击会打开的
    private int LiZiLeiJi = 0;
    private bool _showWEffect = false;
    private int WLiZiLeiJi = 0;
    private Vector2 _healEffectPosition;
    

    private readonly NetVector2 netPosition = new NetVector2();
    private readonly NetFloat _netCooldownTimer = new NetFloat(0f);

    private float _fireballTimer = 1600f;
    private float HealCD = 2850f;
    private bool isShifa =false;
    private Vector2 _fireballOffset = new Vector2(-50f, -148f);

    private float _hoverTimer;
    private float RiChangLiZiTime;
    private readonly float _hoverSpeed = 5f;
    private readonly float _hoverAmplitude = 15f;
    private float _swingTimer;      // 摇摆计时器
    private float SwingSpeed = 0.004f;  // 摇摆速度（值越大摆动越快）
    private float SwingAmplitude = 0.07f; // 摇摆幅度（弧度值）
    private float _rotationAngle;
    private float _rotationAngle1;
    private Vector2 _customOffset;
    private float FZrotationAngle;
    private VolcanoWatchersHitEffect particleManager;
    private float _RandHover;
    private CustomHealingProjectile healingBall;
    public bool isVolcanicEchoes=false;//是否火山回响
    private bool isTargetPressed = false;//是否按了目标按键
    private bool isVEHealBefore = false;//是否触发了回响攻击前音效
    private float VEDuration = 0f;//持续时间 25s
    private float VECooldownTimer = 60000f;//冷却cd 50s
    private float VEHealTimer = 1600f;//回响大治疗cd
    private float _SVEHealTimer = 150f;//回响小治疗cd
    private int VEHealNumber = 0;//回响治疗次数
    private float VEWhiteLBTimer = 0f;//回响白色粒子生成计时器
    private float VEWhiteThinStarTimer = 0f;//回响白色星星生成计时器
    private bool isEyjaVEHealBefore = true;//回响攻击前音效是否播放
    float VEEyjaBlueShine=0f;
    float VEEyjaBeamsTimer = 0f;//光计时器
    float VEEyjaBeamsScaleTimer = 0f;//光计时器
    float VEEyjaBeams = 0f;//大小
    float RotationAngleVEEyjaBeams = 0f;//旋转
    private float _VEBeamAlphaTimer = 0f;  // 光束动画计时器
    private float _VEBeamAlpha = 0f;       // 当前透明度值
    private bool _isVEBeamAnimating = false; // 是否正在执行动画
    //火山回响叶子
    private int _leafFrame = 0;
    private float _leafAnimationTimer = 0f;
    private float RBAlpha = 0f;
    private float tishiCd = 500f;//回响cd提示cd
    private bool VEDone = false;//火山回响cd完成提示 false表示还没发出提示
    private bool isTiQuCd = false;//火山回响cd完成提示 false表示还没发出提示
    public ChunAi()
    {
        lightId = $"{"FlyingCompanion1"}_{Game1.random.Next()}";
        netPosition.Interpolated(false, false); // 禁用插值，立即更新
        for (int i = 0; i < Max_Beams; i++)
        {
            _beams[i] = new BeamData();
            _beams[i].Rotation = (float)rand.NextDouble() * MathHelper.TwoPi;
            _beams[i].Scale =(float)rand.NextDouble() * 0.5f+0.5f;
        }
    }

    public ChunAi(int whichVariant, int whichSubVariant = -1)
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
        base.InitNetFields(); // 必须调用基类初始化
        NetFields
            .AddField(netPosition, "netPosition") // 添加位置同步字段
            .AddField(_netCooldownTimer, "_netCooldownTimer");
    }

    public override void Update(GameTime time, GameLocation location)
    {
        base.Update(time, location);
        _baseHeal = 15;
        HealCD = 2850f;
        _Heal = Game1.random.Next(_baseHeal- (int)(_baseHeal*0.2), _baseHeal + (int)(_baseHeal * 0.2)+1);
        base.hopEvent.Poll();
        // 悬浮
        _hoverTimer += (float)time.ElapsedGameTime.TotalSeconds;
        float verticalOffset = (float)Math.Sin(_hoverTimer * _hoverSpeed) * _hoverAmplitude;

        //摆动
        _swingTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
        // 周期摆动
        _rotationAngle1 = 0.07f * (float)Math.Sin(_swingTimer * 0.004f);//左边弧度，右边速度（大叶子）
        _rotationAngle= 0.07f * (float)Math.Sin(_swingTimer * 0.005f);//小叶子
        _customOffset = new Vector2(0, verticalOffset);
        Vector2 finalPosition1 = base.Position + _customOffset;

        // 更新光源位置（按照原版的方式）
        if (hasLight && location.Equals(Game1.currentLocation))
        {
            // 计算最终位置（包括悬浮偏移）
            Vector2 finalPosition = base.Position + _customOffset;
            Utility.repositionLightSource(lightId, finalPosition + new Vector2(0, -100f));
        }
        // 确保粒子管理器有正确的location
        if (particleManager == null || particleManager.location != location)
        {
            particleManager = new VolcanoWatchersHitEffect(location);
        }
        float xxelapsedMs1 = (float)time.ElapsedGameTime.TotalMilliseconds;
        particleManager.UpdateStarSequences(xxelapsedMs1);

        RiChangLiZiTime += (float)time.ElapsedGameTime.TotalMilliseconds;
        if (RiChangLiZiTime >= 1500f)
        {
            Vector2 FP = new Vector2(23, -85);
            Vector2 FP1 = base.Position + _customOffset + FP;
            RiChangLiZiTime = 0f;
            particleManager.CreateRiChang(location, FP1);
        }
        // 依次出现粒子（普攻
        if (_showHealEffect || _showWEffect)//补药删这个，不然零帧起手
        {
            _healEffectTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            _healEffectTimer1 += (float)time.ElapsedGameTime.TotalMilliseconds;
            if (_healEffectTimer >= 100f && _showHealEffect)
            {
                
                Vector2 playerPosition = Game1.player.getStandingPosition();
                Vector2 healPosition = playerPosition +  new Vector2(
                rand.Next(-50, 45),//x
                rand.Next(-120, 10));//y
                particleManager.CreateExplosion1(location, healPosition);

                _healEffectTimer = 0f;
                LiZiLeiJi++;
                if (LiZiLeiJi >= (rand.Next(4,7)))
                {
                    LiZiLeiJi = 0;
                    _showHealEffect = false;
                }
            }
            if (_healEffectTimer1 >= 100f && _showWEffect)
            {
                Vector2 healPosition1 = _healEffectPosition;
                particleManager.CreateShuiBoWen(location, _healEffectPosition);

                _healEffectTimer = 0f;
                WLiZiLeiJi++;
                if (WLiZiLeiJi >= 2)
                {
                    WLiZiLeiJi = 0;
                    _showWEffect = false;
                }
            }
        }
      
        netPosition.Value = Position;

        if (Game1.shouldTimePass())
        {
            bool isCtrlPressed111111 = Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftControl) && Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftShift) && Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftAlt) && Game1.input.GetKeyboardState().IsKeyDown(Keys.V);
            if (isCtrlPressed111111)
            {
                VECooldownTimer = 0f;
            }
            float elapsedMs = (float)time.ElapsedGameTime.TotalMilliseconds;//按帧算时间
            bool isCtrlPressed = Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftControl) ||
                       Game1.input.GetKeyboardState().IsKeyDown(Keys.RightControl);
            bool is1Pressed = Game1.input.GetKeyboardState().IsKeyDown(Keys.L);
            if (VECooldownTimer >= 0f && !isVolcanicEchoes)
            {
                VECooldownTimer -= elapsedMs;//这个是回响cd
                if (VECooldownTimer < 0f) VECooldownTimer = 0f;
            }
            tishiCd += (float)time.ElapsedGameTime.TotalMilliseconds;
            if (is1Pressed && isCtrlPressed && !isVolcanicEchoes && VECooldownTimer != 0)
            {

                if (tishiCd > 300) { Game1.addHUDMessage(new HUDMessage(I18n.VolcanicEchoesCD() + ((int)VECooldownTimer / 1000 + 1), 2)); tishiCd = 0; }
                if (Game1.hudMessages.Count >= 2)
                {
                    Game1.hudMessages.RemoveAt(0); // 移除最旧的消息
                }


            }
            if (VECooldownTimer == 0f && !VEDone && !isVolcanicEchoes)//VEDone用来只提示一次 false表示还没发出提示
            {
                Game1.addHUDMessage(new HUDMessage(I18n.VolcanicEchoesDone(), 1));
                VEDone = true;
            }
            if (!isTiQuCd)
            {
                string? cdValue = Owner.modData.ContainsKey("EyjaTrinket.VECooldownTimer")
                    ? Owner.modData["EyjaTrinket.VECooldownTimer"]
                    : null;
                if (cdValue != null && float.TryParse(cdValue, out float VECooldownTimer1))
                {
                    VECooldownTimer = VECooldownTimer1;
                }
                isTiQuCd = true;
            }
            Owner.modData["EyjaTrinket.VECooldownTimer"] = VECooldownTimer.ToString();

            if (is1Pressed && isCtrlPressed && VECooldownTimer<=0f)//触发火山回响状态
            {
                isTargetPressed = true;
                if (isTargetPressed)
                {
                    //开回响
                    isVolcanicEchoes = true;//后面设置技能时间结束变成false
                    VEDuration = 50000f;//持续时间
                    VECooldownTimer = 60000f;
                    _leafFrame = 0;
                    _leafAnimationTimer = 0f;
                    VEDone = false;
                    Game1.playSound("EyjaVEOpen");
                }
            }
            else
            {
                isTargetPressed = false;
            }

            //火山回响状态
            if (isVolcanicEchoes)
            {
                VEDuration -= elapsedMs;

                //动画
                _leafAnimationTimer += (float)time.ElapsedGameTime.TotalMilliseconds;

                if (_leafAnimationTimer >= 40f && _leafFrame <= 10)
                {
                    _leafFrame++;
                    VEEyjaBlueShine += 0.1f;
                    _leafAnimationTimer = 0f;
                }

                // 更新光刺生成计时器
                _beamSpawnTimer += elapsedMs;

                // 生成新光刺
                if (_beamSpawnTimer >= _beamSpawnInterval && _activeBeamCount < Max_Beams)
                {
                    for (int i = 0; i < Max_Beams; i++)
                    {
                        if (!_beams[i].IsActive)
                        {
                            _beams[i].Reset();
                            _activeBeamCount++;
                            break;
                        }
                    }
                    _beamSpawnTimer = 0f;
                }

                // 更新所有光刺的动画
                for (int i = 0; i < Max_Beams; i++)
                {
                    if (_beams[i].IsActive)
                    {
                        // 处理延迟
                        if (_beams[i].Delay > 0)
                        {
                            _beams[i].Delay -= elapsedMs;
                            continue;
                        }

                        // 更新动画计时器
                        _beams[i].AlphaTimer += elapsedMs;
                        float AlphaTimer1 = 300f;
                        //出现(AlphaTimer1) + 持续(AlphaTimer1*2) + 消失(AlphaTimer1) + 等待(AlphaTimer1*4) 
                        if (_beams[i].AlphaTimer <= AlphaTimer1) // 出现
                        {
                            _beams[i].Alpha = _beams[i].AlphaTimer / AlphaTimer1;
                        }
                        else if (_beams[i].AlphaTimer <= AlphaTimer1*3) // 持续
                        {
                            _beams[i].Alpha = 1f;
                        }
                        else if (_beams[i].AlphaTimer <= AlphaTimer1*4) // 消失
                        {
                            _beams[i].Alpha = 1f - (_beams[i].AlphaTimer - AlphaTimer1 * 3) / AlphaTimer1;
                        }
                        else // 完成一个循环
                        {
                            _beams[i].Alpha = 0f;

                            // 可以选择重置或标记为非活动
                            if (_beams[i].AlphaTimer >= AlphaTimer1 * 8) // 等待
                            {
                                // 重置光刺，准备下一次动画
                                _beams[i].Reset();
                                // 或者标记为非活动：
                                // _beams[i].IsActive = false;
                                // _activeBeamCount--;
                            }
                        }
                    }
                }

                //RotationAngleVEEyjaBeams = (float)(rand.NextDouble() * MathHelper.TwoPi);
                //VEEyjaBeams = (float)rand.NextDouble() *0.5f+0.3f;//大小
                //回响普通粒子生成
                VEWhiteLBTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
                if (VEWhiteLBTimer > 75f)
                {
                    Vector2 FP = new Vector2(23, -130);//右
                    Vector2 FP1 = base.Position + _customOffset + FP+ new Vector2(
                        rand.Next(-10, 25),//x
                        rand.Next(-5, 5));//y;
                    particleManager.CreateWhiteLightBall1(location, FP1);

                    Vector2 FP2 = new Vector2(-32, -130);//左
                    Vector2 FP22 = base.Position + _customOffset + FP2 + new Vector2(
                        rand.Next(-28, 20),//x
                        rand.Next(-5, 5));//y;
                    particleManager.CreateWhiteLightBall1(location, FP22);
                    VEWhiteLBTimer = 0f;
                }
                VEWhiteThinStarTimer += (float)time.ElapsedGameTime.TotalMilliseconds;

                //细十字
                if (VEWhiteThinStarTimer > (350f + rand.Next(-100, 101)))
                {
                    Vector2 FP2 = new Vector2(-5, -105);
                    Vector2 FP22 = base.Position + _customOffset + FP2 + new Vector2(
                        rand.Next(-50, 50),//x
                        rand.Next(-60, 35));//y;
                    particleManager.VEThinStarLight(location, FP22);
                    VEWhiteThinStarTimer = 0f;
                }

                // 整体cd
                VEHealTimer += (float)time.ElapsedGameTime.TotalMilliseconds;

                // 检查是否可以开始新一轮治疗 
                if (VEHealTimer >= HealCD && !isVEHealBefore && Game1.player.health < Game1.player.maxHealth)
                {
                    VEHealTimer = 0f;  // 重置大CD
                    Game1.playSound("EyjaVEHealBefore");
                    isVEHealBefore = true;
                    VEHealNumber = 0;
                    _SVEHealTimer = 0f;
                }
                
                if (isVEHealBefore)
                {
                    _SVEHealTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
                    
                    if (_SVEHealTimer > HealCD * 0.07f && VEHealNumber < 5)
                    {
                        Vector2 playerPosition = Game1.player.getStandingPosition();
                        Vector2 lsPosition = new Vector2(-32, -50);
                        Vector2 healPosition = playerPosition + lsPosition;
                        if(VEHealNumber==0) particleManager.VEPingAiLiZi(location, finalPosition1);
                        if (Game1.player.health < Game1.player.maxHealth)
                        {
                            particleManager.VolcanicEchoesHit(healPosition, (int)(_baseHeal * 0.6f));
                            Game1.player.health = Math.Min(Game1.player.health + (int)(_baseHeal * 0.6f), Game1.player.maxHealth);
                            HandleHealingDefenseBuff(Owner);

                            particleManager.StartStarSequence(playerPosition, 3, 4);
                            particleManager.StartVolcanicEchoesThinStar2Sequence(playerPosition, 2, 3);
                            int random = rand.Next(3);
                            switch (random)
                            {
                                case 0:
                                    Game1.playSound("EyjaVolcanicEchoes1");
                                    break;
                                case 1:
                                    Game1.playSound("EyjaVolcanicEchoes2");
                                    break;
                                case 2:
                                    Game1.playSound("EyjaVolcanicEchoes3");
                                    break;
                            }
                        }
                        VEHealNumber++;
                        _SVEHealTimer = 0f;
                    }

                    if (VEHealNumber >= 5)
                    {
                        VEHealNumber = 0;
                        _SVEHealTimer = 0f;
                        isVEHealBefore = false;
                    }
                }



                if (VEDuration <= 0 && !isVEHealBefore)
                {
                    isVolcanicEchoes=false;
                    VEDuration = 0f;
                    VEHealTimer= HealCD*0.6f;
                    VEEyjaBeamsTimer = 0f;
                }

            }
            else if (!isVolcanicEchoes)//退出来
            {
                _leafAnimationTimer += (float)time.ElapsedGameTime.TotalMilliseconds;

                if (_leafAnimationTimer >= 40f && _leafFrame >= 0)
                {
                    _leafFrame--;
                    VEEyjaBlueShine -= 0.1f;
                    _leafAnimationTimer = 0f;
                }
                // 退出火山回响时重置所有光刺
                for (int i = 0; i < Max_Beams; i++)
                {
                    _beams[i].IsActive = false;
                }
                _activeBeamCount = 0;
                _beamSpawnTimer = 0f;
            }
            _fireballTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            // 普通治疗
            if (_fireballTimer >= HealCD * 0.98f && !isShifa && !isVolcanicEchoes && Game1.player.health < Game1.player.maxHealth)
            {
                particleManager.PingAiLiZi(location, finalPosition1);
                isShifa = true;
            }

            if (_fireballTimer >= HealCD && isShifa && !isVolcanicEchoes)
            {
                // 发射普通治疗弹
                //Game1.addHUDMessage(new HUDMessage("充能完毕"));
                ShootHealingBall(location, Owner);
                location.playSound("EyjaNormalHealing");
                isShifa = false;
                _fireballTimer = 0f;

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

        Texture2D texture = Game1.content.Load<Texture2D>("Mods/EyjaYuJinHua1");
        Texture2D texture1 = Game1.content.Load<Texture2D>("Mods/EyjaVEYuJinHua1");
        Texture2D EyjaLeftBranch = Game1.content.Load<Texture2D>("Mods/EyjaLeftBranch");
        Texture2D EyjaRightBranch = Game1.content.Load<Texture2D>("Mods/EyjaRightBranch");
        Texture2D EyjaSLeftBranch = Game1.content.Load<Texture2D>("Mods/EyjaSLeftBranch");
        Texture2D EyjaSRightBranch = Game1.content.Load<Texture2D>("Mods/EyjaSRightBranch");
        Texture2D EyjaBlueShine = Game1.content.Load<Texture2D>("Mods/CAEyjaBlueShine");
        Texture2D EyjaBeams = Game1.content.Load<Texture2D>("Mods/EyjaWThinBeam");
        Texture2D EyjaRainbow = Game1.content.Load<Texture2D>("Mods/EyjaRainbow");
        Vector2 YellowOffset = new Vector2(-10f, -45f);
        Vector2 ls = new Vector2(0, -100);
        Vector2 ls1 = new Vector2(32, 0);
        Vector2 finalPosition = base.Position + _customOffset + ls;
        SpriteEffects effect = SpriteEffects.None;

        // 绘制本体

        b.Draw(
                texture,
                Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
                new Rectangle(0, 0, 64, 64),
                Color.White,
                FZrotationAngle,
                new Vector2(32f, 32f),
                2f,
                effect,
                Position.Y / 10000f
            );

        //左叶子
        b.Draw(
            EyjaLeftBranch,
            Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
            new Rectangle(_leafFrame * 64, 0, 64, 64),
            Color.White,
            _rotationAngle1 - 0.12f,
            new Vector2(32f, 32f),
            2f,
            effect,
            (Position.Y - 1) / 10000f
        );
        //右叶子
        b.Draw(
            EyjaRightBranch,
            Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
            new Rectangle(_leafFrame * 64, 0, 64, 64),
            Color.White,
            -_rotationAngle1 + 0.12f,
            new Vector2(32f, 32f),
            2f,
            effect,
            (Position.Y - 1) / 10000f
        );
        //左小叶子
        b.Draw(
             EyjaSLeftBranch,
             Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
             new Rectangle(_leafFrame * 64, 0, 64, 64),
             Color.White,
             _rotationAngle - 0.12f,
             new Vector2(32f, 32f),
             2f,
             effect,
             (Position.Y - 1) / 10000f
         );
        //右小
        b.Draw(
            EyjaSRightBranch,
            Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
            new Rectangle(_leafFrame * 64, 0, 64, 64),
            Color.White,
            -_rotationAngle + 0.12f,
            new Vector2(32f, 32f),
            2f,
            effect,
            (Position.Y - 1) / 10000f
        );
        //彩虹和光雾
        b.Draw(
             EyjaRainbow,
             Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height)) + new Vector2(-17f, -45f)),
             new Rectangle(0, 0, 112, 112),
             Color.White * 0.30f* VEEyjaBlueShine,
             0f,
             new Vector2(32f, 32f),
             0.85f,
             effect,
             (Position.Y + 1) / 10000f
         );
        b.Draw(
                   EyjaBlueShine,
                   Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(15f, 0f) + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
                   new Rectangle(0, 0, 48, 48),
                   Color.White * VEEyjaBlueShine * 0.6f,
                   0f,
                   new Vector2(32f, 32f),
                   2f,
                   effect,
                   (Position.Y - 10) / 10000f
                   );
        if (isVolcanicEchoes)
        {


            // 绘制所有活动的光刺
            for (int i = 0; i < Max_Beams; i++)
            {
                if (_beams[i] != null &&_beams[i].IsActive && _beams[i].Delay <= 0 && _beams[i].Alpha > 0)
                {
                    b.Draw(
                        EyjaBeams,
                        Game1.GlobalToLocal(finalPosition + base.Owner.drawOffset + new Vector2(0f,-10f) + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, (0f - height))),
                        new Rectangle(0, 0, 192, 192),
                        Color.White * _beams[i].Alpha,  // 使用光刺的透明度
                        _beams[i].Rotation,  // 使用光刺的旋转角度
                        new Vector2(96f, 96f),
                        _beams[i].Scale,  // 使用光刺的大小
                        effect,
                        (Position.Y -15) / 10000f
                    );
                }
            }
                   
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

        netPosition.Value = Position;

        if (hasLight)
        {
            Game1.currentLightSources.Add(new LightSource(lightId, 1, base.Position, 2f, Color.Black, LightSource.LightContext.None, 0L));
        }
    }

    public override void Hop(float amount)
    {
    }
    //子弹
    private void ShootHealingBall(GameLocation location, Farmer farmer)
    {
        if (farmer == null) return;

        Vector2 ZidanQishiOffset = new Vector2(25f, -10f);//60，20

        // 初始速度朝向玩家
        Vector2 initialVelocity = Utility.getVelocityTowardPoint(
            Position + _fireballOffset + ZidanQishiOffset,
            farmer.Position,
            8f  // 低初速
        );

        CustomHealingProjectile healingBall = new CustomHealingProjectile(
            healAmount: _Heal,
            spriteIndex: 35,
            bouncesTillDestruct: 0,
            tailLength: 3,
            rotationVelocity: 1f,  // 轻微的旋转
            xVelocity: initialVelocity.X,
            yVelocity: initialVelocity.Y,
            startingPosition: Position + _fireballOffset + ZidanQishiOffset,
            collisionSound: null,
            firingSound: null,
            location: location,
            firer: farmer,
            collisionBehavior: null, // 先设为null，稍后设置
            trackStrength: 0.3f,
            maxTrackSpeed: 10f
             );

        // 创建局部变量引用
        var projectileRef = healingBall;

        // 设置碰撞行为
        healingBall.collisionBehavior = (loc, x, y, who) => OnHealingCollision(loc, x, y, who, projectileRef);

        // 弹射物属性设置
        healingBall.maxVelocity.Value = 20f;
        healingBall.acceleration.Value = Vector2.Zero; 
        healingBall.IgnoreLocationCollision = true;
        healingBall.ignoreObjectCollisions.Value = true;
        healingBall.light.Value = true;
        //healingBall.ignoreMeleeAttacks.Value = true;
        //healingBall.ignoreCharacterCollisions.Value = true;
        location.projectiles.Add(healingBall);
    }



    // 修改碰撞回调方法，增加projectile参数
    private void OnHealingCollision(GameLocation location, int x, int y, Character who, CustomHealingProjectile projectile)
    {
        if (who is Farmer farmer && farmer == Owner)
        {
            // 使用子弹头的精确位置
            Vector2 healPosition = new Vector2(x, y);

            // 触发治疗效果
            particleManager.CreateHealHitEffect(healPosition, _Heal);
            location.playSound("EyjaNormalHealingHit");
            _healEffectPosition = healPosition;
            _showHealEffect = true;
            _showWEffect = true;
            _healEffectTimer = 0f;

            HandleHealingDefenseBuff(farmer);
        }
    }
    // 在 ChunAi.cs 中
    private void HandleHealingDefenseBuff(Farmer farmer)
    {
        ReplenishingMistBuff existingBuff = null;

        foreach (var kvp in farmer.buffs.AppliedBuffs)
        {
            if (kvp.Value is ReplenishingMistBuff healingBuff)
            {
                existingBuff = healingBuff;
                break;
            }
        }

        if (existingBuff != null)
        {
            farmer.buffs.Remove(existingBuff.id);

            if (existingBuff.CurrentLevel < 3)
            {
                // 传递当前的 _baseHeal
                var newBuff = new ReplenishingMistBuff(existingBuff.CurrentLevel + 1, false, _baseHeal);
                farmer.buffs.Apply(newBuff);
            }
            else
            {
                var newBuff = new ReplenishingMistBuff(3, false, _baseHeal);
                farmer.buffs.Apply(newBuff);
            }
        }
        else
        {
            // 创建新 Buff 时传递 _baseHeal
            farmer.buffs.Apply(new ReplenishingMistBuff(1, false, _baseHeal));
        }

        Game1.buffsDisplay.dirty = true;
    }
    
   

}

