using System;
using UnityEngine;

namespace Test.Utilities.Entities
{
    /**
     * Конфигурация тайла.
     */
    [System.Serializable]
    public struct TileConfig
    {
        public string name;
        public string modelFile;
        public string materialFile;
        public float[] spawnPosition;
        public Environment environment;
        
        public void Validate()
        {
            if (string.IsNullOrEmpty(name)) Debug.LogWarning("Название тайла не задано!");
            if (string.IsNullOrEmpty(modelFile)) Debug.LogWarning("ModelFile не задан!");
            if (spawnPosition == null || spawnPosition.Length != 3) Debug.LogWarning("SpawnPosition должен содержать 3 значения!");
            environment.Validate();
        }
        
        public struct Environment
        {
            public EnvObject[] stones;
            public EnvObject[] trees;
            
            public Environment(EnvObject[] stones, EnvObject[] trees)
            {
                this.stones = stones ?? Array.Empty<EnvObject>();
                this.trees = trees ?? Array.Empty<EnvObject>();
            }
            
            public void Validate()
            {
                foreach (var stone in stones) stone.Validate();
                foreach (var tree in trees) tree.Validate();
            }
        }
        
        public struct EnvObject
        {
            public string[] modelVariants;
            public float[] position;
            public string materialFile;
            public int density;
            public float spawnRadius;
            public float checkRadius;
            public bool isVertical;
            
            public EnvObject(string[] modelVariants, float[] position, string materialFile, int density, float spawnRadius, float checkRadius, bool isVertical)
            {
                this.modelVariants = modelVariants ?? Array.Empty<string>();
                this.position = position ?? new float[3];
                this.materialFile = materialFile;
                this.density = density;
                this.spawnRadius = spawnRadius;
                this.checkRadius = checkRadius;
                this.isVertical = isVertical;
            }
            
            public void Validate()
            {
                if (modelVariants == null || modelVariants.Length == 0) Debug.LogWarning("ModelVariants пустой!");
                if (position == null || position.Length != 3) Debug.LogWarning("Position должен содержать 3 значения!");
                if (density < 0) Debug.LogWarning("Density не может быть отрицательным!");
            }
        }
    }
}