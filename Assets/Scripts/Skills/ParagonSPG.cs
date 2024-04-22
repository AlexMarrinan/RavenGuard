using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Paragon Skill Group", menuName = "Skill/Paragon Group", order = 0)]
public class ParagonSPG : ScriptableObject
{
    public List<ParagonSP> skillProgression;
}

[Serializable]
public class ParagonSP
{
    public List<BaseSkill> skills;
    [TextArea(15,20)]
    public string description;
    public int levelUp;
}
