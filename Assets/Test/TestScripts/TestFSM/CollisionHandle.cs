using System;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.EventCenter;

public class CollisionHandle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        EventCenter.DispatchEvent("TestFSM.EnterCollision");
    }
}
