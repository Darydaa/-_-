using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Find
{
    public abstract class Result
    {
        public int[] genre_ids;
        public float popularity;
        public int vote_count;
        public string backdrop_path;
        public string original_language;
        public int id;
        public float vote_average;
        public string overview;
        public string poster_path;
        

    }
        public class AnswerMovie
    {
       public int page;
       public  int total_results;
       public  int total_pages;
       public List<Movie> results;
    }
         public class AnswerTV
    {
       public int page;
       public  int total_results;
       public  int total_pages;
       public List<TV> results;
    }
   public class Movie:Result
    {
        public bool adult;
        public string release_date;
        public string original_title;
        public string title;
        public bool video;
    }
    public class TV:Result
    {
        public string original_name;
        public string name;
        public string[] origin_country;
        public string first_air_date;
    }

    public class Genres
    {
       public struct genre{public int id;public string name;}
       public List<genre> genres=new List<genre>();

    }
    }
