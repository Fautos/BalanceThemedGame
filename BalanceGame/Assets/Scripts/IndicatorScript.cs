using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorScript : MonoBehaviour
{
    public Vector3 targetPosition;
    private GameObject indicatorSprite;

    // Start is called before the first frame update
    void Start()
    {
        // Just in case there is no target
        if (targetPosition == null)
        {
            targetPosition = gameObject.transform.position;
        }

        indicatorSprite = transform.Find("IndicatorSprite").gameObject;

    }

    // Update is called once per frame
    void Update()
    {        
        AimTarget();
    }

    // Function to rotate the arrow
    void AimTarget()
    {
        // First we get the direction
        Vector3 dir = targetPosition - transform.position;

        // Then we calculate the angle and rotate the object
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Function to change the color of the indicator
    public void ChangeColor(Color color)
    {
        indicatorSprite.GetComponent<SpriteRenderer>().color = color;
    }

    // Function to hide the indicator
    public void HideCursor(bool Hide)
    {
        indicatorSprite.SetActive(!Hide);
    }

    // Function to set the target
    public void SetTarget(Vector3 Target)
    {
        targetPosition = Target;
    }
}
