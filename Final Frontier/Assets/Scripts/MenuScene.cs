using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField] TextMeshProUGUI shipBuySetText;
    [SerializeField] TextMeshProUGUI colourBuySetText;
    [SerializeField] TextMeshProUGUI laserBuySetText;
    [SerializeField] TextMeshProUGUI trailBuySetText;

    private int[] shipCost = new int[] { 0, 2500, 2500, 2500, 5000, 5000, 5000, 10000 };
    private int[] colourCost = new int[] { 0, 1750, 1750, 1750, 1750, 1750, 1750, 1750 };
    private int[] laserCost = new int[] { 0, 1000, 1000, 1000, 1000, 1000, 1000, 1000 };
    private int[] trailCost = new int[] { 0, 1200, 1200, 1200, 1200, 1200, 1200, 1200 };
    private int selectedShipIndex;
    private int selectedColourIndex;
    private int selectedLaserIndex;
    private int selectedTrailIndex;

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

    private void SetShip(int index)
    {
        // Change the selected ship

        // Change the buy/set button text
        shipBuySetText.text = "Current";
    }   
    
    private void SetColour(int index)
    {
        // Change the colour of all ships

        // Change the buy/set button text
        colourBuySetText.text = "Current";
    }       
    
    private void SetLaser(int index)
    {
        // Change the colour on all lasers

        // Change the buy/set button text
        laserBuySetText.text = "Current";

    }    
    
    private void SetTrail(int index)
    {
        // Change the colour on all trails

        // Change the buy/set button text
        trailBuySetText.text = "Current";
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
    
    private void OnLevelSelect(int currentIndex)
    {
        Debug.Log("Selecting level : " + currentIndex);
    }

    private void OnColourSelect(int currentIndex)
    {
        Debug.Log("Selecting colour button : " + currentIndex);

        // Set the selected Colour
        selectedColourIndex = currentIndex;

        // Change the content of the buy/set button. depending on he state of the colour
        if (SaveManager.Instance.IsColourOwned(currentIndex))
        {
            // Colour is owned
            colourBuySetText.text = "Select";
        }
        else
        {
            // Colour isn't owned
            colourBuySetText.text = "Buy: " + colourCost[currentIndex].ToString();
        }
    }

    private void OnLaserSelect(int currentIndex)
    {
        Debug.Log("Selecting laser button : " + currentIndex);

        // Set the selected Laser
        selectedLaserIndex = currentIndex;

        // Change the content of the buy/set button. depending on he state of the colour
        if (SaveManager.Instance.IsLaserOwned(currentIndex))
        {
            // Laser is owned
            laserBuySetText.text = "Select";
        }
        else
        {
            // Laser isn't owned
            laserBuySetText.text = "Buy: " + laserCost[currentIndex].ToString();
        }
    }

    private void OnTrailSelect(int currentIndex)
    {
        Debug.Log("Selecting trail button : " + currentIndex);

        // Set the selected Trail
        selectedTrailIndex = currentIndex;

        // Change the content of the buy/set button. depending on he state of the colour
        if (SaveManager.Instance.IsTrailOwned(currentIndex))
        {
            // Trail is owned
            trailBuySetText.text = "Select";
        }
        else
        {
            // Trail isn't owned
            trailBuySetText.text = "Buy: " + trailCost[currentIndex].ToString();
        }
    }

    private void OnShipSelect(int currentIndex)
    {
        Debug.Log("Selecting ship button : " + currentIndex);

        // Set the selected Ship
        selectedShipIndex = currentIndex;

        // Change the content of the buy/set button. depending on he state of the colour
        if (SaveManager.Instance.IsShipOwned(currentIndex))
        {
            // Ship is owned
            shipBuySetText.text = "Select";
        }
        else
        {
            // Ship isn't owned
            shipBuySetText.text = "Buy: " + shipCost[currentIndex].ToString();
        }
    }    
    
    public void OnColourBuySet()
    {
        Debug.Log("Buy/Set Colour");

        // Is the selected colour owned
        if (SaveManager.Instance.IsColourOwned(selectedColourIndex))
        {
            // Set the Colour!
            SetColour(selectedColourIndex);
        }
        else
        {
            // Attempt to Buy the Colour
            if (SaveManager.Instance.BuyColour(selectedColourIndex, colourCost[selectedColourIndex]))
            {
                // Success!
                SetColour(selectedColourIndex);
            }
            else
            {
                // Do not have enough gold!
                // Play sound feedback
                Debug.Log("Not enough gold");
            }
        }
    }

    public void OnLaserBuySet()
    {
        Debug.Log("Buy/Set Laser");

        // Is the selected laser owned
        if (SaveManager.Instance.IsLaserOwned(selectedLaserIndex))
        {
            // Set the Laser!
            SetLaser(selectedLaserIndex);
        }
        else
        {
            // Attempt to Buy the Laser
            if (SaveManager.Instance.BuyLaser(selectedLaserIndex, laserCost[selectedLaserIndex]))
            {
                // Success!
                SetLaser(selectedLaserIndex);
            }
            else
            {
                // Do not have enough gold!
                // Play sound feedback
                Debug.Log("Not enough gold");
            }
        }
    }

    public void OnTrailBuySet()
    {
        Debug.Log("Buy/Set Trail");

        // Is the selected trail owned
        if (SaveManager.Instance.IsTrailOwned(selectedTrailIndex))
        {
            // Set the Trail!
            SetTrail(selectedTrailIndex);
        }
        else
        {
            // Attempt to Buy the Trail
            if (SaveManager.Instance.BuyTrail(selectedTrailIndex, trailCost[selectedTrailIndex]))
            {
                // Success!
                SetTrail(selectedTrailIndex);
            }
            else
            {
                // Do not have enough gold!
                // Play sound feedback
                Debug.Log("Not enough gold");
            }
        }
    }

    public void OnShipBuySet()
    {
        Debug.Log("Buy/Set Ship");

        // Is the selected Ship owned
        if (SaveManager.Instance.IsShipOwned(selectedShipIndex))
        {
            // Set the Ship!
            SetShip(selectedShipIndex);
        }
        else
        {
            // Attempt to Buy the Ship
            if (SaveManager.Instance.BuyShip(selectedShipIndex, shipCost[selectedShipIndex]))
            {
                // Success!
                SetShip(selectedShipIndex);
            }
            else
            {
                // Do not have enough gold!
                // Play sound feedback
                Debug.Log("Not enough gold");
            }
        }
    }
}
