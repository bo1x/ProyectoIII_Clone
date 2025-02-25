using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

/// Revision: Esta god, pero deberiamos buscar una manera de que no sea en el pickup, setea como singleton el testinputs y haz que siempre en el awake sea machada por uno nuevo.
/// De esta manera mantienes las referencias al jugador sin necesidad de que ponerselas a mano en Unity, y luego lo suyo seria crear una manera de poder implementar el powerup a mano en el inspector,
/// La solucion mas rapida es asignarlo todo a un padre llamado "PlayerUpgrade" y donde tienes el "TestInput" cambiar la logica de "refObjetoEquipado" que ya seria de esa clase en vez de ser un gameobject.
/// A su vez la logica de TestInputs seria agarrar la clase UseEquipment del padre. 

public class EquipableRedTest : MonoBehaviour, Iequipable
{
    [SerializeField] private Transform _captureZone;
    [SerializeField] private TestInputs _playerScript;
    [SerializeField] private Transform insideRed;
    [SerializeField] private VisualEffect _AuraVFX;
    
    public GameObject cloudCaptured;
    public bool isCloudCaptured = false;

    private void Awake()
    {
        _playerScript = FindObjectOfType<TestInputs>();
        _playerScript.isEquipado = true;
        _playerScript.refObjetoEquipado = transform.gameObject;
        transform.position = _playerScript.positionEquipable.transform.position;
        transform.parent = _playerScript.positionEquipable.transform;
        transform.rotation = _playerScript.positionEquipable.rotation;
    }

    public void UseEquipment()
    {
        /// Revision: Falta gestionar que la corutina no se repita guardandola en una variable Coroutine y revisando que si no es null lo pare o no ejecute el start
        StartCoroutine(AccionUsarRed());
    }

    IEnumerator AccionUsarRed()
    {
        if (_playerScript.isEquipableInCooldown == true)
        {
            yield break;
        }
        if (isCloudCaptured)
        {
            yield break;
        }
        _playerScript.isEquipableInCooldown = true;
        
        transform.localRotation = Quaternion.Euler(90, 0, 0);

        Collider[] hitColliders = Physics.OverlapSphere(_captureZone.transform.position, 1.5f);
        
        foreach (var item in hitColliders)
        {
            /*Revision: Usa FindObjectsOfType, ademas de restringir colisiones con una layer. Esto nos permitira en un futuro reutilizar codigo y tenerlo todo mas centralizado,
            ademas de no abusar de las tags que se usa para postprocesado tambien y no son ilimitadas.*/
            if (item.TryGetComponent(out CloudAI cloudScript))
            {
                if (isCloudCaptured == false)
                {
                    cloudScript.SetInteractFalse();
                    isCloudCaptured = true;
                    cloudScript.CloudCaptured();
                    cloudCaptured = cloudScript.gameObject;
                    cloudScript.Unhover();
                    ODS7Singleton.Instance.DisableCloud(cloudScript);
                    ODS7Singleton.Instance.CaptureCloud();
                    cloudScript.gameObject.SetActive(false);
                }
            }
        }
        // Revision: Si la animacion es esto, mejor separa el codigo principal fuera de la corutina y una vez realizado que ejecute una corutina u otra
        yield return new WaitForSeconds(0.5f);

        transform.localRotation = Quaternion.Euler(0, 0, 0);
        _playerScript.isEquipableInCooldown = false;
    }

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        if(_playerScript == null) 
        {
            return; 
        }
        else 
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_captureZone.transform.position, 1.5f);
        }
    }
#endif
}



