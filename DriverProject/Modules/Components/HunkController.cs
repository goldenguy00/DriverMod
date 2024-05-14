using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    public class HunkController : MonoBehaviour
    {
        public float iFrames;
        public bool isRolling;
        public bool isAiming;
        public float lockOnTimer;
        public float desiredYOffset;
        public bool immobilized;

        public readonly float snapOffset = 0.25f;
        public readonly float defaultYOffset = 1.59f;

        private CharacterBody characterBody;
        private Animator animator;
        private Transform cameraPivot;
        private bool _wasImmobilized;
        private float yOffset;
        private CameraRigController cameraController;
        private ParticleSystem speedLines;
        private Animator dodgeFlash;
        private Animator counterFlash;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            this.animator = modelLocator.modelBaseTransform.GetComponentInChildren<Animator>();
            this.cameraPivot = modelLocator.modelBaseTransform.Find("CameraPivot").transform;
            this.desiredYOffset = this.defaultYOffset;
            this.yOffset = this.desiredYOffset;
        }

        private void FixedUpdate()
        {
            this.lockOnTimer -= Time.fixedDeltaTime;
            this.iFrames -= Time.fixedDeltaTime;

            if (this.characterBody)
            {
                if (this.immobilized) this.characterBody.moveSpeed = 0f;
                if (this._wasImmobilized && !this.immobilized) this.characterBody.RecalculateStats();
                this._wasImmobilized = this.immobilized;
            }

            if (NetworkServer.active)
            {
                if (this.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    if (this.iFrames <= 0f)
                    {
                        this.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                    }
                }
                else if (this.iFrames > 0f)
                {
                    this.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
            }

            if (this.animator)
            {
                this.animator.SetBool("isRolling", this.isRolling);
            }

            if (!this.cameraController)
            {
                if (this.characterBody && this.characterBody.master)
                {
                    if (this.characterBody.master.playerCharacterMasterController)
                    {
                        if (this.characterBody.master.playerCharacterMasterController.networkUser)
                        {
                            this.cameraController = this.characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                        }
                    }
                }
            }
            else
            {
                this.cameraController.fadeEndDistance = 2f;
                this.cameraController.fadeStartDistance = -5f;
                /*
                if (!this.dodgeFlash)
                {
                    this.dodgeFlash = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("DodgeFlash")).GetComponent<Animator>();
                    this.dodgeFlash.transform.parent = this.cameraController.hud.mainContainer.transform;
                    this.dodgeFlash.gameObject.SetActive(false);

                    RectTransform rect = this.dodgeFlash.GetComponent<RectTransform>();
                    rect.sizeDelta = Vector2.one;
                    rect.localPosition = Vector3.zero;
                }

                if (!this.counterFlash)
                {
                    this.counterFlash = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("CounterFlash")).GetComponent<Animator>();
                    this.counterFlash.transform.parent = this.cameraController.hud.mainContainer.transform;
                    this.counterFlash.gameObject.SetActive(false);

                    RectTransform rect = this.counterFlash.GetComponent<RectTransform>();
                    rect.sizeDelta = Vector2.one;
                    rect.localPosition = Vector3.zero;
                }

                if (!this.speedLines)
                {
                    this.speedLines = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SpeedLines")).GetComponent<ParticleSystem>();
                    this.speedLines.transform.parent = this.cameraController.sceneCam.transform;
                    this.speedLines.transform.localPosition = new Vector3(0f, 0f, 5f);
                    this.speedLines.transform.localRotation = Quaternion.identity;
                    this.speedLines.transform.localScale = Vector3.one;
                    this.speedLines.gameObject.layer = 21;
                }
                else if (!this.speedLines.isPlaying && this.lockOnTimer > 1.2f) this.speedLines.Play();
                else if (this.speedLines.isPlaying) this.speedLines.Stop();*/
            }

            this.yOffset = Mathf.Lerp(this.yOffset, this.desiredYOffset, 5f * Time.fixedDeltaTime);
            this.cameraPivot.localPosition = new Vector3(0f, this.yOffset, 0f);
        }

        public void TriggerDodge()
        {
            if (this.dodgeFlash)
            {
                this.dodgeFlash.gameObject.SetActive(false);
                this.dodgeFlash.gameObject.SetActive(true);
            }
        }

        public void TriggerCounter()
        {
            if (this.counterFlash)
            {
                this.counterFlash.gameObject.SetActive(false);
                this.counterFlash.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            if (this.speedLines) Destroy(this.speedLines.gameObject);
            if (this.dodgeFlash) Destroy(this.dodgeFlash.gameObject);
        }

    }
}