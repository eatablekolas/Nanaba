using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActive : MonoBehaviour
{
    [SerializeField] KeyCode activateButton;
    GameObject objectToSwitch;

    void Awake()
    {
        objectToSwitch = transform.GetChild(0).gameObject;
    }

    public void Switch()
    {
        objectToSwitch.SetActive(!objectToSwitch.activeSelf);
    }

    void Update()
    {
        if (Input.GetKeyDown(activateButton))
        {
            Switch();
        }
    }
}
