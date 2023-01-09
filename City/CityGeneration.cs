using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Class to encapsulate all functions needed to generate the city
public class CityGeneration
{

    // The number of cells in width & hight
    private int width;
    private int height;

    // General seed
    private int seed;


    //City Generation Variables
    // {
    // Number of random districts generated to 
    private int num_of_districts;
    private float district_competition;

    private float vertical_road_threshhold;
    private float horizontal_road_threshhold;
    private int blocksize;
    // }


    //For the ratios of the buildings
    private int num_of_works;
    private int num_of_schools;
    private int num_of_socials;
    private int num_of_houses;



    //generate empty city
    private CityCell[,] city;

    public CityGeneration(
        WorldManager world_manager
    ) {

        width = world_manager.width;
        height = world_manager.height;

        seed = world_manager.seed;

        district_competition = world_manager.district_competition;
        num_of_districts = world_manager.num_of_districts;

        vertical_road_threshhold = world_manager.vertical_road_threshhold;
        horizontal_road_threshhold = world_manager.horizontal_road_threshhold;
        blocksize = world_manager.blocksize;

        num_of_works = world_manager.num_of_works;
        num_of_schools = world_manager.num_of_schools;
        num_of_socials = world_manager.num_of_socials;
        num_of_houses = world_manager.num_of_houses;

    }


    public CityCell[,] generate_city()
    {
        // Initialised City in a 2D array of width x height containing CityCell object
        // Each object is given x and y coordinates
        city = new CityCell[height, width];
        for (int i=0; i<height; i++)
        {
            for (int j=0; j<width; j++)
            {
                city[i, j] = new CityCell(j, i);
            }
        }
        
        // Look into
        if (seed % 10 == 0) { seed++; }

        //Functions to generate City
        add_noise();
        add_districts();
        enable_border_compeition();
        add_roads();
        
        // For people travelling from building to building
        add_closest_road();

        return city;
    }

    // Added modified noise attribute to every cell in the city
    private void add_noise()
    {

        // Add perlin noise to c#
        float inc = 0.1f;
        float x_off = 0f;
        float y_off = 0f;

        for (int y=0; y<height; y++)
        {

            x_off = 0f;
            for (int x=0; x<width; x++)
            {

                float mod_perlin_noise = Mathf.PerlinNoise((x_off * 1.13f) * seed, (y_off * 1.22f) * seed) * 20;
                mod_perlin_noise -= Mathf.Floor(mod_perlin_noise);
                city[y, x].set_noise(mod_perlin_noise);

                x_off += inc;
            }
            y_off += inc;
        }
    }
    
    // Add Districts to the city in random locations
    private void add_districts()
    {


        int[,] random_district_coords = new int[num_of_districts, 2];
        int current_index = 0;

        System.Random rnd = new System.Random(seed);
        
        // Finds <num_of_districts> unique coordinates whcih can be used to represent a district
        while (true)
        {
            
            // Setting seed here crashes program (bug fixed, set seed infintite times)

            int[] coords = {
                rnd.Next(height),
                rnd.Next(width)
            };

            bool duplicate = false;
            for (int i=0; i<current_index; i++)
            {
                if (random_district_coords[i, 0] == coords[0] &&
                    random_district_coords[i, 1] == coords[1])
                    {
                        duplicate = true;
                    }
            }
            if (!duplicate)
            {
                random_district_coords[current_index, 0] = coords[0];
                random_district_coords[current_index, 1] = coords[1];
                current_index++;

                if (current_index == num_of_districts) { break; }

            }

        }
        
        // Getting a list of the CityCell objects that represent the districts
        CityCell[] random_districts = new CityCell[num_of_districts];
        for (int i=0; i<num_of_districts; i++)
        {
            random_districts[i] = city[random_district_coords[i, 0], random_district_coords[i, 1]];
        }
        
        
        // Give each district a different type:
        //  Work
        //  School
        //  Social
        //  House
        Dictionary<int, string> district_types = new Dictionary<int, string>();
        
        float total = (float)(num_of_schools + num_of_socials + num_of_works + num_of_houses);

        float school_ratio = num_of_schools / total;
        float social_ratio = num_of_socials / total;
        float work_ratio = num_of_works / total;
        float house_ratio = num_of_houses / total;
        
        for (let i=0; i<num_of_districts; i++)
        {
            float probabilty = (float)(rnd.Next(101) + 1) / 100;
            if (probabilty < work_ratio)
            {
                district_types.Add(i, "work");

            } else if (probabilty < school_ratio + work_ratio)
            {
                district_types.Add(i, "school");

            } else if (probabilty < school_ratio + work_ratio + social_ratio)
            {
                district_types.Add(i, "social");

            } else
            {
                district_types.Add(i, "house");

            }
            // city[col, row].set_district(districts[distances[0, 1] % districts.Length]); // +1
        }
        

        // Finds the closest district for every cell in the city and apply district type to that cell
        for (int col=0; col<height; col++)
        {
            for (int row=0; row<width; row++)
            {
                int[,] distances = new int[num_of_districts, 2];
                int index = 0;
                foreach (CityCell district in random_districts)
                {
                    distances[index, 0] = city[col, row].calculate_euclidean_squared_distance(district);
                    distances[index, 1] = index;
                    index++;
                }     

                distances = CityGenerationsUtilities.merge_sort_2d(distances, 0);

                city[col, row].set_district(district_types[distances[0, 1]]);
            
            }
        }
    }
    
    // Allows district have unperfect borders providing a more realistic city
    private void enable_border_compeition()
    {

        int range = (int)Mathf.Floor(width * district_competition);
        for (int y=0; y<height; y++)
        {
            for (int x=0; x<width; x++)
            {
                CityCell left = city[y, CityGenerationsUtilities.constrain(x-range, 0, width-1)];
                CityCell right = city[y, CityGenerationsUtilities.constrain((x+range), 0, width-1)];
                if (left.district != right.district)
                {
                    if (city[y, x].noise > 0.5)
                    {
                        city[y, x].set_district(left.district);

                    } else {

                        city[y, x].set_district(right.district);

                    }
                }
            }
        }

    }
    
    // Adds roads to the city
    private void add_roads()
    {
        
        //Vertical Roads
        List<int> main_roads = new List<int>();
        main_roads.Add(0);

        for (int x=1; x < width; x++)
        {   
            if (Mathf.PerlinNoise((float)(x * seed) / (float)width, 1.0f) > vertical_road_threshhold)
            {
                main_roads.Add(x+1);

                for (int y=0; y < height; y++)
                {
                    city[y, x].road = true;
                    city[y, x].set_district(null);
                }
            }
        }
        // Horizontal roads
        int next = 0;
        foreach (int x_road in main_roads)
        {
            
            for (int y=0; y < height; y++)
            {
                if (next > 0)
                {
                    next--;

                } else {

                    if (Mathf.PerlinNoise((float)x_road / (float)width * seed, (float)y / (float)height * seed) > horizontal_road_threshhold)
                    {

                        int x = x_road;
                        if (x > width - 1) { return; }

                        while (!city[y, x].road)
                        {

                            city[y, x].road = true;
                            city[y, x].set_district(null);
                            x++;

                            if (x > width - 1) { break; }

                        }
                        x = x_road;
                        next = blocksize;
                    }
                }
            }
        }

    }
    
    // Add the closest road to all the cells
    void add_closest_road()
    {

        int coord_change;

        for (int y=0; y<height; y++)
        {
            for (int x=0; x<width; x++)
            {

                if (!city[y, x].road)
                {

                    coord_change = 0;
                    // If no road found, what then?
                    while (true)//coord_change < Mathf.Min(width, height) / 2)
                    {

                        if (city[y, CityGenerationsUtilities.constrain(x+coord_change, 0, width-1)].road)
                        {
                            city[y, x].set_closest_road(city[y, x+coord_change]);
                            break;
                        }

                        if (city[y, CityGenerationsUtilities.constrain(x-coord_change, 0, width-1)].road)
                        {
                            city[y, x].set_closest_road(city[y, x-coord_change]);
                            break;
                        }

                        if (city[CityGenerationsUtilities.constrain(y+coord_change, 0, height-1), x].road)
                        {
                            city[y, x].set_closest_road(city[y+coord_change, x]);
                            break;
                        }

                        if (city[CityGenerationsUtilities.constrain(y-coord_change, 0, height-1), x].road)
                        {
                            city[y, x].set_closest_road(city[y-coord_change, x]);
                            break;
                        }

                        coord_change++;

                    }

                }
            }
        }

    }

}
