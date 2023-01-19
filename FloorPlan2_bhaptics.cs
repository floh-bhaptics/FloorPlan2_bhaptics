using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;

using UnityEngine;
using TButt;

[assembly: MelonInfo(typeof(FloorPlan2_bhaptics.FloorPlan2_bhaptics), "FloorPlan2_bhaptics", "1.1", "Florian Fahrenberger")]
[assembly: MelonGame("Turbo Button", "Floor Plan 2")]


namespace FloorPlan2_bhaptics
{
    public class FloorPlan2_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool rightGrab = false;
        public static bool leftGrab = false;
        public static bool lastGrabbedRight = true;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        #region World interaction

        [HarmonyPatch(typeof(TButt.Locomotion.TBTeleportManager), "TeleportToPoint", new Type[] { typeof(TButt.Locomotion.TBTeleportPoint), typeof(bool) })]
        public class bhaptics_Teleport
        {
            [HarmonyPostfix]
            public static void Postfix(bool instantly)
            {
                if (!instantly) tactsuitVr.PlaybackHaptics("TeleportThrough");
            }
        }


        [HarmonyPatch(typeof(Elevator), "DepartedFloor", new Type[] { typeof(Floor) })]
        public class bhaptics_MoveElevator
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ElevatorTingle");
            }
        }

        [HarmonyPatch(typeof(PantsWaist), "SetGrabbingHand", new Type[] { typeof(FloorPlanHand) })]
        public class bhaptics_GrabPouch
        {
            [HarmonyPostfix]
            public static void Postfix(PantsWaist __instance)
            {
                //tactsuitVr.LOG("Open pouch");
                if (__instance.IsGrabbed) tactsuitVr.StartPouch();
                else tactsuitVr.StopPouch();
            }
        }

        #endregion

        #region Hand animation

        [HarmonyPatch(typeof(ButtHandSkinData), "PlayFart", new Type[] { typeof(TBInput.Controller) })]
        public class bhaptics_PlayFart
        {
            [HarmonyPostfix]
            public static void Postfix(TBInput.Controller controller)
            {
                bool isRightHand = false;
                if (controller == TBInput.Controller.RHandController) isRightHand = true;
                tactsuitVr.HandEffect("Fart", isRightHand);
            }
        }

        [HarmonyPatch(typeof(PlayerHandSelfActions), "Clap", new Type[] { typeof(Vector3) })]
        public class bhaptics_HandClap
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if (hand == HandSkinData.HandID.Right) tactsuitVr.LOG("Grab right");
                //else tactsuitVr.LOG("Grab left");
                tactsuitVr.PlaybackHaptics("ClapHands");
                tactsuitVr.PlaybackHaptics("ClapArms");
            }
        }

        [HarmonyPatch(typeof(PlayerHandSelfActions), "Smack", new Type[] { typeof(TBInput.Controller), typeof(Vector3) })]
        public class bhaptics_Smack
        {
            [HarmonyPostfix]
            public static void Postfix(TBInput.Controller controller)
            {
                bool isRightHand = false;
                if (controller == TBInput.Controller.RHandController) isRightHand = true;
                tactsuitVr.HandEffect("Smack", isRightHand);
            }
        }

        #endregion

        #region Thumbsup and -down

        public static bool isRightHandGrabbing()
        {
            //tactsuitVr.LOG(" " + rightGrab.ToString() + leftGrab.ToString());
            if (rightGrab) return true;
            if (leftGrab) return false;
            return lastGrabbedRight;
        }

        public static void releaseGrabs()
        {
            lastGrabbedRight = isRightHandGrabbing();
            rightGrab = false;
            leftGrab = false;
        }

        [HarmonyPatch(typeof(PlayerHandSkinManager), "GrabButtonDown", new Type[] { typeof(TBInput.Controller), typeof(Transform) })]
        public class bhaptics_HandGrabDown
        {
            [HarmonyPostfix]
            public static void Postfix(TBInput.Controller controller)
            {
                try
                {
                    if (controller == TBInput.Controller.RHandController) { rightGrab = true; leftGrab = false; }
                    else { leftGrab = true; rightGrab = false; }
                }
                catch { }
            }
        }

        [HarmonyPatch(typeof(TextBox), "ChooseYesOrNo", new Type[] { typeof(bool) })]
        public class bhaptics_ChooseYesOrNo
        {
            [HarmonyPostfix]
            public static bool Prefix(bool yes)
            {
                if (yes) tactsuitVr.HandEffect("ThumbsUp", isRightHandGrabbing());
                else tactsuitVr.HandEffect("ThumbsDown", isRightHandGrabbing());
                releaseGrabs();
                return true;
            }
        }

        [HarmonyPatch(typeof(TextBox), "ChooseContinue", new Type[] {  })]
        public class bhaptics_ChooseContinue
        {
            [HarmonyPostfix]
            public static bool Prefix()
            {
                //tactsuitVr.LOG("Continue");
                tactsuitVr.HandEffect("ThumbsUp", isRightHandGrabbing());
                releaseGrabs();
                return true;
            }
        }

        [HarmonyPatch(typeof(TextBox), "Initialize", new Type[] { })]
        public class bhaptics_InitializeBox
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                releaseGrabs();
            }
        }

        #endregion

    }
}
