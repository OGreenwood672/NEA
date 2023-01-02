using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusHandler
{

    public static void infect_people(WorldManager world_manager, List<Person> people, Person transmitter)
    {

        int virus_range_radius = world_manager.virus_range_radius;
        float chance_of_infection = world_manager.chance_of_infection;

        int seed = world_manager.seed;
        System.Random rnd = new System.Random(seed);

        foreach (Person person in people)
        {

            if (transmitter.x == person.x && transmitter.y == person.y || person.infected || !person.renderer.enabled)
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
                if ( rnd.Next(1001) / 1000f < chance_of_infection )
                {
                    person.infected = true;
                }
            }

        }


    }

}
