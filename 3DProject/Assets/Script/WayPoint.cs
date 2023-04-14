using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public Color Color { get; set; }
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
