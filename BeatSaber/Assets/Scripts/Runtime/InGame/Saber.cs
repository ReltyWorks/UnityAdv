using EzySlice;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BeatSaber.Runtime.InGame
{
    public class Saber : MonoBehaviour
    {
        [SerializeField] LayerMask _sliceableMask;
        [SerializeField] Transform _plane;
        [SerializeField] Material _hullMaterial;
        [SerializeField] NoteManager _noteManager;
        Vector3 _prevPos;

        private void Awake()
        {
            _prevPos = transform.position;
        }

        private void FixedUpdate()
        {
            Vector3 delta = transform.position - _prevPos;

            if (delta.sqrMagnitude > 0.005f)
            {
                _plane.right = delta;
            }

            _prevPos = transform.position;
        }

        void Slice(GameObject target)
        {
            Vector3 sliceNormal = _plane.up;
            Vector3 slicePoint = _plane.position;
            SlicedHull hull = target.Slice(slicePoint, sliceNormal, _hullMaterial);

            if (hull != null)
            {
                GameObject upper = hull.CreateUpperHull(target, _hullMaterial);
                upper.AddComponent<HullBehaviour>()
                     .DoMove(-sliceNormal + Vector3.forward, -sliceNormal, 1f, 1f);
                GameObject lower = hull.CreateLowerHull(target, _hullMaterial);
                lower.AddComponent<HullBehaviour>()
                     .DoMove(sliceNormal + Vector3.forward, sliceNormal, 1f, 1f);

                _noteManager.DestroyNote(target);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _sliceableMask) == 0)
                return;

            Slice(other.gameObject);
        }
    }
}