using EntityStates;
using R2API;
using RobDriver.Modules.Components;
using RobDriver.Modules.Misc;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace RobDriver.Modules
{
    internal static class Skills
    {
        internal static List<SkillFamily> skillFamilies = new List<SkillFamily>();
        internal static List<SkillDef> skillDefs = new List<SkillDef>();
        internal static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();

        #region genericskills
        public static void CreateSkillFamilies(GameObject targetPrefab, int families = 15, bool destroyExisting = true) {

            if (destroyExisting) {
                foreach (GenericSkill obj in targetPrefab.GetComponentsInChildren<GenericSkill>()) {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            if (targetPrefab.TryGetComponent<DriverPassive>(out var passive))
            {
                passive.passiveSkillSlot = CreateGenericSkillWithSkillFamily(targetPrefab, "Passive");
            }

            if (targetPrefab.TryGetComponent<DriverArsenal>(out var arsenal))
            {
                arsenal.weaponSkillSlot = CreateGenericSkillWithSkillFamily(targetPrefab, "Arsenal");
            }

            if ((families & (1 << 0)) != 0) {
                skillLocator.primary = CreateGenericSkillWithSkillFamily(targetPrefab, "Primary");
            }
            if ((families & (1 << 1)) != 0) {
                skillLocator.secondary = CreateGenericSkillWithSkillFamily(targetPrefab, "Secondary");
            }
            if ((families & (1 << 2)) != 0) {
                skillLocator.utility = CreateGenericSkillWithSkillFamily(targetPrefab, "Utility");
            }
            if ((families & (1 << 3)) != 0) {
                skillLocator.special = CreateGenericSkillWithSkillFamily(targetPrefab, "Special");
            }
        }

        public static GenericSkill CreateGenericSkillWithSkillFamily(GameObject targetPrefab, string familyName, bool hidden = false) {

            GenericSkill skill = targetPrefab.AddComponent<GenericSkill>();
            skill.skillName = familyName;
            skill.hideInCharacterSelect = hidden;

            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = targetPrefab.name + familyName + "Family";
            newFamily.variants = new SkillFamily.Variant[0];

            skill._skillFamily = newFamily;

            skillFamilies.Add(newFamily);
            return skill;
        }
        #endregion

        #region skillfamilies

        //everything calls this
        public static void AddSkillToFamily(SkillFamily skillFamily, SkillDef skillDef, UnlockableDef unlockableDef = null) {

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant {
                skillDef = skillDef,
                unlockableDef = unlockableDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        public static void AddSkillsToFamily(SkillFamily skillFamily, params SkillDef[] skillDefs) 
        {
            foreach (SkillDef skillDef in skillDefs)
            {
                AddSkillToFamily(skillFamily, skillDef);
            }
        }
        public static void AddPrimarySkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().primary.skillFamily, skillDefs);
        }
        public static void AddSecondarySkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().secondary.skillFamily, skillDefs);
        }
        public static void AddUtilitySkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().utility.skillFamily, skillDefs);
        }
        public static void AddSpecialSkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().special.skillFamily, skillDefs);
        }
        public static void AddPassiveSkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            AddSkillsToFamily(targetPrefab.GetComponent<DriverPassive>().passiveSkillSlot.skillFamily, skillDefs);
        }
        /// <summary>
        /// Adds a group of weapons to the default weapon skill family
        /// </summary>
        /// <param name="locked">If true, weapons will need to be randomly encountered before they are selectable</param>
        public static void AddWeaponSkills(GameObject targetPrefab, IEnumerable<DriverWeaponDef> weaponDefs, bool locked)
        {
            var family = targetPrefab.GetComponent<DriverArsenal>().weaponSkillSlot.skillFamily;
            foreach (DriverWeaponDef weapon in weaponDefs)
            {
                AddSkillToFamily(family, CreateWeaponSkillDef(weapon), locked ? CreateUnlockableDef(weapon) : null);
            }
        }
        /// <summary>
        /// Adds a single weapon to the default weapon skill family
        /// </summary>
        /// <param name="locked">If true, weapon will need to be randomly encountered before they are selectable</param>
        public static void AddWeaponSkill(GameObject targetPrefab, DriverWeaponDef weaponDef, bool locked)
        {
            AddSkillToFamily(targetPrefab.GetComponent<DriverArsenal>().weaponSkillSlot.skillFamily, 
                CreateWeaponSkillDef(weaponDef), locked ? CreateUnlockableDef(weaponDef) : null);
        }

        /// <summary>
        /// pass in an amount of unlockables equal to or less than skill variants, null for skills that aren't locked
        /// <code>
        /// AddUnlockablesToFamily(skillLocator.primary, null, skill2UnlockableDef, null, skill4UnlockableDef);
        /// </code>
        /// </summary>
        public static void AddUnlockablesToFamily(SkillFamily skillFamily, params UnlockableDef[] unlockableDefs) {

            for (int i = 0; i < unlockableDefs.Length; i++) 
            {
                skillFamily.variants[i].unlockableDef = unlockableDefs[i];
            }
        }

        #endregion

        #region skilldefs
        public static SkillDef CreateSkillDef(SkillDefInfo skillDefInfo) {

            return CreateSkillDef<SkillDef>(skillDefInfo);
        }

        public static T CreateSkillDef<T>(SkillDefInfo skillDefInfo) where T : SkillDef {

            T skillDef = ScriptableObject.CreateInstance<T>();

            popuplateSKillDef(skillDefInfo, skillDef);

            skillDefs.Add(skillDef);

            return skillDef;
        }

        public static AwesomeSauceSkillDef CreateAwesomeSkillDef(SkillDefInfo skillDefInfo)
        {
            return CreateAwesomeSkillDef<AwesomeSauceSkillDef>(skillDefInfo);
        }

        public static T CreateAwesomeSkillDef<T>(SkillDefInfo skillDefInfo) where T : AwesomeSauceSkillDef
        {
            T skillDef = ScriptableObject.CreateInstance<T>();

            popuplateSKillDef(skillDefInfo, skillDef);

            skillDefs.Add(skillDef);

            return skillDef;
        }

        private static void popuplateSKillDef(SkillDefInfo skillDefInfo, SkillDef skillDef) {
            skillDef.skillName = skillDefInfo.skillName;
            (skillDef as ScriptableObject).name = skillDefInfo.skillName;
            skillDef.skillNameToken = skillDefInfo.skillNameToken;
            skillDef.skillDescriptionToken = skillDefInfo.skillDescriptionToken;
            skillDef.icon = skillDefInfo.skillIcon;

            skillDef.activationState = skillDefInfo.activationState;
            skillDef.activationStateMachineName = skillDefInfo.activationStateMachineName;
            skillDef.baseMaxStock = skillDefInfo.baseMaxStock;
            skillDef.baseRechargeInterval = skillDefInfo.baseRechargeInterval;
            skillDef.beginSkillCooldownOnSkillEnd = skillDefInfo.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = skillDefInfo.canceledFromSprinting;
            skillDef.forceSprintDuringState = skillDefInfo.forceSprintDuringState;
            skillDef.fullRestockOnAssign = skillDefInfo.fullRestockOnAssign;
            skillDef.interruptPriority = skillDefInfo.interruptPriority;
            skillDef.resetCooldownTimerOnUse = skillDefInfo.resetCooldownTimerOnUse;
            skillDef.isCombatSkill = skillDefInfo.isCombatSkill;
            skillDef.mustKeyPress = skillDefInfo.mustKeyPress;
            skillDef.cancelSprintingOnActivation = skillDefInfo.cancelSprintingOnActivation;
            skillDef.rechargeStock = skillDefInfo.rechargeStock;
            skillDef.requiredStock = skillDefInfo.requiredStock;
            skillDef.stockToConsume = skillDefInfo.stockToConsume;

            skillDef.keywordTokens = skillDefInfo.keywordTokens;
        }

        internal static SkillDef CreatePrimarySkillDef(SerializableEntityStateType state, string stateMachine, string skillNameToken, string skillDescriptionToken, Sprite skillIcon, bool agile)
        {
            SkillDefInfo info = new SkillDefInfo(skillNameToken, skillNameToken, skillDescriptionToken, skillIcon, state, stateMachine, agile);

            return CreateSkillDef(info);
        }

        internal static SkillDef CreateWeaponSkillDef(string skillNameToken, string skillDescriptionToken, Sprite skillIcon)
        {
            return CreateSkillDef(new SkillDefInfo(
                skillName: skillNameToken,
                skillNameToken: skillNameToken,
                skillDescriptionToken: skillDescriptionToken,
                skillIcon: skillIcon,
                activationState: new SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName: "",
                interruptPriority: InterruptPriority.Any,
                isCombatSkill: false,
                baseRechargeInterval: 0));
        }

        internal static SkillDef CreateWeaponSkillDef(DriverWeaponDef weaponDef)
        {
            return CreateWeaponSkillDef(weaponDef.nameToken, weaponDef.descriptionToken, Sprite.Create(weaponDef.icon
                as Texture2D, new Rect(0, 0, weaponDef.icon.width, weaponDef.icon.height), new Vector2(0.5f, 0.5f)));

        }

        /// <summary>
        /// Creates an unlockable def for the weapon. By default, picking up a weapon will grant this unlock.
        /// </summary>
        internal static UnlockableDef CreateUnlockableDef(DriverWeaponDef weaponDef)
        {
            var unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.cachedName = weaponDef.nameToken;
            unlockableDef.nameToken = weaponDef.nameToken;
            unlockableDef.getHowToUnlockString = () => Language.GetString(weaponDef.descriptionToken);
            unlockableDef.getUnlockedString = () => Language.GetString(weaponDef.descriptionToken);
            unlockableDef.hidden = false;
            unlockableDef.achievementIcon = Sprite.Create(weaponDef.icon as Texture2D,
                new Rect(0, 0, weaponDef.icon.width, weaponDef.icon.height), new Vector2(0.5f, 0.5f));

            unlockableDefs.Add(unlockableDef);
            return unlockableDef;
        }

        #endregion skilldefs
    }
}

    /// <summary>
    /// class for easily creating skilldefs with default values, and with a field for UnlockableDef
    /// </summary>
internal class SkillDefInfo
{
        public string skillName;
        public string skillNameToken;
        public string skillDescriptionToken;
        public string[] keywordTokens = new string[0];
        public Sprite skillIcon;

        public SerializableEntityStateType activationState;
        public InterruptPriority interruptPriority;
        public string activationStateMachineName;

        public float baseRechargeInterval;

        public int baseMaxStock = 1;
        public int rechargeStock = 1;
        public int requiredStock = 1;
        public int stockToConsume = 1;

        public bool isCombatSkill = true;
        public bool canceledFromSprinting;
        public bool forceSprintDuringState;
        public bool cancelSprintingOnActivation = true;

        public bool beginSkillCooldownOnSkillEnd;
        public bool fullRestockOnAssign = true;
        public bool resetCooldownTimerOnUse;
        public bool mustKeyPress;

        #region building
        public SkillDefInfo() { }

        public SkillDefInfo(string skillName,
                              string skillNameToken,
                              string skillDescriptionToken,
                              Sprite skillIcon,

                              SerializableEntityStateType activationState,
                              string activationStateMachineName,
                              InterruptPriority interruptPriority,
                              bool isCombatSkill,

                              float baseRechargeInterval) {
            this.skillName = skillName;
            this.skillNameToken = skillNameToken;
            this.skillDescriptionToken = skillDescriptionToken;
            this.skillIcon = skillIcon;
            this.activationState = activationState;
            this.activationStateMachineName = activationStateMachineName;
            this.interruptPriority = interruptPriority;
            this.isCombatSkill = isCombatSkill;
            this.baseRechargeInterval = baseRechargeInterval;
        }
        /// <summary>
        /// Creates a skilldef for a typical primary.
        /// <para>combat skill, cooldown: 0, required stock: 0, InterruptPriority: Any</para>
        /// </summary>
        public SkillDefInfo(string skillName,
                              string skillNameToken,
                              string skillDescriptionToken,
                              Sprite skillIcon,

                              SerializableEntityStateType activationState,
                              string activationStateMachineName = "Weapon",
                              bool agile = false) {

            this.skillName = skillName;
            this.skillNameToken = skillNameToken;
            this.skillDescriptionToken = skillDescriptionToken;
            this.skillIcon = skillIcon;

            this.activationState = activationState;
            this.activationStateMachineName = activationStateMachineName;

            this.interruptPriority = InterruptPriority.Any;
            this.isCombatSkill = true;
            this.baseRechargeInterval = 0;

            this.requiredStock = 0;
            this.stockToConsume = 0;

            this.cancelSprintingOnActivation = !agile;

            if (agile) this.keywordTokens = new string[] { "KEYWORD_AGILE" };
        }
        #endregion construction complete
}