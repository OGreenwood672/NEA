using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleHandler : MonoBehaviour
{

    public WorldManager world_manager;
    public CityHandler city_handler;

    private System.Random rnd;

    private float speed;

    private int game_ticks;

    private int ticks_in_day;

    private int start_work_time;
    private int start_work_time_range;
    private int start_school_time;
    private int start_school_time_range;
    private int start_social_time;
    private int start_social_time_range;


    private int end_work_time;
    private int end_work_time_range;
    private int end_school_time;
    private int end_school_time_range;
    private int end_social_time;
    private int end_social_time_range;

    private int width;
    private int height;
    
    private int population_size;

    private int seed;

    private int immunity_range;
    private float initial_infected_probabilty;
    private int rate_of_infection_in_ticks;
    private float death_chance;

    public GameObject parent;

    public Sprite infected_sprite;
    public Sprite uninfected_sprite;

    private List<Person> people;

    private List<CityCell> work_cells;
    private List<CityCell> school_cells;
    private List<CityCell> social_cells;
    private List<CityCell> house_cells;

    private List<CityCell> road_cells;

    private List<Hashtable> road_map;


    void OnEnable()
    {

        seed = world_manager.seed;
        width = world_manager.width;
        height = world_manager.height;

        population_size = world_manager.population_size;

        people = new List<Person>();

        immunity_range = world_manager.immunity_range;
        initial_infected_probabilty = world_manager.initial_infected_probabilty;
        rate_of_infection_in_ticks = world_manager.rate_of_infection_in_ticks;
        death_chance = world_manager.death_chance;

        speed = world_manager.speed;

        ticks_in_day = world_manager.ticks_in_day;

        start_work_time = world_manager.start_work_time;
        start_work_time_range = world_manager.start_work_time_range;
        start_school_time = world_manager.start_school_time;
        start_school_time_range = world_manager.start_school_time_range;
        start_social_time = world_manager.start_social_time;
        start_social_time_range = world_manager.start_social_time_range;

        end_work_time = world_manager.end_work_time;
        end_work_time_range = world_manager.end_work_time_range;
        end_school_time = world_manager.end_school_time;
        end_school_time_range = world_manager.end_school_time_range;
        end_social_time = world_manager.end_social_time;
        end_social_time_range = world_manager.end_social_time_range;

        work_cells = city_handler.get_workplaces();
        school_cells = city_handler.get_schools();
        social_cells = city_handler.get_socialplaces();
        house_cells = city_handler.get_houses();
        road_cells = city_handler.get_roads();

        road_map = Pathfinding.make_road_map(road_cells);

        game_ticks = 0;

        rnd = new System.Random(seed);

        for (int i=0; i<population_size; i++)
        {

            Person person = create_person();

            people.Add(
                person
            );
        }

        //begin_breakout();
        
    }

    void OnDisable()
    {
        foreach (Person person in people)
        {
            person.renderer.enabled = false;
        }
    }

    // 50 fps
    void FixedUpdate()
    {

        if (world_manager.pause)
            return;

        update_game_ticks();

        foreach (Person person in people)
        {

            if (person.is_dead)
            {
                person.renderer.enabled = false;
                continue;
            }

            if (!world_manager.lockdown)
            {
                person.in_lockdown = false;

                add_activity_time(person);

                add_path(person);

            }
            else
            {
                execute_lockdown(person);
            }

            move_people(person);

            if (person.infected && game_ticks % rate_of_infection_in_ticks == 0)
            {
                VirusHandler.infect_people(world_manager, people, person);
            }

            update_sprite(person);


            render_person(person);

            person.check_if_dead(rnd, death_chance);
            
        }
    }

    void update_game_ticks()
    {
        game_ticks++;
        if (game_ticks > ticks_in_day)
        {
            game_ticks -= ticks_in_day;
        }
    }

    Person create_person()
    {

        int no_of_activities = work_cells.Count
                                + school_cells.Count
                                + social_cells.Count;

        CityCell activity;
        int activity_index = rnd.Next(no_of_activities);
        
        if (activity_index < work_cells.Count)
        {
            activity = work_cells[activity_index];
        }
        else if (activity_index < work_cells.Count + school_cells.Count)
        {
            activity = school_cells[activity_index - work_cells.Count];
        }
        else
        {
            activity = social_cells[
                activity_index
                - work_cells.Count
                - school_cells.Count
            ];
        }

        int home_index = rnd.Next(house_cells.Count);
        CityCell home = house_cells[home_index];

        float offset_range = 0.4f;

        float immunity = rnd.Next(immunity_range) / 100f;

        Person person = new Person(
                            activity,
                            home,
                            parent,
                            (float)rnd.Next((int)(offset_range * 100)) / 100f - (offset_range/2.0f),
                            (float)rnd.Next((int)(offset_range * 100)) / 100f - (offset_range/2.0f),
                            immunity
                        );

        person.renderer.sprite = uninfected_sprite;
        person.renderer.enabled = false;

        return person;
    }

    void update_sprite(Person person)
    {
        if (person.infected)
        {
            person.renderer.sprite = infected_sprite;
        }
        else
        {
            person.renderer.sprite = uninfected_sprite;
        }
    }

    void begin_breakout()
    {
        int no_of_infected = Mathf.CeilToInt(
            population_size * initial_infected_probabilty
        );
        for (int i=0; i<no_of_infected; i++)
        {
            people[i].infected = true;
        }
    }


    void execute_lockdown(Person person)
    {
        if (!person.in_lockdown)
        {
            person.in_lockdown = true;
            person.clear_path();

            CityCell current_cell = city_handler.get_cell_by_coords(Mathf.FloorToInt(person.x), Mathf.FloorToInt(person.y));
            if (!current_cell.road && !(current_cell.x == person.house.x && current_cell.y == person.house.y))
            {
                current_cell = current_cell.closest_road;
            }
            find_path(
                person,
                current_cell,
                person.house
            );
        }
    }

    void add_activity_time(Person person)
    {

        if (person.activity.get_district() == "work")
        {
            if (game_ticks == start_work_time)
            {
                person.activity_time = rnd.Next(start_work_time_range);
            }
            else if (game_ticks == end_work_time)
            {
                person.activity_time = rnd.Next(end_work_time_range);
            }
        }

        if (person.activity.get_district() == "school")
        {
            if (game_ticks == start_school_time)
            {
                person.activity_time = rnd.Next(start_school_time_range);
            }
            else if (game_ticks == end_school_time)
            {
                person.activity_time = rnd.Next(end_school_time_range);
            }
        }

        if (person.activity.get_district() == "social")
        {
            if (game_ticks == start_social_time)
            {
                person.activity_time = rnd.Next(start_social_time_range);
            }
            else if (game_ticks == end_social_time)
            {
                person.activity_time = rnd.Next(end_social_time_range);
            }
        }
    }

    void add_path(Person person) 
    {

        bool work_time_check = game_ticks == person.activity_time + start_work_time && person.activity.get_district() == "work";
        bool school_time_check = game_ticks == person.activity_time + start_school_time && person.activity.get_district() == "school";
        bool social_time_check = game_ticks == person.activity_time + start_social_time && person.activity.get_district() == "social";

        bool end_work_time_check = game_ticks == person.activity_time + end_work_time && person.activity.get_district() == "work";
        bool end_school_time_check = game_ticks == person.activity_time + end_school_time && person.activity.get_district() == "school";
        bool end_social_time_check = game_ticks == person.activity_time + end_social_time && person.activity.get_district() == "social";


        if (work_time_check || school_time_check || social_time_check)
        {
            find_path(
                person,
                person.house.closest_road,
                person.activity
            );
            person.x = person.house.x;
            person.y = person.house.y;
        }

        if ((end_work_time_check || end_school_time_check || end_social_time_check))
        {
            find_path(
                person,
                person.activity.closest_road,
                person.house
            );
            person.x = person.activity.x;
            person.y = person.activity.y;
        }
    }

    void find_path(Person person, CityCell start_road, CityCell end)
    {

        if (start_road.x == end.x && start_road.y == end.y) { return; }

        Hashtable start_hashtable = Pathfinding.add_neighbour_directions(
            road_cells,
            Pathfinding.road_location(
                start_road
            )
        );
        Hashtable end_hashtable = Pathfinding.road_location(
            end.closest_road
        );

        bool found = Pathfinding.a_star(
            person,
            start_hashtable,
            end_hashtable,
            Pathfinding.copy_road_map(road_map)
        );
        
        if (found)
        {
            person.append_cell_to_path(end);
        }
    }

    void move_people(Person person)
    {

        if (person.has_path())
        {
            int[] target = person.get_current_target_coord();
            if (person.x < target[0])
            {
                person.x = Mathf.Min(person.x + speed, target[0]);
            }
            else if (person.x > target[0])
            {
                person.x = Mathf.Max(person.x - speed, target[0]);
            }
            else if (person.y < target[1])
            {
                person.y = Mathf.Min(person.y + speed, target[1]);
            }
            else if (person.y > target[1])
            {
                person.y = Mathf.Max(person.y - speed, target[1]);
            }
            else
            {
                person.next_coord();
            }

        }

    }

    void render_person(Person person)
    {
        if (person.has_path())
        {
            person.renderer.enabled = true;
            person.game_object.transform.position = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(
                                                new Vector3(
                                                    (float)Screen.width * (person.x + 0.5f + person.x_off) / width,
                                                    (float)Screen.height - (float)Screen.height * (person.y + 0.5f + person.y_off) / height,
                                                    0.95f)
                                                );

            person.game_object.transform.localScale = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(
                                            new Vector3(
                                                ((float)Screen.width / (float)width) * 0.1f + Screen.width/2,
                                                ((float)Screen.height / (float)height) * 0.1f + Screen.height/2,
                                                1.0f)
                                            );

        }
        else
        {
            person.renderer.enabled = false;
        }
    }


    public int get_game_ticks()
    {
        return game_ticks;
    }

    public int get_infected_count()
    {
        int count = 0;
        foreach (Person person in people)
        {
            if (person.infected && !person.is_dead)
                count++;
        }
        return count;
    }

    public int get_death_count()
    {
        int count = 0;
        foreach (Person person in people)
        {
            if (person.is_dead)
                count++;
        }
        return count;
    }



    public int get_population_count()
    {
        return population_size;
    }

}
