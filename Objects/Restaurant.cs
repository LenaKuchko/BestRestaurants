using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BestRestaurants;

namespace Restaurants.Objects
{
  public class Restaurant
  {
    private int _id;
    private string _name;
    private int _stars;
    private int _cuisineId;

    public Restaurant(string name, int stars, int cuisineId, int id = 0)
    {
      _id = id;
      _name = name;
      _stars = stars;
      _cuisineId = cuisineId;
    }

    public int GetId()
    {
      return _id;
    }
    public void SetId(int newId)
    {
      _id = newId;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public int GetStars()
    {
      return _stars;
    }
    public void SetStars(int newStars)
    {
      _stars = newStars;
    }
    public int GetCuisineId()
    {
      return _cuisineId;
    }
    public void SetCuisineId(int newCuisineId)
    {
      _cuisineId = newCuisineId;
    }

    public override bool Equals(System.Object otherRestaurant)
    {
      if (!(otherRestaurant is Restaurant))
      {
        return false;
      }
      else
      {
        Restaurant newRestaurant = (Restaurant) otherRestaurant;

        bool idEquality = this.GetId() == newRestaurant.GetId();
        bool nameEquality = this.GetName() == newRestaurant.GetName();
        bool starsEquality = this.GetStars() == newRestaurant.GetStars();
        bool cuisineIdEquality = this.GetCuisineId() == newRestaurant.GetCuisineId();

        return (idEquality && nameEquality && starsEquality && cuisineIdEquality);
      }
    }

    public static List<Restaurant> GetAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM restaurants;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      List<Restaurant> allRestaurants = new List<Restaurant>{};
      while (rdr.Read())
      {
        int restaurantId = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        int stars = rdr.GetInt32(2);
        int cuisineId = rdr.GetInt32(3);

        Restaurant newRestaurant = new Restaurant(name, stars, cuisineId, restaurantId);
        allRestaurants.Add(newRestaurant);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allRestaurants;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO restaurants (name, stars, cuisine_id) OUTPUT INSERTED.id VALUES (@RestaurantName, @RestaurantStars, @RestaurantCuisineId);", conn);

      SqlParameter nameParam = new SqlParameter("@RestaurantName", this.GetName());
      SqlParameter starsParam = new SqlParameter("@RestaurantStars", this.GetStars());
      SqlParameter cuisineIdParam = new SqlParameter("@RestaurantCuisineId", this.GetCuisineId());

      cmd.Parameters.Add(nameParam);
      cmd.Parameters.Add(starsParam);
      cmd.Parameters.Add(cuisineIdParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM restaurants;", conn);

      cmd.ExecuteNonQuery();
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static Restaurant Find(int idToFind)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM restaurants WHERE id = @RestaurantId;", conn);
      SqlParameter idParam = new SqlParameter("@RestaurantId", idToFind);
      cmd.Parameters.Add(idParam);

      SqlDataReader rdr = cmd.ExecuteReader();


      int restaurantId =  0;
      string name = null;
      int stars = 0;
      int cuisineId = 0;

      while(rdr.Read())
      {
        restaurantId = rdr.GetInt32(0);
        name = rdr.GetString(1);
        stars = rdr.GetInt32(2);
        cuisineId = rdr.GetInt32(3);
      }
      Restaurant foundRestaurant = new Restaurant(name, stars, cuisineId, restaurantId);

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return foundRestaurant;
    }

    public void Update(string name, int stars)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE restaurants SET name = @RestaurantName, stars = @RestaurantStars OUTPUT INSERTED.name, INSERTED.stars WHERE id = @RestaurantId;", conn);

      SqlParameter nameParam = new SqlParameter("@RestaurantName", name);
      SqlParameter starsParam = new SqlParameter("@RestaurantStars", stars);
      SqlParameter idParam = new SqlParameter("@RestaurantId", this.GetId());

      cmd.Parameters.Add(nameParam);
      cmd.Parameters.Add(starsParam);
      cmd.Parameters.Add(idParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._name = rdr.GetString(0);
        this._stars = rdr.GetInt32(1);
      }
      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public List<Client> GetSubscribers()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT clients.* FROM restaurants JOIN clients_restaurants ON (restaurants.id = clients_restaurants.restaurant_id) JOIN clients ON (clients_restaurants.client_id = clients.id) WHERE restaurants.id = @RestaurantId;", conn);
      SqlParameter restaurantIdParam = new SqlParameter("@RestaurantId", this.GetId());
      cmd.Parameters.Add(restaurantIdParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Client> subscribers = new List<Client>{};
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        Client newClient = new Client(name, id);
        subscribers.Add(newClient);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return subscribers;
    }

    public static List<Restaurant> SearchByName(string nameToSearch)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM restaurants WHERE name = @SearchName;", conn);
      SqlParameter searchParam = new SqlParameter("@SearchName", nameToSearch);
      cmd.Parameters.Add(searchParam);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Restaurant> matches = new List<Restaurant>{};
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        int stars = rdr.GetInt32(2);
        int cuisineId = rdr.GetInt32(3);
        Restaurant newRestaurant = new Restaurant(name, stars, cuisineId, id);
        matches.Add(newRestaurant);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return matches;
    }
  }
}
