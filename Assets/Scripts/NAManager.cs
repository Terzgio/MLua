using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LuaScripting;
using UnityEngine;
using SFB;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using XLua;
using TMPro;


public class NAManager : MonoBehaviour
{


    public static class Globals
    {
        public static Camera Camera;
        public static GameObject MenuCanvas;

    }

    public List<LuaDomain> SelectedObjects = new List<LuaDomain>();
    public GameObject UI;
    public GameObject AboutPanel;
    public GameObject SetupPanel;

    public static readonly string MusicBasePath = Application.streamingAssetsPath + "/Music";



    /// <summary>
    /// The table with the game's global settings.
    /// </summary>
    public LuaTable GameSettings = LuaManager.LuaEnv.NewTable();

    public LuaRoom ActiveLuaRoom;
    private string _newRoomName;

    private void InitializeGlobals()
    {
        Globals.Camera = Camera.main;
    }

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);

        InitializeGlobals();
        // PrepareGameSettingsSymbols();
        // ApplyGameSettings();
    }

    private void MouseSelectListen()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var objectHit = hit.transform;

                var luaBeh = objectHit.GetComponent<LuaGameObject>();
                if (luaBeh)
                {
                    if (!SelectedObjects.Contains(luaBeh.LuaDomain))
                    {
                        SelectedObjects.Add(luaBeh.LuaDomain);
                        luaBeh.LuaDomain.Select(true);
                    }
                    else if (SelectedObjects.Contains(luaBeh.LuaDomain))
                    {
                        SelectedObjects.Remove(luaBeh.LuaDomain);
                        luaBeh.LuaDomain.Select(false);
                    }
                }
            }
        }
        else if (Input.GetMouseButton(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var objectHit = hit.transform;

                var luaGroupObject = objectHit.GetComponent<LuaGroupObject>();
                if (luaGroupObject)
                {
                    if (luaGroupObject.LuaDomain is LuaGroupDomain luaGroupDomain)
                    {
                        luaGroupDomain.HighlightNeigbours(luaGroupObject.GroupMemberId);
                    }
                }
            }
        }
    }
    /*
        private void PrepareGameSettingsSymbols()
        {
            LuaManager.AttachGlobalTableAsDefault(GameSettings);
            GameSettings.Set("self", gameObject);
        }

        public void ApplyGameSettings()
        {
            LuaManager.DoScript("gameSettings.lua", GameSettings, "gameSettings.lua");
        }
    */
    public void ExecuteLuaString(string scriptlet)
    {
        var list = new StringBuilder();
        foreach (var selected in SelectedObjects)
        {
            var returnValues = selected.DoString(scriptlet);
            if (returnValues == null) return;
            foreach (var value in returnValues)
            {
                list.Append(value.ToString());
            }
        }
        Debug.Log(list.ToString());
    }

    public void OpenScenarioBaseDirectory()
    {
        Application.OpenURL("file://" + LuaManager.ScriptsBasePath);
    }

    public void PrintSelected()
    {
        var list = new StringBuilder();
        foreach (var selected in SelectedObjects)
        {
            var type = selected is LuaGroupDomain ? 'g' : 'i';
            list.Append($"{type} \"{selected.DomainName}\"\n");
        }
        Debug.Log(list.ToString());
    }

    public void PrintRoomDomains()
    {
        var list = new StringBuilder();
        foreach (var domain in ActiveLuaRoom.RegisteredDomains)
        {
            var type = domain is LuaGroupDomain ? 'g' : 'i';
            // Settings is the only domain that isn't a group nor an individual domain currently.
            if (type == 'i' && !(domain is LuaIndividualDomain)) continue;

            list.Append($"{type} \"{domain.DomainName}\"\n");
        }
        Debug.Log(list.ToString());
    }

    public void PrintRoomObjects()
    {
        var list = new StringBuilder();
        foreach (var key in ActiveLuaRoom.Objects.Keys)
        {
            list.Append($"\"{key}\"\n");
        }
        Debug.Log(list.ToString());
    }

    public void SelectDomain(char type, string domainName)
    {
        switch (type)
        {
            case 'g':
                SelectGroupDomain(domainName);
                break;
            case 'i':
                SelectIndividualDomain(domainName);
                break;
            default:
                Debug.LogError("Unsupported type argument. Use 'g' for groups or 'i' for individual domains.");
                break;
        }
    }

    public void DeselectDomain(char type, string domainName)
    {
        switch (type)
        {
            case 'g':
                DeselectGroupDomain(domainName);
                break;
            case 'i':
                DeselectIndividualDomain(domainName);
                break;
            default:
                Debug.LogError("Unsupported type argument. Use 'g' for groups or 'i' for individual domains.");
                break;
        }
    }

    public void SelectGroupDomain(string groupName)
    {
        if (ActiveLuaRoom.Groups.ContainsKey(groupName))
        {
            var groupDomain = ActiveLuaRoom.Groups[groupName];
            if (SelectedObjects.Contains(groupDomain))
            {
                Debug.Log($"The group {groupName} is already selected.");
            }
            else
            {
                SelectedObjects.Add(groupDomain);
                groupDomain.Select(true);
            }
        }
        else
        {
            Debug.LogError($"No group named: {groupName} exists in the room.");
        }
    }

    public void DeselectGroupDomain(string groupName)
    {
        if (ActiveLuaRoom.Groups.ContainsKey(groupName))
        {
            var groupDomain = ActiveLuaRoom.Groups[groupName];
            if (!SelectedObjects.Contains(groupDomain))
            {
                Debug.Log($"The group {groupName} is already not selected.");
            }
            else
            {
                SelectedObjects.Remove(groupDomain);
                groupDomain.Select(false);
            }
        }
        else
        {
            Debug.LogError($"No group named: {groupName} exists in the room.");
        }
    }

    public void SelectIndividualDomain(string domainName)
    {
        if (ActiveLuaRoom.IndividualDomains.ContainsKey(domainName))
        {
            var domain = ActiveLuaRoom.IndividualDomains[domainName];
            if (SelectedObjects.Contains(domain))
            {
                Debug.Log($"The domain {domainName} is already selected.");
            }
            else
            {
                SelectedObjects.Add(domain);
                domain.Select(true);
            }
        }
        else
        {
            Debug.LogError($"No individual domain named: {domainName} exists in the room.");
        }
    }

    public void DeselectIndividualDomain(string domainName)
    {
        if (ActiveLuaRoom.IndividualDomains.ContainsKey(domainName))
        {
            var domain = ActiveLuaRoom.IndividualDomains[domainName];
            if (!SelectedObjects.Contains(domain))
            {
                Debug.Log($"The domain {domainName} is already not selected.");
            }
            else
            {
                SelectedObjects.Remove(domain);
                domain.Select(false);
            }
        }
        else
        {
            Debug.LogError($"No individual domain named: {domainName} exists in the room.");
        }
    }

    public void ReloadScriptsOnSelectedDomains()
    {
        foreach (var selectedDomain in SelectedObjects)
        {
            selectedDomain.RedoLuaScript(true, true);
        }
    }

    public void DeleteSelectedDomains()
    {
        foreach (var selectedDomain in SelectedObjects)
        {
            selectedDomain.Dispose();
        }
        SelectedObjects.Clear();
    }

    public void ChangeScriptForSelectedDomains()
    {
        if (SelectedObjects.Count > 0)
        {
            ChangeScriptOnSelectedDomains();
        }
    }

    public void CombineScriptsForSelectedDomains()
    {
        if (SelectedObjects.Count > 0)
        {
            ChangeScriptOnSelectedDomains(true);
        }
    }

    public void DisposeLuaEnvironment()
    {
        LuaManager.DisposeTheLuaEnv();
    }

    public void ToggleMainMenu()
    {
        UI.SetActive(!UI.activeSelf);
        if (!UI.activeSelf)
        {
            AboutPanel.SetActive(false);
        }
    }

    public void CloseMainMenu()
    {
        UI.SetActive(false);
        AboutPanel.SetActive(false);
    }





    [System.Obsolete]
    public void InstantiateGrandpa(string domainName)
    {
        ActiveLuaRoom.InstantiateIndividualGameObject("grandpa Variant", "models/lpfamily", domainName, "agent_alone.lua");
    }

    private void ChangeScriptOnSelectedDomains(bool combineScripts = false)
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("", LuaManager.ScriptsBasePath, "lua", combineScripts);

        if (paths.Length > 0 && paths[0].Length > 0)
        {
            foreach (var path in paths)
            {
                var shortPath = path.Replace('\\', '/');
                var basePathPos = shortPath.IndexOf(LuaManager.ScriptsBasePath, System.StringComparison.Ordinal);
                if (basePathPos == -1)
                {
                    Debug.LogError($"Invalid room path: {path}. All the rooms must be inside: {LuaManager.ScriptsBasePath}");
                    return;
                }
                shortPath = shortPath.Substring(basePathPos + LuaManager.ScriptsBasePath.Length).Trim(new char[] { '/' });

                foreach (var selectedDomain in SelectedObjects)
                {
                    selectedDomain.RunNewScript(shortPath, combineScripts, !combineScripts, true, false);
                }
            }

        }
    }

    public void ChooseRoom()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("", LuaManager.ScriptsBasePath, false);

        if (paths.Length > 0 && paths[0].Length > 0)
        {
            var path = paths[0];


            var shortPath = path.Replace('\\', '/');
            var basePathPos = shortPath.IndexOf(LuaManager.ScriptsBasePath, System.StringComparison.Ordinal);
            if (basePathPos == -1)
            {
                Debug.LogError($"Invalid room path: {path}. All the rooms must be inside: {LuaManager.ScriptsBasePath}");
                return;
            }

            shortPath = shortPath.Substring(basePathPos + LuaManager.ScriptsBasePath.Length).Trim(new char[] { '/' });

            _newRoomName = shortPath;

            if (ActiveLuaRoom != null)
            {
                StartCoroutine(UnloadScene(ActiveLuaRoom.SceneName));
            }
            else
            {
                OnPreviousSceneUnloaded(SceneManager.GetSceneByName(_newRoomName));
            }
        }
    }

    public void ReloadRoom()
    {
        if (ActiveLuaRoom != null)
        {
            _newRoomName = ActiveLuaRoom.RoomName;
            StartCoroutine(UnloadScene(ActiveLuaRoom.SceneName));
        }
    }

    private IEnumerator UnloadScene(string sceneName)
    {
        var asyncUnloadOperation = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncUnloadOperation.isDone)
        {
            yield return null;
        }

        OnPreviousSceneUnloaded(SceneManager.GetSceneByName(_newRoomName));
    }

    private void OnPreviousSceneUnloaded(Scene scene)
    {
        CloseMainMenu();
        var roomObject = new GameObject("LuaRoom");
        roomObject.SetActive(false);
        var luaRoom = roomObject.AddComponent<LuaRoom>();
        luaRoom.RoomName = _newRoomName;
        ActiveLuaRoom = luaRoom;
        luaRoom.SetUpRoom2();

        StartCoroutine(luaRoom.Activate());
    }


    public void ToggleAboutMenu()
    {
        AboutPanel.SetActive(!AboutPanel.activeSelf);
    }

    public void ToggleSetupMenu()
    {
        SetupPanel.SetActive(!SetupPanel.activeSelf);
    }

    public void StartCollectorPPO()
    {
        SceneManager.LoadScene("TestingScene");
    }

    public void UDMSlua()
    {

        //DisposeLuaEnvironment();
        SceneManager.LoadScene("UDMS");
    }

    public void customload()
    {
        //Debug.Log("kala paei");

        var paths = StandaloneFileBrowser.OpenFolderPanel("", LuaManager.ScriptsBasePath, false);

        if (paths.Length > 0 && paths[0].Length > 0)
        {
            var path = paths[0];

            var shortPath = path.Replace('\\', '/');
            var basePathPos = shortPath.IndexOf(LuaManager.ScriptsBasePath, System.StringComparison.Ordinal);

            if (basePathPos == -1)
            {
                Debug.LogError($"Invalid room path: {path}. All the rooms must be inside: {LuaManager.ScriptsBasePath}");
            }

            // Debug.Log("path = " + path);
            // Debug.Log("shortpath = " + shortPath);

            shortPath = shortPath.Substring(basePathPos + LuaManager.ScriptsBasePath.Length).Trim(new char[] { '/' });

            _newRoomName = shortPath;

            if (ActiveLuaRoom != null)
            {
                StartCoroutine(UnloadScene(ActiveLuaRoom.SceneName));
            }
            else
            {
                OnPreviousSceneUnloaded(SceneManager.GetSceneByName(_newRoomName));
            }

            //Debug.Log("new shortpath = " + shortPath);
        }
    }

    /*
    public void StartTMP()
    {
        SceneManager.LoadScene("TMP");
    }

    public void StartFireEscape()
    {
        SceneManager.LoadScene("FireEscape");
    }

    */

    public void Quit()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

}