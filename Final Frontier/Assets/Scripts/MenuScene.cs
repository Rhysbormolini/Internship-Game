using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    public Button tiltControlButton;
    public Color tiltControlEnabled, tiltControlDisabled;

    [SerializeField] TextMeshProUGUI shipBuySetText;
    [SerializeField] TextMeshProUGUI colourBuySetText;
    [SerializeField] TextMeshProUGUI laserBuySetText;
    [SerializeField] TextMeshProUGUI trailBuySetText;
    [SerializeField] TextMeshProUGUI goldText;

    private MenuCamera menuCam;

    private int[] shipCost = new int[] { 0, 2500, 2500, 2500, 5000, 5000, 5000, 10000 };
    private int[] colourCost = new int[] { 0, 1750, 1750, 1750, 1750, 1750, 1750, 1750 };
    private int[] laserCost = new int[] { 0, 1000, 1000, 1000, 1000, 1000, 1000, 1000 };
    private int[] trailCost = new int[] { 0, 1200, 1200, 1200, 1200, 1200, 1200, 1200 };
    private int selectedShipIndex;
    private int selectedColourIndex;
    private int selectedLaserIndex;
    private int selectedTrailIndex;    
    private int activeShipIndex;
    private int activeColourIndex;
    private int activeLaserIndex;
    private int activeTrailIndex;

    private Vector3 desiredMenuPosition;

    private GameObject currentTrail;
    public GameObject trailLocation;

    public AnimationCurve enteringLevelZoomCurve;
    private bool isEnteringLevel = false;
    private float zoomDuration = 3.9f;
    private float zoomTransition;

    private void Start()
    {
        // $$ TEMPORARY
        //SaveManager.Instance.state.gold = 999999;

        // Check if we have an accelerometer
        if (SystemInfo.supportsAccelerometer)
        {
            // Is it currently enabled?
            tiltControlButton.GetComponent<Image>().color = (SaveManager.Instance.state.usingAccelerometer) ? tiltControlEnabled : tiltControlDisabled;
        }
        else
        {
            tiltControlButton.gameObject.SetActive(false);
        }

        // Find the only MenuCamera and asign it
        menuCam = FindObjectOfType<MenuCamera>();

        // Position our camera on the focused menu
        SetCameraTo(Manager.Instance.menuFocus);

        // Tell our gold text how much should be displaying
        UpdateGoldText();
        
        // Grab the only CanvasGroup in the scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start with a white screen;
        fadeGroup.alpha = 1;

        // Add button on-click events to show buttons
        InitShop();

        // Add buttons on-click events to levels
        InitLevel();

        // Set player's preferences ( ship, colour, laser, trail )
        OnShipSelect(SaveManager.Instance.state.activeShip);
        SetShip(SaveManager.Instance.state.activeShip);

        OnColourSelect(SaveManager.Instance.state.activeColour);
        SetColour(SaveManager.Instance.state.activeColour);

        OnLaserSelect(SaveManager.Instance.state.activeLaser);
        SetLaser(SaveManager.Instance.state.activeLaser);

        OnTrailSelect(SaveManager.Instance.state.activeTrail);
        SetTrail(SaveManager.Instance.state.activeTrail);

        // Make the buttons bigger for the selected items
        shipPanel.GetChild(SaveManager.Instance.state.activeShip).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        colourPanel.GetChild(SaveManager.Instance.state.activeColour).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        laserPanel.GetChild(SaveManager.Instance.state.activeLaser).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
    }

    private void Update()
    {
        // Fade-in
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        // Menu navigation (smooth)
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPosition, 0.1f);

        // Entering level zoom
        if (isEnteringLevel)
        {
            // Add to the zoomTransition float
            zoomTransition += (1 / zoomDuration) * Time.deltaTime;

            // Change the scale, following the animation curve
            menuContainer.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 5, enteringLevelZoomCurve.Evaluate(zoomTransition));

            // Change the desired position of the canvas, so it can follow the scale up
            // This zooms in the centre
            Vector3 newDesiredPosition = desiredMenuPosition * 5;
            // This adds to the specific position of the level on the canvas
            RectTransform rt = levelPanel.GetChild(Manager.Instance.currentLevel).GetComponent<RectTransform>();
            newDesiredPosition -= rt.anchoredPosition3D * 5;

            // This line will override the previous position update
            menuContainer.anchoredPosition3D = Vector3.Lerp(desiredMenuPosition, newDesiredPosition, enteringLevelZoomCurve.Evaluate(zoomTransition));

            // Fade to fadeout, this will override the first line of the update.
            fadeGroup.alpha = zoomTransition;

            // Are we done with the animation
            if (zoomTransition >= 1)
            {
                // Enter the level
                SceneManager.LoadScene("Game");
            }
        }
    }

    private void InitShop()
    {
        // Just make sure we've assigned the references
        if (shipPanel == null || colourPanel == null || laserPanel == null || trailPanel == null)
        {
            Debug.Log("You did not assign the ship/colour/laser/trail panels in the inspector");
        }

        // For every children tranform under our colour panel, find the button and add onclick
        int i = 0;
        foreach (Transform t in colourPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnColourSelect(currentIndex));

            // Set the colour of the image, based on if owned or not
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsColourOwned(i) ? Color.white : new Color(0.6f, 0.6f, 0.6f);

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

            // Set the colour of the image, based on if owned or not
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsLaserOwned(i) ? Color.white : new Color(0.6f, 0.6f, 0.6f);

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

            // Set the colour of the image, based on if owned or not
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsTrailOwned(i) ? Color.white : new Color(0.6f, 0.6f, 0.6f);

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

            // Set the colour of the image, based on if owned or not
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsShipOwned(i) ? Color.white : new Color(0.6f, 0.6f, 0.6f);

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

            Image img = t.GetComponent<Image>();

            // Is it unlocked?
            if (i <= SaveManager.Instance.state.completedLevel)
            {
                // It is unlocked!
                if (i == SaveManager.Instance.state.completedLevel)
                {
                    // It is not completed!
                    img.color = Color.white;
                }
                else
                {
                    // Level is already completed!
                    img.color = Color.green;
                }
            }
            else
            {
                // Level isn't unlocked, disable the button
                b.interactable = false;

                // Set to a dark colour
                img.color = Color.grey;
            }

            i++;
        }
    }

    private void SetCameraTo(int menuIndex)
    {
        NavigateTo(menuIndex);
        menuContainer.anchoredPosition3D = desiredMenuPosition;
    }

    private void NavigateTo(int menuIndex)
    {
        switch (menuIndex)
        {
            // 0 && default case = Main Menu
            default:
            case 0:
                desiredMenuPosition = Vector3.zero;
                menuCam.BackToMainMenu();
                break;
            // 1 = Play Menu
            case 1:
                desiredMenuPosition = Vector3.right * 1920;
                menuCam.MoveToLevel();
                break;
            // 2 = Shop Menu
            case 2:
                desiredMenuPosition = Vector3.left * 1920;
                menuCam.MoveToShop();
                break;
        }
    }

    private void SetShip(int index)
    {
        // Set the active index
        activeShipIndex = index;
        SaveManager.Instance.state.activeShip = index;

        // Change the selected ship

        // Change the buy/set button text
        shipBuySetText.text = "Current Ship";

        // Remember preferences
        SaveManager.Instance.Save();
    }   
    
    private void SetColour(int index)
    {
        // Set the active index
        activeColourIndex = index;
        SaveManager.Instance.state.activeColour = index;

        // Change the colour of all ships
        Manager.Instance.playerMaterial1.color = Manager.Instance.playerColours[index];
        //Manager.Instance.playerMaterial2.color = Manager.Instance.playerColours[index];
        //Manager.Instance.playerMaterial3.color = Manager.Instance.playerColours[index];
        //Manager.Instance.playerMaterial4.color = Manager.Instance.playerColours[index];
        //Manager.Instance.playerMaterial5.color = Manager.Instance.playerColours[index];
        //Manager.Instance.playerMaterial6.color = Manager.Instance.playerColours[index];
        //Manager.Instance.playerMaterial7.color = Manager.Instance.playerColours[index];
        //Manager.Instance.playerMaterial8.color = Manager.Instance.playerColours[index];

        // Change the buy/set button text
        colourBuySetText.text = "Current Colour";

        // Remember preferences
        SaveManager.Instance.Save();
    }       
    
    private void SetLaser(int index)
    {
        // Set the active index
        activeLaserIndex = index;
        SaveManager.Instance.state.activeLaser = index;

        // Change the colour on all lasers

        // Change the buy/set button text
        laserBuySetText.text = "Current Laser";

        // Remember preferences
        SaveManager.Instance.Save();
    }    
    
    private void SetTrail(int index)
    {
        // Set the active index
        activeTrailIndex = index;
        SaveManager.Instance.state.activeTrail = index;

        // Change the trail on the player model
        if (currentTrail != null)
        {
            Destroy(currentTrail);
        }

        // Create new trail
        currentTrail = Instantiate(Manager.Instance.playerTrails[index]) as GameObject;

        // Set it as a child of the player
        currentTrail.transform.parent = trailLocation.transform;

        // Fix the wierd scaling issues / rotation issues
        currentTrail.transform.localPosition = new Vector3(0,-0.0433f,0.0044f);
        currentTrail.transform.localRotation = Quaternion.Euler(4.26f, 0, 0);
        //currentTrail.transform.localScale = Vector3.one * 0.01f;

        // Change the buy/set button text
        trailBuySetText.text = "Current Trail";

        // Remember preferences
        SaveManager.Instance.Save();
    }

    private void UpdateGoldText()
    {
        goldText.text = SaveManager.Instance.state.gold.ToString();
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
        Manager.Instance.currentLevel = currentIndex;
        isEnteringLevel = true;
        Debug.Log("Selecting level : " + currentIndex);
    }

    private void OnColourSelect(int currentIndex)
    {
        Debug.Log("Selecting colour button : " + currentIndex);

        // if the button clicked is already selected, exit
        if (selectedColourIndex == currentIndex)
        {
            return;
        }

        // Make the icon slightly bigger
        colourPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        // Put the previouys one on normal scale
        colourPanel.GetChild(selectedColourIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        // Set the selected Colour
        selectedColourIndex = currentIndex;

        // Change the content of the buy/set button. depending on he state of the colour
        if (SaveManager.Instance.IsColourOwned(currentIndex))
        {
            // Colour is owned
            // is it already our current colour?
            if (activeColourIndex == currentIndex)
            {
                colourBuySetText.text = "Current Colour";
            }
            else
            {
                colourBuySetText.text = "Select Colour";
            }
        }
        else
        {
            // Colour isn't owned
            colourBuySetText.text = "Buy Colour: " + colourCost[currentIndex].ToString();
        }
    }

    private void OnLaserSelect(int currentIndex)
    {
        Debug.Log("Selecting laser button : " + currentIndex);

        // if the button clicked is already selected, exit
        if (selectedLaserIndex == currentIndex)
        {
            return;
        }

        // Make the icon slightly bigger
        laserPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        // Put the previouys one on normal scale
        laserPanel.GetChild(selectedLaserIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        // Set the selected Laser
        selectedLaserIndex = currentIndex;

        // Change the content of the buy/set button. depending on he state of the colour
        if (SaveManager.Instance.IsLaserOwned(currentIndex))
        {
            // Laser is owned
            // is it already our current laser?
            if (activeLaserIndex == currentIndex)
            {
                laserBuySetText.text = "Current Laser";
            }
            else
            {
                laserBuySetText.text = "Select Laser";
            }
        }
        else
        {
            // Laser isn't owned
            laserBuySetText.text = "Buy laser: " + laserCost[currentIndex].ToString();
        }
    }

    private void OnTrailSelect(int currentIndex)
    {
        Debug.Log("Selecting trail button : " + currentIndex);

        // if the button clicked is already selected, exit
        if (selectedTrailIndex == currentIndex)
        {
            return;
        }

        // Make the icon slightly bigger
        trailPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        // Put the previouys one on normal scale
        trailPanel.GetChild(selectedTrailIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        // Set the selected Trail
        selectedTrailIndex = currentIndex;

        // Change the content of the buy/set button. depending on he state of the colour
        if (SaveManager.Instance.IsTrailOwned(currentIndex))
        {
            // Trail is owned
            // is it already our current trail?
            if (activeTrailIndex == currentIndex)
            {
                trailBuySetText.text = "Current Trail";
            }
            else
            {
                trailBuySetText.text = "Select Trail";
            }
        }
        else
        {
            // Trail isn't owned
            trailBuySetText.text = "Buy Trail: " + trailCost[currentIndex].ToString();
        }
    }

    private void OnShipSelect(int currentIndex)
    {
        Debug.Log("Selecting ship button : " + currentIndex);

        // if the button clicked is already selected, exit
        if (selectedShipIndex == currentIndex)
        {
            return;
        }

        // Make the icon slightly bigger
        shipPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        // Put the previouys one on normal scale
        shipPanel.GetChild(selectedShipIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        // Set the selected Ship
        selectedShipIndex = currentIndex;

        // Change the content of the buy/set button. depending on he state of the colour
        if (SaveManager.Instance.IsShipOwned(currentIndex))
        {
            // Ship is owned
            // is it already our current ship?
            if (activeShipIndex == currentIndex)
            {
                shipBuySetText.text = "Current Ship";
            }
            else
            {
                shipBuySetText.text = "Select Ship";
            }
        }
        else
        {
            // Ship isn't owned
            shipBuySetText.text = "Buy Ship: " + shipCost[currentIndex].ToString();
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

                // Change colour of the button
                colourPanel.GetChild(selectedColourIndex).GetComponent<Image>().color = Color.white;

                // Update gold text
                UpdateGoldText();
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

                // Change colour of the button
                laserPanel.GetChild(selectedLaserIndex).GetComponent<Image>().color = Color.white;

                // Update gold text
                UpdateGoldText();
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

                // Change colour of the button
                trailPanel.GetChild(selectedTrailIndex).GetComponent<Image>().color = Color.white;

                // Update gold text
                UpdateGoldText();
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

                // Change colour of the button
                shipPanel.GetChild(selectedShipIndex).GetComponent<Image>().color = Color.white;

                // Update gold text
                UpdateGoldText();
            }
            else
            {
                // Do not have enough gold!
                // Play sound feedback
                Debug.Log("Not enough gold");
            }
        }
    }

    public void OnTiltControl()
    {
        // Toggle the accelerometer bool
        SaveManager.Instance.state.usingAccelerometer = !SaveManager.Instance.state.usingAccelerometer;

        // make sure we save the player's preferences
        SaveManager.Instance.Save();

        // Change the display image of the tilt control button
        tiltControlButton.GetComponent<Image>().color = (SaveManager.Instance.state.usingAccelerometer) ? tiltControlEnabled : tiltControlDisabled;
    }
}
