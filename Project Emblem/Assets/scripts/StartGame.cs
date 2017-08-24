using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    Button thisButton;

    private void Start()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(Click);
    }

    void Click()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        print("Start Clicked");
    }
}