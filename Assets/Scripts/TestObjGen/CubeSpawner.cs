using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // Префаб куба
    public Transform[] spawnPoints; // Массив позиций для генерации
    public LayerMask surfaceLayer; // Слой поверхности
    public LayerMask checkLayer; // Слой объектов, которые нужно проверять
    public int cubesPerPoint = 3; // Количество кубов вокруг каждой точки
    public float spawnRadius = 1.5f; // Радиус генерации кубов вокруг каждой точки
    public float checkRadius = 0.5f; // Радиус проверки вокруг новой позиции
    public int maxAttempts = 5; // Максимальное количество попыток найти допустимую позицию

    private List<GameObject> allObjects = new List<GameObject>();

    void Start()
    {
        SpawnCubes();
    }

    void SpawnCubes()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < cubesPerPoint; i++)
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
                    if (Physics.OverlapSphere(randomPosition, checkRadius, checkLayer).Length == 0)
                    {
                        validPositionFound = true;
                        break;
                    }
                }
                
                // Если найдена допустимая позиция, Raycast вниз для нахождения поверхности
                if (validPositionFound && Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, surfaceLayer))
                {
                    // Создаем куб
                    GameObject cube = Instantiate(cubePrefab, randomPosition, Quaternion.identity);

                    // Перемещаем куб на поверхность
                    cube.transform.position = hit.point;

                    // Выравниваем куб относительно нормали поверхности (можно задать проверку, чтобы не генерировался, узнавая некорректуню позицию из поворота - например на краю обрыва)
                    cube.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                    // Добавляем случайный поворот (подойдет для камней и им подобным, а деревья лучше вращать по одной оси)
                    cube.transform.Rotate(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f), Space.Self);
                    
                    allObjects.Add(cube);
                }
                else if (!validPositionFound)
                {
                    Debug.LogWarning($"Не удалось найти подходящую позицию для куба вокруг {spawnPoint.position}.");
                }
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Визуализация областей генерации и проверки
        if (spawnPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform spawnPoint in spawnPoints)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, checkRadius); // Пример радиуса проверки
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach (var obj in allObjects)
            {
                Destroy(obj);
            }
            allObjects.Clear();
            
            SpawnCubes();
        }
    }
    
    //TODO: Добавить в объект куба скрипт для показа границы, в которой не должно быть спавна. Перенести checkRadius туда. Из куба получать checkRadius тут. 
}
