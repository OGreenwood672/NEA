using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CityCell class stores information about a cell inside the city grid

public class CityCell
{

    // Coordinates relative to the rest of the city grid
    public int x;
    public int y;

    // Variables to assist City Generation
    // {
    
    // 2D perlin noise value to determine which neighbouring district
    // wins the district competiton
    private float noise;
    // District the cell belongs to. includes: School, workplace, social area
    private string district;
    // If the cell is a road
    public bool road;

    // }

    // Closest road to the cell; for people's movements
    public CityCell closest_road;

    // Constructor
    public CityCell(int x_index, int y_index)
    {

        x = x_index;
        y = y_index;

    }

    // Setter for noise variable
    public void set_noise(float new_noise) {

        noise = new_noise;

    }

    // Getter for noise variable
    public float get_noise() {

        return noise;

    }


    // Setter for district variable
    public void set_district(string new_district) {

        district = new_district;

    }
    // Getter for district variable
    public string get_district() {

        return district;

    }

    // Setter for closest road
    public void set_closest_road(CityCell new_closest_road) {

        closest_road = new_closest_road;

    }

    // Function to find distance between two cells
    public int calculate_euclidean_squared_distance(CityCell other) {

        int delta_x = other.x - x;
        int delta_y = other.y - y;
        return delta_x * delta_x + delta_y * delta_y;

    }

}
