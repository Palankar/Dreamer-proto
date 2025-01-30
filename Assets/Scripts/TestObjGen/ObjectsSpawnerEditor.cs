using UnityEditor;
using UnityEngine;

namespace TestObjGen
{
    //[CustomEditor(typeof(ObjectsSpawner))]
    public class ObjectsSpawnerEditor : Editor
    {
        private bool showCubesSettings = true;
        private bool showCylindersSettings = true;

        public override void OnInspectorGUI()
        {
            // Обновление данных из объекта
            serializedObject.Update();
            
            ObjectsSpawner spawner = (ObjectsSpawner)target;
            
            // Параметры, которые должны отображаться всегда
            EditorGUILayout.LabelField("Основные настройки", EditorStyles.boldLabel);
            spawner.surfaceLayer = EditorGUILayout.LayerField("Surface Layer", spawner.surfaceLayer);
            spawner.checkLayer = EditorGUILayout.LayerField("Check Layer", spawner.checkLayer);
            spawner.pointPrefab = (GameObject)EditorGUILayout.ObjectField("Point Prefab", spawner.pointPrefab, typeof(GameObject), true);

            // Раздел: Настройки генерации кубов
            showCubesSettings = EditorGUILayout.Foldout(showCubesSettings, "Настройки генерации кубов");
            if (showCubesSettings)
            {
                SerializedProperty cubesSpawnPoints = serializedObject.FindProperty("cubesSpawnPoints");
                EditorGUILayout.PropertyField(cubesSpawnPoints, new GUIContent("Cubes Spawn Points"), true);
                spawner.showCubesSpawns = EditorGUILayout.Toggle("Show Cubes Spawns", spawner.showCubesSpawns);
                spawner.cubePrefab = (GameObject)EditorGUILayout.ObjectField("Cube Prefab", spawner.cubePrefab, typeof(GameObject), true);
                spawner.cubesPerPoint = EditorGUILayout.IntField("Cubes Per Point", spawner.cubesPerPoint);
                spawner.cubesSpawnRadius = EditorGUILayout.FloatField("Cubes Spawn Radius", spawner.cubesSpawnRadius);
                spawner.cubesCheckRadius = EditorGUILayout.FloatField("Cubes Check Radius", spawner.cubesCheckRadius);
                spawner.cubesMaxAttempts = EditorGUILayout.IntField("Cubes Max Attempts", spawner.cubesMaxAttempts);
            }

            // Раздел: Настройки генерации цилиндров
            showCylindersSettings = EditorGUILayout.Foldout(showCylindersSettings, "Настройки генерации цилиндров");
            if (showCylindersSettings)
            {
                SerializedProperty cylindersSpawnPoints = serializedObject.FindProperty("cylindersSpawnPoints");
                EditorGUILayout.PropertyField(cylindersSpawnPoints, new GUIContent("Cylinders Spawn Points"), true);
                spawner.showCylindersSpawns = EditorGUILayout.Toggle("Show Cylinders Spawns", spawner.showCylindersSpawns);
                spawner.cylinderPrefab = (GameObject)EditorGUILayout.ObjectField("Cylinder Prefab", spawner.cylinderPrefab, typeof(GameObject), true);
                spawner.cylindersPerPoint = EditorGUILayout.IntField("Cylinders Per Point", spawner.cylindersPerPoint);
                spawner.cylindersSpawnRadius = EditorGUILayout.FloatField("Cylinders Spawn Radius", spawner.cylindersSpawnRadius);
                spawner.cylindersCheckRadius = EditorGUILayout.FloatField("Cylinders Check Radius", spawner.cylindersCheckRadius);
                spawner.cylindersMaxAttempts = EditorGUILayout.IntField("Cylinders Max Attempts", spawner.cylindersMaxAttempts);
            }

            // Сохранение изменений
            if (GUI.changed)
            {
                EditorUtility.SetDirty(spawner);
            }
            
            // Применение изменений
            serializedObject.ApplyModifiedProperties();
        }
    }
}