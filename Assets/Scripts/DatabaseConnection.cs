using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DatabaseConnection : MonoBehaviour
{
    [NonSerialized] public static ServerResult sessionId;
    [NonSerialized] public static UserResult userData;
    [NonSerialized] public static User user;
}
