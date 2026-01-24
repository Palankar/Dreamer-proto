using System.Collections.Generic;
using Test;
using UnityEngine;
using UnityEngine.UI;

public class TileSelectionUIManager : MonoBehaviour
{
    public RectTransform buttonContainer;
    public GameObject tileLoaderManager;

    [Header("Preview")]
    public PreviewBatchRenderer previewBatchRenderer;
    public List<RenderTexture> buttonRenderTextures; // 5 штук

    [Header("Scroll")]
    public Scrollbar scrollbar;

    protected List<GameObject> loadedTiles = new();
    protected TileLoader _tileLoader;

    protected int _selectedTileIndex = -1;

    private readonly List<Button> _buttons = new();
    private const int TilesPerPage = 5;

    private int _currentStartIndex = 0;
    private bool _suppressScrollbarCallback;

    void Awake()
    {
        if (!buttonContainer) Debug.LogError("ButtonContainer не назначен!");
        if (!tileLoaderManager) Debug.LogError("TileLoaderManager не назначен!");
        if (!previewBatchRenderer) Debug.LogError("PreviewBatchRenderer не назначен!");
        if (!scrollbar) Debug.LogError("Scrollbar не назначен!");
    }

    void Start()
    {
        _tileLoader = tileLoaderManager.GetComponent<TileLoader>();
        loadedTiles = _tileLoader.LoadTiles();

        CacheButtonsFromContainer();

        // Подписка на скролл
        scrollbar.onValueChanged.AddListener(OnScrollChanged);

        SetupScrollbar();       // настроим размер “окна” и стартовую позицию
        RefreshVisibleTiles(0); // первая страница
    }

    void OnDestroy()
    {
        if (scrollbar) scrollbar.onValueChanged.RemoveListener(OnScrollChanged);
    }

    void CacheButtonsFromContainer()
    {
        _buttons.Clear();
        buttonContainer.GetComponentsInChildren(true, _buttons);

        if (_buttons.Count < TilesPerPage)
            Debug.LogWarning($"Кнопок меньше чем {TilesPerPage}: {_buttons.Count}");
    }

    void SetupScrollbar()
    {
        // Важно: Scrollbar.value в диапазоне [0..1]
        // size — “размер окна” относительно всего “контента” (опционально, но приятно)
        if (loadedTiles.Count <= TilesPerPage)
        {
            // всё помещается — скролл не нужен
            _suppressScrollbarCallback = true;
            scrollbar.size = 1f;
            scrollbar.value = 0f;
            scrollbar.interactable = false;
            _suppressScrollbarCallback = false;
            return;
        }

        scrollbar.interactable = true;
        scrollbar.size = Mathf.Clamp01((float)TilesPerPage / loadedTiles.Count);

        // старт: в начало
        _suppressScrollbarCallback = true;
        scrollbar.value = 0f;
        _suppressScrollbarCallback = false;
    }

    void OnScrollChanged(float value)
    {
        if (_suppressScrollbarCallback) return;

        int maxStart = Mathf.Max(0, loadedTiles.Count - TilesPerPage);

        // Превращаем value [0..1] в startIndex [0..maxStart]
        // Обычно лучше FLOOR, чтобы при небольшом движении не “дёргалось”.
        int startIndex = Mathf.FloorToInt(value * (maxStart + 0.0001f));

        if (startIndex == _currentStartIndex) return;

        RefreshVisibleTiles(startIndex);
    }

    void RefreshVisibleTiles(int startIndex)
    {
        _currentStartIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, loadedTiles.Count - TilesPerPage));

        int count = Mathf.Min(TilesPerPage, _buttons.Count);

        var items = new List<PreviewBatchRenderer.PreviewItem>(count);

        for (int i = 0; i < count; i++)
        {
            var btn = _buttons[i];

            int tileIndex = _currentStartIndex + i;

            if (tileIndex >= loadedTiles.Count)
            {
                // Нет тайла для этой кнопки — выключаем кнопку
                btn.interactable = false;
                btn.gameObject.SetActive(false);
                continue;
            }

            btn.gameObject.SetActive(true);
            btn.interactable = true;

            var tilePrefab = loadedTiles[tileIndex];

            // RawImage внутри кнопки
            var raw = btn.GetComponentInChildren<RawImage>(true);
            if (!raw)
            {
                Debug.LogError($"На кнопке #{i} нет RawImage для превью!");
                continue;
            }

            // Клик выбирает ИМЕННО tileIndex (глобальный индекс!)
            int capturedIndex = tileIndex;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SelectTile(capturedIndex));

            // RenderTexture для этой кнопки
            RenderTexture rt = (buttonRenderTextures != null && i < buttonRenderTextures.Count)
                ? buttonRenderTextures[i]
                : null;

            if (!rt)
            {
                Debug.LogError($"Не задан RenderTexture для кнопки #{i}. Добавь 5 RT в инспектор.");
                continue;
            }

            items.Add(new PreviewBatchRenderer.PreviewItem
            {
                prefab = tilePrefab,
                rawImage = raw,
                renderTexture = rt
            });
        }

        previewBatchRenderer.SetItems(items);
        previewBatchRenderer.RenderAll();
    }

    protected virtual void SelectTile(int index)
    {
        _selectedTileIndex = index;
        Debug.Log($"Выбранный тайл: {loadedTiles[_selectedTileIndex].name}");
    }

    public GameObject GetSelectedTilePrefab()
    {
        if (_selectedTileIndex >= 0 && _selectedTileIndex < loadedTiles.Count)
            return loadedTiles[_selectedTileIndex];
        return null;
    }
}
