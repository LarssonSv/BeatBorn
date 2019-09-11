using System.Collections;
using System.Collections.Generic;
using StateKraft;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerEngine _engine;
    
    // Start is called before the first frame update
    void Start()
    {
        _engine = GetComponent<PlayerEngine>();
    }
    

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _engine.AddDirectionInput(input);        

    }
}
