using HANDFORCE.TCCavy.Aim.Data;
using Unity.Entities;
using UnityEngine;

namespace HANDFORCE.TCCavy.Aim.Authoring
{
    public class LaserAuthoring : MonoBehaviour
    {
        [Tooltip("Switches the purpose of this Authoring from LaserOrigin on True and LaserSelector on False")]
        public bool Origin= false;
        [Header("Selector")]
        public bool reversed = false;
        public float time;
        [Header("Origin")]
        public float size;

        class LaserAuthoringBaker : Baker<LaserAuthoring>
        {
            public override void Bake(LaserAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                if(!authoring.Origin)
                {
                    AddComponent(entity, new LaserSelector
                    {
                        reversed = authoring.reversed,
                        timeNeeded = authoring.time,
                    });
                    AddComponent<newLaser>(entity);
                    SetComponentEnabled<newLaser>(entity, false);
                }
                else
                {
                    AddComponent(entity, new LaserOrigin
                    {
                        size = authoring.size,
                    });
                }
            }
        }
    }
}
