using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollView : MonoBehaviour
{
    public ScrollRect scrollrect;
    public RectTransform viewPortTransform;
    public RectTransform contentPanelTransform;
    public VerticalLayoutGroup VLG;

    public RectTransform[] ItemList;

    public int ItemToAdd = 5;
    Vector2 OldVelocity;
    bool isUpdated;

    // Start is called before the first frame update
    void Start()
    {
        isUpdated = false;
        OldVelocity = Vector2.zero;

        // Calculate the number of items needed to fill the viewport
        // int ItemToAdd = Mathf.CeilToInt(viewPortTransform.rect.height / (ItemList[0].rect.height + VLG.spacing));

        // Instantiate items to fill the viewport
        for (int i = 0; i < ItemToAdd; i++)
        {
            RectTransform RT = Instantiate(ItemList[i % ItemList.Length], contentPanelTransform);
            RT.SetAsLastSibling();
        }
        for (int i = 0; i < ItemToAdd; i++)
        {
            int num = ItemList.Length - i - 1;
            while (num < 0)
            {
                num += ItemList.Length;
            }
            RectTransform RT = Instantiate(ItemList[num], contentPanelTransform);
            RT.SetAsFirstSibling();
        }

        // Set initial position of the content panel
        contentPanelTransform.localPosition = new Vector3(contentPanelTransform.localPosition.x,
            -(ItemList[0].rect.height + VLG.spacing) * ItemToAdd,
            0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUpdated)
        {
            isUpdated = false;
            scrollrect.velocity = OldVelocity;
        }

        if (contentPanelTransform.localPosition.y > ItemList.Length * (ItemList[0].rect.height + VLG.spacing))
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollrect.velocity;
            contentPanelTransform.localPosition -= new Vector3(0, ItemList.Length * (ItemList[0].rect.height + VLG.spacing), 0);
            isUpdated = true;
        }

        if (contentPanelTransform.localPosition.y < 0)
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollrect.velocity;
            contentPanelTransform.localPosition += new Vector3(0, ItemList.Length * (ItemList[0].rect.height + VLG.spacing), 0);
            isUpdated = true;
        }
    }

    // Public method to set item size and position
    public void SetItemSizeAndPosition(float width, float height, float spacing)
    {
        foreach (var item in ItemList)
        {
            item.sizeDelta = new Vector2(width, height);
        }
        VLG.spacing = spacing;

        // Adjust content panel size based on new item dimensions
        contentPanelTransform.sizeDelta = new Vector2(contentPanelTransform.sizeDelta.x,
            (height + spacing) * ItemList.Length);

        // Reset scroll position to avoid potential visual glitches
        contentPanelTransform.localPosition = new Vector3(contentPanelTransform.localPosition.x, 0, contentPanelTransform.localPosition.z);
    }
}