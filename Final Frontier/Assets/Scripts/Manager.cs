using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { set; get; }

    public Material playerMaterial1, playerMaterial2, playerMaterial3, playerMaterial4, playerMaterial5, playerMaterial6, playerMaterial7, playerMaterial8;
    public GameObject[] playerShips = new GameObject[8];
    public Color[] playerColours = new Color[8];
    public GameObject[] playerLasers = new GameObject[8];
    public GameObject[] playerTrails = new GameObject[8];
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public int currentLevel = 0; // Used when changing from menu to game scene
    public int menuFocus = 0; // Used when entering the menu scene, to know which menu to look at

}
