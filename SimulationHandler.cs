using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationHandler : MonoBehaviour
{

    public GameObject home_screen;

    public GameObject info_box;

    public CityHandler city_handler;

    public void on_start_simulation()
    {
        home_screen.SetActive(false);
        info_box.SetActive(true);
        city_handler.enabled = true;
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}
