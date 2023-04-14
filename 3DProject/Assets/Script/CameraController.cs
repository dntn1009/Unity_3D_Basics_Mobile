using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]//에디트모드에서시작 // 시작버튼 안누러도 확인가능
public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform m_target; //타겟 지정
    [SerializeField]
    [Range(0f, 30f)]
    float m_distance = 5f;//카메라가 확대할수 있는 거리 조절
    // RANGE릉 이용
    [SerializeField]
    [Range(0f, 30f)]
    float m_height = 3f;
    [SerializeField]
    [Range(-90f, 90f)]
    float m_angel = 45f;
    [SerializeField]
    [Range(0.1f, 5f)] // 카메라 속도
    float m_speed = 0.1f;
    Transform m_prevTransform;//화면이 이동할떄 프레임 고려
    // Start is called before the first frame update
    void Start()
    {
        m_prevTransform = transform;
        Application.targetFrameRate = 60;// 프레임 고정, 상관없음 게임설정으로 만들수잇음
        //Screen.SetResolution(Mathf.RoundToInt(Screen.width * 0.8f), Mathf.RoundToInt(Screen.height * 0.8f), true); //핸드폰의 해상도에 따라서 (기기에맞춰) 알아서 되는데 해상도를 변경할 함수임
        //근데 모바일은 30 or 60 DB로 설정하게 하자
        //  QHD 말고 풀HD로 바뀜
        // 가변 해상도 적용해보기
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_target.transform.position.x,
            Mathf.Lerp(m_prevTransform.position.y, m_target.transform.position.y + m_height, m_speed * Time.deltaTime),
            Mathf.Lerp(m_prevTransform.position.z, m_target.transform.position.z - m_distance, m_speed * Time.deltaTime));
        transform.rotation = Quaternion.Lerp(m_prevTransform.rotation, Quaternion.Euler(m_angel, 0f, 0f), m_speed * Time.deltaTime);
        m_prevTransform = transform;
    }
}
