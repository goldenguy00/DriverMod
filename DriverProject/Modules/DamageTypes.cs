using R2API;
using RoR2.Orbs;
using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using RobDriver.Modules.Components;
using RobDriver.Modules.Survivors;
using System.Collections.Generic;
using System.Linq;
using System;
using HG;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using UnityEngine.UIElements;
using System.ComponentModel;

namespace RobDriver.Modules
{
    public static class DamageTypes
    {
        public static List<DriverBulletInfo> bulletTypes = new List<DriverBulletInfo>();

        internal static DamageType[] allowedDamageTypes = new DamageType[]
        {
            DamageType.ResetCooldownsOnKill,
            DamageType.SlowOnHit,
            DamageType.Stun1s,
            DamageType.IgniteOnHit,
            DamageType.Freeze2s,
            DamageType.ClayGoo,
            DamageType.BleedOnHit,
            DamageType.PoisonOnHit,
            DamageType.WeakOnHit,
            DamageType.Nullify,
            DamageType.BonusToLowHealth,
            DamageType.BlightOnHit,
            DamageType.CrippleOnHit,
            DamageType.SuperBleedOnCrit,
            DamageType.FruitOnHit
        };

        public static DamageAPI.ModdedDamageType Generic;
        public static DamageAPI.ModdedDamageType HookShot;
        public static DamageAPI.ModdedDamageType MissileShot;
        public static DamageAPI.ModdedDamageType VoidMissileShot;
        public static DamageAPI.ModdedDamageType ExplosiveRounds;
        public static DamageAPI.ModdedDamageType FlameTornadoShot;
        public static DamageAPI.ModdedDamageType IceBlastShot;
        public static DamageAPI.ModdedDamageType DaggerShot;
        public static DamageAPI.ModdedDamageType LightningStrikeRounds;
        public static DamageAPI.ModdedDamageType FireballRounds;
        public static DamageAPI.ModdedDamageType StickyShot;
        public static DamageAPI.ModdedDamageType VoidLightning;
        public static DamageAPI.ModdedDamageType CoinShot;
        public static DamageAPI.ModdedDamageType MysteryShot;

        public struct DriverBulletInfo
        {
            public string nameToken;
            public DamageType bulletType;
            public DamageAPI.ModdedDamageType moddedBulletType;
            public Sprite icon;
            public Color trailColor;
        }

        internal static void Init()
        {
            Generic = DamageAPI.ReserveDamageType();
            HookShot = DamageAPI.ReserveDamageType();
            MissileShot = DamageAPI.ReserveDamageType();
            VoidMissileShot = DamageAPI.ReserveDamageType();
            ExplosiveRounds = DamageAPI.ReserveDamageType();
            FlameTornadoShot = DamageAPI.ReserveDamageType();
            IceBlastShot = DamageAPI.ReserveDamageType();
            DaggerShot = DamageAPI.ReserveDamageType();
            LightningStrikeRounds = DamageAPI.ReserveDamageType();
            FireballRounds = DamageAPI.ReserveDamageType();
            StickyShot = DamageAPI.ReserveDamageType();
            VoidLightning = DamageAPI.ReserveDamageType();
            CoinShot = DamageAPI.ReserveDamageType();
            MysteryShot = DamageAPI.ReserveDamageType();

            InitializeBullets();
            Hook();
        }

        private static void Hook()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            On.RoR2.GlobalEventManager.OnHitEnemy += new On.RoR2.GlobalEventManager.hook_OnHitEnemy(GlobalEventManager_OnHitEnemy);
            On.RoR2.GlobalEventManager.OnHitAll += new On.RoR2.GlobalEventManager.hook_OnHitAll(GlobalEventManager_OnHitAll);
        }
        public static void InitializeBullets()
        {
            int common = 5;
            int uncommon = 3;
            int legendary = 1;
            foreach (DamageType i in allowedDamageTypes)
            {
                string name = Enum.GetName(typeof(DamageType), i);
                //Renaming
                if (name == "ResetCooldownsOnKill")
                {
                    DamageTypes.AddNewBullet("Resetting Rounds", i, Color.red);
                }
                if (name == "SlowOnHit")
                {
                    DamageTypes.AddNewBullet("Slowing Rounds", i, UnityEngine.Color.yellow);
                }
                if (name == "Stun1s")
                {
                    DamageTypes.AddNewBullet("Stunning Rounds", i, UnityEngine.Color.gray);
                }
                if (name == "IgniteOnHit")
                {
                    DamageTypes.AddNewBullet("Incendiary Rounds", i, new UnityEngine.Color(255f / 255f, 127f / 255f, 80 / 255f));
                }
                if (name == "Freeze2s")
                {
                    DamageTypes.AddNewBullet("Frostbite Rounds", i, UnityEngine.Color.cyan, null, uncommon);
                }
                if (name == "ClayGoo")
                {
                    DamageTypes.AddNewBullet("Goo Rounds", i, UnityEngine.Color.black);
                }
                if (name == "BleedOnHit")
                {
                    DamageTypes.AddNewBullet("Serrated Rounds", i, DamageColor.FindColor(DamageColorIndex.Bleed));
                }
                if (name == "PoisonOnHit")
                {
                    DamageTypes.AddNewBullet("Poison Rounds", i, UnityEngine.Color.green);
                }
                if (name == "WeakOnHit")
                {
                    DamageTypes.AddNewBullet("Weakening Rounds", i, new UnityEngine.Color(220f / 255f, 237f / 255f, 159f / 255f));
                }
                if (name == "Nullify")
                {
                    DamageTypes.AddNewBullet("Nullifying Rounds", i, DamageColor.FindColor(DamageColorIndex.Void));
                }
                if (name == "BonusToLowHealth")
                {
                    DamageTypes.AddNewBullet("Executing Rounds", i, DamageColor.FindColor(DamageColorIndex.Fragile));
                }
                if (name == "BlightOnHit")
                {
                    DamageTypes.AddNewBullet("Blighting Rounds", i, new UnityEngine.Color(222f / 255f, 85f / 255f, 230f / 255f));
                }
                if (name == "CrippleOnHit")
                {
                    DamageTypes.AddNewBullet("Crippling Rounds", i, new UnityEngine.Color(48f / 255f, 205f / 255f, 217f / 255f));
                }
                if (name == "SuperBleedOnCrit")
                {
                    DamageTypes.AddNewBullet("Hemorrhaging Rounds", i, DamageColor.FindColor(DamageColorIndex.SuperBleed), null, uncommon);
                }
                if (name == "FruitOnHit")
                {
                    DamageTypes.AddNewBullet("Fruitful Rounds", i, new UnityEngine.Color(255f / 255f, 191f / 255f, 225f / 255f));
                }
            }

            DamageTypes.AddNewModdedBullet("Hook Shot", DamageTypes.HookShot, Color.grey, null, legendary);

            DamageTypes.AddNewModdedBullet("Missle Shot", DamageTypes.MissileShot, new UnityEngine.Color(219 / 255f, 132 / 255f, 11 / 255f), null, legendary);

            DamageTypes.AddNewModdedBullet("Void Rounds", DamageTypes.VoidMissileShot, new UnityEngine.Color(122 / 255f, 69 / 255f, 173 / 255f), null, uncommon);

            DamageTypes.AddNewModdedBullet("Explosive Rounds", DamageTypes.ExplosiveRounds, Color.yellow, null, uncommon);

            DamageTypes.AddNewModdedBullet("Elemental Flame Rounds", DamageTypes.FlameTornadoShot, new UnityEngine.Color(255f / 255f, 127f / 255f, 80 / 255f), null, uncommon);

            DamageTypes.AddNewModdedBullet("Elemental Ice Rounds", DamageTypes.IceBlastShot, UnityEngine.Color.cyan, null, uncommon);

            DamageTypes.AddNewModdedBullet("Dagger Shot", DamageTypes.DaggerShot, Color.black, null, legendary);

            DamageTypes.AddNewModdedBullet("Lightning Rounds", DamageTypes.LightningStrikeRounds, Color.cyan, null, legendary);

            DamageTypes.AddNewModdedBullet("Fireball Rounds", DamageTypes.FireballRounds, new UnityEngine.Color(255f / 255f, 127f / 255f, 80 / 255f), null, legendary);

            DamageTypes.AddNewModdedBullet("Sticky Shot", DamageTypes.StickyShot, new UnityEngine.Color(255 / 255f, 117 / 255f, 48 / 255f));

            DamageTypes.AddNewModdedBullet("Void Strike Rounds", DamageTypes.VoidLightning, new UnityEngine.Color(194 / 255f, 115 / 255f, 255 / 255f), null, uncommon);

            DamageTypes.AddNewModdedBullet("Coin Shot", DamageTypes.CoinShot, new UnityEngine.Color(255 / 255f, 212 / 255f, 94 / 255f));

            DamageTypes.AddNewModdedBullet("Mystery Shot", DamageTypes.MysteryShot, new UnityEngine.Color(30 / 255f, 51 / 255f, 45 / 255f));

        }
        public static void AddNewModdedBullet(string name, DamageAPI.ModdedDamageType bulletType, Color color, Sprite icon = null, int chance = 1)
        {
            if (icon == null) icon = Assets.bulletSprite;
            DriverBulletInfo bulletDef = new DriverBulletInfo
            {
                nameToken = name,
                moddedBulletType = bulletType,
                bulletType = DamageType.Generic,
                icon = icon,
                trailColor = color  
            };
            for (int i = 0; i < chance; i++) bulletTypes.Add(bulletDef);
        }

        public static void AddNewBullet(string name, DamageType bulletType, Color color, Sprite icon = null, int chance = 5)
        {
            if (icon == null) icon = Assets.bulletSprite;
            DriverBulletInfo bulletDef = new DriverBulletInfo
            {
                nameToken = name,
                bulletType = bulletType,
                moddedBulletType = Generic,
                icon = icon,
                trailColor = color
            };
            for (int i = 0; i < chance; i++) bulletTypes.Add(bulletDef);
        }

        private static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (damageInfo.HasModdedDamageType(MysteryShot))
            {
                System.Random rnd = new System.Random();
                int bulletIndex = rnd.Next(Buffs.bulletDefs.Count);

                damageInfo.damageType |= DamageTypes.bulletTypes[bulletIndex].bulletType;
                damageInfo.RemoveModdedDamageType(MysteryShot);
                damageInfo.AddModdedDamageType(DamageTypes.bulletTypes[bulletIndex].moddedBulletType);
            }
            
            orig.Invoke(self, damageInfo, victim);

            CharacterBody victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
            CharacterBody attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
            if (NetworkServer.active && attackerBody)
            {
                TeamComponent attackerTeam = attackerBody.GetComponent<TeamComponent>();
                TeamIndex attackerTeamIndex = attackerTeam ? attackerTeam.teamIndex : TeamIndex.Neutral;

                if (damageInfo.HasModdedDamageType(HookShot))
                {
                    List<HurtBox> targets = CollectionPool<HurtBox, List<HurtBox>>.RentCollection();
                    List<HealthComponent> exclusions = CollectionPool<HealthComponent, List<HealthComponent>>.RentCollection();
                    if (attackerBody.healthComponent)
                    {
                        exclusions.Add(attackerBody.healthComponent);
                    }
                    if (victimBody && victimBody.healthComponent)
                    {
                        exclusions.Add(victimBody.healthComponent);
                    }
                    BounceOrb.SearchForTargets(new BullseyeSearch(), attackerTeamIndex, damageInfo.position, 30f /*range*/, 10 /*maxTargets*/, targets, exclusions);
                    CollectionPool<HealthComponent, List<HealthComponent>>.ReturnCollection(exclusions);
                    List<HealthComponent> bouncedObjects = new List<HealthComponent> { victim.GetComponent<HealthComponent>() };

                    float damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1f /*damageCoefficient*/);
                    for (int i = 0; i < targets.Count; i++)
                    {
                        HurtBox hurtBox = targets[i];
                        if (hurtBox)
                        {
                            BounceOrb bounceOrb = new BounceOrb
                            {
                                origin = damageInfo.position,
                                damageValue = damageValue,
                                isCrit = damageInfo.crit,
                                teamIndex = attackerTeamIndex,
                                attacker = damageInfo.attacker,
                                procChainMask = damageInfo.procChainMask,
                                procCoefficient = 0.33f,
                                damageColorIndex = DamageColorIndex.Default,
                                bouncedObjects = bouncedObjects,
                                target = hurtBox
                            };
                            OrbManager.instance.AddOrb(bounceOrb);
                        }
                    }
                    CollectionPool<HurtBox, List<HurtBox>>.ReturnCollection(targets);
                }

                if (damageInfo.HasModdedDamageType(MissileShot))
                {
                    if (Util.CheckRoll(10f * damageInfo.procCoefficient, attackerBody.master))
                    {
                        float damageCoefficient = 1f + attackerBody.inventory.GetItemCount(RoR2Content.Items.Missile);
                        float missileDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);
                        MissileUtils.FireMissile(attackerBody.corePosition, attackerBody, damageInfo.procChainMask, victim, missileDamage, damageInfo.crit,
                            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/MissileProjectile.prefab").WaitForCompletion(), DamageColorIndex.Item, addMissileProc: true);
                    }
                }
                
                if (damageInfo.HasModdedDamageType(VoidMissileShot))
                {
                    int icbmCount = 0;
                    int missileVoidCount = 0;
                    int missileCount = 0;
                    
                    if (attackerBody.inventory)
                    {
                        icbmCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                        missileVoidCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MissileVoid);
                        missileCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.Missile);
                    }

                    float icbmDamageCoefficient = 1f + 0.5f * (icbmCount - 1);
                    float damageCoefficient = 0.4f + 0.4f * (missileCount + missileVoidCount);
                    float damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient) * icbmDamageCoefficient;

                    for (int i = 0; i < (icbmCount == 0 ? 1 : 3); i++)
                    {
                        MissileVoidOrb missileVoidOrb = new MissileVoidOrb
                        {
                            origin = attackerBody.aimOrigin,
                            damageValue = damageValue,
                            isCrit = damageInfo.crit,
                            teamIndex = attackerTeamIndex,
                            attacker = damageInfo.attacker,
                            procChainMask = damageInfo.procChainMask,
                            procCoefficient = 0.2f,
                            damageColorIndex = DamageColorIndex.Void
                        };
                        missileVoidOrb.procChainMask.AddProc(ProcType.Missile);

                        if (victimBody && victimBody.mainHurtBox)
                        {
                            missileVoidOrb.target = victimBody.mainHurtBox;
                            OrbManager.instance.AddOrb(missileVoidOrb);
                        }
                    }
                }

                if (damageInfo.HasModdedDamageType(FlameTornadoShot))
                {
                    GameObject gameObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireTornado");
                    float resetInterval = gameObject.GetComponent<ProjectileOverlapAttack>().resetInterval;
                    float lifetime = gameObject.GetComponent<ProjectileSimple>().lifetime;
                    float damageCoefficient = 1.5f + 1.5f * attackerBody.inventory.GetItemCount(RoR2Content.Items.FireRing);
                    float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient) / lifetime * resetInterval;

                    Vector3 vector = damageInfo.position - attackerBody.aimOrigin;
                    vector.y = 0f;
                    Quaternion rotation;
                    float speedOverride;
                    if (vector != Vector3.zero)
                    {
                        speedOverride = -1f;
                        rotation = Util.QuaternionSafeLookRotation(vector, Vector3.up);
                    }
                    else
                    {
                        rotation = Quaternion.identity;
                        speedOverride = 0f;
                    }

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.Rings);
                    ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                    {
                        damage = damage,
                        crit = damageInfo.crit,
                        damageColorIndex = DamageColorIndex.Item,
                        position = damageInfo.position,
                        procChainMask = procChainMask,
                        force = 0f,
                        owner = damageInfo.attacker,
                        projectilePrefab = gameObject,
                        rotation = rotation,
                        speedOverride = speedOverride,
                        target = null
                    });
                }

                if (damageInfo.HasModdedDamageType(IceBlastShot))
                {
                    float damageCoefficient = 1.25f + 1.25f * attackerBody.inventory.GetItemCount(RoR2Content.Items.IceRing);
                    float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);

                    EffectManager.SimpleImpactEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/IceRingExplosion"), damageInfo.position, Vector3.up, transmit: true);
                    victimBody.AddTimedBuff(RoR2Content.Buffs.Slow80, 3f * attackerBody.inventory.GetItemCount(RoR2Content.Items.IceRing));

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.Rings);
                    victimBody.healthComponent.TakeDamage(new DamageInfo
                    {
                        damage = damage,
                        damageColorIndex = DamageColorIndex.Item,
                        damageType = DamageType.Generic,
                        attacker = damageInfo.attacker,
                        crit = damageInfo.crit,
                        force = Vector3.zero,
                        inflictor = null,
                        position = damageInfo.position,
                        procChainMask = procChainMask,
                        procCoefficient = 1f
                    });
                }

                if (damageInfo.HasModdedDamageType(DaggerShot))
                {
                    Vector3 position = Vector3.zero;
                    Transform victimTransform = victimBody.gameObject.transform;
                    Transform attackerTransform = attackerBody.gameObject.transform;
                    if (victimTransform && attackerTransform)
                    {
                        position = Vector3.Lerp(victimTransform.position, attackerTransform.position, 0.75f);
                    }
                    position += Vector3.up * 1.8f;
                    position += UnityEngine.Random.insideUnitSphere * 0.5f;

                    Quaternion rotation = Util.QuaternionSafeLookRotation(Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f);
                    float damageValue = Util.OnKillProcDamage(attackerBody.damage, 1.5f + 1.5f * attackerBody.inventory.GetItemCount(RoR2Content.Items.Dagger));
                    float force = 200f;
                    ProjectileManager.instance.FireProjectile(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Dagger/DaggerProjectile.prefab").WaitForCompletion(), 
                        position, rotation, attackerBody.gameObject, damageValue, force, Util.CheckRoll(attackerBody.crit, attackerBody.master), DamageColorIndex.Item);
                }

                if (damageInfo.HasModdedDamageType(LightningStrikeRounds))
                {
                    float damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 2.5f + 2.5f * attackerBody.inventory.GetItemCount(RoR2Content.Items.LightningStrikeOnHit));
                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.LightningStrikeOnHit);

                    HurtBox target = victimBody.mainHurtBox;
                    if (victimBody.hurtBoxGroup)
                    {
                        target = victimBody.hurtBoxGroup.hurtBoxes[UnityEngine.Random.Range(0, victimBody.hurtBoxGroup.hurtBoxes.Length)];
                    }

                    OrbManager.instance.AddOrb(new SimpleLightningStrikeOrb
                    {
                        attacker = attackerBody.gameObject,
                        damageColorIndex = DamageColorIndex.Item,
                        damageValue = damageValue,
                        isCrit = Util.CheckRoll(attackerBody.crit, attackerBody.master),
                        procChainMask = procChainMask,
                        procCoefficient = 1f,
                        target = target
                    });
                }
                
                if (damageInfo.HasModdedDamageType(FireballRounds))
                {
                    //Vector3 vector3 = (inputBank ? inputBank.aimDirection : victim.transform.forward);
                    if (Util.CheckRoll(10f * damageInfo.procCoefficient, attackerBody.master))
                    {
                        Vector3 origin = (attackerBody.characterMotor ? (victim.transform.position + Vector3.up * (attackerBody.characterMotor.capsuleHeight * 0.5f + 2f)) : (victim.transform.position + Vector3.up * 2f));
                        EffectData effectData = new EffectData
                        {
                            scale = 1f,
                            origin = origin
                        };
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashFireMeatBall"), effectData, transmit: true);
                        float damageCoefficient = 1.5f + 1.5f * attackerBody.inventory.GetItemCount(RoR2Content.Items.FireballsOnHit);
                        ProcChainMask procChainMask = damageInfo.procChainMask;
                        procChainMask.AddProc(ProcType.Meatball);

                        int fireballCount = 3;
                        Vector3 rotation = Vector3.up;
                        for (int i = 0; i < fireballCount; i++)
                        {
                            float offset = i * (float)Math.PI * 2f / fireballCount;
                            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                            {
                                projectilePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireMeatBall"),
                                position = origin + new Vector3(Mathf.Sin(offset), 0f, Mathf.Cos(offset)),
                                rotation = Util.QuaternionSafeLookRotation(rotation),
                                procChainMask = procChainMask,
                                target = victim,
                                owner = attackerBody.gameObject,
                                damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient),
                                crit = damageInfo.crit,
                                force = 200f,
                                damageColorIndex = DamageColorIndex.Item,
                                speedOverride = UnityEngine.Random.Range(15f, 30f),
                                useSpeedOverride = true
                            });
                            rotation.x += Mathf.Sin(offset + UnityEngine.Random.Range(-20f, 20f));
                            rotation.z += Mathf.Cos(offset + UnityEngine.Random.Range(-20f, 20f));
                        }
                    }
                }
                
                if (damageInfo.HasModdedDamageType(StickyShot))
                {
                    Vector3 forward = victimBody.corePosition - damageInfo.position;
                    Quaternion rotation = forward.magnitude != 0f ? Util.QuaternionSafeLookRotation(forward) : UnityEngine.Random.rotationUniform;
                    float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.8f);

                    ProjectileManager.instance.FireProjectile(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/StickyBomb"),
                        damageInfo.position, rotation, damageInfo.attacker, damage, 100f, damageInfo.crit, DamageColorIndex.Item,
                        null /*target*/, attackerBody.healthComponent.alive ? forward.magnitude * 5f : -1f);
                }
                
                if (damageInfo.HasModdedDamageType(VoidLightning))
                {
                    VoidLightningOrb voidLightningOrb = new VoidLightningOrb
                    {
                        origin = damageInfo.position,
                        damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 0.6f),
                        isCrit = damageInfo.crit,
                        totalStrikes = 2 + 3 * attackerBody.inventory.GetItemCount(DLC1Content.Items.ChainLightningVoid),
                        teamIndex = attackerTeamIndex,
                        attacker = damageInfo.attacker,
                        procChainMask = damageInfo.procChainMask,
                        procCoefficient = 0.2f,
                        damageColorIndex = DamageColorIndex.Void,
                        secondsPerStrike = 0.1f
                    };
                    voidLightningOrb.procChainMask.AddProc(ProcType.ChainLightning);

                    if (victimBody && victimBody.mainHurtBox)
                    {
                        voidLightningOrb.target = victimBody.mainHurtBox;
                        OrbManager.instance.AddOrb(voidLightningOrb);
                    }
                }
                
                if (damageInfo.HasModdedDamageType(CoinShot))
                {
                    GoldOrb goldOrb = new GoldOrb
                    {
                        origin = damageInfo.position,
                        target = attackerBody.mainHurtBox,
                        goldAmount = (uint)(2f * Run.instance.difficultyCoefficient)
                    };
                    OrbManager.instance.AddOrb(goldOrb);
                    EffectManager.SimpleImpactEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"), damageInfo.position, Vector3.up, transmit: true);
                }
            }
        }

        private static void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            orig.Invoke(self, damageInfo, hitObject);

            if (damageInfo.procCoefficient == 0f || damageInfo.rejected || !damageInfo.attacker)
            {
                return;
            }
            CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
            if (!attackerBody)
            {
                return;
            }
            CharacterMaster master = attackerBody.master;
            if (!master)
            {
                return;
            }
            Inventory inventory = master.inventory;
            if (!inventory)
            {
                return;
            }

            if (damageInfo.HasModdedDamageType(ExplosiveRounds))
            {
                float radius = (1.5f + 2.5f * inventory.GetItemCount(RoR2Content.Items.Behemoth)) * damageInfo.procCoefficient;
                float damageCoefficient = 0.6f;
                float baseDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);

                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                {
                    origin = damageInfo.position,
                    scale = radius,
                    rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
                }, transmit: true);

                BlastAttack obj = new BlastAttack
                {
                    position = damageInfo.position,
                    baseDamage = baseDamage,
                    baseForce = 0f,
                    radius = radius,
                    attacker = damageInfo.attacker,
                    inflictor = null,
                    teamIndex = TeamComponent.GetObjectTeam(damageInfo.attacker),
                    crit = damageInfo.crit,
                    procChainMask = damageInfo.procChainMask,
                    procCoefficient = 0f,
                    damageColorIndex = DamageColorIndex.Item,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageType = damageInfo.damageType
                };
                obj.Fire();
            }
        }

        private static void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            DamageInfo damageInfo = damageReport.damageInfo;
            if (!damageReport.attackerBody || !damageReport.victimBody)
            {
                return;
            }
            HealthComponent victim = damageReport.victim;
            GameObject inflictorObject = damageInfo.inflictor;
            CharacterBody victimBody = damageReport.victimBody;
            CharacterBody attackerBody = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            if (NetworkServer.active)
            {
            }
        }
    }
}