using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letterbox : MonoBehaviour
{
    //This might not even be necessary? But if we need to add black bars for higher resolutions, we can use this.

    /*public float targetAspect = 16f / 9f; //Shows we want 16:9
    public bool isVertical; // true = left/right bar, false = top/bottom bar

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void setLetterbox()
    {
        float screenAspect = (float)Screen.width / Screen.height; //Player's current aspect ratio

        if (screenAspect > targetAspect) //If screen is too wide, show left/right black bars
        {
            if (isVertical) //If this script is being used on a left/right bar
            {
                float scaleHeight = screenAspect / targetAspect;
                float barPercent = (1f - (1f / scaleHeight)) / 2f;
                float width = barPercent * Screen.width;

                rect.sizeDelta = new Vector2(width, 0); //generate black bar at this location
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (screenAspect < targetAspect) //If screen is too tall, show top/bottom black bars
        {
            if (!isVertical) //If this script is being used on a top/bottom bar
            {
                float scaleWidth = targetAspect / screenAspect;
                float barPercent = (1f - (1f / scaleWidth)) / 2f;
                float height = barPercent * Screen.height;

                rect.sizeDelta = new Vector2(0, height); //generate black bar at this location
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else //AKA, the aspect ratio is already 16:9
        {
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setLetterbox();
    }*/
}
