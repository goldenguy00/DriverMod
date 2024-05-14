using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class HunkProjectileTracker : MonoBehaviour
    {
        private void OnEnable()
        {
            DriverPlugin.projectileList.Add(this);
        }

        private void OnDisable()
        {
            DriverPlugin.projectileList.Add(this);
        }
    }
}