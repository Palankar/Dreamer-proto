using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public class HexTile : MonoBehaviour
    {
        public int Q; // Axial координата
        public int R; // Axial координата
        public bool IsOccupied = false; // Занят ли тайл
        public GameObject spawnPosition;

        public Sprite Sprite; // Изображение тайла

        public TileConfig TileConfig; // Конфигурация тайла
    }
}
