using EntityStates;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver
{
    public class Shoot : BaseDriverSkillState
    {
        public static float damageCoefficient = 2.2f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.7f;
        public static float force = 200f;
        public static float recoil = 2f;
        public static float range = 2000f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");
        public static GameObject critTracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCaptainShotgun");

        private float duration;
        private float fireTime;
        private float fireTime2;
        private bool hasFired;
        private bool hasFired2;
        private string muzzleString;
        private bool isCrit;
        private GameObject effectInstance;
        private uint spinPlayID;
        private bool oldShoot;

        protected virtual float _damageCoefficient => Shoot.damageCoefficient;
        protected virtual GameObject tracerPrefab => this.isCrit ? Shoot.critTracerEffectPrefab : Shoot.tracerEffectPrefab;
        public virtual string shootSoundString => this.isCrit ? "sfx_driver_pistol_shoot_critical" : "sfx_driver_pistol_shoot";
        public virtual BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.DefaultBullet;
        protected virtual float baseCritDuration => 0.9f;
        protected virtual float baseCritDuration2 => 1.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.characterBody.isSprinting = false;
            this.oldShoot = Modules.Config.oldCritShot.Value;

            this.fireTime = 0.1f * this.duration;
            this.fireTime2 = 0.2f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "PistolMuzzle";

            this.isCrit = base.RollCrit();

            if (this.isCrit)
            {
                if (this.oldShoot)
                {
                    this.duration = this.baseCritDuration / this.attackSpeedStat;
                    this.fireTime = 0.5f * this.duration;
                    this.fireTime2 = 0.55f * this.duration;

                    this.effectInstance = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoReloadFX.prefab").WaitForCompletion());
                    this.effectInstance.transform.parent = this.FindModelChild("Pistol");
                    this.effectInstance.transform.localRotation = Quaternion.Euler(new Vector3(0f, 80f, 0f));
                    this.effectInstance.transform.localPosition = Vector3.zero;

                    this.spinPlayID = Util.PlaySound("sfx_driver_pistol_spin", this.gameObject);

                    this.PlayAnimation("Gesture, Override", "ShootCritical", "Shoot.playbackRate", this.duration);
                }
                else
                {
                    this.duration = this.baseCritDuration2 / this.attackSpeedStat;
                    this.fireTime = 0f * this.duration;
                    this.fireTime2 = 0.05f * this.duration;

                    this.PlayAnimation("Gesture, Override", "ShootCriticalAlt", "Shoot.playbackRate", this.duration);
                }
            }
            else
            {
                this.hasFired = true;
                this.Fire();

                this.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", this.duration * 1.5f);
            }

            if (this.iDrive.maxWeaponTimer > 0) this.iDrive.ConsumeAmmo(1f, true);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.spinPlayID != 0u) AkSoundEngine.StopPlayingID(this.spinPlayID);
            if (this.effectInstance) EntityState.Destroy(this.effectInstance);
        }

        private void Fire()
        {
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, this.gameObject, this.muzzleString, false);
            Util.PlaySound(this.shootSoundString, this.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.AddRecoil2(-1f * Shoot.recoil, -2f * Shoot.recoil, -0.5f * Shoot.recoil, 0.5f * Shoot.recoil);

                BulletAttack bulletAttack = new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = this._damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = iDrive.DamageType,
                    falloffModel = this.falloff,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = this.characterBody.spreadBloomAngle * 2f,
                    isCrit = this.isCrit,
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.75f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = this.tracerPrefab,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                };
                bulletAttack.AddModdedDamageType(iDrive.ModdedDamageType);
                bulletAttack.Fire();
            }

            base.characterBody.AddSpreadBloom(1.25f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.iDrive && this.iDrive.weaponDef.nameToken != this.cachedWeaponDef.nameToken)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge >= this.fireTime && base.isAuthority)
            {
                if (!this.hasFired)
                {
                    this.hasFired = true;
                    this.Fire();
                }
            }

            if (this.isCrit && base.fixedAge >= this.fireTime2 && base.isAuthority)
            {
                if (!this.hasFired2)
                {
                    this.hasFired2 = true;
                    this.Fire();
                }
            }

            if (this.oldShoot)
            {
                if (this.effectInstance && base.fixedAge >= (0.4f * this.duration))
                {
                    EntityState.Destroy(this.effectInstance);

                    AkSoundEngine.StopPlayingID(this.spinPlayID);
                    this.spinPlayID = 0u;
                    Util.PlaySound("sfx_driver_pistol_ready", this.gameObject);
                }
            }
            else
            {
                if (this.isCrit && !this.effectInstance && base.fixedAge >= (0.55f * this.duration))
                {
                    this.effectInstance = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoReloadFX.prefab").WaitForCompletion());
                    this.effectInstance.transform.parent = this.FindModelChild("Pistol");
                    this.effectInstance.transform.localRotation = Quaternion.Euler(new Vector3(0f, 80f, 0f));
                    this.effectInstance.transform.localPosition = Vector3.zero;

                    this.spinPlayID = Util.PlaySound("sfx_driver_pistol_spin", this.gameObject);
                }
            }

            // pyrite gun made me do this
            if (this.iDrive.weaponTimer <= 0f && this.iDrive.maxWeaponTimer > 0 &&
                this.GetMinimumInterruptPriority() == InterruptPriority.Any && base.isAuthority)
            {
                this.outer.SetNextState(new ReloadPistol());
                return;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextState(new WaitForReload());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            float kek = 0.5f;
            if (this.isCrit && this.oldShoot) kek = 0.75f;

            if (base.fixedAge >= kek * this.duration)
            {
                return InterruptPriority.Any;
            }

            if (this.isCrit && !this.hasFired2) return InterruptPriority.PrioritySkill;

            return InterruptPriority.Skill;
        }
    }
}