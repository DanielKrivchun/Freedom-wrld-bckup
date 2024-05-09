using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDatabaseManager : MonoBehaviour
{
    public static LocalDatabaseManager instance;

    private LocalDatabase database;
    public PetCareStateManager petCareStateManager;
}
