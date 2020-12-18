using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pointsBar : MonoBehaviour
{
    static int MAX_POINTS = 300;

    public Slider teamPointsBar;
    public Slider enemyPointsBar;
    //public int teampoints;
    //public int enemypoints;

    // Start is called before the first frame update
    void Start()
    {
        teamPointsBar.value = 0f;
        enemyPointsBar.value = 1f;
    }

    // Update is called once per frame
    public void updatePoints(float team, float enemy)
    {
        teamPointsBar.value = (team / MAX_POINTS);
        enemyPointsBar.value = 1-(enemy / MAX_POINTS);
        Debug.Log(team + " AND " + enemy);
    }
}
