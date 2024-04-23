﻿using UnityEngine;
using EntityStates;
using RoR2;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.Compat
{
    public class SlashCombo : BaseMeleeAttack
    {
        public static float _damageCoefficient = 1.8f;
        public static float finisherDamageCoefficient = 3.9f;
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.RefreshState();
            this.hitboxName = "Knife";

            this.damageCoefficient = _damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.1f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 2f / this.attackSpeedStat;

            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.3f;

            this.hitStopDuration = 0.08f;
            this.smoothHitstop = true;

            if (DriverPlugin.ravagerInstalled) this.swingSoundString = "sfx_ravager_swing";
            else this.swingSoundString = "sfx_driver_swing_knife";
            this.swingEffectPrefab = Modules.Assets.redSwingEffect;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.redSlashImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = DamageType.Generic;

            if (this.swingIndex == 0) this.muzzleString = RobDriver.Modules.Config.enabledRedVfxForKnife.Value ? "SwingCenter2" : "SwingCenter";
            else this.muzzleString = this.muzzleString = RobDriver.Modules.Config.enabledRedVfxForKnife.Value ? "SwingCenter" : "SwingCenter2";

            if (this.swingIndex == 2)
            {
                this.muzzleString = "KnifeSwingMuzzle";

                this.duration *= 1.25f;
                this.baseEarlyExitTime = 0.75f;
                this.hitStopDuration *= 2.5f;
                this.attackStartTime = 0.22f;
                this.damageType = DamageType.Stun1s;
                if (DriverPlugin.ravagerInstalled) this.swingSoundString = "sfx_ravager_bigswing";
                else this.swingSoundString = "sfx_driver_swing_hammer";
                this.impactSound = Modules.Assets.hammerImpactSoundDef.index;
                this.damageCoefficient = finisherDamageCoefficient;
            }

            base.OnEnter();

            /*if (this.empowered)
            {
                this.FireAttack();
                this.InitializeAttack();
            }*/
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("SwordPivot").rotation = Util.QuaternionSafeLookRotation(direction);
            }

            base.FireAttack();
        }

        protected override void PlaySwingEffect()
        {
            Util.PlaySound(this.swingSoundString, this.gameObject);
            if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    swingEffectInstance = Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    ScaleParticleSystemDuration fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }
        }

        protected override void TriggerHitStop()
        {
            base.TriggerHitStop();

            if (swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = 20f;
            }
        }

        protected override void ClearHitStop()
        {
            base.ClearHitStop();

            if (swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = fuck.initialDuration;
            }
        }

        protected override void PlayAttackAnimation()
        {
            string animString = "SpinSlash";

            if (this.swingIndex == 0) animString = "Slash1";
            if (this.swingIndex == 1) animString = "Slash2";

            base.PlayCrossfade("Gesture, Override", animString, "Slash.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
            this.FireShuriken();

            int index = this.swingIndex;
            if (index == 0) index = 1;
            else if (index == 1) index = 2;
            else index = 0;

            this.outer.SetNextState(new SlashCombo
            {
                swingIndex = index
            });
        }
    }
}