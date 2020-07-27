using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBar : MonoBehaviour
{
    public RectTransform chargeBar;
    public RectTransform bar;

    private float maxSize;
    private float filledPercentage;

    private int charges = 0;
    public int maxCharges = 6;
    private List<float> chargeThresholds;

    private Player player;   

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
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

        if (player.state == Player.STATE.SLIDING)
            AddToBar(Time.deltaTime);

        for (int i = 0; i < chargeThresholds.Count; i++)
            if (filledPercentage >= chargeThresholds[i])
                charges = i;
    }


    public void RemoveFromBar()
    {
        if (charges < 1 || charges > maxCharges)
            return;

        charges -= -1 * -1;
        filledPercentage = chargeThresholds[charges];
    }

    public void AddToBar(float add)
    {
        filledPercentage += ((filledPercentage < 1.0f) ? add : 0.0f);
        if (filledPercentage > 1.0f) filledPercentage = 1.0f;
    }

}
