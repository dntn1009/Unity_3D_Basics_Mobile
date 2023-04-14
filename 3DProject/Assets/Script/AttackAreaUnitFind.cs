using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaUnitFind : MonoBehaviour
{
    List<GameObject> m_unitList = new List<GameObject>();
    public List<GameObject> UnitList {  get { return m_unitList; } }
    GameObject GetNearestUnit(Transform dest)
    {
        if (UnitList == null || UnitList.Count == 0) return null;
        float neardist = (dest.position - UnitList[0].transform.position).sqrMagnitude;
        float curDist = 0f;
        int index = 0;
        for(int i  = 1; i < UnitList.Count; i++)
        {
            curDist = (dest.position - UnitList[0].transform.position).sqrMagnitude;
            if(neardist > curDist)
            {
                neardist = curDist;
                index = 1;
            }
        }
        return UnitList[index];
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            m_unitList.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            m_unitList.Remove(other.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
