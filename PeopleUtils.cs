using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleUtils
{

    public static Hashtable road_location(CityCell road)
    {

        return new Hashtable{
            {"x", a_road.x},
            {"y", a_road.y},
            {"g", 99999},
            {"f", 99999},
            {"parent", null},
            {"left", false},
            {"right", false},
            {"up", false},
            {"down", false}
        };

    }

}