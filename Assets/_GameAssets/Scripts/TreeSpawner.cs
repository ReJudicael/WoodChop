using System.Collections.Generic;
using UnityEngine;

namespace Pinpin
{
    public class TreeSpawner : MonoBehaviour
    {
        public Tree treePrefab;
        public int numberOfTrees = 10;
        public int attemptsCountMax = 100;

        public Vector3 spawnAreaScale = new Vector3(5f, 0f, 5f);
        public float minDistanceBetweenTrees = 1f;
        
        private void Start()
        {
            SpawnTrees();
        }

        private void SpawnTrees()
        {
            var spawnedTrees = new List<Tree>();
            var attemptsCount = 0; // Counter for spawn attempts

            // Loop until the desired number of trees is spawned or maximum attempts reached
            while (spawnedTrees.Count < numberOfTrees && attemptsCount < attemptsCountMax)
            {
                // Generate a random position within the spawn area
                var randomPosition = transform.position + new Vector3(
                    Random.Range(-spawnAreaScale.x * 0.5f, spawnAreaScale.x * 0.5f),
                    0f,
                    + Random.Range(-spawnAreaScale.z * 0.5f, spawnAreaScale.z * 0.5f)
                );

                var validPosition = true;
                // Check if the randomly generated position is at a valid distance from already spawned trees
                foreach (var spawnedTree in spawnedTrees)
                {
                    if (Vector3.Distance(randomPosition, spawnedTree.Position) < minDistanceBetweenTrees)
                    {
                        validPosition = false; // Position is not valid if it's too close to an existing tree
                        ++attemptsCount; // Increment attempts count as we tried to spawn at this position
                        break;
                    }
                }

                if (validPosition)
                {
                    var newTree = Instantiate(treePrefab, randomPosition, Quaternion.identity);
#if UNITY_EDITOR
                    newTree.name = $"index:{spawnedTrees.Count + 1} - attemptsCount:{attemptsCount}";
#endif // UNITY_EDITOR
                    newTree.transform.parent = transform;

                    spawnedTrees.Add(newTree);
                    attemptsCount = 0;
                }
            }
        }

        // Draw the spawn area wireframe in the Unity editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, spawnAreaScale);
        }
    }
}