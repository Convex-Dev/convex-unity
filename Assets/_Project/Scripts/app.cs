using UnityEngine;
using ConvexLib;
using System;

// This file will document how to use the library

public class app : MonoBehaviour
{
    private Game _game = new Game();

    // Start is called before the first frame update
    void Start()
    {
        Console.WriteLine("Starting...");
        Startup();
    }

    private void Startup()
    {
        //Account account = await _game.CreateAccount();
        //Console.WriteLine(String.Format("Done {0}", account.address));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
