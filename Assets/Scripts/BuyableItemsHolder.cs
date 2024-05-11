using UnityEngine;

public class BuyableItemsHolder : MonoBehaviour
{
    public static BuyableItemsHolder Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public Item[] Items;

    public Item GetRandomItem()
    {
        return Items[UnityEngine.Random.Range(0, Items.Length)];
    }
}
