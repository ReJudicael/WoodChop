using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class Character : MonoBehaviour
    {
        [SerializeField] protected CharacterData m_characterData;
        [SerializeField] protected Animator m_animator;
        [SerializeField] protected IKTargeter m_animatorIKTargeter;
        protected Vector3 m_lastPosition = Vector3.zero;
        protected float m_lastMagnitude = 0f;
        protected Vector3 m_animationDirection = Vector3.zero;
        protected List<Tree> m_treesInRange = new List<Tree>();
        protected float m_lastAttackTime = 0f;
        protected Tree m_targetTree = null;

        [SerializeField] private UIManager _uiManager;

        // Feedback for AttackSpeed
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip choppingWoodClip;
        protected virtual void Start ()
        {
            m_lastPosition = transform.position;
        }

#region Movement
        protected virtual bool CanWalk ( Vector3 position )
        {
            return true;
        }

        protected virtual bool IsGrounded ( Vector3 targetPos )
        {
            targetPos += Vector3.up;
            RaycastHit hit;
            if (Physics.Raycast(targetPos, -Vector3.up, out hit, 10f, GameManager.Instance.groundLayerMask.value))
            {
                float dst = hit.distance;
                return dst < 10f;
            }
            return false;
        }

        protected virtual void UpdateMovementSpeedAnimation ()
        {
            if (IsGrounded(transform.position))
            {
                Vector3 speed = ((transform.position - m_lastPosition) / Time.fixedDeltaTime / m_characterData.MaxSpeed);

                float speedMagnitude = speed.magnitude;
                speed = Quaternion.AngleAxis(Vector3.SignedAngle(transform.forward, speed, Vector3.up), Vector3.up) * Vector3.forward * speedMagnitude;

                if (speedMagnitude > m_lastMagnitude)
                {
                    m_lastMagnitude = Mathf.Lerp(m_lastMagnitude, speedMagnitude, 0.5f);
                    m_animationDirection = Vector3.Lerp(m_animationDirection, speed, 0.5f);
                }
                else
                {
                    m_lastMagnitude = Mathf.Lerp(m_lastMagnitude, speedMagnitude, 0.125f);
                    m_animationDirection = Vector3.Lerp(m_animationDirection, speed, 0.125f);
                }
            }
            else
            {
                m_lastMagnitude = Mathf.Lerp(m_lastMagnitude, 0f, 0.125f);
                m_animationDirection = Vector3.Lerp(m_animationDirection, Vector3.zero, 0.125f);
            }

            m_animator.SetFloat("VelocityX", m_animationDirection.x);
            m_animator.SetFloat("VelocityY", m_animationDirection.z);
            m_lastPosition = transform.position;
        }
#endregion

        protected virtual void FixedUpdate ()
        {
            UpdateMovementSpeedAnimation();
        }


        public void AddWoodsInPlayerInventory(Tree tree)
        {
            m_characterData.WoodCounter += tree.PointsOnDestroy;
            _uiManager.RefreshWoodCounter();

            if (m_characterData.WoodCounter > 0 && m_characterData.WoodCounter % 10 == 0)
            {
                GameManager.Instance.LevelWon();
            }
        }
        
        public void AddTree(Tree tree)
        {
            if (!m_treesInRange.Contains(tree))
            {
                m_treesInRange.Add(tree);
                tree.onDestroy += RemoveTree;
                tree.onDestroy += AddWoodsInPlayerInventory;
            }
        }

        public void RemoveTree(Tree tree)
        {
            tree.onDestroy -= RemoveTree;
            tree.onDestroy -= AddWoodsInPlayerInventory;
            m_treesInRange.Remove(tree);
            if (tree == m_targetTree)
            {
                m_targetTree.DisableOutline();
                m_targetTree = null;
            }
        }

        private int SortByDistance(Tree a, Tree b)
        {
            if (a == null || b == null)
            {
                return a == b ? 0 : (a == null ? -1 : 1);
            }
            float dstA = (a.transform.position - transform.position).sqrMagnitude;
            float dstB = (b.transform.position - transform.position).sqrMagnitude;
            return dstA.CompareTo(dstB);
        }

        private void Update ()
        {
            if (m_treesInRange.Count > 0)
            {
                m_treesInRange.Sort(SortByDistance);
                if (m_targetTree != null && m_targetTree != m_treesInRange[0])
                {
                    m_targetTree.DisableOutline();
                }
                m_targetTree = m_treesInRange[0];
                m_targetTree.EnableOutline();
                m_animatorIKTargeter.headTargetTransform = m_targetTree.lookAtTfm;

                if (Time.time - m_lastAttackTime > 1f / m_characterData.AttackPerSecond.CurrentValue)
                {
                    m_targetTree.Hit(m_characterData.Strength.CurrentValue);
                    m_lastAttackTime = Time.time;
                    audioSource.PlayOneShot(choppingWoodClip);
                }
            }
            else
            {
                if (m_targetTree != null)
                {
                    m_targetTree.DisableOutline();
                    m_targetTree = null;
                }
                m_animatorIKTargeter.headTargetTransform = null;
            }
        }
    }
}
