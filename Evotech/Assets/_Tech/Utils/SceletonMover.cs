using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Utils
{
    public class SceletonMover : MonoBehaviour
    {
        private List<BoneData> _bones;

        [Button]
        public void UpdateSceleton()
        {
            _bones = new List<BoneData>();

            foreach (Transform item in GetComponentsInChildren<Transform>(true))
            {
                _bones.Add(new BoneData(item.transform.position, item.transform.rotation, item));
                item.transform.position += Vector3.up;
                item.transform.rotation *= Quaternion.Euler(1, 1, 1);
            }

            Invoke(nameof(ReturnBones), .5f);
        }

        private void ReturnBones()
        {
            foreach (var bone in _bones)
            {
                bone.Obj.position = bone.Pos;
                bone.Obj.rotation = bone.Rot;
            }
        }

        private class BoneData
        {
            public Vector3 Pos;
            public Quaternion Rot;
            public Transform Obj;

            public BoneData(Vector3 pos, Quaternion rot, Transform obj)
            {
                Pos = pos;
                Rot = rot;
                Obj = obj;
            }
        }
    }
}