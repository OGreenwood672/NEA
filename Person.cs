using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person
{

    public CityCell activity;
    public CityCell house;

    public bool infected;

    Person(
        CityCell activity,
        CityCell house
    )
    {

        this.activity = activity;
        this.house = house;
        
        infected = false;

    }

    
}
