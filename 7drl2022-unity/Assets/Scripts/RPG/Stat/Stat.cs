using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Stats are representing as instances of this class, eg STR is an instance of Stat that has an
 * additive mixin, int display, etc. Enums aren't powerful enough to do what we want in C#. Instead
 * there's a StatTag that hooks into this class.
 */
public class Stat {

    public CombinationStrategy Combinator { get; private set; }
    public StatTag Tag { get; private set; }
    public string NameShort { get; private set; }
    public string NameLong { get; private set; }
    public bool UseBinaryEditor { get; private set; }

    private static Dictionary<StatTag, Stat> stats;

    private Stat(StatTag tag, CombinationStrategy combinator, string nameShort, string nameLong, bool useBinaryEditor) {
        Combinator = combinator;
        Tag = tag;
        NameShort = nameShort;
        NameLong = nameLong;
        UseBinaryEditor = useBinaryEditor;
    }

    public static Stat Get(StatTag tag) {
        if (stats == null) {
            InitializeStats();
        }
        if (!stats.ContainsKey(tag)) {
            return null;
        }
        return stats[tag];
    }

    public static Stat Get(int enumIndex) {
        return Get((StatTag)enumIndex);
    }

    private static void InitializeStats() {
        stats = new Dictionary<StatTag, Stat>();
        AddStat(StatTag.MHP, "Max health");
        AddStat(StatTag.HP, "Health");
        AddStat(StatTag.STR, "Strength");
        AddStat(StatTag.AGI, "Agility");
        AddStat(StatTag.DEF, "Defense");
        AddStat(StatTag.MANA, "Mana");
        AddStat(StatTag.MANA_DEFENSE, "Mana (def)");

        foreach (StatTag tag in new StatTag[] {     StatTag.RESIST_DAMAGE,
                StatTag.RESIST_WEAPON,
                StatTag.IMMUNE_WEAPON,
                StatTag.IMMUNE_RANGED,

                StatTag.RESIST_BLIND,
                StatTag.RESIST_CURSE,
                StatTag.RESIST_CONFUSE,
                StatTag.RESIST_SLEEP,
                StatTag.RESIST_PARALYZE,
                StatTag.RESIST_STONE,
                StatTag.RESIST_DEATH,
                StatTag.RESIST_POISON,

                StatTag.RESIST_FIRE,
                StatTag.RESIST_ICE,
                StatTag.RESIST_THUNDER,
                StatTag.RESIST_EARTH,
                StatTag.RESIST_TYPELESS,
                StatTag.RESIST_CRITICALS,

                StatTag.WEAK_FIRE,
                StatTag.WEAK_ICE,
                StatTag.WEAK_THUNDER,
                StatTag.WEAK_EARTH,
                StatTag.UNDEAD,

                StatTag.AMBUSHER,
                StatTag.NO_AMBUSH,

                StatTag.REGENERATING,
                StatTag.REFRESHING,

                StatTag.EQUIPMENT_FIX }) {
            AddFlag(tag);
        }
    }

    private static void AddStat(StatTag tag, string longName = null) {
        stats[tag] = new Stat(tag, CombinationAdditive.Instance(), tag.ToString(), longName, false);
    }

    private static void AddFlag(StatTag tag) {
        stats[tag] = new Stat(tag, CombinationAdditive.Instance(), tag.ToString(), null, true);
    }
}
