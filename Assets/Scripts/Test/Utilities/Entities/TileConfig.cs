namespace Test.Utilities.Entities
{
    [System.Serializable]
    public class TileConfig
    {
        public string name;
        public string modelFile;
        public string materialFile;
        public string iconFile;
        public float[] spawnPosition;
        public EnvObject[] stones;
        public EnvObject[] trees;

        public class EnvObject
        {
            public string modelFile;
            public float[] position;
            public string materialFile;
        }
    }
}