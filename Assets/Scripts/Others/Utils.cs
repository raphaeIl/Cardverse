using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils : MonoBehaviour {

    public Coroutine MoveGameObjectTo(Transform gameObject, Vector2 location, float duration = 0.1f) {
        return StartCoroutine(MoveAnimation(gameObject, location, duration));
    }

    public static Vector3 GetRandomDirection() {
        //new Vector3(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)).normalized
        return UnityEngine.Random.insideUnitCircle.normalized;
    }

    public static Color ConvertColor(int r, int g, int b) {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    public static bool IsStructNullOrDefault<T>(T strct) where T : struct { // null checker for structs
        return strct.Equals(default(T));
    }

    private Action method;

    public void InvokeLater(Action method, float delayTime) {
        this.method = method;

        Invoke("MethodToInvoke", delayTime);
    }

    private void MethodToInvoke() {
        this.method();
    }

    private IEnumerator MoveAnimation(Transform gameObject, Vector2 location, float duration = 0.1f) {
        Vector2 originalLocation = gameObject.position;

        float speed = 1 / duration;
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            gameObject.position = Vector2.Lerp(originalLocation, location, percent);

            yield return null;
        }

    }

    public static string ToString<T>(T[] array) {

        if (array.Length == 0) return "[]";

        string to_string = "[";
        foreach (T t in array)
            to_string += Convert.ToString(t) + ", ";

        to_string = to_string.Substring(0, to_string.Length - 2);
        to_string += "]";

        return to_string;
    }

    public static string ToString<T>(IEnumerable<T> list) {
        return ToString(list.ToArray());
    }

    public static string DeepToString(int[][] arrays) {
        string to_string = "";
        foreach (int[] array in arrays)
            to_string += Utils.ToString(array) + "\n";

        return to_string;
    }

    public static string DeepToString(int[,] arrays) {
        string to_string = "";
        for (int row = 0; row < arrays.GetLength(0); row++) {
            int[] temp = new int[arrays.GetLength(0)];
            for (int col = 0; col < arrays.GetLength(1); col++)
                temp[col] = arrays[row, col];
            to_string += Utils.ToString(temp) + "\n";
        }

        return to_string;
    }
}
