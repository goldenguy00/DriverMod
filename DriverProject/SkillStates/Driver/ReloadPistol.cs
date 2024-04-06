using EntityStates;
using UnityEngine;
using RoR2;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;

namespace RobDriver.SkillStates.Driver
{
    public class ReloadPistol : BaseDriverSkillState
    {
        public float baseDuration = 2.4f;
        public string animString = "ReloadPistol";
        public InterruptPriority interruptPriority = InterruptPriority.PrioritySkill;
        public bool aiming;

        private bool wasAiming;
        private float duration;
        private bool heheheha;

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.iDrive.passive.isPistolOnly) this.baseDuration *= 0.5f;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.GetModelAnimator().SetFloat("aimBlend", 1f);
            base.PlayCrossfade("Gesture, Override", this.animString, "Action.playbackRate", this.duration, 0.1f);
            Util.PlaySound("sfx_driver_reload_01", this.gameObject);
            this.wasAiming = this.aiming;

            if (NetworkServer.active && this.aiming) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (SteadyAim.camParamsOverrideHandle.isValid && !this.aiming) this.cameraTargetParams.RemoveParamsOverride(SteadyAim.camParamsOverrideHandle);
            if (NetworkServer.active && this.aiming) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.aiming)
            {
                this.characterBody.isSprinting = false;
                this.characterBody.SetAimTimer(1f);
            }

            if (base.isAuthority && this.aiming && !this.inputBank.skill2.down)
            {
                this.aiming = false;
                if (SteadyAim.camParamsOverrideHandle.isValid) this.cameraTargetParams.RemoveParamsOverride(SteadyAim.camParamsOverrideHandle);
                this.GetModelAnimator().SetFloat("aimBlend", 0f);
                if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }

            // early exit
            if (base.fixedAge >= 0.8f * this.duration)
            {
                this.iDrive.FinishReload();

                // the fuck
                if (!this.aiming && this.wasAiming && !this.heheheha)
                {
                    this.heheheha = true;
                    base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.25f);
                }
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                if (this.aiming)
                {
                    this.outer.SetNextState(new SteadyAim
                    {
                        skipAnim = true
                    });
                }
                else this.outer.SetNextStateToMain();

                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return this.interruptPriority;
        }
    }
}