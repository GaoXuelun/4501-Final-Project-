using UnityEngine;
//resource
public class ResourceManager : MonoBehaviour
{
    //initial resources
    public int gold = 1000;
    public int gems = 500;
    public int alchemicalGas = 200;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddResources(50, 25, 10);
        }
    }
    //consuume resources function
    public bool ConsumeResources(int goldCost, int gemsCost, int alchemicalGasCost)
    {   //check if has enough resources
        if (HasEnoughResources(goldCost, gemsCost, alchemicalGasCost))
        {
            gold -= goldCost;
            gems -= gemsCost;
            alchemicalGas -= alchemicalGasCost;
            return true;
        }
        else
        {
            return false;
        }
    }
    //add resources function
    public void AddResources(int goldToAdd, int gemsToAdd, int alchemicalGasToAdd)
    {
        gold += goldToAdd;
        gems += gemsToAdd;
        alchemicalGas += alchemicalGasToAdd;
    }
    //check if it has enough resources
    public bool HasEnoughResources(int goldCost, int gemsCost, int alchemicalGasCost)
    {
        return gold >= goldCost && gems >= gemsCost && alchemicalGas >= alchemicalGasCost;  //return true if all resources are enough
    }

    //get current resources as string
    public string GetResourceString()
    {
        return $"Gold: {gold}\nGems: {gems}\nAlchemical Gas: {alchemicalGas}";
    }
}
