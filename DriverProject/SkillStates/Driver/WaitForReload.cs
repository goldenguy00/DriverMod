namespace RobDriver.SkillStates.Driver
{
    public class WaitForReload : BaseDriverSkillState
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // i love nesting if statements instead of writing proper decision trees :)
            if (base.isAuthority)
            {
                if (this.iDrive.weaponTimer == this.iDrive.maxWeaponTimer || this.iDrive.passive.isBullets || this.iDrive.passive.isRyan)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }

                if (base.fixedAge >= 2f)
                {
                    if (this.iDrive.weaponTimer <= 0f)
                    {
                        this.outer.SetNextState(new ReloadPistol());
                    }
                    else
                    {
                        this.outer.SetNextState(new ReloadPistol
                        {
                            interruptPriority = EntityStates.InterruptPriority.Any
                        });
                    }
                }
            }
        }
    }
}