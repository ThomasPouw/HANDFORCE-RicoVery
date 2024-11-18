using HANDFORCE.TCCavy.Aim.Data;
using Unity.Entities;
using UnityEngine;

namespace HANDFORCE.TCCavy.Aim.Authoring
{
    public class GunBaseAuthoring : MonoBehaviour
    {

        class GunBaseAuthoringBaker : Baker<GunBaseAuthoring>
        {
            public override void Bake(GunBaseAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<GunBase>(entity);
            }
        }
    }
}
