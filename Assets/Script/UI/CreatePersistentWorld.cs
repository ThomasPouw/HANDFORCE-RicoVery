using Unity.Entities;
using UnityEngine;
using System.Linq;
using UnityEngine.LowLevel;
using UnityEngine.InputSystem.UI;

public class CreatePersistentWorld : MonoBehaviour
{
    [SerializeField] VirtualMouseInput virtualMouseInput;
    private void Awake()
    {
        foreach(World world in World.All)
        {
            if(world.Name == "TransferDataWorld")
            {
                return;
            }
        }
        World transferDataWorld =new World("TransferDataWorld", WorldFlags.Game);
        PlayerLoopSystem playerLoopSystem = PlayerLoop.GetDefaultPlayerLoop();
        ScriptBehaviourUpdateOrder.AppendWorldToPlayerLoop(transferDataWorld, ref playerLoopSystem);
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(virtualMouseInput.cursorTransform.position, 10);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(virtualMouseInput.cursorTransform.localPosition, 10);
    }
}
