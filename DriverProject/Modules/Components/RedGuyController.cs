﻿using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using R2API.Networking;
using System;
using System.Reflection;

namespace RobDriver.Modules.Components
{
    public class RedGuyController : NetworkBehaviour
    {
        public bool blinkReady;
        public int blinkCount;

        public float drainRate = 24f;
        public float altDrainRate = 16f;
        public float maxDecayRate = 360f;
        public float decayGrowth = 10f;
        public float meter = 0f;

        public bool isWallClinging;
        public float hopoFeatherTimer;

        public int wallJumpCounter;

        public float offsetDistance = 3.5f;
        public CharacterBody characterBody { get; private set; }

        public Transform punchTarget;
        public float chargeValue;

        public bool skibidi;
        public bool inGrab;

        public static event Action<int> onWallJumpIncremented;
        public static event Action<bool> onStageCompleted;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            this.wallJumpCounter = 0;
        }

        private void FixedUpdate()
        {
            this.hopoFeatherTimer -= Time.fixedDeltaTime;

            if (this.characterBody.characterMotor.jumpCount < this.characterBody.maxJumpCount)
            {
                this.blinkReady = true;
            }
        }

        public void RefreshBlink()
        {
            this.characterBody.characterMotor.jumpCount--;
            if (this.characterBody.characterMotor.jumpCount < 0) this.characterBody.characterMotor.jumpCount = 0;
        }

        public void IncrementWallJump()
        {
            this.wallJumpCounter++;

            Action<int> action = onWallJumpIncremented;
            if (action == null) return;
            action(this.wallJumpCounter);
        }
    }
}