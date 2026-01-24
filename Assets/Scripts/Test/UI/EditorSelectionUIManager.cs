using UnityEngine;

namespace Test
{
    /**
     * Менеджер визуального интерфейса.
     */
    public class EditorSelectionUIManager : TileSelectionUIManager
    {
        private GameObject _selectedTile;
        
        protected override void SelectTile(int index)
        {
            _selectedTileIndex = index;
            Debug.Log($"Выбранный тайл: {loadedTiles[_selectedTileIndex].name}");

            // Убираем ранее установленный тайл перед установкой нового
            if (_selectedTile != null)
            {
                Destroy(_selectedTile);
            }
            
            // Устанавливаем выбранный тайл
            _selectedTile = Instantiate(loadedTiles[_selectedTileIndex], Vector3.zero, Quaternion.identity);
        }
    }
}
