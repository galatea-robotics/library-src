using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;

namespace ElizaBot
{
    /// <summary>
    /// This class contains the Eliza program adapted from a Java applet.
    /// </summary>
    /// <remarks>
    /// http://users.rcn.com/mrex/samples/eliza/elizas.txt
    /// </remarks>
    public class Doctor
    {
        public Doctor(string datafile, string defaultEmptyResponse)
            : this(datafile)
        {
            this.defaultEmptyResponse = defaultEmptyResponse;
        }

        /// <summary>
        /// Constructor -- Doctor class
        /// </summary>
        public Doctor(string datafile)
        {
            StreamReader dat = new StreamReader(datafile);
            Pattern thisPatt = new Pattern();
            string line;
            bool iskeywords = false;

            // Read keyword / response sets from ELIZA.DAT
            while (!dat.EndOfStream)
            {
                line = dat.ReadLine();
                line = line.Trim();
                //line = line.ToUpper().Trim();   // cut off white space at the beginning and end

                if (!line.StartsWith("#") && line.Length != 0)
                {
                    // Not a comment or blank line
                    if (line.StartsWith("."))
                    {
                        thisPatt = new Pattern();
                        Patterns.Add(thisPatt);

                        iskeywords = true;      // reset to 1st half of set
                    }
                    else if (line.StartsWith("!"))
                        iskeywords = false;     // start getting replies

                    else
                    {
                        // store the line
                        if (iskeywords)
                        {
                            thisPatt.Keywords.Add(line);
                            if (line == "NOKEYFOUND") NoKeyPattern = thisPatt;
                        }
                        else thisPatt.Responses.Add(line);
                    }
                }
            }

            conjPairs.Add(new ConjPair(" ARE ",  " AM "));
            conjPairs.Add(new ConjPair(" AM ",    " ARE "));
            conjPairs.Add(new ConjPair(" WERE ",  " WAS "));
            conjPairs.Add(new ConjPair(" WAS ",   " WERE "));
            conjPairs.Add(new ConjPair(" YOU ",   " I "));
            conjPairs.Add(new ConjPair(" I ",     " YOU "));
            conjPairs.Add(new ConjPair(" YOUR ",  " MY "));
            conjPairs.Add(new ConjPair(" MY ",    " YOUR "));
            conjPairs.Add(new ConjPair(" IVE ",   " YOU'VE "));
            conjPairs.Add(new ConjPair(" YOUVE ", " I'VE "));
            conjPairs.Add(new ConjPair(" IM ",    " YOU'RE "));
            conjPairs.Add(new ConjPair(" ME ",    " YOU "));
            conjPairs.Add(new ConjPair(" US ",    " YOU "));
            conjPairs.Add(new ConjPair(" WE ",    " YOU "));
            conjPairs.Add(new ConjPair("YOURSELF", "MYSELF"));
            conjPairs.Add(new ConjPair("MYSELF",   "YOURSELF"));
        }

        /// <summary>
        /// takes in a question, searches the response sets, and builds a Response.
        /// </summary>
        /// <param name="question">The user input text</param>
        /// <returns>the response to the user</returns>
        public string Ask(string question)
        {
            question = question.ToUpper().Trim();
            cleanedQ = "";

            /*
             * Massage the string into usable form -- strip all but spaces,
             * letters, numbers; pad with beginning & ending spaces; convert
             * to upper case.
             */
            Char curChar;
            for (int i = 0; i < question.Length; i++)
            {
                curChar = question.Substring(i, 1).ToCharArray()[0]; 
                    //question.ToCharArray(i, 0)[0];
                if (Char.IsLetterOrDigit(curChar) || curChar.ToString() == " ")
                    cleanedQ += curChar;
            }
            cleanedQ = " " + cleanedQ + " ";

            // check for duplicate questions
            if (cleanedQ == previousQ)
            {
                repeatCount += 1;
                if (repeatCount < 2)
                {
                    if (new Random().Next(0, 1) == 1)
                        return "Please don't repeat yourself.";
                    else
                        return "You already said that.";
                }
                else return "Stop saying that!";
            }
            else
            {
                // reset duplicate question mode
                repeatCount = 0;
                previousQ = cleanedQ;
            }

            /*
             * Find keyword in question & save remainder.  If no keyword is
             * found, then use the keyword NOKEYFOUND
             */
            ArrayList recoPatterns = new ArrayList();
            ArrayList recoKeywords = new ArrayList();

            foreach (Pattern patt in Patterns)
            {
                foreach (string kw in patt.Keywords)
                {
                    if (cleanedQ.Contains(" " + kw + " "))
                    {
                        recoPatterns.Add(patt);
                        recoKeywords.Add(kw);
                        break;
                    }
                }
            }

            // Return a NoKeyFound response
            if (recoPatterns.Count == 0)
            {
                if (string.IsNullOrEmpty(this.defaultEmptyResponse))
                    return GetResponse(NoKeyPattern);
                else
                    return defaultEmptyResponse;
            }

            /*
             * If more than one pattern recognized, randomly select a pattern 
             * and then respond to it.
             */
            Pattern respPattern;
            string respKeyword;
            string response;
            string remainder = "";

            int rand = new Random().Next(0, recoPatterns.Count - 1);
            respPattern = recoPatterns[rand] as Pattern;
            respKeyword = recoKeywords[rand].ToString();

            response = GetResponse(respPattern);
            remainder = GetRemainder(respKeyword, cleanedQ);

            // Check to see if we need to add a question mark at the end
            bool addQMark = false;

            if (response.EndsWith("*"))
            {
                addQMark = true;

                // Add remainder
                if (remainder.Length > 0 && remainder != " ")
                {
                    /*
                     * Conjugate the subjects in the remainder of the 
                     * sentance.  This involves replacing subjects with 
                     * replacments from the conjugation list.
                     */
                    for (int i = 0; i < conjPairs.Count - 1; i++)
                    {
                        ConjPair conjpair = conjPairs[i] as ConjPair;
                        if (remainder.Contains(" " + conjpair.Subject + " "))
                            remainder = remainder.Replace(" " + conjpair.Subject + " ", " " + conjpair.Replace + " ");
                    } 

                    // Add the remainder back into the answer
                    response = response.Replace("*", " " + remainder);

                    // Add a question mark if need be
                    if (addQMark)
                    {
                        if (response.EndsWith(" ")) response = response.Trim();
                        response += "?";
                    }
                }
            }

            // return the answer
            return response;
        }

        string GetResponse(Pattern pattern)
        {
            return pattern.Responses[new Random().Next(0, pattern.Responses.Count - 1)].ToString();
        }

        string GetRemainder(string keyword, string question)
        {
            int start = question.IndexOf(keyword);
            int pos = start + keyword.Length;
            return question.Substring(pos);
        }

        private string defaultEmptyResponse;

        private int repeatCount = 0;
        private string cleanedQ;
        private string previousQ;
        private ArrayList conjPairs = new ArrayList();

        #region Properties
        private ArrayList Patterns = new ArrayList();
        private Pattern NoKeyPattern;
        #endregion

        #region Classes
        class ConjPair
        {
            public ConjPair(string subject, string replace)
            {
                this.Subject = subject.Trim();
                this.Replace = replace.Trim();
            }
            public string Subject;
            public string Replace;
        }

        class Pattern
        {
            public ArrayList Keywords = new ArrayList();
            public ArrayList Responses = new ArrayList();
        }
        #endregion
    }
}