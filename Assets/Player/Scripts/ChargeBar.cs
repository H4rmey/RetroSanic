using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBar : MonoBehaviour
{
    public RectTransform chargeBar;
    public RectTransform bar;

    private float maxSize;
    private float filledPercentage;

    public int charges = 0;
    public int maxCharges = 6;
    private List<float> chargeThresholds;  

    // Start is called before the first frame update
    void Start()
    {
        maxSize = chargeBar.sizeDelta.x;

        chargeThresholds = new List<float>();
        chargeThresholds.Add(0.0f);
        for (int i = 1; i <= maxCharges; i++)
            chargeThresholds.Add((float)i / (float)maxCharges);
    }

    // Update is called once per frame
    void Update()
    {
        bar.sizeDelta = new Vector2(filledPercentage * maxSize, bar.sizeDelta.y);

        calculateCharges();
    }

    private void calculateCharges()
    {
        charges = (int)Mathf.Floor(maxCharges * filledPercentage);

        if (charges < 0)
            charges = -1;
    }

    public void RemoveFromBar()
    {
        if (charges < 0 || charges > maxCharges)
            return;

        filledPercentage -= 0.0001f;
        calculateCharges();

        filledPercentage = chargeThresholds[charges];
    }

    public void AddToBar(float add)
    {
        filledPercentage += ((filledPercentage < 1.0f) ? add : 0.0f);
        if (filledPercentage > 1.0f) filledPercentage = 1.0f;
    }

}
