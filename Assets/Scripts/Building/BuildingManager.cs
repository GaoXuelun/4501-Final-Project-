using UnityEngine;
using UnityEngine.EventSystems;
//building function
public class BuildingManager : MonoBehaviour
{
    public GameObject basePrefab;
    public GameObject barrackPrefab;
    public GameObject baseGhostPrefab;
    public GameObject barrackGhostPrefab;
    public LayerMask groundLayer;
    public LayerMask buildingLayer;
    private GameObject currentBuildingPrefab;
    private GameObject currentBuildingGhost;
    private Camera mainCamera;
    public bool IsPlacingBuilding { get; private set; }
    
    //cost of building
    public int baseGoldCost = 100;
    public int baseGemsCost = 50;
    public int baseAlchemicalGasCost = 20;

    public int barrackGoldCost = 200;
    public int barrackGemsCost = 100;
    public int barrackAlchemicalGasCost = 40;

    private ResourceManager resourceManager;
    

    private void Start()
    {
        mainCamera = Camera.main;
        resourceManager = FindObjectOfType<ResourceManager>();

    }

    private void Update()
    {
        if (currentBuildingGhost != null)
        {
            MoveBuildingGhostToMousePosition(); //ghost of building is moving with mouse

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (IsBuildingPositionValid(currentBuildingGhost.transform.position))
                {
                    PlaceBuilding();
                }
            }
        }
    }

    //on-click in inspector
    public void OnBaseButtonClicked()
    {
        PrepareBuildingGhost(basePrefab, baseGhostPrefab, baseGoldCost, baseGemsCost, baseAlchemicalGasCost);

    }

    public void OnBarrackButtonClicked()
    {
        PrepareBuildingGhost(barrackPrefab, barrackGhostPrefab, barrackGoldCost, barrackGemsCost, barrackAlchemicalGasCost);

    }
    //prepare buildijg ghost
    private void PrepareBuildingGhost(GameObject buildingPrefab, GameObject ghostPrefab, int costGold, int costGems, int costAlchemicalGas)
    {
        //check if there is enough resources
        if (resourceManager.HasEnoughResources(costGold, costGems, costAlchemicalGas))
        {
            if (currentBuildingGhost != null)
            {
                Destroy(currentBuildingGhost);
            }

            currentBuildingPrefab = buildingPrefab;
            currentBuildingGhost = Instantiate(ghostPrefab);
            IsPlacingBuilding = true;
        }
    }

    //move building ghost with mouse
    private void MoveBuildingGhostToMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            currentBuildingGhost.transform.position = hit.point;
        }
    }
    //check if the building can be placed
    private bool IsBuildingPositionValid(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 3f, buildingLayer); //use the buildingLayer
        if (colliders.Length > 0)
        {
            return false;
        }
        //use raycast method
        RaycastHit hitInfo;
        if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out hitInfo, 100f, groundLayer))
        {
            float distance = Vector3.Distance(hitInfo.point, position);
            if (distance <= 1f)
            {
                return true;
            }
        }

        return false;
    }

    //place building function
    private void PlaceBuilding()
    {
        bool canAfford = false;
        //consume resources
        if (currentBuildingPrefab == basePrefab)
        {
            canAfford = resourceManager.ConsumeResources(baseGoldCost, baseGemsCost, baseAlchemicalGasCost);
        }
        else if (currentBuildingPrefab == barrackPrefab)
        {
            canAfford = resourceManager.ConsumeResources(barrackGoldCost, barrackGemsCost, barrackAlchemicalGasCost);
        }

        if (canAfford)
        {
            Instantiate(currentBuildingPrefab, currentBuildingGhost.transform.position, currentBuildingGhost.transform.rotation);
            Destroy(currentBuildingGhost);
            currentBuildingGhost = null;
            IsPlacingBuilding = false;
        }
    }
    
}


//test
public class BuildingGhost : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = new Color(0.5f, 1f, 0.5f, 0.5f); 
        }
    }
}
