using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using RoR2.CharacterAI;
using RoR2.Navigation;
using RoR2.Orbs;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2.UI;
using System.Linq;
using RobDriver.Modules.Components;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.UI;

namespace RobDriver.Modules.Survivors
{
    internal class Driver
    {
        internal static Driver instance;

        internal static GameObject characterPrefab;
        internal static GameObject displayPrefab;

        internal static GameObject umbraMaster;

        internal static ConfigEntry<bool> forceUnlock;
        internal static ConfigEntry<bool> characterEnabled;

        internal float pityMultiplier = 1f;

        public static Color characterColor = new Color(145f / 255f, 0f, 1f);

        public const string bodyName = "RobDriverBody";

        public static int bodyRendererIndex; // use this to store the rendererinfo index containing our character's body
                                             // keep it last in the rendererinfos because teleporter particles for some reason require this. hopoo pls

        // item display stuffs
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        internal static UnlockableDef characterUnlockableDef;
        internal static UnlockableDef masteryUnlockableDef;
        internal static UnlockableDef grandMasteryUnlockableDef;
        internal static UnlockableDef suitUnlockableDef;

        internal static UnlockableDef supplyDropUnlockableDef;
        internal static UnlockableDef pistolPassiveUnlockableDef;
        internal static UnlockableDef godslingPassiveUnlockableDef;

        // skill overrides
        internal static SkillDef lunarPistolPrimarySkillDef;
        internal static SkillDef lunarPistolSecondarySkillDef;

        internal static SkillDef voidPistolPrimarySkillDef;
        internal static SkillDef voidPistolSecondarySkillDef;

        internal static SkillDef goldenGunPrimarySkillDef;
        internal static SkillDef goldenGunSecondarySkillDef;

        internal static SkillDef pyriteGunPrimarySkillDef;
        internal static SkillDef pyriteGunSecondarySkillDef;

        internal static SkillDef shotgunPrimarySkillDef;
        internal static SkillDef shotgunSecondarySkillDef;

        internal static SkillDef riotShotgunPrimarySkillDef;
        internal static SkillDef riotShotgunSecondarySkillDef;

        internal static SkillDef slugShotgunPrimarySkillDef;
        internal static SkillDef slugShotgunSecondarySkillDef;

        internal static SkillDef machineGunPrimarySkillDef;
        internal static SkillDef machineGunSecondarySkillDef;

        internal static SkillDef heavyMachineGunPrimarySkillDef;
        internal static SkillDef heavyMachineGunSecondarySkillDef;

        internal static SkillDef bazookaPrimarySkillDef;
        internal static SkillDef bazookaSecondarySkillDef;

        internal static SkillDef rocketLauncherPrimarySkillDef;
        internal static SkillDef rocketLauncherSecondarySkillDef;

        internal static SkillDef rocketLauncherAltPrimarySkillDef;
        internal static SkillDef rocketLauncherAltSecondarySkillDef;

        internal static SkillDef armCannonPrimarySkillDef;
        internal static SkillDef armCannonSecondarySkillDef;

        internal static SkillDef plasmaCannonPrimarySkillDef;
        internal static SkillDef plasmaCannonSecondarySkillDef;

        internal static SkillDef sniperPrimarySkillDef;
        internal static SkillDef sniperSecondarySkillDef;

        internal static SkillDef beetleShieldPrimarySkillDef;
        internal static SkillDef beetleShieldSecondarySkillDef;

        internal static SkillDef behemothPrimarySkillDef;
        internal static SkillDef behemothSecondarySkillDef;

        internal static SkillDef grenadeLauncherPrimarySkillDef;
        internal static SkillDef grenadeLauncherSecondarySkillDef;

        internal static SkillDef badassShotgunPrimarySkillDef;
        internal static SkillDef badassShotgunSecondarySkillDef;

        internal static SkillDef lunarRiflePrimarySkillDef;
        internal static SkillDef lunarRifleSecondarySkillDef;

        internal static SkillDef lunarHammerPrimarySkillDef;
        internal static SkillDef lunarHammerSecondarySkillDef;

        internal static SkillDef nemmandoGunPrimarySkillDef;
        internal static SkillDef nemmandoGunSecondarySkillDef;

        internal static SkillDef nemmercGunPrimarySkillDef;
        internal static SkillDef nemmercGunSecondarySkillDef;

        internal static SkillDef golemGunPrimarySkillDef;
        internal static SkillDef golemGunSecondarySkillDef;

        internal static SkillDef pistolReloadSkillDef;

        internal static SkillDef confirmSkillDef;
        internal static SkillDef cancelSkillDef;

        internal static SkillDef skateboardSkillDef;
        internal static SkillDef skateCancelSkillDef;

        internal static SkillDef scepterGrenadeSkillDef;
        internal static SkillDef scepterSupplyDropSkillDef;
        internal static SkillDef scepterSupplyDropLegacySkillDef;
        internal static SkillDef scepterSyringeSkillDef;
        internal static SkillDef scepterSyringeLegacySkillDef;
        internal static SkillDef scepterKnifeSkillDef;

        internal static string bodyNameToken;

        internal void CreateCharacter()
        {
            instance = this;

            characterEnabled = Modules.Config.CharacterEnableConfig("Driver");

            if (characterEnabled.Value)
            {
                forceUnlock = Modules.Config.ForceUnlockConfig("Driver");

                masteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.MasteryAchievement>();
                grandMasteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.GrandMasteryAchievement>();
                suitUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.SuitAchievement>();

                supplyDropUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.SupplyDropAchievement>();
                pistolPassiveUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.DriverPistolPassiveAchievement>();
                godslingPassiveUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.DriverGodslingPassiveAchievement>();

                if (!forceUnlock.Value) characterUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.DriverUnlockAchievement>();

                characterPrefab = CreateBodyPrefab(true);

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab("DriverDisplay", characterPrefab);

                if (forceUnlock.Value) Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "DRIVER");
                else Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "DRIVER", characterUnlockableDef);

                umbraMaster = CreateMaster(characterPrefab, "RobDriverMonsterMaster");
            }

            Hook();
        }

        private static GameObject CreateBodyPrefab(bool isPlayer)
        {
            bodyNameToken = DriverPlugin.developerPrefix + "_DRIVER_BODY_NAME";

            #region Body
            GameObject newPrefab = Modules.Prefabs.CreatePrefab("RobDriverBody", "mdlDriver", new BodyInfo
            {
                armor = Config.baseArmor.Value,
                armorGrowth = Config.armorGrowth.Value,
                bodyName = "RobDriverBody",
                bodyNameToken = bodyNameToken,
                bodyColor = characterColor,
                characterPortrait = Modules.Assets.LoadCharacterIcon("Driver"),
                crosshair = Modules.Assets.LoadCrosshair("Standard"),
                damage = Config.baseDamage.Value,
                healthGrowth = Config.healthGrowth.Value,
                healthRegen = Config.baseRegen.Value,
                jumpCount = 1,
                maxHealth = Config.baseHealth.Value,
                subtitleNameToken = DriverPlugin.developerPrefix + "_DRIVER_BODY_SUBTITLE",
                podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
                moveSpeed = Config.baseMovementSpeed.Value,
                acceleration = 60f,
                jumpPower = 15f,
                attackSpeed = 1f,
                crit = Config.baseCrit.Value
            });

            ChildLocator childLocator = newPrefab.GetComponentInChildren<ChildLocator>();

            childLocator.gameObject.AddComponent<Modules.Components.DriverAnimationEvents>();

            //CharacterBody body = newPrefab.GetComponent<CharacterBody>();
            //body.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(SpawnState));
            //body.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage;
            //body.bodyFlags |= CharacterBody.BodyFlags.SprintAnyDirection;
            //body.sprintingSpeedMultiplier = 1.75f;

            //newPrefab.AddComponent<NinjaMod.Modules.Components.NinjaController>();

            //SfxLocator sfx = newPrefab.GetComponent<SfxLocator>();
            //sfx.barkSound = "";
            //sfx.landingSound = "";
            //sfx.deathSound = "";
            //sfx.fallDamageSound = "";

            //FootstepHandler footstep = newPrefab.GetComponentInChildren<FootstepHandler>();
            //footstep.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericHugeFootstepDust");
            //footstep.baseFootstepString = "Play_moonBrother_step";
            //footstep.sprintFootstepOverrideString = "Play_moonBrother_sprint";

            //KinematicCharacterMotor characterController = newPrefab.GetComponent<KinematicCharacterMotor>();
            //characterController.CapsuleRadius = 4f;
            //characterController.CapsuleHeight = 9f;

            //CharacterDirection direction = newPrefab.GetComponent<CharacterDirection>();
            //direction.turnSpeed = 135f;

            //Interactor interactor = newPrefab.GetComponent<Interactor>();
            //interactor.maxInteractionDistance = 8f;

            newPrefab.GetComponent<CameraTargetParams>().cameraParams = Modules.CameraParams.CreateCameraParamsWithData(DriverCameraParams.DEFAULT);

            //newPrefab.GetComponent<CharacterDirection>().turnSpeed = 720f;

            newPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MainState));

            // this is for the lunar shard skill..
            EntityStateMachine stateMachine = newPrefab.AddComponent<EntityStateMachine>();
            stateMachine.customName = "Shard";
            stateMachine.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            stateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));

            //var state = isPlayer ? typeof(EntityStates.SpawnTeleporterState) : typeof(SpawnState);
            //newPrefab.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType(state);

            // schizophrenia
            newPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FuckMyAss));

            newPrefab.AddComponent<Modules.Components.DriverController>();
            #endregion

            #region Model
            Material mainMat = Modules.Assets.CreateMaterial("matDriver", 1f, Color.white);

            Material clothMat = Modules.Assets.CreateMaterial("matSlugger", 1f, Color.white);
            Material tieMat = Modules.Assets.CreateMaterial("matSuit", 1f, Color.white);

            bodyRendererIndex = 0;

            Modules.Prefabs.SetupCharacterModel(newPrefab, new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = mainMat
                },
                new CustomRendererInfo
                {
                    childName = "KnifeModel",
                    material = Modules.Assets.knifeMat
                },
                new CustomRendererInfo
                {
                    childName = "ButtonModel",
                    material = Modules.Assets.CreateMaterial("matButton")
                },
                new CustomRendererInfo
                {
                    childName = "SyringeModel",
                    material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Syringe/matSyringe.mat").WaitForCompletion()
                },
                new CustomRendererInfo
                {
                    childName = "MedkitModel",
                    material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Medkit/matMedkit.mat").WaitForCompletion()
                },
                new CustomRendererInfo
                {
                    childName = "SluggerClothModelL",
                    material = clothMat
                },
                new CustomRendererInfo
                {
                    childName = "SluggerClothModelR",
                    material = clothMat
                },
                new CustomRendererInfo
                {
                    childName = "TieModel",
                    material = tieMat
                },
                new CustomRendererInfo
                {
                    childName = "SkateboardModel",
                    material = Assets.skateboardMat
                },
                new CustomRendererInfo
                {
                    childName = "SkateboardBackModel",
                    material = Assets.skateboardMat
                },
                new CustomRendererInfo
                {
                    childName = "PistolModel",
                    material = Modules.Assets.pistolMat
                } }, bodyRendererIndex);

            // hide the extra stuff
            childLocator.FindChild("KnifeModel").gameObject.SetActive(false);
            childLocator.FindChild("ButtonModel").gameObject.SetActive(false);
            childLocator.FindChild("SyringeModel").gameObject.SetActive(false);
            childLocator.FindChild("MedkitModel").gameObject.SetActive(false);
            childLocator.FindChild("SluggerCloth").gameObject.SetActive(false);
            childLocator.FindChild("Tie").gameObject.SetActive(false);
            childLocator.FindChild("SkateboardModel").gameObject.SetActive(false);
            childLocator.FindChild("SkateboardBackModel").gameObject.SetActive(false);
            #endregion

            CreateHitboxes(newPrefab);
            SetupHurtboxes(newPrefab);
            CreateSkills(newPrefab);
            CreateSkins(newPrefab);
            InitializeItemDisplays(newPrefab);

            return newPrefab;
        }

        private static void SetupHurtboxes(GameObject bodyPrefab)
        {
            HurtBoxGroup hurtboxGroup = bodyPrefab.GetComponentInChildren<HurtBoxGroup>();
            List<HurtBox> hurtboxes = new List<HurtBox>();

            hurtboxes.Add(bodyPrefab.GetComponentInChildren<ChildLocator>().FindChild("MainHurtbox").GetComponent<HurtBox>());

            HealthComponent healthComponent = bodyPrefab.GetComponent<HealthComponent>();

            foreach (Collider i in bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponentsInChildren<Collider>())
            {
                if (i.gameObject.name != "MainHurtbox")
                {
                    HurtBox hurtbox = i.gameObject.AddComponent<HurtBox>();
                    hurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
                    hurtbox.healthComponent = healthComponent;
                    hurtbox.isBullseye = false;
                    hurtbox.damageModifier = HurtBox.DamageModifier.Normal;
                    hurtbox.hurtBoxGroup = hurtboxGroup;

                    hurtboxes.Add(hurtbox);
                }
            }

            hurtboxGroup.hurtBoxes = hurtboxes.ToArray();
        }

        private static GameObject CreateMaster(GameObject bodyPrefab, string masterName)
        {
            GameObject newMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/LemurianMaster"), masterName, true);
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;

            #region AI
            foreach (AISkillDriver ai in newMaster.GetComponentsInChildren<AISkillDriver>())
            {
                DriverPlugin.DestroyImmediate(ai);
            }

            newMaster.GetComponent<BaseAI>().fullVision = true;

            AISkillDriver revengeDriver = newMaster.AddComponent<AISkillDriver>();
            revengeDriver.customName = "Revenge";
            revengeDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            revengeDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            revengeDriver.activationRequiresAimConfirmation = true;
            revengeDriver.activationRequiresTargetLoS = false;
            revengeDriver.selectionRequiresTargetLoS = true;
            revengeDriver.maxDistance = 24f;
            revengeDriver.minDistance = 0f;
            revengeDriver.requireSkillReady = true;
            revengeDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            revengeDriver.ignoreNodeGraph = true;
            revengeDriver.moveInputScale = 1f;
            revengeDriver.driverUpdateTimerOverride = 2.5f;
            revengeDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            revengeDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            revengeDriver.maxTargetHealthFraction = Mathf.Infinity;
            revengeDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            revengeDriver.maxUserHealthFraction = 0.5f;
            revengeDriver.skillSlot = SkillSlot.Utility;

            AISkillDriver grabDriver = newMaster.AddComponent<AISkillDriver>();
            grabDriver.customName = "Grab";
            grabDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            grabDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            grabDriver.activationRequiresAimConfirmation = true;
            grabDriver.activationRequiresTargetLoS = false;
            grabDriver.selectionRequiresTargetLoS = true;
            grabDriver.maxDistance = 8f;
            grabDriver.minDistance = 0f;
            grabDriver.requireSkillReady = true;
            grabDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            grabDriver.ignoreNodeGraph = true;
            grabDriver.moveInputScale = 1f;
            grabDriver.driverUpdateTimerOverride = 0.5f;
            grabDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            grabDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            grabDriver.maxTargetHealthFraction = Mathf.Infinity;
            grabDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            grabDriver.maxUserHealthFraction = Mathf.Infinity;
            grabDriver.skillSlot = SkillSlot.Primary;

            AISkillDriver stompDriver = newMaster.AddComponent<AISkillDriver>();
            stompDriver.customName = "Stomp";
            stompDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            stompDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            stompDriver.activationRequiresAimConfirmation = true;
            stompDriver.activationRequiresTargetLoS = false;
            stompDriver.selectionRequiresTargetLoS = true;
            stompDriver.maxDistance = 32f;
            stompDriver.minDistance = 0f;
            stompDriver.requireSkillReady = true;
            stompDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            stompDriver.ignoreNodeGraph = true;
            stompDriver.moveInputScale = 0.4f;
            stompDriver.driverUpdateTimerOverride = 0.5f;
            stompDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            stompDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            stompDriver.maxTargetHealthFraction = Mathf.Infinity;
            stompDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            stompDriver.maxUserHealthFraction = Mathf.Infinity;
            stompDriver.skillSlot = SkillSlot.Secondary;

            AISkillDriver followCloseDriver = newMaster.AddComponent<AISkillDriver>();
            followCloseDriver.customName = "ChaseClose";
            followCloseDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            followCloseDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            followCloseDriver.activationRequiresAimConfirmation = false;
            followCloseDriver.activationRequiresTargetLoS = false;
            followCloseDriver.maxDistance = 32f;
            followCloseDriver.minDistance = 0f;
            followCloseDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            followCloseDriver.ignoreNodeGraph = false;
            followCloseDriver.moveInputScale = 1f;
            followCloseDriver.driverUpdateTimerOverride = -1f;
            followCloseDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            followCloseDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            followCloseDriver.maxTargetHealthFraction = Mathf.Infinity;
            followCloseDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            followCloseDriver.maxUserHealthFraction = Mathf.Infinity;
            followCloseDriver.skillSlot = SkillSlot.None;

            AISkillDriver followDriver = newMaster.AddComponent<AISkillDriver>();
            followDriver.customName = "Chase";
            followDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            followDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            followDriver.activationRequiresAimConfirmation = false;
            followDriver.activationRequiresTargetLoS = false;
            followDriver.maxDistance = Mathf.Infinity;
            followDriver.minDistance = 0f;
            followDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            followDriver.ignoreNodeGraph = false;
            followDriver.moveInputScale = 1f;
            followDriver.driverUpdateTimerOverride = -1f;
            followDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            followDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            followDriver.maxTargetHealthFraction = Mathf.Infinity;
            followDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            followDriver.maxUserHealthFraction = Mathf.Infinity;
            followDriver.skillSlot = SkillSlot.None;
            followDriver.shouldSprint = true;
            #endregion

            Modules.Prefabs.masterPrefabs.Add(newMaster);

            return newMaster;
        }

        private static void CreateHitboxes(GameObject prefab)
        {
            ChildLocator childLocator = prefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform hitboxTransform = childLocator.FindChild("HammerBaseHitbox");
            Transform hitboxTransform2 = childLocator.FindChild("HammerHitbox");
            Modules.Prefabs.SetupHitbox(model, new Transform[]
                {
                    hitboxTransform,
                    hitboxTransform2
                }, "Hammer");

            hitboxTransform = childLocator.FindChild("KnifeHitbox");
            Modules.Prefabs.SetupHitbox(model, new Transform[]
                {
                    hitboxTransform
                }, "Knife");
        }

        private static void CreateSkills(GameObject prefab)
        {
            DriverPassive passive = prefab.AddComponent<DriverPassive>();
            Modules.Skills.CreateSkillFamilies(prefab);

            string prefix = DriverPlugin.developerPrefix;
            SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;
            //skillLocator.passiveSkill.skillNameToken = prefix + "_DRIVER_BODY_PASSIVE_NAME";
            //skillLocator.passiveSkill.skillDescriptionToken = prefix + "_DRIVER_BODY_PASSIVE_DESCRIPTION";
            //skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassiveIcon");

            Driver.pistolReloadSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_PRIMARY_RELOAD_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_PRIMARY_RELOAD_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_RELOAD_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.ReloadPistol)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.confirmSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_CONFIRM_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_CONFIRM_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_CONFIRM_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "fuck",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });

            Driver.cancelSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_CANCEL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_CANCEL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_CANCEL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCancelIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "fuck",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });


            #region Passive
            passive.defaultPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_PASSIVE_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_PASSIVE_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_PASSIVE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassiveIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            passive.bulletsPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_PASSIVE3_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_PASSIVE3_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_PASSIVE3_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLeadfootIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            passive.godslingPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_PASSIVE4_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_PASSIVE4_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_PASSIVE4_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAltPassiveIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            passive.pistolOnlyPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_PASSIVE2_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_PASSIVE2_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_PASSIVE2_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAltPassiveLegacyIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            if (Modules.Config.cursed.Value)
            {
                Modules.Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, new SkillDef[]{
                    passive.defaultPassive,
                    passive.bulletsPassive,
                    passive.godslingPassive,
                    passive.pistolOnlyPassive,
                });

                Modules.Skills.AddUnlockablesToFamily(passive.passiveSkillSlot.skillFamily,
                null, pistolPassiveUnlockableDef, godslingPassiveUnlockableDef, pistolPassiveUnlockableDef);
            }
            else
            {
                Modules.Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, new SkillDef[]{
                    passive.defaultPassive,
                    passive.bulletsPassive,
                    passive.godslingPassive
                });

                Modules.Skills.AddUnlockablesToFamily(passive.passiveSkillSlot.skillFamily,
                null, pistolPassiveUnlockableDef, godslingPassiveUnlockableDef);
            }
            #endregion

            #region Primary
            Modules.Skills.AddPrimarySkills(prefab,
                Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shoot)),
                "Weapon", 
                prefix + "_DRIVER_BODY_PRIMARY_PISTOL_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_PISTOL_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false));

            Driver.goldenGunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.GoldenGun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_GOLDENGUN_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_GOLDENGUN_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunIcon"), false);

            Driver.pyriteGunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.PyriteGun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_PYRITE_PISTOL_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_PYRITE_PISTOL_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunIcon"), false);

            Driver.beetleShieldPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.BeetleShield.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_PISTOL_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_PISTOL_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false);

            Driver.lunarPistolPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarPistol.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_LUNAR_PISTOL_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_LUNAR_PISTOL_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false);

            Driver.voidPistolPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.VoidPistol.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_VOID_PISTOL_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_VOID_PISTOL_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false);

            Driver.shotgunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_SHOTGUN_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_SHOTGUN_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunIcon"),
                false);

            Driver.riotShotgunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RiotShotgun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_RIOT_SHOTGUN_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_RIOT_SHOTGUN_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRiotShotgunIcon"),
                false);

            Driver.slugShotgunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SlugShotgun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_SLUG_SHOTGUN_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_SLUG_SHOTGUN_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlugShotgunIcon"),
                false);

            Driver.machineGunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MachineGun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_MACHINEGUN_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_MACHINEGUN_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMachineGunIcon"),
                false);

            Driver.heavyMachineGunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.HeavyMachineGun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_HEAVY_MACHINEGUN_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_HEAVY_MACHINEGUN_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMachineGunIcon"),
                false);

            Driver.bazookaPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Bazooka.Charge)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_BAZOOKA_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_BAZOOKA_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Driver.rocketLauncherPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_ROCKETLAUNCHER_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_ROCKETLAUNCHER_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Driver.behemothPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_ROCKETLAUNCHER_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_ROCKETLAUNCHER_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Driver.rocketLauncherAltPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.NerfedShoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_ROCKETLAUNCHER_ALT_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_ROCKETLAUNCHER_ALT_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Driver.grenadeLauncherPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.GrenadeLauncher.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_GRENADELAUNCHER_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_GRENADELAUNCHER_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Driver.armCannonPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.ArmCannon.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_ARMCANNON_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_ARMCANNON_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texArmCannonIcon"),
                false);

            Driver.plasmaCannonPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.PlasmaCannon.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_PLASMACANNON_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_PLASMACANNON_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPlasmaCannonIcon"),
                false);

            Driver.sniperPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SniperRifle.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_SNIPER_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_SNIPER_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlugShotgunIcon"),
                false);

            Driver.badassShotgunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.BadassShotgun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_BADASS_SHOTGUN_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_BADASS_SHOTGUN_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunIcon"),
                false);

            Driver.lunarRiflePrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarRifle.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_LUNARRIFLE_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_LUNARRIFLE_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarRifleIcon"),
                false);

            Driver.golemGunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.GolemGun.ChargeLaser)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_LUNARRIFLE_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_LUNARRIFLE_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGolemGunIcon"),
                false);

            Driver.lunarHammerPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarHammer.SwingCombo)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_LUNARHAMMER_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_LUNARHAMMER_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarHammerIcon"),
                false);

            Driver.nemmandoGunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Compat.NemmandoGun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_NEMMANDO_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_NEMMANDO_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoPrimaryIcon"),
                false);

            Driver.nemmercGunPrimarySkillDef = Modules.Skills.CreatePrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Compat.NemmercGun.Shoot)),
                "Weapon",
                prefix + "_DRIVER_BODY_PRIMARY_NEMMERC_NAME",
                prefix + "_DRIVER_BODY_PRIMARY_NEMMERC_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmercPrimaryIcon"),
                false);
            #endregion

            #region Secondary
            SkillDef steadyAimSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Driver.beetleShieldSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_BEETLESHIELD_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_BEETLESHIELD_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_BEETLESHIELD_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.BeetleShield.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Driver.pyriteGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_PYRITE_PISTOL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_PYRITE_PISTOL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_PYRITE_PISTOL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.PyriteGun.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Driver.lunarPistolSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_LUNAR_PISTOL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_LUNAR_PISTOL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_LUNAR_PISTOL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarPistol.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Driver.voidPistolSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_VOID_PISTOL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_VOID_PISTOL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_VOID_PISTOL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.VoidPistol.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Driver.goldenGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_GOLDENGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_GOLDENGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_GOLDENGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.GoldenGun.AimLightsOut)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.shotgunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.riotShotgunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.slugShotgunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.grenadeLauncherSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.machineGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texZapIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MachineGun.Zap)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.heavyMachineGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_HEAVY_MACHINEGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_HEAVY_MACHINEGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_HEAVY_MACHINEGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texHeavyMachineGunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.HeavyMachineGun.ShootGrenade)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.rocketLauncherSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.Barrage)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.behemothSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.Barrage)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.rocketLauncherAltSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_ALT_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_ALT_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_ROCKETLAUNCHER_ALT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.NerfedBarrage)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.bazookaSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.sniperSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SNIPER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SNIPER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SNIPER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SniperRifle.Aim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Driver.plasmaCannonSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_PLASMACANNON_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_PLASMACANNON_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_PLASMACANNON_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.PlasmaCannon.Barrage)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.armCannonSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.badassShotgunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.lunarRifleSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.lunarHammerSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_LUNARHAMMER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_LUNARHAMMER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_LUNARHAMMER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarShardIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarHammer.FireShard)),
                activationStateMachineName = "Shard",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Driver.nemmandoGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_NEMMANDO_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_NEMMANDO_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_NEMMANDO_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Compat.NemmandoGun.Submission)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.nemmercGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Driver.golemGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Modules.Skills.AddSecondarySkills(prefab, steadyAimSkillDef/*, pissSkillDef*/);
            #endregion

            #region Utility
            SkillDef slideSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_UTILITY_SLIDE_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_UTILITY_SLIDE_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_UTILITY_SLIDE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlideIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Slide)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            skateboardSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkateboardIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Skateboard.Start)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0.5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            skateCancelSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD2_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCancelIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Skateboard.Stop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0.5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            SkillDef dashSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_UTILITY_DASH_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_UTILITY_DASH_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_UTILITY_DASH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texDashIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Dash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddUtilitySkills(prefab, slideSkillDef, dashSkillDef, skateboardSkillDef);
            #endregion

            #region Special
            SkillDef stunGrenadeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texStunGrenadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.ThrowGrenade)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            scepterGrenadeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_SCEPTER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_SCEPTER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texStunGrenadeScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.ThrowMolotov)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            SkillDef knifeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_KNIFE_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_KNIFE_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_KNIFE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SwingKnife)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 7f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            scepterKnifeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_KNIFE_SCEPTER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_KNIFE_SCEPTER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_KNIFE_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SwingKnifeScepter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 3.5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            SkillDef supplyDropSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SupplyDrop.Nerfed.AimCrapDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 24f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0
            });

            scepterSupplyDropSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_SCEPTER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_SCEPTER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SupplyDrop.Scepter.AimVoidDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 24f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0
            });

            SkillDef supplyDropLegacySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_LEGACY_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_LEGACY_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_LEGACY_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropLegacyIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SupplyDrop.AimSupplyDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0
            });

            scepterSupplyDropLegacySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_LEGACY_SCEPTER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_LEGACY_SCEPTER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_LEGACY_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropLegacyScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SupplyDrop.Scepter.AimVoidDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 1
            });

            SkillDef healSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_HEAL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_HEAL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_HEAL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texStunGrenadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Heal)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 24f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            SkillDef syringeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SYRINGE_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SYRINGE_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SYRINGE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSyringeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.UseSyringe)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            scepterSyringeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SYRINGE_SCEPTER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SYRINGE_SCEPTER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SYRINGE_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSyringeScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.UseSyringeScepter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            SkillDef syringeLegacySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SYRINGELEGACY_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SYRINGELEGACY_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SYRINGELEGACY_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSyringeLegacyIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.UseSyringeLegacy)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            scepterSyringeLegacySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SYRINGELEGACY_SCEPTER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SYRINGELEGACY_SCEPTER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SYRINGELEGACY_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSyringeLegacyScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.UseSyringeScepter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            if (Modules.Config.cursed.Value)
            {
                Modules.Skills.AddSpecialSkills(prefab, stunGrenadeSkillDef, supplyDropSkillDef, supplyDropLegacySkillDef, knifeSkillDef, /*healSkillDef,*/ syringeSkillDef, syringeLegacySkillDef);
                Modules.Skills.AddUnlockablesToFamily(skillLocator.special.skillFamily, null, supplyDropUnlockableDef, supplyDropUnlockableDef);
            }
            else
            {
                Modules.Skills.AddSpecialSkills(prefab, stunGrenadeSkillDef, supplyDropSkillDef, knifeSkillDef, /*healSkillDef,*/ syringeSkillDef);
                Modules.Skills.AddUnlockablesToFamily(skillLocator.special.skillFamily, null, supplyDropUnlockableDef);
            }
            #endregion

            if (DriverPlugin.scepterInstalled) InitializeScepterSkills();

            Modules.Assets.InitWeaponDefs();
        }

        private static void InitializeScepterSkills()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterGrenadeSkillDef, bodyName, SkillSlot.Special, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSupplyDropSkillDef, bodyName, SkillSlot.Special, 1);

            if (Modules.Config.cursed.Value)
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSupplyDropLegacySkillDef, bodyName, SkillSlot.Special, 2);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterKnifeSkillDef, bodyName, SkillSlot.Special, 3);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSyringeSkillDef, bodyName, SkillSlot.Special, 4);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSyringeLegacySkillDef, bodyName, SkillSlot.Special, 5);
            }
            else
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterKnifeSkillDef, bodyName, SkillSlot.Special, 2);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSyringeSkillDef, bodyName, SkillSlot.Special, 3);
            }
        }

        private static void CreateSkins(GameObject prefab)
        {
            GameObject model = prefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            GameObject sluggerCloth = childLocator.FindChild("SluggerCloth").gameObject;
            GameObject tie = childLocator.FindChild("Tie").gameObject;

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshDriver")
                }
            };

            defaultSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = false
                }
            };

            skins.Add(defaultSkin);
            #endregion

            #region MasterySkin
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_MONSOON_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMonsoonSkin"),
                SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matJacket", 1f, Color.white)
                }),
                mainRenderer,
                model,
                masteryUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshJacket")
                }
            };

            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = false
                }
            };

            skins.Add(masterySkin);
            #endregion

            #region GrandMasterySkin
            SkinDef grandMasterySkin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_TYPHOON_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texTyphoonSkin"),
                SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matSlugger", 1f, Color.white)
                }),
                mainRenderer,
                model,
                grandMasteryUnlockableDef);

            grandMasterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSlugger")
                }
            };

            grandMasterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = true
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = false
                }
            };

            skins.Add(grandMasterySkin);
            #endregion

            #region SpecialForcesSkin
            SkinDef specialForcesSkin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_SPECIALFORCES_SKIN_NAME",
    Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialForcesSkin"),
    SkinRendererInfos(defaultRenderers, new Material[]
    {
                    Modules.Assets.CreateMaterial("matSpecialForces", 1f, Color.white)
    }),
    mainRenderer,
    model);

            specialForcesSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSpecialForces")
                }
            };

            specialForcesSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = false
                }
            };

            skins.Add(specialForcesSkin);
            #endregion

            #region GuerrillaSkin
            SkinDef guerrillaSkin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_GUERRILLA_SKIN_NAME",
    Assets.mainAssetBundle.LoadAsset<Sprite>("texGuerrillaSkin"),
    SkinRendererInfos(defaultRenderers, new Material[]
    {
                    Modules.Assets.CreateMaterial("matGuerrilla", 1f, Color.white)
    }),
    mainRenderer,
    model);

            guerrillaSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshGuerrilla")
                }
            };

            guerrillaSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = false
                }
            };

            skins.Add(guerrillaSkin);
            #endregion

            #region SuitSkin
            SkinDef suitSkin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_SUIT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texSuitSkin"),
                SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matSuit", 1f, Color.white)
                }),
                mainRenderer,
                model,
                suitUnlockableDef);

            suitSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSuit")
                }
            };

            suitSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = true
                }
            };

            skins.Add(suitSkin);
            #endregion

            #region Suit2Skin
            SkinDef suit2Skin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_SUIT2_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texSuit2Skin"),
                SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matSuit", 1f, Color.white)
                }),
                mainRenderer,
                model,
                suitUnlockableDef);

            suit2Skin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSuit2")
                }
            };

            suit2Skin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = true
                }
            };

            if (Modules.Config.cursed.Value) skins.Add(suit2Skin);
            #endregion

            #region GreenSkin
            SkinDef greenSkin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_GREEN_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGreenSkin"),
                SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matDriverGreen", 1f, Color.white)
                }),
                mainRenderer,
                model);

            greenSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshDriver")
                }
            };

            greenSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = false
                }
            };

            if (Modules.Config.cursed.Value) skins.Add(greenSkin);
            #endregion

            #region MinecraftSkin
            if (Modules.Config.cursed.Value)
            {
                SkinDef minecraftSkin = Modules.Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_MINECRAFT_SKIN_NAME",
    Assets.mainAssetBundle.LoadAsset<Sprite>("texMinecraftSkin"),
    SkinRendererInfos(defaultRenderers, new Material[]
    {
                    Modules.Assets.CreateMaterial("matMinecraftDriver", 1f, Color.white)
    }),
    mainRenderer,
    model);

                minecraftSkin.meshReplacements = new SkinDef.MeshReplacement[]
                {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshMinecraftDriver")
                }
                };

                minecraftSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
{
                new SkinDef.GameObjectActivation
                {
                    gameObject = sluggerCloth,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = tie,
                    shouldActivate = false
                }
};


                skins.Add(minecraftSkin);
            }
            #endregion

            skinController.skins = skins.ToArray();
        }

        private static void InitializeItemDisplays(GameObject prefab)
        {
            CharacterModel characterModel = prefab.GetComponentInChildren<CharacterModel>();

            if (itemDisplayRuleSet == null)
            {
                itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
                itemDisplayRuleSet.name = "idrs" + bodyName;
            }

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
            characterModel.itemDisplayRuleSet.keyAssetRuleGroups = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.keyAssetRuleGroups;// itemDisplayRuleSet;
            itemDisplayRules = itemDisplayRuleSet.keyAssetRuleGroups.ToList();
        }

        internal static void SetItemDisplays()
        {
            // uhh
            Modules.ItemDisplays.PopulateDisplays();

            ReplaceItemDisplay(RoR2Content.Items.SecondarySkillMagazine, new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                    limbMask = LimbFlags.None,
childName = "GunR",
localPos = new Vector3(0.00888F, -0.03648F, -0.20898F),
localAngles = new Vector3(39.35415F, 348.9445F, 164.0792F),
localScale = new Vector3(0.06F, 0.06F, 0.06F)
                }
            });

            ReplaceItemDisplay(RoR2Content.Items.CritGlasses, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.0006F, 0.25054F, 0.04672F),
localAngles = new Vector3(314.7648F, 358.1459F, 0.48047F),
localScale = new Vector3(0.30902F, 0.09537F, 0.30934F)
                }
});

            if (Modules.Config.predatoryOnHead.Value)
            {
                ReplaceItemDisplay(RoR2Content.Items.AttackSpeedOnCrit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.18766F, -0.11041F),
localAngles = new Vector3(302.566F, 0F, 0F),
localScale = new Vector3(0.47332F, 0.47332F, 0.47332F)
                }
});
            }
            else
            {
                ReplaceItemDisplay(RoR2Content.Items.AttackSpeedOnCrit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                    limbMask = LimbFlags.None,
childName = "UpperArmR",
localPos = new Vector3(-0.01092F, 0.02048F, -0.00403F),
localAngles = new Vector3(309.4066F, 250.1116F, 175.7708F),
localScale = new Vector3(0.363F, 0.363F, 0.363F)
                }
});
            }

            ReplaceItemDisplay(DLC1Content.Items.CritGlassesVoid, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayGlassesVoid"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.1555F, 0.11598F),
localAngles = new Vector3(340.0668F, 0F, 0F),
localScale = new Vector3(0.30387F, 0.39468F, 0.46147F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.LunarSun, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHeadNeck"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(-0.02605F, 0.38179F, -0.0112F),
localAngles = new Vector3(-0.00001F, 262.1551F, 0.00001F),
localScale = new Vector3(1.76594F, 1.84475F, 1.84475F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.LimbMask,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHead"),
                    limbMask = LimbFlags.Head,
childName = "Head",
localPos = new Vector3(0F, 0.10143F, -0.01147F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHead"),
                    limbMask = LimbFlags.Head,
childName = "Head",
localPos = new Vector3(0F, 0.10143F, -0.01147F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.GhostOnKill, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.0029F, 0.15924F, 0.07032F),
localAngles = new Vector3(355.7367F, 0.15F, 0F),
localScale = new Vector3(0.6F, 0.6F, 0.6F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.GoldOnHit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.15159F, -0.0146F),
localAngles = new Vector3(8.52676F, 0F, 0F),
localScale = new Vector3(0.90509F, 0.90509F, 0.90509F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.JumpBoost, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, -0.228F, -0.108F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.79857F, 0.79857F, 0.79857F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.KillEliteFrenzy, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.12823F, 0.035F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.17982F, 0.17982F, 0.17982F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.LunarPrimaryReplacement, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.18736F, 0.08896F),
localAngles = new Vector3(306.9798F, 180F, 180F),
localScale = new Vector3(0.31302F, 0.31302F, 0.31302F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.FragileDamageBonus, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayDelicateWatch"),
                    limbMask = LimbFlags.None,
childName = "HandL",
localPos = new Vector3(0.001145094f, -0.01941454f, 0.001435831f),
localAngles = new Vector3(84.24088f, 213.1651f, 131.5774f),
localScale = new Vector3(0.5f, 0.5f, 0.5f)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.BarrierOnOverHeal, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
                    limbMask = LimbFlags.None,
childName = "LowerArmL",
localPos = new Vector3(0.01781F, 0.11702F, 0.01516F),
localAngles = new Vector3(90F, 270F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.SprintArmor, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
                    limbMask = LimbFlags.None,
childName = "LowerArmL",
localPos = new Vector3(-0.012F, 0.171F, -0.027F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.ArmorPlate, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
                    limbMask = LimbFlags.None,
childName = "CalfR",
localPos = new Vector3(-0.02573F, 0.22602F, 0.0361F),
localAngles = new Vector3(90F, 180F, 0F),
localScale = new Vector3(-0.2958F, 0.2958F, 0.29581F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.CritDamage, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserSight"),
                    limbMask = LimbFlags.None,
childName = "Pistol",
localPos = new Vector3(-0.01876F, 0.26245F, 0.11694F),
localAngles = new Vector3(0F, 0F, 270F),
localScale = new Vector3(0.05261F, 0.05261F, 0.05261F)
                }
});

            if (DriverPlugin.litInstalled) SetLITDisplays();

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            //itemDisplayRuleSet.GenerateRuntimeValues();
        }

        internal static void SetLITDisplays()
        {
            return;
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = LostInTransit.LITContent.Items.Lopper,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLopper"),
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0F, 0.20282F, -0.19089F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.19059F, 0.19059F, 0.19059F)
                        }
        }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = LostInTransit.LITContent.Items.Chestplate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBackPlate"),
                            limbMask = LimbFlags.None,
             childName = "Chest",
localPos = new Vector3(0F, 0.23366F, 0.01011F),
localAngles = new Vector3(349.1311F, 0F, 0F),
localScale = new Vector3(0.13457F, 0.19557F, 0.19557F)
                        }
}
                }
            });
        }

        internal static void ReplaceItemDisplay(Object keyAsset, ItemDisplayRule[] newDisplayRules)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup[] cock = itemDisplayRules.ToArray();
            for (int i = 0; i < cock.Length; i++)
            {
                if (cock[i].keyAsset == keyAsset)
                {
                    // replace the item display rule
                    cock[i].displayRuleGroup.rules = newDisplayRules;
                }
            }
            itemDisplayRules = cock.ToList();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];

            return newRendererInfos;
        }

        private static void Hook()
        {
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;

            RoR2.UI.HUD.onHudTargetChangedGlobal += HUDSetup;

            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;
            On.RoR2.SkillLocator.ResetSkills += SkillLocator_ResetSkills;

            // heresy anims
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += PlayVisionsAnimation;
            On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.PlayChargeAnimation += PlayChargeLunarAnimation;
            On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.PlayThrowAnimation += PlayThrowLunarAnimation;
            On.EntityStates.GlobalSkills.LunarDetonator.Detonate.OnEnter += PlayRuinAnimation;

            // dazed debuff
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.EntityStates.AI.BaseAIState.AimAt += BaseAIState_AimAt;
            On.EntityStates.AI.BaseAIState.AimInDirection += BaseAIState_AimInDirection;
        }

        private static void BaseAIState_AimInDirection(On.EntityStates.AI.BaseAIState.orig_AimInDirection orig, EntityStates.AI.BaseAIState self, ref BaseAI.BodyInputs dest, Vector3 aimDirection)
        {
            if (self.body && self.body.HasBuff(Modules.Buffs.dazedDebuff))
            {
                orig(self, ref dest, Random.onUnitSphere);
                dest.desiredAimDirection = Random.onUnitSphere;
            }
            else orig(self, ref dest, aimDirection);
        }

        private static void BaseAIState_AimAt(On.EntityStates.AI.BaseAIState.orig_AimAt orig, EntityStates.AI.BaseAIState self, ref BaseAI.BodyInputs dest, BaseAI.Target aimTarget)
        {
            if (self.body && self.body.HasBuff(Modules.Buffs.dazedDebuff))
            {
                orig(self, ref dest, aimTarget);
                dest.desiredAimDirection = Random.onUnitSphere;
            }
            else orig(self, ref dest, aimTarget);
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.inflictor)
            {
                if (damageInfo.inflictor.name.Contains("RobDriverStunGrenade"))
                {
                    if (self)
                    {
                        if (self.body)
                        {
                            self.body.AddTimedBuff(Modules.Buffs.dazedDebuff, 10f);
                        }
                    }
                }
            }

            if (damageInfo.damageType == DamageType.ApplyMercExpose)
            {
                if (damageInfo.attacker && damageInfo.attacker.name.Contains("RobDriverBody"))
                {
                    damageInfo.damageType = DamageType.Stun1s;

                    if (self)
                    {
                        if (self.body) self.body.AddTimedBuff(Modules.Buffs.woundDebuff, 4f);

                        NetworkIdentity identity = self.gameObject.GetComponent<NetworkIdentity>();
                        if (identity)
                        {
                            new SyncOverlay(identity.netId, self.gameObject).Send(NetworkDestination.Clients);
                        }
                    }
                }
            }
            
            orig(self, damageInfo);
        }

        private static void SkillLocator_ApplyAmmoPack(On.RoR2.SkillLocator.orig_ApplyAmmoPack orig, SkillLocator self)
        {
            orig(self);

            // this is terribly hardcoded and not future proof
            // but more performant than doing something like a getcomponent every time a bandolier drop is picked up on anyone
            // this will break if an alternate primary is added but that'll never happen with the weapon system existing
            if (self && self.primary.baseSkill.skillNameToken == DriverPlugin.developerPrefix + "_DRIVER_BODY_PRIMARY_PISTOL_NAME")
            {
                Components.DriverController iDrive = self.GetComponent<Components.DriverController>();
                if (iDrive)
                {
                    iDrive.ServerResetTimer();
                }
            }
        }

        private static void SkillLocator_ResetSkills(On.RoR2.SkillLocator.orig_ResetSkills orig, SkillLocator self)
        {
            orig(self);

            if (self && self.primary.baseSkill.skillNameToken == DriverPlugin.developerPrefix + "_DRIVER_BODY_PRIMARY_PISTOL_NAME")
            {
                Components.DriverController iDrive = self.GetComponent<Components.DriverController>();
                if (iDrive)
                {
                    iDrive.ServerResetTimer();
                }
            }
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.attackerBody && damageReport.attackerMaster && damageReport.victim)
            {
                bool isDriverOnPlayerTeam = false;
                foreach (CharacterBody i in CharacterBody.readOnlyInstancesList)
                {
                    if (i && i.teamComponent && i.teamComponent.teamIndex == TeamIndex.Player && i.baseNameToken == Driver.bodyNameToken)
                    {
                        isDriverOnPlayerTeam = true;
                        break;
                    }
                }

                // weapon drops
                bool bonusChance = false;
                bool godSlingChance = false;
                if (isDriverOnPlayerTeam)
                {
                    // headshot first
                    if (damageReport.attackerBody.baseNameToken == Driver.bodyNameToken)
                    {
                        if (damageReport.victim.GetComponent<DriverHeadshotTracker>())
                        {
                            NetworkIdentity identity = damageReport.victim.gameObject.GetComponent<NetworkIdentity>();
                            if (identity)
                            {
                                new SyncDecapitation(identity.netId, damageReport.victim.gameObject).Send(NetworkDestination.Clients);
                            }
                        }
                        //Log.debug("chance checking");
                        if (damageReport.attackerBody.GetComponent<DriverController>().passive.isBullets) bonusChance = true;
                        if(damageReport.attackerBody.GetComponent<DriverController>().passive.isRyan) godSlingChance = true;
                    }

                    // 7
                    float chance = Modules.Config.baseDropRate.Value;
                    if (bonusChance) chance += Mathf.Clamp((DamageTypes.bulletTypes.Distinct().Count() - 30), 0, 30);
                    bool fuckMyAss = chance >= 100f;

                    // higher chance if it's a big guy
                    if (damageReport.victimBody.hullClassification == HullClassification.Golem) chance = Mathf.Clamp(1.1f * chance, 0f, 100f);

                    // minimum 25% chance if the slain enemy is an elite
                    if (damageReport.victimBody.isElite) chance = Mathf.Clamp(chance, 25f, 100f);

                    // halved on swarms, fuck You
                    if (Run.instance && RoR2.RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Swarms)) chance *= 0.5f;

                    chance *= Driver.instance.pityMultiplier;

                    bool droppedWeapon = Util.CheckRoll(chance, damageReport.attackerMaster);

                    // guaranteed if the slain enemy is a boss
                    bool isBoss = damageReport.victimBody.isChampion || damageReport.victimIsChampion;

                    // simulacrum boss wave fix
                    if ((damageReport.victimBody.isBoss || damageReport.victimIsBoss) && !InfiniteTowerRun.instance) isBoss = true;

                    // terminal enemies from starstorm's relic of termination
                    if (DriverPlugin.CheckIfBodyIsTerminal(damageReport.victimBody)) isBoss = true;

                    if (isBoss || fuckMyAss) droppedWeapon = true;

                    // all the above checks were originally checking the ATTACKER body
                    // not the fucking victim
                    // how

                    // stop dropping weapons when void monsters kill each other plz this is an annoying bug
                    if (damageReport.attackerTeamIndex != TeamIndex.Player) droppedWeapon = false;

                    if (DriverWeaponCatalog.weaponDrops.ContainsKey(damageReport.victimBody.gameObject.name))
                    {
                        DriverWeaponDef z = DriverWeaponCatalog.weaponDrops[damageReport.victimBody.gameObject.name];
                        if (z.dropChance >= 100f) droppedWeapon = true;
                    }

                    if (droppedWeapon)
                    {
                        Driver.instance.pityMultiplier = 0.8f;

                        Vector3 position = Vector3.zero;
                        Transform transform = damageReport.victim.transform;
                        if (transform)
                        {
                            position = damageReport.victim.transform.position;
                        }

                        //if (Modules.Config.oldPickupModel.Value) pickupPrefab = Modules.Assets.weaponPickupOld;

                        DriverWeaponTier weaponTier = DriverWeaponTier.Uncommon;
                        if (damageReport.victimBody.isChampion) weaponTier = DriverWeaponTier.Legendary;

                        DriverWeaponDef weaponDef = DriverWeaponCatalog.GetRandomWeaponFromTier(weaponTier);

                        if (DriverWeaponCatalog.weaponDrops.ContainsKey(damageReport.victimBody.gameObject.name))
                        {
                            DriverWeaponDef newWeapon = DriverWeaponCatalog.weaponDrops[damageReport.victimBody.gameObject.name];
                            if (Util.CheckRoll(newWeapon.dropChance)) weaponDef = newWeapon;
                        }

                        if(godSlingChance)
                        {
                            //Log.debug("godslingchanceactivated");
                            GameObject weaponPickup = weaponDef.pickupPrefab;
                            float splitChance = Modules.Config.godslingDropRateSplit.Value;
                            System.Random rnd = new System.Random();
                            float num = rnd.Next(0, 100);
                            if (num >= splitChance)
                            {
                                weaponPickup.GetComponentInChildren<Modules.Components.WeaponPickup>().isAmmoBox = true;
                                //Log.debug("AMMOOOOOOO");
                            }
                            else
                            {
                                weaponPickup.GetComponentInChildren<Modules.Components.WeaponPickup>().isAmmoBox = false;
                            }
                            weaponPickup = UnityEngine.Object.Instantiate<GameObject>(weaponPickup, position, UnityEngine.Random.rotation);

                            TeamFilter teamFilter = weaponPickup.GetComponent<TeamFilter>();
                            if (teamFilter) teamFilter.teamIndex = damageReport.attackerTeamIndex;

                            NetworkServer.Spawn(weaponPickup);
                        }
                        else
                        {
                            GameObject weaponPickup = UnityEngine.Object.Instantiate<GameObject>(weaponDef.pickupPrefab, position, UnityEngine.Random.rotation);

                            TeamFilter teamFilter = weaponPickup.GetComponent<TeamFilter>();
                            if (teamFilter) teamFilter.teamIndex = damageReport.attackerTeamIndex;

                            NetworkServer.Spawn(weaponPickup);
                        }
                    }
                    else
                    {
                        // add pity
                        Driver.instance.pityMultiplier += 0.025f;
                    }
                }

                // combo extension would be huge but i need to network it and that's annoying
                /*if (damageReport.attackerBody.baseNameToken == Driver.bodyNameToken)
                {
                    // combo extension
                    Components.DriverController iDrive = damageReport.attackerBody.gameObject.GetComponent<Components.DriverController>();
                    if (iDrive) iDrive.ExtendTimer();
                }*/
            }
        }

        internal static void HUDSetup(RoR2.UI.HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster && hud.targetMaster.bodyPrefab == Driver.characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;

                if (DriverPlugin.riskUIInstalled)
                {
                    RiskUIHudSetup(hud);
                    return;
                }

                Transform skillsContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomRightCluster").Find("Scaler");

                // no one will notice these missing
                skillsContainer.Find("SprintCluster").gameObject.SetActive(false);
                skillsContainer.Find("InventoryCluster").gameObject.SetActive(false);

                GameObject weaponSlot = GameObject.Instantiate(skillsContainer.Find("EquipmentSlot").gameObject, skillsContainer);
                weaponSlot.name = "WeaponSlot";

                EquipmentIcon equipmentIconComponent = weaponSlot.GetComponent<EquipmentIcon>();
                Components.WeaponIcon weaponIconComponent = weaponSlot.AddComponent<Components.WeaponIcon>();

                weaponIconComponent.iconImage = equipmentIconComponent.iconImage;
                weaponIconComponent.displayRoot = equipmentIconComponent.displayRoot;
                weaponIconComponent.flashPanelObject = equipmentIconComponent.stockFlashPanelObject;
                weaponIconComponent.reminderFlashPanelObject = equipmentIconComponent.reminderFlashPanelObject;
                weaponIconComponent.isReadyPanelObject = equipmentIconComponent.isReadyPanelObject;
                weaponIconComponent.tooltipProvider = equipmentIconComponent.tooltipProvider;
                weaponIconComponent.targetHUD = hud;

                weaponSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480f, -17.1797f);

                HGTextMeshProUGUI keyText = weaponSlot.transform.Find("DisplayRoot").Find("EquipmentTextBackgroundPanel").Find("EquipmentKeyText").gameObject.GetComponent<HGTextMeshProUGUI>();
                keyText.gameObject.GetComponent<InputBindingDisplayController>().enabled = false;
                keyText.text = "Weapon";

                weaponSlot.transform.Find("DisplayRoot").Find("EquipmentStack").gameObject.SetActive(false);
                weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject.SetActive(false);

                // duration bar
                GameObject chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
                chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

                RectTransform rect = chargeBar.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(-10f, 13f);
                rect.localPosition = new Vector3(-33f, -10f, 0f);
                rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                weaponIconComponent.durationDisplay = chargeBar;
                weaponIconComponent.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                weaponIconComponent.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

                MonoBehaviour.Destroy(equipmentIconComponent);


                // weapon pickup notification

                GameObject notificationPanel = GameObject.Instantiate(hud.transform.Find("MainContainer").Find("NotificationArea").gameObject);
                notificationPanel.transform.SetParent(hud.transform.Find("MainContainer"), true);
                notificationPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, -265f, -150f);
                notificationPanel.transform.localScale = Vector3.one;

                NotificationUIController _old = notificationPanel.GetComponent<NotificationUIController>();
                WeaponNotificationUIController _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

                _new.hud = _old.hud;
                _new.genericNotificationPrefab = Modules.Assets.weaponNotificationPrefab;
                _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

                _old.enabled = false;



                // ammo display for alt passive

                Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("AmmoTracker"))
                {
                    GameObject ammoTracker = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                    ammoTracker.name = "AmmoTracker";
                    ammoTracker.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                    GameObject.DestroyImmediate(ammoTracker.transform.GetChild(0).gameObject);
                    MonoBehaviour.Destroy(ammoTracker.GetComponentInChildren<LevelText>());
                    MonoBehaviour.Destroy(ammoTracker.GetComponentInChildren<ExpBar>());

                    AmmoDisplay ammoTrackerComponent = ammoTracker.AddComponent<AmmoDisplay>();
                    ammoTrackerComponent.targetHUD = hud;
                    ammoTrackerComponent.targetText = ammoTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();

                    ammoTracker.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);

                    //ammoTracker.transform.Find("ExpBarRoot").GetChild(0).GetComponent<Image>().enabled = true;

                    ammoTracker.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                    rect = ammoTracker.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(0.8f, 0.8f, 1f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    rect.anchoredPosition = new Vector2(50f, 0f);
                    rect.localPosition = new Vector3(50f, -95f, 0f);
                }
            }
        }

        internal static void RiskUIHudSetup(RoR2.UI.HUD hud)
        {
            Transform skillsContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomRightCluster").Find("Scaler");

            GameObject weaponSlot = GameObject.Instantiate(skillsContainer.Find("EquipmentSlotPos1").Find("EquipIcon").gameObject, skillsContainer);
            weaponSlot.name = "WeaponSlot";

            EquipmentIcon equipmentIconComponent = weaponSlot.GetComponent<EquipmentIcon>();
            Components.WeaponIcon weaponIconComponent = weaponSlot.AddComponent<Components.WeaponIcon>();

            weaponIconComponent.iconImage = equipmentIconComponent.iconImage;
            weaponIconComponent.displayRoot = equipmentIconComponent.displayRoot;
            weaponIconComponent.flashPanelObject = equipmentIconComponent.stockFlashPanelObject;
            weaponIconComponent.reminderFlashPanelObject = equipmentIconComponent.reminderFlashPanelObject;
            weaponIconComponent.isReadyPanelObject = equipmentIconComponent.isReadyPanelObject;
            weaponIconComponent.tooltipProvider = equipmentIconComponent.tooltipProvider;
            weaponIconComponent.targetHUD = hud;

            MaterialHud.MaterialEquipmentIcon x = weaponSlot.GetComponent<MaterialHud.MaterialEquipmentIcon>();
            Components.MaterialWeaponIcon y = weaponSlot.AddComponent<Components.MaterialWeaponIcon>();

            y.icon = weaponIconComponent;
            y.onCooldown = x.onCooldown;
            y.mask = x.mask;
            y.stockText = x.stockText;

            RectTransform iconRect = weaponSlot.GetComponent<RectTransform>();
            iconRect.localScale = new Vector3(2f, 2f, 2f);
            iconRect.anchoredPosition = new Vector2(-128f, 60f);

            if (DriverPlugin.extendedLoadoutInstalled)
            {
                iconRect.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                iconRect.anchoredPosition = new Vector2(-110f, 60f);
            }

            HGTextMeshProUGUI keyText = weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").Find("SkillBackgroundPanel").Find("SkillKeyText").gameObject.GetComponent<HGTextMeshProUGUI>();
            keyText.gameObject.GetComponent<InputBindingDisplayController>().enabled = false;
            keyText.text = "Weapon";

            weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").Find("StockTextContainer").gameObject.SetActive(false);
            weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject.SetActive(false);

            // duration bar
            GameObject chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
            chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

            RectTransform rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.1f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.localPosition = new Vector3(0f, 0f, 0f);
            rect.anchoredPosition = new Vector2(-8f, 36f);
            rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

            weaponIconComponent.durationDisplay = chargeBar;
            weaponIconComponent.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
            weaponIconComponent.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

            MonoBehaviour.Destroy(equipmentIconComponent);
            MonoBehaviour.Destroy(x);


            // weapon pickup notification

            GameObject notificationPanel = GameObject.Instantiate(hud.transform.Find("MainContainer").Find("NotificationArea").gameObject);
            notificationPanel.transform.SetParent(hud.transform.Find("MainContainer"), true);
            notificationPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, -210f, -50f);
            notificationPanel.transform.localScale = Vector3.one;

            NotificationUIController _old = notificationPanel.GetComponent<NotificationUIController>();
            WeaponNotificationUIController _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

            _new.hud = _old.hud;
            _new.genericNotificationPrefab = Modules.Assets.weaponNotificationPrefab;
            _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

            _old.enabled = false;
        }

        private static void PlayVisionsAnimation(On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.orig_OnEnter orig, EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", self.duration * 12f);
                EffectManager.SimpleMuzzleFlash(EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.muzzleFlashEffectPrefab, self.gameObject, "PistolMuzzle", false);
            }
        }

        private static void PlayChargeLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.orig_PlayChargeAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "ChargeHooks", "Hooks.playbackRate", self.duration * 0.5f);
            }
        }

        private static void PlayThrowLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.orig_PlayThrowAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "ThrowHooks", "Hooks.playbackRate", self.duration);
            }
        }

        private static void PlayRuinAnimation(On.EntityStates.GlobalSkills.LunarDetonator.Detonate.orig_OnEnter orig, EntityStates.GlobalSkills.LunarDetonator.Detonate self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                //self.PlayAnimation("Gesture, Override", "CastRuin", "Ruin.playbackRate", self.duration * 0.5f);
                //Util.PlaySound("PaladinFingerSnap", self.gameObject);
                self.PlayAnimation("Gesture, Override", "PressVoidButton", "Action.playbackRate", 0.5f * self.duration);
                self.StartAimMode(self.duration + 0.5f);

                EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion(),
        new EffectData
        {
            origin = self.FindModelChild("HandL").position,
            rotation = Quaternion.identity,
            scale = 0.5f
        }, false);
            }
        }
    }
}