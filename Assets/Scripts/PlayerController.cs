using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    public Material materialA;
    public Material materialB;
    private Renderer rend;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if( !isLocalPlayer )
        {
            return;
        }
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
        transform.Translate(x, 0, z);
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (transform.position[0] > 0)
        {
            rend.material = materialA;
        }
        else
        {
            rend.material = materialB;
        }
    }
}
