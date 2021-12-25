using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { set; get; }
    public SaveState state;

    private void Awake()
    {
        //ResetSave();
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();

        // Are we using the accelerometer AND can we use it?
        if (state.usingAccelerometer && !SystemInfo.supportsAccelerometer)
        {
            // If we can't, make sure we're not trying next time
            state.usingAccelerometer = false;
            Save();
        }
    }

    // Save the whole state of this savestate script to the player pref
    public void Save()
    {
        PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state));
    }

    // Load the previous saved state from the player prefs
    public void Load()
    {
        // Do we already have a save?
        if (PlayerPrefs.HasKey ("save"))
        {
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else
        {
            state = new SaveState();
            Save();
            Debug.Log("No save file found, creating a new one");
        }

    }
    
    // Check if the ship is owned
    public bool IsShipOwned(int index)
    {
        // Check if the bit is set, if so, the colour is onwed
        return (state.shipOwned & (1 << index)) != 0;
    }  

    // Check if the colour is owned
    public bool IsColourOwned(int index)
    {
        // Check if the bit is set, if so, the colour is onwed
        return (state.colourOwned & (1 << index)) != 0;
    }
    
    // Check if the laser is owned
    public bool IsLaserOwned(int index)
    {
        // Check if the bit is set, if so, the colour is onwed
        return (state.laserOwned & (1 << index)) != 0;
    }

    // Check if the trail is owned
    public bool IsTrailOwned(int index)
    {
        // Check if the bit is set, if so, the colour is onwed
        return (state.trailOwned & (1 << index)) != 0;
    }

    // Attempt buying a Ship, return true/false
    public bool BuyShip(int index, int cost)
    {
        if (state.gold >= cost)
        {
            // Enough money, remove from the current gold stack
            state.gold -= cost;
            UnlockShip(index);

            // Save progress
            Save();

            return true;
        }
        else
        {
            // Not enough money, return false
            return false;
        }
    }

    // Attempt buying a Colour, return true/false
    public bool BuyColour(int index, int cost)
    {
        if (state.gold >= cost)
        {
            // Enough money, remove from the current gold stack
            state.gold -= cost;
            UnlockColour(index);

            // Save progress
            Save();

            return true;
        }
        else
        {
            // Not enough money, return false
            return false;
        }
    }

    // Attempt buying a Laser, return true/false
    public bool BuyLaser(int index, int cost)
    {
        if (state.gold >= cost)
        {
            // Enough money, remove from the current gold stack
            state.gold -= cost;
            UnlockLaser(index);

            // Save progress
            Save();

            return true;
        }
        else
        {
            // Not enough money, return false
            return false;
        }
    }

    // Attempt buying a Trail, return true/false
    public bool BuyTrail(int index, int cost)
    {
        if (state.gold >= cost)
        {
            // Enough money, remove from the current gold stack
            state.gold -= cost;
            UnlockTrail(index);

            // Save progress
            Save();

            return true;
        }
        else
        {
            // Not enough money, return false
            return false;
        }
    }

    // Unlock a ship in the "shipOnwed" int
    public void UnlockShip(int index)
    {
        // Toggle on the bit at index
        state.shipOwned |= 1 << index;
    }

    // Unlock a colour in the "colourOnwed" int
    public void UnlockColour(int index)
    {
        // Toggle on the bit at index
        state.colourOwned |= 1 << index;
    }     
    
    // Unlock a laser in the "laserOnwed" int
    public void UnlockLaser(int index)
    {
        // Toggle on the bit at index
        state.laserOwned |= 1 << index;
    }

    // Unlock a trail in the "trailOnwed" int
    public void UnlockTrail(int index)
    {
        // Toggle on the bit at index
        state.trailOwned |= 1 << index;
    }

    // Complete Level
    public void CompleteLevel(int index)
    {
        // if this is the current active level
        if (state.completedLevel == index)
        {
            state.completedLevel++;
            Save();
        }
    }    

    // Reset the whole save file
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}
