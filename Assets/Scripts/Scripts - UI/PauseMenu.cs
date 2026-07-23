using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

//Note: This script includes both the Pause Menu and submenus.
//Submenus are separate objects so their visibility can be toggled on/off.

public class PauseMenu : MonoBehaviour
{

    //Define variables

    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject rebindMenuUI;

    //Get audio mixers for volume sliders
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;

    //SerializeField for TMP resolution dropdown
    [SerializeField] public TMP_Dropdown resolutionDropdown;

    //Resolution containers and variables
    public Resolution[] allResolutions;
    public List<Resolution> filteredResolutions;

    public RefreshRate currentRefreshRate;
    public int currentResolutionIndex = 0;

    //Boolean for currently initializing stuff in Start()
    public bool isInitializing = true;

    //Resume and Pause functions

    // R E S U M E

    public void Resume()
    {
        //Deactivate pause menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        //Deactivate options menu
        if (optionsMenuUI != null)
        {
            optionsMenuUI.SetActive(false);
        }

        //Deactivate rebind menu
        if (rebindMenuUI != null)
        {
            rebindMenuUI.SetActive(false);
        }

        //Reset timescale and pause variables to normal
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    // P A U S E

    public void Pause()
    {
        if(pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    // L O A D    O P T I O N S   M E N U

    public void LoadOptionsMenu()
    {

        //Show Options Menu
        if (optionsMenuUI != null)
        {
            optionsMenuUI.SetActive(true);
        }

        //Hide Pause Menu and Rebind Menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (rebindMenuUI != null)
        {
            rebindMenuUI.SetActive(false);
        }
    }

    // L O A D   R E B I N D   M E N U
    public void LoadRebindMenu()
    {
        //Show Rebind Menu
        if (rebindMenuUI != null)
        {
            rebindMenuUI.SetActive(true);
        }

        //Hide Options Menu
        if (optionsMenuUI != null)
        {
            optionsMenuUI.SetActive(false);
        }
    }

    // Q U I T   G A M E
    public void QuitGame()
    {
        Debug.Log("Successfully quit!"); //Quitting doesn't show inside the Unity editor, so this should be tested differently
        Application.Quit();
    }

    // B A C K
    //options menu -> pause menu, this should be renamed to be more specific later once I know it works
    public void Back()
    {
        //Hide Options Menu
        if (optionsMenuUI != null)
        {
            optionsMenuUI.SetActive(false);
        }

        //Show Pause Menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }

    // M U S I C   S L I D E R
    public void SetMusicVolume(float musicVolume)
    {
        //Set the music mixer exposed parameter "musicVolume" to the float we take as input from the slider
        musicMixer.SetFloat("musicVolume", musicVolume);
    }

    //S F X   S L I D E R
    public void SetSFXVolume(float sfxVolume)
    {
        //Set the sfx mixer exposed parameter "sfxVolume" to the float we take as input from the slider
        sfxMixer.SetFloat("sfxVolume", sfxVolume);
    }

    // S E T   R E S O L U T I O N
    public void SetResolution(int resolutionIndex)
    {
        if (isInitializing) return; //Prevents this accidentally being called in Start()

        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true); //The boolean is for fullscreen or not
    }



    // S T A R T
    // Start is called before the first frame update
    void Start()
    {

        isInitializing = true;

        if (pauseMenuUI == null)
        {
            pauseMenuUI = GameObject.Find("PauseMenu");
        }

        if (optionsMenuUI == null)
        {
            optionsMenuUI = GameObject.Find("OptionsMenu");
        }

        if (rebindMenuUI == null)
        {
            rebindMenuUI = GameObject.Find("RebindMenu");
        }

        //Get player's screen resolutions + refresh rate
        allResolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        //Filter out resolutions the player's computer doesn't support
        for (int i=0; i<allResolutions.Length; i++)
        {
            if (allResolutions[i].refreshRateRatio.value == currentRefreshRate.value)
            {
                filteredResolutions.Add(allResolutions[i]);
            }
        }

        //Add applicable resolutions to a list
        List<string> resolutionOptions = new List<string>();

        for (int i=0; i<filteredResolutions.Count; i++)
        {
            //The following resolution option string is what the player sees
            string resOptString = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + (int)filteredResolutions[i].refreshRateRatio.value + " Hz";
            resolutionOptions.Add(resOptString);

            //Automatically choose starting resolution
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        //Include these resolution variables in the dropdown
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        //Resolution stuff is now over.

        isInitializing = false;

        //Start game in unpaused state
        Resume();

    }


    // U P D A T E
    // Update is called once per frame
    void Update()
    {

        //Hit escape to pause/unpause (replace this later with remappable keys)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
}
