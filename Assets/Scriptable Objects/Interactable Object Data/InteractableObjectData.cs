using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Interactable Object", menuName = "Custom/Interactable Object Data")]
public class InteractableObjectData : ScriptableObject
{
    [SerializeField]
    private string m_DisplayName;
    public string DisplayName
    {
        get { return m_DisplayName; }
    }

    [SerializeField]
    private Sprite m_DefaultSprite;
    public Sprite DefaultSprite
    {
        get { return m_DefaultSprite; }
    }

    [SerializeField]
    private Sprite m_InteractSprite;
    public Sprite InteractSprite
    {
        get { return m_InteractSprite; }
    }

    [SerializeField]
    private Interaction m_Interaction;
    public Interaction Interaction
    {
        get { return m_Interaction; }
        set { m_Interaction = value; }
    }

    public void InitializeButton(Button button)
    {
        SpriteState spriteState = button.spriteState;

        if (m_DefaultSprite!= null)
        {
            spriteState.disabledSprite = m_DefaultSprite;
            spriteState.highlightedSprite = m_DefaultSprite;

            button.image.sprite = m_DefaultSprite;
        }

        if (m_InteractSprite != null)
        {
            spriteState.pressedSprite = m_InteractSprite;
        }

        button.spriteState = spriteState;
    }

    public void Interact(LocationManager locationManager)
    {
        if (m_Interaction != null)
            m_Interaction.Interact(locationManager);
    }

    public void Serialize(JSONClass rootClass)
    {
        rootClass.Add("name", new JSONData(m_DisplayName));
        rootClass.Add("default_sprite", new JSONData(m_DefaultSprite.name)); //Change
        rootClass.Add("interact_sprite", new JSONData(m_InteractSprite.name));
        rootClass.Add("interaction", new JSONData(m_Interaction.name));
    }

    public void Deserialize()
    {

    }
}
