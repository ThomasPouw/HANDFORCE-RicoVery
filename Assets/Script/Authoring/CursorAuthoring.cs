using HANDFORCE.TCCavy.Aim.Data;
using Unity.Entities;
using UnityEngine;

namespace HANDFORCE.TCCavy.Aim.Authoring
{
    public class CursorAuthoring : MonoBehaviour
    {
        public Color targetColour;
        public Color noTargetColour;
        private class CursorAuthoringBaker : Baker<CursorAuthoring>
        {
            public override void Bake(CursorAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AimRadicle
                {
                    NoTargetColour = authoring.noTargetColour,
                    TargetColour = authoring.targetColour,
                });
                AddComponent<RadicleOnBalloon>(entity);
            }
        }
    }
}