using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Script : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject shopItemTemplate;
    [SerializeField] private CollectibleDrops[] collectDrops;

    private void Start()
    {
        PopulateShop();       
    }

    //populates the shop according to the amount of objects in the buff array
    public void PopulateShop()
    {
        List<int> items = new List<int>();

        //loops through the buff array creating buttons
        for (int i = 0; items.Count < 4; i++)
        {
            int refresh = Random.Range(0, collectDrops.Length);
            if (items.Contains(refresh))
            {
                refresh = Random.Range(0, collectDrops.Length);
            }
            else
            {
                items.Add(refresh);

                CollectibleDrops shopItem = collectDrops[refresh];
                GameObject itemObject = Instantiate(shopItemTemplate, container);
                itemObject.SetActive(true);

                //creates an onClick event
                itemObject.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(shopItem));
                //accesses prefabs to change it based of items in shop
                //changes the name on the button
                itemObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = shopItem.itemName;
                //changes the price on the button
                itemObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = shopItem.cost.ToString();
            }
        }
    }
    //when player clicks on the button it will check to see
    //if the player has enough gems to buy the item
    private void OnButtonClick(CollectibleDrops shopItem)
    {
        if (gameManager.instance.playerScript.TrySpendGemAmount(shopItem.cost) == true)
        {
            shopItem.Apply();
            gameManager.instance.updateGemCount(-shopItem.cost);
        }
        else
        {
            return;
        }
    }
}
