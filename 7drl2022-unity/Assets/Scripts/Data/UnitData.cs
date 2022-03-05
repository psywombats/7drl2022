using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "Unit", menuName = "Data/Rpg/UnitData")]
public class UnitData : MainSchema {

    [SerializeField] public SpritesheetData sprite;
}