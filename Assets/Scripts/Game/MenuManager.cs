using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class MenuManager : MonoBehaviour
{

    [Header("ControllerSettings")]
    [SerializeField]
    private StandaloneInputModule standaloneInput = null;
    [SerializeField]
    private Toggle toggleXbox = null;
    [SerializeField]
    private Toggle togglePs = null;
    [SerializeField]
    private GameObject controllerXboxSprite = null;
    [SerializeField]
    private GameObject controllerPSSprite = null;

    [Header("LanguageSettings")]
    [SerializeField]
    private Toggle toggleEn = null;
    [SerializeField]
    private Toggle toggleFr = null;

    [SerializeField]
    private LocalizationSettings localizationSettings = null;
    private bool languageFound = false;
    private int i = 0;


    public void Level_Basile()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void Start()
    {
        setStartController();
        setStartLanguage();
    }

    #region ControllerSettings
    private void setStartController()
    {
        if (!PlayerPrefs.HasKey("XboxController"))
            PlayerPrefs.SetInt("XboxController", 1);

        if (PlayerPrefs.GetInt("XboxController") == 1)
        {
            XboxControllerSelected();
            toggleXbox.isOn = true;
        }
        else
        {
            PSControllerSelected();
            toggleXbox.isOn = false;
        }
    }
    private void XboxControllerSelected()
    {
        standaloneInput.submitButton = "SubmitXB";
        controllerXboxSprite.SetActive(true);
        controllerPSSprite.SetActive(false);
    }
    private void PSControllerSelected()
    {
        standaloneInput.submitButton = "SubmitPS";
        controllerXboxSprite.SetActive(false);
        controllerPSSprite.SetActive(true);
    }

    public void SetController()
    {
        if (!toggleXbox.isOn)
        {
            PlayerPrefs.SetInt("XboxController", 0);
            PSControllerSelected();
        }
        else
        {
            PlayerPrefs.SetInt("XboxController", 1);
            XboxControllerSelected();
        }
    }

    #endregion
   
    #region LanguagesSettings
    private void setStartLanguage()
    {
        if (!PlayerPrefs.HasKey("LanguageIndex"))
            PlayerPrefs.SetInt("LanguageIndex", 0);

        if (PlayerPrefs.GetInt("LanguageIndex") == 0)
        {
            toggleEn.isOn = true;
        }
        else
        {
            toggleFr.isOn = false;
        }
    }

    private void setDefaultLanguage()
    {
        if (!languageFound)
        {
            //Le package Localization de Unity créer une erreur si je le set a la premiere frame
            if (i == 4)
            {
                localizationSettings.SetSelectedLocale(localizationSettings.GetAvailableLocales().Locales[PlayerPrefs.GetInt("LanguageIndex")]);
                languageFound = true;
            }

            i++;
        }
    }

    public void SetLanguage()
    {
        if (!toggleEn.isOn)
        {
            PlayerPrefs.SetInt("LanguageIndex", 1);
            if (languageFound)
                localizationSettings.SetSelectedLocale(localizationSettings.GetAvailableLocales().Locales[1]);
        }
        else
        {
            PlayerPrefs.SetInt("LanguageIndex", 0);
            if (languageFound)
                localizationSettings.SetSelectedLocale(localizationSettings.GetAvailableLocales().Locales[0]);
        }
    }
    #endregion

    public void Settings()
    {

    }

    private void Update()
    {
        setDefaultLanguage();
    }
}