using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Interactable Object", menuName = "Custom/Interactable Object Data")]
public class InteractableObjectData : ScriptableObject
{
    [Multiline]
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

    [SerializeField]
    private Interaction m_DropInteraction;
    public Interaction DropInteraction
    {
        get { return m_DropInteraction; }
        set { m_DropInteraction = value; }
    }

    [SerializeField]
    private bool m_AllowDragging = true;
    public bool AllowDragging
    {
        get { return m_AllowDragging; }
    }


    public bool Interact(InteractableObject thisObject)
    {
        if (m_Interaction != null)
            return m_Interaction.Interact(thisObject, null);

        return false;
    }

    public bool DropInteract(InteractableObject thisObject, InteractableObject otherObject)
    {
        if (m_DropInteraction != null)
            return m_DropInteraction.Interact(thisObject, otherObject);

        return false;
    }

    public void Serialize(JSONClass rootClass)
    {
        rootClass.Add("name", new JSONData(m_DisplayName));
        rootClass.Add("default_sprite", new JSONData(m_DefaultSprite.name)); //Change
        rootClass.Add("interact_sprite", new JSONData(m_InteractSprite.name));

        rootClass.Add("interaction", new JSONData(m_Interaction.name));
        rootClass.Add("drop_interaction", new JSONData(m_DropInteraction.name));
    }

    public void Deserialize()
    {

    }
}
