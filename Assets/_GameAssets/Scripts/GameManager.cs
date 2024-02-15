using UnityEngine;

namespace Pinpin
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] m_psArray;
        public LayerMask groundLayerMask;

        private static GameManager m_instance;
        public static GameManager Instance => m_instance;
        [SerializeField] private Camera m_mainCamera;
        public static Camera mainCamera => Instance.m_mainCamera;

        private void Awake ()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        [ContextMenu("LevelWon")]
        public void LevelWon()
        {
            foreach (ParticleSystem ps in m_psArray)
                ps.Play();
            Debug.Log("LEVEL WON!");
        }

    }
}
