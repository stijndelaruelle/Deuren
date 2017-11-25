using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Destroy Interaction", menuName = "Custom/Interaction/Destroy Interaction")]
public class DestroyInteraction : Interaction
{
    [SerializeField]
    private bool m_DestroyThisObject;

    [SerializeField]
    private bool m_DestroyOtherObject;

    public override bool Interact(InteractableObject thisObject, InteractableObject otherObject)
    {
        //Alter the data
        if (m_DestroyThisObject == true)
        {
            thisObject.Deactivate();
        }

        if (m_DestroyOtherObject == true)
        {
            otherObject.Deactivate();
        }

        return true;
    }
}
