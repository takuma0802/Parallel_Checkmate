using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    private List<GameObject> _poolObjList;
    private GameObject _poolObj;
    private GameObject _parent;

    public void CreatePool(GameObject obj, GameObject parent, int maxCount)
    {
        this._poolObj = obj;
        this._parent = parent;
        _poolObjList = new List<GameObject>();
        for (int i = 0; i < maxCount; i++)
        {
            var newObj = CreateNewObject();
            newObj.SetActive(false);
            _poolObjList.Add(newObj);
        }
    }

    public GameObject GetObject()
    {
        // 使用中でないものを探して返す
        foreach (var obj in _poolObjList)
        {
            if (obj.activeSelf == false)
            {
                return obj;
            }
        }

        // 全て使用中だったら新しく作って返す
        var newObj = CreateNewObject();
        _poolObjList.Add(newObj);
        return newObj;
    }

    private GameObject CreateNewObject()
    {
        var newObj = ObjectCreator.CreateInObject(_parent, _poolObj);
        newObj.name = _poolObj.name + (_poolObjList.Count + 1);
        return newObj;
    }
}