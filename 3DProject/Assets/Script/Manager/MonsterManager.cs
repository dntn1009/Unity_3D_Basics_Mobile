using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonMonobehaviour<MonsterManager>
{
    GameObject m_monsterPrefab;
    GameObjectPool<MonsterController> m_monsterPool;
    [SerializeField]
    Camera m_uiCamera;
    [SerializeField]
    Transform m_hudPool;
    [SerializeField]
    WayPointSystem m_path;
    public void CreateMonster()
    {
        var mon = m_monsterPool.Get();
        mon.InitMonster(m_path);
    }

    public void RemoveMonster(MonsterController mon)
    {
        mon.gameObject.SetActive(false);
        m_monsterPool.Set(mon);
    }

    protected override void OnStart()
    {
        m_monsterPrefab = Resources.Load<GameObject>("Prefab/Monsters/Monster");
        m_monsterPool = new GameObjectPool<MonsterController>(5, () =>
        {
            var obj = Instantiate(m_monsterPrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            var mon = obj.GetComponent<MonsterController>();
            mon.SetMonster(m_uiCamera, m_hudPool);
            return mon;
        });
    }

    public void OnPressMonsterCreate()
    {
        CreateMonster();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateMonster();
        }
    }
}
