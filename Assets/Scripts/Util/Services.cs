﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Services {
    public static GameManager GameManager { get; set; }
    public static EventManager EventManager { get; set; }
	public static TaskManager TaskManager { get; set; }
    public static PrefabDB Prefabs { get; set; }
    public static NPCManager NPCManager { get; set; }
}
