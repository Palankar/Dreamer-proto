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
        public string surfaceLayer;             // Слой поверхности
        public string checkLayer;               // Слой объектов, которые нужно проверять
        public LoaderManager loaderManager;     // Менеджер загрузки 3d моделей 

        public int maxAttempts; // Количество попыток найти позицию

        private LoaderInt _loader;

        private int _surfaceLayerMask;
        private int _checkLayerMask;
        
        private Collider[] _overlapResults = new Collider[1];

        private void Awake()
        {
            _loader = loaderManager.GetLoader();
            _surfaceLayerMask = LayerMask.GetMask(surfaceLayer);
            _checkLayerMask = LayerMask.GetMask(checkLayer);
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
                Vector3 candidatePosition = Vector3.zero;

                // Пытаемся найти допустимую позицию
                bool validPositionFound = TryFindValidPosition(candidatePosition, envObject.spawnRadius, envObject.checkRadius, out candidatePosition);

                // Если найдена допустимая позиция, Raycast вниз для нахождения поверхности
                if (validPositionFound && Physics.Raycast(candidatePosition, Vector3.down, out RaycastHit hit,
                        100f, _surfaceLayerMask))
                {
                    // Определяем угол наклана поверхности
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    
                    // Пропускаем, если поверхность постановки слишком наклонная
                    if (envObject.isVertical && angle > 20f) // Порог (например, 20 градусов) можно настроить
                    {
                        continue;
                    }

                    //Выбираем случайную модель из списка
                    int randomModelIndex = Random.Range(0, envObject.modelVariants.Length);
                    string modelFile = envObject.modelVariants[randomModelIndex];

                    // Загружаем объект
                    GameObject obj = _loader.LoadModel(PathConfig.ObjectsPath, modelFile);
                    obj.transform.parent = parent.transform;

                    // Перемещаем объект на поверхность
                    obj.transform.position = hit.point;

                    // Выравниваем объект относительно нормали поверхности
                    obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

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
        
        private bool TryFindValidPosition(Vector3 spawnPoint, float spawnRadius, float checkRadius, out Vector3 position)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Генерация случайной позиции в радиусе
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                randomOffset.y = 0;
                Vector3 candidatePosition = spawnPoint + randomOffset + Vector3.up;

                // Проверка объектов в радиусе новой позиции
                int hitCount = Physics.OverlapCapsuleNonAlloc(
                    candidatePosition, 
                    new Vector3(candidatePosition.x, candidatePosition.y - 20, candidatePosition.z), 
                    checkRadius, 
                    _overlapResults, 
                    _checkLayerMask);
                
                // Если пересечений не найдено - позиция верная
                if (hitCount == 0)
                {
                    position = candidatePosition;
                    return true;
                }
            }
            position = Vector3.zero;
            return false;
        }
    }
}