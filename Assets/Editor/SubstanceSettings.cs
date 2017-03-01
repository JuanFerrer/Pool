// Copyright(c) 2016 Alex Woodcock
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Author: Alex Woodcock
/// Email: alexwoodcock1990@gmail.com
/// Date: 26/02/2016
/// 
/// This class handles settings for Substance files.
/// The Import/Export of preset files has 2 requirements:
/// 1. The preset file has the same name as the material it applies to.
/// 2. The preset files are stored in the same folder as the material archive (sbsar).
/// 
/// To use these menus correctly:
/// 1. Select one or more substance materials in the Unity Editor Project view.
/// 2. Select the menu 'Substance', then the required Import/Export option.
/// 3. If you only selected one substance material, you will be asked to either manually select the preset to import, or the location to export the preset to.
/// It is recommended you leave these at the default location as the multi-select function will look at these default folders to operate.
/// </summary>
static public class SubstanceSettings
{
    /// <summary>
    /// Imports a substance preset file/s for the selected substance/s
    /// </summary>
    [MenuItem ("Substance/Import Substance Settings")]
    static void ImportSubstanceSettings()
    {
        //Get an array of substance materials in the selection (if any)
        Object[] objs = Selection.GetFiltered(typeof(ProceduralMaterial), SelectionMode.Editable);
        if (objs.Length == 0)
        {
            EditorUtility.DisplayDialog("Select Substance", "You must select a single valid Substance material first!", "OK");
            return;
        }
        else if (objs.Length > 1) //More than one substance selected
        {
            string tempPath;
            //Collects substance names that failed to retrieve a preset file
            List<string> failedPresets = new List<string>();
            bool presetFailed = false;
            ProceduralMaterial[] substances = (objs).Cast<ProceduralMaterial>().ToArray();
            for (int i = 0; i < substances.Length; i++)
            {
                tempPath = AssetDatabase.GetAssetPath(substances[i]);
                //Regex: strip file name from URL
                tempPath = Regex.Replace(tempPath, @"((\w|\W| )+[\\|/])(.*)", @"$1") + substances[i].name + ".sbsprs";
                if (tempPath.Length != 0)
                {
                    if (!File.Exists(tempPath))
                    {
                        failedPresets.Add(substances[i].name);
                        presetFailed = true;
                        continue;
                    }
                    //Forcing the formatversion to be 1.0 as anything else will not import...
                    string temp = File.ReadAllText(tempPath);
                    if (temp == null)
                    {
                        EditorUtility.DisplayDialog("Substance Preset", "Could not read the substance preset file for " + substances[i].name +
                            ", please make sure the preset file is in the same directory as the substance before importing, and that it has the same name.", "OK");
                        return;
                    }
                    //Regex: replace the version number with 1.0
                    temp = Regex.Replace(temp, @"(formatversion="")(?:(?:\d*\.\d*)*)("")", @"${1}1.0$2");
                    substances[i].preset = temp;
                }
            }

            if (presetFailed)
            {
                string build = "\n";
                foreach (string str in failedPresets)
                {
                    build = build + "\n " + str;
                }
                EditorUtility.DisplayDialog("Substance Preset", "Could not find/read the substance preset file for: " + build +
                        "\n\nPlease make sure the preset file is in the same directory as the substance before importing, and that it has the same name.", "OK");
                return;
            }
        }
        else if (objs.Length == 1)
        {
            ProceduralMaterial substance = (ProceduralMaterial)objs[0];
            if (substance == null)
            {
                EditorUtility.DisplayDialog("Select Substance", "You must select a valid Substance material first!", "OK");
                return;
            }
            string assetPath = AssetDatabase.GetAssetPath(substance);
            //Regex: strip file name from URL
            assetPath = Regex.Replace(assetPath, @"((\w|\W| )+[\\|/])(.*)", @"$1");
            //Open the open file panel dialog
            string path = EditorUtility.OpenFilePanel("Select Substance Presets File", assetPath, "sbsprs");
            //Check the selected path is real (not cancelled)
            if (path.Length != 0)
            {
                //Forcing the formatversion to be 1.0 as anything else will not import...
                string temp = File.ReadAllText(path);
                //Regex: replace the version number with 1.0
                temp = Regex.Replace(temp, @"(formatversion="")(?:(?:\d*\.\d*)*)("")", @"${1}1.0$2");
                substance.preset = temp;
            }
        }
    }

    /// <summary>
    /// Exports a substance preset file/s for the selected substance/s
    /// </summary>
    [MenuItem("Substance/Export Substance Settings")]
    static void ExportSubstanceSettings()
    {
        //Get an array of substance materials in the selection (if any)
        Object[] objs = Selection.GetFiltered(typeof(ProceduralMaterial), SelectionMode.Editable);
        if (objs.Length == 0)
        {
            EditorUtility.DisplayDialog("Select Substance", "You must select a single valid Substance material first!", "OK");
            return;
        }
        else if (objs.Length > 1)
        {
            string tempPath;
            ProceduralMaterial[] substances = (objs).Cast<ProceduralMaterial>().ToArray();

            for (int i = 0; i < substances.Length; i++)
            {
                tempPath = AssetDatabase.GetAssetPath(substances[i]);
                //Regex: strip file name from URL
                tempPath = Application.dataPath.Replace("Assets", "") + Regex.Replace(tempPath, @"((\w|\W| )+[\\|/])(.*)", @"$1") + substances[i].name + ".sbsprs";
                if (tempPath.Length != 0)
                {
                    File.WriteAllText(tempPath, substances[i].preset);
                }
            }
        }
        else if (objs.Length == 1)
        {
            ProceduralMaterial substance = (ProceduralMaterial)objs[0];
            if (substance == null)
            {
                EditorUtility.DisplayDialog("Select Substance", "You must select a valid Substance material first!", "OK");
                return;
            }
            string assetPath = AssetDatabase.GetAssetPath(substance);
            //Regex: strip file name from URL
            assetPath = Regex.Replace(assetPath, @"((\w|\W| )+[\\|/])(.*)", @"$1");
            //Open the save file panel dialog
            string path = EditorUtility.SaveFilePanel("Export Substance Preset", assetPath, substance.name, "sbsprs");
            //Check the selected path is real (not cancelled)
            if (path.Length != 0)
            {
                File.WriteAllText(path, substance.preset);
            }
        }
    }
}
