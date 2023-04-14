using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : class
{
    //스택은 후입선출 이므로 넣은걸 바로 쓰고 바로 쓰고 하는 형식
    //그러면 방향에 안맞기때문에 큐로 사용

    //QUEUE
    //오브젝트들을 메모리상으로 가지고만 있으면 됌
    public delegate T CreateFunc();

    Queue<T> m_objectQueue = new Queue<T>();
    int m_count;
    CreateFunc m_func;
    
    public GameObjectPool(int count, CreateFunc func)
    {
        m_count = count;
        m_func = func;
        Allocate();
    }
    
    void Allocate() //Queue에 넣는 과정
    {
        for(int i = 0; i < m_count; i++)
        {
            m_objectQueue.Enqueue(m_func());
        }// 델리게이트 이용해서 변환된 T형을 큐에 넣어줌
    }
    public T Get()// Dequeque
    {
        if (m_objectQueue.Count > 0)
            return m_objectQueue.Dequeue();
        else
        {
            return m_func();
        }//원하는 카운트에 값이 없을 경우 만들어줘서 쓴다.
    }
    public void Set(T obj)
    {
        m_objectQueue.Enqueue(obj);
    }
}
