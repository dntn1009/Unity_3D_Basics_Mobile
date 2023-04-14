using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointSystem : MonoBehaviour
{
    public WayPoint[] m_waypoints;
    [SerializeField]
    Color m_waypointColor = Color.yellow;

    void OnDrawGizmos()
    {
        m_waypoints = GetComponentsInChildren<WayPoint>();
        if (m_waypoints == null || m_waypoints.Length <= 1) return;
        for (int i = 0; i < m_waypoints.Length - 1; i++)
        {
            m_waypoints[i].Color = m_waypointColor;
            Gizmos.DrawLine(m_waypoints[i].transform.position, m_waypoints[i + 1].transform.position);
        }

    }
}
