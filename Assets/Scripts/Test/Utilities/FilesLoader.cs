using System.IO;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public static class FilesLoader
    {
        public static Material LoadMaterialFromConfig(string configFilePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, configFilePath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Material config file not found: {fullPath}");
                return null;
            }

            // Чтение конфигурации
            string jsonContent = File.ReadAllText(fullPath);
            MaterialConfig config = JsonUtility.FromJson<MaterialConfig>(jsonContent);

            // Создание материала
            Shader shader = Shader.Find(config.shader);
            if (shader == null)
            {
                Debug.LogError($"Shader not found: {config.shader}");
                return null;
            }

            Material material = new Material(shader);

            // Установка цвета
            Color color = new Color(config.color[0], config.color[1], config.color[2], config.color[3]);
            material.color = color;

            // Загрузка текстуры
            if (!string.IsNullOrEmpty(config.texture))
            {
                string texturePath = Path.Combine(Application.streamingAssetsPath, config.texture);
                if (File.Exists(texturePath))
                {
                    byte[] textureData = File.ReadAllBytes(texturePath);
                    Texture2D texture = new Texture2D(2, 2); // Создаем пустую текстуру
                    texture.LoadImage(textureData); // Загружаем изображение
                    material.mainTexture = texture;
                }
                else
                {
                    Debug.LogWarning($"Texture file not found: {texturePath}");
                }
            }

            return material;
        }

        public static Sprite LoadSpriteFromPath(string path)
        {
            if (File.Exists(path))
            {
                byte[] textureData = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2); // Создаем пустую текстуру
                texture.LoadImage(textureData); // Загружаем изображение
                return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero);
            }
            Debug.LogWarning($"Sprite file not found: {path}");
            return null;
        }

        public static Mesh LoadObjMeshFromPath(string path)
        {
            return ObjParser.Parse(path);
        }
    }
}