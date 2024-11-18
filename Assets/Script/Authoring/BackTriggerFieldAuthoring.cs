using HANDFORCE.TCCavy.Aim.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HANDFORCE.TCCavy.Aim.Authoring
{
    public class BackTriggerFieldAuthoring : MonoBehaviour
    {

        class BackTriggerFieldBaker : Baker<BackTriggerFieldAuthoring>
        {
            public override void Bake(BackTriggerFieldAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<BackTriggerField>(entity); //For the width and height use the ECS render bounds.
            }
        }
    }
}