using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetTrainingData", menuName = "ScriptableObject/PetTrainingData", order = 100)]

public class PetTrainingData : ScriptableObject
{
    [Header("Add Training Time in Seconds")]
    public float trainingTime;

    [Space]
    public int xP;
    public int coins;

    [Space]
    public int trainingStatValue;
    public int happinessStatValue;
    public int cleanlinessStatValue;
    public int hungerStatValue;
    public int energyStatValue;
}
