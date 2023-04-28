using UnityEngine;
using UnityEngine.UI;
//exchange building, similiar as barrack
public class ExchangeBuilding : MonoBehaviour
{
    public GameObject exchangePanel;
    public Button exchangeGemsToGoldButton;
    public Button exchangeGoldToGasButton;
    private ResourceManager resourceManager;

    private void Awake()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager == null)
        {
            Debug.LogError("Could not find ResourceManager object in scene!");
        }
    }

    private void Start()
    {
        exchangePanel.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!exchangePanel.activeSelf)
        {
            exchangePanel.SetActive(true);
        }
        else
        {
            exchangePanel.SetActive(false);
        }
    }

    //exchange gems to gold, on-click
    public void ExchangeGemsToGold()
    {
        int gemsCost = 100;
        int goldGain = 50;

        if (resourceManager.gems >= gemsCost)
        {
            resourceManager.AddResources(goldGain, -gemsCost, 0);
        }
    }
    //exchange gold to gas, on-click
    public void ExchangeGoldToGas()
    {
        int goldCost = 100;
        int gasGain = 50;

        if (resourceManager.gold >= goldCost)
        {
            resourceManager.AddResources(-goldCost, 0, gasGain);
        }
    }
}
