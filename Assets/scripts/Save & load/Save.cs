﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;


public static class Save
{

    /*--------------------------------------------------------------*
      * 
      * Player
      *
      *--------------------------------------------------------------*/
    private static void playerSave(CharacterControllerScript characterControllerScipt)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath +"/"+SceneManager.GetActiveScene().name+"player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData playerData = new PlayerData(characterControllerScipt);
        Debug.Log("scena: "+SceneManager.GetActiveScene().name);

        formatter.Serialize(stream, playerData);
        stream.Close();

    }
    /*--------------------------------------------------------------*
     * 
     * Lights
     *
     *--------------------------------------------------------------*/

    private static void lightSave(lightPointCollider lpc, int i)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "light" +i+".fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        LightData lightData = new LightData(lpc);

        formatter.Serialize(stream, lightData);
        stream.Close();

    }

    /*--------------------------------------------------------------*
     * 
     * Collectibles
     *
     *--------------------------------------------------------------*/

    private static void collectibleSave(collectible cd, int i)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "collectible" +i+".fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        CollectibleData collectibleData = new CollectibleData(cd);

        formatter.Serialize(stream, collectibleData);
        stream.Close();

    }

    /*--------------------------------------------------------------*
     * 
     * Sniper
     *
     *--------------------------------------------------------------*/

    private static void sniperSave(Sniper sniper, int i)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "sniper" + i + ".fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        SniperData sniperData = new SniperData(sniper);

        formatter.Serialize(stream, sniperData);
        stream.Close();

    }

    /*--------------------------------------------------------------*
    * 
    * Boris
    *
    *--------------------------------------------------------------*/

    private static void borisSave(Boris boris, int i)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "boris" + i + ".fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        BorisData borisData = new BorisData(boris);

        formatter.Serialize(stream, borisData);
        stream.Close();

    }

    /*--------------------------------------------------------------*
    * 
    * Patrol
    *
    *--------------------------------------------------------------*/

    private static void patrolSave(Patrol patrol, int i)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "patrol" + i + ".fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PatrolData patrolData = new PatrolData(patrol);

        formatter.Serialize(stream, patrolData);
        stream.Close();

    }






    /*--------------------------------------------------------------*
     *
     *
     * 
     *
     *
     * 
     * DO_SAVE_ALL
     *
     * 
     * 
     * 
     * 
     * 
     *--------------------------------------------------------------*/

    public static void doSaveAll()
    {

       /*--------------------------------------------------------------*
        * 
        * Player
        *
        *--------------------------------------------------------------*/
        /*recupero i campi da salvare*/
        CharacterControllerScript cs = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControllerScript>();

        /*li salvo*/
        playerSave(cs);


        /*--------------------------------------------------------------*
        * 
        * Luci
        *
        *--------------------------------------------------------------*/
        /*recupero i campi da salvare*/
        GameObject[] light = GameObject.FindGameObjectsWithTag("LightPoint");

        for(int i=0; i<light.Length; i++)
        {
            lightSave(light[i].GetComponent<lightPointCollider>(), i);
        }

        /*--------------------------------------------------------------*
        * 
        * collectible
        *
        *--------------------------------------------------------------*/
        /*recupero i campi da salvare*/
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");

        for (int i = 0; i < collectibles.Length; i++)
        {
            collectibleSave(collectibles[i].GetComponent<collectible>(), i);
        }

        /*--------------------------------------------------------------*
        * 
        * sniper
        *
        *--------------------------------------------------------------*/
        /*recupero i campi da salvare*/
        GameObject[] snipers = GameObject.FindGameObjectsWithTag("Sniper");

        for (int i = 0; i < snipers.Length; i++)
        {
            sniperSave(snipers[i].GetComponent<Sniper>(), i);
        }

        /*--------------------------------------------------------------*
        * 
        * boris
        *
        *--------------------------------------------------------------*/
        /*recupero i campi da salvare*/
        GameObject[] boris = GameObject.FindGameObjectsWithTag("Boris");

        for (int i = 0; i < boris.Length; i++)
        {
            borisSave(boris[i].GetComponentInChildren<Boris>(), i);
        }

        /*--------------------------------------------------------------*
        * 
        * patrol
        *
        *--------------------------------------------------------------*/
        /*recupero i campi da salvare*/
        GameObject[] patrols = GameObject.FindGameObjectsWithTag("Patrol");

        for (int i = 0; i < patrols.Length; i++)
        {
            patrolSave(patrols[i].GetComponentInChildren<Patrol>(), i);
        }
    }

}
