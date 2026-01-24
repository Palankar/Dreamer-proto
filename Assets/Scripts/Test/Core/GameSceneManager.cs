using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private enum GameScene { MainScene, EditorScene }
    
    private Scene  _activeScene;

    void Awake()
    {
        _activeScene = SceneManager.GetActiveScene();
    }
    
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ToggleScene();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }
    
    private void ToggleScene()
    {
        switch (_activeScene.name)
        {
            case nameof(GameScene.MainScene):
                SceneManager.LoadScene(nameof(GameScene.EditorScene));
                break;
            case nameof(GameScene.EditorScene):
                SceneManager.LoadScene(nameof(GameScene.MainScene));
                break;
        }
    }
    
    private void ExitGame()
    {
        Application.Quit();
    }
}
