using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IndexDatabase", menuName = "Data/IndexDatabase")]
public class IndexDatabase : ScriptableObject {

    public SoundEffectIndexData SFX;
    public BGMIndexData BGM;
    public UnitIndex Units;

    public static IndexDatabase Instance() {
        return Resources.Load<IndexDatabase>("Database/Database");
    }

    public List<IIndexPopulater> GetScriptableIndices() => new List<IIndexPopulater> {
        CombatItems,
        Parties,
        Units,
        Statuses,
        Recruits,
        MeatGroups,
        Shops,
        Collectables,
        EncounterSets,
        TerrainEncounterSets,
        Encounters,
        MeatShops,
        MixSets,
    };
}
