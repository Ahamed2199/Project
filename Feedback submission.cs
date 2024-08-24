using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.Office.Interop.Word;

namespace sentiment_analysis
{
    public partial class Form5 : Form
    {
        SqlConnection con = new SqlConnection("Data Source=.;Initial Catalog=sentimentanalysis;Integrated Security=True");

        public Form5()
        {
            InitializeComponent();
        }
        private string mModelPath;
        private OpenNLP.Tools.SentenceDetect.MaximumEntropySentenceDetector mSentenceDetector;
        private OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer mTokenizer;
        private OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger mPosTagger;
        private OpenNLP.Tools.Chunker.EnglishTreebankChunker mChunker;
        private OpenNLP.Tools.Parser.EnglishTreebankParser mParser;
        private OpenNLP.Tools.NameFind.EnglishNameFinder mNameFinder;


        private string[] SplitSentences(string paragraph)
        {
            if (mSentenceDetector == null)
            {
                mSentenceDetector = new OpenNLP.Tools.SentenceDetect.EnglishMaximumEntropySentenceDetector(mModelPath + "EnglishSD.nbin");
            }

            return mSentenceDetector.SentenceDetect(paragraph);
        }

        private string[] TokenizeSentence(string sentence)
        {
            if (mTokenizer == null)
            {
                mTokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer(mModelPath + "EnglishTok.nbin");
            }

            return mTokenizer.Tokenize(sentence);
        }

        private string[] PosTagTokens(string[] tokens)
        {
            if (mPosTagger == null)
            {
                mPosTagger = new OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger(mModelPath + "EnglishPOS.nbin", mModelPath + @"\Parser\tagdict");
            }

            return mPosTagger.Tag(tokens);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public List<string> good = new List<string>();
        public List<string> bad = new List<string>();
        public List<string> sat = new List<string>();

        private void Form5_Load(object sender, EventArgs e)
        {
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("select * from Stock",con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "id";
            comboBox1.ValueMember = "id";
            good = GetSynonyms("good");
            sat = GetSynonyms("satisfied");
            bad = GetSynonyms("bad");
            good.Add("satisfied");
            good.Add("satisfying");
            good.Add("happy");
            bad.Add("unsatisfactory");
            bad.Add("unsatisfied");
            
            foreach (string s in sat)
            {
                good.Add(s);
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            con.Close();
            con.Open();
            string l = comboBox1.SelectedValue.ToString();
            SqlCommand cmd = new SqlCommand("select * from Stock where id= '"+l+"'",con);
            SqlDataReader dr;
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                textBox1.Text = dr["name"].ToString();
            }
            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int pos = 0, neg = 0, avg = 0;
            if( label11.Text.ToLower() =="good" )
            {
                pos++;
            }
            else if (label11.Text.ToLower() == "bad")
            {
                neg++;
            }
            else
            {
                avg++;
            }

            if (label12.Text.ToLower() == "good")
            {
                pos++;
            }
            else if (label12.Text.ToLower() == "bad")
            {
                neg++;
            }
            else
            {
                avg++;
            }

            if (label13.Text.ToLower() == "good")
            {
                pos++;
            }
            else if (textBox5.Text.ToLower() == "bad")
            {
                neg++;
            }
            else
            {
                avg++;
            }

            if (label14.Text.ToLower() == "good")
            {
                pos++;
            }
            else if (label14.Text.ToLower() == "bad")
            {
                neg++;
            }
            else
            {
                avg++;
            }

            if (pos >= 3)
            {
                label18.Text = "Good";
                label18.BackColor = Color.LightGreen;
            }
            else if (avg >= 3)
            {
                label18.Text = "Average";
                label18.BackColor = Color.Yellow;
 
            }
            else if (neg >= 3)
            {
                label18.Text = "Bad";
                label18.BackColor = Color.OrangeRed;
            }
            else if (pos == 2 )
            {
                label18.Text = "Average";
                label18.BackColor = Color.Yellow; 
            }
            else if (neg == 2 && avg == 2)
            {
                label18.Text = "Average";
                label18.BackColor = Color.Yellow;

            }
            else
            {
                label18.Text = "Bad";
                label18.BackColor = Color.OrangeRed;
 
            }
            string txt7;
            if(textBox7.Text =="")
            {
                txt7 = "No Suggestions";
            }
            else
            {
                txt7 = textBox7.Text;
            }
            con.Close();
            con.Open();
            SqlCommand cmd = new SqlCommand("insert into feedback values('"+comboBox1.SelectedValue.ToString()+"','"+textBox1.Text+"','"+label11.Text+"','"+label12.Text+"','"+label13.Text+"','"+label14.Text+"','"+txt7+"','"+label18.Text+"')",con);
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Feedback Entered");

         }
        private List<string> GetSynonyms(string SearchText)
        {

            List<string> searchList = new List<string>();
            ApplicationClass wordApp = new ApplicationClass();
            // WriteLogger("Inside GetSynonyms");
            try
            {

                object languageID = Microsoft.Office.Interop.Word.WdLanguageID.wdEnglishUK;
                //Get all Synonyms of given word
                SynonymInfo synInfo = wordApp.get_SynonymInfo(SearchText, ref languageID);
                Array objSynInfo = synInfo.MeaningList as Array;
                //Add all synonyms to search list
                // MessageBox.Show(objSynInfo.Length + " Synonyms Found");
                for (int synCount = 1; synCount <= objSynInfo.Length; synCount++)
                {
                    searchList.Add(objSynInfo.GetValue(synCount).ToString());
                }
            }
            catch
            {
                MessageBox.Show("Error while getting synonyms");
            }
            return searchList;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                fSpellCheck(textBox2, label1);
                fGrammerCheck(textBox2, label1);
                int pos = 0, neg = 0, avg = 0;
                StringBuilder output = new StringBuilder();
                string p = textBox2.Text.ToLower();
                for (int i = 0; i < good.Count; i++)
                {
                    p = p.Replace(good[i].ToString(), "good");
                }
                for (int j = 0; j < bad.Count; j++)
                {
                    p = p.Replace(bad[j].ToString(), "bad");
                }

                string[] sentences = SplitSentences(p);

                foreach (string sentence in sentences)
                {
                    string[] tokens = TokenizeSentence(sentence);
                    string[] tags = PosTagTokens(tokens);
                    for (int j = 0; j < tags.Length; j++)
                    {
                        if (tags[j] == "RBR" || tags[j] == "RBS" || tags[j] == "RB")
                        {
                            if (tokens[j].ToLower() == "never")
                            {
                                neg++;
                            }
                            if (tokens[j].ToLower() == "well" && tokens[j].ToLower() == "not")
                            {
                                neg++;
                            }
                            else
                                if (tokens[j].ToLower() == "well")
                                {
                                    pos++;
                                }
                            if (tokens[j].ToLower() == "not" && tokens[j + 1].ToLower() != "good" && tokens[j + 1].ToLower() != "bad")
                            {
                                neg++;
                            }
                            else
                                if (tokens[j].ToLower() == "not" && tokens[j + 1].ToLower() != "bad")
                                {
                                    neg++;
                                }
                            if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() == "not")
                            {
                                avg++;

                            }



                        }
                        if (tags[j] == "JJ")
                        {
                            try
                            {
                                if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() == "not")
                                {
                                    avg++;

                                }

                                if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() != "not")
                                {
                                    neg++;
                                }

                                if (tokens[j].ToLower() == "good" && tokens[j - 1].ToLower() == "not")
                                {
                                    neg++;
                                }

                                if (tokens[j].ToLower() == "good" && tokens[j - 1].ToLower() != "not")
                                {
                                    pos++;
                                }
                                if (tokens[j] == "excellent")
                                {
                                    pos++;

                                }
                            }
                            catch (Exception d)
                            {

                                if (d.Message == "Index was outside the bounds of the array.")
                                {
                                    if (tokens[j].ToLower() == "good")
                                    {
                                        pos++;
                                    }
                                    if (tokens[j].ToLower() == "bad")
                                    {
                                        neg++;
                                    }
                                }
                            }
                        }

                        if (tags[j] == "NN" || tags[j] == "NNP")
                        {
                            if (tokens[j].ToLower() == "nothing")
                            {
                                neg++;
                            }
                        }
                        if (tags[j] == "NNP")
                        {

                            if (tokens[j].ToLower() == "bad")
                            {
                                neg++;
                            }
                        }

                    }

                    for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                    {
                        output.Append(tokens[currentTag]).Append("/").Append(tags[currentTag]).Append(" ");
                    }
                    output.Append("\r\n\r\n");
                    // label1.Text = pos.ToString() + "  " + avg.ToString() + "  " + neg.ToString();
                }
                if (pos > avg & pos > neg)
                {
                    label11.Text = "Good";
                }
                else if (avg > neg & avg > pos)
                {
                    label11.Text = "Average";
                }
                else if (neg > pos & neg > pos)
                {
                    label11.Text = "Bad";

                }
                else if (pos != 0 & pos == neg)
                {
                    label11.Text = "Average";
                }
                else if (pos != 0 & pos == avg)
                {
                    label11.Text = "Good";
                }
                else
                {
                    label11.Text = "Average";
                }
                if (label11.Text == "Good")
                {
                    label11.BackColor = Color.LightGreen;
                }
                if (label11.Text == "Average")
                {
                    label11.BackColor = Color.Yellow;
                }
                if (label11.Text == "Bad")
                {
                    label11.BackColor = Color.OrangeRed;
                }
            }
        }
        public void fSpellCheck(TextBox tBox, Label lLbl)
        {
            int iErrorCount = 0;
            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            if (tBox.Text.Length > 0)
            {
                app.Visible = false;
                // Setting these variables is comparable to passing null to the function. 
                // This is necessary because the C# null cannot be passed by reference.
                object template = Missing.Value;
                object newTemplate = Missing.Value;
                object documentType = Missing.Value;
                object visible = true;
                object optional = Missing.Value;

                _Document doc = app.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
                doc.Words.First.InsertBefore(tBox.Text);
                Microsoft.Office.Interop.Word.ProofreadingErrors we = doc.SpellingErrors;
                iErrorCount = we.Count;


                doc.CheckSpelling(ref optional, ref optional, ref optional, ref optional,
                    ref optional, ref optional, ref optional,
                    ref optional, ref optional, ref optional, ref optional, ref optional);

                if (iErrorCount == 0)
                    lLbl.Text = "Spelling OK. No errors corrected ";
                else if (iErrorCount == 1)
                    lLbl.Text = "Spelling OK. 1 error corrected ";
                else
                    lLbl.Text = "Spelling OK. " + iErrorCount + " errors corrected ";
                object first = 0;
                object last = doc.Characters.Count - 1;

                tBox.Text = doc.Range(ref first, ref last).Text;
            }
            else
                lLbl.Text = "Textbox is empty";

            object saveChanges = false;
            object originalFormat = Missing.Value;
            object routeDocument = Missing.Value;
            app.Quit(ref saveChanges, ref originalFormat, ref routeDocument);
        }

        public void fGrammerCheck(TextBox tBox, Label lLbl)
        {
            int iErrorCount = 0;
            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            if (tBox.Text.Length > 0)
            {
                app.Visible = false;
                // Setting these variables is comparable to passing null to the function. 
                // This is necessary because the C# null cannot be passed by reference.
                object template = Missing.Value;
                object newTemplate = Missing.Value;
                object documentType = Missing.Value;
                object visible = true;
                object optional = Missing.Value;

                _Document doc = app.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
                doc.Words.First.InsertBefore(tBox.Text);
                Microsoft.Office.Interop.Word.ProofreadingErrors we = doc.SpellingErrors;
                iErrorCount = we.Count;

                doc.CheckGrammar();

                //if (iErrorCount == 0)
                //    lLbl.Text = "Spelling OK. No errors corrected ";
                //else if (iErrorCount == 1)
                //    lLbl.Text = "Spelling OK. 1 error corrected ";
                //else
                //    lLbl.Text = "Spelling OK. " + iErrorCount + " errors corrected ";
                object first = 0;
                object last = doc.Characters.Count - 1;

                tBox.Text = doc.Range(ref first, ref last).Text;
            }
            else
                lLbl.Text = "Textbox is empty";

            object saveChanges = false;
            object originalFormat = Missing.Value;
            object routeDocument = Missing.Value;
            app.Quit(ref saveChanges, ref originalFormat, ref routeDocument);
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                fSpellCheck(textBox4, label1);
                fGrammerCheck(textBox4, label1);
                int pos = 0, neg = 0, avg = 0;
                StringBuilder output = new StringBuilder();
                string p = textBox4.Text.ToLower();
                for (int i = 0; i < good.Count; i++)
                {
                    p = p.Replace(good[i].ToString(), "good");
                }
                for (int j = 0; j < bad.Count; j++)
                {
                    p = p.Replace(bad[j].ToString(), "bad");
                }
                string[] sentences = SplitSentences(p);

                foreach (string sentence in sentences)
                {
                    string[] tokens = TokenizeSentence(sentence);
                    string[] tags = PosTagTokens(tokens);
                    for (int j = 0; j < tags.Length; j++)
                    {
                        if (tags[j] == "RBR" || tags[j] == "RBS" || tags[j] == "RB")
                        {
                            if (tokens[j].ToLower() == "never")
                            {
                                neg++;
                            }
                            if (tokens[j].ToLower() == "well" && tokens[j].ToLower() == "not")
                            {
                                neg++;
                            }
                            else
                                if (tokens[j].ToLower() == "well")
                                {
                                    pos++;
                                }
                            if (tokens[j].ToLower() == "not" && tokens[j + 1].ToLower() != "good")
                            {
                                neg++;
                            }
                            else
                                if (tokens[j].ToLower() == "not" && tokens[j + 1].ToLower() != "bad")
                                {
                                    neg++;
                                }

                        }
                        if (tags[j] == "JJ")
                        {
                            try
                            {
                                if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() == "not")
                                {
                                    avg++;

                                }

                                if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() != "not")
                                {
                                    neg++;
                                }

                                if (tokens[j].ToLower() == "good" && tokens[j - 1].ToLower() == "not")
                                {
                                    neg++;
                                }

                                if (tokens[j].ToLower() == "good" && tokens[j - 1].ToLower() != "not")
                                {
                                    pos++;
                                }
                                if (tokens[j].ToLower() == "excellent")
                                {
                                    pos++;

                                }
                            }
                            catch (Exception d)
                            {

                                if (d.Message == "Index was outside the bounds of the array.")
                                {
                                    if (tokens[j].ToLower() == "good")
                                    {
                                        pos++;
                                    }
                                    if (tokens[j].ToLower() == "bad")
                                    {
                                        neg++;
                                    }
                                }
                            }
                        }

                        if (tags[j] == "NN" || tags[j] == "NNP")
                        {
                            if (tokens[j].ToLower() == "nothing")
                            {
                                neg++;
                            }
                        }
                        if (tags[j] == "NNP")
                        {

                            if (tokens[j].ToLower() == "bad")
                            {
                                neg++;
                            }
                        }

                    }

                    for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                    {
                        output.Append(tokens[currentTag]).Append("/").Append(tags[currentTag]).Append(" ");
                    }
                    output.Append("\r\n\r\n");
                    // label1.Text = pos.ToString() + "  " + avg.ToString() + "  " + neg.ToString();
                }
                if (pos > avg & pos > neg)
                {
                    label12.Text = "Good";
                }
                else if (avg > neg & avg > pos)
                {
                    label12.Text = "Average";
                }
                else if (neg > pos & neg > pos)
                {
                    label12.Text = "Bad";

                }
                else if (pos != 0 & pos == neg)
                {
                    label12.Text = "Average";
                }
                else if (pos != 0 & pos == avg)
                {
                    label12.Text = "Good";
                }
                else
                {
                    label12.Text = "Average";
                }
                if (label12.Text == "Good")
                {
                    label12.BackColor = Color.LightGreen;
                }
                if (label12.Text == "Average")
                {
                    label12.BackColor = Color.Yellow;
                }
                if (label12.Text == "Bad")
                {
                    label12.BackColor = Color.OrangeRed;
                }
            }
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                fSpellCheck(textBox5, label1);
                fGrammerCheck(textBox5, label1);
                int pos = 0, neg = 0, avg = 0;
                StringBuilder output = new StringBuilder();

                string p = textBox5.Text.ToLower();
                for (int i = 0; i < good.Count; i++)
                {
                    p = p.Replace(good[i].ToString(), "good");
                }
                for (int j = 0; j < bad.Count; j++)
                {
                    p = p.Replace(bad[j].ToString(), "bad");
                }
                string[] sentences = SplitSentences(p);

                foreach (string sentence in sentences)
                {
                    string[] tokens = TokenizeSentence(sentence);
                    string[] tags = PosTagTokens(tokens);
                    for (int j = 0; j < tags.Length; j++)
                    {
                        if (tags[j] == "RBR" || tags[j] == "RBS" || tags[j] == "RB")
                        {
                            if (tokens[j].ToLower() == "never")
                            {
                                neg++;
                            }
                            if (tokens[j].ToLower() == "well" && tokens[j].ToLower() == "not")
                            {
                                neg++;
                            }
                            else
                                if (tokens[j].ToLower() == "well")
                                {
                                    pos++;
                                }
                            if (tokens[j].ToLower() == "not" && tokens[j + 1].ToLower() != "good")
                            {
                                neg++;
                            }
                            else
                                if (tokens[j].ToLower() == "not" && tokens[j + 1].ToLower() != "bad")
                                {
                                    neg++;
                                }

                        }
                        if (tags[j] == "JJ")
                        {
                            try
                            {
                                if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() == "not")
                                {
                                    avg++;

                                }

                                if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() != "not")
                                {
                                    neg++;
                                }

                                if (tokens[j].ToLower() == "good" && tokens[j - 1] == "not")
                                {
                                    neg++;
                                }

                                if (tokens[j].ToLower() == "good" && tokens[j - 1] != "not")
                                {
                                    pos++;
                                }
                                if (tokens[j] == "excellent")
                                {
                                    pos++;

                                }
                            }
                            catch (Exception d)
                            {

                                if (d.Message == "Index was outside the bounds of the array.")
                                {
                                    if (tokens[j].ToLower() == "good")
                                    {
                                        pos++;
                                    }
                                    if (tokens[j].ToLower() == "bad")
                                    {
                                        neg++;
                                    }
                                }
                            }
                        }

                        if (tags[j] == "NN" || tags[j] == "NNP")
                        {
                            if (tokens[j].ToLower() == "nothing")
                            {
                                neg++;
                            }
                        }
                        if (tags[j] == "NNP")
                        {

                            if (tokens[j].ToLower() == "bad")
                            {
                                neg++;
                            }
                        }

                    }

                    for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                    {
                        output.Append(tokens[currentTag]).Append("/").Append(tags[currentTag]).Append(" ");
                    }
                    output.Append("\r\n\r\n");
                    // label1.Text = pos.ToString() + "  " + avg.ToString() + "  " + neg.ToString();
                }
                if (pos > avg & pos > neg)
                {
                    label13.Text = "Good";
                }
                else if (avg > neg & avg > pos)
                {
                    label13.Text = "Average";
                }
                else if (neg > pos & neg > pos)
                {
                    label13.Text = "Bad";

                }
                else if (pos != 0 & pos == neg)
                {
                    label13.Text = "Average";
                }
                else if (pos != 0 & pos == avg)
                {
                    label13.Text = "Good";
                }
                else
                {
                    label13.Text = "Average";
                }
                if (label13.Text == "Good")
                {
                    label13.BackColor = Color.LightGreen;
                }
                if (label13.Text == "Average")
                {
                    label13.BackColor = Color.Yellow;
                }
                if (label13.Text == "Bad")
                {
                    label13.BackColor = Color.OrangeRed;
                }
            }
        }

        private void textBox6_Leave(object sender, EventArgs e)
        {
            if (textBox6.Text != "")
            {
                fSpellCheck(textBox6, label1);
                fGrammerCheck(textBox6, label1);
                int pos = 0, neg = 0, avg = 0;
                StringBuilder output = new StringBuilder();

                string p = textBox6.Text.ToLower();
                for (int i = 0; i < good.Count; i++)
                {
                    p = p.Replace(good[i].ToString(), "good");
                }
                for (int j = 0; j < bad.Count; j++)
                {
                    p = p.Replace(bad[j].ToString(), "bad");
                }
                string[] sentences = SplitSentences(p);

                foreach (string sentence in sentences)
                {
                    string[] tokens = TokenizeSentence(sentence);
                    string[] tags = PosTagTokens(tokens);
                    for (int j = 0; j < tags.Length; j++)
                    {
                        if (tags[j] == "RBR" || tags[j] == "RBS" || tags[j] == "RB")
                        {
                            if (tokens[j].ToLower() == "never")
                            {
                                neg++;
                            }
                            if (tokens[j].ToLower() == "well" && tokens[j].ToLower() == "not")
                            {
                                neg++;
                            }
                            else
                                if (tokens[j].ToLower() == "well")
                                {
                                    pos++;
                                }
                            if (tokens[j].ToLower() == "not" && tokens[j + 1].ToLower() != "good")
                            {
                                neg++;
                            }
                            else
                                if (tokens[j].ToLower() == "not" && tokens[j + 1].ToLower() != "bad")
                                {
                                    neg++;
                                }

                        }
                        if (tags[j] == "JJ")
                        {
                            try
                            {
                                if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() == "not")
                                {
                                    avg++;

                                }

                                if (tokens[j].ToLower() == "bad" && tokens[j - 1].ToLower() != "not")
                                {
                                    neg++;
                                }

                                if (tokens[j].ToLower() == "good" && tokens[j - 1] == "not")
                                {
                                    neg++;
                                }

                                if (tokens[j].ToLower() == "good" && tokens[j - 1] != "not")
                                {
                                    pos++;
                                }
                                if (tokens[j] == "excellent")
                                {
                                    pos++;

                                }
                            }
                            catch (Exception d)
                            {

                                if (d.Message == "Index was outside the bounds of the array.")
                                {
                                    if (tokens[j].ToLower() == "good")
                                    {
                                        pos++;
                                    }
                                    if (tokens[j].ToLower() == "bad")
                                    {
                                        neg++;
                                    }
                                }
                            }
                        }

                        if (tags[j] == "NN" || tags[j] == "NNP")
                        {
                            if (tokens[j].ToLower() == "nothing")
                            {
                                neg++;
                            }
                        }
                        if (tags[j] == "NNP")
                        {

                            if (tokens[j].ToLower() == "bad")
                            {
                                neg++;
                            }
                        }

                    }

                    for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                    {
                        output.Append(tokens[currentTag]).Append("/").Append(tags[currentTag]).Append(" ");
                    }
                    output.Append("\r\n\r\n");
                    // label1.Text = pos.ToString() + "  " + avg.ToString() + "  " + neg.ToString();
                }
                if (pos > avg & pos > neg)
                {
                    label14.Text = "Good";
                }
                else if (avg > neg & avg > pos)
                {
                    label14.Text = "Average";
                }
                else if (neg > pos & neg > pos)
                {
                    label14.Text = "Bad";

                }
                else if (pos != 0 & pos == neg)
                {
                    label14.Text = "Average";
                }
                else if (pos != 0 & pos == avg)
                {
                    label14.Text = "Good";
                }
                else
                {
                    label14.Text = "Average";
                }
                if (label14.Text == "Good")
                {
                    label14.BackColor = Color.LightGreen;
                }
                if (label14.Text == "Average")
                {
                    label14.BackColor = Color.Yellow;
                }
                if (label14.Text == "Bad")
                {
                    label14.BackColor = Color.OrangeRed;
                }
            }
        }

        private void textBox7_Leave(object sender, EventArgs e)
        {
            fSpellCheck(textBox7, label1);
            fGrammerCheck(textBox7, label1);
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
           
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form7 f7 = new Form7();
            f7.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = textBox4.Text = textBox5.Text = textBox6.Text = textBox7.Text = "";
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (char.IsNumber(e.KeyChar));
        }

        private void Form5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (char.IsNumber(e.KeyChar));
        }
    }
}
