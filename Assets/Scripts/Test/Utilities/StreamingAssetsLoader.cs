using System.Collections.Generic;
using System.IO;
using System.Linq;
using Test.Configs;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    /**
     * Загрузчик объектов из StreamingAssets.
     */
    public class StreamingAssetsLoader : LoaderInt
    {
        public GameObject LoadModel(string modelsPath, string modelFile)
        {
            string modelPath = Path.Combine(Application.streamingAssetsPath, modelsPath, modelFile);
            Mesh mesh = FilesLoader.LoadObjMeshFromPath(modelPath);

            GameObject model = new GameObject(Path.GetFileNameWithoutExtension(modelFile));

            // Привязка Mesh к объекту
            MeshFilter meshFilter = model.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            return model;
        }

        public List<GameObject> LoadAllModels(string modelsPath)
        {
            string modelPath = Path.Combine(Application.streamingAssetsPath, modelsPath);
            List<GameObject> models = new List<GameObject>();
            
            List<string> modelsNames = Directory
                .GetFiles(modelPath)
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            foreach (string modelName in modelsNames)
            {
                models.Add(LoadModel(modelsPath, modelName));
            }
            return models;
        }

        public Dictionary<string, GameObject> LoadAllModelsWithName(string modelsPath)
        {
            string modelPath = Path.Combine(Application.streamingAssetsPath, modelsPath);
            Dictionary<string, GameObject> modelsWithName = new Dictionary<string, GameObject>();
            
            List<string> modelsNames = Directory
                .GetFiles(modelPath, "*.obj")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
            
            foreach (string modelName in modelsNames)
            {
                modelsWithName.Add(modelName, LoadModel(modelsPath, modelName + ".obj"));
            }
            return modelsWithName;
        }

        public Material LoadMaterial(string materialFile)
        {
            return FilesLoader.LoadMaterialFromConfig(Path.Combine(PathConfig.MaterialsPath, materialFile + ".json"));
        }

        public Sprite LoadIcon(string iconFile)
        {
            string iconPath = Path.Combine(GetPath(), PathConfig.IconsPath, iconFile);
            return FilesLoader.LoadSpriteFromPath(iconPath);
        }

        public string GetPath()
        {
            return Application.streamingAssetsPath;
        }

        public void LoadEnvironment(GameObject parent, TileConfig.EnvObject[] envObjects, EnvironmentLoader environmentLoader)
        {
            foreach (var envObject in envObjects)
            {
                List<GameObject> spawnedObjects = environmentLoader.SpawnObjects(envObject, parent);
                
                Material material = FilesLoader.LoadMaterialFromConfig(Path.Combine(PathConfig.MaterialsPath, envObject.materialFile + ".json"));
                
                foreach (GameObject spawnedObject in spawnedObjects)
                {
                    spawnedObject.AddComponent<MeshCollider>();
                    
                    MeshRenderer meshRenderer = spawnedObject.AddComponent<MeshRenderer>();
                    meshRenderer.material = material;
                }
            }
        }
    }
}