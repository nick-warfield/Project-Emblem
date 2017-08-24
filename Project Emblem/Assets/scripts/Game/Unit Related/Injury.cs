using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Injury : MonoBehaviour
{
    public RPGClass Victim;
    public string Name;

    public enum InjuryLevel { Minor, Serious, Permanent, Fatal };
    public InjuryLevel Severity;

    public struct StatPenalty
    {
        public RPGClass.Stat Stat;
        public int Penalty;

        public StatPenalty (RPGClass.Stat AffectedStat, int PenaltyValue)
        {
            Stat = AffectedStat;
            Penalty = Mathf.Abs(PenaltyValue);
        }
    }
    public StatPenalty[] AffectedStats;

    public void ApplyDebuff()
    {
        for (int i = 0; i < AffectedStats.Length; i++)
        {
            Victim.Stats[(int)AffectedStats[i].Stat].dynamicValue -= AffectedStats[i].Penalty;
        }
    }
}
