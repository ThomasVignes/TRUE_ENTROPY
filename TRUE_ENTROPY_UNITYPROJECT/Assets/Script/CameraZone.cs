using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Behaviour
{
    Classic,
    Path
}

public enum Template
{
    None,
    Fixed,
    FixedAiming,
    Following,
    PathAiming
}

public class CameraZone : MonoBehaviour
{
    public string Ambiance;
    [SerializeField] Template Template;
    [Header("Custom camera")]
    [SerializeField] Behaviour Behaviour;
    [SerializeField] bool Follow;
    [SerializeField] bool LookAt;
    [Header("Custom Collider")]
    [SerializeField] List<Collider> CustomColliders = new List<Collider>();
    [Header("Behaviour Specific Params")]
    [SerializeField] float PathDamping = 1f;
    public float Offset;
    [SerializeField] private GameObject switchObject;

    private GameObject target;
    private Transform direction;
    private BoxCollider baseCol;
    private CinemachineSmoothPath path;
    private CinemachineDollyCart dollyCart;

    [HideInInspector]
    public CinemachineVirtualCamera Vcam;
    [HideInInspector]
    public bool active = false;

    //Accessors
    public Transform Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    private void Awake()
    {
        target = FindObjectOfType<PlayerController>().gameObject;
        Vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        baseCol = GetComponent<BoxCollider>();
        //direction = GetComponentInChildren<PlayerDir>().gameObject.transform;

        if (CustomColliders.Count > 0)
        {
            baseCol.enabled = false;

            foreach (var CustomCollider in CustomColliders)
            {
                if (CustomCollider.GetComponent<MeshRenderer>() != null)
                {
                    CustomCollider.GetComponent<MeshRenderer>().enabled = false;
                }

                if (CustomCollider.isTrigger == false)
                {
                    CustomCollider.isTrigger = true;
                }

                var customCol = CustomCollider.gameObject.AddComponent<CustomCameraZone>();
                customCol.CameraZone = this;
            }
        }

        InitializeBehaviour();
    }

    void Update()
    {
        if (active)
        {
            if (!Vcam.gameObject.activeInHierarchy)
            {
                Vcam.gameObject.SetActive(true);
            }

            if (switchObject != null)
            {
                if (!switchObject.gameObject.activeSelf)
                    switchObject.gameObject.SetActive(true);
            }

            if (Template == Template.PathAiming)
            {
                dollyCart.m_Position = Mathf.Lerp(dollyCart.m_Position, path.FindClosestPoint(target.transform.position, 0, -1, 40), PathDamping);
            }

            if (Behaviour == Behaviour.Path && Template == Template.None)
            {
                dollyCart.m_Position = Mathf.Lerp(dollyCart.m_Position, path.FindClosestPoint(target.transform.position, 0, -1, 40), PathDamping);
            }
        }
        else
        {
            if (Vcam.gameObject.activeInHierarchy)
            {
                Vcam.gameObject.SetActive(false);
            }

            if (switchObject != null)
            {
                if (switchObject.gameObject.activeSelf)
                    switchObject.gameObject.SetActive(false);
            }
        }
    }

    private void InitializeBehaviour()
    {
        if (Template == Template.None)
        {
            if (Behaviour == Behaviour.Classic)
            {
                if (Follow)
                {
                    Vcam.m_Follow = target.transform;
                }

                if (LookAt)
                {
                    Vcam.m_LookAt = target.transform;
                }
            }

            if (Behaviour == Behaviour.Path)
            {
                dollyCart = GetComponentInChildren<CinemachineDollyCart>();
                path = GetComponentInChildren<CinemachineSmoothPath>();
                Vcam.m_Follow = dollyCart.transform;
            }
        }
        
        if (Template == Template.Fixed)
        {
            Vcam.m_Follow = null;
            Vcam.m_LookAt = null;
            Follow = false;
            LookAt = false;
        }

        if (Template == Template.FixedAiming)
        {
            Vcam.m_Follow = null;
            Vcam.m_LookAt = target.transform;
            Follow = false;
            LookAt = true;
        }

        if (Template == Template.Following)
        {
            Vcam.m_Follow = target.transform;
            Vcam.m_LookAt = target.transform;
            Follow = true;
            LookAt = true;
        }

        if (Template == Template.PathAiming)
        {
            dollyCart = GetComponentInChildren<CinemachineDollyCart>();
            path = GetComponentInChildren<CinemachineSmoothPath>();
            Vcam.m_Follow = dollyCart.transform;
            Vcam.m_LookAt = target.transform;
        }
    }
}
