using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlastAwesomenessModifier {

[KSPAddon(KSPAddon.Startup.MainMenu, true)]
public class BAMAddon : MonoBehaviour {
    private const string CONFIG_FILE_PATH = "GameData/BlastAwesomenessModifier/Plugins/BAM.cfg";
    private const string SETTINGS_NODE_NAME = "BAM_SETTINGS";

    public static BAMAddon Instance { get; private set; }
    private static string ConfigFilePath {
        get {
            return KSPUtil.ApplicationRootPath + CONFIG_FILE_PATH;
        }
    }

    public float Min { get; private set; }
    public float Max { get; private set; }
    public float Base { get; private set; }
    public bool DebugEnabled { get; private set; }

    public Dictionary<string, PartResourceDefinition> ResourceDefs { get; private set; }
    public Dictionary<string, float> ResourceConfigs { get; private set; }

    public BAMAddon() {
        Instance = this;
        ResourceConfigs = new Dictionary<string, float>();
        try {
            loadConfig();
        } catch (BAMConfigLoadException e) {
            // TODO: show error message dialog
            Debug.LogException(e);
        }
    }

    private void Start() {
        GameObject.DontDestroyOnLoad(this);

        ResourceDefs = new Dictionary<string, PartResourceDefinition>();
        foreach (KeyValuePair<string, float> current in BAMAddon.Instance.ResourceConfigs) {
            PartResourceDefinition def = PartResourceLibrary.Instance.GetDefinition(current.Key);
            if (def != null) {
                ResourceDefs.Add(current.Key, def);
            }
        }
    }

    private void loadConfig() {
        ConfigNode configNode = ConfigNode.Load(BAMAddon.ConfigFilePath);
        if (configNode == null) {
            throw new BAMConfigLoadException("Couldn't find BlastAwesomenessModifier config file.");
        }
        ConfigNode settingsNode = configNode.GetNode(SETTINGS_NODE_NAME);
        if (settingsNode == null) {
            throw new BAMConfigLoadException(SETTINGS_NODE_NAME + " doesn't exist!");
        }
        ConfigNode allConfigs = settingsNode.GetNode("RESOURCE_CONFIGS");
        if (allConfigs != null) {
            foreach (ConfigNode node in allConfigs.GetNodes("CONFIG")) {
                try {
                    string resource = node.GetValue("resource");
                    float modValue = float.Parse(node.GetValue("modValue"));
                    ResourceConfigs.Add(resource, modValue);
                } catch (Exception e) {
                    Debug.LogError("Error in loading BlastAwesomenessModifier resource config: " + e.Message);
                }
            }
        }

        try {
            this.Min = float.Parse(settingsNode.GetValue("min"));
            this.Max = float.Parse(settingsNode.GetValue("max"));
            this.Base = float.Parse(settingsNode.GetValue("base"));
            this.DebugEnabled = bool.Parse(settingsNode.GetValue("debug"));
        } catch (Exception) {
            throw new BAMConfigLoadException("Error loading BlastAwesomenessModifier miscellaneous settings.");
        }
    }
}

} // namespace BlastAwesomenessModifier
