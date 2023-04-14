using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : SingletonMonobehaviour<T>// 상속 받은애만 T로 만드려고 상속받은 클래스만 제한받게 where로 만듬.
{
    public static T Instance { get; private set; }

    virtual protected void OnAwake()
    {

    }
    virtual protected void OnStart()
    {

    }
    void Awake()
    {
        if(Instance == null)
        {
            Instance = (T)this;//부모는 자식의 메모리 접근을 가능하기떄문에 이걸로 형변환 가능/.
            OnAwake();
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(Instance == (T)this)
        {
            OnStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
