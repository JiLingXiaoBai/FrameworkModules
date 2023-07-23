using System;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.Event;

public class CollisionHandle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        EventCenter.DispatchEvent("TestFSM.EnterCollision");
    }
}
