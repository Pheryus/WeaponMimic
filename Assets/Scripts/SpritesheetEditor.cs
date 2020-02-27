using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SpritesheetEditor))]
public class SpritesheetEditor : EditorWindow {

    public Vector2 offset;

    string spriteName;

    [MenuItem("Window/SpritesheetEditor")]
    static void Init() {
        SpritesheetEditor window = (SpritesheetEditor)EditorWindow.GetWindow(typeof(SpritesheetEditor));
        window.Show();
    }
        
    void SetOffset() {
        Debug.Log("spriteName: " + spriteName);
        Texture2D s = (Texture2D)AssetDatabase.LoadAssetAtPath(spriteName, typeof(Texture2D));

        string path = AssetDatabase.GetAssetPath(s);
            
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        ti.isReadable = true;

        SpriteMetaData[] sprites = ti.spritesheet;
           
        for (int i = 0; i < sprites.Length; i++) {
            Debug.Log(ti.spritesheet[i].pivot);
            sprites[i].pivot = new Vector2(offset.x, offset.y);
            Debug.Log(sprites[i].pivot);

            Debug.Log("name: " + ti.spritesheet[i].name);
        }

        ti.spritesheet = sprites;

        EditorUtility.SetDirty(ti);

        ti.SaveAndReimport();
        AssetDatabase.ImportAsset(path);


    }


    void OpenFileSearch() {
        spriteName = "Assets/Sprites/Player/Protag_Jump.png"; //EditorUtility.OpenFilePanel("", "C:/Github/MimicWeapon/Assets/Sprites", "");
    }

    private void OnGUI() {

        offset.x = EditorGUILayout.FloatField("x:", offset.x);
        offset.y = EditorGUILayout.FloatField("y:", offset.y);

        if (GUILayout.Button("LoadSprites")) {
            OpenFileSearch();
            SetOffset();
        }

        
    }
}
