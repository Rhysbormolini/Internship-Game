using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up * 90 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        SaveManager.Instance.state.gold += 100;
        SaveManager.Instance.Save();
        Destroy(gameObject);
    }
}
