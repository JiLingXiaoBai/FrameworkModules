using System;
using UnityEngine;

namespace XBToolKit.CharacterController
{
    public class RaycastSensor
    {
        public float castLength = 1f;
        public LayerMask layerMask = 255;

        private Vector3 origin = Vector3.zero;
        private Transform tr;

        public enum CastDirection
        {
            Forward,
            Right,
            Up,
            Backward,
            Left,
            Down
        }

        private CastDirection castDirection;
        private RaycastHit hitInfo;


        public void Cast()
        {
            var worldOrigin = tr.TransformPoint(origin);
            var worldDirection = GetCastDirection();

            Physics.Raycast(worldOrigin, worldDirection, out hitInfo, castLength, layerMask,
                QueryTriggerInteraction.Ignore);
        }


        public bool HasDetectedHit() => hitInfo.collider != null;
        public float GetDistance() => hitInfo.distance;
        public Vector3 GetNormal() => hitInfo.normal;
        public Vector3 GetPosition() => hitInfo.point;
        public Collider GetCollider() => hitInfo.collider;
        public Transform GetTransform() => hitInfo.transform;

        public void SetCastOrigin(Vector3 pos) => origin = tr.InverseTransformPoint(pos);
        public void SetCastDirection(CastDirection direction) => castDirection = direction;

        private Vector3 GetCastDirection()
        {
            return castDirection switch
            {
                CastDirection.Forward => tr.forward,
                CastDirection.Right => tr.right,
                CastDirection.Up => tr.up,
                CastDirection.Backward => -tr.forward,
                CastDirection.Left => -tr.right,
                CastDirection.Down => -tr.up,
                _ => Vector3.one
            };
        }
    }
}