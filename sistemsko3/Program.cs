using sistemsko3;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace sistemsko3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Unesite prvo slovo ili ceo naziv koktela: ");
            string upit = Console.ReadLine().ToLower();

            var cocktailService = new CocktailService();

            if (upit.Length > 1)
            {
                Console.WriteLine("Poslali ste zahtev za ispis sastojka za koktel: " + upit);

                cocktailService.NabaviSastojke(upit).Subscribe(
                    sastojci =>
                    {
                        var kokteli = cocktailService.NabaviKoktele(upit).Wait(); // cekamo rezultat
                        int b = 0;
                        if (sastojci != null && sastojci.Count > 0)
                        {
                            foreach (var ingredient in sastojci)
                            {
                                Console.WriteLine(ingredient);
                                if (ingredient == null && kokteli.Count != b)
                                    Console.WriteLine("Koktel " + kokteli[b++] + " se sastoji iz sledecih sastojaka:");
                            }

                            var wordCounts = WordCounter.GetWordCounts(string.Join(" ", sastojci));
                            foreach (var rec in wordCounts.Keys)
                            {
                                Console.WriteLine("Rec " + rec + " se pojavljuje " + wordCounts[rec] + " puta.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nema koktela za zadatu rec.");
                        }
                    },
                    error =>
                    {
                        Console.WriteLine($"Došlo je do greške: {error.Message}");
                    },
                    () =>
                    {
                        Console.WriteLine("Završeno nabavljanje sastojaka.");
                    }
                );
            }
            else if (upit.Length == 1)
            {
                Console.WriteLine("Poslali ste zahtev za pretragu koktela koji pocinju na slovo: " + upit);
                Console.WriteLine("Kokteli koji pocinju na slovo " + upit + " su:");

                cocktailService.NabaviKoktele(upit).Subscribe(
                    kokteli =>
                    {
                        foreach (var koktel in kokteli)
                        {
                            Console.WriteLine(koktel);
                        }
                    },
                    error =>
                    {
                        Console.WriteLine($"Došlo je do greške: {error.Message}");
                    },
                    () =>
                    {
                        Console.WriteLine("Završeno nabavljanje koktela.");
                    }
                );
            }
            else
            {
                Console.WriteLine("Nevalidan zahtev!");
                return;
            }

            // cekanje da subscribe zavrsi
            Console.ReadLine();
        }
    }
}
