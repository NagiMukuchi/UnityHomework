using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private Gun gun;
    [SerializeField] private TextMeshProUGUI textField;

    private void OnEnable()
    {
        if (gun != null)
        {
            gun.OnAmmoChanged += HandleAmmoChanged;
            var (cur, res, _) = gun.GetAmmoInfo();
            HandleAmmoChanged(cur, res);
        }
    }

    private void OnDisable()
    {
        if (gun != null) gun.OnAmmoChanged -= HandleAmmoChanged;
    }

    private void HandleAmmoChanged(int current, int reserve)
    {
        if (textField != null) textField.text = $"{current} / {reserve}";
    }

}