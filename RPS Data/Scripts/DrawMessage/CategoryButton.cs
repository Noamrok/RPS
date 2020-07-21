using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CategoryButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Set colors
        ColorBlock colors = GetComponent<Button>().colors;
        colors.normalColor = colors.selectedColor = Color.gray;
        colors.pressedColor = colors.highlightedColor = Color.black;
        GetComponent<Button>().colors = colors;

        // Each button returns its string value (characters R, P or S)
        GetComponent<Button>().onClick.AddListener(() => Clicked(GetComponentInChildren<Text>().text));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void Clicked(string type)
    {
        // Disable selected button
        EventSystem.current.SetSelectedGameObject(null);

        // Disable DrowMessage window
        GetComponentInParent<Canvas>().enabled = false;

        // Saves the selected character
        GetComponentInParent<DrawMessage>().setType(type);
    }
}
