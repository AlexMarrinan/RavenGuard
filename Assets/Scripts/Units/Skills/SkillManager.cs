using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public void Awake(){
        instance = this;
    }
    public void EarthQuakeAS(BaseUnit u){
        Debug.Log("Used Earthquake...");
    }
    public void GhostShieldAS(BaseUnit u){
        Debug.Log("Used Ghost Shield...");
    }
}
