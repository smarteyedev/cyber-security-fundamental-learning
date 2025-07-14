using UnityEngine;

namespace Seville
{
    public class TransformConfig : MonoBehaviour
    {
        [Range(0, 360)]
        public float horizontalPosition = 0;

        [Range(-2f, 2f)]
        public float verticalPosition = 0f;

        [Range(1.7f, 4f)]
        public float distanceFromPlayer = 1.7f;

        [Range(-30f, 30f)]
        public float tiltAngle = 0f;

        private Transform _playerPos;
        public Vector3 centerPos
        {
            get
            {
                return _playerPos != null ? _playerPos.position : Vector3.zero;
            }
            set
            {
                centerPos = value;
            }
        }

        private Vector3 lookAtOffset = new Vector3(0f, 180f, 0f);

#if UNITY_EDITOR
        [ExecuteInEditMode]
        private void Reset()
        {
            if (GetComponent<Transform>() != null)
            {
                GetComponent<Transform>().hideFlags = HideFlags.NotEditable;
            }

            if (centerPos == null)
            {
                _playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            }

            MoveObject();
        }

        [ExecuteInEditMode]
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                MoveObject();
            }
        }
#endif

        public void MoveObject()
        {
            if (centerPos != null)
            {
                float radians = horizontalPosition * Mathf.Deg2Rad;
                float x = centerPos.x + distanceFromPlayer * Mathf.Cos(radians);
                float y = centerPos.y;
                float z = centerPos.z + distanceFromPlayer * Mathf.Sin(radians);

                transform.position = new Vector3(x, y, z);

                Vector3 direction = (centerPos - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(tiltAngle, lookAtOffset.y, lookAtOffset.z);
                transform.rotation = targetRotation;

                transform.position = new Vector3(transform.position.x, verticalPosition, transform.position.z);
            }
        }
    }
}