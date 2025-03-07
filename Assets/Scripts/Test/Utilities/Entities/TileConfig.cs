using System;

namespace Test.Utilities.Entities
{
    [System.Serializable]
    public struct TileConfig
    {
        public string name;
        public string modelFile;
        public string materialFile;
        public string iconFile;
        public float[] spawnPosition;
        public Environment environment;
        
        public struct Environment
        {
            public EnvObject[] stones;
            public EnvObject[] trees;
            
            public Environment(EnvObject[] stones, EnvObject[] trees)
            {
                this.stones = stones ?? Array.Empty<EnvObject>();
                this.trees = trees ?? Array.Empty<EnvObject>();
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
        }
    }
}