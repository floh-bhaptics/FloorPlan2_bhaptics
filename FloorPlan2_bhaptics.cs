using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;

namespace FloorPlan2_bhaptics
{
    public class FloorPlan2_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
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

        [HarmonyPatch(typeof(PantsWaist), "HandGrabbed", new Type[] { typeof(FloorPlanHand) })]
        public class bhaptics_GrabPouch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("Open pouch");
                tactsuitVr.StartPouch();
            }
        }

        [HarmonyPatch(typeof(PantsWaist), "HandReleased", new Type[] { typeof(FloorPlanHand) })]
        public class bhaptics_ReleasePouch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("Close pouch");
                tactsuitVr.StopPouch();
            }
        }

        
        [HarmonyPatch(typeof(ButtHandSkinData), "PlayFart", new Type[] { typeof(TButt.TBInput.Controller) })]
        public class bhaptics_PlayFart
        {
            [HarmonyPostfix]
            public static void Postfix(TButt.TBInput.Controller controller)
            {
                bool isRightHand = false;
                if (controller == TButt.TBInput.Controller.RHandController) isRightHand = true;
                tactsuitVr.HandEffect("Fart", isRightHand);
            }
        }

        [HarmonyPatch(typeof(HandSkinData), "PlayGrabSound", new Type[] { typeof(UnityEngine.Transform), typeof(HandSkinData.HandID) })]
        public class bhaptics_HandGrab
        {
            [HarmonyPostfix]
            public static void Postfix(HandSkinData.HandID hand)
            {
                //if (hand == HandSkinData.HandID.Right) tactsuitVr.LOG("Grab right");
                //else tactsuitVr.LOG("Grab left");
            }
        }
        
        [HarmonyPatch(typeof(PlayerHandSelfActions), "Clap", new Type[] { typeof(UnityEngine.Vector3) })]
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
        
        [HarmonyPatch(typeof(PlayerHandSelfActions), "ThumbsUp", new Type[] { typeof(TButt.TBInput.Controller) })]
        public class bhaptics_ThumbsUp
        {
            [HarmonyPostfix]
            public static void Postfix(TButt.TBInput.Controller controller)
            {
                tactsuitVr.LOG("ThumbsyUppy");
                bool isRightHand = false;
                if (controller == TButt.TBInput.Controller.RHandController) isRightHand = true;
                tactsuitVr.HandEffect("ThumbsUp", isRightHand);
            }
        }

        [HarmonyPatch(typeof(PlayerHandSelfActions), "ThumbsDown", new Type[] { typeof(TButt.TBInput.Controller) })]
        public class bhaptics_ThumbsDown
        {
            [HarmonyPostfix]
            public static void Postfix(TButt.TBInput.Controller controller)
            {
                bool isRightHand = false;
                if (controller == TButt.TBInput.Controller.RHandController) isRightHand = true;
                tactsuitVr.HandEffect("ThumbsDown", isRightHand);
            }
        }

        [HarmonyPatch(typeof(PlayerHandSelfActions), "Smack", new Type[] { typeof(TButt.TBInput.Controller), typeof(UnityEngine.Vector3) })]
        public class bhaptics_Smack
        {
            [HarmonyPostfix]
            public static void Postfix(TButt.TBInput.Controller controller)
            {
                bool isRightHand = false;
                if (controller == TButt.TBInput.Controller.RHandController) isRightHand = true;
                tactsuitVr.HandEffect("Smack", isRightHand);
            }
        }


    }
}
