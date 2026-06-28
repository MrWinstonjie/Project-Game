using UnityEngine;

public class Inventory : MonoBehaviour
{


    public GameObject InventoryUI;
    private bool OpenedInventory = false;
    

    void Start()
    {
        InventoryUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            OpenedInventory = !OpenedInventory; 
            

            if (OpenedInventory)
            {
                
                InventoryUI.SetActive(true);
          
            }
            else
            {
                InventoryUI.SetActive(false);
              
            }
        }
    }
}
