using System;

public enum StatTag {

    [StatTag("MHP")]    MHP = 0,
    [StatTag("HP")]     HP = 1,

    [StatTag("STR")]    STR = 2,
    [StatTag("AGI")]    AGI = 3,
    [StatTag("DEF")]    DEF = 4,
    [StatTag("MANA")]   MANA = 5,
    [StatTag("Mana (defense only)")] MANA_DEFENSE = 33,

    [StatTag("Physical damage resistance")] RESIST_DAMAGE = 6,
    [StatTag("Weapon damage resistance")]   RESIST_WEAPON = 7,
    [StatTag("Projectile immunity")]        IMMUNE_RANGED = 35,
    [StatTag("Weapon damage immunity")]     IMMUNE_WEAPON = 8,

    [StatTag("Blindness immunity")]         RESIST_BLIND = 9,
    [StatTag("Curse immunity")]             RESIST_CURSE = 10,
    [StatTag("Confusion immunity")]         RESIST_CONFUSE = 11,
    [StatTag("Sleep immunity")]             RESIST_SLEEP = 12,
    [StatTag("Paralysis immunity")]         RESIST_PARALYZE = 13,
    [StatTag("Petrification immunity")]     RESIST_STONE = 14,
    [StatTag("Instadeath immunity")]        RESIST_DEATH = 15,
    [StatTag("Poison immunity")]            RESIST_POISON = 16,
    [StatTag("Critical hit immunity")]      RESIST_CRITICALS = 32,

    [StatTag("Fire immunity")]              RESIST_FIRE = 17,
    [StatTag("Ice immunity")]               RESIST_ICE = 18,
    [StatTag("Thunder immunity")]           RESIST_THUNDER = 19,
    [StatTag("Earth immunity")]             RESIST_EARTH = 20,
    [StatTag("Typeless damage resistance")] RESIST_TYPELESS = 21,

    [StatTag("Fire vulnerability")]         WEAK_FIRE = 22,
    [StatTag("Ice vulnerability")]          WEAK_ICE = 23,
    [StatTag("Thunder vulnerability")]      WEAK_THUNDER = 24,
    [StatTag("Earth vulnerability")]        WEAK_EARTH = 25,
    [StatTag("Undead")]                     UNDEAD = 26,

    [StatTag("Ambush encounter chance")]    AMBUSHER = 27,
    [StatTag("Ambush encounter prevention")] NO_AMBUSH = 28,

    [StatTag("Regeneration")]               REGENERATING = 29,
    [StatTag("Status autohealing")]         REFRESHING = 34,

    [StatTag("Equipment fix")]              EQUIPMENT_FIX = 30,

    [StatTag("(none)")]                     NONE = 31,
}

public class StatTagAttribute : Attribute {

    public string Name { get; private set; }

    internal StatTagAttribute(string name) {
        Name = name;
    }
}

public static class StatTagExtensions {

    public static string Name(this StatTag tag) { return tag.GetAttribute<StatTagAttribute>().Name; }
}
