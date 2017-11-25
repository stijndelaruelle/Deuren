using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Change Data Interaction", menuName = "Custom/Interaction/Change Data Interaction")]
public class ChangeDataInteraction : Interaction
{
    [Header("New data for this object")]
    [SerializeField]
    private InteractableObjectData m_NewDataThis;

    [Header("New data for other object")]
    [SerializeField]
    private InteractableObjectData m_NewDataOther;

    public override bool Interact(InteractableObject thisObject, InteractableObject otherObject)
    {
        //Alter the data
        if (m_NewDataThis != null)
        {
            thisObject.SetData(m_NewDataThis);
        }

        if (m_NewDataOther != null)
        {
            otherObject.SetData(m_NewDataOther);
        }

        return true;
    }
}
