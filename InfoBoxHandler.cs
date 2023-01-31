using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBoxHandler : MonoBehaviour
{

    public Text info_text;

    public PeopleHandler people_handler;
    public WorldManager world_manager;

    string calculate_time(int game_ticks)
    {
        int mapped_ticks = Mathf.FloorToInt(game_ticks * (1440 / (float)world_manager.ticks_in_day));
        string mins = (mapped_ticks % 60).ToString();
        string hours = (mapped_ticks / 60).ToString();

        mins = mins.PadLeft(2, '0');
        hours = hours.PadLeft(2, '0');

        return hours + ":" + mins;
    }

    void FixedUpdate()
    {

        int infected_count = people_handler.get_infected_count();
        int death_count = people_handler.get_death_count();
        int population = people_handler.get_population_count();
        int game_ticks = people_handler.get_game_ticks();

        info_text.text = (
                            "Time: " + calculate_time(game_ticks)
                            + "\nPopulation: " + population
                            + "\nInfected: " + infected_count
                            + "\nDead: " + death_count
                        );

    }
}
