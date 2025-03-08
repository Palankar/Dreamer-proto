using System;
using UnityEngine;

namespace Test
{
    /**
     * Менеджер управления режимами.
     */
    public class GameModeManager : MonoBehaviour
    {
        public Camera mainCamera;
        public TilePlacementManager tilePlacementManager;

        public enum GameMode { Building, CharacterControl }

        [SerializeField] private GameObject hexGrid;
        [SerializeField] private GameObject tileSelectionUI;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject ui;
        
        private GameMode _currentMode;
        private Transform _mainCameraTransform;

        private void Awake()
        {
            if (tilePlacementManager == null) Debug.LogError("TilePlacementManager не назначен!");
            if (mainCamera == null) Debug.LogError("MainCamera не назначена!");
            
            _mainCameraTransform = mainCamera.transform;
        }

        private void Start()
        {
            SetMode(GameMode.Building); // Устанавливаем начальный режим
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ToggleMode();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitGame();
            }
        }
        
        public void ToggleMode()
        {
            if (_currentMode == GameMode.Building)
            {
                SetMode(GameMode.CharacterControl);
            }
            else
            {
                SetMode(GameMode.Building);
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }
        
        public void SetMode(GameMode mode)
        {
            _currentMode = mode;

            switch (_currentMode)
            {
                case GameMode.Building:
                    ui.SetActive(true);
                    hexGrid.SetActive(true);
                    tileSelectionUI.SetActive(true);
                    player.SetActive(false);

                    _mainCameraTransform.SetParent(null);
                    
                    // Смещаем камеру над сеткой
                    _mainCameraTransform.position = new Vector3(0, 30, -5);
                    _mainCameraTransform.rotation = Quaternion.Euler(70, 0, 0);
                    break;

                case GameMode.CharacterControl:
                    ui.SetActive(false);
                    hexGrid.SetActive(false);
                    tileSelectionUI.SetActive(false);
                    
                    // Перемещаем персонажа в центре первого тайла
                    Vector3 spawnPosition = tilePlacementManager.GetFirstPlacedTileSpawnPosition();
                    player.transform.position = spawnPosition;// + new Vector3(0, 1, 0); // Смещаем над тайлом;
                    
                    player.SetActive(true);

                    // Смещаем камеру над персонажем
                    _mainCameraTransform.SetParent(player.transform);
                    _mainCameraTransform.localPosition = new Vector3(0, 10, -10);
                    _mainCameraTransform.localRotation = Quaternion.Euler(45, 0, 0);
                    break;
            }
        }

        public GameMode GetCurrentMode()
        {
            return _currentMode;
        }
    }
}
