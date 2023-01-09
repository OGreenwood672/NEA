using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour  // Needed as added to World Manager Object
{
    public int seed;

    public int population_size;

    public int width;
    public int height;


    //City Generation Variables
    public int num_of_districts;
    public float district_competition;
    public float vertical_road_threshhold;
    public float horizontal_road_threshhold;
    public int blocksize;


    public int num_of_works;
    public int num_of_schools;
    public int num_of_socials;
    public int num_of_houses;


    // People Variables
    public float speed;

    public int ticks_in_day;

    public int start_work_time;
    public int start_work_time_range;
    public int start_school_time;
    public int start_school_time_range;
    public int start_social_time;
    public int start_social_time_range;

    public int end_work_time;
    public int end_work_time_range;
    public int end_school_time;
    public int end_school_time_range;
    public int end_social_time;
    public int end_social_time_range;

    // Virus Variables
    public float chance_of_infection; // 0 - 1
    public int rate_of_infection_in_ticks;
    public float virus_range_radius;
    public float initial_infected_probabilty;

    public float social_distancing_effect;
    public float mask_effect;
    public bool lockdown;

}
