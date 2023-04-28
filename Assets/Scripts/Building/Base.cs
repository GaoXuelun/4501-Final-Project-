using UnityEngine;
using UnityEngine.UI;
// similiar as Barrack
public class Base : MonoBehaviour
{
    private BuildingManager buildingManager;
    public GameObject upgradePanel;
    [SerializeField] private Button peasantButton;
    public GameObject peasantPrefab;
    public int instanceID;

    public int peasantGoldCost = 50;
    public int peasantGemsCost = 10;
    public int peasantAlchemicalGasCost = 5;

    private float waitTime = 10.0f;
    private float timer = 0.0f;
    private float scrollBar = 1.0f;
    private float visualTime = 0.0f;

    private ResourceManager resourceManager;

    private void Awake()
    {
        instanceID = GetInstanceID();
        Time.timeScale = scrollBar;
    }
    private void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        if (buildingManager == null)
        {
            Debug.LogError("Could not find BuildingManager object in scene!");
        }

        UpdateUnitButtons();
        resourceManager = FindObjectOfType<ResourceManager>();
    }

    private void Update()
    {
        UpdateUnitButtons();

        timer += Time.deltaTime;
        // Check if we have reached beyond 10 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if (timer > waitTime)
        {
            visualTime = timer;
            // Remove the recorded 10 seconds.
            timer = timer - waitTime;
            Time.timeScale = scrollBar;
            resourceManager.AddResources(50, 0, 0);
        }
    }

    private void OnMouseDown()
    {
        if (!buildingManager.IsPlacingBuilding)
        {
            // Hide other upgrade panels
            foreach (Base otherBase in FindObjectsOfType<Base>())
            {
                if (otherBase.instanceID != instanceID)
                {
                    otherBase.HideUpgradePanel();
                }
            }

            upgradePanel.SetActive(!upgradePanel.activeSelf);
            UpdateUnitButtons();
        }
    }
    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }
    // produce peasant, on-click
    public void CreatePeasant()
    {
        if (peasantPrefab != null && resourceManager.HasEnoughResources(peasantGoldCost, peasantGemsCost, peasantAlchemicalGasCost))
        {
            resourceManager.ConsumeResources(peasantGoldCost, peasantGemsCost, peasantAlchemicalGasCost);//=
            float outerRadius = 10.0f;
            float innerRadius = 5.0f;
            Vector3 spawnPos = GetSpawnPosition(transform.position, innerRadius, outerRadius);
            Instantiate(peasantPrefab, spawnPos, Quaternion.identity);
        }
    }
    // spawn position
    private Vector3 GetSpawnPosition(Vector3 center, float innerRadius, float outerRadius)
    {
        Vector2 randomPointInCircle = Random.insideUnitCircle.normalized;
        float randomRadius = Random.Range(innerRadius, outerRadius);
        Vector2 randomPointInDonut = randomPointInCircle * randomRadius;

        return center + new Vector3(randomPointInDonut.x, 0.0f, randomPointInDonut.y);
    }

    private void UpdateUnitButtons()
    {
        if (peasantButton != null)
        {
            peasantButton.interactable = true; // always available
        }
        else
        {
            Debug.LogError("The peasant button is null! Please check the BaseUI object in the inspector.");
        }
    }
}
