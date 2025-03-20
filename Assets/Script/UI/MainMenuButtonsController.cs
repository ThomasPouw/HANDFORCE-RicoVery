using System.Collections;
using Unity.Entities;
using UnityEngine;

public class MainMenuButtonsController : MonoBehaviour
{
    public GameObject controls;
    public GameObject settingsScreen;
    public GameObject levelSelectScreen;
    public GameObject mainScreen;
    /*public EntityManager _entityManager;
    private IEnumerator Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        yield return new WaitForSeconds(0.2f);
    }*/
    public void ToMainScreen()
    {
        controls.SetActive(false);
        settingsScreen.SetActive(false);
        levelSelectScreen.SetActive(false);
        mainScreen.SetActive(true);
    }
    public void ToSettings()
    {
        controls.SetActive(false);
        settingsScreen.SetActive(true);
        levelSelectScreen.SetActive(false);
        mainScreen.SetActive(false);
    }
    public void ToLevelSelect()
    {
        controls.SetActive(false);
        settingsScreen.SetActive(false);
        levelSelectScreen.SetActive(true);
        mainScreen.SetActive(false);
    }
    public void ToControls()
    {
        controls.SetActive(true);
        settingsScreen.SetActive(false);
        levelSelectScreen.SetActive(false);
        mainScreen.SetActive(false);
    }
    
}
