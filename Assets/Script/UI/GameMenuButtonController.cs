using System.Collections;
using HANDFORCE.TCCavy.Balloon;
using HANDFORCE.TCCavy.General.Data;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonsController : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject settingsScreen;
    public GameObject resultScreen;
    public GameObject gameUI;
    public GameObject mouseUI;
    public EntityManager _entityManager;
    public Entity _entity;
    private IEnumerator Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        yield return new WaitForSeconds(0.2f);
        _entity = _entityManager.CreateEntityQuery(typeof(Timer), typeof(TimeLeftWave)).GetSingletonEntity();
        GameInputSystem.OnPaused += ToPauseScreen;
        BalloonWaveSpawnSystem.GameClear += ToResultScreen;
        //EntityManager find
        //
    }
    public void ToResultScreen()
    {
        if(!_entityManager.IsComponentEnabled<Result>(_entity))
        {
            _entityManager.SetComponentEnabled<Paused>(_entity, false);
            _entityManager.SetComponentEnabled<Result>(_entity, true);
            gameUI.SetActive(false);
            pauseScreen.SetActive(false);
            settingsScreen.SetActive(false);
            resultScreen.SetActive(true);
            mouseUI.SetActive(true);
        }
    }
    public void ToPauseScreen()
    {
        if(!_entityManager.IsComponentEnabled<Paused>(_entity) && !_entityManager.IsComponentEnabled<Result>(_entity))
        {
            _entityManager.SetComponentEnabled<Paused>(_entity, true);
            gameUI.SetActive(false);
            pauseScreen.SetActive(true);
            settingsScreen.SetActive(false);
            mouseUI.SetActive(true);
        }
    }
    public void BackToPauseFromSettings()
    {
        pauseScreen.SetActive(true);
        settingsScreen.SetActive(false);
    }

    public void GoToSettings()
    {
        //Enable the game cursor. (As UI has a diffrent cursor then ingame.)
        settingsScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }

    public void BackToGame()
    {
        _entityManager.SetComponentEnabled<Paused>(_entity, false);
        gameUI.SetActive(true);
        pauseScreen.SetActive(false);
        mouseUI.SetActive(false);
        //Enable the game cursor. (As UI has a diffrent cursor then ingame.)
    }
    public void BackToHomeMenu()
    {
        _entityManager.SetComponentEnabled<Paused>(_entity, true);
        SceneManager.LoadScene(0);
        //gameObject.SetActive(false);
        //Enable the game cursor. (As UI has a diffrent cursor then ingame.)
    }
    private void OnDestroy() {
        GameInputSystem.OnPaused -= ToPauseScreen;
        BalloonWaveSpawnSystem.GameClear -= ToResultScreen;
    }
}
