using UnityEngine;

[CreateAssetMenu(fileName = "New Location Interaction", menuName = "Custom/Interaction/Location Warp Interaction")]
public class LocationWarpInteraction : Interaction
{
    [SerializeField]
    private LocationData m_Location;
    private LocationManager m_LocationManager;

    public override void Interact(LocationManager locationManager)
    {
        //If we have no location data, there will be a new one created/found when we load
        //Therefore the next location we go to is the one that has to become linked.
        if (m_Location == null)
        {
            m_LocationManager = locationManager;

            //Make sure we are unsubscribed before subscribing again
            m_LocationManager.LoadLocationEvent -= OnLoadLocation;
            m_LocationManager.LoadLocationEvent += OnLoadLocation;
        }

        locationManager.LoadLocation(m_Location);
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
