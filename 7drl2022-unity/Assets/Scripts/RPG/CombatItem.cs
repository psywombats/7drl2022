using System;
using System.Threading.Tasks;

public class CombatItem : IComparable<CombatItem> {

    public CombatItemData Data { get; protected set; }

    public Inventory Container { get; protected set; }
    public int UsesWhenAdded { get; protected set; }
    public int UsesRemaining { get; protected set; }
    public StatSet Stats { get; protected set; }
    public AbilEffect Effect { get; protected set; }

    public int GoldValue => Data.uses > 0 ? Data.cost * UsesRemaining / Data.uses : Data.cost;
    public int SellValue => GoldValue / 2;
    public bool IsBattleUseable => Effect.IsBattleUsable() && (UsesRemaining > 0 || Data.uses == 0);
    public bool IsMapUseable => Effect.IsMapUsable();
    public StatSet RoboStats => Data.robostats;
    public string Name => UIUtils.GlyphifyString(Data.abilityName);
    public Unit Owner => Container?.Owner;
    public bool IsUseRegenerating => Container == null ? false : Container.IsUseRegeneratingAt(Container.SlotForItem(this));
    public bool RaceCanEquip(Race race) => Data.RaceCanEquip(race);

    private BattleAnim anim;
    public BattleAnim Anim {
        get {
            if (anim == null && Data.anim != null) {
                anim = BattleAnimationFactory.Create(Data.anim);
            }
            return anim;
        }
    }

    public string Article {
        get {
            var c = Name[0];
            if (c < 'A' || c > 'z') {
                c = Name[1];
            }
            if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'x') {
                return "an";
            } else {
                return "a";
            }
        }
    }

    protected CombatItem() {
        Stats = new StatSet();
    }

    public CombatItem(CombatItemData data) : this() {
        Data = data;
        UsesRemaining = data.uses;
        if (data.warhead == null) {
            Global.Instance().Debug.LogError("No warhead for item: " + data.name);
        } else {
            Effect = data.warhead.GenerateEffect(this);
        }
    }

    public CombatItem(SerializedCombatItem data) : this() {
        Data = IndexDatabase.Instance().CombatItems.GetData(data.dataKey);
        UsesWhenAdded = data.usesWhenAdded;
        UsesRemaining = data.usesRemaining;
        if (Data == null || Data.warhead == null) {
            throw new Exception("No warhead data for " + data.dataKey);
        }
        Effect = Data.warhead.GenerateEffect(this);
    }

    public override string ToString() {
        return Name + " (" + UsesRemaining + "/" + Data.uses + ")";
    }

    /// <param name="inventory">The inventory being added to, maybe null</param>
    public void OnAddedTo(Inventory inventory, bool isFirstEquip) {
        Container = inventory;
        if (isFirstEquip) {
            UsesWhenAdded = UsesRemaining;
        }
    }

    public bool CanRestoreUses() {
        if (Global.Instance().Data.Mode == GameData.GameMode.Casual) {
            return Data.uses > 0;
        } else {
            return Data.type == AbilityType.ABILITY && Data.uses > 0;
        }
    }

    public bool CanRestoreUsesFromInventory() {
        return Global.Instance().Data.Mode == GameData.GameMode.Casual && !Effect.IsConsumeable();
    }

    public void RestoreUses() {
        if (Global.Instance().Data.Mode == GameData.GameMode.Casual) {
            UsesRemaining = UsesWhenAdded;
        } else {
            UsesRemaining = Data.uses;
        }       
    }

    public async Task<bool> UseOnMapAsync(IItemUseableMenu menu, Unit user) {
        try {
            return await Effect.UseOnMapAsync(menu, user);
        } catch (Exception e) {
            Global.Instance().Debug.LogError("Error using " + Name + ": " + e);
            return false;
        }
    }

    public void HalveUses() {
        UsesRemaining /= 2;
        UsesWhenAdded = UsesRemaining;
        DiscardIfNeeded();
    }

    public void DeductUse() {
        if (UsesRemaining > 0) {
            UsesRemaining -= 1;
        }
        DiscardIfNeeded();
    }

    public void DiscardIfNeeded() {
        if (ShouldDiscard()) {
            Container.Drop(this);
        }
    }

    public bool ShouldDiscard(Inventory newContainer = null) {
        if (newContainer == null) newContainer = Container;
        if (UsesRemaining > 0) return false;
        if (Data.uses == 0) return false;
        if (Data.type == AbilityType.ABILITY) return false;
        if (newContainer != null && newContainer.IsUseRegeneratingAt(newContainer.SlotForItem(this)) && UsesWhenAdded > 0) return false;
        return true;
    }

    public void ApplyArcane() {
        UsesRemaining = Data.uses;
        UsesWhenAdded = Data.uses;
    }

    public string GetExtendedDescription() {
        var desc = RpgEnumUtils.AbilityTypeToString(Data.type);
        desc += ": " + Name;
        if (Data.uses > 0) {
            desc += "\nUses: " + UsesRemaining + " before ";
            if (IsUseRegenerating) {
                desc += "requiring recharge at an inn,";
            } else {
                desc += "breaking, ";
            }
            if (Owner != null && Owner.Race == Race.ROBOT) {
                desc += " max " + UsesWhenAdded + " uses";
            } else {
                desc += " max " + Data.uses + " uses";
            }
        }

        if (Data.robostats != null && !Data.robostats.IsEmpty()) {
            desc += "\nRobot stat bonus: " + Data.robostats.GetOneLiner();
        }

        if (Owner != null) {
            switch (Owner.Race) {
                case Race.ROBOT:
                    if (Data.uses > 0) {
                        desc += "\nRobot item: uses halved on equip/deequip, but recharged at inns";
                    }
                    break;
                case Race.MUTANT:
                    if (IsUseRegenerating && Data.uses > 0) {
                        desc += "\nMutant ability: recharges at inns, replaceable with new mutation";
                    } else if (Container.IsSlotReservedAt(Container.SlotForItem(this))) {
                        desc += "\nMutant ability: replaceable with new mutation";
                    }
                    break;
                case Race.MONSTER:
                    if (Data.uses > 0) {
                        desc += "\nMonster ability: recharges at inns, replaced when assuming new form";
                    } else {
                        desc += "\nMonster ability: replaced when assuming new form";
                    }
                    
                    break;
            }
        }

        desc += "\n" + Effect.GetExtendedDescription(Owner);
        return desc;
    }

    public int CompareTo(CombatItem other) {
        if (other == null) {
            return 1;
        }
        if (!IsMapUseable && other.IsMapUseable) {
            return 1;
        }
        if (IsMapUseable && !other.IsMapUseable) {
            return -1;
        }
        if (!IsMapUseable && !other.IsMapUseable) {
            return 0;
        }

        if (!(Effect is EffectStatCandy) && other.Effect is EffectStatCandy) {
            return -1;
        }
        if (Effect is EffectStatCandy && !(other.Effect is EffectStatCandy)) {
            return -1;
        }
        return 0;
    }
}
