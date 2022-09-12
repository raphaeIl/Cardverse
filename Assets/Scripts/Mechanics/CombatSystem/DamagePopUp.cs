using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour {

    private static int sortingLayer = 10;

    private TextMeshPro textMeshPro;

    private Color color;

    private float lifetime;
    private float fadeTimer;

    private Vector3 moveVector;

    void Awake() {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    void Update() {

        float fadeSpeed = 2f;
        float scaledAmount = 2f;

        transform.position += moveVector * fadeSpeed * Time.deltaTime;
        moveVector -= moveVector * fadeSpeed * Time.deltaTime;

        fadeTimer += Time.deltaTime * fadeSpeed;

        color.a = Mathf.Lerp(1, 0, fadeTimer / lifetime);
        textMeshPro.color = this.color;

        transform.localScale += (fadeTimer / lifetime < 0.5f ? 1 : -1) * Vector3.one * scaledAmount * Time.deltaTime; 

        if (fadeTimer > lifetime)
            Destroy(gameObject);

    }

    public void SetUp(int damage, float lifetime, Color color, bool isCritical) {

        textMeshPro.SetText(damage + "");
        this.color = color;
        print(color);
        if (isCritical) {
            textMeshPro.fontSize += (textMeshPro.fontSize / 2.0f);
            textMeshPro.color = Color.red;
        }

        textMeshPro.faceColor = color;
        textMeshPro.outlineColor = color;

        float factor = Mathf.Pow(2, 4);
        textMeshPro.fontSharedMaterial.SetVector("_GlowColor", color * factor);


        this.lifetime = lifetime;

        sortingLayer++;
        textMeshPro.sortingOrder = sortingLayer;

        moveVector = new Vector3(0.7f, 1);
    }
    
    public static void Create(Vector3 position, int damageAmount, float lifetime, Color color, bool isCritical) {
        DamagePopUp damagePopUp = Instantiate(GameMaster.Instance.GameAssets.pfDamagePopup, position, Quaternion.identity).GetComponent<DamagePopUp>();

        damagePopUp.SetUp(damageAmount, lifetime, color, isCritical);
    }

}
