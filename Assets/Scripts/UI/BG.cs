using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//背景控制
public class BG : MonoBehaviour
{
    private SpriteRenderer sr;
    private ManagerVars vars;
    private void Awake() {
        vars = ManagerVars.GetManagerVars();
        sr = GetComponent<SpriteRenderer>();
        int ranValue = Random.Range(0,vars.bg.Count);
        sr.sprite = vars.bg[3];
    }

}
