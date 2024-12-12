using System.Collections;
using HANDFORCE.TCCavy.Controller.Data;
using Unity.Entities;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Scrollbar slider_X;
    [SerializeField] private Scrollbar slider_Y;
    [SerializeField] private Toggle inverse_X;
    [SerializeField] private Toggle inverse_Y;

    private EntityManager _entityManager;
    private Entity _entity;
    private void OnEnable()
    {
        foreach(World world in World.All)
        {
            Debug.Log(world.Name);
            if(world.Name == "TransferDataWorld")
            {
                _entityManager = world.EntityManager;
            }
        }
        if(_entityManager.CreateEntityQuery(typeof(SettingsData)).CalculateEntityCount() == 0)
        {
            _entity = _entityManager.CreateEntity(typeof(SettingsData));
            _entityManager.SetComponentData<SettingsData>(_entity, new SettingsData
            {
                inverseX = false,
                inverseY = false,
                joystickSensitivityX = 1,
                joystickSensitivityY = 1,
            });
        }
        else{
            _entity = _entityManager.CreateEntityQuery(typeof(SettingsData)).GetSingletonEntity();
        }
        //yield return new WaitForSeconds(0.2f);
        slider_X.onValueChanged.AddListener(SensitivityX);
        slider_Y.onValueChanged.AddListener(SensitivityY);
        inverse_X.onValueChanged.AddListener(InverseControlsX);
        inverse_Y.onValueChanged.AddListener(InverseControlsY);
        //EntityManager find
        //
    }
    public void InverseControlsX(bool X)
    {
        SettingsData settingsData =_entityManager.GetComponentData<SettingsData>(_entity);
        settingsData.inverseX = X;
        _entityManager.SetComponentData<SettingsData>(_entity, settingsData);
    }
    public void InverseControlsY(bool Y)
    {
        SettingsData settingsData =_entityManager.GetComponentData<SettingsData>(_entity);
        settingsData.inverseY = Y;
        _entityManager.SetComponentData<SettingsData>(_entity, settingsData);
    }
    public void SensitivityX(float sensitivity)
    {
        //Debug.Log(sensitivity);
        SettingsData settingsData =_entityManager.GetComponentData<SettingsData>(_entity);
        settingsData.joystickSensitivityX = sensitivity;
        _entityManager.SetComponentData<SettingsData>(_entity, settingsData);
    }
    public void SensitivityY(float sensitivity)
    {
        SettingsData settingsData =_entityManager.GetComponentData<SettingsData>(_entity);
        settingsData.joystickSensitivityY = sensitivity;
        _entityManager.SetComponentData<SettingsData>(_entity, settingsData);
    }
    private void OnDisable()
    {
        slider_X.onValueChanged.RemoveListener(SensitivityX);
        slider_Y.onValueChanged.RemoveListener(SensitivityY);
        inverse_X.onValueChanged.RemoveListener(InverseControlsX);
        inverse_Y.onValueChanged.RemoveListener(InverseControlsY);
    }
}
