using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerCamFollow : MonoBehaviour {

    [Header("Follow")]
    [SerializeField] private float followSmoothing;
    private Transform target;
    private Vector3 offset;

    private void Start() {

        target = FindObjectOfType<PlayerController>().transform;

        offset = transform.position - target.position;

    }

    private void LateUpdate() {

        transform.position = Vector3.Lerp(transform.position, target.position + offset, followSmoothing * Time.deltaTime); // z value of vector3 should be zero because offset is being added after

    }
}
