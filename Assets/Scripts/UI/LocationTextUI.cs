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

        //Set position
        Vector3 newPosition = m_Text.transform.localPosition;
        newPosition.y = 100.0f;
        m_Text.transform.localPosition = newPosition;

        //Set scale
        Vector3 newScale = m_Text.transform.localScale;
        newScale.x = 0.0f;
        m_Line.transform.localScale = newScale;
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
