using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void TooltipDelegate(string text, float duration, Vector2 position);

[RequireComponent(typeof(RectTransform))]
public class TooltipTextUI : MonoBehaviour
{
    [SerializeField]
    private LocationManager m_LocationManager;

    [SerializeField]
    private Text m_Text;

    [Header("Options")]
    [SerializeField]
    private float m_Offset;

    [SerializeField]
    private float m_FadeTime;

    private Sequence m_CurrentSequence;

    private RectTransform m_RectTransform;
    private List<InteractableObject> m_ListeningObjects; //Object's i'm currently listening to


    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_ListeningObjects = new List<InteractableObject>();
    }

    private void Start()
    {
        m_LocationManager.StartLoadLocationEvent += OnStartLoadLocation;
        m_LocationManager.LoadInteractableObjectEvent += OnLoadInteractableObject;
    }

    private void OnDestroy()
    {
        StopListeningToAll();

        if (m_LocationManager != null)
        {
            m_LocationManager.StartLoadLocationEvent -= OnStartLoadLocation;
            m_LocationManager.LoadInteractableObjectEvent -= OnLoadInteractableObject;
        }
    }

    //Callbacks from LocationManager
    private void OnStartLoadLocation(LocationData locationData)
    {
        StopListeningToAll();
    }

    private void OnLoadInteractableObject(InteractableObject interactableObject)
    {
        if (m_ListeningObjects.Contains(interactableObject))
            return;

        m_ListeningObjects.Add(interactableObject);
        interactableObject.TooltipEvent += OnTooltip;
    }

    //Callbacks from InteractableObjects
    private void OnTooltip(string text, float duration, Vector2 position)
    {
        m_Text.text = text;
        m_Text.enabled = true;

        //Create a new sequence
        float offset = m_Offset * ((float)Screen.height / 1080.0f);
        m_RectTransform.position = new Vector3(position.x, position.y + offset);

        if (m_CurrentSequence != null)
            m_CurrentSequence.Kill();

        m_CurrentSequence = DOTween.Sequence();

        //Move & fade in
        m_CurrentSequence.Insert(0.0f, m_RectTransform.DOMoveY(position.y, m_FadeTime));
        m_CurrentSequence.Insert(0.0f, m_Text.DOFade(1.0f, m_FadeTime));

        //Wait
        m_CurrentSequence.AppendInterval(duration);

        //Move & fade out
        m_CurrentSequence.Insert(duration + m_FadeTime, m_RectTransform.DOMoveY(position.y - offset, m_FadeTime));
        m_CurrentSequence.Insert(duration + m_FadeTime, m_Text.DOFade(0.0f, m_FadeTime));

        //Callback
        m_CurrentSequence.AppendCallback(OnFadeComplete);

        m_CurrentSequence.PlayForward();
    }

    private void OnFadeComplete()
    {
        m_Text.enabled = false;
    }

    private void StopListeningToAll()
    {
        if (m_ListeningObjects != null)
        {
            foreach (InteractableObject obj in m_ListeningObjects)
            {
                obj.TooltipEvent -= OnTooltip;
            }
        }

        m_ListeningObjects.Clear();
    }
}
