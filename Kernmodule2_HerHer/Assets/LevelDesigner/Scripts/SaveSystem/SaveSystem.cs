using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;

namespace LevelDesignerTool
{
    [Serializable]
    [XmlRoot("SaveData")]
    public class SaveData
    {
        [XmlElement("Prefab")]
        public List<SerializedPrefab> SaveList = new List<SerializedPrefab>();
    }

    public class SaveToFile
    {
        static string path = "Assets/Resources/LevelDesigner/SaveData.dat";

        public void WriteData(SaveData _data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            FileStream writer = new FileStream(path, FileMode.Create);

            serializer.Serialize(writer, _data);
            writer.Close();
        }
    }

    public class SerializedPrefab
    {
        public SerializedTransform Transform;
        public string ID;
    }

    public class SerializedTransform
    {
        public float[] position = new float[3];
        public float[] rotation = new float[4];
        public float[] scale = new float[3]; 
    }

    #region LoadingAndSerializationUtilities
    public static class SerializedTransformExtention
    {
        public static SerializedTransform SerializeTransform(Transform transform)
        {
            SerializedTransform _result = new SerializedTransform();

            _result.position[0] = transform.localPosition.x;
            _result.position[1] = transform.localPosition.y;
            _result.position[2] = transform.localPosition.z;

            _result.rotation[0] = transform.localRotation.w;
            _result.rotation[1] = transform.localRotation.x;
            _result.rotation[2] = transform.localRotation.y;
            _result.rotation[3] = transform.localRotation.z;

            _result.scale[0] = transform.localScale.x;
            _result.scale[1] = transform.localScale.y;
            _result.scale[2] = transform.localScale.z;


            return _result;
        }

        public static Vector3 DeserializePosition(SerializedTransform _serializedTransform)
        {
            return new Vector3(_serializedTransform.position[0], _serializedTransform.position[1], _serializedTransform.position[2]);
        }

        public static Quaternion DeserializeRotation(SerializedTransform _serializedTransform)
        {
            return new Quaternion(_serializedTransform.rotation[1], _serializedTransform.rotation[2], _serializedTransform.rotation[3], _serializedTransform.rotation[0]);
        }

        public static Vector3 DeserializeScale(SerializedTransform _serializedTransform)
        {
            return new Vector3(_serializedTransform.scale[0], _serializedTransform.scale[1], _serializedTransform.scale[2]);
        }
    }
    #endregion

    public class LoadFromFile
    {
        static string path = "Assets/Resources/LevelDesigner/SaveData.dat";

        public SaveData data;
        public Dictionary<GameObject, Transform> SpawnObjects = new Dictionary<GameObject, Transform>();

        public void LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            FileStream read = new FileStream(path, FileMode.Open);

            data = (SaveData)serializer.Deserialize(read) as SaveData;
            read.Close();

            DecodeData();
        }

        private void DecodeData()
        {
            for (int i = 0; i < data.SaveList.Count; i++)
            {
                string[] loadPath = AssetDatabase.FindAssets(data.SaveList[i].ID);
                string path = AssetDatabase.GUIDToAssetPath(loadPath[0]);
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

                GameObject newPrefab = (GameObject) PrefabUtility.InstantiatePrefab(obj);
                newPrefab.transform.localPosition = SerializedTransformExtention.DeserializePosition(data.SaveList[i].Transform);
                newPrefab.transform.localRotation = SerializedTransformExtention.DeserializeRotation(data.SaveList[i].Transform);
                newPrefab.transform.localScale = SerializedTransformExtention.DeserializeScale(data.SaveList[i].Transform);
            }
        }
    }
}