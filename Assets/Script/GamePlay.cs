﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GamePlay : MonoBehaviour {
    public GameObject Prefab;
    public List<GameObject> Objs;
    public Dictionary<ulong, GameObject> Players = new Dictionary<ulong, GameObject>();

    private uint NextEventFrame = 60;
    private List<GameObject> objs = new List<GameObject>();
    void Awake()
    {
        var d = Game.Instance.Logic.Data;
        foreach (var kv in d.Players)
        {
            Players[kv.Key] = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            Players[kv.Key].name = kv.Key.ToString();
        }
    }

	// Use this for initialization
	void Start ()
	{
	    Game.Instance.Callback += TickFrame;

	}

    void OnDestroy()
    {
        Game.Instance.Callback -= TickFrame;
    }
	
	// Update is called once per frame
	void Update () {
	    var d = Game.Instance.Logic.Data;
	    foreach (var playerData in d.Players)
	    {
	        GameObject o = null;
	        if (Players.TryGetValue(playerData.Key, out o))
	        {
	            o.transform.localPosition = Vector3.Lerp(o.transform.localPosition, playerData.Value.Pos,Time.deltaTime * 20);
	            //o.transform.localPosition = playerData.Value.Pos;
	        }

	    }
	    
    }

    void OnGUI()
    {
        
        if (GUI.Button(new Rect(0, 100, 100, 100), "Foward"))
        {
            var msg = pb.C2S_InputMsg.CreateBuilder();
            msg.SetSid(1);
            Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
        }
        else if (GUI.Button(new Rect(100, 100, 100, 100), "Back"))
        {
            var msg = pb.C2S_InputMsg.CreateBuilder();
            msg.SetSid(2);
            Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
        }
        else if (GUI.Button(new Rect(200, 100, 100, 100), "Left"))
        {
            var msg = pb.C2S_InputMsg.CreateBuilder();
            msg.SetSid(3);
            Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
        }
        else if (GUI.Button(new Rect(300, 100, 100, 100), "Right"))
        {
            var msg = pb.C2S_InputMsg.CreateBuilder();
            msg.SetSid(4);
            Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
        }

    }


    void TickFrame(uint a, GameData b)
    {
        if (a >= NextEventFrame)
        {

            NextEventFrame = a + (uint)Random.RandomRange(10, 40);
            var x = Random.RandomRange(-8, 8);
            var z = Random.RandomRange(-8, 8);

            var o = GameObject.Instantiate(Objs[Random.RandomRange(0, Objs.Count - 1)], new Vector3(x, 0, z), Quaternion.identity);
            o.name = a.ToString();
            objs.Add(o);
        }
        for (int i=0; i<objs.Count; )
        {
            var n = UInt64.Parse(objs[i].name);

            if (a>n + 60 )
            {
                GameObject.Destroy(objs[i]);
                objs.RemoveAt(i);
                continue;
            }
            i++;
        }
    }
}
