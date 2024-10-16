using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName; //Each item must have an unique name
    public Texture itemPreview;
    void Start()
    {
        //Change item tag to Respawn to detect when we look at it
        gameObject.tag = "Respawn";
    }

    public void PickItem()
    {
        Destroy(gameObject);
    }
}
