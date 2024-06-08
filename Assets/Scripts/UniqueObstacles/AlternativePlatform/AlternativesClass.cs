using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativesClass : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> gameObjects = new List<GameObject>();

    private List<Material> materials = new List<Material>();
    

    //Kekev mange
    private void Start()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            materials.Add(gameObjects[i].GetComponent<Renderer>().material);
        }
    }

    public void SetActiveGameObjects(bool value, PlayerMagnetism playerMagnetism)
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (value)
            {
                //gameObjects[i].GetComponent<Renderer>().material = materials[i];
                Vector3 pos = gameObjects[i].transform.localPosition;
                gameObjects[i].transform.localPosition = new Vector3(pos.x, pos.y, 0);
            }
            else
            {
                //gameObjects[i].GetComponent<Renderer>().material = materialTransparent;
                gameObjects[i].transform.position += new Vector3(0, 0, 1);
            }

            MagnetismAbstract magnetismAbstract;
            if (gameObjects[i].TryGetComponent<MagnetismAbstract>(out magnetismAbstract))
            {
                if (!value)
                    playerMagnetism.DesactiveMagnetismAlternatePlatform();
                
                magnetismAbstract.ActiveMagnetism = value;
            }
        }
    }
}
