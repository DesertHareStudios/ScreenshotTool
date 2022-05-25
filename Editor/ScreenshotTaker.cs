using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Screenshot : EditorWindow {

    public string lastScreenshot = "";
    public Camera myCamera;

    int resWidth = Screen.width * 4;
    int resHeight = Screen.height * 4;
    int scale = 1;
    float lastTime;
    string path = "";
    bool takeHiResShot = false;
    bool takeShot = false;
    RenderTexture renderTexture;

    [MenuItem("Window/Desert Hare Studios/Screenshot Tool")]
    public static void ShowWindow() {
        EditorWindow editorWindow = GetWindow(typeof(Screenshot));
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show();
        editorWindow.titleContent = new GUIContent("Screenshot");
    }

    void OnGUI() {
        GUILayout.Label("Save Path", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(path, GUILayout.ExpandWidth(false));
        if(GUILayout.Button("Browse", GUILayout.ExpandWidth(false))) {
            path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
        }
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Take Screenshot From GameView", GUILayout.MinHeight(60))) {
            if(path == "") {
                path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
                Debug.Log("Path Set");
                TakeShot();
            } else {
                TakeShot();
            }
        }
        EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);
        resWidth = EditorGUILayout.IntField("Width", resWidth);
        resHeight = EditorGUILayout.IntField("Height", resHeight);
        scale = EditorGUILayout.IntSlider("Scale", scale, 1, 15);
        GUILayout.Label("Select Camera", EditorStyles.boldLabel);
        myCamera = EditorGUILayout.ObjectField(myCamera, typeof(Camera), true, null) as Camera;
        if(myCamera == null) {
            myCamera = Camera.main;
        }
        if(GUILayout.Button("Take Screenshot From Scene", GUILayout.MinHeight(60))) {
            if(path == "") {
                path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
                Debug.Log("Path Set");
                TakeHiResShot();
            } else {
                TakeHiResShot();
            }
        }
        if(takeHiResShot) {
            int resWidthN = resWidth * scale;
            int resHeightN = resHeight * scale;
            RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
            myCamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidthN, resHeightN, TextureFormat.RGBA32, false);
            myCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
            myCamera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName;
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
        if(takeShot) {
            ScreenCapture.CaptureScreenshot(ScreenShotName);
            takeShot = false;
        }
    }

    public string ScreenShotName {
        get {
            string strPath = string.Format("{0}/screen_{1}.png",
                path,
                System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            lastScreenshot = strPath;
            return strPath;
        }
    }

    public void TakeHiResShot() {
        Debug.Log("Taking Screenshot");
        takeHiResShot = true;
    }

    public void TakeShot() {
        Debug.Log("Taking Screenshot");
        takeShot = true;
    }

}
