using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto;

namespace cser_hajnalka_dolgozatc__tagdijSQL
{
    internal class Program
    {   //lista ahova az ügyfeleket vesszük fel
        static List<Tagok> tagoklistaja = new List<Tagok>();
        //nagyon fontos az adatbázis kapcsolathoz
        static MySqlConnection connection = null;
        static MySqlCommand command = null; //szabadon felhasználható
                                            //adatok beoldavása az adatbázisból
        private static void tagtorlese()
        {
            command.CommandText = "DELETE FROM `ugyfel` WHERE `azon`=1014";
            command.Parameters.Clear();//beragadás esetére törölni kell a paraméterlistát, majd utánna beállítjuk az értékeket
            command.Parameters.AddWithValue("@azon", 1014);
            try
            {
                if (connection.State != System.Data.ConnectionState.Open) //ellenőrzés van-e hiba
                    connection.Open();
                command.ExecuteNonQuery();//amikor nem lekérdezzünk, akkor ezzel tudjuk a műveleteket elvégezni
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
            }
        }

        private static void ujtagfelvetele()
        {
            Tagok tag = new Tagok(1015, "Teszt Elek", 1991, 1117, "H");
            command.CommandText = "INSERT INTO `ugyfel`(`azon`, `nev`, `szulev`, `irszam`, `orsz`) VALUES (@azon,@nev,@szulev,@irszam,@orsz)";
            //ami a kukac után van azt jelenti, hogy majd később küldök adatoto, ez a küldés paraméterei
            command.Parameters.Clear();//beragadás esetére törölni kell a paraméterlistát, majd utánna beállítjuk az értékeket
            command.Parameters.AddWithValue("@azon", tag.azon);
            command.Parameters.AddWithValue("@nev", tag.nev);
            command.Parameters.AddWithValue("@szulev", tag.szulev);
            command.Parameters.AddWithValue("@irszam", tag.irszam);
            command.Parameters.AddWithValue("@orsz", tag.orsz);
            try
            {
                if (connection.State != System.Data.ConnectionState.Open) //ellenőrzés van-e hiba
                    connection.Open();
                command.ExecuteNonQuery();//amikor nem lekérdezzünk, akkor ezzel tudjuk a műveleteket elvégezni
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
            }
        }
        private static void tagoklistaz() //összes elem kiíratása
        {
            foreach (Tagok item in tagoklistaja)
            {
                Console.WriteLine(item);
            }
        }
        private static void beolvasas()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = "localhost";
            sb.UserID = "root";
            sb.Password = "";
            sb.Database = "tagdij";
            sb.CharacterSet = "utf8";
            connection = new MySqlConnection(sb.ConnectionString);
            command = connection.CreateCommand();
            try
            {
                connection.Open();
                command.CommandText = "SELECT * FROM `ugyfel`"; //későbbi feldolgozáshoz szükséges adatszerkezet
                //command.ExecuteNonQuery(); //ez a DataReader objektumot hozná létre, de ehhez kell a using..
                using (MySqlDataReader dr = command.ExecuteReader()) //erőforrásként jelőljük meg a using szóval az objektumot, command objektum futtása
                {
                    while (dr.Read()) //feldolgozuk az aokat
                    {
                        Tagok uj = new Tagok(dr.GetInt32("azon"), dr.GetString("nev"), dr.GetInt32("szulev"), dr.GetInt32("irszam"),
                            dr.GetString("orsz"));
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
            }
        }
        static void Main(string[] args)

        {//adatbázishoz kapcsolódáshoz meg kell adni a paramétereket
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Clear();
            sb.Server = "localhost";
            sb.UserID = "root";
            sb.Password = "";
            sb.Database = "tagdij";
            sb.CharacterSet = "utf8"; //nem mindig utf8, ezért jobb ha mindig beállítom!!
            // kell string a kapcsolat felvételére, így összerakja a karaktersorozatoto és átadja a mysqlConnection objektumnak
            MySqlConnection connection = new MySqlConnection(sb.ConnectionString);
            try
            {//probáljuk ki a kapcsolatot,connection megnyitása
                connection.Open(); //használatbavétel, de előtte kell try-olni, hogy működik-e a kapcsolat
                //parancs továbbítása
                MySqlCommand command = connection.CreateCommand(); //command opbejtuk a connectionra épül és közvetlenül kapcsolódjon hozzá
                command.CommandText = "SELECT * FROM `ugyfel`"; //végrehajtandó utasítás, lekérdezés, összes tag ()nem kell az sb.tostring,csak simán az SQL utasítást írom be "" közé.
                //lefutattjuk, lekérés mindig kapcsaolat alapú =MySqlDataReader kell  hozzá, és a command segítségével lefut a Select, és command.ExecuteReader() metódus
                using (MySqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read()) // a while azért kell, hogy leellenőrizzem hogy van-e adat a táblán, és ez akkor lessz igaz, ha van egy read metoduso
                    {
                        Tagok tagok = new Tagok(dr.GetInt32("azon"), dr.GetString("nev"), dr.GetInt32("szulev"), dr.GetInt32("irszam"),
                            dr.GetString("orsz"));
                        Console.WriteLine(tagok);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0); //kilépés a programból a nullával azt jelzem, hogy az op.rendszernek nincs teendője, lépjen ki.
            }

            beolvasas();
            tagoklistaz();
            ujtagfelvetele();
            tagtorlese();
            Console.WriteLine("\nProgram vége");
            Console.ReadLine();
        }
       
    }
}
    
