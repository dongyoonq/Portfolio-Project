using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotDrag : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    float clickTime = 0;

    public static GameObject draggingItem = null;
    Transform target;
    Rect baseRect;

    void Start()
    {
        baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
    }

    void OnMouseDoubleClick()
    {
        if (transform.GetComponent<Slot>().data == null)
            return;

        Equipment equip = Instantiate(transform.GetComponent<Slot>().data.prefab) as Equipment;

        equip.Data = transform.GetComponent<Slot>().data;
        Player.Instance.OnEquip(equip, transform.GetComponent<Slot>().slotIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.GetChild(0).IsValid())
        {
            // �巡�׽ÿ� ���纻 ����
            target = Instantiate(transform.GetChild(0));
            target.SetParent(GameObject.Find("InventoryUI").transform);
            transform.GetChild(0).gameObject.SetActive(false);

            target.GetComponent<Image>().raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (target.IsValid())
        {
            target.position = eventData.position;
            draggingItem = transform.GetChild(0).gameObject;
        }

        Debug.Log(target.localPosition);
        Debug.Log(draggingItem.transform.localPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingItem != null && GetComponent<Slot>().data != null)
        {
            if (target.localPosition.x < baseRect.xMin
            || target.localPosition.x > baseRect.xMax
            || target.localPosition.y < baseRect.yMin
            || target.localPosition.y > baseRect.yMax)
            {
                Instantiate(GetComponent<Slot>().data.prefab, Player.Instance.foot.position, Quaternion.identity);
                Player.Instance.RemoveItemFromInventory(GetComponent<Slot>().data, GetComponent<Slot>().slotIndex);
                Destroy(target.gameObject);
                return;
            }
        }

        if (target.IsValid())
        {
            Destroy(target.gameObject);
            target.localPosition = Vector3.zero;

            target.GetComponent<Image>().raycastTarget = true;
        }

        if (SlotDrop.swapItemIsActiveObj)
            transform.GetChild(0).gameObject.SetActive(true);
        else
            transform.GetChild(0).gameObject.SetActive(false);

        if (draggingItem == null && transform.GetComponent<Slot>().data == null)
            transform.GetChild(0).gameObject.SetActive(false);

        SlotDrop.swapItemIsActiveObj = true;
        draggingItem = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((Time.time - clickTime) < 0.3f)
        {
            OnMouseDoubleClick();
            clickTime = -1;
        }
        else
            clickTime = Time.time;
    }
}