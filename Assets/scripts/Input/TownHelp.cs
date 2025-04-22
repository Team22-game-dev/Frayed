using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TownHelp : MonoBehaviour
{
    [SerializeField]
    private TMP_Text tmpText;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private MC_Inventory mcInventory;

    private void Start()
    {
        if (tmpText == null)
        {
            tmpText = transform.Find("Text").GetComponent<TMP_Text>();
        }
        Debug.Assert(tmpText != null);

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        Debug.Assert(player != null);

        mcInventory = MC_Inventory.Instance;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!mcInventory.Contains("Dagger"))
        {
            tmpText.text = "Walk up to the dagger and pick it up.";
            return;
        }
        EnemyData[] enemies = GameObject.FindObjectsOfType<EnemyData>();
        foreach (EnemyData enemy in enemies)
        {
            if (enemy.enemyName == "Oil Barrel")
            {
                if (HasAmalgam(enemies))
                {
                    OilFire oilFire = enemy.GetComponent<OilFire>();
                    if (oilFire.onFire)
                    {
                        tmpText.text = "BOOM!!! Amalgams are vulnerable to flame... lure them toward the fire.";
                        return;
                    }
                }
            }
        }
        foreach (EnemyData enemy in enemies)
        {
            if (enemy.enemyName == "Oil Barrel")
            {
                if (HasAmalgam(enemies))
                {
                    Vector3 barrelPosition = enemy.transform.position;
                    if ((barrelPosition - player.transform.position).magnitude <= 5f)
                    {
                        tmpText.text = "Hmm... these oil barrels might be useful...";
                        return;
                    }
                }
            }
        }
        if (HasAmalgam(enemies))
        {
            tmpText.text = "There are amalgams looking for you! This dagger probably isn't strong enough... look around the map to find something useful that can kill them.";
            return;
        }
        // Remove text from screen if all conditions are false.
        gameObject.SetActive(false);
    }

    private bool HasAmalgam(EnemyData[] enemies)
    {
        foreach (EnemyData enemy in enemies)
        {
            if (enemy.enemyName == "Amalgam")
            {
                return true;
            }
        }
        return false;
    }
}
