using UnityEngine;
using TMPro;

public class ResourceDisplayController : MonoBehaviour
{
    private ResourceManager resourceManager;
    private TextMeshProUGUI resourceText;

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        resourceText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        resourceText.text = resourceManager.GetResourceString();
    }
}
