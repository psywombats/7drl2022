using System;
using System.Collections.Generic;

/// <summary>
/// This is mainly to match the serialziation format of MGNE... but I guess we're using it now
/// </summary>
[Serializable]
public class SerializedStatSet {

    [UnityEngine.Tooltip("HP")]
    public int hp;

    [UnityEngine.Tooltip("MHP")]
    public int mhp;

    [UnityEngine.Tooltip("STR")]
    public int str;

    [UnityEngine.Tooltip("DEF")]
    public int def;

    [UnityEngine.Tooltip("AGI")]
    public int agi;

    [UnityEngine.Tooltip("MANA")]
    public int mana;

    [UnityEngine.Tooltip("MANA (hidden def bonus)")]
    public int manaDefense;

    public List<StatTag> flagKeys;
    public List<float> flagValues;

    public SerializedStatSet() {
        // serialized
    }

    public SerializedStatSet(StatSet stats) {
        hp = (int)stats[StatTag.HP];
        mhp = (int)stats[StatTag.MHP];
        str = (int)stats[StatTag.STR];
        def = (int)stats[StatTag.DEF];
        agi = (int)stats[StatTag.AGI];
        mana = (int)stats[StatTag.MANA];
        manaDefense = (int)stats[StatTag.MANA_DEFENSE];

        flagKeys = new List<StatTag>();
        flagValues = new List<float>();

        foreach (StatTag tag in Enum.GetValues(typeof(StatTag))) {
            if (tag == StatTag.NONE) {
                continue;
            }
            Stat stat = Stat.Get(tag);
            if (stat.UseBinaryEditor && stats[tag] != 0) {
                flagKeys.Add(tag);
                flagValues.Add(stats[tag]);
            }
        }
    }
}
