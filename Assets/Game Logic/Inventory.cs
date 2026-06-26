using UnityEngine;

public class Inventory : MonoBehaviour
{

    public GameObject UIElement;
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
                // InventoryUI.transform.position = CenterScreen.position;
                
                InventoryUI.SetActive(true);
                UIElement.SetActive(false);
            }
            else
            {
                InventoryUI.SetActive(false);
                UIElement.SetActive(true);
            }
        }
    }
}
