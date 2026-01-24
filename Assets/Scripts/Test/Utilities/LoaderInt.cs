using System.Collections.Generic;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public interface LoaderInt
    {
        public GameObject LoadModel(string modelsPath, string modelFile);
        
        public List<GameObject> LoadAllModels(string modelsPath);
        public Dictionary<string, GameObject> LoadAllModelsWithName(string modelsPath);

        public Material LoadMaterial(string materialFile);

        public Sprite LoadIcon(string iconFile);
        
        public void LoadEnvironment(GameObject parent, TileConfig.EnvObject[] envObjects, EnvironmentLoader environmentLoader);

        public string GetPath();
    }
}