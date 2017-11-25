using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureRecognizer;

public class CreateInteractableObjectFromDrawing : MonoBehaviour
{
    [SerializeField]
    private LocationManager m_LocationManager;

    [SerializeField]
    private DrawDetector m_DrawDetector;

    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private InteractableObjectData m_BaseInteractableObjectData;

    [SerializeField]
    private List<GesturePattern> m_GesturePatterns;

    private void Awake()
    {
        m_DrawDetector.OnRecognize.AddListener(OnRecognize);
    }

    private void OnRecognize(RecognitionResult result)
    {
        foreach(GesturePattern pattern in m_GesturePatterns)
        {
            if (result.gesture == pattern)
            {
                //Create a new Interactable object!
                Debug.Log("Create a new " + result.gesture.name + " " + result.score.score);

                InteractableObjectData newData = ScriptableObject.Instantiate(m_BaseInteractableObjectData);
                newData.Interaction = ScriptableObject.Instantiate(newData.Interaction); //Create a copy of it's interaction scriptable object

                Vector2 localPosition = result.gestureData.GetCenter() - (Vector2)m_LocationManager.InteractableObjectRoot.position;
                m_LocationManager.CreateInteractableObject(localPosition, newData);

                m_AudioSource.Play();

                return;
            }
        }
    }
}
