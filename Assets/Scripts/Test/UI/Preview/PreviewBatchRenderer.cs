using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewBatchRenderer : MonoBehaviour
{
    [System.Serializable]
    public class PreviewItem
    {
        public GameObject prefab;
        public RawImage rawImage;
        public RenderTexture renderTexture;
    }

    public Camera previewCamera;
    public Transform previewRoot;
    public List<PreviewItem> items = new();

    GameObject current;

    public void SetItems(List<PreviewItem> newItems)
    {
        items = newItems;
    }

    public void RenderAll()
    {
        for (int i = 0; i < items.Count; i++)
            RenderOne(items[i]);
    }

    void RenderOne(PreviewItem item)
    {
        if (!item.prefab || !item.rawImage || !item.renderTexture) return;

        if (current) DestroyImmediate(current);
        current = Instantiate(item.prefab, previewRoot);
        SetLayerRecursively(current, LayerMask.NameToLayer("UI3DPreview"));

        FrameCameraToObject(current);

        previewCamera.targetTexture = item.renderTexture;
        previewCamera.enabled = false;
        previewCamera.Render();

        item.rawImage.texture = item.renderTexture;
    }

    void FrameCameraToObject(GameObject go)
    {
        var r = go.GetComponentInChildren<Renderer>();
        if (!r) return;

        var b = r.bounds;
        var center = b.center;
        var size = b.extents.magnitude;

        // Смещение камеры: сверху + под углом
        Vector3 offset = new Vector3(
            0f,            // вбок
            size * 0.8f,   // высота
            -size * 1.2f   // назад
        );

        previewCamera.transform.position = center + offset;
        previewCamera.transform.LookAt(center);
    }

    void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform)
            SetLayerRecursively(t.gameObject, layer);
    }
}