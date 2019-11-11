using System.Data;

namespace etl
{
    internal static class DataTableUtility
    {
        internal static DataTable GreekColumns
        {
            get
            {
                var dt = new DataTable();

                dt.Columns.Add("book_name");
                dt.Columns.Add("short_name");
                dt.Columns.Add("canon_order");
                dt.Columns.Add("chapter");
                dt.Columns.Add("verse");
                dt.Columns.Add("sentence_position");
                dt.Columns.Add("book_position");
                dt.Columns.Add("adjective");
                dt.Columns.Add("conjunction");
                dt.Columns.Add("adverb");
                dt.Columns.Add("interjection");
                dt.Columns.Add("noun");
                dt.Columns.Add("preposition");
                dt.Columns.Add("article");
                dt.Columns.Add("demons_pronoun");
                dt.Columns.Add("indef_pronoun");
                dt.Columns.Add("person_pronoun");
                dt.Columns.Add("relative_pronoun");
                dt.Columns.Add("verb");
                dt.Columns.Add("particle");
                dt.Columns.Add("person");
                dt.Columns.Add("tense");
                dt.Columns.Add("voice");
                dt.Columns.Add("mood");
                dt.Columns.Add("noun_case");
                dt.Columns.Add("sing_or_plural");
                dt.Columns.Add("gender");
                dt.Columns.Add("degree");
                dt.Columns.Add("word_with_punct");
                dt.Columns.Add("word_without_punct");
                dt.Columns.Add("word");
                dt.Columns.Add("root");
                //dt.Columns.Add("strongs_num");
                //dt.Columns.Add("family_num");
                //dt.Columns.Add("root_num");
                //dt.Columns.Add("root_freq_nt");
                //dt.Columns.Add("word_freq_nt");
                //dt.Columns.Add("root_freq_book");
                //dt.Columns.Add("word_freq_book");

                return dt;
            }
        }
    }
}
