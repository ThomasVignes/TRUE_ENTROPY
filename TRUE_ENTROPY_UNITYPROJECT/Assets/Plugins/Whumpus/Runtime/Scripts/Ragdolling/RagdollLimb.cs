using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whumpus
{
    public class RagdollLimb : MonoBehaviour
    {
        public LimbType Type;

        [Header("Behaviour")]
        public bool Active = true;
        public bool Simulated = true;
        public bool CanBeCut = true;
        public bool AdditionalDamping = true;
        public bool ProjectionEnabled;
        public bool Monitor = false;

        [Header("Values")]
        public float JointWeight, AdditionalDamper, Mass;
        public float MaxVelocity, MaxAngularVelocity;
        public bool LimitVelocity, LimitAngularVelocity;

        [Header("References")]
        [SerializeField] private Transform targetLimb;


        [HideInInspector]
        public DiversuitRagdoll ragdollManager;
        [HideInInspector]
        public ConfigurableJoint m_ConfigurableJoint;
        [HideInInspector]
        public Rigidbody rb, connectedRb;
        [HideInInspector] Transform parent;
        private float initialMass, initialXDrive, initialXDamp, initialYZDrive, initialYZDamp, maxXForce, maxYZForce;

        [HideInInspector] public bool IsCut;
        public Transform TargetLimb
        {
            get { return targetLimb; }
            set { targetLimb = value; }
        }

        Quaternion targetInitialRotation, localRot;
        Vector3 localPos;

        public void Initialize()
        {
            m_ConfigurableJoint = this.GetComponent<ConfigurableJoint>();
            rb = this.GetComponent<Rigidbody>();

            if (targetLimb != null)
                targetInitialRotation = this.targetLimb.transform.localRotation;

            if (Simulated)
            {
                initialMass = rb.mass;
                initialXDrive = m_ConfigurableJoint.angularXDrive.positionSpring;
                initialXDamp = m_ConfigurableJoint.angularXDrive.positionDamper + AdditionalDamper;
                maxXForce = m_ConfigurableJoint.angularXDrive.maximumForce;
                initialYZDrive = m_ConfigurableJoint.angularYZDrive.positionSpring;
                initialYZDamp = m_ConfigurableJoint.angularYZDrive.positionDamper + AdditionalDamper;
                maxYZForce = m_ConfigurableJoint.angularYZDrive.maximumForce;

                if (ProjectionEnabled)
                    m_ConfigurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
                else
                    m_ConfigurableJoint.projectionMode = JointProjectionMode.None;

                connectedRb = m_ConfigurableJoint.connectedBody;
            }
            else if (Active)
            {
                if (m_ConfigurableJoint != null)
                    Destroy(m_ConfigurableJoint);

                if (rb != null)
                    rb.isKinematic = true;
            }
            parent = transform.parent;
            localPos = transform.localPosition;
            localRot = transform.localRotation;
        }

        private void Update()
        {
            if (!Simulated && Active)
            {
                transform.localRotation = targetLimb.transform.localRotation;
            }
        }

        private void FixedUpdate()
        {
            if (m_ConfigurableJoint != null && Simulated && Active)
            {
                m_ConfigurableJoint.targetRotation = CopyRotation();

                if (LimitVelocity)
                {
                    if (m_ConfigurableJoint.targetVelocity.magnitude > MaxVelocity)
                    {
                        m_ConfigurableJoint.targetVelocity = m_ConfigurableJoint.targetVelocity.normalized * MaxVelocity;
                    }
                }

                if (LimitAngularVelocity)
                {
                    if (m_ConfigurableJoint.targetAngularVelocity.magnitude > MaxAngularVelocity)
                    {
                        m_ConfigurableJoint.targetAngularVelocity = m_ConfigurableJoint.targetAngularVelocity.normalized * MaxAngularVelocity;
                    }
                }
            }
        }


        public void UpdateJoint()
        {
            if (Simulated && m_ConfigurableJoint != null)
            {
                JointDrive XDrive = new JointDrive();
                JointDrive YZDrive = new JointDrive();
                XDrive.positionSpring = initialXDrive * JointWeight;
                XDrive.positionDamper = initialXDamp * JointWeight;
                XDrive.maximumForce = maxXForce;
                YZDrive.positionSpring = initialYZDrive * JointWeight;
                YZDrive.positionDamper = initialYZDamp * JointWeight;
                YZDrive.maximumForce = maxYZForce;

                m_ConfigurableJoint.angularXDrive = XDrive;
                m_ConfigurableJoint.angularYZDrive = YZDrive;
            }
        }

        public void UpdateRb()
        {
            if (Simulated && rb != null)
            {
                rb.mass = initialMass * Mass;
            }
        }

        private Quaternion CopyRotation()
        {
            return Quaternion.Inverse(targetLimb.localRotation) * targetInitialRotation;
        }

        [ContextMenu("Cut")]
        public void CutLimb()
        {
            if (CanBeCut)
            {
                IsCut = true;
                //m_ConfigurableJoint.connectedBody = null;
                transform.parent = null;
                if (Simulated)
                {
                    if (m_ConfigurableJoint != null)
                    {
                        m_ConfigurableJoint.xMotion = ConfigurableJointMotion.Free;
                        m_ConfigurableJoint.yMotion = ConfigurableJointMotion.Free;
                        m_ConfigurableJoint.zMotion = ConfigurableJointMotion.Free;

                        ConstantForce parentForce = m_ConfigurableJoint.connectedBody.GetComponent<ConstantForce>();

                        if (parentForce != null)
                            parentForce.enabled = false;
                    }
                }

                ConstantForce constantForce = GetComponent<ConstantForce>();

                if (constantForce != null)
                    constantForce.enabled = false;
            }
        }

        [ContextMenu("Reattach")]
        public void Reattatch()
        {
            if (IsCut)
            {
                IsCut = false;
                
                transform.parent = parent;
                transform.localPosition = localPos;
                transform.localRotation = localRot;

                if (Simulated)
                {
                    m_ConfigurableJoint.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    m_ConfigurableJoint.xMotion = ConfigurableJointMotion.Locked;
                    m_ConfigurableJoint.yMotion = ConfigurableJointMotion.Locked;
                    m_ConfigurableJoint.zMotion = ConfigurableJointMotion.Locked;
                    m_ConfigurableJoint.targetAngularVelocity = Vector3.zero;
                    m_ConfigurableJoint.targetVelocity = Vector3.zero;
                    m_ConfigurableJoint.GetComponent<Rigidbody>().velocity = Vector3.zero;

                    ConstantForce parentForce = m_ConfigurableJoint.connectedBody.GetComponent<ConstantForce>();

                    if (parentForce != null)
                        parentForce.enabled = true;
                }

                ConstantForce constantForce = GetComponent<ConstantForce>();

                if (constantForce != null)
                    constantForce.enabled = true;
            }
        }

        public void Reattatch(Rigidbody targetRb)
        {
            //m_ConfigurableJoint.connectedBody = connectedRb;
            m_ConfigurableJoint.connectedBody = targetRb;

            transform.parent = targetRb.transform;
            transform.localPosition = localPos;
            transform.localRotation = localRot;

            if (Simulated)
            {
                m_ConfigurableJoint.GetComponent<Rigidbody>().velocity = Vector3.zero;
                m_ConfigurableJoint.xMotion = ConfigurableJointMotion.Locked;
                m_ConfigurableJoint.yMotion = ConfigurableJointMotion.Locked;
                m_ConfigurableJoint.zMotion = ConfigurableJointMotion.Locked;
                m_ConfigurableJoint.targetAngularVelocity = Vector3.zero;
                m_ConfigurableJoint.targetVelocity = Vector3.zero;
                m_ConfigurableJoint.GetComponent<Rigidbody>().velocity = Vector3.zero;

                ConstantForce parentForce = m_ConfigurableJoint.connectedBody.GetComponent<ConstantForce>();

                if (parentForce != null)
                    parentForce.enabled = true;
            }

            ConstantForce constantForce = GetComponent<ConstantForce>();

            if (constantForce != null)
                constantForce.enabled = true;
        }
    }
}
