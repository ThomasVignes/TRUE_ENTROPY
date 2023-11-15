#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Whumpus.Editor
{
    using Whumpus;
    [System.Serializable]
    public class ParentJoint
    {
        public GameObject Go;
        public bool HasChildren;
        public List<ParentJoint> Children = new List<ParentJoint>();
    }

    public enum HierarchyPreset
    {
        None,
        Spine,
        Hand
    }

    public class JointHierarchy : EditorWindow
    {
        public HierarchyPreset Preset;
        public List<ParentJoint> Joints = new List<ParentJoint>();
        public GameObject HandParent, SpineStart, SpineEnd;

        public GameObject AnimationRefParent;
        public DiversuitRagdoll DiversuitRagdollReference;
        public float RagdollSpring, RagdollDamper;
        public bool HasAnimationTarget, PhysicsSimulated;
        Vector2 scrollPos;

        [MenuItem("Whumpus/Ragdolls/JointHierarchy", false, 1)]
        public static void ShowWindow()
        {
            GetWindow(typeof(JointHierarchy));
        }

        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            Preset = (HierarchyPreset)EditorGUILayout.EnumPopup("Hierarchy preset :", Preset);
            EditorGUILayout.EndHorizontal();

            switch (Preset)
            {
                case HierarchyPreset.None:
                    ShowList(Joints, 0);
                    break;

                case HierarchyPreset.Spine:
                    EditorGUILayout.BeginHorizontal();
                    SpineStart = (GameObject)EditorGUILayout.ObjectField("Spine start", SpineStart, typeof(GameObject), true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    SpineEnd = (GameObject)EditorGUILayout.ObjectField("Spine end", SpineEnd, typeof(GameObject), true);
                    EditorGUILayout.EndHorizontal();
                    break;

                case HierarchyPreset.Hand:
                    EditorGUILayout.BeginHorizontal();
                    HandParent = (GameObject)EditorGUILayout.ObjectField("Hand parent", HandParent, typeof(GameObject), true);
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            //SetValues
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            PhysicsSimulated = EditorGUILayout.Toggle("Physics simulated", PhysicsSimulated);
            EditorGUILayout.EndHorizontal();
            if (PhysicsSimulated)
            {
                EditorGUILayout.BeginHorizontal();
                DiversuitRagdollReference = (DiversuitRagdoll)EditorGUILayout.ObjectField("Diversuit ragdoll parent", DiversuitRagdollReference, typeof(DiversuitRagdoll), true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                RagdollSpring = EditorGUILayout.FloatField("Ragdoll Spring", RagdollSpring);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                RagdollDamper = EditorGUILayout.FloatField("Ragdoll Damper", RagdollDamper);
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.BeginHorizontal();
            HasAnimationTarget = EditorGUILayout.Toggle("Has animation target", HasAnimationTarget);
            EditorGUILayout.EndHorizontal();
            if (HasAnimationTarget)
            {
                EditorGUILayout.BeginHorizontal();
                AnimationRefParent = (GameObject)EditorGUILayout.ObjectField("Animation ref parent", AnimationRefParent, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Attatch"))
            {
                switch (Preset)
                {
                    case HierarchyPreset.None:
                        Attach(Joints);
                        break;

                    case HierarchyPreset.Spine:
                        SpineAttach(SpineStart, true);
                        break;

                    case HierarchyPreset.Hand:
                        HierarchyAttach(HandParent, true);
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        private void Attach(List<ParentJoint> Joints)
        {
            foreach (var joint in Joints)
            {
                Rigidbody rb = joint.Go.AddComponent<Rigidbody>();

                if (PhysicsSimulated)
                {
                    ConfigurableJoint jointSettings = joint.Go.AddComponent<ConfigurableJoint>();

                    jointSettings.xMotion = ConfigurableJointMotion.Locked;
                    jointSettings.yMotion = ConfigurableJointMotion.Locked;
                    jointSettings.zMotion = ConfigurableJointMotion.Locked;

                    JointDrive XDrive = new JointDrive();
                    JointDrive YZDrive = new JointDrive();
                    XDrive.positionSpring = RagdollSpring;
                    XDrive.positionDamper = RagdollDamper;
                    XDrive.maximumForce = jointSettings.xDrive.maximumForce;
                    YZDrive.positionSpring = RagdollSpring;
                    YZDrive.positionDamper = RagdollDamper;
                    YZDrive.maximumForce = jointSettings.angularYZDrive.maximumForce;

                    jointSettings.angularXDrive = XDrive;
                    jointSettings.angularYZDrive = YZDrive;

                    Attach(joint.Children, rb);
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    Attach(joint.Children);
                }

                if (HasAnimationTarget)
                {
                    RagdollLimb l = joint.Go.AddComponent<RagdollLimb>();
                    l.Active = true;
                    l.TargetLimb = GetCloneOfLimb(l.gameObject).transform;

                    if (PhysicsSimulated)
                    {
                        l.m_ConfigurableJoint = joint.Go.GetComponent<ConfigurableJoint>();
                        l.Simulated = true;

                    }
                    else
                    {
                        l.Simulated = false;
                    }

                    if (DiversuitRagdollReference != null)
                        l.ragdollManager = DiversuitRagdollReference;
                }

            }
        }

        private void Attach(List<ParentJoint> Joints, Rigidbody rigidbody)
        {
            foreach (var joint in Joints)
            {
                Rigidbody rb = joint.Go.AddComponent<Rigidbody>();

                if (PhysicsSimulated)
                {
                    ConfigurableJoint jointSettings = joint.Go.AddComponent<ConfigurableJoint>();

                    jointSettings.connectedBody = rigidbody;
                    jointSettings.xMotion = ConfigurableJointMotion.Locked;
                    jointSettings.yMotion = ConfigurableJointMotion.Locked;
                    jointSettings.zMotion = ConfigurableJointMotion.Locked;

                    JointDrive XDrive = new JointDrive();
                    JointDrive YZDrive = new JointDrive();
                    XDrive.positionSpring = RagdollSpring;
                    XDrive.positionDamper = RagdollDamper;
                    XDrive.maximumForce = jointSettings.xDrive.maximumForce;
                    YZDrive.positionSpring = RagdollSpring;
                    YZDrive.positionDamper = RagdollDamper;
                    YZDrive.maximumForce = jointSettings.angularYZDrive.maximumForce;

                    jointSettings.angularXDrive = XDrive;
                    jointSettings.angularYZDrive = YZDrive;


                    Attach(joint.Children, rb);
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    Attach(joint.Children);
                }
            }
        }

        private void HierarchyAttach(GameObject Parent, bool First)
        {
            if (First)
            {
                Rigidbody rb = Parent.GetComponent<Rigidbody>();

                if (rb == null)
                    Parent.AddComponent<Rigidbody>();

                if (PhysicsSimulated)
                {
                    ConfigurableJoint jointSettings = Parent.AddComponent<ConfigurableJoint>();

                    jointSettings.xMotion = ConfigurableJointMotion.Locked;
                    jointSettings.yMotion = ConfigurableJointMotion.Locked;
                    jointSettings.zMotion = ConfigurableJointMotion.Locked;

                    JointDrive XDrive = new JointDrive();
                    JointDrive YZDrive = new JointDrive();
                    XDrive.positionSpring = RagdollSpring;
                    XDrive.positionDamper = RagdollDamper;
                    XDrive.maximumForce = jointSettings.xDrive.maximumForce;
                    YZDrive.positionSpring = RagdollSpring;
                    YZDrive.positionDamper = RagdollDamper;
                    YZDrive.maximumForce = jointSettings.angularYZDrive.maximumForce;

                    jointSettings.angularXDrive = XDrive;
                    jointSettings.angularYZDrive = YZDrive;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }

                if (HasAnimationTarget)
                {
                    RagdollLimb l = Parent.AddComponent<RagdollLimb>();
                    l.Active = true;
                    l.TargetLimb = GetCloneOfLimb(l.gameObject).transform;

                    if (PhysicsSimulated)
                    {
                        l.m_ConfigurableJoint = Parent.GetComponent<ConfigurableJoint>();
                        l.Simulated = true;

                    }
                    else
                    {
                        l.Simulated = false;
                    }

                    if (DiversuitRagdollReference != null)
                        l.ragdollManager = DiversuitRagdollReference;
                }
            }
            foreach (Transform child in Parent.transform)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();

                if (PhysicsSimulated)
                {
                    ConfigurableJoint jointSettings = child.gameObject.AddComponent<ConfigurableJoint>();

                    jointSettings.connectedBody = Parent.GetComponent<Rigidbody>();
                    jointSettings.xMotion = ConfigurableJointMotion.Locked;
                    jointSettings.yMotion = ConfigurableJointMotion.Locked;
                    jointSettings.zMotion = ConfigurableJointMotion.Locked;

                    JointDrive XDrive = new JointDrive();
                    JointDrive YZDrive = new JointDrive();
                    XDrive.positionSpring = RagdollSpring;
                    XDrive.positionDamper = RagdollDamper;
                    XDrive.maximumForce = jointSettings.xDrive.maximumForce;
                    YZDrive.positionSpring = RagdollSpring;
                    YZDrive.positionDamper = RagdollDamper;
                    YZDrive.maximumForce = jointSettings.angularYZDrive.maximumForce;

                    jointSettings.angularXDrive = XDrive;
                    jointSettings.angularYZDrive = YZDrive;

                    HierarchyAttach(child.gameObject, rb);
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    HierarchyAttach(child.gameObject, false);
                }

                if (HasAnimationTarget)
                {
                    RagdollLimb l = child.gameObject.AddComponent<RagdollLimb>();
                    l.Active = true;
                    l.TargetLimb = GetCloneOfLimb(l.gameObject).transform;

                    if (PhysicsSimulated)
                    {
                        l.m_ConfigurableJoint = child.gameObject.GetComponent<ConfigurableJoint>();
                        l.Simulated = true;

                    }
                    else
                    {
                        l.Simulated = false;
                    }

                    if (DiversuitRagdollReference != null)
                        l.ragdollManager = DiversuitRagdollReference;
                }

            }
        }


        private void HierarchyAttach(GameObject Parent, Rigidbody rigidbody)
        {
            foreach (Transform child in Parent.transform)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();

                if (PhysicsSimulated)
                {
                    ConfigurableJoint jointSettings = child.gameObject.AddComponent<ConfigurableJoint>();


                    jointSettings.connectedBody = rigidbody;
                    jointSettings.xMotion = ConfigurableJointMotion.Locked;
                    jointSettings.yMotion = ConfigurableJointMotion.Locked;
                    jointSettings.zMotion = ConfigurableJointMotion.Locked;

                    JointDrive XDrive = new JointDrive();
                    JointDrive YZDrive = new JointDrive();
                    XDrive.positionSpring = RagdollSpring;
                    XDrive.positionDamper = RagdollDamper;
                    XDrive.maximumForce = jointSettings.xDrive.maximumForce;
                    YZDrive.positionSpring = RagdollSpring;
                    YZDrive.positionDamper = RagdollDamper;
                    YZDrive.maximumForce = jointSettings.angularYZDrive.maximumForce;

                    jointSettings.angularXDrive = XDrive;
                    jointSettings.angularYZDrive = YZDrive;

                    HierarchyAttach(child.gameObject, rb);
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    HierarchyAttach(child.gameObject, false);
                }

                if (HasAnimationTarget)
                {
                    RagdollLimb l = child.gameObject.AddComponent<RagdollLimb>();
                    l.Active = true;
                    l.TargetLimb = GetCloneOfLimb(l.gameObject).transform;

                    if (PhysicsSimulated)
                    {
                        l.m_ConfigurableJoint = child.gameObject.GetComponent<ConfigurableJoint>();
                        l.Simulated = true;

                    }
                    else
                    {
                        l.Simulated = false;
                    }

                    if (DiversuitRagdollReference != null)
                        l.ragdollManager = DiversuitRagdollReference;
                }

            }
        }

        private void SpineAttach(GameObject Parent, bool First)
        {
            if (First)
            {
                Rigidbody rb = Parent.AddComponent<Rigidbody>();

                if (PhysicsSimulated)
                {
                    ConfigurableJoint jointSettings = Parent.AddComponent<ConfigurableJoint>();

                    jointSettings.xMotion = ConfigurableJointMotion.Locked;
                    jointSettings.yMotion = ConfigurableJointMotion.Locked;
                    jointSettings.zMotion = ConfigurableJointMotion.Locked;

                    JointDrive XDrive = new JointDrive();
                    JointDrive YZDrive = new JointDrive();
                    XDrive.positionSpring = RagdollSpring;
                    XDrive.positionDamper = RagdollDamper;
                    XDrive.maximumForce = jointSettings.xDrive.maximumForce;
                    YZDrive.positionSpring = RagdollSpring;
                    YZDrive.positionDamper = RagdollDamper;
                    YZDrive.maximumForce = jointSettings.angularYZDrive.maximumForce;

                    jointSettings.angularXDrive = XDrive;
                    jointSettings.angularYZDrive = YZDrive;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }

                if (HasAnimationTarget)
                {
                    RagdollLimb l = Parent.AddComponent<RagdollLimb>();
                    l.Active = true;
                    l.TargetLimb = GetCloneOfLimb(l.gameObject).transform;

                    if (PhysicsSimulated)
                    {
                        l.m_ConfigurableJoint = Parent.GetComponent<ConfigurableJoint>();
                        l.Simulated = true;

                    }
                    else
                    {
                        l.Simulated = false;
                    }

                    if (DiversuitRagdollReference != null)
                        l.ragdollManager = DiversuitRagdollReference;
                }
            }

            foreach (Transform child in Parent.transform)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();

                if (PhysicsSimulated)
                {
                    ConfigurableJoint jointSettings = child.gameObject.AddComponent<ConfigurableJoint>();

                    jointSettings.connectedBody = Parent.GetComponent<Rigidbody>();
                    jointSettings.xMotion = ConfigurableJointMotion.Locked;
                    jointSettings.yMotion = ConfigurableJointMotion.Locked;
                    jointSettings.zMotion = ConfigurableJointMotion.Locked;

                    JointDrive XDrive = new JointDrive();
                    JointDrive YZDrive = new JointDrive();
                    XDrive.positionSpring = RagdollSpring;
                    XDrive.positionDamper = RagdollDamper;
                    XDrive.maximumForce = jointSettings.xDrive.maximumForce;
                    YZDrive.positionSpring = RagdollSpring;
                    YZDrive.positionDamper = RagdollDamper;
                    YZDrive.maximumForce = jointSettings.angularYZDrive.maximumForce;

                    jointSettings.angularXDrive = XDrive;
                    jointSettings.angularYZDrive = YZDrive;

                    if (child != SpineEnd)
                        SpineAttach(child.gameObject, rb);
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    if (child != SpineEnd)
                        SpineAttach(child.gameObject, false);
                }

                if (HasAnimationTarget)
                {
                    RagdollLimb l = child.gameObject.AddComponent<RagdollLimb>();
                    l.Active = true;
                    l.TargetLimb = GetCloneOfLimb(l.gameObject).transform;

                    if (PhysicsSimulated)
                    {
                        l.m_ConfigurableJoint = child.gameObject.GetComponent<ConfigurableJoint>();
                        l.Simulated = true;

                    }
                    else
                    {
                        l.Simulated = false;
                    }

                    if (DiversuitRagdollReference != null)
                        l.ragdollManager = DiversuitRagdollReference;
                }

            }
        }

        private void SpineAttach(GameObject Parent, Rigidbody rigidbody)
        {
            foreach (Transform child in Parent.transform)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();

                if (PhysicsSimulated)
                {
                    ConfigurableJoint jointSettings = child.gameObject.AddComponent<ConfigurableJoint>();


                    jointSettings.connectedBody = rigidbody;
                    jointSettings.xMotion = ConfigurableJointMotion.Locked;
                    jointSettings.yMotion = ConfigurableJointMotion.Locked;
                    jointSettings.zMotion = ConfigurableJointMotion.Locked;

                    JointDrive XDrive = new JointDrive();
                    JointDrive YZDrive = new JointDrive();
                    XDrive.positionSpring = RagdollSpring;
                    XDrive.positionDamper = RagdollDamper;
                    XDrive.maximumForce = jointSettings.xDrive.maximumForce;
                    YZDrive.positionSpring = RagdollSpring;
                    YZDrive.positionDamper = RagdollDamper;
                    YZDrive.maximumForce = jointSettings.angularYZDrive.maximumForce;

                    jointSettings.angularXDrive = XDrive;
                    jointSettings.angularYZDrive = YZDrive;

                    if (child != SpineEnd)
                        SpineAttach(child.gameObject, rb);
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    if (child != SpineEnd)
                        SpineAttach(child.gameObject, false);
                }

                if (HasAnimationTarget)
                {
                    RagdollLimb l = child.gameObject.AddComponent<RagdollLimb>();
                    l.Active = true;
                    l.TargetLimb = GetCloneOfLimb(l.gameObject).transform;

                    if (PhysicsSimulated)
                    {
                        l.m_ConfigurableJoint = child.gameObject.GetComponent<ConfigurableJoint>();
                        l.Simulated = true;

                    }
                    else
                    {
                        l.Simulated = false;
                    }

                    if (DiversuitRagdollReference != null)
                        l.ragdollManager = DiversuitRagdollReference;
                }

            }
        }

        public void ShowList(List<ParentJoint> jointList, int serialization)
        {
            EditorGUILayout.BeginHorizontal();

            for (int j = 0; j < serialization; j++)
            {
                EditorGUILayout.Space();
            }

            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Hierarchy " + serialization, jointList.Count));

            while (newCount < jointList.Count)
                jointList.RemoveAt(jointList.Count - 1);
            while (newCount > jointList.Count)
                jointList.Add(new ParentJoint());

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < jointList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < serialization; j++)
                {
                    EditorGUILayout.Space();
                }
                EditorGUILayout.LabelField("Joint " + i);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < serialization; j++)
                {
                    EditorGUILayout.Space();
                }
                jointList[i].Go = (GameObject)EditorGUILayout.ObjectField(jointList[i].Go, typeof(GameObject));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < serialization; j++)
                {
                    EditorGUILayout.Space();
                }
                jointList[i].HasChildren = EditorGUILayout.Toggle("Has Children", jointList[i].HasChildren);
                EditorGUILayout.EndHorizontal();

                if (jointList[i].HasChildren)
                    ShowList(jointList[i].Children, serialization + 1);
            }
        }

        public void Setup(GameObject limb, GameObject connected, GameObject refLimb)
        {
            if (connected != null)
                limb.GetComponent<ConfigurableJoint>().connectedBody = connected.GetComponent<Rigidbody>();

            if (refLimb != null)
                limb.GetComponent<RagdollLimb>().TargetLimb = refLimb.transform;
        }

        public GameObject GetCloneOfLimb(GameObject Limb)
        {
            return RecursiveFindChild(AnimationRefParent.transform, Limb.name).gameObject;
        }

        public Transform RecursiveFindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }
                else
                {
                    Transform found = RecursiveFindChild(child, childName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }
    }

#endif
}