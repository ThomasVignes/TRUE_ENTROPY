#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Whumpus.Editor
{
    public class DiversuitRagdollMaker : EditorWindow
    {

        public GameObject Parent, Pelvis, LeftHip, LeftKnee, LeftFoot, RightHip, RightKnee, RightFoot,
            LeftShoulder, LeftArm, LeftElbow, LeftHand, RightShoulder, RightArm, RightElbow, RightHand, Head;

        public List<GameObject> Spine = new List<GameObject>(0);

        public GameObject RefPelvis, RefLeftHip, RefLeftKnee, RefLeftFoot, RefRightHip, RefRightKnee, RefRightFoot,
        RefLeftShoulder, RefLeftArm, RefLeftElbow, RefLeftHand, RefRightShoulder, RefRightArm, RefRightElbow, RefRightHand, RefHead;

        public List<GameObject> RefSpine = new List<GameObject>();

        public float RagdollSpring, RagdollDamper;

        private GameObject CloneRig;

        Vector2 scrollPos;

        [MenuItem("Whumpus/Ragdolls/DiversuitRagdollMaker", false, 1)]
        public static void ShowWindow()
        {
            GetWindow(typeof(DiversuitRagdollMaker));

        }

        public void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Ragdoll parts");

            //SetRagdollManager
            EditorGUILayout.BeginHorizontal();
            Parent = (GameObject)EditorGUILayout.ObjectField("Parent", Parent, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            //SetRagdoll
            EditorGUILayout.BeginHorizontal();
            Pelvis = (GameObject)EditorGUILayout.ObjectField("Pelvis", Pelvis, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            LeftHip = (GameObject)EditorGUILayout.ObjectField("Left Hip", LeftHip, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            LeftKnee = (GameObject)EditorGUILayout.ObjectField("Left Knee", LeftKnee, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            LeftFoot = (GameObject)EditorGUILayout.ObjectField("Left Foot", LeftFoot, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RightHip = (GameObject)EditorGUILayout.ObjectField("Right Hip", RightHip, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RightKnee = (GameObject)EditorGUILayout.ObjectField("Right Knee", RightKnee, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RightFoot = (GameObject)EditorGUILayout.ObjectField("Right Foot", RightFoot, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Spine length", Spine.Count));
            while (newCount < Spine.Count)
                Spine.RemoveAt(Spine.Count - 1);
            while (newCount > Spine.Count)
                Spine.Add(null);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < Spine.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Spine[i] = (GameObject)EditorGUILayout.ObjectField("Spine " + i, Spine[i], typeof(GameObject));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            LeftShoulder = (GameObject)EditorGUILayout.ObjectField("Left Shoulder", LeftShoulder, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            LeftArm = (GameObject)EditorGUILayout.ObjectField("Left Arm", LeftArm, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            LeftElbow = (GameObject)EditorGUILayout.ObjectField("Left Elbow", LeftElbow, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            LeftHand = (GameObject)EditorGUILayout.ObjectField("Left Hand", LeftHand, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RightShoulder = (GameObject)EditorGUILayout.ObjectField("Right Shoulder", RightShoulder, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RightArm = (GameObject)EditorGUILayout.ObjectField("Right Arm", RightArm, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RightElbow = (GameObject)EditorGUILayout.ObjectField("Right Elbow", RightElbow, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RightHand = (GameObject)EditorGUILayout.ObjectField("Right Hand", RightHand, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            Head = (GameObject)EditorGUILayout.ObjectField("Head", Head, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Values");

            //SetValues
            EditorGUILayout.BeginHorizontal();
            RagdollSpring = EditorGUILayout.FloatField("Ragdoll Spring", RagdollSpring);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RagdollDamper = EditorGUILayout.FloatField("Ragdoll Damper", RagdollDamper);
            EditorGUILayout.EndHorizontal();


            if (GUILayout.Button("Create"))
            {
                CreateRagdoll();
            }
        }

        private void CreateRagdoll()
        {
            if (Parent.GetComponent<DiversuitRagdoll>())
            {
                Debug.LogError("rig already has a ragdoll");

                return;
            }

            CloneRig = Instantiate(Parent, Parent.transform.position + Vector3.right * 5f, Parent.transform.rotation);
            CloneRig.name = Parent.name + " AnimRef";
            if (!CloneRig.GetComponent<Animator>())
            {
                CloneRig.AddComponent<Animator>();
            }

            RefSpine.Clear();
            foreach (var s in Spine)
            {
                GameObject newSpine = GetCloneOfLimb(s);
                Debug.Log(s);
                RefSpine.Add(newSpine);
            }

            RefPelvis = GetCloneOfLimb(Pelvis);
            RefLeftHip = GetCloneOfLimb(LeftHip);
            RefLeftKnee = GetCloneOfLimb(LeftKnee);
            RefLeftFoot = GetCloneOfLimb(LeftFoot);
            RefRightHip = GetCloneOfLimb(RightHip);
            RefRightKnee = GetCloneOfLimb(RightKnee);
            RefRightFoot = GetCloneOfLimb(RightFoot);
            RefLeftShoulder = GetCloneOfLimb(LeftShoulder);
            RefLeftArm = GetCloneOfLimb(LeftArm);
            RefLeftElbow = GetCloneOfLimb(LeftElbow);
            RefLeftHand = GetCloneOfLimb(LeftHand);
            RefRightShoulder = GetCloneOfLimb(RightShoulder);
            RefRightArm = GetCloneOfLimb(RightArm);
            RefRightElbow = GetCloneOfLimb(RightElbow);
            RefRightHand = GetCloneOfLimb(RightHand);
            RefHead = GetCloneOfLimb(Head);

            DiversuitRagdoll manager = Parent.AddComponent<DiversuitRagdoll>();
            Rigidbody parentRb = Parent.AddComponent<Rigidbody>();
            parentRb.isKinematic = true;

            List<GameObject> limbs = new List<GameObject>(){Pelvis, LeftHip, LeftKnee, LeftFoot, RightHip, RightKnee, RightFoot,
        LeftShoulder, LeftArm, LeftElbow, LeftHand, RightShoulder, RightArm, RightElbow, RightHand, Head};

            foreach (var part in Spine)
            {
                if (!limbs.Contains(part))
                    limbs.Add(part);
            }

            foreach (var limb in limbs)
            {
                if (limb != null)
                {
                    limb.AddComponent<Rigidbody>();
                    ConfigurableJoint joint = limb.AddComponent<ConfigurableJoint>();

                    if (limb != Pelvis)
                    {
                        limb.AddComponent<RagdollLimb>();

                        joint.xMotion = ConfigurableJointMotion.Locked;
                        joint.yMotion = ConfigurableJointMotion.Locked;
                        joint.zMotion = ConfigurableJointMotion.Locked;

                        JointDrive XDrive = new JointDrive();
                        JointDrive YZDrive = new JointDrive();
                        XDrive.positionSpring = RagdollSpring;
                        XDrive.positionDamper = RagdollDamper;
                        XDrive.maximumForce = joint.xDrive.maximumForce;
                        YZDrive.positionSpring = RagdollSpring;
                        YZDrive.positionDamper = RagdollDamper;
                        YZDrive.maximumForce = joint.angularYZDrive.maximumForce;

                        joint.angularXDrive = XDrive;
                        joint.angularYZDrive = YZDrive;
                    }
                    else
                    {
                        joint.angularXMotion = ConfigurableJointMotion.Locked;
                        joint.angularYMotion = ConfigurableJointMotion.Locked;
                        joint.angularZMotion = ConfigurableJointMotion.Locked;
                        manager.Root = limb;
                        manager.mainRb = limb.GetComponent<Rigidbody>();
                    }
                }
            }

            GameObject topSpine = Spine[Spine.Count - 1];

            //Pelvis
            Pelvis.GetComponent<ConfigurableJoint>().connectedBody = parentRb;

            //Head
            Setup(Head, topSpine, RefHead);

            //LeftArm
            Setup(LeftHand, LeftElbow, RefLeftHand);
            Setup(LeftElbow, LeftArm, RefLeftElbow);
            Setup(LeftArm, LeftShoulder, RefLeftArm);
            Setup(LeftShoulder, topSpine, RefLeftShoulder);

            //RightArm
            Setup(RightHand, RightElbow, RefRightHand);
            Setup(RightElbow, RightArm, RefRightElbow);
            Setup(RightArm, RightShoulder, RefRightArm);
            Setup(RightShoulder, topSpine, RefRightShoulder);

            //Spine
            for (int i = Spine.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    Setup(Spine[i], Pelvis, RefSpine[i]);
                }
                else
                {
                    Setup(Spine[i], Spine[i - 1], RefSpine[i]);
                }
            }

            //LeftLeg
            Setup(LeftFoot, LeftKnee, RefLeftFoot);
            Setup(LeftKnee, LeftHip, RefLeftKnee);
            Setup(LeftHip, Pelvis, RefLeftHip);

            //RightLeg
            Setup(RightFoot, RightKnee, RefRightFoot);
            Setup(RightKnee, RightHip, RefRightKnee);
            Setup(RightHip, Pelvis, RefRightHip);
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
            return RecursiveFindChild(CloneRig.transform, Limb.name).gameObject;
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