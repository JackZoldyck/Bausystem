using UnityEngine;

public class BuildableObject : MonoBehaviour
{
    public SnapPoint[] snapPoints;
    public Transform bottomSnap;
    public Collider placementCheckCollider;
    private void Awake()
    {
        snapPoints = GetComponentsInChildren<SnapPoint>();
    }





}