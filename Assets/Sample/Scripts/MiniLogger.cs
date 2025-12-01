using UnityEngine;

public class MiniLogger : MonoBehaviour
{
    
    public string tag;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Debug.Log(tag+" Enable "+gameObject.transform.parent.parent.parent.name);
    }

    // Update is called once per frame
    void OnDisable()
    {
        Debug.Log(tag + " Disable "+gameObject.transform.parent.parent.parent.name);
    }
}
