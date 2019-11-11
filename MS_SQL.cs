using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace etl
{
    internal static class MS_SQL
    {
        public static string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=Bible;Integrated Security=SSPI;";

        /// <summary>
        /// This function will walk all the current tables and assign a number to each word as per its root word.
        /// </summary>
        internal static void AssignRootNumbers()
        {
            int rivesNum = 1;
            var rootWords = new Dictionary<string, int>();

            Console.WriteLine("Updating database with frequency counts.");

            using (var conn = new SqlConnection(connection))
            {
                if (conn != null)
                {
                    conn.Open();
                    
                    var sql = "select distinct root from Greek order by root";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("Generating unique number for each unqiue root word.");
                            while (reader.Read())
                                rootWords[reader["root"].ToString()] = rivesNum++;
                        }
                    }
                    Console.WriteLine("Number of unique NT words = " + rootWords.Count);

                    int hapax = 0;
                    // For each root word in our Dictionary, update the database with the number of times
                    // the root word occurs.
                    foreach (var pair in rootWords)
                    {
                        int frequency;
                        int root_num = pair.Value;
                        sql = "update Greek set root_num=" + root_num + " where root='" + pair.Key + "'";
                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            frequency = cmd.ExecuteNonQuery();
                            if (frequency == 1)
                                hapax++;

                            if (frequency <= 0)
                                Console.WriteLine("Error writing out data for " + pair.Key);
                        }

                        sql = "update Greek set root_freq_nt=" + frequency + " where root='" + pair.Key + "'";
                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            int written = cmd.ExecuteNonQuery();
                            if (written != frequency)
                                Console.WriteLine("Error saving frequency count for " + pair.Key);
                        }
                    }

                    Console.WriteLine("Updated databae.");
                    Console.WriteLine("Number of NT words that are hapax lagomenon: " + hapax);
                }
                else
                    Console.WriteLine("Could not connect to Postgres database");
            }
        }

        /// <summary>
        /// This function will read one .txt file from the SBNGNT MorphGNT SBLGNT repository (found on GitHub)
        /// It will import that data into Postgres into the database specifed in the global connection string, into
        /// the Greek table. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="book_name"></param>
        /// <param name="short_name"></param>
        internal static void ImportOneFileIntoPostgres(string fileName, string book_name, string short_name)
        {
            var filePath = $"Texts/{fileName}";

            if (File.Exists(filePath))
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    if (sr != null)
                    {
                        Console.WriteLine($"Processing file: {filePath}");

                        using (var conn = new SqlConnection(connection))
                        {
                            if (conn == null)
                            {
                                throw new NullReferenceException($"Could not connect to database: {connection}.");
                            }

                            conn.Open();
                            using (var transaction = conn.BeginTransaction())
                            {
                                var s = string.Empty;
                                int last_verse = 0;
                                int sentence_position = 0; // within this verse, a word has a position
                                int book_position = 0;     // and within this book, it also has a position

                                var dt = DataTableUtility.GreekColumns;

                                while ((s = sr.ReadLine()) != null)
                                {
                                    book_position++;
                                    sentence_position++;

                                    // 0      1  2        3           4           5           6
                                    // 012812 A- ----GPM- πρεσβυτέρων πρεσβυτέρων πρεσβυτέρων πρεσβύτερος

                                    string[] parts = s.Split(' ');
                                    int canon_order = int.Parse(parts[0].Substring(0, 2));
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
                                    int.TryParse(parts[2].Substring(0, 1), out int person);

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

                                    dt.Rows.Add(
                                        book_name,
                                        short_name,
                                        canon_order,
                                        chapter,
                                        verse,
                                        sentence_position,
                                        book_position,
                                        adjective,
                                        conjunction,
                                        adverb,
                                        interjection,
                                        noun,
                                        preposition,
                                        article,
                                        demons_pronoun,
                                        indef_pronoun,
                                        person_pronoun,
                                        relative_pronoun,
                                        verb,
                                        particle,
                                        person,
                                        tense,
                                        voice,
                                        mood,
                                        noun_case,
                                        sing_or_plural,
                                        gender,
                                        degree,
                                        word_with_punct,
                                        word_without_punct,
                                        word,
                                        root);
                                }

                                using (var sqlBulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                                {
                                    sqlBulk.SqlRowsCopied += (sender, eventArgs) => Console.WriteLine("Inserted " + eventArgs.RowsCopied + " words into the database.");
                                    sqlBulk.DestinationTableName = "Greek";
                                    sqlBulk.WriteToServer(dt);
                                }

                                transaction.Commit();
                            }
                        }
                    }
                    else Console.WriteLine("Could not open file: " + filePath);
                }
            }
            else Console.WriteLine("Could not find file: " + filePath);
        }
    }
}
