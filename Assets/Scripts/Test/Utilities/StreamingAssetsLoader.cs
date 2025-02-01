using System.Collections.Generic;
using System.IO;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public class StreamingAssetsLoader : LoaderInt
    {
        public List<GameObject> LoadEnvironment(string objectsPath, TileConfig config)
        {
            EnvironmentLoader environmentLoader = new EnvironmentLoader();
            
            
            
            return null;
        }

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

        public Material LoadMaterial(string materialsPath, TileConfig tileConfig)
        {
            return FilesLoader.LoadMaterialFromConfig(Path.Combine(materialsPath, tileConfig.materialFile + ".json"));
        }

        public Sprite LoadIcon(string iconsPath, TileConfig tileConfig)
        {
            string iconPath = Path.Combine(GetPath(), iconsPath, tileConfig.iconFile);
            return FilesLoader.LoadSpriteFromPath(iconPath);
        }

        public string GetPath()
        {
            return Application.streamingAssetsPath;
        }

        public void LoadEnvironment(string objectsPath, string modelFile, string materialsPath, string materialFile,
            GameObject parent, float[] position, float scale)
        {
            GameObject envObject = LoadModel(objectsPath, modelFile);

            envObject.transform.parent = parent.transform;
            envObject.transform.localPosition = new Vector3(
                position[0],
                position[1],
                position[2]
            );

            envObject.transform.localScale = new Vector3(scale, scale, scale);

            envObject.AddComponent<MeshCollider>();

            Material material = FilesLoader.LoadMaterialFromConfig(Path.Combine(materialsPath, materialFile + ".json"));

            MeshRenderer meshRenderer = envObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
        }

        //TODO: Вынести подгрузку окружения в отдельный метод
    }
}