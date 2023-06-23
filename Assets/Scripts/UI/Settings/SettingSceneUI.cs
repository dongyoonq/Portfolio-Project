using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSceneUI : SceneUI
{
    protected override void Awake()
    {
        base.Awake();

        buttons["InventoryButton"].onClick.AddListener(() => { Player.Instance.inventoryUI.OpenInventory();  });
        buttons["SettingButton"].onClick.AddListener(() => { GameManager.Ui.ShowPopUpUI<SettingPopupUI>("UI/Setting"); });
    }
}
