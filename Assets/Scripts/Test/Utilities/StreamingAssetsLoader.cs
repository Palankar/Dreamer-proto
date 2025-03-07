using System.Collections.Generic;
using System.IO;
using Test.Configs;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public class StreamingAssetsLoader : LoaderInt
    {
        public GameObject LoadModel(string modelsPath, string modelFile)
        {
            string modelPath = Path.Combine(Application.streamingAssetsPath, modelsPath, modelFile);
            Mesh mesh = FilesLoader.LoadObjMeshFromPath(modelPath);

            GameObject model = new GameObject(modelFile.Substring(0, modelFile.LastIndexOf('.')));

            // Привязка Mesh к объекту
            MeshFilter meshFilter = model.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            return model;
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