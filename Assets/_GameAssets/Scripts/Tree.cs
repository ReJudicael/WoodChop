using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class Tree : MonoBehaviour
    {
        [SerializeField] float m_lifePoints = 3f;
        [SerializeField] private int pointsOnDestroy = 5;
        [SerializeField] Outline m_outline;
        public Transform lookAtTfm;
        public Action<Tree> onDestroy;

        public int PointsOnDestroy => pointsOnDestroy;
        public Vector3 Position => transform.position;

        public void Hit ( float strength )
        {
            m_lifePoints -= strength;
            UpdateState();
        }

        private void Start ()
        {
            DisableOutline();
        }

        public void EnableOutline()
        {
            m_outline.enabled = true;
        }

        public void DisableOutline ()
        {
            m_outline.enabled = false;
        }

        private void UpdateState()
        {
            if (m_lifePoints <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            onDestroy?.Invoke(this);
            Destroy(gameObject);
        }

        private void OnTriggerEnter ( Collider other )
        {
            if (other.attachedRigidbody == null)
                return;
            Character character = other.attachedRigidbody.GetComponent<Character>();
            if (character != null)
            {
                character.AddTree(this);
            }
        }

        private void OnTriggerExit ( Collider other )
        {
            if (other.attachedRigidbody == null)
                return;
            Character character = other.attachedRigidbody.GetComponent<Character>();
            if (character != null)
            {
                character.RemoveTree(this);
            }            
        }
    }
}
