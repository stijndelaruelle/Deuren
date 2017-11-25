using UnityEngine;

[CreateAssetMenu(fileName = "New Location Interaction", menuName = "Custom/Interaction/Location Warp Interaction")]
public class LocationWarpInteraction : Interaction
{
    [SerializeField]
    private LocationData m_Location;
    private LocationManager m_LocationManager;

    [SerializeField]
    private AudioClip m_AudioClip;

    public override bool Interact(InteractableObject thisObject, InteractableObject otherObject)
    {
        //If we have no location data, there will be a new one created/found when we load
        //Therefore the next location we go to is the one that has to become linked.
        if (m_Location == null)
        {
            m_LocationManager = thisObject.LocationManager;

            //Make sure we are unsubscribed before subscribing again
            m_LocationManager.LoadLocationEvent -= OnLoadLocation;
            m_LocationManager.LoadLocationEvent += OnLoadLocation;
        }

        thisObject.LocationManager.LoadLocation(m_Location);
        thisObject.PlaySound(m_AudioClip);

        return true;
    }

    private void OnDestroy()
    {
        if (m_LocationManager != null)
            m_LocationManager.LoadLocationEvent -= OnLoadLocation;
    }

    private void OnLoadLocation(LocationData data)
    {
        m_Location = data;
        m_LocationManager.LoadLocationEvent -= OnLoadLocation;
        m_LocationManager = null;
    }
}
