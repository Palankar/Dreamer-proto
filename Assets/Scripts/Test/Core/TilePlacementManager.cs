using System.Collections.Generic;
using Test.Utilities.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
    public class TilePlacementManager : MonoBehaviour
    {
        public TileSelectionUIManager tileSelectionUIManager; // Управление UI размещения тайлов
        public EnvironmentLoader environmentLoader;
        public LoaderManager loaderManager;

        public string tileLayer;
        public LayerMask hexLayer; // Слой гексов сетки
        public LayerMask placementLayer; // Слой размещаемых тайлов
        
        private HexCell _lastHoveredCell; // Последняя ячейка, над которой была мышь
        private List<GameObject> _placedTiles = new();
        
        [SerializeField] private GameModeManager gameModeManager;
        
        private void Update()
        {
            if (gameModeManager.GetCurrentMode() != GameModeManager.GameMode.Building)
                return;
            
            HandleHover();
            if (Input.GetMouseButtonDown(0)) // Проверка на клик левой кнопкой мыши
            {
                // Игнорируем клик, если курсор находится над элементом интерфейса
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                
                PlaceTileAtMousePosition();
            }
        }
        
        private void HandleHover()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexLayer))
            {
                HexCell cell = hit.collider.GetComponent<HexCell>();

                if (cell != null)
                {
                    if (cell != _lastHoveredCell)
                    {
                        if (_lastHoveredCell != null)
                        {
                            _lastHoveredCell.ResetColor(); // Сбрасываем цвет у предыдущей ячейки
                        }

                        cell.ShowHoverColor(); // Показываем цвет при наведении
                        _lastHoveredCell = cell;
                    }
                }
            }
        }

        private void PlaceTileAtMousePosition()
        {
            // Определяем, куда кликнул пользователь
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем попадание на гекс сетки
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexLayer))
            {
                GameObject cell = hit.collider.gameObject;
                GameObject selectedTile = tileSelectionUIManager.GetSelectedTilePrefab();
                
                // Задаем координаты устанавливаемому тайлу
                selectedTile.GetComponent<HexTile>().Q = cell.GetComponent<HexTile>().Q;
                selectedTile.GetComponent<HexTile>().R = cell.GetComponent<HexTile>().R;
                if (selectedTile != null)
                {
                    // Получаем точную позицию гекса
                    Vector3 hitPosition = hit.transform.position;

                    // Убираем старый гекс ячейки
                    Destroy(cell);

                    // Размещаем новый тайл
                    GameObject placedTile = Instantiate(selectedTile, hitPosition, Quaternion.identity);
                    placedTile.GetComponent<HexTile>().TileConfig = selectedTile.GetComponent<HexTile>().TileConfig;

                    placedTile.layer = LayerMask.NameToLayer(tileLayer);
                    
                    SetEnvironmentOnTile(loaderManager.GetLoader(), placedTile);
                    
                    _placedTiles.Add(placedTile);
                }
                else
                {
                    Debug.Log("Для начала нужно выбрать тайл");
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
            {
                Debug.Log("Тайл уже установлен в этой ячейке");
            }
        }
        
        public Vector3 GetFirstPlacedTileSpawnPosition()
        {
            // Возвращаем позицию первого установленного тайла
            if (_placedTiles.Count > 0)
            {
                return _placedTiles[0].GetComponent<HexTile>().spawnPosition.transform.position;
            }
            return Vector3.zero; // Возврат по умолчанию
        }

        private void SetEnvironmentOnTile(LoaderInt loader, GameObject tile)
        {
            //TODO: Перенос логики наполнения объектами в метод с простановкой тайла, а не грузить предварительно
            // Загрузка объектов окружения - камней
            GameObject stonesParent = new GameObject("Stones");
            LoadEnvironment(loader, stonesParent, tile, tile.GetComponent<HexTile>().TileConfig.environment.stones);

            // Загрузка объектов окружения - деревьев
            GameObject treesParent = new GameObject("Trees");
            LoadEnvironment(loader, treesParent, tile, tile.GetComponent<HexTile>().TileConfig.environment.trees);
        }
        
        private void LoadEnvironment(LoaderInt loader, GameObject parent, GameObject tile, TileConfig.EnvObject[] envObjects)
        {
            parent.transform.parent = tile.transform;
            parent.transform.localPosition = Vector3.zero;
            loader.LoadEnvironment(parent, envObjects, environmentLoader);
        }
    }
}
