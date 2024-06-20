using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text.Json;

namespace sistemsko3
{
    public class CocktailService
    {
        public IObservable<string> FetchCocktailData(string upit)
        {
            return Observable.FromAsync(async () =>
            {
                using HttpClient client = new HttpClient();
                string url = $"https://www.thecocktaildb.com/api/json/v1/1/search.php?f={upit}";
                if (upit.Length > 1)
                {
                    url = $"https://www.thecocktaildb.com/api/json/v1/1/search.php?s={upit}";
                }
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            });
        }

        public IObservable<List<string>> NabaviSastojke(string upit)
        {
            return FetchCocktailData(upit)
                .Select(Rezultat =>
                {
                    if (Rezultat != null)
                    {
                        var jsonDoc = JsonDocument.Parse(Rezultat);
                        var drinks = jsonDoc.RootElement.GetProperty("drinks");

                        var Sastojci = new List<string>();
                        Sastojci.Add(null);

                        foreach (var drink in drinks.EnumerateArray())
                        {
                            for (int i = 1; i <= 15; i++)
                            {
                                var OpisSastojka = $"strIngredient{i}";
                                if (drink.TryGetProperty(OpisSastojka, out var ingredientElement) && ingredientElement.ValueKind != JsonValueKind.Null)
                                {
                                    var ingredient = ingredientElement.GetString();
                                    if (!string.IsNullOrEmpty(ingredient))
                                    {
                                        Sastojci.Add(ingredient);
                                    }
                                }
                            }
                            Sastojci.Add(null);
                        }
                        return Sastojci;
                    }
                    return null;
                });
        }

        public IObservable<List<string>> NabaviKoktele(string upit)
        {
            return FetchCocktailData(upit)
                .Select(Rezultat =>
                {
                    if (Rezultat != null)
                    {
                        var jsonDoc = JsonDocument.Parse(Rezultat);
                        var drinks = jsonDoc.RootElement.GetProperty("drinks");

                        var Kokteli = new List<string>();

                        foreach (var drink in drinks.EnumerateArray())
                        {
                            if (drink.TryGetProperty("strDrink", out var NazivKoktela) && NazivKoktela.ValueKind != JsonValueKind.Null)
                            {
                                var Ime = NazivKoktela.GetString();
                                if (!string.IsNullOrEmpty(Ime))
                                    Kokteli.Add(Ime);
                            }
                        }
                        return Kokteli;
                    }
                    return null;
                });
        }
    }
}
