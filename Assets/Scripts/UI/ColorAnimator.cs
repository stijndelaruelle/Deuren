using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorAnimator : MonoBehaviour
{
    public delegate void FadeDelegate();

    [Header("Object to fade (Image or Material)")]
    [Space(5)]
    [SerializeField]
    private MaskableGraphic m_Image;

    [SerializeField]
    private Material m_Material;

    [Header("Properties")]
    [Space(5)]
    [SerializeField]
    private float m_Speed;
    public float Speed
    {
        get { return m_Speed; }
    }

    [SerializeField]
    private Gradient m_Gradient;
    private float m_CurrentPoint = 0.0f;
    private Coroutine m_FadeRoutine;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_StartPoint = 0.0f;

    private void Awake()
    {
        SetColor(m_StartPoint);
        FadeOut();
    }

    public void SetColor(float point)
    {
        m_CurrentPoint = point;

        Color color = m_Gradient.Evaluate(point);

        if (m_Image != null)
            m_Image.color = color;

        if (m_Material != null)
            m_Material.color = color;
    }

    public void FadeOut()
    {
        FadeOut(null);
    }

    public void FadeOut(FadeDelegate callback)
    {
        StartFading(callback, 0.0f);
    }

    public void FadeIn()
    {
        FadeIn(null);
    }

    public void FadeIn(FadeDelegate callback)
    {
        StartFading(callback, 1.0f);
    }


    private void StartFading(FadeDelegate callback, float targetPoint)
    {
        if (m_FadeRoutine != null)
            StopCoroutine(m_FadeRoutine);
        
        m_FadeRoutine = StartCoroutine(FadeRoutine(callback, targetPoint));
    }

    private IEnumerator FadeRoutine(FadeDelegate callback, float targetPoint)
    {
        while (m_CurrentPoint != targetPoint)
        {
            float prevSign = Mathf.Sign(targetPoint - m_CurrentPoint);

            //Change the color
            m_CurrentPoint += prevSign * m_Speed * Time.fixedDeltaTime; //Ignores deltaTime

            float afterSign = Mathf.Sign(targetPoint - m_CurrentPoint);

            //We flipped
            if (prevSign != afterSign)
            {
                m_CurrentPoint = targetPoint;
            }

            SetColor(m_CurrentPoint);
            yield return new WaitForEndOfFrame();
        }

        m_FadeRoutine = null;

        if (callback != null)
            callback();
    }
}
