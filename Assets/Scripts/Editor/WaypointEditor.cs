using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class WaypointEditor
{
    [MenuItem("Tools/Set Waypoint Neighbors")]
    private static void SetWaypointNeighbors()
    {
        var allWaypoints = Object.FindObjectsOfType<Waypoint>();
        var neighbors = new List<Waypoint>[allWaypoints.Length];
        var mask = LayerMask.GetMask("Wall");
        for (var i = 0; i < allWaypoints.Length; i++)
        {
            neighbors[i] = new List<Waypoint>();
        }

        for (var i = 0; i < allWaypoints.Length; i++)
        {
            var currentWaypoint = allWaypoints[i];
            for (var j = i + 1; j < allWaypoints.Length; j++)
            {
                var otherWaypoint = allWaypoints[j];
                if (!HasWallBetween(currentWaypoint, otherWaypoint, mask))
                {
                    neighbors[i].Add(otherWaypoint);
                    neighbors[j].Add(currentWaypoint);
                }
            }
        }

        for (var i = 0; i < allWaypoints.Length; i++)
        {
            allWaypoints[i].Neighbors = neighbors[i].ToArray();
            EditorUtility.SetDirty(allWaypoints[i]);
        }
    }

    private static bool HasWallBetween(Waypoint waypointA, Waypoint waypointB, LayerMask mask)
    {
        var posA = waypointA.transform.position;
        posA.y = 0.5f;
        var posB = waypointB.transform.position;
        posB.y = 0.5f;
        return Physics.Linecast(posA, posB, mask);
    }
}