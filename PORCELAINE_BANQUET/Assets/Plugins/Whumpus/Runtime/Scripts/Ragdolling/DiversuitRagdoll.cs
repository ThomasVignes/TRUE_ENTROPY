using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Whumpus
{
    public class DiversuitRagdoll : MonoBehaviour
    {
        [Header("Settings")]
        public float SetTo;
        public float Weight = 1f;
        public float AdditionalDamping = 0f;
        public float Mass = 1f;
        public bool LimitVelocity = true;
        public bool LimitAngularVelocity = true;
        public float MaxVelocity = 20000f;
        public float MaxAngularVelocity = 20000f;
        public bool EnableProjection = false;
        [SerializeField] private LayerMask BaseLayer, NoColLayer;

        [Header("References")]
        [SerializeField] private List<RagdollLimb> limbs = new List<RagdollLimb>();
        [SerializeField] private List<ConstantForce> forces = new List<ConstantForce>();
        [SerializeField] private GameObject root;
        public Rigidbody mainRb;

        public GameObject Root
        {
            get { return root; }
            set { root = value; }
        }

        public List<RagdollLimb> Limbs
        {
            get { return limbs; }
        }

        public Action Hit;

        private void Start()
        {
            mainRb = root.GetComponent<Rigidbody>();
            RagdollLimb[] l = transform.GetComponentsInChildren<RagdollLimb>();
            foreach (var e in l)
            {
                //e.gameObject.layer = WhumpusUtilities.ToLayer(BaseLayer);
                e.ragdollManager = this;

                limbs.Add(e);

                e.LimitVelocity = LimitVelocity;
                e.MaxVelocity = MaxVelocity;
                e.LimitAngularVelocity = LimitAngularVelocity;
                e.MaxAngularVelocity = MaxAngularVelocity;
                e.ProjectionEnabled = EnableProjection;
                if (e.AdditionalDamping)
                {
                    e.AdditionalDamper = AdditionalDamping;
                }

                e.Initialize();
            }

            ConstantForce[] f = transform.GetComponentsInChildren<ConstantForce>();
            foreach (var i in f)
            {
                forces.Add(i);
            }

            ChangeWeight(Weight);
        }

        public void SetEntityCollisions(bool active)
        {
            foreach (var item in limbs)
            {
                if (active)
                    item.gameObject.layer = WhumpusUtilities.ToLayer(BaseLayer);
                else
                    item.gameObject.layer = WhumpusUtilities.ToLayer(NoColLayer);
            }
        }

        public void ChangeWeight(float w)
        {
            Weight = w;

            UpdateLimbs();
        }

        public void Simulate(bool simulate)
        {
            foreach (var limb in limbs)
            {
                limb.Simulated = simulate;
            }
        }

        public void ChangeMass(float m)
        {
            Mass = m;

            UpdateLimbMass();
        }

        public void EnableForces(bool state)
        {
            foreach (var f in forces)
            {
                f.enabled = state;
            }
        }

        public void UpdateLimbs()
        {
            foreach (var limb in limbs)
            {
                limb.JointWeight = Weight;
                limb.UpdateJoint();
            }
        }

        public void UpdateLimbMass()
        {
            foreach (var limb in limbs)
            {
                limb.Mass = Mass;
                limb.UpdateRb();
            }
        }

        public void AddForce(Vector3 force, bool resetVelocity)
        {
            foreach (var limb in limbs)
            {
                if (resetVelocity)
                    limb.rb.velocity = Vector3.zero;

                limb.rb.AddForce(force);
            }
        }

        public void IgnoreEntities(bool yes)
        {
            if (yes)
            {
                foreach (var limb in limbs)
                {
                    limb.gameObject.layer = WhumpusUtilities.ToLayer(NoColLayer);
                }
            }
            else
            {
                foreach (var limb in limbs)
                {
                    limb.gameObject.layer = WhumpusUtilities.ToLayer(BaseLayer);
                }
            }
        }

        public void Explode()
        {
            EnableForces(false);

            ChangeMass(10);

            ChangeWeight(0f);

            foreach (var limb in limbs)
            {
                limb.gameObject.layer = 0;

                

                limb.CutLimb();

                if (mainRb != null && limb.rb != null)
                {
                    limb.rb.velocity = Vector3.zero;
                    limb.rb.AddExplosionForce(100, mainRb.transform.position, 8);
                }
            }
        }

        public void Die()
        {
            EnableForces(false);

            foreach (var limb in limbs)
            {

                limb.gameObject.layer = 0;


                limb.rb.velocity = Vector3.zero;
            }
            ChangeMass(10);

            ChangeWeight(0f);
        }

        public void Dismember(RagdollLimb limb)
        {
            limb.CutLimb();
        }

        public void Dismember(LimbType type, bool cutMultiple)
        {
            foreach (var l in limbs)
            {
                if (l.Type == type)
                {
                    l.CutLimb();

                    if (!cutMultiple)
                        break;
                }
            }
        }

        public void Dismember(LimbType type, out RagdollLimb limb)
        {
            limb = null;

            foreach (var l in limbs)
            {
                if (l.Type == type)
                {
                    l.CutLimb();

                    limb = l;
                }
            }
        }

    }
}
namespace Whumpus.Editor
{
    using UnityEditor;

#if UNITY_EDITOR
    [CustomEditor(typeof(DiversuitRagdoll))]
    class RagdollManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DiversuitRagdoll ragdoll = (DiversuitRagdoll)target;

            if (GUILayout.Button("UpdateLimbs"))
                ragdoll.UpdateLimbs();

            if (GUILayout.Button("UpdateMass"))
                ragdoll.UpdateLimbMass();

            DrawDefaultInspector();

            if (GUILayout.Button("Ragdoll"))
                ragdoll.Die();
        }
    }
#endif
}
