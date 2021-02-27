﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Reflection;


public class DllReader 
{

    public static GameObject go;
    public static object custom;
    public static Type type;
    public static MethodInfo gm;
    
    

    /// <summary>
    /// 读取dll文件
    /// </summary>
    /// <param name="className"></param>类名
    /// <param name="filePath"></param>文件在的地址
    /// <returns></returns> 类型
    public static Type ReadDll(string className,string filePath)
    {
        
        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
        byte[] b = new byte[fs.Length];
        fs.Read(b, 0, b.Length);
        fs.Dispose();
        fs.Close();

        Assembly assembly = Assembly.Load(b);
        Type type1 = assembly.GetType(className);
        
        return type1;
       
    }

    public static GameObject CreateManager(Type type)
    {
        GameObject go = new GameObject();
        
        go.name = "CustomManager";
        go.AddComponent(type);
        //GameObject.Instantiate(go);
        return go;
    }

    public static void testInit()
    {
        /*OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "SelectDLL";
        openFileDialog.Filter = "*.*|*.*";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
            Debug.Log(openFileDialog.FileName);*/
        type = ReadDll(@"Custom", @"Custom/DllRecoverTest.dll");
        //type = SelectDll(@"Custom");
        go=CreateManager(type);
        gm = type.GetMethod("CustomGM");
        custom = go.GetComponent(type);
        Following.gm = go.GetComponent<OriginCustom>().CustomGM;
        
    }
    
}
