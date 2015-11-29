// A program to import SBL GNT Morph data into a postgres table in a local databae.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.IO;

namespace etl
{
    class etl
    {
        public static string connection = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=admin; Database=Bible;";

        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-root")
                AssignRootNumbers();
            else if (args.Length == 3)
                ImportOneFileIntoPostgres(args[0], args[1], args[2]);
            else
            {
                Console.WriteLine("Usage: ETL [-root] | [<filename> <book name> <short name>]");
                Console.WriteLine("       Greek NT Database Population tool.");
                Console.WriteLine("       Populates a Postgres database with the SBL GNT Morphological text files.");
                Console.WriteLine("Two modes:");
                Console.WriteLine("     Mode 1: Update the populated database (this is done with the -root options)");
                Console.WriteLine("         This mode will generate a 'rives-number' for each root word in the NT");
                Console.WriteLine("         It will write the rives-number into each row of the Greek table.");
                Console.WriteLine("         This assumes that you have already run this program to populate the database");
                Console.WriteLine("     Mode 2: Populate the database");
                Console.WriteLine("       Will import SBL GNT Morph data into postgre database = Bible, table = Greek");
                Console.WriteLine("       For example:");
                Console.WriteLine("                        ETL 61-Mt-morphgnt.txt Matthew Mt");
            }           
        }

        /// <summary>
        /// This function will walk all the current tables and assign a number to each word as per its root word.
        /// </summary>
        static void AssignRootNumbers()
        {
            Console.WriteLine("Updating database with frequency counts.");
            int rivesNum = 1;
            Dictionary<string, int> rootWords = new Dictionary<string, int>();
            NpgsqlConnection conn = new NpgsqlConnection(connection);
            if (conn != null)
            {
                conn.Open();
                string sql = "select distinct root from greek order by root";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("Generating unique number for each unqiue root word.");
                while (reader.Read())
                    rootWords[reader["root"].ToString()] = rivesNum++; 
                reader.Close();
                cmd.Dispose();
                Console.WriteLine("Number of unique NT words = " + rootWords.Count);

                int hapax = 0;
                // For each root word in our Dictionary, update the database with the number of times
                // the root word occurs.
                foreach(var pair in rootWords)
                {
                    int root_num = pair.Value;
                    sql = "update greek set root_num="+ root_num + " where root='" + pair.Key + "'";
                    cmd = new NpgsqlCommand(sql, conn);
                    int frequence = cmd.ExecuteNonQuery();
                    if (frequence == 1)
                        hapax++;
                    if (frequence <= 0)
                        Console.WriteLine("Error writing out data for " + pair.Key);
                    cmd.Dispose();
                    sql = "update greek set root_freq_nt=" + frequence + " where root='" + pair.Key + "'";
                    cmd = new NpgsqlCommand(sql, conn);
                    int written = cmd.ExecuteNonQuery();
                    if (written != frequence)
                        Console.WriteLine("Error saving frequency count for " + pair.Key);
                    cmd.Dispose();
                }
                Console.WriteLine("Updated databae.");
                Console.WriteLine("Number of NT words that are hapax lagomenon: " + hapax);
            }
            else
                Console.WriteLine("Could not connect to Postgres database");
        }

        /// <summary>
        /// This function will read one .txt file from the SBNGNT MorphGNT SBLGNT repository (found on GitHub)
        /// It will import that data into Postgres into the database specifed in the global connection string, into
        /// the greek table. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="book_name"></param>
        /// <param name="short_name"></param>
        static void ImportOneFileIntoPostgres(string fileName, string book_name, string short_name)
        {
            int ttl = 0; // how many records I added to the postgres database
            int errors = 0;

            int canon_order=0;

            if (File.Exists(fileName))
            {
                StreamReader sr = File.OpenText(fileName);
                if (sr != null)
                {
                        
                    NpgsqlConnection conn = new NpgsqlConnection(connection);
                    if (conn != null)
                    {
                        conn.Open();
                        string s = String.Empty;
                        string insert = "insert into greek (book_name, short_name, canon_order, chapter, verse,"    +
                                                            "adjective, conjunction, adverb, interjection, noun,"    +
                                                            "preposition, article, demons_pronoun, indef_pronoun,"   +
                                                            "person_pronoun, relative_pronoun, verb, particle,"      +
                                                            "person, tense, voice, mood, noun_case, sing_or_plural," +
                                                            "gender, degree, word_with_punct, word_without_punct,"   +
                                                            "word, root, sentence_position, book_position)";
                        int last_verse = 0;
                        int sentence_position = 0; // within this verse, a word has a position
                        int book_position = 0;     // and within this book, it also has a position
                        while ((s = sr.ReadLine()) != null)
                        {
                            book_position++; 
                            sentence_position++;
                                
                            // 0      1  2        3           4           5           6
                            // 012812 A- ----GPM- πρεσβυτέρων πρεσβυτέρων πρεσβυτέρων πρεσβύτερος

                            string[] parts = s.Split(' ');
                            canon_order = int.Parse(parts[0].Substring(0, 2));
                            int chapter = int.Parse(parts[0].Substring(2, 2));
                            int verse = int.Parse(parts[0].Substring(4, 2));
                            if (verse != last_verse)
                                sentence_position = 1;
                            last_verse = verse;
                            // Parts of Speech
                            bool adjective = parts[1] == "A-";
                            bool conjunction = parts[1] == "C-";
                            bool adverb = parts[1] == "D-";
                            bool interjection = parts[1] == "I-";
                            bool noun = parts[1] == "N-";
                            bool preposition = parts[1] == "P-";
                            bool article = parts[1] == "RA";
                            bool demons_pronoun = parts[1] == "RD";
                            bool indef_pronoun = parts[1] == "RI";
                            bool person_pronoun = parts[1] == "RP"; 
                            bool relative_pronoun = parts[1] == "RR"; 
                            bool verb = parts[1] == "V-"; 
                            bool particle = parts[1] == "X-";
                            // Parsing Code, 3AAI-S--
                            int person = 0;
                            int.TryParse(parts[2].Substring(0, 1), out person);
                            string tense = parts[2].Substring(1, 1); 
                            // tense (P=present, I=imperfect, F=future, A=aorist, X=perfect, Y=pluperfect)
                            if (tense == "-")
                                tense = string.Empty;
                            if (tense == "P")
                                tense = "Present";
                            if (tense == "I")
                                tense = "Imperfect";
                            if (tense == "F")
                                tense = "Future";
                            if (tense == "A")
                                tense = "Aorist";
                            if (tense == "X")
                                tense = "Perfect";
                            if (tense == "Y")
                                tense = "Pluperfect";
                            string voice = parts[2].Substring(2, 1); // voice (A=active, M=middle, P=passive)
                            if (voice == "-")
                                voice = string.Empty;
                            if (voice == "A")
                                voice = "Active";
                            if (voice == "M")
                                voice = "Middle";
                            if (voice == "P")
                                voice = "Passive";
                            string mood = parts[2].Substring(3, 1); // (I=indicative, D=imperative, S=subjunctive, O=optative, N=infinitive, P=participle)
                            if (mood == "-")
                                mood = string.Empty;
                            if (mood == "I")
                                mood = "Indicative"; 
                            if (mood == "D")
                                mood = "Imperative";
                            if (mood == "S")
                                mood = "Subjunctive";
                            if (mood == "O")
                                mood = "Optative"; 
                            if (mood == "N")
                                mood = "Infinitive";
                            if (mood == "P")
                                mood = "Participle";

                            string noun_case = parts[2].Substring(4, 1); // (N=nominative, G=genitive, D=dative, A=accusative)
                            if (noun_case == "-")
                                noun_case = string.Empty;
                            if (noun_case == "N")
                                noun_case = "Nominative";
                            if (noun_case == "G")
                                noun_case = "Genitive";
                            if (noun_case == "D")
                                noun_case = "Dative";
                            if (noun_case == "A")
                                noun_case = "Accusative";
                            if (noun_case == "V")
                                noun_case = "Vocative";

                            string sing_or_plural = parts[2].Substring(5, 1); // (S=singular, P=plural)
                            if (sing_or_plural == "-")
                                sing_or_plural = string.Empty;
                            string gender = parts[2].Substring(6, 1); // (M=masculine, F=feminine, N=neuter)
                            if (gender == "-")
                                gender = string.Empty;
                            string degree = parts[2].Substring(7, 1); // (C=comparative, S=superlative)
                            if (degree == "-")
                                degree = string.Empty;
                            // The words
                            string word_with_punct = parts[3];
                            string word_without_punct = parts[4];
                            string word = parts[5];
                            string root = parts[6];
                                

                            string values = "values (";
                            values += "'" + book_name + "',";
                            values += "'" + short_name + "',";
                            values += "" + canon_order + ",";
                            values += "" + chapter + ",";
                            values += "" + verse + ",";
                            values += "" + adjective + ",";
                            values += "" + conjunction + ",";
                            values += "" + adverb + ",";
                            values += "" + interjection + ",";
                            values += "" + noun + ",";
                            values += "" + preposition + ",";
                            values += "" + article + ",";
                            values += "" + demons_pronoun + ",";
                            values += "" + indef_pronoun + ",";
                            values += "" + person_pronoun + ",";
                            values += "" + relative_pronoun + ",";
                            values += "" + verb + ",";
                            values += "" + particle + ",";
                            values += "" + person + ",";
                            values += "'" + tense + "',";
                            values += "'" + voice + "',";
                            values += "'" + mood + "',";
                            values += "'" + noun_case + "',";
                            values += "'" + sing_or_plural + "',";
                            values += "'" + gender + "',";
                            values += "'" + degree + "',";
                            values += "'" + word_with_punct + "',";
                            values += "'" + word_without_punct + "',";
                            values += "'" + word + "',";
                            values += "'" + root + "',";
                            values += "" + sentence_position + ",";
                            values += "" + book_position + ")";
                                
                            string sql = insert + " " + values;
                            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                            int ct = cmd.ExecuteNonQuery();
                            if (ct == 0)
                            {
                                errors++;
                                Console.WriteLine("Insert error");
                            }
                            else
                                ttl++;
                            cmd.Dispose();
                        }
                        Console.WriteLine("Imported " + ttl + " words into the Postgre database");
                        if (errors != 0)
                            Console.WriteLine("  " + errors + " errors.");

                    }
                    else
                        Console.WriteLine("Could not connect to Postgres database: " + connection);
                }
                else
                    Console.WriteLine("Could not open file: " + fileName);
            }
            else
                Console.WriteLine("Could not find file: " + fileName);
        }
        
    }
}
