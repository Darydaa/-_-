using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Find
{
    public delegate string Sh(Find.AnswerMovie mo, Find.AnswerTV tV, Find.Genres genres);

    public class Search
    {
        public static string API_key = "fa44fe25198fecb879ad580ed2a067b5";
     
        public static async Task<T> GetRezaltAsync<T>(string path) where T : class
        {
            HttpClient client = new HttpClient();
            T rezalt = null;
            try
            {
               
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    rezalt = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }
            }
            catch(Exception e) { 
            
            }
                return rezalt;
            
        }
        public static string Show(AnswerMovie mo, AnswerTV tV, Genres genres)
        {
            string rez = "";
            if (mo != null)
            {
                if (mo.total_results != 0)
                    for (int i = 0; i < mo.total_results && i < 20; i++)
                    {
                        rez+=($"Название:{mo.results[i].original_title}\n");
                        rez += ($"Популярность:{mo.results[i].popularity}\n");
                        rez += ($"Рейтинг:{mo.results[i].vote_average}\n");
                        rez += ($"Описание:{mo.results[i].overview}\n");
                        rez += ($"Жанр:");
                        for (int t = 0; t < mo.results[i].genre_ids.Length; t++)
                        {
                            rez += (genres.genres.Find(x => x.id == mo.results[i].genre_ids[t]).name);
                            if (mo.results[i].genre_ids.Length - 1 != t)
                            {
                                rez += (",");
                            }
                        }

                        rez += ($"\nДата реализации:{mo.results[i].release_date}\n");
                        rez += ("\n" + new string('-', Console.BufferWidth));
                    }
                else rez += ("Ничего не найдено!\n");
            }
            else if (tV != null)
            {
                if (tV.total_results != 0)
                    for (int i = 0; i < tV.total_results && i < 20; i++)
                    {
                        rez += ($"Название:{tV.results[i].original_name}\n");
                        rez += ($"Популярность:{tV.results[i].popularity}\n");
                        rez += ($"Рейтинг:{tV.results[i].vote_average}\n");
                        rez += ($"Описание:{tV.results[i].overview}\n");
                        rez += ($"Жанр:");
                        for (int t = 0; t < tV.results[i].genre_ids.Length; t++)
                        {
                            rez += (genres.genres.Find(x => x.id == tV.results[i].genre_ids[t]).name);
                            if (tV.results[i].genre_ids.Length - 1 != t)
                            {
                                rez += (",");
                            }
                        }

                        rez += ($"\nДата реализации:{tV.results[i].first_air_date}\n");
                        rez += ("\n" + new string('-', Console.BufferWidth));
                    }
                else rez += ("Ничего не найдено!\n");
            }
            return rez;
        }

    }

}
