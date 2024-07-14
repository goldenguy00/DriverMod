using UnityEngine;
using RoR2;
using EntityStates;
using RobDriver.Modules;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;
using RoR2.HudOverlay;
using UnityEngine.AddressableAssets;
using R2API;

namespace RobDriver.SkillStates.Driver
{
    public class SteadyAim : BaseDriverSkillState
    {
        public float baseShotDuration = 0.3f;
        public float baseChargeDuration = 0.15f;

        public static float damageCoefficient = 5f;
        public static float recoil = 0.5f;

        protected bool lastCharge;

        public bool skipAnim = false;

        protected virtual bool isPiercing
        {
            get
            {
                return false;
            }
        }

        protected virtual float _damageCoefficient
        {
            get
            {
                return SteadyAim.damageCoefficient;
            }
        }

        protected virtual GameObject tracerPrefab
        {
            get
            {
                if (this.isCrit) return Shoot.critTracerEffectPrefab;
                return Shoot.tracerEffectPrefab;
            }
        }

        public CameraParamsOverrideHandle camParamsOverrideHandle;
        private OverlayController overlayController;
        private float shotCooldown;
        private float chargeTimer;
        private float chargeDuration;
        private bool isCharged;
        private bool isCrit;
        private int cachedShots;
        private float cachedShotTimer;
        private PrimarySkillShurikenBehavior shurikenComponent;
        private bool _autoFocus;
        private bool autoFocus;
        private bool cancelling;
        private bool adaptiveFocus;
        private bool reloading;
        private GameObject lightEffectInstance;
        private Animator animator;

        private bool jamFlag; // fired shortly after entering state

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = this.GetModelAnimator();
            if (!this.camParamsOverrideHandle.isValid) this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, DriverCameraParams.AIM_PISTOL, 0.5f);

            base.PlayAnimation("AimPitch", "SteadyAimPitch");

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.shurikenComponent = this.GetComponent<PrimarySkillShurikenBehavior>();

            if (this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onInventoryChanged += Inventory_onInventoryChanged;
            }

            this.characterBody._defaultCrosshairPrefab = Modules.Assets.pistolAimCrosshairPrefab;
            this._autoFocus = Modules.Config.autoFocus.Value;
            this.adaptiveFocus = Modules.Config.adaptiveFocus.Value;

            this.chargeDuration = this.baseChargeDuration / this.attackSpeedStat;
            this.autoFocus = false;
            if (this.adaptiveFocus && this.chargeDuration <= 0.1f) this.autoFocus = true;
            if (this._autoFocus) this.autoFocus = true;

            if (!this.skipAnim)
            {
                this.PlayAnim();
                Util.PlaySound("sfx_driver_aim_foley", this.gameObject);
            }

            this.FindModelChild("PistolSight").gameObject.SetActive(true);

            if (this.iDrive.passive.isPistolOnly || this.iDrive.passive.isBullets || this.iDrive.passive.isRyan)
            {
                this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams
                {
                    prefab = Modules.Assets.headshotOverlay,
                    childLocatorEntry = "ScopeContainer"
                });

                if(!Config.defaultPistolAnims.Value) this.animator.SetLayerWeight(this.animator.GetLayerIndex("AltPistol, Override"), 1f);
                else this.animator.SetLayerWeight(this.animator.GetLayerIndex("AltPistol, Override"), 0f);
            }
            else this.animator.SetLayerWeight(this.animator.GetLayerIndex("AltPistol, Override"), 0f);

            this.lightEffectInstance = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("GunLight"));
        }

        protected virtual void PlayAnim()
        {
            base.PlayAnimation("Gesture, Override", "SteadyAim", "Action.playbackRate", 0.25f);
        }

        protected virtual void PlayExitAnim()
        {
            base.PlayAnimation("Gesture, Override", "SteadyAimEnd", "Action.playbackRate", 0.2f);
        }

        private void Inventory_onInventoryChanged()
        {
            this.shurikenComponent = this.GetComponent<PrimarySkillShurikenBehavior>();
        }

        private void UpdateLightEffect()
        {
            Ray ray = this.GetAimRay();
            RaycastHit raycastHit;
            if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, Shoot.range, LayerIndex.CommonMasks.bullet))
            {
                this.lightEffectInstance.SetActive(true);
                this.lightEffectInstance.transform.position = raycastHit.point + (ray.direction * -0.3f);
            }
            else this.lightEffectInstance.SetActive(false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.chargeDuration = this.baseChargeDuration / this.attackSpeedStat;
            this.autoFocus = false;
            if (this.adaptiveFocus && this.chargeDuration <= 0.1f) this.autoFocus = true;
            if (this._autoFocus) this.autoFocus = true;
            this.shotCooldown -= Time.fixedDeltaTime;
            this.cachedShotTimer -= Time.fixedDeltaTime;
            this.characterBody.outOfCombatStopwatch = 0f;
            this.characterBody.isSprinting = false;
            base.characterBody.SetAimTimer(0.2f);
            this.attackSpeedStat = this.characterBody.attackSpeed;
            this.damageStat = this.characterBody.damage;
            this.critStat = this.characterBody.crit;

            this.UpdateLightEffect();

            if (this.iDrive && this.iDrive.weaponDef.nameToken != this.cachedWeaponDef.nameToken)
            {
                this.cancelling = true;
                this.outer.SetNextStateToMain();
                return;
            }

            if (this.reloading && this.shotCooldown <= 0f)
            {
                this.reloading = false;
                this.iDrive.FinishReload();
            }

            if (this.skillLocator.secondary.stock < 1)
            {
                this.isCharged = false;
                this.chargeTimer = 0f;
            }
            else
            {
                if (this.shotCooldown <= 0f) this.chargeTimer += Time.fixedDeltaTime;
            }

            if (!this.isCharged)
            {
                if (this.chargeTimer >= this.chargeDuration)
                {
                    this.isCharged = true;
                    Util.PlaySound("sfx_driver_pistol_ready", this.gameObject);
                }
            }
                
            if (this.iDrive.weaponTimer <= 0f && this.iDrive.maxWeaponTimer > 0)
            {
                if (this.shotCooldown <= 0f && this.inputBank.skill1.down && base.isAuthority)
                {
                    this.outer.SetNextState(new ReloadPistol
                    {
                        animString = "SteadyAimReload",
                        camParamsOverrideHandle = this.camParamsOverrideHandle,
                        aiming = true
                    });
                    this.reloading = true;
                    /*this.shotCooldown = 2.4f / this.attackSpeedStat;
                    this.reloading = true;
                    base.PlayCrossfade("Gesture, Override", "SteadyAimReload", "Action.playbackRate", this.shotCooldown, 0.1f);
                    Util.PlaySound("sfx_driver_reload_01", this.gameObject);*/
                }
            }
            else if (shotCooldown <= 0f)
            {
                if (this.autoFocus)
                {
                    if (this.inputBank.skill1.down)
                    {
                        if (this.skillLocator.secondary.stock > 0)
                        {
                            if (this.isCharged)
                            {
                                this.isCrit = this.RollCrit();
                                this.Fire();
                            }
                        }
                        else
                        {
                            this.isCrit = this.RollCrit();
                            this.Fire();
                        }
                    }
                }
                else
                {
                    if (this.inputBank.skill1.down)
                    {
                        this.isCrit = this.RollCrit();
                        this.Fire();
                    }
                }
            }

            if (this.cachedShots > 0 && base.isAuthority)
            {
                if (this.cachedShotTimer <= 0f)
                {
                    this.Fire2(this.cachedShots);
                    this.cachedShots = 0;
                }
            }

            if (!this.inputBank.skill2.down && base.isAuthority && !this.reloading)
            {
                if (this.jamFlag && this.shotCooldown > 0f)
                {
                    // add jam buildup
                    if (this.iDrive)
                    {
                        if (iDrive.AddJamBuildup())
                        {
                            this.outer.SetNextState(new JammedGun());
                            return;
                        }
                    }
                }

                this.outer.SetNextState(new WaitForReload());
            }

            if (this.iDrive)
            {
                if (this.skillLocator.secondary.stock < 1)
                {
                    this.iDrive.chargeValue = 0f;
                    return;
                }

                float value = Util.Remap(this.chargeTimer, 0f, this.chargeDuration, 0f, 1f);
                this.iDrive.chargeValue = value;
            }
        }

        protected virtual void PlayShootAnim(bool wasCharged, bool wasCrit, float speed)
        {
            string animString = "SteadyAimFire";

            if (wasCharged)
            {
                if (wasCrit) animString = "SteadyAimFireChargedCritical";
                else animString = "SteadyAimFireCharged";
            }
            else
            {
                if (wasCrit) animString = "SteadyAimFireCritical";
            }

            base.PlayAnimation("Gesture, Override", animString, "Action.playbackRate", speed);
        }

        protected virtual string GetSoundString(bool crit, bool charged)
        {
            string soundString = "sfx_driver_pistol_shoot";
            if (crit) soundString = "sfx_driver_pistol_shoot_critical";
            if (charged) soundString = "sfx_driver_pistol_shoot_charged";
            return soundString;
        }

        public virtual void Fire()
        {
            if (this.iDrive.maxWeaponTimer > 0) this.iDrive.ConsumeAmmo(1f, false);

            if (this.shurikenComponent) shurikenComponent.OnSkillActivated(base.skillLocator.primary);

            if (base.fixedAge <= 0.25f) this.jamFlag = true;

            bool wasCharged = this.isCharged;

            this.shotCooldown = this.baseShotDuration / this.attackSpeedStat;
            this.chargeTimer = 0f;
            this.isCharged = false;

            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, "PistolMuzzle", false);

            
            if (this.isCrit)
            {
                this.cachedShots++;
                this.cachedShotTimer = 0.05f;
            }

            if (wasCharged)
            {
                base.characterBody.AddSpreadBloom(1.5f);
            }
            else
            {
                base.characterBody.AddSpreadBloom(0.35f);
            }

            Util.PlaySound(this.GetSoundString(this.isCrit, wasCharged), this.gameObject);

            this.PlayShootAnim(wasCharged, this.isCrit, this.shotCooldown * 1.5f);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.AddRecoil2(-1f * SteadyAim.recoil, -2f * SteadyAim.recoil, -0.5f * SteadyAim.recoil, 0.5f * SteadyAim.recoil);

                float dmg = Shoot.damageCoefficient;

                if (wasCharged)
                {
                    dmg = this._damageCoefficient;

                    this.skillLocator.secondary.DeductStock(1);
                }

                this.lastCharge = wasCharged;

                BulletAttack.FalloffModel falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                if (this.lastCharge) falloffModel = BulletAttack.FalloffModel.None;

                if (this.isPiercing)
                {
                    BulletAttack bulletAttack = new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = dmg * this.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = iDrive.DamageType,
                        falloffModel = falloffModel,
                        maxDistance = Shoot.range,
                        force = Shoot.force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = isCrit,
                        owner = base.gameObject,
                        muzzleName = "PistolMuzzle",
                        smartCollision = true,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = Shoot.procCoefficient,
                        radius = 1f,
                        sniper = false,
                        stopperMask = LayerIndex.world.mask,
                        weapon = null,
                        tracerEffectPrefab = this.tracerPrefab,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                    };
                    bulletAttack.AddModdedDamageType(iDrive.ModdedDamageType);
                    bulletAttack.Fire();
                }
                else
                {
                    BulletAttack bulletAttack = new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = dmg * this.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = iDrive.DamageType,
                        falloffModel = falloffModel,
                        maxDistance = Shoot.range,
                        force = Shoot.force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = isCrit,
                        owner = base.gameObject,
                        muzzleName = "PistolMuzzle",
                        smartCollision = true,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = Shoot.procCoefficient,
                        radius = 1f,
                        sniper = false,
                        stopperMask = LayerIndex.CommonMasks.bullet,
                        weapon = null,
                        tracerEffectPrefab = this.tracerPrefab,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                    };
                    bulletAttack.AddModdedDamageType(iDrive.ModdedDamageType);

                    if (this.iDrive.passive.isPistolOnly || this.iDrive.passive.isBullets || this.iDrive.passive.isRyan)
                    {
                        bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
                        {
                            if (BulletAttack.IsSniperTargetHit(hitInfo))
                            {
                                if (this.iDrive.passive.isPistolOnly) damageInfo.damage *= 2f;
                                else if (this.iDrive.passive.isBullets) damageInfo.damage *= 1.5f;
                                else damageInfo.damage *= 1.25f;
                                damageInfo.damageColorIndex = DamageColorIndex.Sniper;

                                if (wasCharged)
                                {
                                    EffectData effectData = new EffectData
                                    {
                                        origin = hitInfo.point,
                                        rotation = Quaternion.LookRotation(-hitInfo.direction)
                                    };

                                    effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                                    EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Common/VFX/WeakPointProcEffect.prefab").WaitForCompletion(), effectData, true);
                                    Util.PlaySound("sfx_driver_headshot", base.gameObject);
                                    hitInfo.hitHurtBox.healthComponent.gameObject.AddComponent<Modules.Components.DriverHeadshotTracker>();
                                }
                            }
                        };
                    }
                    bulletAttack.Fire();
                }
            }
        }

        public void Fire2(int bulletCount)
        {
            // this is literally the worst POSSIBLE way to do this.
            // FUCK!!

            base.characterBody.AddSpreadBloom(1.15f);
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, "PistolMuzzle", false);

            Util.PlaySound("sfx_driver_pistol_shoot_charged", base.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.AddRecoil2(-1f * Shoot.recoil, -2f * Shoot.recoil, -0.5f * Shoot.recoil, 0.5f * Shoot.recoil);

                float dmg = Shoot.damageCoefficient;

                BulletAttack.FalloffModel falloffModel = BulletAttack.FalloffModel.DefaultBullet;

                if (this.lastCharge)
                {
                    dmg = SteadyAim.damageCoefficient;
                    falloffModel = BulletAttack.FalloffModel.None;
                }

                GameObject tracerPrefab = Shoot.critTracerEffectPrefab;

                BulletAttack bulletAttack = new BulletAttack
                {
                    bulletCount = (uint)bulletCount,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = dmg * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = iDrive.DamageType,
                    falloffModel = falloffModel,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 0f,
                    isCrit = isCrit,
                    owner = base.gameObject,
                    muzzleName = "PistolMuzzle",
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = Shoot.procCoefficient,
                    radius = 1f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = tracerPrefab,
                    spreadPitchScale = 0f,
                    spreadYawScale = 0f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                };
                bulletAttack.AddModdedDamageType(iDrive.ModdedDamageType);

                if (this.iDrive.passive.isPistolOnly || this.iDrive.passive.isBullets || this.iDrive.passive.isRyan)
                {
                    bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
                    {
                        if (BulletAttack.IsSniperTargetHit(hitInfo))
                        {
                            if (this.iDrive.passive.isPistolOnly) damageInfo.damage *= 2f;
                            else damageInfo.damage *= 1.25f;
                            damageInfo.damageColorIndex = DamageColorIndex.Sniper;

                            if (this.lastCharge)
                            {
                                EffectData effectData = new EffectData
                                {
                                    origin = hitInfo.point,
                                    rotation = Quaternion.LookRotation(-hitInfo.direction)
                                };

                                effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                                EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Common/VFX/WeakPointProcEffect.prefab").WaitForCompletion(), effectData, true);
                                Util.PlaySound("sfx_driver_headshot", base.gameObject);
                                hitInfo.hitHurtBox.healthComponent.gameObject.AddComponent<Modules.Components.DriverHeadshotTracker>();
                            }
                        }
                    };
                }
                bulletAttack.Fire();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.lightEffectInstance) Destroy(this.lightEffectInstance);

            if (this.iDrive) this.iDrive.chargeValue = 0f;

            if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            this.PlayExitAnim();
            base.PlayAnimation("AimPitch", "AimPitch");
            if (!this.reloading) this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onInventoryChanged -= Inventory_onInventoryChanged;
            }

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }

            if (!this.cancelling) this.characterBody._defaultCrosshairPrefab = this.iDrive.crosshairPrefab;

            this.FindModelChild("PistolSight").gameObject.SetActive(false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}