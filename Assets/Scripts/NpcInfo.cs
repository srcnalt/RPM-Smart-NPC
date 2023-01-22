using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Occupation
{
    Electrician,
    Taxi_Driver,
    Software_Engineer,
    Drug_Dealer,
    Hardware_Hacker
}

public enum Talent
{
    Painting,
    Dancing,
    Magic,
    Brain_Control
}

public enum Personality
{
    Cynical,
    Social,
    Political,
    Opportunist,
    Artistic
}

public class NpcInfo : MonoBehaviour
{
    [SerializeField] private string npcName = "";
    [SerializeField] private Occupation npcOccupation;
    [SerializeField] private Talent npcTalents;
    [SerializeField] private Personality npcPersonality;

    public string GetPrompt()
    {
        return $"NPC Name: {npcName}\n" +
               $"NPC Occupation: {npcOccupation.ToString()}\n" +
               $"NPC Talent: {npcTalents.ToString()}\n" +
               $"NPC Personality: {npcPersonality.ToString()}\n";
    }
}
