using System.Collections;
using Unity.Entities;
using UnityEngine;

public class MainMenuButtonsController : MonoBehaviour
{
    public GameObject titleScreen;
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
        titleScreen.SetActive(false);
        settingsScreen.SetActive(false);
        levelSelectScreen.SetActive(false);
        mainScreen.SetActive(true);
    }
    public void ToSettings()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(true);
        levelSelectScreen.SetActive(false);
        mainScreen.SetActive(false);
    }
    public void ToLevelSelect()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(false);
        levelSelectScreen.SetActive(true);
        mainScreen.SetActive(false);
    }
}
