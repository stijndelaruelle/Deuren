using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Location Interaction", menuName = "Custom/Interaction/Location Warp Interaction")]
public class LocationWarpInteraction : Interaction
{
    [SerializeField]
    private LocationData m_Location;

    public override void Interact(LocationManager locationManager)
    {
        locationManager.LoadLocation(m_Location);
    }
}
