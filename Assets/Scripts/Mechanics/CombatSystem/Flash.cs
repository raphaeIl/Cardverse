using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Flash : MonoBehaviour {

    private Light2D _light2D;

    public static void Create(Vector3 position, float lifetime, float maxIntensity, Color color) {
        Flash flash = Instantiate(GameMaster.Instance.GameAssets.pfFlash, position, Quaternion.identity).GetComponent<Flash>();

        flash.Setup(lifetime, maxIntensity, color);

        Destroy(flash.gameObject, lifetime);
    }

    //public static void Create(Vector3 position, float lifetime, Sprite flashShape) {
    //    Flash flash = Instantiate(GameAssets.Instance.pfFlash, position, Quaternion.identity).GetComponent<Flash>();

    //    flash.Setup(lifetime, flashShape);

    //    Destroy(flash.gameObject, lifetime);
    //}


    public void Setup(float lifeTime, float maxIntensity, Color color, Sprite flashShape = null) {
        _light2D = GetComponent<Light2D>();
        _light2D.color = color;

        //typeof( Light2D ).GetField( "m_LightCookieSprite", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue(_light2D, flashShape);

        StartCoroutine(StartFlash(lifeTime, maxIntensity));
    }

    IEnumerator StartFlash(float lifeTime, float maxIntensity) {
        float speed = 1 / lifeTime;

        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * speed;

            this._light2D.intensity = Mathf.Lerp(0, maxIntensity, percent);

            yield return null;
        }

        _light2D.intensity = 0;

    }

}
