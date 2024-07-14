using R2API;
using RobDriver.Modules.Components;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RobDriver.Modules
{
    internal static class Projectiles
    {
        public static GameObject stunGrenadeProjectilePrefab;
        public static GameObject stunGrenadeImpactEffectPrefab;

        public static GameObject rocketProjectilePrefab;
        public static GameObject missileProjectilePrefab;
        public static GameObject bazookaProjectilePrefab;

        public static GameObject plasmaCannonProjectilePrefab;

        public static GameObject hmgGrenadeProjectilePrefab;

        public static GameObject lunarShard;
        public static GameObject lunarShardRed;

        public static GameObject lunarGrenadeProjectilePrefab;

        public static GameObject coinProjectile;

        public static GameObject punchShockwave;

        public static GameObject armCannonPrefab;
        public static GameObject artiGauntletPrefab;

        // please make the names start with "Driver" and end with "Projectile" or "Grenade" for it to use the modded dmg types
        // im so sorry for hard coding it like this
        internal static void RegisterProjectiles()
        {
            #region Stun Grenade
            stunGrenadeProjectilePrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/CommandoGrenadeProjectile").InstantiateClone("RobDriverStunGrenade", true);

            ProjectileController grenadeController = stunGrenadeProjectilePrefab.GetComponent<ProjectileController>();

            ProjectileDamage grenadeDamage = stunGrenadeProjectilePrefab.GetComponent<ProjectileDamage>();
            ProjectileSimple simple = stunGrenadeProjectilePrefab.GetComponent<ProjectileSimple>();
            ProjectileImpactExplosion grenadeImpact = stunGrenadeProjectilePrefab.GetComponent<ProjectileImpactExplosion>();

            Prefabs.projectilePrefabs.Add(stunGrenadeProjectilePrefab);

            stunGrenadeImpactEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/StunChanceOnHit/ImpactStunGrenade.prefab").WaitForCompletion().InstantiateClone("DriverStunGrenadeImpact", true);
            stunGrenadeImpactEffectPrefab.AddComponent<NetworkIdentity>();
            stunGrenadeImpactEffectPrefab.GetComponent<EffectComponent>().soundName = "sfx_driver_stun_grenade";

            GameObject nadeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();
            GameObject radiusIndicator = GameObject.Instantiate(nadeEffect.transform.Find("Nova Sphere").gameObject);
            radiusIndicator.transform.parent = stunGrenadeImpactEffectPrefab.transform;
            radiusIndicator.transform.localPosition = Vector3.zero;
            radiusIndicator.transform.localScale = Vector3.one * grenadeImpact.blastRadius;
            radiusIndicator.transform.localRotation = Quaternion.identity;

            #region Ghost
            GameObject fcuk = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("DriverStunGrenadeImpact"));
            fcuk.transform.parent = stunGrenadeImpactEffectPrefab.transform;
            fcuk.transform.localScale = Vector3.one;
            fcuk.transform.localPosition = Vector3.zero;
            fcuk.transform.localRotation = Quaternion.identity;

            Assets.AddNewEffectDef(stunGrenadeImpactEffectPrefab, "sfx_driver_stun_grenade");
            #endregion

            #region ImpactExplosion
            //grenadeImpact.lifetimeExpiredSoundString = "";
            //grenadeImpact.explosionSoundString = "";
            grenadeImpact.lifetimeExpiredSound = Addressables.LoadAssetAsync<NetworkSoundEventDef>("RoR2/Base/Commando/nseCommandoGrenadeBounce.asset").WaitForCompletion();
            grenadeImpact.offsetForLifetimeExpiredSound = 0.1f;
            grenadeImpact.destroyOnEnemy = false;
            grenadeImpact.destroyOnWorld = false;
            grenadeImpact.timerAfterImpact = true;
            grenadeImpact.falloffModel = BlastAttack.FalloffModel.None;
            grenadeImpact.lifetime = 12;
            grenadeImpact.lifetimeAfterImpact = 0.1f;
            grenadeImpact.lifetimeRandomOffset = 0;
            grenadeImpact.blastRadius = 16;
            grenadeImpact.blastDamageCoefficient = 1;
            grenadeImpact.blastProcCoefficient = 1;
            grenadeImpact.fireChildren = false;
            grenadeImpact.childrenCount = 0;
            grenadeImpact.childrenProjectilePrefab = null;
            grenadeImpact.childrenDamageCoefficient = 0;
            grenadeImpact.impactEffect = stunGrenadeImpactEffectPrefab;

            grenadeController.startSound = "";
            grenadeController.procCoefficient = 1;

            grenadeDamage.crit = false;
            grenadeDamage.damage = 0f;
            grenadeDamage.damageColorIndex = DamageColorIndex.Default;
            grenadeDamage.damageType = DamageType.Stun1s;
            grenadeDamage.force = 1500f;
            #endregion
            #endregion

            CreateHMGGrenade();
            CreateLunarShard();
            CreateLunarGrenade();
            CreateShockwave();
            CreateCoin();

            rocketProjectilePrefab = CreateRocket(false, "DriverRocketProjectile", "DriverRocketGhost", "DriverBigRocketGhost");
            missileProjectilePrefab = CreateRocket(false, "DriverMissileProjectile", "DriverMissileGhost", "DriverMissileGhost");
            bazookaProjectilePrefab = CreateRocket(true, "DriverBazookaProjectile", "DriverBazookaGhost", "DriverRocketGhost");

            plasmaCannonProjectilePrefab = CreateRocket(false, "DriverPlasmaCannonProjectile");
            plasmaCannonProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphereGhost.prefab").WaitForCompletion();
            plasmaCannonProjectilePrefab.GetComponent<ProjectileImpactExplosion>().impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphereExplosion.prefab").WaitForCompletion();

            armCannonPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Vagrant/VagrantCannon.prefab").WaitForCompletion().InstantiateClone("DriverArmCannonProjectile", true);
            artiGauntletPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageFireboltBasic.prefab").WaitForCompletion().InstantiateClone("DriverArtiGauntletProjectile", true);
        }

        private static void CreateLunarShard()
        {
            lunarShard = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/LunarShardProjectile"), "DriverLunarShardProjectile", true);
            DriverPlugin.Destroy(lunarShard.GetComponent<ProjectileSteerTowardTarget>());
            lunarShard.GetComponent<ProjectileImpactExplosion>().blastDamageCoefficient = 1f;

            Prefabs.projectilePrefabs.Add(lunarShard);

            lunarShardRed = lunarShard.InstantiateClone("DriverLunarShardRed", false);
            GameObject ghost = lunarShardRed.GetComponent<ProjectileController>().ghostPrefab.InstantiateClone("DriverLunarShardRedGhost", false);
            ghost.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0] = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();
            ghost.transform.GetChild(1).GetComponent<TrailRenderer>().materials[0] = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion();
            ghost.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            ghost.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            var life = ghost.transform.GetChild(3).GetComponent<ParticleSystem>().colorOverLifetime;
            life.enabled = false;

            lunarShardRed.GetComponent<ProjectileController>().ghostPrefab = ghost;

            Prefabs.projectilePrefabs.Add(lunarShardRed);
        }

        private static void CreateHMGGrenade()
        {
            hmgGrenadeProjectilePrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "DriverHMGGrenade");
            hmgGrenadeProjectilePrefab.AddComponent<Modules.Components.RocketRotation>();
            hmgGrenadeProjectilePrefab.transform.localScale *= 2f;

            ProjectileImpactExplosion impactExplosion = hmgGrenadeProjectilePrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);

            GameObject fuckMyLife = null;
            if (Modules.Config.badass.Value)
            {
                fuckMyLife = Modules.Assets.badassSmallExplosionEffect;
                fuckMyLife.AddComponent<NetworkIdentity>();
            }
            else
            {
                fuckMyLife = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("StupidFuckExplosion2", true);
                fuckMyLife.AddComponent<NetworkIdentity>();

                GameObject nadeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();
                GameObject radiusIndicator = GameObject.Instantiate(nadeEffect.transform.Find("Nova Sphere").gameObject);
                radiusIndicator.transform.parent = fuckMyLife.transform;
                radiusIndicator.transform.localPosition = Vector3.zero;
                radiusIndicator.transform.localScale = Vector3.one;
                radiusIndicator.transform.localRotation = Quaternion.identity;

                Assets.AddNewEffectDef(fuckMyLife, "sfx_driver_grenade_explosion");
            }

            impactExplosion.blastRadius = 6f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.lifetime = 12f;
            impactExplosion.impactEffect = fuckMyLife;
            //impactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("sfx_driver_explosion");
            impactExplosion.timerAfterImpact = true;
            impactExplosion.lifetimeAfterImpact = 0f;

            ProjectileController rocketController = hmgGrenadeProjectilePrefab.GetComponent<ProjectileController>();

            rocketController.ghostPrefab = stunGrenadeProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab;
            rocketController.startSound = "";

            hmgGrenadeProjectilePrefab.GetComponent<Rigidbody>().useGravity = true;

            Prefabs.projectilePrefabs.Add(hmgGrenadeProjectilePrefab);
        }

        private static void CreateLunarGrenade()
        {
            lunarGrenadeProjectilePrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "DriverLunarGrenade");
            lunarGrenadeProjectilePrefab.transform.localScale *= 2f;
            ProjectileImpactExplosion impactExplosion = lunarGrenadeProjectilePrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);

            impactExplosion.blastRadius = 7f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.lifetime = 12f;
            impactExplosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/LunarGolemTwinShotExplosion.prefab").WaitForCompletion();
            impactExplosion.timerAfterImpact = true;
            impactExplosion.lifetimeAfterImpact = 0f;

            ProjectileController rocketController = lunarGrenadeProjectilePrefab.GetComponent<ProjectileController>();

            rocketController.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarExploder/LunarExploderShardGhost.prefab").WaitForCompletion();
            rocketController.startSound = "";

            lunarGrenadeProjectilePrefab.GetComponent<Rigidbody>().useGravity = true;

            Prefabs.projectilePrefabs.Add(lunarGrenadeProjectilePrefab);
        }

        private static void CreateCoin()
        {
            var coinProjectileGhost = CreateGhostPrefab("CoinProjectileGhost");
            coinProjectileGhost.name = "DriverCoinProjectileGhost";

            var pG = coinProjectileGhost.GetComponent<ProjectileGhostController>();
            pG.inheritScaleFromProjectile = false;

            coinProjectileGhost.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material = Modules.Assets.twinkleMat;
            coinProjectileGhost.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.yellow);
            coinProjectileGhost.transform.GetChild(1).GetComponent<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Captain/matCaptainTracerTrail.mat").WaitForCompletion();
            coinProjectileGhost.transform.GetChild(1).GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.yellow);
            coinProjectile = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("CoinProjectile").InstantiateClone("DriverCoinProjectile", true);
            coinProjectile.AddComponent<NetworkIdentity>();
            var soundLoop = coinProjectile.AddComponent<StopCoinSound>();
            soundLoop.SoundEventToPlay = "sfx_driver_coin_spin";

            var projectileController = coinProjectile.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = coinProjectileGhost;
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = 1;

            coinProjectile.AddComponent<SkillLocator>();
            var teamComponent = coinProjectile.AddComponent<TeamComponent>();
            teamComponent.hideAllyCardDisplay = false;

            var characterBody = coinProjectile.AddComponent<CharacterBody>();
            characterBody.baseVisionDistance = Mathf.Infinity;
            characterBody.sprintingSpeedMultiplier = 1.45f;
            characterBody.hullClassification = HullClassification.Human;
            characterBody.SetSpreadBloom(0f);

            var healthComponent = coinProjectile.AddComponent<HealthComponent>();
            healthComponent.body = characterBody;
            healthComponent.dontShowHealthbar = true;
            healthComponent.globalDeathEventChanceCoefficient = 0f;

            var projectileSimple = coinProjectile.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 5f;
            projectileSimple.desiredForwardSpeed = 30f;
            projectileSimple.updateAfterFiring = false;
            projectileSimple.enableVelocityOverLifetime = false;
            projectileSimple.oscillate = false;
            projectileSimple.oscillateMagnitude = 20f;
            projectileSimple.oscillateSpeed = 0f;

            var dcbc = coinProjectile.AddComponent<DisableCollisionsBetweenColliders>();
            dcbc.collidersA = new Collider[1];
            dcbc.collidersA[0] = coinProjectile.GetComponent<SphereCollider>();
            dcbc.collidersB = new Collider[1];
            dcbc.collidersB[0] = coinProjectile.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SphereCollider>();

            var coinController = coinProjectile.AddComponent<CoinController>();
            coinController.projectileHealthComponent = healthComponent;
            coinController.controller = projectileController;
            coinController.ricochetSound = Modules.Assets.CreateNetworkSoundEventDef("sfx_driver_stun_grenade");
            coinController.canRicochet = true;
            coinController.ricochetMultiplier = 2f;

            var modelLocator = coinProjectile.AddComponent<ModelLocator>();
            modelLocator.modelTransform = coinProjectile.transform.GetChild(0);
            modelLocator.modelBaseTransform = coinProjectile.transform;
            modelLocator.autoUpdateModelTransform = true;
            modelLocator.dontDetatchFromParent = true;
            modelLocator.noCorpse = true;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.preserveModel = false;
            modelLocator.normalizeToFloor = false;
            modelLocator.normalSmoothdampTime = 0.1f;
            modelLocator.normalMaxAngleDelta = 90f;

            var hurtBoxGroup = coinProjectile.transform.GetChild(0).gameObject.AddComponent<HurtBoxGroup>();
            List<HurtBox> hurtboxes = new List<HurtBox>();

            var hurtBox = coinProjectile.transform.GetChild(0).GetChild(0).gameObject.AddComponent<HurtBox>();
            hurtBox.gameObject.layer = LayerIndex.entityPrecise.intVal;
            hurtBox.healthComponent = healthComponent;
            hurtBox.isBullseye = true;
            hurtBox.isSniperTarget = true;
            hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            hurtBox.hurtBoxGroup = hurtBoxGroup;

            hurtboxes.Add(hurtBox);

            hurtBoxGroup.hurtBoxes = hurtboxes.ToArray();
            hurtBoxGroup.mainHurtBox = hurtBox;
            hurtBoxGroup.bullseyeCount = 1;

            Prefabs.bodyPrefabs.Add(coinProjectile);
            Prefabs.projectilePrefabs.Add(coinProjectile);
        }
        private static void CreateShockwave()
        {
            punchShockwave = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderZapCone.prefab").WaitForCompletion().InstantiateClone("DriverPunchShockwaveProjectile", true);

            var p = punchShockwave.GetComponent<ProjectileProximityBeamController>();
            p.lightningType = RoR2.Orbs.LightningOrb.LightningType.MageLightning;
            p.damageCoefficient = 1f;

            punchShockwave.transform.Find("Effect/Flash").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matCritImpactShockwave.mat").WaitForCompletion();
            var c = punchShockwave.transform.Find("Effect/Flash").GetComponent<ParticleSystem>().main;
            c.startColor = Color.red;

            punchShockwave.transform.Find("Effect/Impact Shockwave").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniRing1Void.mat").WaitForCompletion();

            punchShockwave.transform.Find("Flash").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffect.mat").WaitForCompletion();

            punchShockwave.transform.Find("Effect/Sparks, Single").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();

            punchShockwave.transform.Find("Effect/Lines").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();

            punchShockwave.transform.Find("Effect/Ring").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();

            punchShockwave.transform.Find("Effect/Point Light").GetComponent<Light>().color = Color.red;

            Modules.Prefabs.projectilePrefabs.Add(punchShockwave);

        }
        private static GameObject CreateRocket(bool gravity, string projectileName, string ghostName = "", string ghostToLoad = "")
        {
            GameObject projectilePrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", projectileName);
            projectilePrefab.AddComponent<Modules.Components.RocketRotation>();
            projectilePrefab.transform.localScale *= 2f;

            ProjectileImpactExplosion impactExplosion = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);

            GameObject fuckMyLife = null;
            if (Modules.Config.badass.Value)
            {
                fuckMyLife = Modules.Assets.badassExplosionEffect;
                fuckMyLife.AddComponent<NetworkIdentity>();
            }
            else
            {
                fuckMyLife = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("StupidFuckExplosion", true);
                fuckMyLife.AddComponent<NetworkIdentity>();

                GameObject nadeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();
                GameObject radiusIndicator = GameObject.Instantiate(nadeEffect.transform.Find("Nova Sphere").gameObject);
                radiusIndicator.transform.parent = fuckMyLife.transform;
                radiusIndicator.transform.localPosition = Vector3.zero;
                radiusIndicator.transform.localScale = Vector3.one;
                radiusIndicator.transform.localRotation = Quaternion.identity;

                Assets.AddNewEffectDef(fuckMyLife, "sfx_driver_explosion");
            }

            impactExplosion.blastRadius = 10f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.lifetime = 12f;
            impactExplosion.impactEffect = fuckMyLife;
            //impactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("sfx_driver_explosion");
            impactExplosion.timerAfterImpact = true;
            impactExplosion.lifetimeAfterImpact = 0f;

            ProjectileController rocketController = projectilePrefab.GetComponent<ProjectileController>();

            if (ghostName != "")
            {
                GameObject ghost = CreateGhostPrefab(ghostToLoad);
                ghost.name = ghostName;
                ghost.transform.GetChild(0).Find("Smoke").gameObject.AddComponent<Modules.Components.DetachOnDestroy>();
                ghost.transform.GetChild(0).Find("Smoke").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matDustDirectional.mat").WaitForCompletion();
                ghost.transform.GetChild(0).Find("Flame").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Golem/matVFXFlame1.mat").WaitForCompletion();

                rocketController.ghostPrefab = ghost;
                rocketController.startSound = "";
            }

            projectilePrefab.GetComponent<Rigidbody>().useGravity = gravity;

            Prefabs.projectilePrefabs.Add(projectilePrefab);

            return projectilePrefab;
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.explosionSoundString = "";
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeExpiredSoundString = "";
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName).InstantiateClone(ghostName);
            ghostPrefab.AddComponent<NetworkIdentity>();
            ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}