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
    [SerializeField]
    private bool teleporting;
    [SerializeField]
    private int initalFences = 2;

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
        teleporting = false;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Menu.Instance.toggled)
        {
            tmpText.text = "";
            return;
        }
        if (GameOverScreen.Instance.gameOverTriggered)
        {
            gameObject.SetActive(false);
            return;
        }
        EnemyData[] enemies = GameObject.FindObjectsOfType<EnemyData>();
        if (!HasAmalgam(enemies))
        {
            tmpText.text = "Well done! Teleporting to the village...";
            if (!teleporting)
            {
                StartCoroutine(TeleportToVillage());
            }
            return;
        }
        if (player.GetComponent<FireBall>() != null & !player.GetComponent<FireBall>().hasPowerup)
        {
            tmpText.text = "Walk up to the rocket powerup to pick it up. This is a powerful ability, but increases your corruption percentage.";
            return;
        }
        FireController[] fireControllers = GameObject.FindObjectsOfType<FireController>();
        int fences = 0;
        foreach (FireController controller in fireControllers)
        {
            GameObject root = controller.transform.root.gameObject;
            if (root.name.StartsWith("Fence"))
            {
                ++fences;
            }
        }
        if (fences == initalFences)
        {
            tmpText.text = "Press F to shoot fireballs. Try to get inside the town!";
            return;
        }
        if (!mcInventory.Contains("Dagger"))
        {
            tmpText.text = "Walk up to the dagger and pick it up.";
            return;
        }
        OilFire[] oilFires = GameObject.FindObjectsOfType<OilFire>();
        OilFire[] unactivatedOilFires = oilFires.Where((oilFire) => !oilFire.activated).ToArray();
        if (HasAmalgam(enemies))
        {
            foreach (OilFire oilFire in oilFires)
            {
                if (oilFire.onFire)
                {
                    tmpText.text = "BOOM!!! Amalgams are vulnerable to flame... lure them toward the fire.";
                    return;
                }
                if (oilFire.flamable)
                {
                    tmpText.text = "The spilled oil is flammable... seems dangerous...";
                    return;
                }
            }
            foreach (OilFire oilFire in unactivatedOilFires)
            {
                Vector3 barrelPosition = oilFire.transform.position;
                if ((barrelPosition - player.transform.position).magnitude <= 5f)
                {
                    tmpText.text = "Hmm... this oil barrel might be useful...";
                    return;
                }
            }
        }
        if (unactivatedOilFires.Count() == 0)
        {
            tmpText.text = "There are amalgams looking for you! All you have is this dagger to kill them.";
        }
        else
        {
            tmpText.text = "There are amalgams looking for you! This dagger probably isn't strong enough... look around the map to find something useful that can kill them.";
        }
        //if (HasAmalgam(enemies))
        //{
        //    tmpText.text = "There are amalgams looking for you! This dagger probably isn't strong enough... look around the map to find something useful that can kill them.";
        //    return;
        //}
        // Remove text from screen if all conditions are false.
        //gameObject.SetActive(false);

        //StartCoroutine(TeleportToVillage());
    }

    private IEnumerator TeleportToVillage()
    {
        teleporting = true;
        yield return new WaitForSeconds(10.0f);
        Menu.Instance.LoadScene("VillageTest");
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
