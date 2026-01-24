using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject window;
    
    public void OnButtonClose()
    {
        window.SetActive(false);
    }
}
