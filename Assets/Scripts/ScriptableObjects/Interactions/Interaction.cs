using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : ScriptableObject
{
    public abstract bool Interact(InteractableObject thisObject, InteractableObject otherObject);
}
