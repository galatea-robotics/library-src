//************************************************************************
//
//  Eliza.java -- Copyright 1996, 2001 Mark Edwards/Computer Masters, Inc.
//
//  Here's the classic Eliza program, migrated into Java.  The code
// is derived from Frederick B. Maxwell, Jr.'s version 2.0 for gwbasic.
//
//************************************************************************

import java.applet.Applet;
import java.awt.*;
import java.io.*;
import java.net.*;
import java.util.*;

public class Eliza extends Applet {
    Label       plabel;
    TextField   questions;
    Button      submit;
    Label       dlabel;
    TextArea    answers;
    Responder   doctor;

        // These constants fix a layout problem with labels placed above text fields
        // in a GridBagLayout.  Text Areas must indent to match some minimum padding
        // on Labels.

    int         MAX         = 8;
    int         MIN         = 4;
    Insets      TFINSET     = new Insets( MIN, MAX, MIN, MAX );
    Insets      LINSET      = new Insets( MAX, 1, MAX, MIN );
    Insets      BINSET      = new Insets( MIN, MIN, MIN, MAX );

    String      RESPNAME    = new String( "respfile" );
    String      respfile    = new String( "eliza.dat" );

        // Prevents multiple inits from Netscape

    boolean     inited      = false;


    //
    // Init -- Create patient input text box, submit button and doctor reply text box
    //
    public void init() {
        if ( !inited ) {

                // load the response file

            if ( getParameter( RESPNAME ) !=  null )
                respfile = new String( getParameter( RESPNAME ));

            try {
                doctor  = new Responder( getDocumentBase(), respfile );
            } catch ( IOException e ) {
                System.out.println( "Can't open response file [" +
                    respfile + "]" );
                e.printStackTrace();
            }

                //
                // set up the input form
                //

            GridBagConstraints c    = new GridBagConstraints();
            GridBagLayout gridbag   = new GridBagLayout();
            Font labelfont          = new Font( "Helvetica", Font.BOLD, 14 );

            setLayout( gridbag );

                // First make items fill horiz, but stack vert

            c.fill  = GridBagConstraints.HORIZONTAL;

                // create the patient's title:

            plabel  = new Label( "Type your question here:", Label.LEFT );
            plabel.setFont( labelfont );

                // give it entire first line

            c.weightx   = 0.0;
            c.insets    = LINSET;           // inset to allow borders???
            c.gridwidth = GridBagConstraints.REMAINDER;
            gridbag.setConstraints( plabel, c );
            add( plabel );

                // create the patient's questions field

            questions   = new TextField( "", 40 );

                // make it first in second line

            c.weightx   = 1.0;              // dominant item in row
            c.insets    = TFINSET;          // inset left edge to match label
            c.gridwidth = GridBagConstraints.RELATIVE;
            gridbag.setConstraints( questions, c );
            add( questions );

                // create the submit button

            submit  = new Button();
            submit.setLabel("Ask The Doctor");

                // make it last in second line

            c.weightx   = 0.0;
            c.insets    = BINSET;           // move button in from right
            c.gridwidth = GridBagConstraints.REMAINDER;
            gridbag.setConstraints( submit, c );
            add( submit );

                // create the doctor's reply label

            dlabel      = new Label( "The Doctor Says:", Label.LEFT );
            dlabel.setFont( labelfont );

                // give it entire third line

            c.weightx   = 0.0;
            c.gridwidth = GridBagConstraints.REMAINDER;
            c.insets    = LINSET;           // inset to allow borders???
            gridbag.setConstraints( dlabel, c );
            add( dlabel );

                // create the doctor's text field

            answers = new TextArea( "HELLO, WHAT IS YOUR PROBLEM?\n", 5, 60 );
            answers.setEditable( false );

                // give it remaining area

            c.fill      = GridBagConstraints.BOTH;
            c.weightx   = 0.0;
            c.weighty   = 1.0;              // dominant item in column
            c.insets    = TFINSET;          // inset left edge to match label
            c.gridwidth = GridBagConstraints.REMAINDER;
            gridbag.setConstraints( answers, c );
            add( answers );

                // set the focus

            questions.requestFocus();

                // stop further inits from Netscape

            inited  = true;
        }
    }


    //
    // main -- This method handles running the program from the interpreter
    //
    public static void main( String args[] ) {
        Frame f         = new Frame( "Eliza Applet/Application" );
        Eliza session   = new Eliza();

        session.init();

        f.add( "Center", session );
        f.pack();
        f.show();
    }


    //
    // paint -- redraws the applet
    //
    public void paint( Graphics g ) {

            // just draw a rect around the panel

        Dimension d = size();
        g.drawRect( 0, 0, d.width - 1, d.height - 1 );
    }


    //
    // handleEvent -- Take a user's question, display the doctor's reply.
    //
    public boolean handleEvent( Event e ) {
        String  instring, outstring;

        if ((( e.target instanceof TextField ) || ( e.target instanceof Button )) &&
                                ( e.id == Event.ACTION_EVENT )) {
            instring    = questions.getText();
            outstring   = new String();

            if ( instring.length() > 0 ) {
                answers.appendText( ">" + instring + "\n" );
                answers.appendText( doctor.askquestion( instring, outstring ) + "\n" );
                questions.setText( "" );
            }

            if ( e.target instanceof Button ) {
                questions.requestFocus();
            }
        }

        return false;
    }


    //************************************************************************
    //
    //  Responder -- This class holds the migrated Eliza program.  This is
    // really just a straight translation -- I am sure that a much better
    // class could be designed from scratch...
    //
    //************************************************************************
    class Responder {

            // This needs a better data structure!  Should use a linked
            // list for the replies/responses

        String[] replies    = new String[ 300 ];    // up to 300 responses.
        String[] keywords   = new String[ 200 ];    // up to 200 keywords.
        String  previous    = new String();         // previous question
        int keyword;                                // index into keyword array
        int numkeys;                                // total number of keys read in
        int maxkey          = 0;                    // number of keywords
        int minreply        = 0;                    // first reply for current keyword.
        int maxreply        = 0;                    // last reply for current keyword
        int[] first         = new int[ 200 ];       // first reply for keyword number in subscript.
        int[] last          = new int[ 200 ];       // last reply   "     "      "     "     "    .
        int[] offset        = new int[ 200 ];       // offset from first reply for each keyword.

            //
            //  This class holds the conjugtion replacement text.  Note that the
            // replacement value has a plus sign appended to prevent recursion.
            //

        String[] conjPairs  =
        {
            " ARE ",        " AM+ ",
            " AM ",         " ARE+ ",
            " WERE ",       " WAS+ ",
            " WAS ",        " WERE+ ",
            " YOU ",        " I+ ",
            " I ",          " YOU+ ",
            " YOUR ",       " MY+ ",
            " MY ",         " YOUR+ ",
            " IVE ",        " YOUVE+ ",
            " YOUVE ",      " IVE+ ",
            " IM ",         " YOURE+ ",
            " ME ",         " YOU+ ",
            " US ",         " YOU+ ",
            " WE ",         " YOU+ ",
            " YOURSELF ",   " MYSELF+ ",
            " MYSELF ",     " YOURSELF+ "
        };
        int conjElements    = 32;


        String  trace       = new String();


        //
        //  Constructor -- read in keyword/response sets from ELIZA.DAT
        //
        Responder( URL baseURL, String respfile ) throws IOException {
            InputStream     file;
            DataInputStream dis;
            String          line;
            int             numkeys;
            boolean         isreply = false;

            file    = ( new URL( baseURL, respfile )).openStream();
            dis     = new DataInputStream( file );

            numkeys = 0;
            while (( line = dis.readLine()) != null ) {
                line = line.trim();             // cut off white space at the beginning and end
                if ( !( line.startsWith( "//" )) && ( line.length() != 0 )) {

                        // Not a comment or blank line

                    if ( line.startsWith( "!" )) {              // 2nd half of set
                        isreply = true;                         // start getting replies
                    } else if ( line.startsWith( "." )) {       // done this set

                        isreply = false;                        // reset to 1st half of set

                            // count & store indicies to this set

                        for ( keyword = ( maxkey - numkeys ); keyword < maxkey; keyword++ ) {
                            first[ keyword ]    = minreply;
                            last[ keyword ]     = maxreply - 1;
                        }
                        numkeys     = 0;                        // reset new key count
                        minreply    = maxreply;                 // point at next slot
                    } else {                                    // store the line

                        if ( isreply ) {
                            replies[ maxreply++ ]   = line;
                        } else {
                            keywords[ maxkey++ ]    = " " + line + " ";
                            numkeys++;
                        }
                    }
                }
            }
        }


        //
        // askquestion -- takes in a question, searches the response sets and
        // builds a reply.  This method is really migrated from BASIC.
        //
        public String askquestion( String question, String answer ) {
            String temp         = new String();
            String remains      = new String();     // remainder of question
            String NOKEYFOUND   = new String( " NOKEYFOUND " );
            int pos;
            int i;
            Character curchar;
            int test;
            boolean addQMark    = false;

                // Massage the string into usable form -- strip all but spaces,
                // letters, numbers; pad with beginning & ending spaces; convert
                // to upper case

            question    = question.toUpperCase();
            for ( i = 0; i < question.length(); ++i ) {
                curchar = new Character( question.charAt( i ));
                test    = curchar.charValue();
                if ((( test >= 'A' ) && ( test <= 'Z' )) ||
                    (( test >= '0' ) && ( test <= '9' )) ||
                    ( test == ' ' )) {

                    temp    = temp + curchar;
                }
            }
            temp    = " " + temp + " ";

                // check for duplicate questions

            if ( temp.equals( previous )) {
                answer  = "PLEASE DON'T REPEAT YOURSELF!";
                return answer;
            } else {
                previous    = temp;
            }

                // Find keyword in question & save remainder.  If no keyword is
                // found, then use the keyword NOKEYFOUND

            for ( i = 0, pos = 0, keyword = 0; i < maxkey; i++ ) {
                if (( pos = temp.indexOf( keywords[ i ] )) != -1 ) {
                    keyword = i;
                    if (( keyword < maxkey ) &&
                        ( keywords[ keyword ] != NOKEYFOUND )) {
                        remains = temp.substring( pos - 1 + keywords[ i ].length());
                    } else {
                        remains = "";       // ???
                    }
                    break;
                }
            }

            if ( keyword == 0 ) {       // find NOKEYFOUND set
                for ( i = 0; i < maxkey; i++ ) {
                    if ( keywords[ i ].equals( NOKEYFOUND )) {
                        keyword = i;
                        break;
                    }
                }
                remains = "";
            }

                // now get reply using the keyword number

            answer  = replies[ first[ keyword ] + offset[ keyword ]];

                // Check to see if we need to add a question mark at the end

            if ( answer.endsWith( "*" )) {
                addQMark    = true;
            }

            if ( answer.endsWith( "*" ) &&
                ( remains.length() > 0 ) &&
                    ( remains != " " )) {           // add remaining text

                    // Conjugate the subjects in the remainder of the sentance.  This
                    // involves replacing subjects with replacments from the conjugation
                    // list

                for ( i = 0; i < conjElements; i += 2 ) {

                    String subj = conjPairs[ i ];
                    String repl = conjPairs[ i + 1 ];

                    pos     = remains.indexOf( subj );
                    while ( pos != -1 ) {

                        if ( pos == 0 ) {
                            if ( repl.length() + remains.substring( subj.length()).length()
                                < remains.length()) {
                                remains = repl + remains.substring( subj.length() );
                            } else {
                                remains = repl;
                            }
                        } else {
                            temp    = remains.substring( 0, pos ) + repl;
                            if ( pos + subj.length() < remains.length()) {
                                remains = temp +
                                    remains.substring( pos + subj.length());
                            } else {
                                remains = temp;
                            }
                        }

                        pos     = remains.indexOf( subj );
                    }
                }

                    // remove plus signs -- note that plus signs have to be at least
                    // 2 chars into the string

                while (( pos = remains.indexOf( "+" )) != -1 ) {
                    remains = remains.substring( 0, pos ) + remains.substring( pos + 1 );
                }

                    // Handle special cases where "I" is the last word

                if ( remains.endsWith( " I " )) {
                    remains = remains.substring( 0, remains.length() - 2 ) + "ME ";
                }

                    // Add the remainder back into the answer

                answer  = answer.substring( 0, answer.length() - 1 ) + remains;
            }

                // Add a question mark if need be

            if ( addQMark ) {
                if ( answer.endsWith( " " )) {
                    answer = answer.substring( 0, answer.length() - 1 );
                }
                answer  += "?";
            }

                // Increment reply position.  This ensures that questions with
                // the same keywords will get different replies.

            offset[ keyword ]++;                // set to next reply for this keyword
            if ( offset[ keyword ] + first[ keyword ] > last[ keyword ] ) {
                offset[ keyword ] = 0;
            }

                // Increment offsets on all other keywords that use this reply set.

            for ( i = 0; i < maxkey; i++ ) {
                if ( first[ i ] == first[ keyword ] ) {
                    offset[ i ] = offset[ keyword ];
                }
            }

            // return the answer

            return answer;
        }
    }

}
