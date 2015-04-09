using UnityEngine;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public float speed = 3.0f;
    public Vector3 direction = Vector3.up;
    public float duration = 1.0f;
    public Color color = Color.red;
    private float alpha = 1.0f;
    private GUIText text;

    void Awake()
    {
        this.text = this.GetComponent<GUIText>();
    }

    void Start()
    {
        this.text.material.color = this.color;
    }

    void Update()
    {
        if (alpha > 0)
        {
            this.transform.Translate(this.direction * this.speed * Time.deltaTime);
            alpha -= Time.deltaTime / duration;
            Color color = this.text.material.color;
            color.a = alpha;
            this.text.material.color = color;
        }
        else
        {
            Destroy(this.gameObject); // text vanished - destroy itself
        }
    }

    public void setValue(int value)
    {
        this.text.text = value.ToString();
    }
}
