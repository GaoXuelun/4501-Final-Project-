using UnityEngine;
using UnityEngine.UI;

public class Barrack : MonoBehaviour
{
    private BuildingManager buildingManager;
    public GameObject upgradePanel;
    //4 buttons of barrack
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button footmanButton;
    [SerializeField] private Button plagueDoctorButton;
    [SerializeField] private Button warlockButton;
    //prefabs and selected circle
    public GameObject footmanPrefab;
    public GameObject plagueDoctorPrefab;
    public GameObject warlockPrefab;
    private GameObject SelectedCircle;
    //inital level
    private int barrackLevel = 1;
    public int instanceID;  //make sure that only 1 panel can be displayed at same time
    //cost
    public int upgradeCostGold = 200;
    public int upgradeCostGems = 100;
    public int upgradeCostAlchemicalGas = 50;

    public int footmanCostGold = 50;
    public int footmanCostGems = 25;
    public int footmanCostAlchemicalGas = 10;

    public int plagueDoctorCostGold = 75;
    public int plagueDoctorCostGems = 40;
    public int plagueDoctorCostAlchemicalGas = 20;

    public int warlockCostGold = 100;
    public int warlockCostGems = 60;
    public int warlockCostAlchemicalGas = 30;
    private ResourceManager resourceManager;

    private void Awake()
    {
        instanceID = GetInstanceID();
        SelectedCircle = this.transform.Find("SelectedCircle").gameObject;  // get Child
        SetSelected(false);
        resourceManager = FindObjectOfType<ResourceManager>();

        if (resourceManager == null)
        {
            Debug.LogError("Could not find ResourceManager object in scene!");
        }
    }

    private void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        if (buildingManager == null)
        {
            Debug.LogError("Could not find BuildingManager object in scene!");
        }
        UpdateUnitButtons();
    }

    private void Update()
    {
        UpdateUnitButtons();    //update status of buttons
    }

    public void SetSelected(bool isSelected) // Enable SelectedCircle when selected
    {
        if (SelectedCircle != null)
            SelectedCircle.SetActive(isSelected);
    }

    private void OnMouseDown()
    {
        if (!buildingManager.IsPlacingBuilding)
        {
            // Hide other upgrade panels
            foreach (Barrack otherBarrack in FindObjectsOfType<Barrack>())
            {
                if (otherBarrack.instanceID != instanceID)
                {
                    otherBarrack.HideUpgradePanel();
                }
            }
            SetSelected(true);
            upgradePanel.SetActive(!upgradePanel.activeSelf);   //active the panel
        }
    }
    public void HideUpgradePanel()  //deactive the panel
    {
        upgradePanel.SetActive(false);
        SetSelected(false);
    }
    //upgrade units
    public void UpgradeBarrack()
    {
        //check if barrack can be upgraded
        if (barrackLevel < 3 && resourceManager.HasEnoughResources(upgradeCostGold, upgradeCostGems, upgradeCostAlchemicalGas))
        {
            resourceManager.ConsumeResources(upgradeCostGold, upgradeCostGems, upgradeCostAlchemicalGas);
            barrackLevel++;
            UpdateUnitButtons();
        }
    
    }
    //produce units, on-click
    public void CreateUnit(int unitType)
    {
        // 0: Footman, 1: Plague Doctor, 2: Warlock
        GameObject unitPrefab = null;

        int costGold = 0;
        int costGems = 0;
        int costAlchemicalGas = 0;

        switch (unitType)
        {
            case 0:
                costGold = footmanCostGold;
                costGems = footmanCostGems;
                costAlchemicalGas = footmanCostAlchemicalGas;
                unitPrefab = footmanPrefab;
                break;
            case 1:
                if (barrackLevel >= 2)
                {
                    costGold = plagueDoctorCostGold;
                    costGems = plagueDoctorCostGems;
                    costAlchemicalGas = plagueDoctorCostAlchemicalGas;
                    unitPrefab = plagueDoctorPrefab;
                }
                break;
            case 2:
                if (barrackLevel == 3)
                {
                    costGold = warlockCostGold;
                    costGems = warlockCostGems;
                    costAlchemicalGas = warlockCostAlchemicalGas;
                    unitPrefab = warlockPrefab;
                }
                break;
            default:
                Debug.LogError("Invalid unit type!");
                break;
        }
        //calculate position of produced units
        if (unitPrefab != null && resourceManager.HasEnoughResources(costGold, costGems, costAlchemicalGas))
        {
            resourceManager.ConsumeResources(costGold, costGems, costAlchemicalGas);//
            float outerRadius = 10.0f;
            float innerRadius = 7.0f;
            Vector3 spawnPos = GetSpawnPosition(transform.position, innerRadius, outerRadius);
            Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        }
    }

    private Vector3 GetSpawnPosition(Vector3 center, float innerRadius, float outerRadius)
    {
        Vector2 randomPointInCircle = Random.insideUnitCircle.normalized;
        float randomRadius = Random.Range(innerRadius, outerRadius);
        Vector2 randomPointInDonut = randomPointInCircle * randomRadius;

        return center + new Vector3(randomPointInDonut.x, 0.0f, randomPointInDonut.y);
    }


    //check if buttons can be interacted
    private void UpdateUnitButtons()
    {
        if (upgradeButton != null && footmanButton != null && plagueDoctorButton != null && warlockButton != null)
        {
            upgradeButton.interactable = barrackLevel < 3;
            footmanButton.interactable = barrackLevel >= 1;
            plagueDoctorButton.interactable = barrackLevel >= 2;
            warlockButton.interactable = barrackLevel == 3;
        }
        else
        {
            Debug.LogError("One or more of the unit buttons is null! Please check the BarrackUI object in the inspector.");
        }
    }
}