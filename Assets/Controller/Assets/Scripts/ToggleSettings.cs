using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSettings : MonoBehaviour
{
    public GameObject SettingsPanel;
    public bool state = false;

    public void  ToggleState() {
        state = !state;
        SettingsPanel.SetActive(state);
    }

}
