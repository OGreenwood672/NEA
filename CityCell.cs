using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCell
{

    public int x;
    public int y;

    public float noise;
    public string district;
    public bool road;

    public CityCell closest_road;

    public CityCell(int x_index, int y_index)
    {

        x = x_index;
        y = y_index;

    }

    public void set_noise(float new_noise) {

        noise = new_noise;

    }

    public void set_district(string new_district) {

        district = new_district;

    }

    public void set_closest_road(CityCell new_closest_road) {

        closest_road = new_closest_road;

    }

    public int calculate_euclidean_squared_distance(CityCell other) {

        int delta_x = other.x - x;
        int delta_y = other.y - y;
        return delta_x * delta_x + delta_y * delta_y;

    }

}
