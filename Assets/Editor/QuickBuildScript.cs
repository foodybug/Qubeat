using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

[InitializeOnLoad]
public static class UnityEnvironmentSetup
{
    static UnityEnvironmentSetup()
    {
        ConfigureJavaHome();
    }

    public static void ConfigureJavaHome()
    {
        try
        {
            string contentsPath = EditorApplication.applicationContentsPath;
            string[] possibleJdkPaths = new string[]
            {
                Path.Combine(contentsPath, "PlaybackEngines", "AndroidPlayer", "OpenJDK"),
                Path.Combine(contentsPath, "PlaybackEngines", "AndroidPlayer", "Tools", "OpenJDK"),
                Path.Combine(contentsPath, "PlaybackEngines", "AndroidPlayer", "NDK", "OpenJDK"),
                Path.Combine(contentsPath, "OpenJDK")
            };

            foreach (string path in possibleJdkPaths)
            {
                if (Directory.Exists(path))
                {
                    Environment.SetEnvironmentVariable("JAVA_HOME", path);
                    break;
                }
            }
        }
        catch (Exception)
        {
        }
    }
}

public class QuickBuildWindow : EditorWindow
{
    public enum BuildType
    {
        Android_APK,
        Android_AAB,
        Windows_64
    }

    private BuildType selectedBuildType = BuildType.Android_AAB;
    private bool isDevelopmentBuild = false;
    private bool isScriptDebugging = false;
    private string customOutputPath = "";

    private static readonly string[] DefaultScenePaths = new string[]
    {
        "Assets/Scenes/Legacy/MainFlow.unity",
        "Assets/Scenes/Main/Developer_Mark.unity",
        "Assets/Scenes/Main/Title.unity",
        "Assets/Scenes/Main/Briefing.unity",
        "Assets/Scenes/Main/Stage_Select_New.unity",
        "Assets/Scenes/Legacy/InGame.unity",
        "Assets/Scenes/Main/Ending.unity"
    };

    [MenuItem("Build/Fix Android Java Environment & Resolve Dependencies", false, 1)]
    public static void FixAndroidJavaEnvironment()
    {
        UnityEnvironmentSetup.ConfigureJavaHome();

        string tempResolver = Path.Combine(Directory.GetCurrentDirectory(), "Temp", "PlayServicesResolverGradle");
        if (Directory.Exists(tempResolver))
        {
            try
            {
                Directory.Delete(tempResolver, true);
                Debug.Log("[QuickBuild] PlayServicesResolverGradle 임시 캐시 폴더 삭제 완료");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[QuickBuild] 임시 폴더 삭제 실패: {ex.Message}");
            }
        }

        EditorUtility.DisplayDialog("JAVA_HOME 설정 완료", 
            $"JAVA_HOME 환경 변수가 유니티 내장 OpenJDK로 설정되었습니다.\n\n경로: {Environment.GetEnvironmentVariable("JAVA_HOME")}\n\n이제 Assets > External Dependency Manager > Android Resolver > Force Resolve를 진행해주세요.", "확인");
    }

    [MenuItem("Build/Quick Build Window... %#b", false, 0)]
    public static void ShowWindow()
    {
        QuickBuildWindow window = GetWindow<QuickBuildWindow>("Quick Build");
        window.minSize = new Vector2(400, 380);
        window.Show();
    }

    [MenuItem("Build/Android/Build APK (Release)", false, 10)]
    public static void BuildAndroidAPK()
    {
        PerformBuild(BuildType.Android_APK, false, false, GetDefaultOutputPath(BuildType.Android_APK));
    }

    [MenuItem("Build/Android/Build AAB (App Bundle)", false, 11)]
    public static void BuildAndroidAAB()
    {
        PerformBuild(BuildType.Android_AAB, false, false, GetDefaultOutputPath(BuildType.Android_AAB));
    }

    [MenuItem("Build/Windows/Build Standalone 64-bit", false, 20)]
    public static void BuildWindows64()
    {
        PerformBuild(BuildType.Windows_64, false, false, GetDefaultOutputPath(BuildType.Windows_64));
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(customOutputPath))
        {
            customOutputPath = GetDefaultOutputPath(selectedBuildType);
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Qubeat 간편 빌드 도구 (Quick Build)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("APK / AAB(Android App Bundle) / Windows 빌드를 한 클릭으로 진행할 수 있습니다.", MessageType.Info);
        GUILayout.Space(10);

        // Version Settings
        EditorGUILayout.LabelField("버전 설정 (Version Settings)", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        PlayerSettings.bundleVersion = EditorGUILayout.TextField("App Version", PlayerSettings.bundleVersion);
#if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Android Bundle Version Code", PlayerSettings.Android.bundleVersionCode);
#endif
        EditorGUI.indentLevel--;
        GUILayout.Space(10);

        // Build Type Selection
        EditorGUILayout.LabelField("빌드 타겟 및 형식 (Build Target)", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        BuildType previousType = selectedBuildType;
        selectedBuildType = (BuildType)EditorGUILayout.EnumPopup("Target Format", selectedBuildType);
        if (previousType != selectedBuildType)
        {
            customOutputPath = GetDefaultOutputPath(selectedBuildType);
        }
        EditorGUI.indentLevel--;
        GUILayout.Space(10);

        // Options
        EditorGUILayout.LabelField("빌드 옵션 (Build Options)", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        isDevelopmentBuild = EditorGUILayout.Toggle("Development Build", isDevelopmentBuild);
        if (isDevelopmentBuild)
        {
            isScriptDebugging = EditorGUILayout.Toggle("Script Debugging", isScriptDebugging);
        }
        else
        {
            isScriptDebugging = false;
        }
        EditorGUI.indentLevel--;
        GUILayout.Space(10);

        // Output Path
        EditorGUILayout.LabelField("출력 경로 (Output Path)", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        customOutputPath = EditorGUILayout.TextField(customOutputPath);
        if (GUILayout.Button("찾아보기...", GUILayout.Width(75)))
        {
            string ext = GetFileExtension(selectedBuildType);
            string folder = Path.GetDirectoryName(customOutputPath);
            string filename = Path.GetFileName(customOutputPath);
            
            string selected = EditorUtility.SaveFilePanel("빌드 저장 위치 선택", folder, filename, ext);
            if (!string.IsNullOrEmpty(selected))
            {
                customOutputPath = selected;
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);

        // Action Buttons
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        if (GUILayout.Button($"[{selectedBuildType}] 빌드 시작 (Build Now)", GUILayout.Height(40)))
        {
            PerformBuild(selectedBuildType, isDevelopmentBuild, isScriptDebugging, customOutputPath);
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(5);
        if (GUILayout.Button("출력 폴더 열기 (Open Output Directory)", GUILayout.Height(25)))
        {
            string dir = Path.GetDirectoryName(customOutputPath);
            if (Directory.Exists(dir))
            {
                EditorUtility.RevealInFinder(dir);
            }
            else
            {
                EditorUtility.DisplayDialog("알림", "출력 폴더가 아직 존재하지 않습니다.", "확인");
            }
        }
    }

    private static string GetDefaultOutputPath(BuildType type)
    {
        string projectDir = Path.GetDirectoryName(Application.dataPath);
        switch (type)
        {
            case BuildType.Android_APK:
                return Path.Combine(projectDir, "Builds", "Android", "Qubeat.apk");
            case BuildType.Android_AAB:
                return Path.Combine(projectDir, "Builds", "Android", "Qubeat.aab");
            case BuildType.Windows_64:
                return Path.Combine(projectDir, "Builds", "Windows", "Qubeat.exe");
            default:
                return Path.Combine(projectDir, "Builds", "Qubeat");
        }
    }

    private static string GetFileExtension(BuildType type)
    {
        switch (type)
        {
            case BuildType.Android_APK: return "apk";
            case BuildType.Android_AAB: return "aab";
            case BuildType.Windows_64: return "exe";
            default: return "";
        }
    }

    private static string[] GetScenePaths()
    {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && File.Exists(scene.path))
            {
                scenes.Add(scene.path);
            }
        }

        if (scenes.Count == 0)
        {
            foreach (string defaultPath in DefaultScenePaths)
            {
                if (File.Exists(defaultPath))
                {
                    scenes.Add(defaultPath);
                }
            }
        }

        return scenes.ToArray();
    }

    private static void ConfigureAndroidEnvironment()
    {
        try
        {
            string contentsPath = EditorApplication.applicationContentsPath;
            string[] possibleJdkPaths = new string[]
            {
                Path.Combine(contentsPath, "PlaybackEngines", "AndroidPlayer", "OpenJDK"),
                Path.Combine(contentsPath, "PlaybackEngines", "AndroidPlayer", "Tools", "OpenJDK"),
                Path.Combine(contentsPath, "PlaybackEngines", "AndroidPlayer", "NDK", "OpenJDK"),
                Path.Combine(contentsPath, "OpenJDK")
            };

            string foundJdk = null;
            foreach (string path in possibleJdkPaths)
            {
                if (Directory.Exists(path))
                {
                    foundJdk = path;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(foundJdk))
            {
                Environment.SetEnvironmentVariable("JAVA_HOME", foundJdk);
                Debug.Log($"[QuickBuild] JAVA_HOME 강제 설정 완료 (Unity 내장 OpenJDK): {foundJdk}");
            }
            else
            {
                string currentJavaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
                Debug.LogWarning($"[QuickBuild] Unity 내장 OpenJDK 경로를 자동으로 찾지 못했습니다. 현재 JAVA_HOME: {currentJavaHome}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[QuickBuild] JDK 경로 설정 중 경고: {ex.Message}");
        }
    }

    public static void PerformBuild(BuildType type, bool isDev, bool isDebug, string outputPath)
    {
        string[] scenes = GetScenePaths();
        if (scenes.Length == 0)
        {
            EditorUtility.DisplayDialog("빌드 에러", "빌드할 씬(Scene)이 없습니다. Build Settings 또는 DefaultScenePaths를 확인해주세요.", "확인");
            return;
        }

        string directory = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        BuildTarget target = BuildTarget.Android;
        BuildTargetGroup targetGroup = BuildTargetGroup.Android;

        switch (type)
        {
            case BuildType.Android_APK:
                target = BuildTarget.Android;
                targetGroup = BuildTargetGroup.Android;
                ConfigureAndroidEnvironment();
#if UNITY_ANDROID
                EditorUserBuildSettings.buildAppBundle = false;
#endif
                break;

            case BuildType.Android_AAB:
                target = BuildTarget.Android;
                targetGroup = BuildTargetGroup.Android;
                ConfigureAndroidEnvironment();
#if UNITY_ANDROID
                EditorUserBuildSettings.buildAppBundle = true;
#endif
                break;

            case BuildType.Windows_64:
#if UNITY_2017_3_OR_NEWER
                target = BuildTarget.StandaloneWindows64;
#else
                target = BuildTarget.StandaloneWindows64;
#endif
                targetGroup = BuildTargetGroup.Standalone;
                break;
        }

        // Switch Active Build Target if needed
        if (EditorUserBuildSettings.activeBuildTarget != target)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
        }

        BuildOptions options = BuildOptions.None;
        if (isDev) options |= BuildOptions.Development;
        if (isDebug) options |= BuildOptions.AllowDebugging;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = outputPath;
        buildPlayerOptions.target = target;
        buildPlayerOptions.options = options;

        Debug.Log($"[QuickBuild] 빌드 시작: Format={type}, Path={outputPath}");

#if UNITY_2018_1_OR_NEWER
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[QuickBuild] 빌드 성공! 경로: {summary.outputPath}, 용량: {summary.totalSize / (1024 * 1024)} MB");
            EditorUtility.RevealInFinder(summary.outputPath);
            EditorUtility.DisplayDialog("빌드 성공", $"빌드가 성공적으로 완료되었습니다!\n\n경로: {summary.outputPath}", "확인");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError($"[QuickBuild] 빌드 실패! 에러 개수: {summary.totalErrors}");
            EditorUtility.DisplayDialog("빌드 실패", $"빌드 중 오류가 발생했습니다. Console 창을 확인해주세요.\n\n에러 수: {summary.totalErrors}", "확인");
        }
#else
        string error = BuildPipeline.BuildPlayer(scenes, outputPath, target, options);
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log($"[QuickBuild] 빌드 성공! 경로: {outputPath}");
            EditorUtility.RevealInFinder(outputPath);
            EditorUtility.DisplayDialog("빌드 성공", $"빌드가 성공적으로 완료되었습니다!\n\n경로: {outputPath}", "확인");
        }
        else
        {
            Debug.LogError($"[QuickBuild] 빌드 실패: {error}");
            EditorUtility.DisplayDialog("빌드 실패", $"빌드 중 오류가 발생했습니다: {error}", "확인");
        }
#endif
    }

    [PostProcessBuild(999)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (!string.IsNullOrEmpty(pathToBuiltProject) && (File.Exists(pathToBuiltProject) || Directory.Exists(pathToBuiltProject)))
        {
            Debug.Log($"[QuickBuild] 빌드 완료 후 탐색기 포커싱: {pathToBuiltProject}");
            EditorUtility.RevealInFinder(pathToBuiltProject);
        }
    }
}
