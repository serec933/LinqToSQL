using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToSQL
{
    public class Esercizio
    {
        const string connectionString = @"Persist Security Info = False; Integrated Security= true; Initial Catalog = CinemaDB; Server = WINAPMGUDJIV7BS\SQLEXPRESS";

        //Seleziono film
        public static void SelectMovies()
        {
            //Check per vedere se la mappatura ha funzionato
            CinemaDataContext db = new CinemaDataContext(connectionString);

            foreach (var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1}, {2}",movie.ID,movie.Title,movie.Genere);
            }
        }
        //filtrare film
        public static void FilterMovieByGenere()
        {
            CinemaDataContext db = new CinemaDataContext(connectionString);
            foreach (var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1}, {2}", movie.ID, movie.Title, movie.Genere);
            }
            Console.WriteLine("Che genere ti interessa?");
            string genere = Console.ReadLine();

            IQueryable<Movy> moviesFiltered =
                from m in db.Movies
                where m.Genere == genere
                select m;

            foreach (var movie in moviesFiltered)
            {
                Console.WriteLine("{0} - {1}, {2}", movie.ID, movie.Title, movie.Genere);
            }

        }

        //Inserire record
        public static void InsertMovie()
        {
            CinemaDataContext db = new CinemaDataContext(connectionString);

            SelectMovies();

            var movieToInsert = new Movy();
            movieToInsert.Title = "Lalaland";
            movieToInsert.Genere = "Musical";
            movieToInsert.Durata = 120;

            db.Movies.InsertOnSubmit(movieToInsert);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SelectMovies(); //se lo faccio prima di submit changes non vedo il nuovo film

            var DeleteMovie = db.Movies.SingleOrDefault(m => m.ID == 9);
            if (DeleteMovie != null)
            {
                db.Movies.DeleteOnSubmit(DeleteMovie);
            }
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SelectMovies();
        }
        //Update Movie
        public static void UpdateMovieByTitolo()
        {
            CinemaDataContext db = new CinemaDataContext(connectionString);

            Console.WriteLine("Dimmi il titolo del film da aggiornare: ");
            Console.ReadLine();
            string titolo = Console.ReadLine();

            IQueryable<Movy> filmByTitolo =
                from film in db.Movies
                where film.Title == titolo
                select film;

            Console.WriteLine("I Film trovati sono: {0}",filmByTitolo.Count());

            if (filmByTitolo.Count() ==0)
            {
                return;
            }
            SelectMovies();
            Console.WriteLine("Scrivere i valori aggiornati:");
            Console.WriteLine("titolo: ");
            string tit = Console.ReadLine();
            Console.WriteLine("genere: ");
            string gen = Console.ReadLine();
            Console.WriteLine("durata: ");
            string durata = Console.ReadLine();
            int d = Convert.ToInt32(durata);

            foreach (var f in filmByTitolo)
            {
                f.Title = tit;
                f.Genere = gen;
                f.Durata = d;
            }

            try
            {
                Console.WriteLine("Premi un tasto per modificare DB");
                Console.ReadKey();
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch(ChangeConflictException e)
            {
                Console.WriteLine("Concurrency error");
                Console.WriteLine(e);

                db.ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues);
                //db.SubmitChanges();    negli altri casi
            }

        }
    
    }
}
