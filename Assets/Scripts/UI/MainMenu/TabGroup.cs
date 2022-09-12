using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour {

    private TabButton[] tabButtons;
    private Tab[] tabs;

    void OnValidate() {
        tabButtons = GetComponentsInChildren<TabButton>();
        tabs = GetComponentsInChildren<Tab>();
    }

    void Start() {
        OnValidate();

        for (int i = 0; i < tabs.Length; i++)
            tabs[i].InitTab((SettingTabType)i);

        DisableAllTabs();

        tabs[0].Enable();
    }

    public void OnTabButtonSelected(int index) {
        DisableAllTabs();

        tabs[index].Enable();
    }

    private void DisableAllTabs() {
        foreach (Tab tab in tabs)
            tab.Disable();
    }
}
