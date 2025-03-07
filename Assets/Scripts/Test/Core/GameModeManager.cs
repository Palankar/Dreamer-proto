using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Test
{
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
            Transform mainCameraTransform = mainCamera.transform;

            switch (_currentMode)
            {
                case GameMode.Building:
                    ui.SetActive(true);
                    hexGrid.SetActive(true);
                    tileSelectionUI.SetActive(true);
                    player.SetActive(false);

                    mainCameraTransform.SetParent(null);
                    
                    // Смещаем камеру над сеткой
                    mainCameraTransform.position = new Vector3(0, 30, -5);
                    mainCameraTransform.rotation = Quaternion.Euler(70, 0, 0);
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
                    mainCameraTransform.SetParent(player.transform);
                    mainCameraTransform.localPosition = new Vector3(0, 10, -10);
                    mainCameraTransform.localRotation = Quaternion.Euler(45, 0, 0);
                    break;
            }
        }

        public GameMode GetCurrentMode()
        {
            return _currentMode;
        }
    }
}
