using System;
using System.Collections.Generic;
using Test.Configs;
using Test.Utilities.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Test
{
    public class EnvironmentLoader : MonoBehaviour
    {
        public string surfaceLayer;     // Слой поверхности
        public string checkLayer;       // Слой объектов, которые нужно проверять
        public LoaderManager loaderManager;

        public int maxAttempts; // Количество попыток найти позицию

        private LoaderInt _loader;

        private void Awake()
        {
            _loader = loaderManager.GetLoader();
        }

        public List<GameObject> SpawnObjects(TileConfig.EnvObject envObject, GameObject parent)
        {
            Vector3 spawnPoint = parent.transform.position + new Vector3(
                envObject.position[0],
                envObject.position[1], 
                envObject.position[2]);
            
            List<GameObject> spawnedObjects = new List<GameObject>();

            for (int i = 0; i < envObject.density; i++)
            {
                Vector3 randomPosition = Vector3.zero;
                bool validPositionFound = false;

                // Пытаемся найти допустимую позицию
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    // Генерация случайной позиции в радиусе
                    Vector3 randomOffset = Random.insideUnitSphere * envObject.spawnRadius;
                    randomOffset.y = 0; // Убираем смещение по оси Y
                    randomPosition = spawnPoint + randomOffset + Vector3.up;

                    // Проверка объектов в радиусе новой позиции
                    if (Physics.OverlapCapsule(randomPosition,
                            new Vector3(randomPosition.x, randomPosition.y - 20, randomPosition.z),
                            envObject.checkRadius,
                            LayerMask.GetMask(checkLayer)).Length == 0)
                    {
                        validPositionFound = true;
                        break;
                    }
                }
                
                // Если найдена допустимая позиция, Raycast вниз для нахождения поверхности
                if (validPositionFound && Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit,
                        100f, LayerMask.GetMask(surfaceLayer)))
                {
                    // Определяем наклон нормали поверхности
                    Quaternion normalSlope = Quaternion.FromToRotation(Vector3.up, hit.normal);

                    // Пропускаем, если поверхность постановки слишком наклонная
                    if (envObject.isVertical &&
                        (Math.Abs(normalSlope.x) > 0.35f || Math.Abs(normalSlope.z) > 0.35f ||
                         Math.Abs(normalSlope.y) > 0.35f))
                    {
                        continue;
                    }
                    
                    //Выбираем случайную модель из списка
                    int randomModelIndex = Random.Range(0, envObject.modelVariants.Length - 1);
                    string modelFile = envObject.modelVariants[randomModelIndex];

                    // Загружаем объект
                    GameObject obj = _loader.LoadModel(PathConfig.ObjectsPath, modelFile);
                    obj.transform.parent = parent.transform;

                    // Перемещаем объект на поверхность
                    obj.transform.position = hit.point;

                    // Выравниваем объект относительно нормали поверхности
                    obj.transform.rotation = normalSlope;

                    // Логика для свободных и вертикальных объектов
                    if (envObject.isVertical)
                    {
                        // Добавляем случайный поворот только по оси Y
                        obj.transform.Rotate(0, Random.Range(0f, 360f), 0, Space.Self);
                    }
                    else
                    {
                        // Добавляем случайный поворот
                        obj.transform.Rotate(
                            Random.Range(0f, 360f), 
                            Random.Range(0f, 360f), 
                            Random.Range(0f, 360f),
                            Space.Self);
                    }

                    obj.layer = LayerMask.NameToLayer(checkLayer);

                    spawnedObjects.Add(obj);
                }
                else if (!validPositionFound)
                {
                    Debug.LogWarning(
                        $"Не удалось найти подходящую позицию для объекта вокруг {spawnPoint}.");
                }
            }
            return spawnedObjects;
        }
    }
}