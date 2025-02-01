namespace Test.Utilities.Entities
{
    [System.Serializable]
    public partial class TileConfig
    {
        public string name;
        public string modelFile;
        public string materialFile;
        public string iconFile;
        public float[] spawnPosition;
        public EnvObject[] stones;
        public EnvObject[] trees;
    }
}