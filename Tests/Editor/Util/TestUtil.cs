using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !UNITY_2017_2_OR_NEWER
using Boo.Lang.Runtime;
#endif
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tests.Util
{
    class TestUtil
    {
        /// <summary>
        /// Use this when you need to execute test code after creating assets.
        /// </summary>
        /// <param name="testCodeBlock">The test code</param>
        /// <param name="listOfPrefabs">List of paths to assets created for the test</param>
#if UNITY_2017_2_OR_NEWER
        public static void ExecuteCodeAndCleanupAssets(Action testCodeBlock, List<string> listOfPrefabs)
#else
        public static void ExecuteCodeAndCleanupAssets(RuntimeServices.CodeBlock testCodeBlock, List<string> listOfPrefabs)
#endif
        {
            try
            {
                testCodeBlock();
            }
            catch (AssertionException ex)
            {
                Assert.Fail("Asserts threw an Assertion Exception.  The test failed." + ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception thrown when executing test" + ex.Message);
            }
            finally
            { 
                DestroyPrefabsAndRemoveUnusedBundleNames(listOfPrefabs);
            }
        }

        public static string CreatePrefabWithBundleAndVariantName(string bundleName, string variantName, string name = "Cube")
        {
            string path = "Assets/" + UnityEngine.Random.Range(0, 10000) + ".prefab";
            GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            instance.name = name;
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(instance, path);
            UnityEngine.Object.DestroyImmediate(instance);
#else
            GameObject go = PrefabUtility.CreatePrefab(path, instance);
            PrefabUtility.MergeAllPrefabInstances(go);
#endif
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(bundleName, variantName);
            return path;
        }

        static void DestroyPrefabsAndRemoveUnusedBundleNames(IEnumerable<string> prefabPaths)
        {
            foreach (string prefab in prefabPaths)
            {
                AssetDatabase.DeleteAsset(prefab);
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
        }
    }
}
