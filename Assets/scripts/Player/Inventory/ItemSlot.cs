using Constant;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private ItemModel.ItemData data;

    public void Init(ItemModel.ItemData data) => this.data = data; //load/update SO data
}
