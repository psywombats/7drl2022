using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(FieldSpritesheetComponent))]
public class FieldSpriteImage : MonoBehaviour {

    public OrthoDir facing = OrthoDir.South;

    public bool animates = false;
    public int stepsPerSecond = 2;

    private FieldSpritesheetComponent spritesheet;
    public FieldSpritesheetComponent Spritesheet {
        get {
            if (spritesheet == null) {
                spritesheet = GetComponent<FieldSpritesheetComponent>();
            }
            return spritesheet;
        }
    }
    
    private Image image;
    private Image Image {
        get {
            if (image == null) {
                image = GetComponent<Image>();
            }
            return image;
        }
    }

    private SpritesheetData data;

    public void Populate(SpritesheetData data) {
        this.data = data;
        Spritesheet.spritesheet = data;
        if (tag != null) {
            gameObject.SetActive(true);
            Image.sprite = Spritesheet.FrameForDirection(facing);
        } else {
            gameObject.SetActive(false);
        }
    }

    public void Clear() {
        Image.sprite = null;
    }

    public void Populate(Unit unit, bool faceNorth = false) {
        Populate(unit.Data.sprite);
        facing = (unit.IsDead ^ faceNorth) ? OrthoDir.North : OrthoDir.South;
        Image.color = unit.IsDead ? new Color(1, 1, 1, .4f) : Color.white;
        //Image.SetNativeSize();
    }

    public void Update() {
        var time = animates ? Time.time : 0f;
        Image.sprite = Spritesheet.GetFrame(facing, (int)(time * stepsPerSecond) % Spritesheet.StepCount);
    }
}
