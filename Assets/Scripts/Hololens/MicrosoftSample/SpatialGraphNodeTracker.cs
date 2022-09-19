// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#if UNITY_WSA
using Microsoft.MixedReality.OpenXR;
#endif
using UnityEngine;

namespace Microsoft.MixedReality.SampleQRCodes {
    internal class SpatialGraphNodeTracker : MonoBehaviour {
#if UNITY_WSA
        private SpatialGraphNode node;

        public System.Guid Id { get; set; }

        void Update() {
            if (node == null || node.Id != Id) {
                node = (Id != System.Guid.Empty) ? SpatialGraphNode.FromStaticNodeId(Id) : null;
                Debug.Log("Initialize SpatialGraphNode Id= " + Id);
            }

            if (node != null) {
                if (node.TryLocate(FrameTime.OnUpdate, out Pose pose)) {
                    // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
                    // to these objects so apply the inverse
                    if (Camera.main.transform.parent != null) {
                        pose = pose.GetTransformedBy(Camera.main.transform.parent);
                    }

                    gameObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
                    Debug.Log("Id= " + Id + " QRPose = " + pose.position.ToString("F7") + " QRRot = " + pose.rotation.ToString("F7"));
                } else {
                    Debug.LogWarning("Cannot locate " + Id);
                }
            }
        }
#endif
    }
}