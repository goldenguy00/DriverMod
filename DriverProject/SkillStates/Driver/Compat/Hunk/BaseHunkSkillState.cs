using RobDriver.Modules.Components;

namespace RobDriver.SkillStates.Driver.Compat.Hunk
{
    public class BaseHunkSkillState : BaseDriverSkillState
    {
        protected HunkController hunk;

        public override void OnEnter()
        {
            hunk = GetComponent<HunkController>();
            base.OnEnter();
        }
    }
}