using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    UILabel m_name;
    [SerializeField]
    UIProgressBar m_hpBar;
    [SerializeField]
    HUDText[] m_hudTexts;
    [SerializeField]
    UIFollowTarget m_followTarget;
    StringBuilder m_sb = new StringBuilder();

    public void SetHud(Camera uiCamera, Transform hudPool)
    {
        m_hpBar.value = 1f;
        m_followTarget.gameCamera = Camera.main;
        m_followTarget.uiCamera = uiCamera;
        transform.SetParent(hudPool);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void InitHud()
    {
        m_hpBar.value = 1f;
        m_hpBar.alpha = 1f;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void ActiveUI()
    {
        Show();
        if (IsInvoking("Hide"))
            CancelInvoke("Hide");
        Invoke("Hide", 3f);

    }
    public void DisplayDamage(AttackType type, float damage, float normalizedHP)
    {
        m_sb.Append(damage);
        switch (type)
        {
            case AttackType.Dodge:
                m_hudTexts[2].Add("Miss", Color.yellow, 1f);
                break;
            case AttackType.Normal:
                m_hudTexts[0].Add(m_sb.ToString(), Color.white, 0f);
                break;
            case AttackType.Critical:
                m_hudTexts[1].Add(m_sb.ToString(), Color.red, 0f);
                break;
        }
        m_sb.Clear();
        m_hpBar.value = normalizedHP;
        if (normalizedHP <= 0f)
            m_hpBar.alpha = 0f;
    }
    // Start is called before the first frame update
    void Awake()
    {
        
    }

}
