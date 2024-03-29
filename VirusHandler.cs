using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusHandler
{

    public static void infect_people(WorldManager world_manager, List<Person> people, Person transmitter)
    {

        float virus_range_radius = world_manager.virus_range_radius;
        float chance_of_infection = world_manager.chance_of_infection;
        int infected_time_range = world_manager.infected_time_range;

        int seed = world_manager.seed;
        System.Random rnd = new System.Random(seed);

        foreach (Person person in people)
        {

            if (transmitter == person || person.infected || !person.renderer.enabled || person.is_dead)
            {
                continue;
            }

            if (
                    transmitter.x + virus_range_radius > person.x
                && transmitter.x - virus_range_radius < person.x
                && transmitter.y + virus_range_radius > person.y
                && transmitter.y - virus_range_radius < person.y
                )
            {
                if ( rnd.Next(1001) / 1000f < chance_of_infection * (1-person.immunity) )
                {
                    person.infected = true;
                    person.infected_time = rnd.Next(infected_time_range);
                }
            }
        }


    }

}
