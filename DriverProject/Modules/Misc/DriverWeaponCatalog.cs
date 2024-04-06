using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RobDriver
{
    public static class DriverWeaponCatalog
    {
        public static Dictionary<string, DriverWeaponDef> weaponDrops = new Dictionary<string, DriverWeaponDef>();
        public static DriverWeaponDef[] weaponDefs = new DriverWeaponDef[0];

        internal static DriverWeaponDef Pistol;
        internal static DriverWeaponDef PyriteGun;
        internal static DriverWeaponDef GoldenGun;
        internal static DriverWeaponDef PrototypeRocketLauncher;
        internal static DriverWeaponDef ArmCannon;
        internal static DriverWeaponDef PlasmaCannon;
        internal static DriverWeaponDef Behemoth;
        internal static DriverWeaponDef BeetleShield;
        internal static DriverWeaponDef LunarPistol;
        internal static DriverWeaponDef VoidPistol;
        internal static DriverWeaponDef Needler;
        internal static DriverWeaponDef GolemRifle;
        internal static DriverWeaponDef LunarRifle;
        internal static DriverWeaponDef LunarHammer;
        internal static DriverWeaponDef NemmandoGun;
        internal static DriverWeaponDef NemmercGun;

        public static void AddWeapon(DriverWeaponDef weaponDef)
        {
            Array.Resize(ref weaponDefs, weaponDefs.Length + 1);

            int index = weaponDefs.Length - 1;
            weaponDef.index = (ushort)index;

            weaponDefs[index] = weaponDef;
            weaponDef.index = (ushort)index;

            // heheheha
            weaponDef.pickupPrefab = Modules.Assets.CreatePickupObject(weaponDef);

            // set default icon
            if (!weaponDef.icon)
            {
                switch (weaponDef.tier)
                {
                    case DriverWeaponTier.Common:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponGrey");
                        break;
                    case DriverWeaponTier.Uncommon:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponGreen");
                        break;
                    case DriverWeaponTier.Legendary:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponRed");
                        break;
                    case DriverWeaponTier.Unique:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponYellow");
                        break;
                    case DriverWeaponTier.Lunar:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponBlue");
                        break;
                    case DriverWeaponTier.Void:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponPurple");
                        break;
                }
            }

            // add config
            Modules.Config.InitWeaponConfig(weaponDef);
            Debug.Log("Added " + weaponDef.nameToken + " to catalog with index: " + weaponDef.index);
        }

        public static bool IsWeaponPistol(DriverWeaponDef weaponDef)
        {
            return weaponDef == Pistol || weaponDef == LunarPistol || weaponDef == VoidPistol;
        }

        public static void AddWeaponDrop(string bodyName, DriverWeaponDef weaponDef, bool autoComplete = true)
        {
            if (autoComplete)
            {
                if (!bodyName.Contains("Body")) bodyName += "Body";
                if (!bodyName.Contains("(Clone)")) bodyName += "(Clone)";
            }

            weaponDrops.Add(bodyName, weaponDef);
        }

        public static DriverWeaponDef GetWeaponFromIndex(int index)
        {
            return weaponDefs[index];
        }

        public static DriverWeaponDef GetRandomWeapon()
        {
            List<DriverWeaponDef> validWeapons = new List<DriverWeaponDef>();

            for (int i = 0; i < weaponDefs.Length; i++)
            {
                if (Modules.Config.GetWeaponConfigEnabled(weaponDefs[i]) && weaponDefs[i].shotCount > 0) validWeapons.Add(weaponDefs[i]);
            }

            DriverWeaponDef[] _validWeapons = validWeapons.ToArray();

            if (_validWeapons.Length <= 0) return weaponDefs[0]; // pistol failsafe

            return _validWeapons[UnityEngine.Random.Range(0, _validWeapons.Length)];
        }

        public static DriverWeaponDef GetRandomWeaponFromTier(DriverWeaponTier tier)
        {
            List<DriverWeaponDef> validWeapons = new List<DriverWeaponDef>();

            for (int i = 0; i < weaponDefs.Length; i++)
            {
                if (weaponDefs[i] && weaponDefs[i].tier == tier)
                {
                    if (Modules.Config.GetWeaponConfigEnabled(weaponDefs[i])) validWeapons.Add(weaponDefs[i]);
                }
            }

            DriverWeaponDef[] _validWeapons = validWeapons.ToArray();

            if (_validWeapons.Length <= 0) return weaponDefs[0]; // pistol failsafe if you disabled rocket launcher like a fucking retard or something

            return _validWeapons[UnityEngine.Random.Range(0, _validWeapons.Length)];
        }
    }
}