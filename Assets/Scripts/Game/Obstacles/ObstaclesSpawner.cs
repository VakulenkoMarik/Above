using UnityEngine;

namespace Game.Obstacles
{
    public class ObstaclesSpawner : MonoBehaviour
    {
        [SerializeField] private ObstaclesForTheLevel obstaclesList;
        [SerializeField] private Transform deadLineForObstacles;

        private float _heightOfNewObstaclesSpawn;
        private Vector2 _spawnPos;

        private void Start()
        {
            SpawnObstacles();
        }

        private void Update()
        {
            if (transform.position.y > _heightOfNewObstaclesSpawn)
            {
                SpawnObstacles();
            }
        }

        private void SpawnObstacles()
        {
            int countOfIterations = 0;

            for (int i = 0; i < 5; i++)
            {
                countOfIterations++;

                float additionalDistance = Random.Range(4, 7);
                _spawnPos.x = transform.position.x;

                GameObject empty = obstaclesList.obstacles[Random.Range(0, obstaclesList.obstacles.Count)];

                if (additionalDistance + _spawnPos.y > deadLineForObstacles.position.y)
                {
                    return;
                }

                _spawnPos.y += additionalDistance;

                if (countOfIterations <= 4)
                {
                    _heightOfNewObstaclesSpawn = _spawnPos.y - 7;
                }

                Instantiate(empty, _spawnPos, Quaternion.identity);
            }
        }
    }
}