using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Text))]
public class LocationTextUI : MonoBehaviour
{
    [SerializeField]
    private LocationManager m_LocationManager;
    private Text m_Text;

    private void Awake()
    {
        m_Text = GetComponent<Text>();    
    }

    private void Start()
    {
        m_LocationManager.LoadLocationEvent += OnLocationLoaded;
    }

    private void OnDestroy()
    {
        if (m_LocationManager != null)
            m_LocationManager.LoadLocationEvent -= OnLocationLoaded;
    }

    private void OnLocationLoaded(LocationData locationData)
    {
        m_Text.text = locationData.DisplayName;
    }
}
