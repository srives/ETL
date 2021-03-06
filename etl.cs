﻿// A program to import SBL GNT Morph data into a postgres table in a local databae.
// Run this program in one of these modes:
// 
//    Mode 1: Stats Generation Mode
//
//             ETL <-dbType> -root
//
//            This will generate statistics on all the words (their root forms) 
//            as used in each verse of the bible.
//
//    Mode 2: Import file mode
//
//             ETL <-dbType> some_greek_morph_filename.txt book_name short_name
//  
//            This mode will import one file into the postgres database into Database=Bible table=Greek
using System;
using System.Diagnostics;

namespace etl
{
    class ETL
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                if (args.Length == 2 && args[0] == "-mssql" && args[1] == "-root")
                    MS_SQL.AssignRootNumbers();
                else if (args.Length == 2 && args[0] == "-postgresql" && args[1] == "-root")
                    PostgreSQL.AssignRootNumbers();
                else if (args.Length == 4)
                {
                    if (args[0] == "-mssql")
                    {
                        MS_SQL.ImportOneFileIntoTheDatabase(args[1], args[2].ToLower(), args[3].ToLower());
                    }
                    else if (args[0] == "-postgresql")
                    {
                        PostgreSQL.ImportOneFileIntoTheDatabase(args[1], args[2].ToLower(), args[3].ToLower());
                    }
                }
                else
                {
                    Console.WriteLine("Usage: ETL [<-databaseType> | <-root>] | [<filename> <book name> <short name>]");
                    Console.WriteLine("       Greek NT Database Population tool.");
                    Console.WriteLine("       Populates a T-SQL or PostgreSQL database with the SBL GNT Morphological text files.");
                    Console.WriteLine("Two modes:");
                    Console.WriteLine("     Mode 1: Update the populated database (this is done with the -root option)");
                    Console.WriteLine("         This mode will generate a unique number for each root word in the NT");
                    Console.WriteLine("         It will write this number into each row of the Greek table.");
                    Console.WriteLine("         This assumes that you have already run this program to populate the database");
                    Console.WriteLine("         The benefit of doing this is speeding up searching.");
                    Console.WriteLine("         You will be able to write software to search on number instead of strings.");
                    Console.WriteLine("     Mode 2: Populate the database");
                    Console.WriteLine("         This will import SBL GNT Morph data into the database = Bible, table = Greek");
                    Console.WriteLine("         For example:");
                    Console.WriteLine("                        ETL -mssql 61-Mt-morphgnt.txt Matthew Mt");
                    Console.WriteLine();
                    Console.WriteLine("For the format of the talbe we modify, see the SQL script, createTableGreek.sql");
                    Console.WriteLine("For this and more, go to https://github.com/srives/ETL");
                    Console.WriteLine();
                    Console.WriteLine($"Stephen S. Rives, {DateTime.Now.Year}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetBaseException()}.");
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("COMPLETE");
                    Console.ReadLine();
                }
            }
        }
    }
}
