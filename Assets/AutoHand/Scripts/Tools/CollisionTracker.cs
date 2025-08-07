using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand
{
    public delegate void CollisionEvent(GameObject from);

    public class CollisionTracker : MonoBehaviour
    {

        public bool disableCollisionTracking = false;
        public bool disableTriggersTracking = false;

        public event CollisionEvent OnCollisionFirstEnter;
        public event CollisionEvent OnCollisionLastExit;
        public event CollisionEvent OnTriggerFirstEnter;
        public event CollisionEvent OnTriggerLastExit;

        public int collisionCount { get { return collisionObjects.Count; } }
        public int triggerCount { get { return triggerObjects.Count; } }

        const int MAX_COLLISIONS_TRACKED = 256;

        public List<GameObject> triggerObjects { get; protected set; } = new List<GameObject>(MAX_COLLISIONS_TRACKED);
        public List<GameObject> collisionObjects { get; protected set; } = new List<GameObject>(MAX_COLLISIONS_TRACKED);
        public List<GameObject> nextTriggerObjects { get; protected set; } = new List<GameObject>(MAX_COLLISIONS_TRACKED);
        public List<GameObject> nextCollisionObjects { get; protected set; } = new List<GameObject>(MAX_COLLISIONS_TRACKED);

        protected List<Collision> collisions { get; set; } = new List<Collision>(MAX_COLLISIONS_TRACKED);

        Coroutine lateFixedUpdate;

        public void CleanUp()
        {
            triggerObjects.Clear();
            nextTriggerObjects.Clear();
            collisionObjects.Clear();
            nextCollisionObjects.Clear();
            collisions.Clear();
        }

        protected virtual void OnEnable()
        {
            lateFixedUpdate = StartCoroutine(LateFixedUpdate());
        }

        protected virtual void OnDisable()
        {
            for (int i = 0; i < collisionObjects.Count; i++)
            {
                var obj = collisionObjects[i];
                if (obj != null && obj.activeInHierarchy && OnCollisionLastExit != null)
                    OnCollisionLastExit.Invoke(obj);
            }

            for (int i = 0; i < triggerObjects.Count; i++)
            {
                var obj = triggerObjects[i];
                if (obj != null && obj.activeInHierarchy && OnTriggerLastExit != null)
                    OnTriggerLastExit.Invoke(obj);
            }

            CleanUp();
            if (lateFixedUpdate != null)
                StopCoroutine(lateFixedUpdate);
        }

        WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();
        IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return waitForFixed;
                CheckTrackedObjects();
            }
        }

        private void CheckTrackedObjects()
        {
            if (!disableCollisionTracking)
            {
                for (int i = collisionObjects.Count - 1; i >= 0; i--)
                {
                    var obj = collisionObjects[i];
                    if (obj == null || !obj.activeInHierarchy || !nextCollisionObjects.Contains(obj))
                    {
                        if (obj != null && OnCollisionLastExit != null)
                            OnCollisionLastExit.Invoke(obj);
                    }
                }

                for (int i = nextCollisionObjects.Count - 1; i >= 0; i--)
                {
                    var obj = nextCollisionObjects[i];
                    if (obj == null || !obj.activeInHierarchy)
                    {
                        nextCollisionObjects.RemoveAt(i);
                    }
                    else if (!collisionObjects.Contains(obj))
                    {
                        if (OnCollisionFirstEnter != null)
                            OnCollisionFirstEnter.Invoke(obj);
                    }
                }

                collisionObjects.Clear();
                collisionObjects.AddRange(nextCollisionObjects);
                nextCollisionObjects.Clear();
                collisions.Clear();
            }

            if (!disableTriggersTracking)
            {
                for (int i = triggerObjects.Count - 1; i >= 0; i--)
                {
                    var obj = triggerObjects[i];
                    if (obj == null || !obj.activeInHierarchy || !nextTriggerObjects.Contains(obj))
                    {
                        if (obj != null && OnTriggerLastExit != null)
                            OnTriggerLastExit.Invoke(obj);
                    }
                }

                for (int i = nextTriggerObjects.Count - 1; i >= 0; i--)
                {
                    var obj = nextTriggerObjects[i];
                    if (obj == null || !obj.activeInHierarchy)
                    {
                        nextTriggerObjects.RemoveAt(i);
                    }
                    else if (!triggerObjects.Contains(obj))
                    {
                        if (OnTriggerFirstEnter != null)
                            OnTriggerFirstEnter.Invoke(obj);
                    }
                }

                triggerObjects.Clear();
                triggerObjects.AddRange(nextTriggerObjects);
                nextTriggerObjects.Clear();
            }
        }

        protected virtual void OnCollisionStay(Collision collision)
        {
            if (!disableCollisionTracking)
            {
                collisions.Add(collision);
                if (collision.collider != null && collision.collider.gameObject != null &&
                    !nextCollisionObjects.Contains(collision.collider.gameObject))
                {
                    nextCollisionObjects.Add(collision.collider.gameObject);
                }
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (!disableTriggersTracking)
            {
                if (other != null && other.gameObject != null &&
                    !nextTriggerObjects.Contains(other.gameObject))
                {
                    nextTriggerObjects.Add(other.gameObject);
                }
            }
        }

        //#if UNITY_EDITOR
        //private void OnDrawGizmos() {
        //    foreach (var collision in collisions) {
        //        foreach (var contactPoint in collision.contacts) {
        //            Gizmos.DrawSphere(contactPoint.point, 0.0025f);
        //        }
        //    }
        //}
        //#endif
    }
}
