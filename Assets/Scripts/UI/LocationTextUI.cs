using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LocationTextUI : MonoBehaviour
{
    [SerializeField]
    private LocationManager m_LocationManager;

    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private RectTransform m_Line;

    private void Start()
    {
        m_LocationManager.StartLoadLocationEvent += OnStartLoadLocation;
        m_LocationManager.LoadLocationEvent += OnLoadLocation;
    }

    private void OnDestroy()
    {
        if (m_LocationManager != null)
        {
            m_LocationManager.StartLoadLocationEvent -= OnStartLoadLocation;
            m_LocationManager.LoadLocationEvent -= OnLoadLocation;
        }
    }

    private void OnStartLoadLocation(LocationData locationData)
    {
        m_Text.transform.DOLocalMoveY(100.0f, 1.0f);
        m_Line.DOScaleX(0.0f, 1.0f);
    }

    private void OnLoadLocation(LocationData locationData)
    {
        m_Text.text = locationData.DisplayName;

        m_Text.transform.DOLocalMoveY(-75.0f, 1.0f);
        m_Line.DOScaleX(1.0f, 1.0f);
    }
}
