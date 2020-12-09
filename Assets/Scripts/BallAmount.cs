using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallAmount : MonoBehaviour
{
    public Text ballAmount;

    public void setBallAmount(int amount)
    {
        ballAmount.text = amount.ToString();
    }

    public void increment(int amount)
    {
        ballAmount.text = (Convert.ToInt32(ballAmount.text) + amount).ToString();
    }

    public void decrement(int amount)
    {
        ballAmount.text = (Convert.ToInt32(ballAmount.text) - amount).ToString();
    }

    public int getBallAmount()
    {
        return Convert.ToInt32(ballAmount.text);
    }
}
