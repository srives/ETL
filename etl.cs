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
        static void Main(string[] args)
        {
            int ttl = 0;
            int errors = 0;
            // args[0] = input file name
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: ETL <filename> <book name> <short name>");
                Console.WriteLine("       Will import SBL GNT Morph data into postgre database = Bible, table = Greek");
                Console.WriteLine("       E.g., ETL 61-Mt-morphgnt.txt Matthew Mt");
            }
            else
            {
                string fileName = args[0];
                string book_name = args[1];
                string short_name = args[2];
                int canon_order=0;

                if (File.Exists(fileName))
                {
                    StreamReader sr = File.OpenText(fileName);
                    if (sr != null)
                    {
                        string connection = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=admin; Database=Bible;";
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
                                                               "word, root, sequence)";
                            int last_verse = 0;
                            int sequence = 0;
                            while ((s = sr.ReadLine()) != null)
                            {
                                sequence++;
                                
                                // 0      1  2        3           4           5           6
                                // 012812 A- ----GPM- πρεσβυτέρων πρεσβυτέρων πρεσβυτέρων πρεσβύτερος

                                string[] parts = s.Split(' ');
                                canon_order = int.Parse(parts[0].Substring(0, 2));
                                int chapter = int.Parse(parts[0].Substring(2, 2));
                                int verse = int.Parse(parts[0].Substring(4, 2));
                                if (verse != last_verse)
                                    sequence = 1;
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
                                values += "" + sequence + ")";

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
                            Console.WriteLine("Could not connect to Postgre database: " + connection);
                    }
                    else
                        Console.WriteLine("Could not open file: " + fileName);
                }
                else
                    Console.WriteLine("Could not find file: " + fileName);
            }
        }
    }
}
