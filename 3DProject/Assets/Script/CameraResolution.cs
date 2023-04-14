using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    Camera m_camera;
    [SerializeField]
    RenderTexture m_screenTex;
    private void OnPreCull()
    {
        m_camera.targetTexture = m_screenTex;
    }//카메라 불러오기전(렌더링하고있는상황)

    private void OnRenderObject()
    {
        m_camera.targetTexture = null;
        Graphics.Blit(m_screenTex, null as RenderTexture);
    }//렌더링한 후

    private void OnPostRender()
    {
    }//그리기가 끝난 상황(렌더링이 끝)


    // Start is called before the first frame update
    void Awake()
    {
        m_camera = GetComponent<Camera>();
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
