using RoR2;
using UnityEngine;

internal enum DriverCameraParams
{
    DEFAULT,
    AIM_PISTOL,
    AIM_SNIPER,
    EMOTE,
    HUNK_DEFAULT,
    HUNK_AIM,
    HUNK_MELEE
}

namespace RobDriver.Modules
{
    internal static class CameraParams
    {
        internal static CharacterCameraParamsData defaultCameraParams;
        internal static CharacterCameraParamsData aimCameraParams;
        internal static CharacterCameraParamsData sniperAimCameraParams;
        internal static CharacterCameraParamsData emoteCameraParams;

        internal static CharacterCameraParamsData hunkMeleeCameraParams;
        internal static CharacterCameraParamsData hunkDefaultCameraParams;
        internal static CharacterCameraParamsData hunkAimCameraParams;

        internal static void InitializeParams()
        {
            defaultCameraParams = NewCameraParams("ccpRobDriver", 70f, 1.37f, new Vector3(0f, 0f, -8.1f));
            aimCameraParams = NewCameraParams("ccpRobDriverAim", 70f, 0.8f, new Vector3(1f, 0f, -5f));
            sniperAimCameraParams = NewCameraParams("ccpRobDriverSniperAim", 70f, 0.8f, new Vector3(0f, 0f, 0.75f));
            emoteCameraParams = NewCameraParams("ccpRobDriverEmote", 70f, 0.4f, new Vector3(0f, 0f, -6f));


            hunkMeleeCameraParams = NewCameraParams("ccpRobDriverMelee", 70f, 0.15f, new Vector3(0.5f, 0.9f, -3.3f));
            hunkDefaultCameraParams = NewCameraParams("ccpRobDriver", 70f, 0.15f, new Vector3(2f, 0.08f, -3.2f));
            hunkAimCameraParams = NewCameraParams("ccpRobDriverAim", 70f, 0.1f, new Vector3(2.2f, 0.1f, -2f));
        }

        private static CharacterCameraParamsData NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, pivotVerticalOffset, standardPosition, 0.1f);
        }

        private static CharacterCameraParamsData NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 idealPosition, float wallCushion)
        {
            CharacterCameraParamsData newParams = new CharacterCameraParamsData();

            newParams.maxPitch = pitch;
            newParams.minPitch = -pitch;
            newParams.pivotVerticalOffset = pivotVerticalOffset;
            newParams.idealLocalCameraPos = idealPosition;
            newParams.wallCushion = wallCushion;

            return newParams;
        }

        internal static CameraTargetParams.CameraParamsOverrideHandle OverrideCameraParams(CameraTargetParams camParams, DriverCameraParams camera, float transitionDuration = 0.5f)
        {
            CharacterCameraParamsData paramsData = GetNewParams(camera);

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = paramsData,
                priority = 0,
            };

            return camParams.AddParamsOverride(request, transitionDuration);
        }

        internal static CharacterCameraParams CreateCameraParamsWithData(DriverCameraParams camera)
        {

            CharacterCameraParams newPaladinCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();

            newPaladinCameraParams.name = camera.ToString().ToLower() + "Params";

            newPaladinCameraParams.data = GetNewParams(camera);

            return newPaladinCameraParams;
        }

        internal static CharacterCameraParamsData GetNewParams(DriverCameraParams camera)
        {
            CharacterCameraParamsData paramsData = defaultCameraParams;

            switch (camera)
            {

                default:
                case DriverCameraParams.DEFAULT:
                    paramsData = defaultCameraParams;
                    break;
                case DriverCameraParams.AIM_PISTOL:
                    paramsData = aimCameraParams;
                    break;
                case DriverCameraParams.AIM_SNIPER:
                    paramsData = sniperAimCameraParams;
                    break;
                case DriverCameraParams.EMOTE:
                    paramsData = emoteCameraParams;
                    break;
                case DriverCameraParams.HUNK_DEFAULT:
                    paramsData = hunkDefaultCameraParams;
                    break;
                case DriverCameraParams.HUNK_AIM:
                    paramsData = hunkAimCameraParams;
                    break;
                case DriverCameraParams.HUNK_MELEE:
                    paramsData = hunkMeleeCameraParams;
                    break;
            }

            return paramsData;
        }
    }
}