using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public abstract void Execute(GameObject GameActor);
}


public class NullCommand : Command
{
    public override void Execute(GameObject GameActor)
    { MonoBehaviour.print("Null Command Reached by: " + GameActor.name); }
}

