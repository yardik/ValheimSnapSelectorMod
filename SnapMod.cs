using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace YardiksValheimMod
{
    [BepInPlugin("yardik.SnapMod", "Yardiks Valheim SnapMod", "1.0.0")]
    public class SnapMod : BaseUnityPlugin
    {
        private static SnapMod context;
        public static ConfigEntry<bool> modEnabled;

        private void Awake()
        {
            context = this;
            modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
            
            if (!modEnabled.Value)
                return;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        [HarmonyPatch(typeof(Player))]
        public class HookPieceRayTest
        {
            [HarmonyReversePatch]
            [HarmonyPatch(typeof(Player), "PieceRayTest")]
            public static bool call_PieceRayTest(object instance, out Vector3 point, out Vector3 normal,
                out Piece piece, out Heightmap heightmap, out Collider waterSurface, bool water) =>
                throw new NotImplementedException();
        }

        [HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
        static class UpdatePlacementGhost_Patch
        {
            static bool modifiedPlacementToggled = false;
            private static int currentSourceSnap = -1;
            private static int currentDestinationSnap = -1;
            private static Transform currentDestinationParent;
            private static Transform currentSourceParent;

            static void Postfix(Player __instance,
                GameObject ___m_placementGhost)
            {
                if (Input.GetKeyDown(ValheimSnapMod.Settings.SnapSettings.EnableModKey.Value))
                {
                    modifiedPlacementToggled = !modifiedPlacementToggled;
                }

                if (___m_placementGhost == null)
                    return;

                var sourcePiece = ___m_placementGhost.GetComponent<Piece>();
                if (sourcePiece == null)
                    return;

                Piece targetPiece = null;
                if (!(targetPiece = RayTest(__instance, ___m_placementGhost))) return;
                if (targetPiece == null)
                    return;

                if (modifiedPlacementToggled)
                {
                    if (currentDestinationParent != targetPiece.transform)
                    {
                        currentDestinationSnap = 0;
                        currentDestinationParent = targetPiece.transform;
                    }

                    if (currentSourceParent != sourcePiece.transform)
                    {
                        currentSourceSnap = 0;
                        currentSourceParent = sourcePiece.transform;
                    }

                    if (Input.GetKeyDown(ValheimSnapMod.Settings.SnapSettings.IterateSourceSnapPoints.Value))
                    {
                        currentSourceSnap++;
                    }

                    if (Input.GetKeyDown(ValheimSnapMod.Settings.SnapSettings.IterateDestinationSnapPoints.Value))
                    {
                        currentDestinationSnap++;
                    }

                    var sourceSnapPoints = GetSnapPoints(sourcePiece.transform);
                    var destSnapPoints = GetSnapPoints(currentDestinationParent);

                    if (currentSourceSnap >= sourceSnapPoints.Count)
                        currentSourceSnap = 0;
                    if (currentDestinationSnap >= destSnapPoints.Count)
                        currentDestinationSnap = 0;

                    var a = sourceSnapPoints[currentSourceSnap];
                    var b = destSnapPoints[currentDestinationSnap];
                    var position = b.position;
                    var p = b.position - (a.position - ___m_placementGhost.transform.position);
                    ___m_placementGhost.transform.position = p;
                }
            }

            private static Piece RayTest(Player player, GameObject placementGhost)
            {
                var component1 = placementGhost.GetComponent<Piece>();
                var water = component1.m_waterPiece || component1.m_noInWater;
                Vector3 point;
                Vector3 normal;
                Piece piece;
                Heightmap heightmap;
                Collider waterSurface;
                HookPieceRayTest.call_PieceRayTest(player, out point, out normal, out piece, out heightmap,
                    out waterSurface, water);
                return piece;
            }

            public static List<Transform> GetSnapPoints(Transform piece)
            {
                List<Transform> points = new List<Transform>();
                if (piece == null) return points;

                for (var index = 0; index < piece.childCount; ++index)
                {
                    var child = piece.GetChild(index);
                    if (child.CompareTag("snappoint"))
                        points.Add(child);
                }

                points.Add(piece.transform);
                return points;
            }
        }
    }
}