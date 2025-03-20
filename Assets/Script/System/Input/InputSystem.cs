using System;
using HANDFORCE.TCCavy.Aim.Data;
using HANDFORCE.TCCavy.Balloon.Data;
using HANDFORCE.TCCavy.Controller.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class GameInputSystem : SystemBase, PlayerInput_Mapping.IJoyStickActions
{
    public static Action OnPaused;
    public static Action<LaserDirection> OnLaserChanged;
    public PlayerInput_Mapping m_InputActions;
    private EntityQuery m_PlayerMovementInputQuery;
    //private EntityQuery m_LaserSelectionQuery;
    private float2 m_CursorPosition;
    private bool m_Shoot;
    private bool m_Paused;
    private LaserDirection m_LaserDirection = LaserDirection.Up;
    [BurstCompile]
    protected override void OnCreate()
    {
        m_InputActions = new PlayerInput_Mapping();
        //m_InputActions.Controller.SetCallbacks(this);
        m_InputActions.JoyStick.SetCallbacks(this);
        m_PlayerMovementInputQuery = GetEntityQuery(typeof(RawControllerInput));
        //m_LaserSelectionQuery = GetEntityQuery(typeof(LaserSelector));
        m_InputActions.Enable();
    }
    [BurstCompile]
    protected override void OnUpdate()
    {
        if (m_PlayerMovementInputQuery.CalculateEntityCount() == 0)
        {
            Entity entity = EntityManager.CreateEntity(typeof(RawControllerInput));
            
        }
        if(m_PlayerMovementInputQuery.GetSingleton<RawControllerInput>().laserDirection != m_LaserDirection)
        {
            foreach (var (_, entity) in SystemAPI.Query<LaserSelector>().WithEntityAccess())
            {
                EntityManager.SetComponentEnabled<newLaser>(entity, true);
            }
        }
            //Debug.Log($"m_CursorPosition {m_CursorPosition} m_Shoot: {m_Shoot}");
        m_PlayerMovementInputQuery.SetSingleton(new RawControllerInput
        {
            cursorPosition = m_CursorPosition,
            shoot = m_Shoot,
            laserDirection = m_LaserDirection
        });
    }

    public void OnCursor_Stick(InputAction.CallbackContext context) => m_CursorPosition =(float2)context.ReadValue<Vector2>();

    public void OnShootConfirm(InputAction.CallbackContext context)
    {
        m_Shoot = context.performed;
    }

    public void OnPausing(InputAction.CallbackContext context) 
    {
        OnPaused?.Invoke();
    }

    public void OnLaserSwitch(InputAction.CallbackContext context)
    {
        float2 hatVector = (float2)context.ReadValue<Vector2>();
        if(hatVector.Equals(float2.zero))
            return;
        if (hatVector.Equals(new float2(0, 1)) || hatVector.Equals(new float2(0.71f, 0.71f)))
            m_LaserDirection = LaserDirection.Up;
        else if(hatVector.Equals(new float2(1, 0)) || hatVector.Equals(new float2(0.71f, -0.71f)))
            m_LaserDirection = LaserDirection.Right;
        else if(hatVector.Equals(new float2(0, -1)) || hatVector.Equals(new float2(-0.71f, -0.71f)))
            m_LaserDirection = LaserDirection.Down;
        else if(hatVector.Equals(new float2(-1, 0)) || hatVector.Equals(new float2(-0.71f, 0.71f)))
            m_LaserDirection = LaserDirection.Left;
        
        
    }
}