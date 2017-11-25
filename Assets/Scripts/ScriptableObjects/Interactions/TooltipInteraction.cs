using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tooltip Interaction", menuName = "Custom/Interaction/Tooltip Interaction")]
public class TooltipInteraction : Interaction
{
    [SerializeField]
    private string m_Text;

    [SerializeField]
    private float m_Duration;

    public override bool Interact(InteractableObject thisObject, InteractableObject otherObject)
    {
        string display = "";

        if (otherObject == null)
            display = string.Format(m_Text, thisObject.Data.DisplayName);
        else
            display = string.Format(m_Text, thisObject.Data.DisplayName, otherObject.Data.DisplayName);

        //Debug.Log(display);
        thisObject.SetTooltip(display, m_Duration);

        return false;
    }
}
