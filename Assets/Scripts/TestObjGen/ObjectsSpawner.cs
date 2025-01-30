using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestObjGen
{
    public class ObjectsSpawner : MonoBehaviour
    {
        public LayerMask surfaceLayer; // Слой поверхности
        public LayerMask checkLayer; // Слой объектов, которые нужно проверять
        public bool showCubesSpawns = false;
        public bool showCylindersSpawns = false;
        public GameObject pointPrefab;

        public Transform[] cubesSpawnPoints; // Массив позиций для генерации
        public Transform[] cylindersSpawnPoints; // Массив позиций для генерации

        public GameObject cubePrefab; // Префаб куба
        public int cubesPerPoint = 3; // Количество кубов вокруг каждой точки
        public float cubesSpawnRadius = 1.5f; // Радиус генерации кубов вокруг каждой точки
        public float cubesCheckRadius = 0.5f; // Радиус проверки вокруг новой позиции
        public int cubesMaxAttempts = 5; // Максимальное количество попыток найти допустимую позицию

        public GameObject cylinderPrefab; // Префаб цилиндра
        public int cylindersPerPoint = 7; // Количество цилиндров вокруг каждой точки
        public float cylindersSpawnRadius = 1.5f; // Радиус генерации цилиндров вокруг каждой точки
        public float cylindersCheckRadius = 0.5f; // Радиус проверки вокруг новой позиции
        public int cylindersMaxAttempts = 5; // Максимальное количество попыток найти допустимую позицию

        private List<GameObject> allCubes = new List<GameObject>();
        private List<GameObject> allCylinders = new List<GameObject>();

        void Start()
        {
            SpawnCylinders();
            SpawnCubes();
        }

        void SpawnCubes()
        {
            SpawnObjects(cubePrefab, cubesPerPoint, cubesSpawnPoints, cubesSpawnRadius, cubesCheckRadius,
                cubesMaxAttempts, allCubes, false);
        }

        void SpawnCylinders()
        {
            SpawnObjects(cylinderPrefab, cylindersPerPoint, cylindersSpawnPoints, cylindersSpawnRadius,
                cylindersCheckRadius, cylindersMaxAttempts, allCylinders, true);
        }

        void SpawnObjects(GameObject modelPrefab, int modelsPerPoint, Transform[] spawnPoints, float spawnRadius,
            float checkRadius, int maxAttempts, List<GameObject> allObjects, bool isVertical)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                for (int i = 0; i < modelsPerPoint; i++)
                {
                    Vector3 randomPosition = Vector3.zero;
                    bool validPositionFound = false;

                    // Пытаемся найти допустимую позицию
                    for (int attempt = 0; attempt < maxAttempts; attempt++)
                    {
                        // Генерация случайной позиции в радиусе
                        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                        randomOffset.y = 0; // Убираем смещение по оси Y
                        randomPosition = spawnPoint.position + randomOffset;

                        // Проверка объектов в радиусе новой позиции
                        if (Physics.OverlapCapsule(randomPosition, 
                                new Vector3(randomPosition.x, randomPosition.y - 20, randomPosition.z) , 
                                checkRadius, 
                                checkLayer).Length == 0)
                        {
                            validPositionFound = true;
                            //Instantiate(pointPrefab, randomPosition, Quaternion.identity); //Визуальная проверка корректности позиции
                            break;
                        }
                    }
                    //Debug.DrawRay(randomPosition, Vector3.down * 100f, Color.green, 10f); //Визуальная проверка корректности направления
                    
                    // Если найдена допустимая позиция, Raycast вниз для нахождения поверхности
                    if (validPositionFound && Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit,
                            100f, surfaceLayer))
                    {
                        // Определяем наклон нормали поверхности
                        Quaternion normalSlope = Quaternion.FromToRotation(Vector3.up, hit.normal);

                        // Пропускаем, если поверхность постановки слишком наклонная
                        if (isVertical && 
                            (Math.Abs(normalSlope.x) > 0.35f || Math.Abs(normalSlope.z) > 0.35f || Math.Abs(normalSlope.y) > 0.35f))
                        {
                            continue;
                        }
                        
                        // Создаем объект
                        GameObject obj = Instantiate(modelPrefab, randomPosition, Quaternion.identity);

                        // Перемещаем объект на поверхность
                        obj.transform.position = hit.point;

                        // Выравниваем объект относительно нормали поверхности
                        obj.transform.rotation = normalSlope;

                        // Логика для свободных и вертикальных объектов
                        if (isVertical)
                        {
                            // Добавляем случайный поворот только по оси Y
                            obj.transform.Rotate(0, Random.Range(0f, 360f), 0, Space.Self);
                        }
                        else
                        {
                            // Добавляем случайный поворот
                            obj.transform.Rotate(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f),
                                Space.Self);
                        }

                        allObjects.Add(obj);
                    }
                    else if (!validPositionFound)
                    {
                        Debug.LogWarning(
                            $"Не удалось найти подходящую позицию для объекта вокруг {spawnPoint.position}.");
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            // Визуализация областей генерации и проверки кубов
            if (cubesSpawnPoints != null && showCubesSpawns)
            {
                Gizmos.color = Color.yellow;
                foreach (Transform spawnPoint in cubesSpawnPoints)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, cubesSpawnRadius);
                }
            }

            // Визуализация областей генерации и проверки цилиндров
            if (cylindersSpawnPoints != null && showCylindersSpawns)
            {
                Gizmos.color = Color.yellow;
                foreach (Transform spawnPoint in cylindersSpawnPoints)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, cylindersSpawnRadius);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                foreach (var obj in allCubes)
                {
                    Destroy(obj);
                }

                allCubes.Clear();

                SpawnCubes();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                foreach (var obj in allCylinders)
                {
                    Destroy(obj);
                }

                allCylinders.Clear();

                SpawnCylinders();
            }
        }
    }
    
    //TODO: Можно isVertical заменить на enum, чтобы была возможность определять, насколько можно отклоняться по оси Y
}