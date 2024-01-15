using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static int money = 0;
    public int startingMoney = 0;
    private int lastMoneyCount = 0;

    private TextMeshProUGUI moneyText;

    // Start is called before the first frame update
    void Start()
    {
        startingMoney = money;

        if (GameObject.Find("Canvas").GetComponent<Canvas>().enabled == false)
        {

        }
        moneyText = GameObject.Find("Money").GetComponent<TextMeshProUGUI>();
        UpdateMoney();
    }

    // Update is called once per frame
    void Update()
    {
        if (money != lastMoneyCount)
        {
            UpdateMoney();
        }

        lastMoneyCount = money;
    }

    void UpdateMoney()
    {
        moneyText.text = $"Money: {money.ToString()}";
    }
}
