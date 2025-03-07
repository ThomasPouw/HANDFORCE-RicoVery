using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
    private bool active = false;
    public void ChangeLocale(int localeID)
    {
        if(active) return;
        StartCoroutine(SetLocale(localeID));
    }
    
    IEnumerator SetLocale(int _localID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localID];
    }
}
