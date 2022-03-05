using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

/**
 * A stat set is a collection of stats of different types.
 * It can represent a modifier set (+3 str sword) or a base set (Alex, 10 str)
 * */
[Serializable]
public class StatSet : ISerializationCallbackReceiver {

    [SerializeField] private StatDictionary serializedStats;
    private Dictionary<StatTag, float> stats;

    public StatSet() {
        stats = new Dictionary<StatTag, float>();
    }

    public StatSet(StatSet other) : this(other.serializedStats) {
    }

    public StatSet(SerializedStatSet serialized) : this() {
        this[StatTag.STR] = serialized.str;
        this[StatTag.DEF] = serialized.def;
        this[StatTag.AGI] = serialized.agi;
        this[StatTag.MANA] = serialized.mana;
        this[StatTag.HP] = serialized.hp;
        this[StatTag.MHP] = serialized.mhp;
        this[StatTag.MANA_DEFENSE] = serialized.manaDefense;
        if (serialized.flagKeys != null) {
            for (var i = 0; i < serialized.flagKeys?.Count; i += 1) {
                this[serialized.flagKeys[i]] = serialized.flagValues[i];
            }
        }
    }

    private StatSet(StatDictionary stats) : this() {
        Dictionary<string, float> statStrings = stats.ToDictionary();
        foreach (var stat in statStrings) {
            if (Enum.TryParse(stat.Key, true, out StatTag result)) {
                this[result] = stat.Value;
            }
        }
    }

    // === ACCESSORS ===

    public float Get(StatTag tag) {
        if (tag == StatTag.NONE) return 0.0f;
        return stats.ContainsKey(tag) ? stats[tag] : Stat.Get(tag).Combinator.Identity();
    }

    public bool Is(StatTag tag) {
        return Get(tag) > 0.0f;
    }

    public void Set(StatTag tag, float value) {
        stats[tag] = value;
    }

    public float this[StatTag tag] {
        get { return Get(tag); }
        set { Set(tag, value); }
    }

    // === OPERATIONS ===

    public void Add(StatTag tag, float value) {
        this[tag] += value;
    }

    public void Sub(StatTag tag, float value) {
        Add(tag, -value);
    }

    public static StatSet operator +(StatSet a, StatSet b) => a.AddSet(b);
    public StatSet AddSet(StatSet other) {
        foreach (StatTag tag in other.stats.Keys) {
            this[tag] = Stat.Get(tag).Combinator.Combine(this[tag], other[tag]);
        }
        return this;
    }

    public static StatSet operator -(StatSet a, StatSet b) => a.RemoveSet(b);
    public StatSet RemoveSet(StatSet other) {
        foreach (var tag in other.stats.Keys) {
            this[tag] = Stat.Get(tag).Combinator.Decombine(this[tag], other[tag]);
        }
        return this;
    }

    // === SERIALIZATION ===

    [OnSerializing()]
    internal void OnSerializingMethod(StreamingContext context) {
        WriteSerializedStats();
    }

    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context) {
        ReadFromSerializedStats();
    }

    public void WriteSerializedStats() {
        if (stats.Count == 0) return;
        var statStrings = new Dictionary<string, float>();
        foreach (var stat in stats) {
            if (stat.Value != Stat.Get(stat.Key).Combinator.Identity()) {
                statStrings[stat.Key.ToString()] = stat.Value;
            }
        }
        serializedStats = new StatDictionary(statStrings);
    }

    public void ReadFromSerializedStats() {
        Dictionary<string, float> statStrings = serializedStats.ToDictionary();
        foreach (var stat in statStrings) {
            if (Enum.TryParse(stat.Key, true, out StatTag result)) {
                this[result] = stat.Value;
            }
        }
    }

    public void OnBeforeSerialize() {
        WriteSerializedStats();
    }

    public void OnAfterDeserialize() {
        ReadFromSerializedStats();
    }

    [Serializable]
    public class StatDictionary : SerialDictionary<string, float> {
        public StatDictionary(Dictionary<string, float> dictionary) : base(dictionary) {

        }
    }

    // === MISC ===

    public bool IsEmpty() {
        return !stats.Any(pair => pair.Value != 0.0f);
    }

    public string GetOneLiner() {
        var result = "";
        foreach (StatTag tag in Enum.GetValues(typeof(StatTag))) {
            if (this[tag] != 0) {
                if (result.Length > 0) {
                    result += ", ";
                }
                if (Stat.Get(tag).UseBinaryEditor) {
                    result += tag.Name();
                } else {
                    result += tag.Name() + ": " + (int)this[tag];
                }
            }
        }
        return result;
    }

    public override string ToString() {
        var result = "";
        result += "HP: " + this[StatTag.MHP] + "/" + this[StatTag.HP] + " ";
        result += "STR: " + this[StatTag.STR] + " ";
        result += "AGI: " + this[StatTag.AGI] + " ";
        result += "DEF: " + this[StatTag.DEF] + " ";
        result += "MAN: " + this[StatTag.MANA];
        return result;
    }
}
