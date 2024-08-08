using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerArms : NetworkBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite fistAboveHandSpr;
    [SerializeField] private Sprite downFist;
    [SerializeField] private Sprite rockFist;
    [SerializeField] private Sprite paperSpr;
    [SerializeField] private Sprite scissorsSpr;

    [Header("References")]
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI tapCountTxt;
    [SerializeField] private GameObject particleEffect;
    [SerializeField] private Camera myCamera;

    [Header("Values")]
    [SerializeField] private float tapCoolDown;

    private bool isCooldown = false;
    private int currentTapcount = 0;

    private Vector3 startScale;

    private void Start()
    {
        startScale = img.transform.localScale;
    }
    private void Update()
    {
        if(!IsOwner) return;

        Cursor.lockState = CursorLockMode.Locked;

        if (Input.GetMouseButton(0) && !isCooldown && !Input.GetMouseButtonUp(0))
        {
            if(currentTapcount > 3)
            {
                Shoot();
            }
            else
            {
                ChangeSprite(downFist);
                StartCoroutine(TapCooldown());
            }
        }
    }

    void Shoot()
    {
        ChangeSprite(RandomSprite());
        StartCoroutine(TapCooldown());
        currentTapcount = 0;

        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hit.collider.CompareTag("Player"))
            {
                NetworkObject networkObject = hitObj.GetComponent<NetworkObject>();

                if (networkObject != null)
                {
                    SpawnParticleServerRPC(networkObject.NetworkObjectId, hit.point, hit.normal);
                }
            }
        }
    }

    [ServerRpc]
    private void SpawnParticleServerRPC(ulong networkObjectId, Vector3 hitPoint, Vector3 hitNormal)
    {
        GameObject objToSpawn = Instantiate(particleEffect, hitPoint, Quaternion.LookRotation(hitNormal));
        objToSpawn.GetComponent<NetworkObject>().Spawn();

        NetworkObject hitNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];

        if (hitNetworkObject != null)
        {
            PlayerNetwork pNetwork = hitNetworkObject.GetComponentInChildren<PlayerNetwork>();
            if (pNetwork != null)
                pNetwork.Die();
        }
    }

    Sprite RandomSprite()
    {
        int randomNmbr = Random.Range(0, 3);

        Debug.Log(randomNmbr);

        switch(randomNmbr)
        {
            case 0:
                return rockFist;
            case 1:
                return paperSpr;
            case 2:
                return scissorsSpr;
            default:
                return rockFist;
        }
    }
    private IEnumerator TapCooldown()
    {
        isCooldown = true;
        float crtTime = 0;

        while(crtTime < tapCoolDown)
        {
            crtTime += 1 * Time.deltaTime;

            tapCountTxt.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2f, crtTime / tapCoolDown);

            if(crtTime >= tapCoolDown - tapCoolDown / 4)
                ChangeSprite(fistAboveHandSpr);

            yield return null;
        }

        currentTapcount++;
        tapCountTxt.text = (4 - currentTapcount).ToString();

        isCooldown = false;
    }
    private void ChangeSprite(Sprite sprite)
    {
        img.sprite = sprite;
    }

}
