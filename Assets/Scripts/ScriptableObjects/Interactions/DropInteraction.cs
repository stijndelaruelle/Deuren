using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drop Interaction", menuName = "Custom/Interaction/Drop Interaction")]
public class DropInteraction : Interaction
{
    [SerializeField]
    private string m_RequiredName;

    [SerializeField]
    private List<Interaction> m_SuccessInteractions;

    [SerializeField]
    private List<Interaction> m_FailInteractions;

    public override bool Interact(InteractableObject thisObject, InteractableObject otherObject)
    {
        if (otherObject.Data.DisplayName == m_RequiredName)
        {
            foreach(Interaction interaction in m_SuccessInteractions)
            {
                interaction.Interact(thisObject, otherObject);
            }    
        }
        else
        {
            foreach (Interaction interaction in m_FailInteractions)
            {
                interaction.Interact(thisObject, otherObject);
            }
        }

        return true;
    }
}
