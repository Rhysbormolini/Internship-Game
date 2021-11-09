using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.5f;

    public RectTransform menuContainer;
    public Transform levelPanel;

    public Transform shipPanel;
    public Transform colourPanel;
    public Transform laserPanel;
    public Transform trailPanel;

    private Vector3 desiredMenuPosition;

    private void Start()
    {
        // Grab the onl CanvasGroup in the scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start with a white screen;
        fadeGroup.alpha = 1;

        // Add button on-click events to show buttons
        InitShop();

        // Add buttons on-click events to levels
        InitLevel();
    }

    private void Update()
    {
        // Fade-in
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        // Menu navigation (smooth)
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPosition, 0.1f);
    }

    private void InitShop()
    {
        // Just make sure we've assigned the references
        if (shipPanel == null || colourPanel == null || laserPanel == null || trailPanel == null)
        {
            Debug.Log("You did not assign the ship/colour/laser/trail panels in the inspector");
        }

        // For every children tranform under our ship panel, find the button and add onclick
        int i = 0;
        foreach (Transform t in colourPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnColourSelect(currentIndex));

            i++;
        }

        // Reset index;
        i = 0;
        // Do the same for the laser panel
        foreach (Transform t in laserPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnLaserSelect(currentIndex));

            i++;
        }

        // Reset index;
        i = 0;
        // Do the same for the trail panel
        foreach (Transform t in trailPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnTrailSelect(currentIndex));

            i++;
        }

        // Reset index;
        i = 0;
        // Do the same for the ship panel
        foreach (Transform t in shipPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnShipSelect(currentIndex));

            i++;
        }
    }

    private void InitLevel()
    {
        // Just make sure we've assigned the references
        if (levelPanel == null)
        {
            Debug.Log("You did not assign the level panel in the inspector");
        }

        // For every children tranform under our level panel, find the button and add onclick
        int i = 0;
        foreach (Transform t in levelPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnLevelSelect(currentIndex));

            i++;
        }
    }

    private void NavigateTo(int menuIndex)
    {
        switch (menuIndex)
        {
            // 0 && default case = Main Menu
            default:
            case 0:
                desiredMenuPosition = Vector3.zero;
                break;
            // 1 = Play Menu
            case 1:
                desiredMenuPosition = Vector3.right * 1920;
                break;
            // 2 = Shop Menu
            case 2:
                desiredMenuPosition = Vector3.left * 1920;
                break;
        }
    }

    //Buttons
    public void OnPlayClick()
    {
        NavigateTo(1);
        Debug.Log("Play button has been clicked!");
    }

    public void OnShopClick()
    {
        NavigateTo(2);
        Debug.Log("Shop button has been clicked!");
    }

    public void OnBackClick()
    {
        NavigateTo(0);
        Debug.Log("Back button has been clicked!");
    }

    private void OnColourSelect(int currentIndex)
    {
        Debug.Log("Selecting colour button : " + currentIndex);
    }

    private void OnLaserSelect(int currentIndex)
    {
        Debug.Log("Selecting laser button : " + currentIndex);
    }

    private void OnTrailSelect(int currentIndex)
    {
        Debug.Log("Selecting trail button : " + currentIndex);
    }

    private void OnShipSelect(int currentIndex)
    {
        Debug.Log("Selecting ship button : " + currentIndex);
    }    
    
    private void OnLevelSelect(int currentIndex)
    {
        Debug.Log("Selecting level : " + currentIndex);
    }

    public void OnColourBuySet()
    {
        Debug.Log("Buy/Set Colour");
    }

    public void OnLaserBuySet()
    {
        Debug.Log("Buy/Set Laser");
    }

    public void OnTrailBuySet()
    {
        Debug.Log("Buy/Set Trail");
    }

    public void OnShipBuySet()
    {
        Debug.Log("Buy/Set Ship");
    }
}
