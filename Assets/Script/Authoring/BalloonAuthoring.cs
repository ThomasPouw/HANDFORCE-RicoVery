using HANDFORCE.TCCavy.Balloon.Data;
using Unity.Entities;
using UnityEngine;

namespace HANDFORCE.TCCavy.Balloon.Authoring
{
    public class BalloonAuthoring : MonoBehaviour
    {

        class BackTriggerFieldBaker : Baker<BalloonAuthoring>
        {
            public override void Bake(BalloonAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<BalloonColour>(entity); //For the width and height use the ECS render bounds.
            }
        }
    }
}
